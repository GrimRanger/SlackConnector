﻿using Newtonsoft.Json;

namespace SlackConnector.Connections.Models
{
    internal class Channel : Detail
    {
        [JsonProperty("is_channel")]
        public bool IsChannel { get; set; }

        [JsonProperty("is_archived")]
        public bool IsArchived { get; set; }

        [JsonProperty("is_general ")]
        public bool IsGeneral { get; set; }

        [JsonProperty("is_member")]
        public bool IsMember { get; set; }  
    }
}