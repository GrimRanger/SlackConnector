using SlackConnector.Connections.Clients.Api.Requests;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("groups.list")]
    internal class GroupListResponse : ApiResponceBase
    {
        public Models.Group[] Groups { get; set; }
    }
}
