using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ShipmentRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET ALL SHIPMENTS
        // ==========================================

        public async Task<List<Shipment>> GetAllAsync()
        {
            return await _context.Shipments
                .Include(x => x.Order)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        // ==========================================
        // GET SHIPMENT BY ID
        // ==========================================

        public async Task<Shipment?> GetByIdAsync(
            int shipmentId)
        {
            return await _context.Shipments
                .Include(x => x.Order)
                .FirstOrDefaultAsync(
                    x => x.ShipmentId == shipmentId);
        }


        // ==========================================
        // GET SHIPMENT BY ORDER ID
        // ==========================================

        public async Task<Shipment?> GetByOrderIdAsync(
            int orderId)
        {
            return await _context.Shipments
                .Include(x => x.Order)
                .FirstOrDefaultAsync(
                    x => x.OrderId == orderId);
        }


        // ==========================================
        // ADD SHIPMENT
        // ==========================================

        public async Task<Shipment> AddAsync(
            Shipment shipment)
        {
            await _context.Shipments.AddAsync(
                shipment);

            await _context.SaveChangesAsync();

            return shipment;
        }


        // ==========================================
        // UPDATE SHIPMENT
        // ==========================================

        public async Task<Shipment?> UpdateAsync(
            Shipment shipment)
        {
            var existingShipment =
                await _context.Shipments
                    .FirstOrDefaultAsync(
                        x => x.ShipmentId ==
                             shipment.ShipmentId);

            if (existingShipment == null)
            {
                return null;
            }

            _context.Entry(existingShipment)
                .CurrentValues
                .SetValues(shipment);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(
                shipment.ShipmentId);
        }


        // ==========================================
        // DELETE SHIPMENT
        // ==========================================

        public async Task<bool> DeleteAsync(
            int shipmentId)
        {
            var shipment =
                await _context.Shipments
                    .FirstOrDefaultAsync(
                        x => x.ShipmentId ==
                             shipmentId);

            if (shipment == null)
            {
                return false;
            }

            _context.Shipments.Remove(
                shipment);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}