using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DemoFunctionApp
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Function1
    {
        [FunctionName("ProcessEmpResumes")]
        public void Run(
        [QueueTrigger("employees", Connection = "AzureWebJobsStorage")] Employee emp,
        [Blob("resumes/{id}.txt", FileAccess.Read)] string resume,
        [Blob("resumes/{name}.txt", FileAccess.Write)] out string newfile,
        ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {emp.Id} - {emp.Name}");
            newfile = "This is info about person with id=" + emp.Id;
            log.LogInformation(resume);
        }

    }
}
