using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackConnector.Connections.Sockets.Data.Visitors;
using SlackConnector.Serialising;

namespace SlackConnector.Connections.Sockets.Data.Inbound
{
    internal class UserInboundMessage : BaseInboundMessage
    {
        [JsonProperty("subtype")]
        [JsonConverter(typeof(EnumConverter))]
        public MessageSubType MessageSubType { get; set; }
        public virtual string User { get; set; }
        public string Channel { get; set; }
        public string Text { get; set; }
        public string Team { get; set; }
        
        [JsonProperty("ts")]
        [JsonConverter(typeof(TimeStampConverter))]
        public DateTime Time { get; set; }

        public override Task Accept(IInboundDataVisitor visitor)
        {
            return visitor.HandleUserMessage(this);
        }
    }
}