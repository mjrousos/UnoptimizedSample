using Microsoft.WindowsAzure.Storage.Blob;

namespace ProfilePictureService.Repositories.Configuration
{
    public class StorageOptions
    {
        public string ImageStorageConnectionString { get; set; }
        public string ChecksumStorageConnectionString { get; set; }
        public string ImageContainerName { get; set; }
        public string ChecksumContainerName { get; set; }
    }
}
