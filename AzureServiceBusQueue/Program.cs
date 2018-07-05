using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBusQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            //SendMessageToQueue();
            ReadWriteFromTopicSubscription();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void SendMessageToQueue()
        {
            var conn = ""; //put your service bus connection string
            var queue = "prp-azure-servicebus-queue";

            var queueClient = QueueClient.CreateFromConnectionString(conn, queue);
            var message = new BrokeredMessage($"This is a test message from AzureServiceBusQueue -{DateTime.Now.ToShortDateString()} ");
            queueClient.Send(message);
            ProcessMessageFromQueue(queueClient);
        }

        private static void ProcessMessageFromQueue(QueueClient queClient)
        {
            queClient.OnMessage(message =>
            {
                Console.WriteLine(message.GetBody<string>());
            });
        }

        private static void ReadWriteFromTopicSubscription()
        {
            var conn = "";//put your service bus connection string
            var topic = "prp-azure-servicebus-topic";
            
            var topicClient = TopicClient.CreateFromConnectionString(conn,topic);
           
            var nsm = NamespaceManager.CreateFromConnectionString(conn);
            if (!nsm.SubscriptionExists(topic, "AllMessages"))                
                nsm.CreateSubscription(topic, "AllMessages");

            topicClient.Send(new BrokeredMessage($"This is a test message from ReadWriteFromTopicSubscription - {DateTime.Now.ToString()}"));

            var subscriptionCLient = SubscriptionClient.CreateFromConnectionString(conn,topic, "AllMessages");
            ReadUsingTopicSubscription(subscriptionCLient);
        }

        private static void ReadUsingTopicSubscription(SubscriptionClient client)
        {
            OnMessageOptions options = new OnMessageOptions { AutoComplete = false };
            client.OnMessage(message =>
            {
                Console.WriteLine(message.GetBody<string>());
                message.Complete();
            },options);
         
        }
    }
}
