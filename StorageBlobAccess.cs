using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BlobStorageFunc
{
    public class StorageBlobAccess
    {
        private readonly ILogger<StorageBlobAccess> _logger;

        public StorageBlobAccess(ILogger<StorageBlobAccess> logger)
        {
            _logger = logger;
        }

        [Function("StorageBlobAccess") ]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string storageAccountName = "storageforreactapp";
            string containerName = "datacontainer1";
            string userAssignedClientId = "8a5df5bb-7e7e-4901-aa2a-a29621124a9d";
            string tenantId = "0891528f-b789-41ef-9d10-895a44f5f624";

            Uri blobServiceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/");

            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions { 
                ManagedIdentityClientId = userAssignedClientId,
                TenantId = tenantId
            };

            DefaultAzureCredential credential = new DefaultAzureCredential(options);
            string containerEndpoint = $"{blobServiceUri}{containerName}";
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint), credential);

            try
            {
                // Retrieve access-token
                _logger.LogInformation("Retrieving Access-Token...");
                TokenRequestContext tokenRequestContext = new TokenRequestContext(new[] { "https://storage.azure.com/.default" });
                AccessToken token = await credential.GetTokenAsync(tokenRequestContext);
                _logger.LogInformation($"Access Token: {token.Token}");
                _logger.LogInformation($"Expires On: {token.ExpiresOn}");


                // List all blobs in the container
                _logger.LogInformation("Listing blobs...");
                var blobs = new List<string>();
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    blobs.Add(blobItem.Name);
                }
                return new OkObjectResult(blobs);
            }
            catch (Exception ex) {
                _logger.LogError($"An error occurred while listing blobs: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
