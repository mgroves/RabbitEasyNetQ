using System;
using System.Threading;
using EasyNetQ;
using RabbitEasyNetQ.Messages;

namespace RabbitEasyNetQ.Patterns
{
    public class EasyNetQSendAndReceive
    {
        public void Run()
        {
            SetupReceive();
            StartSending();
        }

        void SetupReceive()
        {
            var bus = RabbitHutch.CreateBus(GlobalInfo.RabbitMqConnectionString);
            bus.Receive("My.Queue", x => x
                .Add<MessageA>(m =>
                {
                    Console.WriteLine("Received MessageA: UserId: {0}, Purchase Price: {1}", m.UserId, m.PurchasePrice);
                }));
            bus.Receive("My.Queue", x=> x
                .Add<MessageB>(m =>
                {
                    Console.WriteLine("Received MessageB: ItemNumber: {0}, Ship to Address: {1}", m.ItemNumber, m.ShipToAddress);
                }));
        }

        void StartSending()
        {
            var bus = RabbitHutch.CreateBus(GlobalInfo.RabbitMqConnectionString);
            var rand = new Random();
            while (true)
            {
                var messageA = new MessageA
                {
                    UserId = rand.Next(5000,9999),
                    PurchasePrice = rand.Next(250,1000) / 10M
                };
                var messageB = new MessageB
                {
                    ShipToAddress = string.Format("{0} {1} St.", rand.Next(100,1000), rand.RandomString()),
                    ItemNumber = rand.Next(100000,200000)
                };

                // these messages will be put in the queue even if there aren't any
                // receivers yet
                bus.Send("My.Queue", messageA);
                Console.WriteLine("Sent message A.");
                Thread.Sleep(1000);

                bus.Send("My.Queue", messageB);
                Console.WriteLine("Sent message B.");
                Thread.Sleep(1000);
            }
        }
    }
}