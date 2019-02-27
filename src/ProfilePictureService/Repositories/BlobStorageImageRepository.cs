using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ProfilePictureService.Repositories.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProfilePictureService.Repositories
{
    public class BlobStorageImageRepository : IImageRepository
    {
        private bool _blobContainerInitialized;
        private readonly CloudBlobContainer _blobContainer;

        private ILogger<BlobStorageImageRepository> Logger { get; }

        private async Task<CloudBlobContainer> GetBlobContainer()
        {
            if (!_blobContainerInitialized)
            {
                if (await _blobContainer.CreateIfNotExistsAsync().ConfigureAwait(false))
                {
                    await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off,
                    })
                    .ConfigureAwait(false);
                }

                _blobContainerInitialized = true;
            }

            return _blobContainer;
        }

        public BlobStorageImageRepository(ILogger<BlobStorageImageRepository> logger, IOptions<AzureStorageOptions> options)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var azureOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _blobContainer = CloudStorageAccount.Parse(azureOptions.StorageConnectionString).CreateCloudBlobClient().GetContainerReference(azureOptions.BlobContainerName);
        }

        public async Task<bool> DeleteAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Blob name required", nameof(name));
            }

            var container = await GetBlobContainer().ConfigureAwait(false);
            var blob = container.GetBlobReference(name);
            return await blob.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        public async Task<byte[]> GetAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Blob name required", nameof(name));
            }

            name = NormalizeName(name);

            var container = await GetBlobContainer().ConfigureAwait(false);
            var blob = container.GetBlockBlobReference(name);
            if (!await blob.ExistsAsync().ConfigureAwait(false))
            {
                return null;
            }

            using (var downloadStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(downloadStream).ConfigureAwait(false);
                return downloadStream.ToArray();
            }
        }

        public async Task SetAsync(string name, byte[] data)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Blob name required", nameof(name));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var container = await GetBlobContainer().ConfigureAwait(false);
            var blob = container.GetBlockBlobReference(name);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        #region Hacky Demo Enablement Code
        const string TestUserName = "1";
        const int MaxUsers = 50000;
        private static string NormalizeName(string name)
        {
            // This method is a hack to minimize the amount of 'fake' Azure storage I need
            // for this scenario! Please assume that in production this would be a no-op
            if (int.TryParse(name, out var nameAsInt) && 0 < nameAsInt && nameAsInt < MaxUsers)
            {
                return TestUserName;
            }

            return name;
        }
        #endregion
    }
}
