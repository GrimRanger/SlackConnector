using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Api.Responces;

namespace SlackConnector.Connections.Clients.Api
{
    internal interface IApiClient
    {
        Task<T> SendRequest<T>(string slackKey) where T : ApiResponceWithoutParametres;
        Task<T> SendRequest<T>(string slackKey, params KeyValuePair<string, string>[] parameters) where T : ApiResponceWithParametres;
    }
}