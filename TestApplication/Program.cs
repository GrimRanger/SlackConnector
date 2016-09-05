using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace Test
{
    class Program
    {
        private static Tester tester;
        static void Main(string[] args)
        {
            tester = new Tester("xoxb-58920871859-7CQkuNtclvv7z2NCEAySoQHf");
            tester.Start();
            tester.MessageReceived += OnMessageReceived;
            tester.TestApi();

            while (true)
            { }
        }

        private static void OnMessageReceived(object sender, SlackMessage e)
        {
            tester.SendData(new BotMessage {Text = "test", ChatHub = e.ChatHub});
        }
    }
}
