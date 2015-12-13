using System;
using System.IO;
using RabbitEasyNetQ.Patterns;

namespace RabbitEasyNetQ.Messages
{
    class Program
    {
        static void Main(string[] args)
        {
            // note that in this example code, the publishing and subscribing
            // is all done within one process for demonstration purposes
            // subscribers should typically be run in a separate process
            // and probably on a separate machine to correctly leverage queueing

            // uncomment one of these at a time to demonstrate

            // use the plain RabbitMQ .NET API
            //new PlainRabbit().Run();

            // EasyNetQ Send & Receive
            //new EasyNetQSendAndReceive().Run();

            // EasyNetQ Publish & Subscribe
            //new EasyNetQPublishAndSubscribe().Run();

            // EasyNetQ Request & Response
            //new EasyNetQRequestAndResponse().Run();

            Console.ReadLine();
        }
    }

    public static class GlobalInfo
    {
        public static string RabbitMqConnectionString
        {
            get { return "amqp://guest:guest@localhost"; }
        }
    }

    public static class RandomExtensions
    {
        public static string RandomString(this Random @this, int size = 8)
        {
            return Path.GetRandomFileName().Replace(".", "").Substring(0, size);
        }
    }
}
