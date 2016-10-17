using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Data.Inbound;
using SlackConnector.Connections.Sockets.Data.Inbound.Event;

namespace SlackConnector.Connections.Sockets.Data.Visitors
{
    internal interface IInboundDataVisitor
    {
        Task HandleUserMessage(UserInboundMessage userInboundData);
        Task HandleHubJoinedEvent(HubJoinedEvent data);
    }
}
