using ShantiEnterprises.API.DTOs.ContactEnquiry;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IContactEnquiryService
    {
        // ==========================================
        // CREATE CONTACT ENQUIRY
        // PUBLIC
        // ==========================================

        Task<ContactEnquiryResponseDto> CreateAsync(
            CreateContactEnquiryDto dto);


        // ==========================================
        // GET ALL
        // ADMIN
        // ==========================================

        Task<List<ContactEnquiryResponseDto>> GetAllAsync();


        // ==========================================
        // GET BY ID
        // ADMIN
        // ==========================================

        Task<ContactEnquiryResponseDto?> GetByIdAsync(
            int contactEnquiryId);


        // ==========================================
        // UPDATE STATUS / REPLY
        // ADMIN
        // ==========================================

        Task<ContactEnquiryResponseDto> UpdateAsync(
            int contactEnquiryId,
            UpdateContactEnquiryDto dto);


        // ==========================================
        // DELETE
        // ADMIN
        // ==========================================

        Task DeleteAsync(
            int contactEnquiryId);
    }
}