using System;
using Newtonsoft.Json.Linq;
using SlackConnector.Serialising;

namespace SlackConnector.Connections.Models.JsonConverters
{
    internal class MessageConverter : JsonCreationConverter<Message>
    {
        protected override Message Create(Type objectType, JObject jObject)
        {
            var target = new Message {RawData = jObject.ToString()};

            return target;
        }
    }
}
