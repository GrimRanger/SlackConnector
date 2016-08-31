using System.Collections.Generic;
using Newtonsoft.Json;
using SlackConnector.Connections.Models.JsonConverters;

namespace SlackConnector.Connections.Models
{
    [JsonConverter(typeof(ProfileConverter))]
    internal class Profile
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("real_name_normalized")]
        public string RealNameNormalised { get; set; }

        public string Email { get; set; }

        public string Skype { get; set; }

        public string Phone { get; set; }

        public Dictionary<string,string> Icons { get; set; } = new Dictionary<string, string>();
    }
}