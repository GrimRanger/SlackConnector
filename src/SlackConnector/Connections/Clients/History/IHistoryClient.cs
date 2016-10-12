using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace SlackConnector.Connections.Clients.History
{
    public interface IHistoryClient
    {
        Task<IEnumerable<SlackMessage>> GetChatHubHistory(string slackKey, SlackChatHub chatHub, int count);
    }
}
