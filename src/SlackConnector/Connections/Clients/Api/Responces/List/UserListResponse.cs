using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("users.list")]
    class UserListResponse : ApiResponceWithoutParametres
    {
        public User[] Members { get; set; }
    }
}
