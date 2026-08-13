namespace ShantiEnterprises.API.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public decimal MRP { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Stock { get; set; }

        public decimal GSTPercentage { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Category? Category { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }
            = new List<ProductImage>();

        public ICollection<ProductPriceTier> PriceTiers { get; set; }
    = new List<ProductPriceTier>();

        public ICollection<BulkEnquiry> BulkEnquiries { get; set; }
    = new List<BulkEnquiry>();
    }
}