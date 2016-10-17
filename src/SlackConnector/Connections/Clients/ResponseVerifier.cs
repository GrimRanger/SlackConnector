using System.Net;
using Newtonsoft.Json;
using RestSharp;
using SlackConnector.Connections.Responses;
using SlackConnector.Exceptions;

namespace SlackConnector.Connections.Clients
{
    internal class ResponseVerifier : IResponseVerifier
    {
        public T VerifyResponse<T>(IRestResponse response) where T : class
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new CommunicationException($"Error occured while sending message '{response.StatusCode}'");
            }

            var result = JsonConvert.DeserializeObject(response.Content, typeof (T)) as StandardResponse;

            if (result?.Error != null && result.Error.Contains("_not_found"))
            {
                return null;
            }

            if (result != null && !result.Ok)
            {
                throw new CommunicationException($"Error occured while posting message '{result.Error}'");
            }

            return result as T;
        }
    }
}