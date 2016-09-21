using Newtonsoft.Json;

namespace SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class BotInboundMessage : InboundMessage
    {
        [JsonProperty("bot_id")]
        public override string User { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
}
