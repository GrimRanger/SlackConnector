using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("user.info")]
    internal class UserInfoResponse : ApiResponceWithParametres
    {
        public User User { get; set; }
    }
}
