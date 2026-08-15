using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class BulkEnquiryRepository
        : IBulkEnquiryRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public BulkEnquiryRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<BulkEnquiry>>
            GetAllAsync()
        {
            return await _context.BulkEnquiries
                .Include(x => x.User)
                .Include(x => x.Product)
                .OrderByDescending(
                    x => x.CreatedDate)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<BulkEnquiry?>
            GetByIdAsync(int id)
        {
            return await _context.BulkEnquiries
                .Include(x => x.User)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(
                    x => x.BulkEnquiryId == id);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<BulkEnquiry>
            AddAsync(BulkEnquiry enquiry)
        {
            _context.BulkEnquiries.Add(enquiry);

            await _context.SaveChangesAsync();

            return enquiry;
        }

        // =========================
        // UPDATE
        // =========================

        public async Task UpdateAsync(
            BulkEnquiry enquiry)
        {
            _context.BulkEnquiries.Update(enquiry);

            await _context.SaveChangesAsync();
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool>
            DeleteAsync(int id)
        {
            var enquiry =
                await _context.BulkEnquiries
                    .FirstOrDefaultAsync(
                        x => x.BulkEnquiryId == id);

            if (enquiry == null)
            {
                return false;
            }

            _context.BulkEnquiries.Remove(enquiry);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}