using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("users.list")]
    internal class UserListResponse : ApiResponceBase
    {
        public User[] Members { get; set; }
    }
}
