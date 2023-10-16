using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        /*
        string tenantId = "9a2b4fd4-c9d2-4e05-82d5-63405d8e2a1f";
        string clientId = "aa12fe23-0e2d-4987-ab05-b5aee4049854";
        string clientSecret = "ETB8Q~F-sg4WsHUwtIfsOaV6CGG7GfKM.1K4Za5e";
        */

        // Replace with your Key Vault URL and secret name
        string keyVaultUrl = "https://jd1-key-vault.vault.azure.net/";
        string secretName = "DemoSecret1";

        /*
        // Create a new ClientSecretCredential using the service principal credentials
        var credentials = new ClientSecretCredential(tenantId, clientId, clientSecret);

        // Create a new SecretClient using the ClientSecretCredential
        var client = new SecretClient(new Uri(keyVaultUrl), credentials);
        */

        // var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

        var credential = new ManagedIdentityCredential("be3c4b05-5bb0-499b-a344-af7f963bb90c");

        // Create a new SecretClient using DefaultAzureCredential
        var client = new SecretClient(new Uri(keyVaultUrl), credential);

        // Get the secret value
        KeyVaultSecret secret = await client.GetSecretAsync(secretName);

        // Access the secret value
        string secretValue = secret.Value;

        Console.WriteLine($"Secret value: {secretValue}");

    }
}