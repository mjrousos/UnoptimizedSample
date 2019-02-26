using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ProfilePictureService.Models;
using ProfilePictureService.Repositories.Configuration;
using System;
using System.Threading.Tasks;

namespace ProfilePictureService.Repositories
{
    public class TableStorageChecksumRepository : IChecksumRepository
    {
        private bool _tableInitialized;
        private readonly CloudTable _tableReference;

        private ILogger<TableStorageChecksumRepository> Logger { get; }

        private async Task<CloudTable> GetTable()
        {
            if (!_tableInitialized)
            {
                await _tableReference.CreateIfNotExistsAsync().ConfigureAwait(false);
                _tableInitialized = true;
            }

            return _tableReference;
        }

        public TableStorageChecksumRepository(ILogger<TableStorageChecksumRepository> logger, IOptions<AzureStorageOptions> options)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var azureOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _tableReference = CloudStorageAccount.Parse(azureOptions.StorageConnectionString).CreateCloudTableClient().GetTableReference(azureOptions.TableName);
        }

        public async Task<string> GetAsync(string name)
        {
            var table = await GetTable().ConfigureAwait(false);
            var result = await table.ExecuteAsync(TableOperation.Retrieve<ChecksumEntity>(string.Empty, name)).ConfigureAwait(false);

            return (result.Result as ChecksumEntity)?.Data;
        }

        public async Task SetAsync(string name, string data)
        {
            var table = await GetTable().ConfigureAwait(false);
            var upsetOperation = TableOperation.InsertOrReplace(new ChecksumEntity(name, data));
            await table.ExecuteAsync(upsetOperation).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(string name)
        {
            var table = await GetTable().ConfigureAwait(false);
            var retrieveOperation = TableOperation.Retrieve<ChecksumEntity>(string.Empty, name);
            var retrieveResult = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            if (retrieveResult.Result is ChecksumEntity entity)
            {
                var deleteOperation = TableOperation.Delete(entity);
                var deleteResult = await table.ExecuteAsync(deleteOperation).ConfigureAwait(false);
                return (deleteResult.HttpStatusCode / 100 == 2);
            }
            else
            {
                return false;
            }
        }

    }
}
