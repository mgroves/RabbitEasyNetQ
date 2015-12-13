using System;
using System.Threading;
using EasyNetQ;
using RabbitEasyNetQ.Messages;

namespace RabbitEasyNetQ.Patterns
{
    public class EasyNetQRequestAndResponse
    {
        public void Run()
        {
            SetupResponse();
            MakeRequests();
        }

        void SetupResponse()
        {
            var bus = RabbitHutch.CreateBus(GlobalInfo.RabbitMqConnectionString);
            bus.Respond<MyRequest, MyResponse>(x =>
            {
                Thread.Sleep(1000);
                return new MyResponse
                {
                    Sum = x.Addend1 + x.Addend2
                };
            });
        }

        void MakeRequests()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus(GlobalInfo.RabbitMqConnectionString);
            while (true)
            {
                var request = new MyRequest
                {
                    Addend1 = rand.Next(1,10),
                    Addend2 = rand.Next(1,10)
                };

                // **** synchronous version
                // if there is no process to receive requests, this will timeout
                Console.Write("Request: {0} + {1} = ", request.Addend1, request.Addend2);
                var response = bus.Request<MyRequest, MyResponse>(request);
                Console.WriteLine(response.Sum);
                // ****

                // ****asynchronous version
//                Console.WriteLine("Sending a request. {0} + {1} = ?", request.Addend1, request.Addend2);
//                var task = bus.RequestAsync<MyRequest, MyResponse>(request);
//                task.ContinueWith(response =>
//                {
//                    Console.WriteLine("{0} + {1} = {2}", request.Addend1, request.Addend2, response.Result.Sum);
//                });
                // ****

                Thread.Sleep(1000);
            }
        }
    }

    public class MyRequest
    {
        public int Addend1 { get; set; }
        public int Addend2 { get; set; }
    }

    public class MyResponse
    {
        public int Sum { get; set; }
    }
}