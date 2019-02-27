using Microsoft.Extensions.Logging;
using ProfilePictureService.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProfilePictureService.Services
{
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;
        private readonly IImageRepository _imageRepository;
        private readonly IChecksumRepository _checksumRepository;
        private readonly IHashingService _hashingService;

        public ImageService(ILogger<ImageService> logger, IImageRepository imageRepository, IChecksumRepository checksumRepository, IHashingService hashingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
            _checksumRepository = checksumRepository ?? throw new ArgumentNullException(nameof(checksumRepository));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
        }

        public async Task<string> GetImageAsBase64Async(string fileName)
        {
            var data = await GetImageAsync(fileName).ConfigureAwait(false);
            return Convert.ToBase64String(data);
        }

        public async Task<byte[]> GetImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be null or empty", nameof(fileName));
            }

            var image = await _imageRepository.GetAsync(fileName).ConfigureAwait(false);
            if ((image?.Length ?? 0) == 0)
            {
                throw new FileNotFoundException("Invalid file name", fileName);
            }

            var checkSum = await _checksumRepository.GetAsync(fileName).ConfigureAwait(false);
            if (string.IsNullOrEmpty(checkSum) ||
                checkSum.Equals(_hashingService.CalculateChecksum(image), StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Invalid checksum for file {fileName}");
            }

            return image;
        }

        public async Task<bool> RemoveImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be null or empty", nameof(fileName));
            }

            // The return value here should indicate whether any changes were made, so return true
            // if either a checksum or image data was deleted.
            var ret = false;
            ret |= await _checksumRepository.DeleteAsync(fileName).ConfigureAwait(false);
            ret |= await _imageRepository.DeleteAsync(fileName).ConfigureAwait(false);

            return ret;
        }

        public async Task StoreImageAsync(string fileName, byte[] image)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be null or empty", nameof(fileName));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var checksum = _hashingService.CalculateChecksum(image);
            await Task.WhenAll(
                _imageRepository.SetAsync(fileName, image),
                _checksumRepository.SetAsync(fileName, checksum)
                ).ConfigureAwait(false);
        }
    }
}
