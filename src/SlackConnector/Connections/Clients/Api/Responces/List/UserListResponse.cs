using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("users.list")]
    class UserListResponse : ApiResponce
    {
        public User[] Members { get; set; }
    }
}
