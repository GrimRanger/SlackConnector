using Newtonsoft.Json;

namespace SlackConnector.Connections.Sockets.Data.Outbound
{
    internal class TypingIndicatorMessage : BaseMessage
    {
        [JsonProperty("channel")]
        public string Channel { get; set; } 
    }
}