using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("bots.info")]
    internal class BotInfoResponse : ApiResponceWithParametres
    {
        public Bot Bot { get; set; }
    }
}
