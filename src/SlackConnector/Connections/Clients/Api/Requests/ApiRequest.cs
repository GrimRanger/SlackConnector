using System.Collections.Generic;
using SlackConnector.Connections.Clients.Api.Responces;

namespace SlackConnector.Connections.Clients.Api.Requests
{
    internal class ApiRequest<T> where T : ApiResponceBase
    {
        public readonly KeyValuePair<string, string>[] Parameters;

        public ApiRequest(params KeyValuePair<string, string>[]  parameters)
        {
            Parameters = parameters;
        }
    }
}
