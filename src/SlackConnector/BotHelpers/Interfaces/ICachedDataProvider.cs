using SlackConnector.Connections.Models;
using SlackConnector.Models;

namespace SlackConnector.BotHelpers.Interfaces
{
    internal interface ICachedDataProvider
    {
        SlackChatHub GetChatHub(Connections.Models.Channel channel);
        SlackChatHub GetChatHub(Group group, string[] users);
        SlackChatHub GetChatHub(Im im, string user);
    }
}
