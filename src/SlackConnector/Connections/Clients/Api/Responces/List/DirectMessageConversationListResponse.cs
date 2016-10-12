using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("im.list")]
    internal class DirectMessageConversationListResponse : ApiResponceBase
    {
        public Im[] Ims { get; set; }
    }
}
