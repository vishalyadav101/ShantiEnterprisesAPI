using ShantiEnterprises.API.DTOs.Shipment;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IShipmentService
    {
        // ==========================================
        // GET ALL SHIPMENTS
        // ==========================================

        Task<List<ShipmentResponseDto>> GetAllAsync();


        // ==========================================
        // GET SHIPMENT BY ID
        // ==========================================

        Task<ShipmentResponseDto> GetByIdAsync(
            int shipmentId);


        // ==========================================
        // GET SHIPMENT BY ORDER
        // ==========================================

        Task<ShipmentResponseDto> GetByOrderIdAsync(
            int orderId,
            int userId,
            bool isAdmin);


        // ==========================================
        // CREATE SHIPMENT
        // ==========================================

        Task<ShipmentResponseDto> CreateAsync(
            ShipmentCreateDto dto);


        // ==========================================
        // UPDATE SHIPMENT
        // ==========================================

        Task<ShipmentResponseDto> UpdateAsync(
            int shipmentId,
            ShipmentUpdateDto dto);


        // ==========================================
        // DELETE SHIPMENT
        // ==========================================

        Task DeleteAsync(
            int shipmentId);
    }
}