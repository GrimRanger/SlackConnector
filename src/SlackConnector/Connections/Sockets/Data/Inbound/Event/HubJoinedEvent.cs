using System.Threading.Tasks;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Sockets.Data.Visitors;

namespace SlackConnector.Connections.Sockets.Data.Inbound.Event
{
    internal class HubJoinedEvent : BaseInboundMessage
    {
        public Channel Channel { get; set; }

        public override Task Accept(IInboundDataVisitor visitor)
        {
            return visitor.HandleHubJoinedEvent(this);
        }
    }
}
