namespace ShantiEnterprises.API.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

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
        // STOCK
        // =========================================================

        public int Stock { get; set; }

        public int ReorderLevel { get; set; } = 10;

        // =========================================================
        // GST
        // =========================================================

        public decimal GSTPercentage { get; set; }

        // =========================================================
        // OTHER
        // =========================================================

        public string SKU { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // =========================================================
        // NAVIGATION PROPERTIES
        // =========================================================

        public Category? Category { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }
            = new List<ProductImage>();

        public ICollection<ProductPriceTier> PriceTiers { get; set; }
            = new List<ProductPriceTier>();

        public ICollection<BulkEnquiry> BulkEnquiries { get; set; }
            = new List<BulkEnquiry>();

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
            = new List<InventoryTransaction>();
    }
}