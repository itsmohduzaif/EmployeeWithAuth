//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using Azure.Storage.Sas;
//using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;

//public class BlobStorageServiceForLeaveRequestFiles
//{
//    private readonly string _connectionString;
//    //private readonly string _containerName;

//    public BlobStorageServiceForLeaveRequestFiles(IConfiguration configuration)
//    {
//        _connectionString = configuration["AzureBlobStorage:ConnectionString"];
//        //_containerName = configuration["AzureBlobStorage:LeaveRequestFilesContainerName"];
//    }

//    public async Task DeleteFileAsync(string blobName, string _containerName)
//    {
//        var containerClient = new BlobContainerClient(_connectionString, _containerName);
//        var blobClient = containerClient.GetBlobClient(blobName);
//        //await blobClient.DeleteIfExistsAsync();
//        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots); //Added this so that snpashots also get deleted
//    }

//    public async Task<string> UploadFileAsync(IFormFile file, string fileName, string _containerName)
//    {
//        BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);
//        await containerClient.CreateIfNotExistsAsync();

//        BlobClient blobClient = containerClient.GetBlobClient(fileName);

//        var blobHttpHeaders = new BlobHttpHeaders
//        {
//            ContentType = file.ContentType // e.g., "image/jpeg"
//        };

//        using (var stream = file.OpenReadStream())
//        {
//            await blobClient.UploadAsync(stream, new BlobUploadOptions
//            {
//                HttpHeaders = blobHttpHeaders
//            });
//        }
//        return fileName;
//    }

//    public string GetTemporaryBlobUrl(string blobName, string _containerName, int expiryMinutes = 30)
//    {
//        var containerClient = new BlobContainerClient(_connectionString, _containerName);
//        var blobClient = containerClient.GetBlobClient(blobName);

//        if (!blobClient.CanGenerateSasUri)
//        {
//            throw new InvalidOperationException("SAS token generation not permitted. Check storage permissions.");
//        }

//        var sasBuilder = new BlobSasBuilder
//        {
//            BlobContainerName = _containerName,
//            BlobName = blobName,
//            Resource = "b",
//            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
//        };

//        sasBuilder.SetPermissions(BlobSasPermissions.Read);

//        var sasUri = blobClient.GenerateSasUri(sasBuilder);
//        return sasUri.ToString();
//    }
//}


////| Method on `blobClient` | What it does                                    |
////| ---------------------- | ----------------------------------------------- |
////| `UploadAsync()`        | Uploads data to this file                       |
////| `DownloadAsync()`      | Downloads the file                              |
////| `DeleteAsync()`        | Deletes the file                                |
////| `ExistsAsync()`        | Checks if the file exists                       |
////| `GenerateSasUri()`     | Generates a secure URL to access it temporarily |
////| `OpenReadAsync()`      | Opens a read-only stream to read from the blob  |



//// If we want to save the file directly with the link
////public async Task<string> UploadFileAsync(IFormFile file, string fileName)
////{
////    BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);
////    await containerClient.CreateIfNotExistsAsync();

////    BlobClient blobClient = containerClient.GetBlobClient(fileName);

////    using (var stream = file.OpenReadStream())
////    {
////        await blobClient.UploadAsync(stream, overwrite: true);
////    }

////    return fileName; // Return blob name, not full URL
////}
