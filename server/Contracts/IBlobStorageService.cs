namespace server.Contracts
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream> DownloadAsync(string fileName);
        Task<bool> ExistsAsync(string fileName);
    }
}
