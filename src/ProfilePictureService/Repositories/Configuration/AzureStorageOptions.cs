using Microsoft.WindowsAzure.Storage.Blob;

namespace ProfilePictureService.Repositories.Configuration
{
    public class AzureStorageOptions
    {
        public string StorageConnectionString { get; set; }
        public string BlobContainerName { get; set; }
        public string TableName { get; set; }
    }
}
