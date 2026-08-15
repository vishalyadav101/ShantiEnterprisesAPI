using ShantiEnterprises.API.DTOs.BulkEnquiry;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IBulkEnquiryService
    {
        Task<List<BulkEnquiryResponseDto>>
            GetAllAsync();

        Task<BulkEnquiryResponseDto?>
            GetByIdAsync(int id);

        Task<BulkEnquiryResponseDto>
            CreateAsync(
                BulkEnquiryCreateDto dto);

        Task<BulkEnquiryResponseDto>
            UpdateAsync(
                int id,
                BulkEnquiryUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}