using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.BotHelpers;
using SlackConnector.Connections;
using SlackConnector.Connections.Clients.Api;
using SlackConnector.Connections.Clients.Api.Responces.Info;
using SlackConnector.Connections.Clients.Api.Responces.List;
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
        private IWebSocketClient _webSocketClient;

        private Dictionary<string, SlackChatHub> _connectedHubs;
        public IApiClient ApiClient => _connectionFactory.CreateApiClient();
        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs => _connectedHubs;

        private Dictionary<string, string> _userNameCache;
        public IReadOnlyDictionary<string, string> UserNameCache => _userNameCache;

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
            _userNameCache = connectionInformation.Users;
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

            if (inboundMessage.Channel != null && !_connectedHubs.ContainsKey(inboundMessage.Channel))
            {
                _connectedHubs[inboundMessage.Channel] = _chatHubInterpreter.FromId(inboundMessage.Channel);
            }

            var message = new SlackMessage
            {
                User = GetUser(inboundMessage.User).Result,
                Text = inboundMessage.Text,
                ChatHub = GetChatHub(inboundMessage.Channel).Result,
               
                RawData = inboundMessage.RawData,
                MentionsBot = _mentionDetector.WasBotMentioned(Self.Name, Self.Id, inboundMessage.Text)
            };

            await RaiseMessageReceived(message);
        }

        private async Task<SlackChatHub> GetChatHub(string channelId)
        {
            var cachedChannel = channelId == null ? null : _connectedHubs[channelId];
            if (cachedChannel == null)
            {
                var apiClient = _connectionFactory.CreateApiClient();
                var channelIdParameter = new KeyValuePair<string, string>("channel", channelId);
                var respose = await apiClient.SendRequest<ChannelInfoResponse>(SlackKey, channelIdParameter);
                var channel = respose.Channel;
                var result = new SlackChatHub
                {
                    Id = channel.Id,
                    Name = channel.Name,
                };

                return result;
            }

            return cachedChannel;
        }

        private async Task<SlackUser> GetUser(string userId)
        {
            var apiClient = _connectionFactory.CreateApiClient();

            var userIdParameter = new KeyValuePair<string, string>("user", userId);
            var respose = await apiClient.SendRequest<UserInfoResponse>(SlackKey, userIdParameter);
            var user = respose.User;
            var result = new SlackUser
            {
                Id = user.Id,
                Name = user.Name,
                Icons = user.Profile.Icons
            };

            return result;
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

        public void SendApi(string command)
        {
            //var result1 = _connectionFactory.CreateApiClient().SendRequest<ChannelListResponce>(SlackKey).Result;
            //var result2 = _connectionFactory.CreateApiClient().SendRequest<DirectMessageConversationListResponse>(SlackKey).Result;
            //var result3 = _connectionFactory.CreateApiClient().SendRequest<GroupListResponse>(SlackKey).Result;
            //var result4 = _connectionFactory.CreateApiClient().SendRequest<UserListResponse>(SlackKey).Result;

            //var result5 = _connectionFactory.CreateApiClient().SendRequest<BotInfoResponse>(SlackKey, new KeyValuePair<string, string>[1]).Result;
            //var result6 = _connectionFactory.CreateApiClient().SendRequest<ChannelInfoResponse>(SlackKey, new KeyValuePair<string, string>[1]).Result;
            //var result7 = _connectionFactory.CreateApiClient().SendRequest<GroupInfoResponse>(SlackKey, new KeyValuePair<string, string>[1]).Result;
            //var result8 = _connectionFactory.CreateApiClient().SendRequest<UserInfoResponse>(SlackKey, new KeyValuePair<string, string>[1]).Result;
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

                }
            }
        }
    }
}