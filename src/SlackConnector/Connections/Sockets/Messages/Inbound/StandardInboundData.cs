using System.Threading.Tasks;

namespace SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class StandardInboundData : InboundData
    {
        public override Task Accept(IInboundDataVisitor visitor)
        {
            return null;
        }
    }
}
