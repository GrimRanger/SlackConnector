using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("bots.info")]
    internal class BotInfoResponse : ApiResponceBase
    {
        public Bot Bot { get; set; }
    }
}
