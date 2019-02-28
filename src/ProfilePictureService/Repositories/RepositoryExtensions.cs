using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProfilePictureService.Repositories;
using ProfilePictureService.Repositories.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryExtensions
    {
        private const string StorageOptionsConfigSection = "StorageOptions";

        public static void AddProfilePictureRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IChecksumRepository, TableStorageChecksumRepository>();

            var configSection = configuration.GetSection(StorageOptionsConfigSection);
            services.Configure<StorageOptions>(options => configSection.Bind(options));

            switch (configSection["ImageRepositoryType"].ToUpperInvariant())
            {
                case "SQLSERVER":
                    services.AddSingleton<IImageRepository, SqlImageRepository>();
                    break;
                case "BLOBSTORAGE":
                    services.AddSingleton<IImageRepository, BlobStorageImageRepository>();
                    break;
            }
        }

        public static void AddProfilePictureRepositoryHeathChecks(this IHealthChecksBuilder hcBuilder, IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider())
            {
                var storageOptions = provider.GetRequiredService<IOptions<StorageOptions>>().Value;
                var configuration = provider.GetRequiredService<IConfiguration>();
                var configSection = configuration.GetSection(StorageOptionsConfigSection);

                hcBuilder.AddAzureTableStorage(storageOptions?.ChecksumStorageConnectionString, "checksumTable");

                switch (configSection["ImageRepositoryType"].ToUpperInvariant())
                {
                    case "SQLSERVER":
                        hcBuilder.AddSqlServer(storageOptions?.ImageStorageConnectionString, name: "imageStore");
                        break;
                    case "BLOBSTORAGE":
                        hcBuilder.AddAzureBlobStorage(storageOptions?.ImageStorageConnectionString, "imageBlobs");
                        break;
                }
            }
        }
    }
}
