using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackConnector.BotHelpers;
using SlackConnector.Connections;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Responses;
using SlackConnector.Connections.Sockets;
using SlackConnector.Exceptions;
using SlackConnector.Models;

namespace SlackConnector
{
    public class SlackConnector : ISlackConnector
    {
        public static ConsoleLoggingLevel LoggingLevel = ConsoleLoggingLevel.FatalErrors;

        private readonly IConnectionFactory _connectionFactory;
        private readonly ISlackConnectionFactory _slackConnectionFactory;
        private readonly ICachedDataProvider _cachedDataProvider;

        public SlackConnector() : this(new ConnectionFactory(), new SlackConnectionFactory(), new CachedDataProvider())
        { }

        internal SlackConnector(IConnectionFactory connectionFactory, ISlackConnectionFactory slackConnectionFactory)
            : this(connectionFactory, slackConnectionFactory, new CachedDataProvider()) { }

        internal SlackConnector(IConnectionFactory connectionFactory, ISlackConnectionFactory slackConnectionFactory, ICachedDataProvider cachedDataProvider)
        {
            _connectionFactory = connectionFactory;
            _slackConnectionFactory = slackConnectionFactory;
            _cachedDataProvider = cachedDataProvider;
        }

        public async Task<ISlackConnection> Connect(string slackKey, ProxySettings proxySettings = null)
        {
            if (string.IsNullOrEmpty(slackKey))
            {
                throw new ArgumentNullException(nameof(slackKey));
            }

            var handshakeClient = _connectionFactory.CreateHandshakeClient();
            HandshakeResponse handshakeResponse = await handshakeClient.FirmShake(slackKey);

            if (!handshakeResponse.Ok)
            {
                throw new HandshakeException(handshakeResponse.Error);
            }

            var connectionInfo = new ConnectionInformation
            {
                SlackKey = slackKey,
                Self = new ContactDetails { Id = handshakeResponse.Self.Id, Name = handshakeResponse.Self.Name },
                Team = new ContactDetails { Id = handshakeResponse.Team.Id, Name = handshakeResponse.Team.Name },
                Users = GenerateUsers(handshakeResponse),
                SlackChatHubs = GetChatHubs(handshakeResponse),
                WebSocket = await GetWebSocket(handshakeResponse, proxySettings)
            };

            return _slackConnectionFactory.Create(connectionInfo);
        }

        private async Task<IWebSocketClient> GetWebSocket(HandshakeResponse handshakeResponse, ProxySettings proxySettings)
        {
            var webSocketClient = _connectionFactory.CreateWebSocketClient(handshakeResponse.WebSocketUrl, proxySettings);
            await webSocketClient.Connect();
            return webSocketClient;
        }

        private Dictionary<string, SlackUser> GenerateUsers(HandshakeResponse handshakeResponse)
        {
            var users = new Dictionary<string, SlackUser>();
            foreach (var user in handshakeResponse.Users)
            {
                var slackUser = new SlackUser
                {
                    Id = user.Id,
                    Name = user.Name,
                    IsBot = false,
                    Icons = user.Profile.Icons
                };
                users.Add(user.Id, slackUser);
            }

            foreach (var bot in handshakeResponse.Bots)
            {
                var slackUser = new SlackUser
                {
                    Id = bot.Id,
                    Name = bot.Name,
                    IsBot = true,
                    Icons = bot.Icons
                };
                users.Add(bot.Id, slackUser);
            }

            return users;
        }

        private Dictionary<string, SlackChatHub> GetChatHubs(HandshakeResponse handshakeResponse)
        {
            var hubs = new Dictionary<string, SlackChatHub>();

            foreach (Channel channel in handshakeResponse.Channels.Where(x => !x.IsArchived))
            {
                if (channel.IsMember)
                {
                    hubs.Add(channel.Id, _cachedDataProvider.GetChatHub(channel));
                }
            }

            foreach (Group group in handshakeResponse.Groups.Where(x => !x.IsArchived))
            {
                if (group.Members.Any(x => x == handshakeResponse.Self.Id))
                {
                    hubs.Add(group.Id, _cachedDataProvider.GetChatHub(group));
                }
            }

            foreach (Im im in handshakeResponse.Ims)
            {
                User user = handshakeResponse.Users.FirstOrDefault(x => x.Id == im.User);
                var userName = user?.Name;
                hubs.Add(im.Id, _cachedDataProvider.GetChatHub(im, userName));
            }

            return hubs;
        }
    }
}