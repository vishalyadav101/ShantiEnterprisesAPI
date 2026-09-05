namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        // =========================================================
        // PRICING
        // =========================================================

        public decimal MRP { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal WholesalePrice { get; set; }

        // =========================================================
        // SHIPPING
        // =========================================================

        public decimal ShippingCharge { get; set; }

        // =========================================================
        // STOCK / GST
        // =========================================================

        public int Stock { get; set; }

        public decimal GSTPercentage { get; set; }

        // =========================================================
        // OTHER
        // =========================================================

        public string SKU { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}