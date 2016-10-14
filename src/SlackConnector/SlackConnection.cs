using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.BotHelpers;
using SlackConnector.Connections;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Sockets;
using SlackConnector.Connections.Sockets.Messages;
using SlackConnector.Connections.Sockets.Messages.Inbound;
using SlackConnector.Connections.Sockets.Messages.Inbound.Event;
using SlackConnector.Connections.Sockets.Messages.Outbound;
using SlackConnector.EventHandlers;
using SlackConnector.Exceptions;
using SlackConnector.Models;

namespace SlackConnector
{
    internal class SlackConnection : ISlackConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IChatHubInterpreter _chatHubInterpreter;
        private readonly IMentionDetector _mentionDetector;
        private IWebSocketClient _webSocketClient;
        private IInboundDataVisitor _inboundDataVisitor;

        private Dictionary<string, SlackChatHub> _connectedHubs;
        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs => _connectedHubs;

        private Dictionary<string, SlackUser> _userCache;
        public IReadOnlyDictionary<string, SlackUser> UserCache => _userCache;

        public bool IsConnected => ConnectedSince.HasValue;
        public DateTime? ConnectedSince { get; private set; }
        public string SlackKey { get; private set; }

        public ContactDetails Team { get; private set; }
        public ContactDetails Self { get; private set; }


        public SlackConnection(IConnectionFactory connectionFactory, IChatHubInterpreter chatHubInterpreter, IMentionDetector mentionDetector)
        {
            _connectionFactory = connectionFactory;
            _chatHubInterpreter = chatHubInterpreter;
            _mentionDetector = mentionDetector;
        }

        public void Initialise(ConnectionInformation connectionInformation)
        {
            SlackKey = connectionInformation.SlackKey;
            Team = connectionInformation.Team;
            Self = connectionInformation.Self;
            _userCache = connectionInformation.Users;
            _connectedHubs = connectionInformation.SlackChatHubs;

            _webSocketClient = connectionInformation.WebSocket;
            _webSocketClient.OnClose += (sender, args) =>
            {
                ConnectedSince = null;
                RaiseOnDisconnect();
            };

            _webSocketClient.OnMessage += async (sender, message) => await ListenTo(message);

            ConnectedSince = DateTime.Now;
        }

        private async Task ListenTo(InboundData inboundData)
        {
           if (inboundData == null || inboundData.MessageType == MessageType.Unknown)
                return;

            _inboundDataVisitor = new InboundDataVisitor(this);
            await inboundData.Accept(_inboundDataVisitor);
        }

        internal async Task HandleInboundData(InboundMessage inboundMessage)
        {
            if (string.IsNullOrEmpty(inboundMessage.User))
                return;

            if (inboundMessage.Channel != null && !_connectedHubs.ContainsKey(inboundMessage.Channel))
            {
                var chatHub = await GetChatHub(inboundMessage.Channel);
                _connectedHubs[inboundMessage.Channel] = chatHub ?? _chatHubInterpreter.FromId(inboundMessage.Channel);
            }

            if (inboundMessage.User != null && !_userCache.ContainsKey(inboundMessage.User))
            {
                _userCache[inboundMessage.User] = await GetUser(inboundMessage.User);
            }

            if (!string.IsNullOrEmpty(Self.Id) && inboundMessage.User == Self.Id)
                return;

            SlackMessage message = new SlackMessage
            {
                User = inboundMessage.User == null ? null : _userCache[inboundMessage.User],
                Text = inboundMessage.Text,
                ChatHub = inboundMessage.Channel == null ? null : _connectedHubs[inboundMessage.Channel],
                Time = inboundMessage.Time,
                RawData = inboundMessage.RawData,
                MentionsBot = _mentionDetector.WasBotMentioned(Self.Name, Self.Id, inboundMessage.Text)
            };

            if (inboundMessage.MessageSubType == MessageSubType.bot_message)
            {
                message.User.Name = ((BotInboundMessage)inboundMessage).UserName;
            }

            await RaiseMessageReceived(message);
        }

        internal async Task HandleInboundData(InboundChatHubJoinedEvent inboundEvent)
        {
                var chatHub = await GetChatHub(inboundEvent.Channel.Id);
                await RaiseChannelJoined(chatHub);
        }

        private async Task<SlackChatHub> GetChatHub(string chatHubid)
        {
            var infoClient = _connectionFactory.CreateInfoClient();
            var result = await infoClient.GetChatHub(SlackKey, chatHubid);

            return result;
        }

        private async Task<SlackUser> GetUser(string userId)
        {
            var infoClient = _connectionFactory.CreateInfoClient();
            var result = await infoClient.GetUser(SlackKey, userId);

            return result;
        }

        public async Task<IEnumerable<SlackMessage>> GetHistory(SlackChatHub slackChatHub, int count)
        {
            var infoClient = _connectionFactory.CreateInfoClient(ConnectedHubs, UserCache);

            return await _connectionFactory.CreateHistoryClient(infoClient).GetChatHubHistory(SlackKey, slackChatHub, count);
        }

        public void Disconnect()
        {
            if (_webSocketClient != null && _webSocketClient.IsAlive)
            {
                _webSocketClient.Close();
            }
        }

        public async Task Say(BotMessage message)
        {
            if (string.IsNullOrEmpty(message.ChatHub?.Id))
            {
                throw new MissingChannelException("When calling the Say() method, the message parameter must have its ChatHub property set.");
            }

            var client = _connectionFactory.CreateChatClient();
            await client.PostMessage(SlackKey, message.ChatHub.Id, message.Text, message.Attachments);
        }

        //TODO: Cache newly created channel, and return if already exists
        public async Task<SlackChatHub> JoinDirectMessageChannel(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentNullException(nameof(user));
            }

            IChannelClient client = _connectionFactory.CreateChannelClient();
            Channel channel = await client.JoinDirectMessageChannel(SlackKey, user);

            return new SlackChatHub
            {
                Id = channel.Id,
                Name = channel.Name,
                Type = SlackChatHubType.DM
            };
        }

        public async Task IndicateTyping(SlackChatHub chatHub)
        {
            var message = new TypingIndicatorMessage
            {
                Channel = chatHub.Id,
                Type = "typing"
            };

            await _webSocketClient.SendMessage(message);
        }

        public event DisconnectEventHandler OnDisconnect;
        private void RaiseOnDisconnect()
        {
            OnDisconnect?.Invoke();
        }

        public event MessageReceivedEventHandler OnMessageReceived;
        private async Task RaiseMessageReceived(SlackMessage message)
        {
            if (OnMessageReceived != null)
            {
                try
                {
                    await OnMessageReceived(message);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public event ChannelJoinedEventHandler OnChannelJoined;
        public string Test(string text)
        {
            var result = new MessageInterpreter().InterpretMessage(text);

            return "";
        }

        private async Task RaiseChannelJoined(SlackChatHub chatHub)
        {
            if (OnChannelJoined != null)
            {
                try
                {
                    await OnChannelJoined(chatHub);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}