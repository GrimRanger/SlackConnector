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
        static void Main(string[] args)
        {
            var tester = new Tester("xoxb-58920871859-7CQkuNtclvv7z2NCEAySoQHf");
            tester.Start();
            tester.MessageReceived += OnMessageReceived;

            while(true)
            { }
        }

        private static void OnMessageReceived(object sender, SlackMessage e)
        {
            Console.WriteLine(e);
        }
    }
}
