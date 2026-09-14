using Microsoft.AspNetCore.Http;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(string Url, string PublicId)> UploadImageAsync(
            IFormFile file,
            string folder);

        Task DeleteImageAsync(string publicId);
    }
}