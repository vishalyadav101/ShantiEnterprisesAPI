using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IShipmentRepository
    {
        // ==========================================
        // GET ALL SHIPMENTS
        // ==========================================

        Task<List<Shipment>> GetAllAsync();


        // ==========================================
        // GET SHIPMENT BY ID
        // ==========================================

        Task<Shipment?> GetByIdAsync(
            int shipmentId);


        // ==========================================
        // GET SHIPMENT BY ORDER
        // ==========================================

        Task<Shipment?> GetByOrderIdAsync(
            int orderId);


        // ==========================================
        // ADD SHIPMENT
        // ==========================================

        Task<Shipment> AddAsync(
            Shipment shipment);


        // ==========================================
        // UPDATE SHIPMENT
        // ==========================================

        Task<Shipment?> UpdateAsync(
            Shipment shipment);


        // ==========================================
        // DELETE SHIPMENT
        // ==========================================

        Task<bool> DeleteAsync(
            int shipmentId);
    }
}