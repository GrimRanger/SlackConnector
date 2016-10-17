using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Data.Visitors;

namespace SlackConnector.Connections.Sockets.Data.Inbound
{
    internal class BaseInboundMessage : IInboundMessage
    {
        public MessageType MessageType { get; set; }
        public string RawData { get; set; }
        public MessageError Error { get; set; }

        public virtual Task Accept(IInboundDataVisitor visitor)
        {
            return null;
        }
    }
}
