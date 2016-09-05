using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
    [RequestPath("bots.info")]
    internal class BotInfoResponse : ApiResponce
    {
        public Bot Bot { get; set; }
    }
}
