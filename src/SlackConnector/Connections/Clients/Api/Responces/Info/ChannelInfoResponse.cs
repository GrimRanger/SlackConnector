using SlackConnector.Connections.Clients.Api.Helpers;

namespace SlackConnector.Connections.Clients.Api.Responces.Info
{
   
    [RequestPath("channels.info")]
    internal class ChannelInfoResponse : ApiResponceWithParametres
    {
        public Models.Channel Channel { get; set; }
    }
}
