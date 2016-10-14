using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackConnector.Serialising;

namespace SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class InboundMessage : InboundData
    {
        [JsonProperty("subtype")]
        [JsonConverter(typeof(EnumConverter))]
        public MessageSubType MessageSubType { get; set; }
        public virtual string User { get; set; }
        public string Channel { get; set; }
        public string Text { get; set; }
        public string Team { get; set; }
        public string RawData { get; set; }
        [JsonProperty("ts")]
        [JsonConverter(typeof(TimeStampConverter))]
        public DateTime Time { get; set; }

        public override Task Accept(IInboundDataVisitor visitor)
        {
            return visitor.HandleInboundMessage(this);
        }
    }
}