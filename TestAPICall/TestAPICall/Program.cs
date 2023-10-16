using System.Net.Http;
using System.Web;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("************* Hello, World! TestAPICall");

        
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "77cfa669f430406e8c9a338a3587868a");
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://jd1-api-mgmt.azure-api.net/topic/2")
            };
            using (var response = client.SendAsync(request).Result)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            };

        Console.WriteLine("************* End TestAPICall");
        Console.ReadLine();
    }
}