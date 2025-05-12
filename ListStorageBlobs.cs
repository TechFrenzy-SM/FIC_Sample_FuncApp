using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobStorageFunc
{
    public class ListStorageBlobs
    {
        private readonly ILogger<StorageBlobAccess> _logger;
        public ListStorageBlobs(ILogger<StorageBlobAccess> logger)
        {
            _logger = logger;
        }

        [Function("ListBlobs")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string storageAccountName = "storagefictest";
            string containerName = "datacontainer1";

            //string tenantId = "0891528f-b789-41ef-9d10-895a44f5f624";
            string tenantId_1 = "6ded650c-53d2-4417-8ac4-4650b0d3f2e1";
            string appId = "83457ed8-19e7-4174-9dad-beb47cdd9630";
            string userAssignedClientId = "8a5df5bb-7e7e-4901-aa2a-a29621124a9d";
            string audience = "api://AzureADTokenExchange";

            Uri blobServiceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/");

            //DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions
            //{
            //    ManagedIdentityClientId = userAssignedClientId,
            //    TenantId = tenantId
            //};

            //DefaultAzureCredential credential = new DefaultAzureCredential(options);

            var managedIdentityCredential = new ManagedIdentityCredential(userAssignedClientId);
            ClientAssertionCredential assertion = new ClientAssertionCredential(
                tenantId_1,
                appId,
                async (token) =>
                {
                    var tokenRequestContext = new TokenRequestContext(new[] { $"{audience}/.default" });
                    var accessToken = await managedIdentityCredential.GetTokenAsync(tokenRequestContext);
                    _logger.LogInformation($"Access Token: {accessToken.Token}");
                    return accessToken.Token;
                });


            string containerEndpoint = $"{blobServiceUri}{containerName}";
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint), assertion);

            try
            {
                // Retrieve access-token
                //_logger.LogInformation("Retrieving Access-Token...");
                //TokenRequestContext tokenRequestContext = new TokenRequestContext(new[] { "https://storage.azure.com/.default" });
                //AccessToken token = await credential.GetTokenAsync(tokenRequestContext);
                //_logger.LogInformation($"Access Token: {token.Token}");
                //_logger.LogInformation($"Expires On: {token.ExpiresOn}");


                // List all blobs in the container
                _logger.LogInformation("Listing blobs...");
                var blobs = new List<string>();
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    blobs.Add(blobItem.Name);
                }
                return new OkObjectResult(blobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while listing blobs: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
