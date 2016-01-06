using System;
using System.Text;
using System.Threading;
using RabbitEasyNetQ.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitEasyNetQ.Patterns
{
    public class PlainRabbit
    {
        public void Run()
        {
            // publish items in a background thread
            ThreadPool.QueueUserWorkItem(x => RunPublisher());

            // subscribe to a queue, process items
            //SetupSubscriber();
        }

        void RunPublisher(int numMessages = 1000)
        {
            var connFactory = new ConnectionFactory();
            connFactory.Uri = GlobalInfo.RabbitMqConnectionString;

            var conn = connFactory.CreateConnection();
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare("MyExchange", ExchangeType.Direct); // exchange name, exchange type
                channel.QueueDeclare("MyQueue", true, false, false, null);  // queue name, queue settings
                channel.QueueBind("MyQueue", "MyExchange", "My.Routing.Key");   // queue-exchange binding, routing key

                for (int i = 0; i < numMessages; i++)
                {
                    var messageString = "This is message #" + i;
                    var message = Encoding.UTF8.GetBytes(messageString);  // encoding, serialization
                    channel.BasicPublish("MyExchange", "My.Routing.Key", null, message);    // exchange name, routing key
                    Console.WriteLine("Published: " + messageString);
                    Thread.Sleep(1000);
                }
            }
        }

        void SetupSubscriber()
        {
            var connFactory = new ConnectionFactory();
            connFactory.Uri = GlobalInfo.RabbitMqConnectionString;

            var conn = connFactory.CreateConnection();
            var channel = conn.CreateModel();

            channel.ExchangeDeclare("MyExchange", ExchangeType.Direct);
            channel.QueueDeclare("MyQueue", true, false, false, null);
            channel.QueueBind("MyQueue", "MyExchange","My.Routing.Key");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, args) =>
            {
                var body = Encoding.UTF8.GetString(args.Body);
                Console.WriteLine("Received: " + body);
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("MyQueue", false, consumer);
        }
    }
}