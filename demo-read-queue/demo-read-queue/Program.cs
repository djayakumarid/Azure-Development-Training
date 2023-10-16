using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("***************** Hello, World from Demo Read Queue");
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=jd1storageaccount;AccountKey=lppOseQmqnPm6HgV8qYKn8MnGXRH+N7H6pyE4yidihmAtQHcvHJGI8XYzhWie7ntDMBlXMhljJUn+AStt2TNJg==;EndpointSuffix=core.windows.net";  //Get this from Azure Portal
        string queueName = "demo-output-queue";
        QueueClient queueClient = new QueueClient(connectionString, queueName);
        queueClient.CreateIfNotExists();

        while (true)
        {
            QueueMessage[] retrievedMessages = queueClient.ReceiveMessages(1); //Fetches only one message from queue (visibilitytimeout=30)
            if (retrievedMessages.Length == 0)
            {
                Console.WriteLine("No Messages, will try again in 5 seconds");
                System.Threading.Thread.Sleep(5000);
                continue;
            }
            foreach (var msg in retrievedMessages)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetString(msg.Body.ToArray());
                Console.WriteLine($"Message: '{Convert.FromBase64String(plainTextBytes)}' - {msg.DequeueCount}");
                Console.WriteLine($"Message: '{plainTextBytes}' - {msg.DequeueCount}");
                // queueClient.DeleteMessage(msg.MessageId, msg.PopReceipt);
            }
        }
    }
}