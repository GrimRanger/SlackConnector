using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Messages.Inbound;
using SlackConnector.Connections.Sockets.Messages.Inbound.Event;

namespace SlackConnector.Connections.Sockets.Messages
{
    internal interface IInboundDataVisitor
    {
        Task HandleInboundMessage(InboundMessage inboundData);
        Task HandleInboundEvent(InboundChatHubJoinedEvent inboundData);
    }
}
