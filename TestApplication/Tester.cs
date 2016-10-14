using System;
using System.Linq;
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

        protected void OnErrorEventHandler(object sender, Exception ex, string message)
        {
        }

        public void Test()
        {
            var slackHub = _connection.ConnectedHubs.FirstOrDefault();
            var text =
                "{\"type\":\"mpim_joined\",\"channel\":{\"id\":\"G2PDUCQK0\",\"name\":\"mpdm-commander--alexeyo--archiver--kirillv-1\",\"is_group\":true,\"created\":1476451460,\"creator\":\"U1P6RLGJG\",\"is_archived\":false,\"is_mpim\":true,\"is_open\":false,\"last_read\":\"0000000000.000000\",\"latest\":null,\"unread_count\":0,\"unread_count_display\":0,\"members\":[\"U1P6RLGJG\",\"U1QT2RMR9\",\"U1R8NMKTP\",\"U0FC1HP61\"],\"topic\":{\"value\":\"Group messaging\",\"creator\":\"U1P6RLGJG\",\"last_set\":1476451460},\"purpose\":{\"value\":\"Group messaging with: @commander @alexeyo @archiver @kirillv\",\"creator\":\"U1P6RLGJG\",\"last_set\":1476451460}}}";
            var t = _connection.Test(text);
        }
    }
}