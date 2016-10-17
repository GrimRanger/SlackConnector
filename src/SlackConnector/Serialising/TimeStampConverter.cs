using System;
using Newtonsoft.Json;

namespace SlackConnector.Serialising
{
    internal class TimeStampConverter : JsonConverter
    {
        protected DateTime Create(Type objectType, string str)
        {
            double value;
            if (double.TryParse(str, out value))
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(Math.Round(value)).ToLocalTime();

                return dtDateTime;
            }
            return DateTime.Now;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Create target object based on JObject
            DateTime target = Create(objectType, reader.Value.ToString());

            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(string).IsAssignableFrom(objectType);
        }
    }
}
