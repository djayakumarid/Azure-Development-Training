using Microsoft.Azure;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions;
using System;

internal class Program
{
    class Employee : TableEntity
    {
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public Employee() { }
        public Employee(string deptName, string empName, string email, int phoneNumber)
        {
            this.PartitionKey = deptName;
            this.RowKey = empName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
        }
    }

    private static void Main(string[] args)
    {
        string cs = "DefaultEndpointsProtocol=https;AccountName=jd1storageaccount;AccountKey=lppOseQmqnPm6HgV8qYKn8MnGXRH+N7H6pyE4yidihmAtQHcvHJGI8XYzhWie7ntDMBlXMhljJUn+AStt2TNJg==;EndpointSuffix=core.windows.net";

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cs);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable tableEmployee = tableClient.GetTableReference("employee");
        tableEmployee.CreateIfNotExists();
        Employee emp1 = new Employee("Training", "Emp1", "emp1@test.com", 1111);
        Employee emp2 = new Employee("Training", "Emp2", "emp2@test.com", 2222);
        Employee emp3 = new Employee("Development", "Emp1", "emp3@test.com", 33333);

        TableBatchOperation batchOperation = new TableBatchOperation();
        batchOperation.InsertOrReplace(emp1);
        batchOperation.InsertOrReplace(emp2);
        tableEmployee.ExecuteBatch(batchOperation);

        batchOperation = new TableBatchOperation();
        batchOperation.InsertOrReplace(emp3);
        tableEmployee.ExecuteBatch(batchOperation);

        TableOperation operation = TableOperation.Retrieve<Employee>("Development", "Emp1");
        TableResult result = tableEmployee.Execute(operation);
        emp3 = result.Result as Employee;

        //emp3.ETag = "*";
        batchOperation = new TableBatchOperation();
        batchOperation.Delete(emp3);
        tableEmployee.ExecuteBatch(batchOperation);

        TableQuery<Employee> query = new TableQuery<Employee>();
        string filter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, "emp1@test.com");
        query = query.Where(filter);
        var emps = tableEmployee.ExecuteQuery(query);
        foreach (Employee emp in emps)
        {
            Console.WriteLine(emp.PartitionKey + " " + emp.RowKey + " " + emp.Email);
        }
        Console.ReadLine();

    }
}