using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Api.LowLevelClient;
using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Clients.Api.Responces.History;
using SlackConnector.Connections.Clients.Api.Responces.Info;
using SlackConnector.Connections.Clients.Api.Responces.List;

namespace SlackConnector.Connections.Clients.Api.HighLevelClient
{
    internal class HighLevelApiClient : IHighLevelApiClient
    {
        public ILowLevelApiClient LowLevelApiClient { get; }

        public HighLevelApiClient(ILowLevelApiClient lowLevelApiClient)
        {
            LowLevelApiClient = lowLevelApiClient;
        }

        public async Task<UserListResponse> GetUserList(string slackKey)
        {
            var request = new ApiRequest<UserListResponse>();
            var result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result; 
        }

        public async Task<ChannelListResponce> GetChannelList(string slackKey)
        {
            var request = new ApiRequest<ChannelListResponce>();
            var result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<GroupListResponse> GetGroupList(string slackKey)
        {
            var request = new ApiRequest<GroupListResponse>();
            var result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<DirectMessageConversationListResponse> GetDirectMessageConversationList(string slackKey)
        {
            var request = new ApiRequest<DirectMessageConversationListResponse>();
            var result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }
       

        public async Task<UserInfoResponse> GetUserInfo(string slackKey, string id)
        {
            var userIdParameter = new KeyValuePair<string, string>("user", id);
            var request = new ApiRequest<UserInfoResponse>(userIdParameter);
            var result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<BotInfoResponse> GetBotInfo(string slackKey, string id)
        {
            var userIdParameter = new KeyValuePair<string, string>("bot", id);
            var request = new ApiRequest<BotInfoResponse>(userIdParameter);
            BotInfoResponse result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<ChannelInfoResponse> GetChannelInfo(string slackKey, string id)
        {
            var channelIdParameter = new KeyValuePair<string, string>("channel", id);
            var request = new ApiRequest<ChannelInfoResponse>(channelIdParameter);
            ChannelInfoResponse result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<GroupInfoResponse> GetGroupInfo(string slackKey, string id)
        {
            var channelIdParameter = new KeyValuePair<string, string>("channel", id);
            var request = new ApiRequest<GroupInfoResponse>(channelIdParameter);
            GroupInfoResponse result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<GroupHistoryResponce> GetGroupHistory(string slackKey, string id, int count)
        {
            var parameters = GetHistoryParameters(id, count);
            var request = new ApiRequest<GroupHistoryResponce>(parameters);
            GroupHistoryResponce result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<ChannelHistoryResponce> GetChannelHistory(string slackKey, string id, int count)
        {
            var parameters = GetHistoryParameters(id, count);
            var request = new ApiRequest<ChannelHistoryResponce>(parameters);
            ChannelHistoryResponce result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        public async Task<DirectMessageConversationHistoryResponse> GetImHistory(string slackKey, string id, int count)
        {
            var parameters = GetHistoryParameters(id, count);
            var request = new ApiRequest<DirectMessageConversationHistoryResponse>(parameters);
            DirectMessageConversationHistoryResponse result = await LowLevelApiClient.SendRequest(slackKey, request);

            return result;
        }

        private KeyValuePair<string, string>[] GetHistoryParameters(string channel, int count)
        {
            var result = new[]
            {
                new KeyValuePair<string, string>("channel", channel),
                new KeyValuePair<string, string>("count", count.ToString())
            };

            return result;
        }
    }
}
