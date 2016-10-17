using RestSharp;

namespace SlackConnector.Connections.ClientFactories
{
    internal class RestSharpFactory : IRestSharpFactory
    {
        public IRestClient CreateClient(string baseUrl)
        {
            return new RestClient(baseUrl);
        }
    }
}