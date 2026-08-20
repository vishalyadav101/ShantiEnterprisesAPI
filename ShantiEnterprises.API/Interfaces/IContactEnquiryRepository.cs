using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IContactEnquiryRepository
    {
        // ==========================================
        // GET ALL
        // ==========================================

        Task<List<ContactEnquiry>> GetAllAsync();


        // ==========================================
        // GET BY ID
        // ==========================================

        Task<ContactEnquiry?> GetByIdAsync(
            int contactEnquiryId);


        // ==========================================
        // CREATE
        // ==========================================

        Task<ContactEnquiry> AddAsync(
            ContactEnquiry contactEnquiry);


        // ==========================================
        // UPDATE
        // ==========================================

        Task<ContactEnquiry> UpdateAsync(
            ContactEnquiry contactEnquiry);


        // ==========================================
        // DELETE
        // ==========================================

        Task<bool> DeleteAsync(
            int contactEnquiryId);
    }
}