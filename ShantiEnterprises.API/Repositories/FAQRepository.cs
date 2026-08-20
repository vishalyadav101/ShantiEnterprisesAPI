using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class FAQRepository : IFAQRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public FAQRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // GET ALL
        // ADMIN
        // ==========================================

        public async Task<List<FAQ>> GetAllAsync()
        {
            return await _context.FAQs
                .OrderBy(x => x.DisplayOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        // ==========================================
        // GET ACTIVE FAQS
        // PUBLIC
        // ==========================================

        public async Task<List<FAQ>> GetActiveAsync()
        {
            return await _context.FAQs
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<FAQ?> GetByIdAsync(
            int faqId)
        {
            return await _context.FAQs
                .FirstOrDefaultAsync(
                    x => x.FAQId == faqId);
        }


        // ==========================================
        // CREATE
        // ==========================================

        public async Task<FAQ> AddAsync(
            FAQ faq)
        {
            await _context.FAQs
                .AddAsync(faq);

            await _context.SaveChangesAsync();

            return faq;
        }


        // ==========================================
        // UPDATE
        // ==========================================

        public async Task<FAQ> UpdateAsync(
            FAQ faq)
        {
            _context.FAQs
                .Update(faq);

            await _context.SaveChangesAsync();

            return faq;
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task<bool> DeleteAsync(
            int faqId)
        {
            var faq =
                await _context.FAQs
                    .FirstOrDefaultAsync(
                        x => x.FAQId == faqId);

            if (faq == null)
            {
                return false;
            }

            _context.FAQs
                .Remove(faq);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}