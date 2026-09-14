using ShantiEnterprises.API.DTOs.Banner;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;

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
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _cloudinaryService = cloudinaryService;
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
            // VALIDATE IMAGE
            // -----------------------------------------

            ValidateImage(dto.Image);

            // -----------------------------------------
            // UPLOAD IMAGE TO CLOUDINARY
            // -----------------------------------------

            var uploadResult =
                await _cloudinaryService.UploadImageAsync(
                    dto.Image,
                    "shanti-enterprises/banners");

            var imageUrl =
                uploadResult.Url;

            // -----------------------------------------
            // CREATE BANNER
            // -----------------------------------------

            var banner = new Banner
            {
                Title =
                    dto.Title.Trim(),

                Subtitle =
                    dto.Subtitle?.Trim()
                    ?? string.Empty,

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
                // remove uploaded Cloudinary image.

                var publicId =
                    uploadResult.PublicId;

                if (!string.IsNullOrWhiteSpace(publicId))
                {
                    try
                    {
                        await _cloudinaryService
                            .DeleteImageAsync(publicId);
                    }
                    catch
                    {
                        // Ignore cleanup errors.
                    }
                }

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

            string? newPublicId = null;

            // -----------------------------------------
            // UPLOAD NEW IMAGE IF SELECTED
            // -----------------------------------------

            if (dto.Image != null &&
                dto.Image.Length > 0)
            {
                ValidateImage(dto.Image);

                var uploadResult =
                    await _cloudinaryService.UploadImageAsync(
                        dto.Image,
                        "shanti-enterprises/banners");

                newImageUrl =
                    uploadResult.Url;

                newPublicId =
                    uploadResult.PublicId;
            }

            // -----------------------------------------
            // UPDATE DATA
            // -----------------------------------------

            banner.Title =
                dto.Title.Trim();

            banner.Subtitle =
                dto.Subtitle?.Trim()
                ?? string.Empty;

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
                    var oldPublicId =
                        ExtractCloudinaryPublicId(
                            oldImageUrl);

                    if (!string.IsNullOrWhiteSpace(
                            oldPublicId))
                    {
                        try
                        {
                            await _cloudinaryService
                                .DeleteImageAsync(
                                    oldPublicId);
                        }
                        catch
                        {
                            // Ignore cleanup errors.
                        }
                    }
                }

                return Map(banner);
            }
            catch
            {
                // If new image was uploaded but
                // database update failed,
                // remove the new Cloudinary image.

                if (!string.IsNullOrWhiteSpace(
                        newPublicId))
                {
                    try
                    {
                        await _cloudinaryService
                            .DeleteImageAsync(
                                newPublicId);
                    }
                    catch
                    {
                        // Ignore cleanup errors.
                    }
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
            // SAVE IMAGE URL BEFORE DELETE
            // -----------------------------------------

            var imageUrl =
                banner.ImageUrl;

            // -----------------------------------------
            // DELETE DATABASE RECORD
            // -----------------------------------------

            var result =
                await _repository.DeleteAsync(id);

            // -----------------------------------------
            // DELETE CLOUDINARY IMAGE
            // -----------------------------------------

            if (result)
            {
                var publicId =
                    ExtractCloudinaryPublicId(
                        imageUrl);

                if (!string.IsNullOrWhiteSpace(
                        publicId))
                {
                    try
                    {
                        await _cloudinaryService
                            .DeleteImageAsync(
                                publicId);
                    }
                    catch
                    {
                        // Ignore Cloudinary cleanup errors.
                    }
                }
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
        // IMAGE VALIDATION
        // =========================

        private void ValidateImage(
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
        // EXTRACT CLOUDINARY PUBLIC ID
        // =========================

        private static string? ExtractCloudinaryPublicId(
            string? imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                        imageUrl))
                {
                    return null;
                }

                var uri =
                    new Uri(imageUrl);

                var path =
                    uri.AbsolutePath;

                var uploadIndex =
                    path.IndexOf(
                        "/upload/",
                        StringComparison.OrdinalIgnoreCase);

                if (uploadIndex < 0)
                {
                    return null;
                }

                var publicPath =
                    path.Substring(
                        uploadIndex +
                        "/upload/".Length);

                var parts =
                    publicPath.Split(
                        '/',
                        StringSplitOptions.RemoveEmptyEntries);

                // Remove Cloudinary version:
                // v123456789/

                if (
                    parts.Length > 1 &&
                    parts[0].StartsWith("v") &&
                    long.TryParse(
                        parts[0].Substring(1),
                        out _)
                )
                {
                    publicPath =
                        string.Join(
                            "/",
                            parts.Skip(1));
                }

                // Remove file extension

                var extension =
                    Path.GetExtension(
                        publicPath);

                if (!string.IsNullOrEmpty(
                        extension))
                {
                    publicPath =
                        publicPath.Substring(
                            0,
                            publicPath.Length -
                            extension.Length);
                }

                return publicPath.Trim('/');
            }
            catch
            {
                return null;
            }
        }
    }
}