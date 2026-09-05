using ShantiEnterprises.API.DTOs.Banner;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // =========================
        // IMAGE SETTINGS
        // =========================

        private readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private const long MaxFileSize = 5 * 1024 * 1024;

        public BannerService(
            IBannerRepository repository,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<BannerResponseDto>>
            GetAllAsync()
        {
            var banners =
                await _repository.GetAllAsync();

            return banners
                .Select(Map)
                .ToList();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<BannerResponseDto?>
            GetByIdAsync(int id)
        {
            var banner =
                await _repository.GetByIdAsync(id);

            if (banner == null)
            {
                return null;
            }

            return Map(banner);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<BannerResponseDto>
            CreateAsync(
                BannerCreateDto dto)
        {
            // -----------------------------------------
            // VALIDATE TEXT DATA
            // -----------------------------------------

            ValidateCreate(dto);

            // -----------------------------------------
            // UPLOAD IMAGE
            // -----------------------------------------

            var imageUrl =
                await UploadImageAsync(dto.Image);

            // -----------------------------------------
            // CREATE BANNER
            // -----------------------------------------

            var banner = new Banner
            {
                Title =
                    dto.Title.Trim(),

                Subtitle =
                    dto.Subtitle?.Trim() ?? string.Empty,

                ImageUrl =
                    imageUrl,

                ButtonText =
                    string.IsNullOrWhiteSpace(
                        dto.ButtonText)
                        ? null
                        : dto.ButtonText.Trim(),

                ButtonUrl =
                    string.IsNullOrWhiteSpace(
                        dto.ButtonUrl)
                        ? null
                        : dto.ButtonUrl.Trim(),

                DisplayOrder =
                    dto.DisplayOrder,

                IsActive =
                    dto.IsActive,

                CreatedDate =
                    DateTime.UtcNow
            };

            // -----------------------------------------
            // SAVE DATABASE
            // -----------------------------------------

            try
            {
                var result =
                    await _repository.AddAsync(
                        banner);

                return Map(result);
            }
            catch
            {
                // If database save fails,
                // remove uploaded physical image.
                DeletePhysicalFile(imageUrl);

                throw;
            }
        }

        // =========================
        // UPDATE
        // =========================

        public async Task<BannerResponseDto>
            UpdateAsync(
                int id,
                BannerUpdateDto dto)
        {
            // -----------------------------------------
            // GET EXISTING BANNER
            // -----------------------------------------

            var banner =
                await _repository.GetByIdAsync(id);

            if (banner == null)
            {
                throw new Exception(
                    "Banner not found.");
            }

            // -----------------------------------------
            // VALIDATE
            // -----------------------------------------

            ValidateUpdate(dto);

            // -----------------------------------------
            // KEEP OLD IMAGE BY DEFAULT
            // -----------------------------------------

            var oldImageUrl =
                banner.ImageUrl;

            var newImageUrl =
                oldImageUrl;

            // -----------------------------------------
            // UPLOAD NEW IMAGE IF SELECTED
            // -----------------------------------------

            if (dto.Image != null &&
                dto.Image.Length > 0)
            {
                newImageUrl =
                    await UploadImageAsync(
                        dto.Image);
            }

            // -----------------------------------------
            // UPDATE DATA
            // -----------------------------------------

            banner.Title =
                dto.Title.Trim();

            banner.Subtitle =
                dto.Subtitle?.Trim() ?? string.Empty;

            banner.ImageUrl =
                newImageUrl;

            banner.ButtonText =
                string.IsNullOrWhiteSpace(
                    dto.ButtonText)
                    ? null
                    : dto.ButtonText.Trim();

            banner.ButtonUrl =
                string.IsNullOrWhiteSpace(
                    dto.ButtonUrl)
                    ? null
                    : dto.ButtonUrl.Trim();

            banner.DisplayOrder =
                dto.DisplayOrder;

            banner.IsActive =
                dto.IsActive;

            // -----------------------------------------
            // SAVE DATABASE
            // -----------------------------------------

            try
            {
                await _repository.UpdateAsync(
                    banner);

                // -------------------------------------
                // DELETE OLD IMAGE
                // ONLY AFTER DB UPDATE SUCCESS
                // -------------------------------------

                if (newImageUrl != oldImageUrl)
                {
                    DeletePhysicalFile(
                        oldImageUrl);
                }

                return Map(banner);
            }
            catch
            {
                // If new image was uploaded but
                // database update failed,
                // remove the new image.
                if (newImageUrl != oldImageUrl)
                {
                    DeletePhysicalFile(
                        newImageUrl);
                }

                throw;
            }
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool>
            DeleteAsync(int id)
        {
            // -----------------------------------------
            // GET BANNER
            // -----------------------------------------

            var banner =
                await _repository.GetByIdAsync(id);

            if (banner == null)
            {
                return false;
            }

            // -----------------------------------------
            // DELETE DATABASE RECORD
            // -----------------------------------------

            var result =
                await _repository.DeleteAsync(id);

            // -----------------------------------------
            // DELETE PHYSICAL IMAGE
            // -----------------------------------------

            if (result)
            {
                DeletePhysicalFile(
                    banner.ImageUrl);
            }

            return result;
        }

        // =========================
        // CREATE VALIDATION
        // =========================

        private static void ValidateCreate(
            BannerCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(
                    dto.Title))
            {
                throw new Exception(
                    "Banner title is required.");
            }

            if (dto.Image == null ||
                dto.Image.Length == 0)
            {
                throw new Exception(
                    "Please select a banner image.");
            }

            if (dto.DisplayOrder < 0)
            {
                throw new Exception(
                    "Display order cannot be negative.");
            }
        }

        // =========================
        // UPDATE VALIDATION
        // =========================

        private static void ValidateUpdate(
            BannerUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(
                    dto.Title))
            {
                throw new Exception(
                    "Banner title is required.");
            }

            if (dto.DisplayOrder < 0)
            {
                throw new Exception(
                    "Display order cannot be negative.");
            }
        }

        // =========================
        // IMAGE UPLOAD
        // =========================

        private async Task<string>
            UploadImageAsync(
                IFormFile image)
        {
            // -----------------------------------------
            // NULL / EMPTY CHECK
            // -----------------------------------------

            if (image == null ||
                image.Length == 0)
            {
                throw new Exception(
                    "Please select a banner image.");
            }

            // -----------------------------------------
            // FILE SIZE
            // -----------------------------------------

            if (image.Length > MaxFileSize)
            {
                throw new Exception(
                    "Banner image size cannot be greater than 5 MB.");
            }

            // -----------------------------------------
            // EXTENSION
            // -----------------------------------------

            var extension =
                Path.GetExtension(
                    image.FileName)
                    .ToLowerInvariant();

            if (!_allowedExtensions.Contains(
                    extension))
            {
                throw new Exception(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");
            }

            // -----------------------------------------
            // UPLOAD FOLDER
            // -----------------------------------------

            var uploadsFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "banners");

            if (!Directory.Exists(
                    uploadsFolder))
            {
                Directory.CreateDirectory(
                    uploadsFolder);
            }

            // -----------------------------------------
            // UNIQUE FILE NAME
            // -----------------------------------------

            var fileName =
                $"{Guid.NewGuid()}{extension}";

            var filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName);

            // -----------------------------------------
            // SAVE PHYSICAL FILE
            // -----------------------------------------

            await using (
                var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create))
            {
                await image.CopyToAsync(
                    stream);
            }

            // -----------------------------------------
            // CREATE URL
            // -----------------------------------------

            var request =
                _httpContextAccessor
                    .HttpContext?
                    .Request;

            if (request == null)
            {
                // Remove file if URL cannot be created.
                DeleteFileByPath(filePath);

                throw new Exception(
                    "Unable to create image URL.");
            }

            var imageUrl =
                $"{request.Scheme}://{request.Host}/uploads/banners/{fileName}";

            return imageUrl;
        }

        // =========================
        // MAP RESPONSE
        // =========================

        private static BannerResponseDto Map(
            Banner banner)
        {
            return new BannerResponseDto
            {
                BannerId =
                    banner.BannerId,

                Title =
                    banner.Title,

                Subtitle =
                    banner.Subtitle,

                ImageUrl =
                    banner.ImageUrl,

                ButtonText =
                    banner.ButtonText,

                ButtonUrl =
                    banner.ButtonUrl,

                DisplayOrder =
                    banner.DisplayOrder,

                IsActive =
                    banner.IsActive,

                CreatedDate =
                    banner.CreatedDate
            };
        }

        // =========================
        // DELETE PHYSICAL IMAGE
        // =========================

        private void DeletePhysicalFile(
            string? imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                        imageUrl))
                {
                    return;
                }

                // -------------------------------------
                // Only process our own uploaded images
                // -------------------------------------

                var uri =
                    new Uri(imageUrl);

                var relativePath =
                    uri.AbsolutePath
                        .TrimStart('/')
                        .Replace(
                            '/',
                            Path.DirectorySeparatorChar);

                // Security: only delete files
                // inside wwwroot.
                var filePath =
                    Path.Combine(
                        _environment.WebRootPath,
                        relativePath);

                var fullWebRootPath =
                    Path.GetFullPath(
                        _environment.WebRootPath);

                var fullFilePath =
                    Path.GetFullPath(
                        filePath);

                if (!fullFilePath.StartsWith(
                        fullWebRootPath,
                        StringComparison
                            .OrdinalIgnoreCase))
                {
                    return;
                }

                if (File.Exists(
                        fullFilePath))
                {
                    File.Delete(
                        fullFilePath);
                }
            }
            catch
            {
                // Ignore physical file deletion errors.
            }
        }

        // =========================
        // DELETE FILE BY PATH
        // =========================

        private static void DeleteFileByPath(
            string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Ignore file deletion errors.
            }
        }
    }
}