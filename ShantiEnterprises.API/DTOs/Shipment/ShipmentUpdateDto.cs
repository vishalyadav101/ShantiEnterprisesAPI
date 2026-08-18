namespace ShantiEnterprises.API.DTOs.Shipment
{
    public class ShipmentUpdateDto
    {
        // ==========================================
        // COURIER / SHIPPING
        // ==========================================

        public string? CourierName { get; set; }

        public string? TrackingNumber { get; set; }

        public string? TrackingUrl { get; set; }

        public string? ShippingMethod { get; set; }


        // ==========================================
        // STATUS
        // ==========================================

        public string? ShipmentStatus { get; set; }

        public string? StatusDescription { get; set; }


        // ==========================================
        // DELIVERY
        // ==========================================

        public DateTime? EstimatedDeliveryDate { get; set; }

        public string? DeliveredTo { get; set; }

        public string? DeliveryNotes { get; set; }
    }
}