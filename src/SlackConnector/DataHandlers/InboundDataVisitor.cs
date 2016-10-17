using System.Threading.Tasks;
using SlackConnector.Connections.Sockets.Data.Inbound;
using SlackConnector.Connections.Sockets.Data.Inbound.Event;
using SlackConnector.Connections.Sockets.Data.Visitors;

namespace SlackConnector.DataHandlers
{
    internal class InboundDataVisitor : IInboundDataVisitor
    {
        private readonly IInboundDataHandler _inboundDataHandler;

        public InboundDataVisitor(IInboundDataHandler inboundDataHandler)
        {
            _inboundDataHandler = inboundDataHandler;
        }

        public async Task HandleUserMessage(UserInboundMessage userInboundData)
        {
            await _inboundDataHandler.HandleInboundData(userInboundData);
        }

        public async Task HandleHubJoinedEvent(HubJoinedEvent data)
        {
            await _inboundDataHandler.HandleInboundData(data);
        }
    }
}
