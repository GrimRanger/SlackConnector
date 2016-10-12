using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("groups.info")]
    internal class GroupInfoResponse : ApiResponceBase
    {
        public Group Group { get; set; }
    }
}
