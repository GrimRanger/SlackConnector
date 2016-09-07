using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Api;
using SlackConnector.Models;

namespace SlackConnector.BotHelpers.Interfaces
{
    internal interface ISlackInfoSearcher
    {
        Task<SlackChatHub> GetChatHub(IApiClient apiClient, string slackKey, string id);
        Task<SlackUser> GetUser(IApiClient apiClient, string slackKey, string userId);
    }
}
