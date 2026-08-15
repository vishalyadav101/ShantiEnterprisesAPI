using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public OfferRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Offer>> GetAllAsync()
        {
            return await _context.Offers
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Offer?> GetByIdAsync(int id)
        {
            return await _context.Offers
                .FirstOrDefaultAsync(x => x.OfferId == id);
        }

        public async Task<Offer> AddAsync(Offer offer)
        {
            _context.Offers.Add(offer);

            await _context.SaveChangesAsync();

            return offer;
        }

        public async Task UpdateAsync(Offer offer)
        {
            _context.Offers.Update(offer);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var offer =
                await _context.Offers
                    .FirstOrDefaultAsync(
                        x => x.OfferId == id);

            if (offer == null)
            {
                return false;
            }

            _context.Offers.Remove(offer);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}