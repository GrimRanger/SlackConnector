using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.BotHelpers;
using SlackConnector.Connections.ClientFactories;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Sockets.Client;
using SlackConnector.Connections.Sockets.Data.Inbound;
using SlackConnector.Connections.Sockets.Data.Inbound.Event;
using SlackConnector.Connections.Sockets.Data.Outbound;
using SlackConnector.Connections.Sockets.Data.Visitors;
using SlackConnector.DataHandlers;
using SlackConnector.EventHandlers;
using SlackConnector.Exceptions;
using SlackConnector.Models;

namespace SlackConnector
{
    internal class SlackConnection : ISlackConnection, IInboundDataHandler
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IChatHubInterpreter _chatHubInterpreter;
        private readonly IMentionDetector _mentionDetector;
        private IWebSocketClient _webSocketClient;
        private IInboundDataVisitor _inboundDataVisitor;

        private Dictionary<string, SlackChatHub> _hubCache;
        public IReadOnlyDictionary<string, SlackChatHub> HubCache => _hubCache;

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
            _hubCache = connectionInformation.SlackChatHubs;

            _webSocketClient = connectionInformation.WebSocket;
            _webSocketClient.OnClose += (sender, args) =>
            {
                ConnectedSince = null;
                RaiseOnDisconnect();
            };

            _webSocketClient.OnMessage += async (sender, message) => await ListenTo(message);

            ConnectedSince = DateTime.Now;
        }

        private async Task ListenTo(IInboundMessage baseMessage)
        {
            if (baseMessage == null || baseMessage.MessageType == MessageType.Unknown)
                return;

            _inboundDataVisitor = new InboundDataVisitor(this);
            await baseMessage.Accept(_inboundDataVisitor);
        }

        public async Task<IEnumerable<SlackMessage>> GetHistory(SlackChatHub slackChatHub, int count)
        {
            var infoClient = _connectionFactory.CreateInfoClient(_hubCache, _userCache);

            return await _connectionFactory.CreateHistoryClient(infoClient).GetChatHubHistory(SlackKey, slackChatHub, count);
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

        public event DisconnectEventHandler OnDisconnect;
        private void RaiseOnDisconnect()
        {
            OnDisconnect?.Invoke();
        }

        public void Disconnect()
        {
            if (_webSocketClient != null && _webSocketClient.IsAlive)
            {
                _webSocketClient.Close();
            }
        }

        public async Task HandleInboundData(UserInboundMessage userInboundMessage)
        {
            if (string.IsNullOrEmpty(userInboundMessage.User))
                return;

            await UpdateHubCache(userInboundMessage);

            await UpdateUserCache(userInboundMessage);

            if (!string.IsNullOrEmpty(Self.Id) && userInboundMessage.User == Self.Id)
                return;

            SlackMessage message = new SlackMessage
            {
                User = userInboundMessage.User == null ? null : _userCache[userInboundMessage.User],
                Text = userInboundMessage.Text,
                ChatHub = userInboundMessage.Channel == null ? null : _hubCache[userInboundMessage.Channel],
                Time = userInboundMessage.Time,
                RawData = userInboundMessage.RawData,
                MentionsBot = _mentionDetector.WasBotMentioned(Self.Name, Self.Id, userInboundMessage.Text)
            };
            if (message.User == null && userInboundMessage.User != null)
                message.User = new SlackUser {Id = userInboundMessage.User, Name = ""};
            if (message.ChatHub == null && userInboundMessage.Channel != null)
                message.ChatHub = new SlackChatHub { Id = userInboundMessage.Channel, Name = "" };

            if (userInboundMessage.MessageSubType == MessageSubType.bot_message)
            {
                message.User.Name = ((BotInboundMessage)userInboundMessage).UserName;
            }

            await RaiseMessageReceived(message);
        }

        public async Task HandleInboundData(HubJoinedEvent inboundEvent)
        {
            var chatHub = await GetChatHub(inboundEvent.Channel.Id);
            await RaiseChannelJoined(chatHub);
        }

        private async Task UpdateUserCache(UserInboundMessage userInboundMessage)
        {
            if (userInboundMessage.User != null && !_userCache.ContainsKey(userInboundMessage.User))
            {
                _userCache[userInboundMessage.User] = await GetUser(userInboundMessage.User);
            }
        }

        private async Task UpdateHubCache(UserInboundMessage userInboundMessage)
        {
            if (userInboundMessage.Channel != null && !_hubCache.ContainsKey(userInboundMessage.Channel))
            {
                var chatHub = await GetChatHub(userInboundMessage.Channel);
                _hubCache[userInboundMessage.Channel] = chatHub ?? _chatHubInterpreter.FromId(userInboundMessage.Channel);
            }
        }

        private async Task<SlackChatHub> GetChatHub(string chatHubid)
        {
            var infoClient = _connectionFactory.CreateInfoClient();
            SlackChatHub result = null;
            if (infoClient != null)
                result = await infoClient.GetChatHub(SlackKey, chatHubid);

            return result;
        }

        private async Task<SlackUser> GetUser(string userId)
        {
            var infoClient = _connectionFactory.CreateInfoClient();
            SlackUser result = null;
            if (infoClient != null)
                result = await infoClient.GetUser(SlackKey, userId);

            return result;
        }
    }
}