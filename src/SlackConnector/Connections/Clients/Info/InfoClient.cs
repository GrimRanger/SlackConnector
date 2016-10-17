using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.BotHelpers;
using SlackConnector.BotHelpers.Interfaces;
using SlackConnector.Connections.Clients.Api.HighLevelClient;
using SlackConnector.Models;

namespace SlackConnector.Connections.Clients.Info
{
    internal class InfoClient : IInfoClient
    {
        private readonly IHighLevelApiClient _highLevelApiClient;
        private readonly ICachedDataProvider _cachedDataProvider;
        private readonly IDictionary<string, SlackChatHub> _hubCache;
        private readonly IDictionary<string, SlackUser> _userCache;

        public InfoClient(IHighLevelApiClient highLevelApiClient) : this(new CachedDataProvider(), highLevelApiClient)
        { }

        public InfoClient(IHighLevelApiClient highLevelApiClient, IDictionary<string, SlackChatHub> hubCache = null, IDictionary<string, SlackUser> userCache = null) :
            this(new CachedDataProvider(), highLevelApiClient, hubCache, userCache)
        { }

        public InfoClient(ICachedDataProvider cachedDataProvider, IHighLevelApiClient highLevelApiClient, IDictionary<string, SlackChatHub> hubCache = null, IDictionary<string, SlackUser> userCache = null)
        {
            _highLevelApiClient = highLevelApiClient;
            _cachedDataProvider = cachedDataProvider;
            _hubCache = hubCache;
            _userCache = userCache;
        }

        public async Task<SlackChatHub> GetChatHub(string slackKey, string id)
        {
            if (_hubCache != null && _hubCache.ContainsKey(id))
                return _hubCache[id];

            var result = await TryGetChannel(slackKey, id);
            if (result == null)
                result = await TryGetDirectMessageConversation(slackKey, id);
            if (result == null)
                result = await TryGetGroup(slackKey, id);

            _hubCache?.Add(result.Id, result);

            return result;
        }

        public async Task<SlackUser> GetUser(string slackKey, string userId)
        {
            if (_userCache != null && _userCache.ContainsKey(userId))
                return _userCache[userId];
            var result = await TryGetUser(slackKey, userId);
            if (result == null)
                result = await TryGetBot(slackKey, userId);

            _userCache?.Add(result.Id, result);

            return result;
        }

        private async Task<SlackChatHub> TryGetChannel(string slackKey, string channelId)
        {

            var respose = await _highLevelApiClient.GetChannelInfo(slackKey, channelId);
            if (respose == null)
                return null;
            var channel = respose.Channel;
            var result = _cachedDataProvider.GetChatHub(channel);

            return result;
        }

        private async Task<SlackChatHub> TryGetDirectMessageConversation(string slackKey, string id)
        {
            var respose = await _highLevelApiClient.GetDirectMessageConversationList(slackKey);
            if (respose == null)
                return null;
            var directMessages = respose.Ims;
            foreach (var directMessage in directMessages)
            {
                if (directMessage.Id.Equals(id))
                {
                    var user = await TryGetUser(slackKey, directMessage.User);
                    var result = _cachedDataProvider.GetChatHub(directMessage, user.Name);

                    return result;
                }
            }

            return null;
        }

        private async Task<SlackChatHub> TryGetGroup(string slackKey, string groupId)
        {
            var respose = await _highLevelApiClient.GetGroupInfo(slackKey, groupId);
            if (respose == null)
                return null;
            var group = respose.Group;
            var usersName = new List<string>();
            foreach (var userId in group.Members)
            {
                var user = await TryGetUser(slackKey, userId);
                usersName.Add(user.Name);

            }
            var result = _cachedDataProvider.GetChatHub(group, usersName.ToArray());

            return result;
        }

        private async Task<SlackUser> TryGetUser(string slackKey, string userId)
        {
            var respose = await _highLevelApiClient.GetUserInfo(slackKey, userId);
            var user = respose.User;
            var result = new SlackUser
            {
                Id = user.Id,
                Name = user.Name,
                Icons = user.Profile.Icons
            };

            return result;
        }

        private async Task<SlackUser> TryGetBot(string slackKey, string userId)
        {
            var respose = await _highLevelApiClient.GetBotInfo(slackKey, userId);
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
