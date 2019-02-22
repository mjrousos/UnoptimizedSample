using System.Threading.Tasks;

namespace ProfilePictureService.Services
{
    interface IImageService
    {
        Task<byte[]> GetImage(string fileName);
        Task<string> GetImageAsBase64(string fileName);
        Task StoreImage(string fileName, byte[] image);
        Task<bool> RemoveImage(string fileName);
    }
}
