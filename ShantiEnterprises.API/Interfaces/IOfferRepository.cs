using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IOfferRepository
    {
        Task<List<Offer>> GetAllAsync();

        Task<Offer?> GetByIdAsync(int id);

        Task<Offer> AddAsync(Offer offer);

        Task UpdateAsync(Offer offer);

        Task<bool> DeleteAsync(int id);
    }
}