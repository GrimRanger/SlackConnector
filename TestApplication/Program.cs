using SlackConnector.Models;

namespace TestApplication
{
    class Program
    {
        private static Tester _tester;
        static void Main()
        {
            _tester = new Tester("xoxb-58920871859-7CQkuNtclvv7z2NCEAySoQHf");
            _tester.Start();
            _tester.MessageReceived += OnMessageReceived;
            _tester.Test();
            while (true)
            { }
        }

        private static void OnMessageReceived(object sender, SlackMessage e)
        {
            //tester.SendData(new BotMessage {Text = "test", ChatHub = e.ChatHub});
        }
    }
}
