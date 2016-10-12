using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("users.info")]
    internal class UserInfoResponse : ApiResponceBase
    {
        public User User { get; set; }
    }
}
