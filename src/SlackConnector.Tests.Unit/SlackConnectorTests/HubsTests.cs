﻿using System.Linq;
using Moq;
using NUnit.Framework;
using Should;
using SlackConnector.Connections.ClientFactories;
using SlackConnector.Connections.Clients.Handshake;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Responses;
using SlackConnector.Connections.Sockets.Client;
using SlackConnector.Tests.Unit.Stubs;
using SpecsFor;

namespace SlackConnector.Tests.Unit.SlackConnectorTests
{
    public static class HubsTests
    {
        public class given_channels_are_archived : SpecsFor<SlackConnector>
        {
            private HandshakeResponse HandshakeResponse { get; set; }
            private SlackConnectionFactoryStub SlackFactoryStub { get; set; }

            protected override void InitializeClassUnderTest()
            {
                SlackFactoryStub = new SlackConnectionFactoryStub();
                SUT = new SlackConnector(GetMockFor<IConnectionFactory>().Object, SlackFactoryStub);
            }

            protected override void Given()
            {
                HandshakeResponse = new HandshakeResponse
                {
                    Ok = true,
                    Channels = new[]
                    {
                        new Channel
                        {
                            Id = "Id1",
                            Name = "Name1",
                            IsArchived = true,
                            IsMember = true //TODO: Need to do self things
                        },
                        new Channel
                        {
                            Id = "Id2",
                            Name = "Name2",
                            IsArchived = true,
                            IsMember = true //TODO: Need to do self things
                        },
                    }
                };

                GetMockFor<IHandshakeClient>()
                    .Setup(x => x.FirmShake(It.IsAny<string>()))
                    .ReturnsAsync(HandshakeResponse);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateHandshakeClient())
                    .Returns(GetMockFor<IHandshakeClient>().Object);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateWebSocketClient(HandshakeResponse.WebSocketUrl, null))
                    .Returns(GetMockFor<IWebSocketClient>().Object);
            }

            protected override void When()
            {
                SUT.Connect("something").Wait();
            }

            [Test]
            public void then_should_not_contain_channels()
            {
                SlackFactoryStub.Create_ConnectionInformation.SlackChatHubs.Count.ShouldEqual(0);
            }
        }
        public class given_channels_are_not_archived_and_not_a_member : SpecsFor<SlackConnector>
        {
            private HandshakeResponse HandshakeResponse { get; set; }
            private SlackConnectionFactoryStub SlackFactoryStub { get; set; }

            protected override void InitializeClassUnderTest()
            {
                SlackFactoryStub = new SlackConnectionFactoryStub();
                SUT = new SlackConnector(GetMockFor<IConnectionFactory>().Object, SlackFactoryStub);
            }

            protected override void Given()
            {
                HandshakeResponse = new HandshakeResponse
                {
                    Ok = true,
                    Channels = new[]
                    {
                        new Channel
                        {
                            Id = "Id1",
                            Name = "Name1",
                            IsArchived = false,
                            IsMember = false
                        }
                    }
                };

                GetMockFor<IHandshakeClient>()
                    .Setup(x => x.FirmShake(It.IsAny<string>()))
                    .ReturnsAsync(HandshakeResponse);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateHandshakeClient())
                    .Returns(GetMockFor<IHandshakeClient>().Object);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateWebSocketClient(HandshakeResponse.WebSocketUrl, null))
                    .Returns(GetMockFor<IWebSocketClient>().Object);
            }

            protected override void When()
            {
                SUT.Connect("something").Wait();
            }

            [Test]
            public void then_should_not_contain_channels()
            {
                SlackFactoryStub.Create_ConnectionInformation.SlackChatHubs.Count.ShouldEqual(0);
            }
        }

        public class given_groups_are_archived : SpecsFor<SlackConnector>
        {
            private HandshakeResponse HandshakeResponse { get; set; }
            private SlackConnectionFactoryStub SlackFactoryStub { get; set; }

            protected override void InitializeClassUnderTest()
            {
                SlackFactoryStub = new SlackConnectionFactoryStub();
                SUT = new SlackConnector(GetMockFor<IConnectionFactory>().Object, SlackFactoryStub);
            }

            protected override void Given()
            {
                HandshakeResponse = new HandshakeResponse
                {
                    Ok = true,
                    Groups = new[]
                    {
                        new Group
                        {
                            Id = "group-id",
                            Name = "group-name",
                            IsArchived = true,
                            //Members = new [] { SelfId } //TODO: Need to do self things
                        }
                    }
                };

                GetMockFor<IHandshakeClient>()
                    .Setup(x => x.FirmShake(It.IsAny<string>()))
                    .ReturnsAsync(HandshakeResponse);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateHandshakeClient())
                    .Returns(GetMockFor<IHandshakeClient>().Object);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateWebSocketClient(HandshakeResponse.WebSocketUrl, null))
                    .Returns(GetMockFor<IWebSocketClient>().Object);
            }

            protected override void When()
            {
                SUT.Connect("key").Wait();
            }

            [Test]
            public void then_should_not_contain_channels()
            {
                SlackFactoryStub.Create_ConnectionInformation.SlackChatHubs.Count.ShouldEqual(0);
            }
        }

        public class given_groups_that_are_not_archived_and_is_not_a_member_of_group : SpecsFor<SlackConnector>
        {
            private HandshakeResponse HandshakeResponse { get; set; }
            private const string SelfId = "abc123";
            private SlackConnectionFactoryStub SlackFactoryStub { get; set; }

            protected override void InitializeClassUnderTest()
            {
                SlackFactoryStub = new SlackConnectionFactoryStub();
                SUT = new SlackConnector(GetMockFor<IConnectionFactory>().Object, SlackFactoryStub);
            }

            protected override void Given()
            {
                HandshakeResponse = new HandshakeResponse
                {
                    Ok = true,
                    Self = new Detail { Id = SelfId },
                    Groups = new[]
                    {
                        new Group
                        {
                            Id = "group-id",
                            Name = "group-name",
                            IsArchived = false,
                            Members = new [] { "something-else" }
                        }
                    }
                };

                GetMockFor<IHandshakeClient>()
                    .Setup(x => x.FirmShake(It.IsAny<string>()))
                    .ReturnsAsync(HandshakeResponse);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateHandshakeClient())
                    .Returns(GetMockFor<IHandshakeClient>().Object);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateWebSocketClient(HandshakeResponse.WebSocketUrl, null))
                    .Returns(GetMockFor<IWebSocketClient>().Object);
            }

            protected override void When()
            {
                SUT.Connect("key").Wait();
            }

            [Test]
            public void then_should_not_contain_any_channels()
            {
                SlackFactoryStub.Create_ConnectionInformation.SlackChatHubs.Count.ShouldEqual(0);
            }
        }

        public class given_instant_message_channel_when_user_id_exists_in_cache : SpecsFor<SlackConnector>
        {
            private HandshakeResponse HandshakeResponse { get; set; }
            private SlackConnectionFactoryStub SlackFactoryStub { get; set; }

            protected override void InitializeClassUnderTest()
            {
                SlackFactoryStub = new SlackConnectionFactoryStub();
                SUT = new SlackConnector(GetMockFor<IConnectionFactory>().Object, SlackFactoryStub);
            }

            protected override void Given()
            {
                HandshakeResponse = new HandshakeResponse
                {
                    Ok = true,
                    Users = new []
                    {
                        new User
                        {
                            Id = "user-id-thingy",
                            Name = "name-4eva"
                        }
                    },
                    Ims = new []
                    {
                        new Im
                        {
                            Id = "im-id-yay",
                            User = "user-id-thingy"
                        }
                    }
                };

                GetMockFor<IHandshakeClient>()
                    .Setup(x => x.FirmShake(It.IsAny<string>()))
                    .ReturnsAsync(HandshakeResponse);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateHandshakeClient())
                    .Returns(GetMockFor<IHandshakeClient>().Object);

                GetMockFor<IConnectionFactory>()
                    .Setup(x => x.CreateWebSocketClient(HandshakeResponse.WebSocketUrl, null))
                    .Returns(GetMockFor<IWebSocketClient>().Object);
            }

            protected override void When()
            {
                SUT.Connect("something").Wait();
            }

            [Test]
            public void then_should_contain_single_channel()
            {
                SlackFactoryStub.Create_ConnectionInformation.SlackChatHubs.Count.ShouldEqual(1);
            }

            [Test]
            public void then_should_use_username()
            {
                var im = SlackFactoryStub.Create_ConnectionInformation.SlackChatHubs.First().Value;
                im.Name.ShouldEqual("@" + HandshakeResponse.Users[0].Name);
            }
        }
    }
}