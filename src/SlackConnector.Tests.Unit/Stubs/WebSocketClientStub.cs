using System;
using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Client;
using SlackConnector.Connections.Sockets.Data.Inbound;

namespace SlackConnector.Tests.Unit.Stubs
{
    internal class WebSocketClientStub : IWebSocketClient
    {
        public bool IsAlive { get; set; }
        public int CurrentMessageId { get; set; }

        public event EventHandler<IInboundMessage> OnMessage;
        public void RaiseOnMessage(UserInboundMessage userInboundMessage)
        {
            OnMessage.Invoke(this, userInboundMessage);
        }

        public event EventHandler OnClose;
        public void RaiseOnClose()
        {
            OnClose.Invoke(this, null);
        }

        public Task Connect()
        {
            return Task.Factory.StartNew(() => { });
        }

        public global::SlackConnector.Connections.Sockets.Data.Outbound.BaseMessage SendMessage_Message { get; private set; }
        public Task SendMessage(global::SlackConnector.Connections.Sockets.Data.Outbound.BaseMessage message)
        {
            SendMessage_Message = message;
            return Task.Factory.StartNew(() => { });
        }
        
        public void Close()
        {

        }
    }
}