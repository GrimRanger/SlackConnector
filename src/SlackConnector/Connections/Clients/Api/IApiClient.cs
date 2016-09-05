using System.Threading.Tasks;

namespace SlackConnector.Connections.Clients.Api
{
    internal interface IApiClient
    {
        Task<T> Send<T>(string slackKey) where T : class;
    }
}
