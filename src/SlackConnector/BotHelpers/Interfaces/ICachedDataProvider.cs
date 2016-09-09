using SlackConnector.Connections.Models;
using SlackConnector.Models;

namespace SlackConnector.BotHelpers
{
    internal interface ICachedDataProvider
    {
        SlackChatHub GetChatHub(Channel channel);
        SlackChatHub GetChatHub(Group group, string[] users);
        SlackChatHub GetChatHub(Im im, string user);
    }
}
