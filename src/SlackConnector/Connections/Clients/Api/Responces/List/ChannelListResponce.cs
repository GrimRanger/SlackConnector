using SlackConnector.Connections.Clients.Api.Helpers;

namespace SlackConnector.Connections.Clients.Api.Responces
{
    [RequestPath("channels.list")]
    internal class ChannelListResponce: ApiResponce
    {
        public Models.Channel[] Channels { get; set; }
    }
}
