using Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
namespace MockTaskQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            //var queueClient = new QueueClient("DefaultEndpointsProtocol=https;AccountName=ubotintegration;AccountKey=JVvbJEnpjLJSV6IqY6qVVBIxdHMPh9cD3eWsX67f1MGsZsVg8M8KObHh1fKqq9Zd3fqUw7S8A0Q6vShnPgPNlA==;EndpointSuffix=core.windows.net", "taskqueue");
            var queueClient = new QueueClient("DefaultEndpointsProtocol=https;AccountName=longnello;AccountKey=XpB3bxUOci9L7UjZji98sM4eA5ZBvNTUrRGChloTLeY8ldLRK2mb5pNxuqAoPjNBUYCdbKdcs5rq0v0YBA9Kbw==;EndpointSuffix=core.windows.net", "testax");
            var path = @"C:\Users\long.nguyentrong\Desktop\fileAMZ.txt";
            var url = System.IO.File.ReadAllText(path);
            var messaage = new
            {
                fileId = Guid.NewGuid().ToString(),
                fileUrl = url,
                function = "processTaxInvoice",
                requestId = Guid.NewGuid().ToString(),
            };
            var messageRequest = JsonConvert.SerializeObject(messaage);
            queueClient.SendMessage(messageRequest);
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
