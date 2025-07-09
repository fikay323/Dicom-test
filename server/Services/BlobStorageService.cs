using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using server.Contracts;


namespace server.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            var conn = config["AzureBlobStorage:ConnectionString"];
            var containerName = config["AzureBlobStorage:ContainerName"];

            _container = new BlobContainerClient(conn, containerName);
            _container.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            return fileName;
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            var blob = _container.GetBlobClient(fileName);
            if (!await blob.ExistsAsync()) throw new FileNotFoundException("Blob not found");
            var response = await blob.DownloadAsync();
            return response.Value.Content;
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            return await _container.GetBlobClient(fileName).ExistsAsync();
        }
    }
}
