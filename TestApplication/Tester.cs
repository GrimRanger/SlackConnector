using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackConnector;
using SlackConnector.Connections.Clients.Api;
using SlackConnector.Models;

namespace Test
{
    class Tester
    {
        private bool _firstStart;
        private readonly ISlackConnector _slackBot;
        private ISlackConnection _connection;

        public string SlackToken { get; }
        public bool IsConected => _connection != null && _connection.IsConnected;

        public event EventHandler<SlackMessage> MessageReceived;
        public event EventHandler DisconnectHandler;
        public event EventHandler<ErrorEventArgs> ErrorHandler;


        public Tester(string slackToken)
        {
            _firstStart = true;
            SlackToken = slackToken;
            _slackBot = new SlackConnector.SlackConnector();
        }

        public  void Start()
        {
            try
            {
                _connection = _slackBot.Connect(SlackToken).Result;
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

        public void TestApi()
        {
            _connection.SendApi("channels.list");
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

        public string FindUserById(string id)
        {
            if (_connection.UserNameCache.ContainsKey(id))
                return _connection.UserNameCache[id];

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

        protected void OnErrorEventHandler(object sender, Exception ex, string message)
        {
        }
    }
}
