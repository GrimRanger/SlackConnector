using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using SlackConnector.Connections.Clients.Api.Helpers;

namespace SlackConnector.Connections.Clients.Api
{
    internal class ApiClient : IApiClient
    {
        private readonly IRequestExecutor _requestExecutor;
        private const string SendMessagePath = "/api/";

        public ApiClient(IRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

         async Task<T> IApiClient.SendRequest<T>(string slackKey)
        {
            RequestPath path = RequestPath.GetRequestPath<T>();
            var request = new RestRequest(SendMessagePath + path.Path);
            request.AddParameter("token", slackKey);

            var response = await _requestExecutor.Execute<T>(request);

            return response;
        }

        async Task<T> IApiClient.SendRequest<T>(string slackKey, KeyValuePair<string, string>[] parameters)
        {
            RequestPath path = RequestPath.GetRequestPath<T>();
            var request = new RestRequest(SendMessagePath + path.Path);
            request.AddParameter("token", slackKey);
            foreach (var parameter in parameters)
            {
                request.AddParameter(parameter.Key, parameter.Value);
            }

            var response = await _requestExecutor.Execute<T>(request);

            return response;
        }
    }
}
