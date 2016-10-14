using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Messages.Inbound;
using SlackConnector.Connections.Sockets.Messages.Inbound.Event;

namespace SlackConnector.Connections.Sockets.Messages
{
    internal class InboundDataVisitor : IInboundDataVisitor
    {
        private SlackConnection slackConnection;

        public InboundDataVisitor(SlackConnection slackConnection)
        {
            this.slackConnection = slackConnection;
        }

        public async Task HandleInboundMessage(InboundMessage inboundData)
        {
            await slackConnection.HandleInboundData(inboundData);
        }

        public async Task HandleInboundEvent(InboundChatHubJoinedEvent inboundData)
        {
            await slackConnection.HandleInboundData(inboundData);
        }
    }
}
