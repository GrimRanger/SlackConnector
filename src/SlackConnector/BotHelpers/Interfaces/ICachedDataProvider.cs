using SlackConnector.Connections.Models;
using SlackConnector.Models;

namespace SlackConnector.BotHelpers
{
    internal interface ICachedDataProvider
    {
        SlackChatHub GetChatHub(Channel channel);
        SlackChatHub GetChatHub(Group group);
        SlackChatHub GetChatHub(Im im, string user);
    }
}
