using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IFAQRepository
    {
        // ==========================================
        // GET ALL
        // ==========================================

        Task<List<FAQ>> GetAllAsync();


        // ==========================================
        // GET ACTIVE FAQS
        // PUBLIC
        // ==========================================

        Task<List<FAQ>> GetActiveAsync();


        // ==========================================
        // GET BY ID
        // ==========================================

        Task<FAQ?> GetByIdAsync(
            int faqId);


        // ==========================================
        // CREATE
        // ==========================================

        Task<FAQ> AddAsync(
            FAQ faq);


        // ==========================================
        // UPDATE
        // ==========================================

        Task<FAQ> UpdateAsync(
            FAQ faq);


        // ==========================================
        // DELETE
        // ==========================================

        Task<bool> DeleteAsync(
            int faqId);
    }
}