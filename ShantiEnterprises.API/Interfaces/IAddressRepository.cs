using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetByUserIdAsync(int userId);

        Task<Address?> GetByIdAsync(
            int addressId,
            int userId);

        Task<Address> AddAsync(Address address);

        Task UpdateAsync(Address address);

        Task DeleteAsync(Address address);

        Task ClearDefaultAsync(int userId);
    }
}