using Newtonsoft.Json;

namespace SlackConnector.Connections.Sockets.Data.Inbound
{
    internal class BotInboundMessage : UserInboundMessage
    {
        [JsonProperty("bot_id")]
        public override string User { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
}
