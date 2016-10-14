using Newtonsoft.Json;
using SlackConnector.Connections.Models.JsonConverters;

namespace SlackConnector.Connections.Models
{
    [JsonConverter(typeof(MessageConverter))]
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
        [JsonIgnore]
        public string RawData { get; set; }
    }
}
