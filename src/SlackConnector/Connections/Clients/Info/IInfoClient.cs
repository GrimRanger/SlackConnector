using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace SlackConnector.Connections.Clients.Info
{
    public interface IInfoClient
    {
        Task<SlackChatHub> GetChatHub(string slackKey, string id);
        Task<SlackUser> GetUser(string slackKey, string userId);
    }
}
