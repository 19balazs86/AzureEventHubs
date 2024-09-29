using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace EventHubsWorker.Workers;

/*
 * When generating a SAS (Shared Access Signature) token for an Azure Blob file, there are two signing methods
 * 1) Account Access Key: use this when you have the full connection string that includes the AccountKey
 * 2) User Delegation Key: use this when creating the client with DefaultAzureCredential
 *    To use the UserDelegationKey, you must have the Storage Blob Data Contributor role
 */
public sealed class BlobSasWorker(BlobServiceClient blobServiceClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Uri uploadUrl = await getUploadUrl_1_AccountKey();

        await uploadWithHttpClient(uploadUrl);

        //Uri uploadUrl = await getUploadUrl_2_UserDelegationKey();

        //await uploadWithHttpClient(uploadUrl);
    }

    // Dev Leader: https://youtu.be/vW7KDz6wNIs
    private async Task<Uri> getUploadUrl_1_AccountKey()
    {
        BlobClient blobClient = await getBlobClient();

        return blobClient.GenerateSasUri(BlobSasPermissions.Write, DateTime.UtcNow.AddMinutes(5));
    }

    // Create a user delegation SAS for a container or blob
    // https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-user-delegation-sas-create-dotnet
    private async Task<Uri> getUploadUrl_2_UserDelegationKey()
    {
        BlobClient blobClient = await getBlobClient();

        UserDelegationKey userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

        var blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName          = blobClient.Name,
            // Resource       = "b", // b for Blob | c for Container | It is not necessary to set, as it gets its value in the EnsureState method
            // Identifier     = "TestPolicy" // You can use a predefined policy and omit the StartsOn and ExpiresOn
            StartsOn          = DateTimeOffset.UtcNow,
            ExpiresOn         = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        blobSasBuilder.SetPermissions(BlobSasPermissions.Write); // (BlobSasPermissions.Read | BlobSasPermissions.Write)

        var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
        {
            Sas = blobSasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName)
        };

        return blobUriBuilder.ToUri();
    }

    private static async Task uploadWithHttpClient(Uri uploadUrl)
    {
        using var httpClient = new HttpClient();

        using HttpContent httpContent = new StringContent("Hello World!");

        httpContent.Headers.Add("x-ms-blob-type", "BlockBlob"); // This header must present
        httpContent.Headers.Add("x-ms-meta-MyDescription", "Sample file upload");

        using HttpResponseMessage response = await httpClient.PutAsync(uploadUrl, httpContent);

        response.EnsureSuccessStatusCode();
    }

    private async Task<BlobClient> getBlobClient()
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("blobtest");

        await blobContainerClient.CreateIfNotExistsAsync();

        return blobContainerClient.GetBlobClient($"{Guid.NewGuid():N}.txt");
    }
}
