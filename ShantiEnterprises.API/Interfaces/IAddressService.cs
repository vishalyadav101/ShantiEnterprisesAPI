using ShantiEnterprises.API.DTOs.Address;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressResponseDto>> GetAllAsync(
            int userId);

        Task<AddressResponseDto?> GetByIdAsync(
            int userId,
            int addressId);

        Task<AddressResponseDto> CreateAsync(
            int userId,
            AddressCreateDto dto);

        Task<AddressResponseDto?> UpdateAsync(
            int userId,
            int addressId,
            AddressUpdateDto dto);

        Task<bool> DeleteAsync(
            int userId,
            int addressId);

        Task<bool> SetDefaultAsync(
            int userId,
            int addressId);
    }
}