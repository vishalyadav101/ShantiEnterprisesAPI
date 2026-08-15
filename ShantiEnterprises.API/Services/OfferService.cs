using ShantiEnterprises.API.DTOs.Offer;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _repository;

        public OfferService(
            IOfferRepository repository)
        {
            _repository = repository;
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<OfferResponseDto>>
            GetAllAsync()
        {
            var offers =
                await _repository.GetAllAsync();

            return offers
                .Select(Map)
                .ToList();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<OfferResponseDto?>
            GetByIdAsync(int id)
        {
            var offer =
                await _repository.GetByIdAsync(id);

            if (offer == null)
            {
                return null;
            }

            return Map(offer);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<OfferResponseDto>
            CreateAsync(OfferCreateDto dto)
        {
            Validate(
                dto.OfferName,
                dto.DiscountPercentage,
                dto.MinimumOrderAmount,
                dto.StartDate,
                dto.EndDate);

            var offer = new Offer
            {
                OfferName = dto.OfferName.Trim(),

                Description =
                    dto.Description.Trim(),

                DiscountPercentage =
                    dto.DiscountPercentage,

                MinimumOrderAmount =
                    dto.MinimumOrderAmount,

                StartDate =
                    dto.StartDate,

                EndDate =
                    dto.EndDate,

                IsActive =
                    dto.IsActive,

                CreatedDate =
                    DateTime.UtcNow
            };

            var result =
                await _repository.AddAsync(offer);

            return Map(result);
        }

        // =========================
        // UPDATE
        // =========================

        public async Task<OfferResponseDto>
            UpdateAsync(
                int id,
                OfferUpdateDto dto)
        {
            var offer =
                await _repository.GetByIdAsync(id);

            if (offer == null)
            {
                throw new Exception(
                    "Offer not found.");
            }

            Validate(
                dto.OfferName,
                dto.DiscountPercentage,
                dto.MinimumOrderAmount,
                dto.StartDate,
                dto.EndDate);

            offer.OfferName =
                dto.OfferName.Trim();

            offer.Description =
                dto.Description.Trim();

            offer.DiscountPercentage =
                dto.DiscountPercentage;

            offer.MinimumOrderAmount =
                dto.MinimumOrderAmount;

            offer.StartDate =
                dto.StartDate;

            offer.EndDate =
                dto.EndDate;

            offer.IsActive =
                dto.IsActive;

            await _repository.UpdateAsync(
                offer);

            return Map(offer);
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool>
            DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // =========================
        // VALIDATION
        // =========================

        private static void Validate(
            string offerName,
            decimal discountPercentage,
            decimal? minimumOrderAmount,
            DateTime startDate,
            DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(
                    offerName))
            {
                throw new Exception(
                    "Offer name is required.");
            }

            if (discountPercentage <= 0 ||
                discountPercentage > 100)
            {
                throw new Exception(
                    "Discount percentage must be greater than 0 and less than or equal to 100.");
            }

            if (minimumOrderAmount.HasValue &&
                minimumOrderAmount.Value < 0)
            {
                throw new Exception(
                    "Minimum order amount cannot be negative.");
            }

            if (endDate <= startDate)
            {
                throw new Exception(
                    "End date must be greater than start date.");
            }
        }

        // =========================
        // MAP
        // =========================

        private static OfferResponseDto Map(
            Offer offer)
        {
            return new OfferResponseDto
            {
                OfferId =
                    offer.OfferId,

                OfferName =
                    offer.OfferName,

                Description =
                    offer.Description,

                DiscountPercentage =
                    offer.DiscountPercentage,

                MinimumOrderAmount =
                    offer.MinimumOrderAmount,

                StartDate =
                    offer.StartDate,

                EndDate =
                    offer.EndDate,

                IsActive =
                    offer.IsActive,

                CreatedDate =
                    offer.CreatedDate
            };
        }
    }
}