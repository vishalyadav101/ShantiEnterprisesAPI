namespace ShantiEnterprises.API.DTOs.Shipment
{
    public class ShipmentResponseDto
    {
        public int ShipmentId { get; set; }

        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;


        // ==========================================
        // COURIER / SHIPPING
        // ==========================================

        public string? CourierName { get; set; }

        public string? TrackingNumber { get; set; }

        public string? TrackingUrl { get; set; }

        public string ShippingMethod { get; set; } = string.Empty;


        // ==========================================
        // STATUS
        // ==========================================

        public string ShipmentStatus { get; set; } = string.Empty;

        public string? StatusDescription { get; set; }


        // ==========================================
        // DELIVERY DATES
        // ==========================================

        public DateTime? ShippedDate { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }

        public DateTime? OutForDeliveryDate { get; set; }

        public DateTime? DeliveredDate { get; set; }


        // ==========================================
        // DELIVERY INFORMATION
        // ==========================================

        public string? DeliveredTo { get; set; }

        public string? DeliveryNotes { get; set; }


        // ==========================================
        // AUDIT
        // ==========================================

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}