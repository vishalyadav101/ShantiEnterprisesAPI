namespace ShantiEnterprises.API.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        // ==========================================
        // ORDER
        // ==========================================

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;


        // ==========================================
        // COURIER / SHIPPING
        // ==========================================

        public string? CourierName { get; set; }

        public string? TrackingNumber { get; set; }

        public string? TrackingUrl { get; set; }

        public string ShippingMethod { get; set; } = "Standard";


        // ==========================================
        // SHIPMENT STATUS
        // ==========================================

        public string ShipmentStatus { get; set; } = "Pending";

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
            = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }
}