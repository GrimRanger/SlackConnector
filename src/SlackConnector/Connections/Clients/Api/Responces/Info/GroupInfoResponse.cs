using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("groups.info")]
    internal class GroupInfoResponse : ApiResponce
    {
        public Group Group { get; set; }
    }
}
