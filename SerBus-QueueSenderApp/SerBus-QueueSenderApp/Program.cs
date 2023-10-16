using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

internal class Program
{
    static string connectionString = "Endpoint=sb://jd1-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HrwlEJkaTiqmESiBQdozWUXsVblTula3R+ASbKR/sl4=";
    static string queueName = "demoqueue";

    static async Task Main()
    {
        ServiceBusClient client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(queueName);
        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
        while (true)
        {
            Console.WriteLine("Enter Message (exit to terminate): ");
            string m = Console.ReadLine();
            if (m == "exit")
                break;
            var msg = new ServiceBusMessage(m);
            msg.ApplicationProperties.Add("Author", "sandeep");
            msg.ApplicationProperties.Add("CreatedAt", DateTime.Now);

            //msg.TimeToLive = new TimeSpan(0, 0, 5);
            //msg.MessageId = msg.ToString();
            await sender.SendMessageAsync(msg);
            Console.WriteLine("Sent...");
        }
        await sender.DisposeAsync();
        await client.DisposeAsync();
    }

}