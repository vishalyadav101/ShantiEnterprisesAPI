using Microsoft.AspNetCore.Hosting;
using ShantiEnterprises.API.DTOs.WebsiteSetting;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class WebsiteSettingService
        : IWebsiteSettingService
    {
        private readonly IWebsiteSettingRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public WebsiteSettingService(
            IWebsiteSettingRepository repository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        public async Task<WebsiteSettingResponseDto?> GetAsync()
        {
            var setting =
                await _repository.GetAsync();

            if (setting == null)
            {
                return null;
            }

            return Map(setting);
        }

        public async Task<WebsiteSettingResponseDto>
            SaveAsync(
                WebsiteSettingCreateUpdateDto dto)
        {
            var setting =
                await _repository.GetAsync();

            // =========================
            // CREATE
            // =========================

            if (setting == null)
            {
                setting = new WebsiteSetting
                {
                    CompanyName =
                        dto.CompanyName,

                    Email =
                        dto.Email,

                    Phone =
                        dto.Phone,

                    WhatsAppNumber =
                        dto.WhatsAppNumber,

                    Address =
                        dto.Address,

                    FacebookUrl =
                        dto.FacebookUrl,

                    InstagramUrl =
                        dto.InstagramUrl,

                    TwitterUrl =
                        dto.TwitterUrl,

                    LinkedInUrl =
                        dto.LinkedInUrl,

                    YouTubeUrl =
                        dto.YouTubeUrl,

                    FooterText =
                        dto.FooterText,

                    UpdatedDate =
                        DateTime.UtcNow
                };

                if (dto.Logo != null)
                {
                    setting.LogoUrl =
                        await SaveImageAsync(
                            dto.Logo,
                            "logo");
                }

                if (dto.Favicon != null)
                {
                    setting.FaviconUrl =
                        await SaveImageAsync(
                            dto.Favicon,
                            "favicon");
                }

                var created =
                    await _repository.CreateAsync(
                        setting);

                return Map(created);
            }

            // =========================
            // UPDATE
            // =========================

            setting.CompanyName =
                dto.CompanyName;

            setting.Email =
                dto.Email;

            setting.Phone =
                dto.Phone;

            setting.WhatsAppNumber =
                dto.WhatsAppNumber;

            setting.Address =
                dto.Address;

            setting.FacebookUrl =
                dto.FacebookUrl;

            setting.InstagramUrl =
                dto.InstagramUrl;

            setting.TwitterUrl =
                dto.TwitterUrl;

            setting.LinkedInUrl =
                dto.LinkedInUrl;

            setting.YouTubeUrl =
                dto.YouTubeUrl;

            setting.FooterText =
                dto.FooterText;

            // Replace logo only if new image uploaded
            if (dto.Logo != null)
            {
                DeleteImage(setting.LogoUrl);

                setting.LogoUrl =
                    await SaveImageAsync(
                        dto.Logo,
                        "logo");
            }

            // Replace favicon only if new image uploaded
            if (dto.Favicon != null)
            {
                DeleteImage(setting.FaviconUrl);

                setting.FaviconUrl =
                    await SaveImageAsync(
                        dto.Favicon,
                        "favicon");
            }

            setting.UpdatedDate =
                DateTime.UtcNow;

            await _repository.UpdateAsync(
                setting);

            return Map(setting);
        }

        // =========================
        // SAVE IMAGE
        // =========================

        private async Task<string>
            SaveImageAsync(
                IFormFile file,
                string prefix)
        {
            var uploadsFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "settings");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(
                    uploadsFolder);
            }

            var extension =
                Path.GetExtension(
                    file.FileName);

            var fileName =
                $"{prefix}-{Guid.NewGuid():N}{extension}";

            var filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName);

            await using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create);

            await file.CopyToAsync(stream);

            return
                $"/images/settings/{fileName}";
        }

        // =========================
        // DELETE IMAGE
        // =========================

        private void DeleteImage(
            string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            var relativePath =
                imageUrl.TrimStart('/')
                        .Replace(
                            '/',
                            Path.DirectorySeparatorChar);

            var filePath =
                Path.Combine(
                    _environment.WebRootPath,
                    relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // =========================
        // MAP
        // =========================

        private static WebsiteSettingResponseDto
            Map(WebsiteSetting setting)
        {
            return new WebsiteSettingResponseDto
            {
                WebsiteSettingId =
                    setting.WebsiteSettingId,

                CompanyName =
                    setting.CompanyName,

                LogoUrl =
                    setting.LogoUrl,

                FaviconUrl =
                    setting.FaviconUrl,

                Email =
                    setting.Email,

                Phone =
                    setting.Phone,

                WhatsAppNumber =
                    setting.WhatsAppNumber,

                Address =
                    setting.Address,

                FacebookUrl =
                    setting.FacebookUrl,

                InstagramUrl =
                    setting.InstagramUrl,

                TwitterUrl =
                    setting.TwitterUrl,

                LinkedInUrl =
                    setting.LinkedInUrl,

                YouTubeUrl =
                    setting.YouTubeUrl,

                FooterText =
                    setting.FooterText,

                UpdatedDate =
                    setting.UpdatedDate
            };
        }
    }
}