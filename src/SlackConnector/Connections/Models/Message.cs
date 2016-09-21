using Newtonsoft.Json;

namespace SlackConnector.Connections.Models
{
    internal class Message : Detail
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("ts")]
        public string TimeStamp { get; set; }
        [JsonProperty("user")]
        public string User { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
