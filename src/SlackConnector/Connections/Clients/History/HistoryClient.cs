using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Api.HighLevelClient;
using SlackConnector.Connections.Clients.Api.Responces.History;
using SlackConnector.Connections.Clients.Info;
using SlackConnector.Connections.Models;
using SlackConnector.Models;

namespace SlackConnector.Connections.Clients.History
{
    internal class HistoryClient : IHistoryClient
    {
        private readonly IHighLevelApiClient _highLevelApiClient;
        private readonly IInfoClient _infoClient;

        public HistoryClient(IHighLevelApiClient highLevelApiClient, IInfoClient infoClient)
        {
            _highLevelApiClient = highLevelApiClient;
            _infoClient = infoClient;
        }

        public async Task<IEnumerable<SlackMessage>> GetChatHubHistory(string slackKey, SlackChatHub chatHub, int count)
        {
            var result = new List<SlackMessage>();
            IEnumerable<SlackMessage> messages;
            switch (chatHub.Type)
            {
                case SlackChatHubType.Channel:
                    messages = await GetChannelHistory(slackKey, chatHub.Id, count);
                    result.AddRange(messages);
                    break;

                case SlackChatHubType.DM:
                    messages = await GetImHistory(slackKey, chatHub.Id, count);
                    result.AddRange(messages);
                    break;

                case SlackChatHubType.Group:
                    messages = await GetGroupHistory(slackKey, chatHub.Id, count);
                    result.AddRange(messages);
                    break;
            }

            return result;
        }

        private async Task<IEnumerable<SlackMessage>> GetChannelHistory(string slackKey, string id, int count)
        {
            ChannelHistoryResponce result = await _highLevelApiClient.GetChannelHistory(slackKey, id, count);

            return await MapMessages(slackKey, result.Messages, id);
        }

        private async Task<IEnumerable<SlackMessage>> GetImHistory(string slackKey, string id, int count)
        {
            DirectMessageConversationHistoryResponse result = await _highLevelApiClient.GetImHistory(slackKey, id, count);

            return await MapMessages(slackKey, result.Messages, id);
        }

        private async Task<IEnumerable<SlackMessage>> GetGroupHistory(string slackKey, string id, int count)
        {
            GroupHistoryResponce result = await _highLevelApiClient.GetGroupHistory(slackKey, id, count);

            return await MapMessages(slackKey, result.Messages, id);
        }

        private async Task<IEnumerable<SlackMessage>> MapMessages(string slackKey, Message[] messages, string channelId)
        {
            var result = new List<SlackMessage>();
            foreach (var message in messages)
            {
                SlackUser user = await _infoClient.GetUser(slackKey, message.User);
                SlackChatHub chatHub = await _infoClient.GetChatHub(slackKey, channelId);
                var slackMessage = new SlackMessage
                {
                    Text = message.Text,
                    ChatHub = chatHub,
                    User = user,
                    RawData = message.RawData
                };
                result.Add(slackMessage);
            }

            return result;
        }
    }
}
