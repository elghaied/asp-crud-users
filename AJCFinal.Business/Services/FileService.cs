using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJCFinal.Business.Abstractions;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace AJCFinal.Business.Services
{
    internal sealed class FileService : IFileService
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
       

        public FileService(IConfiguration configuration)
        {
            _storageConnectionString = configuration["Storage:ConnectionString"];
            _storageContainerName = configuration["Storage:Container"];
          
        }

        public async Task DeleteFromAzureAsync(string fileName)
        {
            BlobContainerClient blobContainerClient = new(_storageConnectionString, _storageContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task<byte[]> GetFromAzureAsync(string fileName)
        {
          
            BlobContainerClient blobContainer = new(_storageConnectionString, _storageContainerName);
            await blobContainer.CreateIfNotExistsAsync();

            BlobClient blobClient = blobContainer.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);

                return ms.ToArray();
            }

            return Array.Empty<byte>();
        }

        public async Task SendToAzureAsync(byte[] data, string fileName, string contentType)
        {
            BlobContainerClient blobContainer = new(_storageConnectionString, _storageContainerName);
            await blobContainer.CreateIfNotExistsAsync();

            BlobClient blobClient = blobContainer.GetBlobClient(fileName);

            using (var ms = new MemoryStream(data, false))
            {
                var blobHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType,
                };

                await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = blobHeaders });

            }
        }
    }
}
