using System;
using System.Threading;
using EasyNetQ;
using RabbitEasyNetQ.Messages;

namespace RabbitEasyNetQ.Patterns
{
    public class EasyNetQPublishAndSubscribe
    {
        public void Run()
        {
            SetupSubscribe("warehouse", "A1");      // warehouse will do something
            SetupSubscribe("warehouse", "A2");      // scale the warehouse task
            SetupSubscribe("bank", "B");            // bank will do something
            SetupSubscribe("shipping", "C");        // UPS/FedEx will do something
            StartPublishing();
        }

        void SetupSubscribe(string subscriptionId, string name)
        {
            var bus = RabbitHutch.CreateBus(GlobalInfo.RabbitMqConnectionString);
            bus.Subscribe<MessageA>(subscriptionId, x =>
            {
                Console.WriteLine("Subscriber {0}({1}) received message: UserId: {2}, Price: {3}", subscriptionId, name, x.UserId, x.PurchasePrice);
            });
        }

        void StartPublishing()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus(GlobalInfo.RabbitMqConnectionString);
            while (true)
            {
                var messageA = new MessageA
                {
                    UserId = rand.Next(5000, 9999),
                    PurchasePrice = rand.Next(250, 1000) / 10M
                };
                // if there are no subscribers, this message will be hurtled into the void
                bus.Publish(messageA);
                Console.WriteLine("Published a message.");
                Thread.Sleep(1000);
            }
        }

    }
}