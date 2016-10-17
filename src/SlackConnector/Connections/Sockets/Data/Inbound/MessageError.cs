using Newtonsoft.Json;

namespace SlackConnector.Connections.Sockets.Data.Inbound
{
    internal class MessageError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}
