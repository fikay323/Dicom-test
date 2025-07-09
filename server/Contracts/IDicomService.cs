namespace server.Contracts
{
    public interface IDicomService
    {
        Task<string> UploadDicomFileAsync(IFormFile? file);
        Task<string> GetHeaderValueAsync(string fileName, string tag);
        Task<Stream> RenderDicomAsPngAsync(string fileName);
    }
}
