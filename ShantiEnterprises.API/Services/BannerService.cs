using ShantiEnterprises.API.DTOs.Banner;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repository;

        public BannerService(
            IBannerRepository repository)
        {
            _repository = repository;
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
            Validate(dto);

            var banner = new Banner
            {
                Title =
                    dto.Title.Trim(),

                Subtitle =
                    dto.Subtitle.Trim(),

                ImageUrl =
                    dto.ImageUrl.Trim(),

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

            var result =
                await _repository.AddAsync(
                    banner);

            return Map(result);
        }

        // =========================
        // UPDATE
        // =========================

        public async Task<BannerResponseDto>
            UpdateAsync(
                int id,
                BannerUpdateDto dto)
        {
            var banner =
                await _repository.GetByIdAsync(id);

            if (banner == null)
            {
                throw new Exception(
                    "Banner not found.");
            }

            Validate(dto);

            banner.Title =
                dto.Title.Trim();

            banner.Subtitle =
                dto.Subtitle.Trim();

            banner.ImageUrl =
                dto.ImageUrl.Trim();

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

            await _repository.UpdateAsync(
                banner);

            return Map(banner);
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool>
            DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(
                id);
        }

        // =========================
        // VALIDATION
        // =========================

        private static void Validate(
            BannerCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(
                    dto.Title))
            {
                throw new Exception(
                    "Banner title is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.ImageUrl))
            {
                throw new Exception(
                    "Banner image URL is required.");
            }

            if (dto.DisplayOrder < 0)
            {
                throw new Exception(
                    "Display order cannot be negative.");
            }
        }

        private static void Validate(
            BannerUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(
                    dto.Title))
            {
                throw new Exception(
                    "Banner title is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.ImageUrl))
            {
                throw new Exception(
                    "Banner image URL is required.");
            }

            if (dto.DisplayOrder < 0)
            {
                throw new Exception(
                    "Display order cannot be negative.");
            }
        }

        // =========================
        // MAP
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
    }
}