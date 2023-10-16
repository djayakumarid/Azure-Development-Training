using Azure.Storage.Queues;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("*************** Hello, Demo Post Queue! **************");
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=jd1storageaccount;AccountKey=lppOseQmqnPm6HgV8qYKn8MnGXRH+N7H6pyE4yidihmAtQHcvHJGI8XYzhWie7ntDMBlXMhljJUn+AStt2TNJg==;EndpointSuffix=core.windows.net";  //Get this from Azure Portal
        string queueName = "demo-input-queue";
        QueueClient queueClient = new QueueClient(connectionString, queueName);
        queueClient.CreateIfNotExists();
        while (true)
        {
            Console.Write("Enter a message to be sent to myqueue:");
            var msg = Console.ReadLine();
            if (msg == "exit")
                break;
            queueClient.SendMessage(msg);
        }
    }
}