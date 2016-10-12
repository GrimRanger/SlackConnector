using SlackConnector.Connections.Clients.Api.Requests;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
   
    [RequestPath("channels.info")]
    internal class ChannelInfoResponse : ApiResponceBase
    {
        public Models.Channel Channel { get; set; }
    }
}
