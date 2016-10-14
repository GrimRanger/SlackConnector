using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackConnector.Serialising;

namespace SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal abstract class InboundData 
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConverter))]
        public MessageType MessageType { get; set; }
        [JsonProperty("error")]
        public Error Error { get; set; }

        public abstract Task Accept(IInboundDataVisitor visitor);
    }

    internal class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}
