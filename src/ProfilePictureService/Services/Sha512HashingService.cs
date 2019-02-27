using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;

namespace ProfilePictureService.Services
{
    public class Sha512HashingService : IHashingService
    {
        private readonly ILogger<Sha512HashingService> _logger;

        public Sha512HashingService(ILogger<Sha512HashingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string CalculateChecksum(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (var algorithm = SHA512.Create())
            {
                return Convert.ToBase64String(algorithm.ComputeHash(data));
            }
        }
    }
}
