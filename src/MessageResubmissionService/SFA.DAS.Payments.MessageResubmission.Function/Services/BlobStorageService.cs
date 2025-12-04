using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Payments.MessageResubmission.Function.Models;

namespace SFA.DAS.Payments.MessageResubmission.Function.Services
{
    public interface IBlobStorageService
    {
        Task<List<ServiceBusMessage>> GetServiceBusMessagesForReprocessing(ResubmitCompletionPaymentMessagesRequest request);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureWebJobsStorage"]);
        }

        public async Task<List<ServiceBusMessage>> GetServiceBusMessagesForReprocessing(ResubmitCompletionPaymentMessagesRequest request)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(request.BlobStorageContainerName);
            var blobClient = containerClient.GetBlobClient(request.FileName);

            if (!await blobClient.ExistsAsync())
            {
                throw new InvalidOperationException($"File not found: {request.FileName} in container {request.BlobStorageContainerName}");
            }

            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;

            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var serviceBusMessages = await JsonSerializer.DeserializeAsync<List<ServiceBusMessage>>(stream, serializerOptions);

            return serviceBusMessages;
        }
    }
}
