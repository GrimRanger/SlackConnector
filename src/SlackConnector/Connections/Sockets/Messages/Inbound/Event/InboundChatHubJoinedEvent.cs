using System.Threading.Tasks;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Sockets.Messages.Inbound.Event
{
    internal class InboundChatHubJoinedEvent : InboundData
    {
        public Channel Channel { get; set; }
        public override Task Accept(IInboundDataVisitor visitor)
        {
            return visitor.HandleInboundEvent(this);
        }
    }
}
