using SlackConnector.BotHelpers.Interfaces;
using SlackConnector.Connections.Models;
using SlackConnector.Models;

namespace SlackConnector.BotHelpers
{
    internal class CachedDataProvider : ICachedDataProvider
    {
        public SlackChatHub GetChatHub(Channel channel)
        {
            var newChannel = new SlackChatHub
            {
                Id = channel.Id,
                Name = "#" + channel.Name,
                Type = SlackChatHubType.Channel
            };

            return newChannel;
        }

        public SlackChatHub GetChatHub(Group group, string[] users)
        {
            var name = string.Join(", ", users);

            if (string.IsNullOrEmpty(name))
                name = group.Name;
            var newGroup = new SlackChatHub
            {
                Id = group.Id,
                Name = "#" + name,
                Type = SlackChatHubType.Group
            };

            return newGroup;
        }

        public SlackChatHub GetChatHub(Im im, string user)
        {
            var dm = new SlackChatHub
            {
                Id = im.Id,
                Name = "@" + (string.IsNullOrEmpty(user) ? im.User : user),
                Type = SlackChatHubType.DM
            };

            return dm;
        }
    }
}
