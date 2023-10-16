using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

internal class Program
{
    static BlobServiceClient? blobServiceClient;
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=jd1storageaccount;AccountKey=lppOseQmqnPm6HgV8qYKn8MnGXRH+N7H6pyE4yidihmAtQHcvHJGI8XYzhWie7ntDMBlXMhljJUn+AStt2TNJg==;EndpointSuffix=core.windows.net";  //Get this from Azure Portal
        blobServiceClient = new BlobServiceClient(connectionString);
        string file1 = "D:\\Users\\jayakumard\\Desktop\\Jay\\images\\img1.jpg";
        string file2 = "D:\\Users\\jayakumard\\Desktop\\Jay\\images\\img2.jpg";
        string file3 = "D:\\Users\\jayakumard\\Desktop\\Jay\\images\\img3.jpg";
        Console.ReadLine();

        Console.WriteLine(file1.ToString());
        Console.WriteLine(file2.ToString());
        Console.WriteLine(file3.ToString());
        Console.ReadLine();

        Console.WriteLine("Now creating private container...");
        var priContainer = CreateContainer("pri-images", false);
        Console.ReadLine();

        Console.WriteLine("Now creating public container...");
        var pubContainer = CreateContainer("pub-images", true);
        Console.ReadLine();

        Console.WriteLine("Now uploading files to private container...");
        UploadBlob(priContainer, file1);
        UploadBlob(priContainer, file2);
        Console.ReadLine();

        Console.WriteLine("Now uploading files to public container...");
        UploadBlob(pubContainer, file1);
        UploadBlob(pubContainer, file3);
        Console.ReadLine();

        Console.WriteLine("Now downloading files...");
        DownloadBlob(priContainer, "img1.jpg");
        DownloadBlob(priContainer, "img2.jpg");
        DownloadBlob(pubContainer, "img3.jpg");
        Console.ReadLine();

        Console.WriteLine("Listing all blobs in private container");
        ListBlobs(priContainer);
        Console.ReadLine();

        Console.WriteLine("Listing all blobs as Anonymous user");
        ListBlobsAsAnonymousUser("pub-images");
        Console.ReadLine();

        //Get Ad-hoc SASToken for a blob
        Console.WriteLine("Generate URL with Ad-hoc SAS Token");
        BlobClient blobClient = priContainer.GetBlobClient("img1.jpg");
        string url = CreateServiceSASforBlob(blobClient, null);
        Console.WriteLine(url);
        Console.ReadLine();

        //Get Stored Access Policy based SASToken for a blob
        Console.WriteLine("Generate Stored Access  Policy");
        string policyName = "TestPolicy-" + DateTime.Now.ToShortTimeString();
        CreateSharedAccessPolicy(priContainer, policyName);
        Console.ReadLine();

        Console.WriteLine("Generate SAS Token based on Stored Access Policy");
        url = CreateServiceSASforBlob(blobClient, policyName);
        Console.WriteLine(url);
        Console.WriteLine("******** END *************");
        Console.ReadLine();
    }

    static BlobContainerClient CreateContainer(string containerName, bool isPublic)
    {
        BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
        if (!container.Exists())
        {
            container.CreateIfNotExists();
            Console.WriteLine($"{containerName} is Created\n");
            if (isPublic)
                container.SetAccessPolicy(PublicAccessType.Blob);
            else
                container.SetAccessPolicy(PublicAccessType.None);
        }
        Console.WriteLine();
        return container;
    }

    static BlobClient UploadBlob(BlobContainerClient container, string path)
    {
        FileInfo fi = new FileInfo(path);
        BlobClient blobClient = container.GetBlobClient(fi.Name);
        blobClient.Upload(path, true);
        Console.WriteLine($"Access blob here  - {blobClient.Uri.AbsoluteUri}");
        Console.WriteLine();
        return blobClient;
    }

    static BlobClient DownloadBlob(BlobContainerClient container, string path)
    {
        FileInfo fi = new FileInfo(path);
        BlobClient blobClient = container.GetBlobClient(fi.Name);
        blobClient.DownloadTo("D:\\Users\\jayakumard\\Desktop\\Jay\\images\\Download\\" + path);
        Console.WriteLine();
        return blobClient;
    }

    static void ListBlobs(BlobContainerClient container)
    {
        Console.WriteLine($"List of Blobs in {container.Name} and {container.GetAccessPolicy().Value}");
        foreach (var blob in container.GetBlobs())
        {
            BlobClient blobClient = container.GetBlobClient(blob.Name);
            Console.WriteLine($"{blob.Name} - {blobClient.Uri}");
        }
        Console.WriteLine("");
    }

    static void ListBlobsAsAnonymousUser(string containerName)
    {
        BlobServiceClient blobClientForAnonymous = new BlobServiceClient(new Uri(@"https://jd1storageaccount.blob.core.windows.net"));
        BlobContainerClient container = blobClientForAnonymous.GetBlobContainerClient(containerName);
        foreach (var blob in container.GetBlobs())
        {
            BlobClient blobClient = container.GetBlobClient(blob.Name);
            Console.WriteLine($"{blob.Name} - {blobClient.Uri}");
        }
        Console.WriteLine("");
    }

    static string CreateServiceSASforBlob(BlobClient blobClient, string? storedPolicyName = null)
    {
        if (blobClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };
            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }
            Console.WriteLine();
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
        else
        {
            Console.WriteLine(@"BlobClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
            return null;
        }
    }
    static void CreateSharedAccessPolicy(BlobContainerClient container, string policyName)
    {
        try
        {
            List<BlobSignedIdentifier> signedIdentifiers = new List<BlobSignedIdentifier>();
            signedIdentifiers.Add(
                    new BlobSignedIdentifier
                    {
                        Id = policyName,
                        AccessPolicy = new BlobAccessPolicy
                        {
                            StartsOn = DateTimeOffset.UtcNow.AddHours(-1),
                            ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                            Permissions = "rw"
                        }
                    }
                );
            // Set the container's access policy.
            container.SetAccessPolicy(permissions: signedIdentifiers);
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine(e.ErrorCode);
            Console.WriteLine(e.Message);
        }
        Console.WriteLine();
    }

    static void DeleteBlobs(BlobContainerClient container)
    {
        foreach (var blob in container.GetBlobs())
        {
            container.DeleteBlob(blob.Name);
        }
        Console.WriteLine();
    }

    static void DeleteContainer(BlobContainerClient container)
    {
        container.Delete();
        Console.WriteLine();
    }

}