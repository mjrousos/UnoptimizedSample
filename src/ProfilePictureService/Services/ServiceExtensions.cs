using ProfilePictureService.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static void AddProfilePictureServices(this IServiceCollection services)
        {
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IHashingService, Sha512HashingService>();
        }
    }
}
