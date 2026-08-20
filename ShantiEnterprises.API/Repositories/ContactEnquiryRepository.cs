using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ContactEnquiryRepository : IContactEnquiryRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ContactEnquiryRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<ContactEnquiry>> GetAllAsync()
        {
            return await _context.ContactEnquiries
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<ContactEnquiry?> GetByIdAsync(
            int contactEnquiryId)
        {
            return await _context.ContactEnquiries
                .FirstOrDefaultAsync(
                    x => x.ContactEnquiryId == contactEnquiryId);
        }


        // ==========================================
        // CREATE
        // ==========================================

        public async Task<ContactEnquiry> AddAsync(
            ContactEnquiry contactEnquiry)
        {
            await _context.ContactEnquiries
                .AddAsync(contactEnquiry);

            await _context.SaveChangesAsync();

            return contactEnquiry;
        }


        // ==========================================
        // UPDATE
        // ==========================================

        public async Task<ContactEnquiry> UpdateAsync(
            ContactEnquiry contactEnquiry)
        {
            _context.ContactEnquiries
                .Update(contactEnquiry);

            await _context.SaveChangesAsync();

            return contactEnquiry;
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task<bool> DeleteAsync(
            int contactEnquiryId)
        {
            var contactEnquiry =
                await _context.ContactEnquiries
                    .FirstOrDefaultAsync(
                        x => x.ContactEnquiryId == contactEnquiryId);

            if (contactEnquiry == null)
            {
                return false;
            }

            _context.ContactEnquiries
                .Remove(contactEnquiry);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}