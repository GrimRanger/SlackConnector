using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackConnector.Connections.Sockets.Data.Visitors;
using SlackConnector.Serialising;

namespace SlackConnector.Connections.Sockets.Data.Inbound
{
    internal interface IInboundMessage
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConverter))]
        MessageType MessageType { get; set; }
        string RawData { get; set; }
        [JsonProperty("error")]
        MessageError Error { get; set; }

        Task Accept(IInboundDataVisitor visitor);
    }
}