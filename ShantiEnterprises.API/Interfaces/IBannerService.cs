using ShantiEnterprises.API.DTOs.Banner;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IBannerService
    {
        Task<List<BannerResponseDto>> GetAllAsync();

        Task<BannerResponseDto?> GetByIdAsync(
            int id);

        Task<BannerResponseDto> CreateAsync(
            BannerCreateDto dto);

        Task<BannerResponseDto> UpdateAsync(
            int id,
            BannerUpdateDto dto);

        Task<bool> DeleteAsync(
            int id);
    }
}