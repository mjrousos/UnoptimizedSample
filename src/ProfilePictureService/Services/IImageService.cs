using System.Threading.Tasks;

namespace ProfilePictureService.Services
{
    interface IImageService
    {
        Task<byte[]> GetImageAsync(string fileName);
        Task<string> GetImageAsBase64Async(string fileName);
        Task StoreImageAsync(string fileName, byte[] image);
        Task<bool> RemoveImageAsync(string fileName);
    }
}
