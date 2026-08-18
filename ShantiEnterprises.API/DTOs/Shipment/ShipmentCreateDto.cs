namespace ShantiEnterprises.API.DTOs.Shipment
{
    public class ShipmentCreateDto
    {
        public int OrderId { get; set; }

        // ==========================================
        // COURIER / SHIPPING
        // ==========================================

        public string? CourierName { get; set; }

        public string? TrackingNumber { get; set; }

        public string? TrackingUrl { get; set; }

        public string ShippingMethod { get; set; } = "Standard";


        // ==========================================
        // DELIVERY
        // ==========================================

        public DateTime? EstimatedDeliveryDate { get; set; }

        public string? DeliveryNotes { get; set; }
    }
}