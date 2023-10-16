using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

class Program
{
    static string connectionString = "Endpoint=sb://jd1-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HrwlEJkaTiqmESiBQdozWUXsVblTula3R+ASbKR/sl4=";
    static string queueName = "demoqueue";
    static string deadLetterQueueName = "demoqueue/$deadletterqueue";
    static ServiceBusClient client;
    static ServiceBusProcessor processor, deadLetterProcessor;
    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}, Count: {args.Message.DeliveryCount}");
        Console.WriteLine(args.Message.ApplicationProperties["CreatedAt"]);
        await args.CompleteMessageAsync(args.Message); //Delete the message from the queue
    }
    static async Task DeadLetterMessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Dead Message: {body}");
        Console.WriteLine(args.Message.ApplicationProperties["CreatedAt"]);
        await args.CompleteMessageAsync(args.Message);
    }
    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
    static async Task Main()
    {
        client = new ServiceBusClient(connectionString);
        var options = new ServiceBusProcessorOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock, //ReceiveAndDelete
            AutoCompleteMessages = false, //If true., the message is automatically deleted after successful execution of Message handler.
                                          // SessionIds = { "MySession" }
        };

        processor = client.CreateProcessor(queueName, options);
        deadLetterProcessor = client.CreateProcessor(deadLetterQueueName, options);
        processor.ProcessMessageAsync += MessageHandler;
        deadLetterProcessor.ProcessMessageAsync += DeadLetterMessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;
        deadLetterProcessor.ProcessErrorAsync += ErrorHandler;

        await processor.StartProcessingAsync();
        await deadLetterProcessor.StartProcessingAsync();
        Console.WriteLine("Wait for a minute and then press any key to end the processing");
        Console.ReadLine();
        // stop processing
        Console.WriteLine("\nStopping the receiver...");
        await processor.StopProcessingAsync();
        await deadLetterProcessor.StopProcessingAsync();

        Console.WriteLine("Stopped receiving messages");
        await processor.DisposeAsync();
        await deadLetterProcessor.DisposeAsync();
        await client.DisposeAsync();
    }
}
