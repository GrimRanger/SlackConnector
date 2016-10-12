using System.Threading.Tasks;
using RestSharp;
using SlackConnector.Connections.Clients.Api.Requests;
using SlackConnector.Connections.Clients.Api.Responces;

namespace SlackConnector.Connections.Clients.Api.LowLevelClient
{
    internal class LowLevelApiClient : ILowLevelApiClient
    {
        private readonly IRequestExecutor _requestExecutor;
        private const string SendMessagePath = "/api/";

        public LowLevelApiClient(IRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<T> SendRequest<T>(string slackKey, ApiRequest<T> apiRequest) where T : ApiResponceBase
        {
            RequestPath path = RequestPath.GetRequestPath<T>();
            var request = new RestRequest(SendMessagePath + path.Path);
            request.AddParameter("token", slackKey);
            foreach (var parameter in apiRequest.Parameters)
            {
                request.AddParameter(parameter.Key, parameter.Value);
            }

            var response = await _requestExecutor.Execute<T>(request);

            return response;
        }
    }
}
