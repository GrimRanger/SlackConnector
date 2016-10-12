using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Clients.Api.Responces;

namespace SlackConnector.Connections.Clients.Api.LowLevelClient
{
    internal interface ILowLevelApiClient
    {
        Task<T> SendRequest<T>(string slackKey, ApiRequest<T> apiRequest) where T : ApiResponceBase;
    }
}