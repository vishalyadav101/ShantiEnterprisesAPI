using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IBulkEnquiryRepository
    {
        Task<List<BulkEnquiry>> GetAllAsync();

        Task<BulkEnquiry?> GetByIdAsync(int id);

        Task<BulkEnquiry> AddAsync(
            BulkEnquiry enquiry);

        Task UpdateAsync(
            BulkEnquiry enquiry);

        Task<bool> DeleteAsync(int id);
    }
}