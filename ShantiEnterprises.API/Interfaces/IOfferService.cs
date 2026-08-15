using ShantiEnterprises.API.DTOs.Offer;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IOfferService
    {
        Task<List<OfferResponseDto>> GetAllAsync();

        Task<OfferResponseDto?> GetByIdAsync(int id);

        Task<OfferResponseDto> CreateAsync(
            OfferCreateDto dto);

        Task<OfferResponseDto> UpdateAsync(
            int id,
            OfferUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}