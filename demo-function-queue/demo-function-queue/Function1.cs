using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace demo_function_queue
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([QueueTrigger("demo-input-queue", Connection = "AzureWebJobsStorage")] string myQueueItem,
             [Queue("demo-output-queue", Connection = "AzureWebJobsStorage")] out string strOutput,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            strOutput = $"Write to output queue={myQueueItem}";
        }
    }
}
