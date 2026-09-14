using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");

            if (string.IsNullOrWhiteSpace(cloudinaryUrl))
            {
                throw new InvalidOperationException(
                    "CLOUDINARY_URL environment variable is not configured.");
            }

            _cloudinary = new Cloudinary(cloudinaryUrl);
            _cloudinary.Api.Secure = true;
        }

        public async Task<(string Url, string PublicId)> UploadImageAsync(
            IFormFile file,
            string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Image file is required.");

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new InvalidOperationException(
                    $"Cloudinary upload failed: {result.Error.Message}");
            }

            return (
                result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? "",
                result.PublicId
            );
        }

        public async Task DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new InvalidOperationException(
                    $"Cloudinary delete failed: {result.Error.Message}");
            }
        }
    }
}