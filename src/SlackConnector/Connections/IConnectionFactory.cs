using System.Collections.Generic;
using SlackConnector.Connections.Clients.Api.HighLevelClient;
using SlackConnector.Connections.Clients.Api.LowLevelClient;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Clients.Chat;
using SlackConnector.Connections.Clients.Handshake;
using SlackConnector.Connections.Clients.History;
using SlackConnector.Connections.Clients.Info;
using SlackConnector.Connections.Sockets;
using SlackConnector.Models;

namespace SlackConnector.Connections
{
    internal interface IConnectionFactory
    {
        IWebSocketClient CreateWebSocketClient(string url, ProxySettings proxySettings);
        IHandshakeClient CreateHandshakeClient();
        IChatClient CreateChatClient();
        IChannelClient CreateChannelClient();
        ILowLevelApiClient CreateLowLevelApiClient();
        IHighLevelApiClient CreateHighLevelApiClient();
        IInfoClient CreateInfoClient(IReadOnlyDictionary<string, SlackChatHub> connectedHubs = null, IReadOnlyDictionary<string, SlackUser> userCache = null);
        IHistoryClient CreateHistoryClient(IInfoClient infoClient = null);
    }
}