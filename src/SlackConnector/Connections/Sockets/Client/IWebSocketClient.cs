using System;
using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Data.Inbound;

namespace SlackConnector.Connections.Sockets.Client
{
    internal interface IWebSocketClient
    {
        bool IsAlive { get; }

        Task Connect();
        Task SendMessage(Data.Outbound.BaseMessage message);
        void Close();

        event EventHandler<IInboundMessage> OnMessage;
        event EventHandler OnClose;
    }
}