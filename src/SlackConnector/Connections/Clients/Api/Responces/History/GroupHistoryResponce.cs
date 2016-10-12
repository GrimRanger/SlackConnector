using SlackConnector.Connections.Clients.Api.Requests;

namespace SlackConnector.Connections.Clients.Api.Responces.History
{
    [RequestPath("groups.history")]
    internal class GroupHistoryResponce : ApiResponceBase
    {
        public Models.Message[] Messages { get; set; }
    }
}
