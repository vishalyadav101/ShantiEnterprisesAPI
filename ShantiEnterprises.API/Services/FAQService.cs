using ShantiEnterprises.API.DTOs.FAQ;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class FAQService : IFAQService
    {
        private readonly IFAQRepository _repository;

        public FAQService(
            IFAQRepository repository)
        {
            _repository = repository;
        }


        // ==========================================
        // CREATE FAQ
        // ADMIN
        // ==========================================

        public async Task<FAQResponseDto> CreateAsync(
            CreateFAQDto dto)
        {
            // ======================================
            // VALIDATION
            // ======================================

            if (string.IsNullOrWhiteSpace(dto.Question))
            {
                throw new ArgumentException(
                    "Question is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Answer))
            {
                throw new ArgumentException(
                    "Answer is required.");
            }

            if (dto.DisplayOrder < 0)
            {
                throw new ArgumentException(
                    "Display order cannot be negative.");
            }


            // ======================================
            // CREATE ENTITY
            // ======================================

            var faq = new FAQ
            {
                Question = dto.Question.Trim(),

                Answer = dto.Answer.Trim(),

                DisplayOrder = dto.DisplayOrder,

                IsActive = dto.IsActive,

                CreatedDate = DateTime.UtcNow,

                UpdatedDate = null
            };


            var created =
                await _repository.AddAsync(faq);


            return MapToDto(created);
        }


        // ==========================================
        // GET ALL
        // ADMIN
        // ==========================================

        public async Task<List<FAQResponseDto>> GetAllAsync()
        {
            var faqs =
                await _repository.GetAllAsync();

            return faqs
                .Select(MapToDto)
                .ToList();
        }


        // ==========================================
        // GET ACTIVE
        // PUBLIC
        // ==========================================

        public async Task<List<FAQResponseDto>> GetActiveAsync()
        {
            var faqs =
                await _repository.GetActiveAsync();

            return faqs
                .Select(MapToDto)
                .ToList();
        }


        // ==========================================
        // GET BY ID
        // ADMIN
        // ==========================================

        public async Task<FAQResponseDto?> GetByIdAsync(
            int faqId)
        {
            var faq =
                await _repository.GetByIdAsync(faqId);

            if (faq == null)
            {
                return null;
            }

            return MapToDto(faq);
        }


        // ==========================================
        // UPDATE FAQ
        // ADMIN
        // ==========================================

        public async Task<FAQResponseDto> UpdateAsync(
            int faqId,
            UpdateFAQDto dto)
        {
            var faq =
                await _repository.GetByIdAsync(faqId);

            if (faq == null)
            {
                throw new KeyNotFoundException(
                    "FAQ not found.");
            }


            // ======================================
            // VALIDATION
            // ======================================

            if (string.IsNullOrWhiteSpace(dto.Question))
            {
                throw new ArgumentException(
                    "Question is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Answer))
            {
                throw new ArgumentException(
                    "Answer is required.");
            }

            if (dto.DisplayOrder < 0)
            {
                throw new ArgumentException(
                    "Display order cannot be negative.");
            }


            // ======================================
            // UPDATE
            // ======================================

            faq.Question =
                dto.Question.Trim();

            faq.Answer =
                dto.Answer.Trim();

            faq.DisplayOrder =
                dto.DisplayOrder;

            faq.IsActive =
                dto.IsActive;

            faq.UpdatedDate =
                DateTime.UtcNow;


            var updated =
                await _repository.UpdateAsync(faq);


            return MapToDto(updated);
        }


        // ==========================================
        // DELETE FAQ
        // ADMIN
        // ==========================================

        public async Task DeleteAsync(
            int faqId)
        {
            var deleted =
                await _repository.DeleteAsync(faqId);

            if (!deleted)
            {
                throw new KeyNotFoundException(
                    "FAQ not found.");
            }
        }


        // ==========================================
        // ENTITY → DTO
        // ==========================================

        private static FAQResponseDto MapToDto(
            FAQ faq)
        {
            return new FAQResponseDto
            {
                FAQId =
                    faq.FAQId,

                Question =
                    faq.Question,

                Answer =
                    faq.Answer,

                DisplayOrder =
                    faq.DisplayOrder,

                IsActive =
                    faq.IsActive,

                CreatedDate =
                    faq.CreatedDate,

                UpdatedDate =
                    faq.UpdatedDate
            };
        }
    }
}