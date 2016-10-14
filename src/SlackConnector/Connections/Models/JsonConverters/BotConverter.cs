using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SlackConnector.Connections.Models.JsonConverters
{
    internal class BotConverter : JsonCreationConverter<Bot>
    {
        protected override Bot Create(Type objectType, JObject jObject)
        {
            var target = new Bot();

            JToken icons;
            
            if (jObject.TryGetValue("icons", out icons))
            {
                foreach (var iconProperties in icons.Children())
                {
                    foreach (var icon in iconProperties)
                    {
                        if (icon.Path.Contains("image_"))
                        {
                            target.Icons.Add(icon.Path, icon.Value<string>());
                        }
                    }
                }
            }

            return target;
        }
    }
}
