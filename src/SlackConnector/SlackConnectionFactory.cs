using SlackConnector.BotHelpers;
using SlackConnector.BotHelpers.Interfaces;
using SlackConnector.Connections;
using SlackConnector.Models;

namespace SlackConnector
{
    internal class SlackConnectionFactory : ISlackConnectionFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IChatHubInterpreter _chatHubInterpreter;
        private readonly IMentionDetector _mentionDetector;
        private readonly ISlackInfoSearcher _slackInfoSearcher;

        public SlackConnectionFactory()
            : this(new ConnectionFactory(), new ChatHubInterpreter(), new MentionDetector(), new SlackInfoSearcher())
        { }

        public SlackConnectionFactory(IConnectionFactory connectionFactory, IChatHubInterpreter chatHubInterpreter, IMentionDetector mentionDetector, ISlackInfoSearcher slackInfoSearcher)
        {
            _connectionFactory = connectionFactory;
            _chatHubInterpreter = chatHubInterpreter;
            _mentionDetector = mentionDetector;
            _slackInfoSearcher = slackInfoSearcher;
        }

        public ISlackConnection Create(ConnectionInformation connectionInformation)
        {
            var slackConnection = new SlackConnection(_connectionFactory, _chatHubInterpreter, _mentionDetector, _slackInfoSearcher);
            slackConnection.Initialise(connectionInformation);
            return slackConnection;
        }
    }
}