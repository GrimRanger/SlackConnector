using RestSharp;

namespace SlackConnector.Connections.ClientFactories
{
    internal interface IRestSharpFactory
    {
        IRestClient CreateClient(string baseUrl);
    }
}