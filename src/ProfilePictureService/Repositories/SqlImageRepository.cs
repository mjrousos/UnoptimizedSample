using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProfilePictureService.Repositories.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProfilePictureService.Repositories
{
    public class SqlImageRepository : IImageRepository
    {
        private readonly ILogger<BlobStorageImageRepository> _logger;
        private readonly string _connectionString;
        private readonly string _tableName;

        public SqlImageRepository(ILogger<BlobStorageImageRepository> logger, IOptions<StorageOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var storageOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _connectionString = storageOptions.ImageStorageConnectionString;
            _tableName = storageOptions.ImageContainerName;

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var tableCreationQuery =
                    $@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE name='{_tableName}')

                    BEGIN
                        CREATE TABLE [dbo].[{_tableName}](
	                        [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	                        [Name] [nvarchar](20) NOT NULL UNIQUE,
	                        [Data] [varbinary](max) NOT NULL
                        )
                        CREATE UNIQUE NONCLUSTERED INDEX [NameIndex] ON [dbo].[{_tableName}]
                        (
                            [Name] ASC
                        )
                    END";

                var command = new SqlCommand(tableCreationQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public async Task<bool> DeleteAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name required", nameof(name));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var retrieveQuery = $@"DELETE FROM {_tableName} WHERE Name=@Name;";
                var command = new SqlCommand(retrieveQuery, connection);
                command.Parameters.AddWithValue("@Name", name);

                return await command.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }
        }

        public async Task<byte[]> GetAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name required", nameof(name));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var retrieveQuery = $@"SELECT Data FROM {_tableName} WHERE Name=@Name;";
                var command = new SqlCommand(retrieveQuery, connection);
                command.Parameters.AddWithValue("@Name", NormalizeName(name));

                return (await command.ExecuteScalarAsync().ConfigureAwait(false)) as byte[];
            }
        }

        public async Task SetAsync(string name, byte[] data)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name required", nameof(name));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var retrieveQuery = $@"MERGE {_tableName} WITH (HOLDLOCK) AS target
                                       USING (values (@Name, @Data)) AS source(Name, Data)
                                       ON target.Name = source.Name
                                       WHEN MATCHED THEN
                                           UPDATE SET Name = @Name
                                       WHEN NOT MATCHED THEN
                                           INSERT (Name, Data) VALUES (@Name, @Data);";
                var command = new SqlCommand(retrieveQuery, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Data", data);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
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
