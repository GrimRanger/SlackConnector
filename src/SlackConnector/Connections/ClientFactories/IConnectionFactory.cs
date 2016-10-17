using System.Collections.Generic;
using SlackConnector.Connections.Clients.Api.HighLevelClient;
using SlackConnector.Connections.Clients.Api.LowLevelClient;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Clients.Chat;
using SlackConnector.Connections.Clients.Handshake;
using SlackConnector.Connections.Clients.History;
using SlackConnector.Connections.Clients.Info;
using SlackConnector.Connections.Sockets;
using SlackConnector.Connections.Sockets.Client;
using SlackConnector.Models;

namespace SlackConnector.Connections.ClientFactories
{
    internal interface IConnectionFactory
    {
        IWebSocketClient CreateWebSocketClient(string url, ProxySettings proxySettings);
        IHandshakeClient CreateHandshakeClient();
        IChatClient CreateChatClient();
        IChannelClient CreateChannelClient();
        ILowLevelApiClient CreateLowLevelApiClient();
        IHighLevelApiClient CreateHighLevelApiClient();
        IInfoClient CreateInfoClient(IDictionary<string, SlackChatHub> hubCache = null, IDictionary<string, SlackUser> userCache = null);
        IHistoryClient CreateHistoryClient(IInfoClient infoClient = null);
    }
}