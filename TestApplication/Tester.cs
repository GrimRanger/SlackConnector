using System;
using System.Threading.Tasks;
using SlackConnector;
using SlackConnector.Models;

namespace TestApplication
{
    class Tester
    {
        private bool _firstStart;
        private readonly ISlackConnector _slackBot;
        private readonly string _slackToken;
        private ISlackConnection _connection;
        public bool IsConected => _connection != null && _connection.IsConnected;

        public event EventHandler<SlackMessage> MessageReceived;
        public event EventHandler DisconnectHandler;


        public Tester(string slackToken)
        {
            _firstStart = true;
            _slackToken = slackToken;
            _slackBot = new SlackConnector.SlackConnector();
        }

        public  void Start()
        {
            try
            {
                _connection = _slackBot.Connect(_slackToken).Result;
                if (!_firstStart)
                    return;

                _connection.OnMessageReceived += OnMessageReceivedEventHandler;
                _connection.OnDisconnect += OnDisconnectEventHandler;
                _firstStart = false;
            }

            catch (Exception ex)
            {
                OnErrorEventHandler(this, ex, "An error occurred while trying to connect.");
            }
        }

        public void Stop()
        {
            try
            {
                _connection.Disconnect();
            }

            catch (Exception ex)
            {
                OnErrorEventHandler(this, ex, "An error occurred while trying to disconnect.");
            }
        }

        public async void SendData(BotMessage message)
        {
            try
            {
                await _connection.Say(message);
            }
            catch (Exception ex)
            {
                OnErrorEventHandler(this, ex, "An error occurred while trying to sending data");
            }
        }

        public SlackUser FindUserById(string id)
        {
            if (_connection.UserCache.ContainsKey(id))
                return _connection.UserCache[id];

            return null;
        }

        private void OnDisconnectEventHandler()
        {
            DisconnectHandler?.Invoke(this, EventArgs.Empty);
        }

        private Task OnMessageReceivedEventHandler(SlackMessage message)
        {
            return Task.Run(() => MessageReceived?.Invoke(null, message));
        }

        private void OnErrorEventHandler(object sender, Exception ex, string message)
        {
        }
    }
}