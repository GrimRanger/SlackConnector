using SlackConnector.Connections.Clients.Api.Helpers;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("channels.list")]
    internal class ChannelListResponce: ApiResponceWithoutParametres
    {
        public Models.Channel[] Channels { get; set; }
    }
}
