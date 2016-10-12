using SlackConnector.Connections.Clients.Api.Requests;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("channels.list")]
    internal class ChannelListResponce: ApiResponceBase
    {
        public Models.Channel[] Channels { get; set; }
    }
}
