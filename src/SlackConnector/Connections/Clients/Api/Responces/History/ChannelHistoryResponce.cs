using SlackConnector.Connections.Clients.Api.Requests;

namespace SlackConnector.Connections.Clients.Api.Responces.History
{
    [RequestPath("channels.history")]
    internal class ChannelHistoryResponce : ApiResponceBase
    {
        public Models.Message[] Messages { get; set; }
    }
}
