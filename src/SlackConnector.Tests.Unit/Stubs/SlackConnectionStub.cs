﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.EventHandlers;
using SlackConnector.Models;

namespace SlackConnector.Tests.Unit.Stubs
{
    public class SlackConnectionStub : ISlackConnection
    {
        public string[] Aliases { get; set; }
        public IEnumerable<SlackChatHub> ConnectedDMs { get; }
        public IEnumerable<SlackChatHub> ConnectedChannels { get; }
        public IEnumerable<SlackChatHub> ConnectedGroups { get; }
        public IReadOnlyDictionary<string, SlackChatHub> HubCache { get; }
        public IReadOnlyDictionary<string, SlackUser> UserCache { get; }
        public bool IsConnected { get; }
        public DateTime? ConnectedSince { get; }
        public string SlackKey { get; }
        public ContactDetails Team { get; }
        public ContactDetails Self { get; }
        public Task Connect(string slackKey)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<SlackMessage>> GetHistory(SlackChatHub slackChatHub, int count)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task Say(BotMessage message)
        {
            throw new NotImplementedException();
        }

        public void SendApi(string command)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinDirectMessageChannel(string user)
        {
            throw new NotImplementedException();
        }

        public Task IndicateTyping(SlackChatHub chatHub)
        {
            throw new NotImplementedException();
        }

        public event DisconnectEventHandler OnDisconnect;
        public void RaiseOnDisconnect()
        {
            OnDisconnect?.Invoke();
        }

        public event MessageReceivedEventHandler OnMessageReceived;
        public event ChannelJoinedEventHandler OnChannelJoined;
        public string Test(string text)
        {
            throw new NotImplementedException();
        }

        public void RaiseOnMessageReceived()
        {
            OnMessageReceived?.Invoke(null);
        }
    }
}