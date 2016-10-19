using SlackConnector.Connections.Clients.Api.Requests;

namespace SlackConnector.Connections.Clients.Api.Responces.History
{
    [RequestPath("im.history")]
    internal class DirectMessageConversationHistoryResponse : ApiResponceBase
    {
        public Models.Message[] Messages { get; set; }
    }
}
