using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackConnector.BotHelpers;
using SlackConnector.BotHelpers.Interfaces;
using SlackConnector.Connections;
using SlackConnector.Connections.Clients.Api;
using SlackConnector.Connections.Clients.Api.Responces.History;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Sockets;
using SlackConnector.Connections.Sockets.Messages.Inbound;
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
        private readonly ISlackInfoSearcher _slackInfoSearcher;
        private IWebSocketClient _webSocketClient;

        private Dictionary<string, SlackChatHub> _connectedHubs;
        public IApiClient ApiClient => _connectionFactory.CreateApiClient();
        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs => _connectedHubs;

        private Dictionary<string, SlackUser> _userCache;
        public IReadOnlyDictionary<string, SlackUser> UserCache => _userCache;

        public bool IsConnected => ConnectedSince.HasValue;
        public DateTime? ConnectedSince { get; private set; }
        public string SlackKey { get; private set; }

        public ContactDetails Team { get; private set; }
        public ContactDetails Self { get; private set; }

        public SlackConnection(IConnectionFactory connectionFactory, IChatHubInterpreter chatHubInterpreter, IMentionDetector mentionDetector, ISlackInfoSearcher slackInfoSearcher)
        {
            _connectionFactory = connectionFactory;
            _chatHubInterpreter = chatHubInterpreter;
            _mentionDetector = mentionDetector;
            _slackInfoSearcher = slackInfoSearcher;
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

        private async Task ListenTo(InboundMessage inboundMessage)
        {
            if (inboundMessage?.MessageType != MessageType.Message)
                return;
            if (string.IsNullOrEmpty(inboundMessage.User))
                return;

            await GetChatHub(inboundMessage.Channel);
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

                RawData = inboundMessage.RawData,
                MentionsBot = _mentionDetector.WasBotMentioned(Self.Name, Self.Id, inboundMessage.Text)
            };

            if (inboundMessage.MessageSubType == MessageSubType.bot_message)
            {
                message.User.Name = ((BotInboundMessage) inboundMessage).UserName;
            }
            
            await RaiseMessageReceived(message);
        }

        private async Task<SlackChatHub> GetChatHub(string chatHubid)
        {
            var apiClient = _connectionFactory.CreateApiClient();
            var result = await _slackInfoSearcher.GetChatHub(apiClient, SlackKey, chatHubid);

            return result;
        }

        private async Task<SlackUser> GetUser(string userId)
        {
            var apiClient = _connectionFactory.CreateApiClient();
            var result = await _slackInfoSearcher.GetUser(apiClient, SlackKey, userId);

            return result;
        }

        //private void GetChannelHistory()
        //{
        //    var apiClient = _connectionFactory.CreateApiClient();
        //    var result =  apiClient.SendRequest<ChannelsHistoryResponce>(SlackKey, new KeyValuePair<string, string>("channel", "C0FB8PZQQ")).Result;
        //    var first = result.Messages.First(t => t.Text == "");
        //}

        //private void Test()
        //{
        //    var text =
        //        "{\"text\":\"\",\"username\":\"MasterCommander\",\"bot_id\":\"B0C48EWRF\",\"icons\":{\"emoji\":\":commanderadama:\"},\"attachments\":[{\"fallback\":\"Master has failed\",\"text\":\"Closed - Do not merge\",\"title\":\"Master has failed\",\"id\":1,\"color\":\"d00000\"}],\"type\":\"message\",\"subtype\":\"bot_message\",\"ts\":\"1474443609.000289\"}";
        //    var interpeter = new MessageInterpreter();
        //    var result = interpeter.InterpretMessage(text);
        //    var check = ListenTo(result);
        //    result.Text = result.Text + "";
        //}

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
    }
}