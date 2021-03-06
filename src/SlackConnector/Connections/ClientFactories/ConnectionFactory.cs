﻿using System.Collections.Generic;
using SlackConnector.Connections.Clients;
using SlackConnector.Connections.Clients.Api.HighLevelClient;
using SlackConnector.Connections.Clients.Api.LowLevelClient;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Clients.Chat;
using SlackConnector.Connections.Clients.Handshake;
using SlackConnector.Connections.Clients.History;
using SlackConnector.Connections.Clients.Info;
using SlackConnector.Connections.Sockets.Client;
using SlackConnector.Connections.Sockets.Data.Interpreters;
using SlackConnector.Models;

namespace SlackConnector.Connections.ClientFactories
{
    internal class ConnectionFactory : IConnectionFactory
    {
        private readonly IRequestExecutor _requestExecutor;

        public ConnectionFactory()
        {
            IRestSharpFactory restSharpFactory = new RestSharpFactory();
            IResponseVerifier responseVerifier = new ResponseVerifier();
            _requestExecutor = new RequestExecutor(restSharpFactory, responseVerifier);
        }

        public IWebSocketClient CreateWebSocketClient(string url, ProxySettings proxySettings)
        {
            return new WebSocketClient(new MessageInterpreter(), url, proxySettings);
        }

        public IHandshakeClient CreateHandshakeClient()
        {
            return new HandshakeClient(_requestExecutor);
        }

        public IChatClient CreateChatClient()
        {
            return new ChatClient(_requestExecutor);
        }

        public IChannelClient CreateChannelClient()
        {
            return new ChannelClient(_requestExecutor);
        }

        public ILowLevelApiClient CreateLowLevelApiClient()
        {
            return new LowLevelApiClient(_requestExecutor);
        }

        public IHighLevelApiClient CreateHighLevelApiClient()
        {
            return new HighLevelApiClient(CreateLowLevelApiClient());
        }

        public IInfoClient CreateInfoClient(IDictionary<string, SlackChatHub> hubCache = null, IDictionary<string, SlackUser> userCache = null)
        {
            return new InfoClient(CreateHighLevelApiClient(), hubCache, userCache);
        }

        public IHistoryClient CreateHistoryClient(IInfoClient infoClient = null)
        {
            if (infoClient == null)
                infoClient = CreateInfoClient();
            return new HistoryClient(CreateHighLevelApiClient(), infoClient);
        }
    }
}