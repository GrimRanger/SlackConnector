using System;
using System.Net;
using Newtonsoft.Json;
using SlackConnector.Connections.Sockets.Messages.Inbound.Event;

namespace SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class MessageInterpreter : IMessageInterpreter
    {
        public InboundData InterpretMessage(string json)
        {
            InboundData data = null;
            try
            {
                data = JsonConvert.DeserializeObject<StandardInboundData>(json);
                if (data.MessageType == MessageType.Message)
                    data = ExtractMessage(json);
                else if (data.MessageType == MessageType.channel_joined || 
                        data.MessageType == MessageType.group_joined ||
                        data.MessageType == MessageType.mpim_joined)
                    data = ExtractEvent(json);

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

        private InboundData ExtractEvent(string json)
        {
            var message = JsonConvert.DeserializeObject<InboundChatHubJoinedEvent>(json);

            return message;
        }

        private InboundMessage ExtractMessage(string json)
        {
            var message = JsonConvert.DeserializeObject<InboundMessage>(json);
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
                message.RawData = json;
            }

            return message;
        }
    }
}