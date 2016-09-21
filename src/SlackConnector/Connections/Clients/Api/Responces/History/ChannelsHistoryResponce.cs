using SlackConnector.Connections.Clients.Api.Helpers;

namespace SlackConnector.Connections.Clients.Api.Responces.History
{
    [RequestPath("channels.history")]
    internal class ChannelsHistoryResponce : ApiResponceWithParametres
    {
        public Models.Message[] Messages { get; set; }
    }
}
