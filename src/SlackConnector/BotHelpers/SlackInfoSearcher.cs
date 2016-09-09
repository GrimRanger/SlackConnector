using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.BotHelpers.Interfaces;
using SlackConnector.Connections.Clients.Api;
using SlackConnector.Connections.Clients.Api.Responces.Info;
using SlackConnector.Connections.Clients.Api.Responces.List;
using SlackConnector.Models;

namespace SlackConnector.BotHelpers
{
    internal class SlackInfoSearcher : ISlackInfoSearcher
    {
        private readonly ICachedDataProvider _cachedDataProvider;

        public SlackInfoSearcher() : this(new CachedDataProvider())
        { }
        public SlackInfoSearcher(ICachedDataProvider cachedDataProvider)
        {
            _cachedDataProvider = new CachedDataProvider();
        }

        public async Task<SlackChatHub> GetChatHub(IApiClient apiClient, string slackKey, string id)
        {
            var result = await TryGetChannel(apiClient, slackKey, id);
            if (result == null)
                result = await TryGetDirectMessageConversation(apiClient, slackKey, id);
            if (result == null)
                result = await TryGetGroup(apiClient, slackKey, id);

            return result;
        }

        public async Task<SlackUser> GetUser(IApiClient apiClient, string slackKey, string userId)
        {
            var result = await TryGetUser(apiClient, slackKey, userId);
            if (result == null)
                result = await TryGetBot(apiClient, slackKey, userId);

            return result;
        }

        private async Task<SlackChatHub> TryGetChannel(IApiClient apiClient, string slackKey, string channelId)
        {
            var channelIdParameter = new KeyValuePair<string, string>("channel", channelId);
            var respose = await apiClient.SendRequest<ChannelInfoResponse>(slackKey, channelIdParameter);
            if (respose == null)
                return null;
            var channel = respose.Channel;
            var result = _cachedDataProvider.GetChatHub(channel);

            return result;
        }

        private async Task<SlackChatHub> TryGetDirectMessageConversation(IApiClient apiClient, string slackKey, string id)
        {
            var respose = await apiClient.SendRequest<DirectMessageConversationListResponse>(slackKey);
            if (respose == null)
                return null;
            var directMessages = respose.Ims;
            foreach (var directMessage in directMessages)
            {
                if (directMessage.Id.Equals(id))
                {
                    var user = await TryGetUser(apiClient, slackKey, directMessage.User);
                    var result = _cachedDataProvider.GetChatHub(directMessage, user.Name);

                    return result;
                }
            }

            return null;
        }

        private async Task<SlackChatHub> TryGetGroup(IApiClient apiClient, string slackKey, string groupId)
        {
            var channelIdParameter = new KeyValuePair<string, string>("channel", groupId);
            var respose = await apiClient.SendRequest<GroupInfoResponse>(slackKey, channelIdParameter);
            if (respose == null)
                return null;
            var group = respose.Group;
            var usersName = new List<string>();
            foreach (var userId in group.Members)
            {
                var user = await TryGetUser(apiClient, slackKey, userId);
                usersName.Add(user.Name);

            }
            var result = _cachedDataProvider.GetChatHub(group, usersName.ToArray());

            return result;
        }

        private async Task<SlackUser> TryGetUser(IApiClient apiClient, string slackKey, string userId)
        {
            var userIdParameter = new KeyValuePair<string, string>("user", userId);
            var respose = await apiClient.SendRequest<UserInfoResponse>(slackKey, userIdParameter);
            var user = respose.User;
            var result = new SlackUser
            {
                Id = user.Id,
                Name = user.Name,
                Icons = user.Profile.Icons
            };

            return result;
        }

        private async Task<SlackUser> TryGetBot(IApiClient apiClient, string slackKey, string userId)
        {
            var userIdParameter = new KeyValuePair<string, string>("bot", userId);
            var respose = await apiClient.SendRequest<BotInfoResponse>(slackKey, userIdParameter);
            var bot = respose.Bot;
            var result = new SlackUser
            {
                Id = bot.Id,
                Name = bot.Name,
                Icons = bot.Icons
            };

            return result;
        }
    }
}
