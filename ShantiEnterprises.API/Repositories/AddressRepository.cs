using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public AddressRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetByUserIdAsync(
            int userId)
        {
            return await _context.Addresses
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(
            int addressId,
            int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(x =>
                    x.AddressId == addressId &&
                    x.UserId == userId);
        }

        public async Task<Address> AddAsync(
            Address address)
        {
            _context.Addresses.Add(address);

            await _context.SaveChangesAsync();

            return address;
        }

        public async Task UpdateAsync(
            Address address)
        {
            _context.Addresses.Update(address);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(
            Address address)
        {
            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync();
        }

        public async Task ClearDefaultAsync(int userId)
        {
            var addresses =
                await _context.Addresses
                    .Where(x =>
                        x.UserId == userId &&
                        x.IsDefault)
                    .ToListAsync();

            foreach (var address in addresses)
            {
                address.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}