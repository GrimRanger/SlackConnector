using System;
using NUnit.Framework;
using Should;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Sockets.Data.Inbound;
using SlackConnector.Connections.Sockets.Data.Inbound.Event;
using SlackConnector.Connections.Sockets.Data.Interpreters;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace SlackConnector.Tests.Unit.Connections.Sockets.Messages
{
    internal class given_standard_message_when_processing_message : SpecsFor<MessageInterpreter>
    {
        private string Json { get; set; }
        private string Ts { get; set; }
        private UserInboundMessage Result { get; set; }

        protected override void Given()
        {
            Ts = "1445366603.000002";
            Json = @"
                {
                  'type': 'message',
                  'channel': '&lt;myChannel&gt;',
                  'user': '&lt;myUser&gt;',
                  'text': 'hi, my name is &lt;noobot&gt;',
                  'ts': '1445366603.000002',
                  'team': '&lt;myTeam&gt;'
                }
            ";
        }

        protected override void When()
        {
            Result = (UserInboundMessage)SUT.InterpretMessage(Json);
        }

        [Test]
        public void then_should_look_like_expected()
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            time = time.AddSeconds(Math.Round(double.Parse(Ts))).ToLocalTime();
            var expected = new UserInboundMessage
            {
                MessageType = MessageType.Message,
                Channel = "<myChannel>",
                User = "<myUser>",
                Text = "hi, my name is <noobot>",
                Team = "<myTeam>",
                Time = time,
                RawData = Json
            };

            Result.ShouldLookLike(expected);
        }
    }

    internal class given_non_message_type_message_when_processing_message : SpecsFor<MessageInterpreter>
    {
        private string Json { get; set; }
        private IInboundMessage Result { get; set; }

        protected override void Given()
        {
            Json = @"{ 'type': 'something_else' }";
        }

        protected override void When()
        {
            Result = SUT.InterpretMessage(Json);
        }

        [Test]
        public void then_should_look_like_expected()
        {
            var expected = new BaseInboundMessage
            {
                MessageType = MessageType.Unknown,
                RawData = Json
            };

            Result.ShouldLookLike(expected);
        }
    }

    internal class given_dodge_json_message_when_processing_message : SpecsFor<MessageInterpreter>
    {
        private string Json { get; set; }
        private IInboundMessage Result { get; set; }

        protected override void Given()
        {
            Json = @"{ 'type': 'channel_joined', 'channel': { 'name': 'Channel' } }";
        }

        protected override void When()
        {
            Result = SUT.InterpretMessage(Json);
        }

        [Test]
        public void then_should_return_null()
        {
            var expected = new HubJoinedEvent
            {
                MessageType = MessageType.channel_joined,
                Channel = new Channel {Name = "Channel"},
                RawData = Json
            };
            Result.ShouldLookLike(expected);
        }
    }
}