using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Data.Inbound;
using SlackConnector.Connections.Sockets.Data.Inbound.Event;

namespace SlackConnector.DataHandlers
{
    internal interface IInboundDataHandler
    {
        Task HandleInboundData(UserInboundMessage userInboundMessage);
        Task HandleInboundData(HubJoinedEvent inboundEvent);
    }
}