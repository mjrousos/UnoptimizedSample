using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProfilePictureService.Repositories;
using ProfilePictureService.Repositories.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryExtensions
    {
        private const string AzureStorageOptionsConfigSection = "AzureStorageOptions";

        public static void AddProfilePictureRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IImageRepository, BlobStorageImageRepository>();
            services.AddSingleton<IChecksumRepository, TableStorageChecksumRepository>();

            services.Configure<AzureStorageOptions>(options => configuration.GetSection(AzureStorageOptionsConfigSection).Bind(options));
        }

        public static void AddProfilePictureRepositoryHeathChecks(this IHealthChecksBuilder hcBuilder, IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider())
            {
                var azureStorageOptions = provider.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
                hcBuilder
                    .AddAzureBlobStorage(azureStorageOptions?.StorageConnectionString, "imageBlobs")
                    .AddAzureTableStorage(azureStorageOptions?.StorageConnectionString, "checksumTable");
            }
        }
    }
}
