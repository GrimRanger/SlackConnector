namespace SlackConnector.Connections.Clients.Api.Responces.History
{
    internal class DirectMessageConversationHistoryResponse : ApiResponceBase
    {
        public Models.Message[] Messages { get; set; }
    }
}
