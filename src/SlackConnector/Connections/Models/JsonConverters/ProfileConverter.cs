using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackConnector.Connections.Models.JsonConverters
{
    internal class ProfileConverter : JsonCreationConverter<Profile>
    {
        protected override Profile Create(Type objectType, JObject jObject)
        {
            var target = new Profile();

            foreach (var field in jObject)
            {
                if (field.Key.Contains("image_"))
                {
                    target.Icons.Add(field.Key, field.Value.ToString());
                }
            }

            return target;
        }
    }
}
