using ShantiEnterprises.API.DTOs.FAQ;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IFAQService
    {
        // ==========================================
        // CREATE FAQ
        // ADMIN
        // ==========================================

        Task<FAQResponseDto> CreateAsync(
            CreateFAQDto dto);


        // ==========================================
        // GET ALL
        // ADMIN
        // ==========================================

        Task<List<FAQResponseDto>> GetAllAsync();


        // ==========================================
        // GET ACTIVE
        // PUBLIC
        // ==========================================

        Task<List<FAQResponseDto>> GetActiveAsync();


        // ==========================================
        // GET BY ID
        // ADMIN
        // ==========================================

        Task<FAQResponseDto?> GetByIdAsync(
            int faqId);


        // ==========================================
        // UPDATE FAQ
        // ADMIN
        // ==========================================

        Task<FAQResponseDto> UpdateAsync(
            int faqId,
            UpdateFAQDto dto);


        // ==========================================
        // DELETE FAQ
        // ADMIN
        // ==========================================

        Task DeleteAsync(
            int faqId);
    }
}