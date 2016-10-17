using System;
using System.Net;
using Newtonsoft.Json;
using SlackConnector.Connections.Sockets.Data.Inbound;
using SlackConnector.Connections.Sockets.Data.Inbound.Event;

namespace SlackConnector.Connections.Sockets.Data.Interpreters
{
    internal class MessageInterpreter : IMessageInterpreter
    {
        public IInboundMessage InterpretMessage(string json)
        {
            IInboundMessage data = null;
            try
            {
                data = JsonConvert.DeserializeObject<BaseInboundMessage>(json);
                switch (data.MessageType)
                {
                    case MessageType.Message:
                        data = ExtractMessage(json);
                        break;
                    case MessageType.mpim_joined:
                        data = ExtractEvent(json);
                        break;
                    case MessageType.group_joined:
                        data = ExtractEvent(json);
                        break;
                    case MessageType.channel_joined:
                        data = ExtractEvent(json);
                        break;
                }
                data.RawData = json;

                return data;
            }
            catch (Exception ex)
            {
                if (SlackConnector.LoggingLevel == ConsoleLoggingLevel.FatalErrors)
                {
                    Console.WriteLine($"Unable to parse message: {json}");
                    Console.WriteLine(ex);
                }
            }

            return data;
        }

        private IInboundMessage ExtractEvent(string json)
        {
            var message = JsonConvert.DeserializeObject<HubJoinedEvent>(json);

            return message;
        }

        private IInboundMessage ExtractMessage(string json)
        {
            var message = JsonConvert.DeserializeObject<UserInboundMessage>(json);
            switch (message?.MessageSubType)
            {
                case MessageSubType.bot_message:
                    {
                        message = JsonConvert.DeserializeObject<BotInboundMessage>(json);
                        break;
                    }
            }
            if (message != null)
            {
                message.Channel = WebUtility.HtmlDecode(message.Channel);
                message.User = WebUtility.HtmlDecode(message.User);
                message.Text = WebUtility.HtmlDecode(message.Text);
                message.Team = WebUtility.HtmlDecode(message.Team);
            }

            return message;
        }
    }
}