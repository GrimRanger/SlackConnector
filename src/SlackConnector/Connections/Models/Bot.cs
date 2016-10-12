using System.Collections.Generic;
using Newtonsoft.Json;
using SlackConnector.Connections.Models.JsonConverters;

namespace SlackConnector.Connections.Models
{

    [JsonConverter(typeof(BotsConverter))]
    internal class Bot : Detail
    {
        public Dictionary<string, string> Icons { get; set; } = new Dictionary<string, string>();
    }
}
