namespace HRManagement.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        Task DeleteFileAsync(string blobName, string containerName);
        Task<string> UploadFileAsync(IFormFile file, string fileName, string containerName);
        string GetTemporaryBlobUrl(string blobName, string containerName, int expiryMinutes = 30);

    }
}
