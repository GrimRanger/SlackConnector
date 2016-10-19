using System;
using Newtonsoft.Json;
using SlackConnector.Connections.Models.JsonConverters;
using SlackConnector.Serialising;

namespace SlackConnector.Connections.Models
{
    [JsonConverter(typeof(MessageConverter))]
    internal class Message : Detail
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("user")]
        public string User { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("ts")]
        [JsonConverter(typeof(TimeStampConverter))]
        public DateTime Time { get; set; }
        [JsonIgnore]
        public string RawData { get; set; }
    }
}
