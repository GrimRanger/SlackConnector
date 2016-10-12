using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Api.LowLevelClient;
using SlackConnector.Connections.Clients.Api.Responces.History;
using SlackConnector.Connections.Clients.Api.Responces.Info;
using SlackConnector.Connections.Clients.Api.Responces.List;

namespace SlackConnector.Connections.Clients.Api.HighLevelClient
{
    internal interface IHighLevelApiClient
    {
        ILowLevelApiClient LowLevelApiClient { get; }
        Task<UserListResponse> GetUserList(string slackKey);
        Task<ChannelListResponce> GetChannelList(string slackKey);
        Task<GroupListResponse> GetGroupList(string slackKey);
        Task<DirectMessageConversationListResponse> GetDirectMessageConversationList(string slackKey);
        Task<UserInfoResponse> GetUserInfo(string slackKey, string id);
        Task<BotInfoResponse> GetBotInfo(string slackKey, string id);
        Task<ChannelInfoResponse> GetChannelInfo(string slackKey, string id);
        Task<GroupInfoResponse> GetGroupInfo(string slackKey, string id);
        Task<ChannelHistoryResponce> GetChannelHistory(string slackKey, string id, int count);
        Task<GroupHistoryResponce> GetGroupHistory(string slackKey, string id, int count);
        Task<DirectMessageConversationHistoryResponse> GetImHistory(string slackKey, string id, int count);
    }
}
