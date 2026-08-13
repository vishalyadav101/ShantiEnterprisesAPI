namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductDetailResponseDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public decimal MRP { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Stock { get; set; }

        public decimal GSTPercentage { get; set; }

        public string SKU { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<ProductImageResponseDto> Images { get; set; }
            = new();

        public List<ProductPriceTierResponseDto> PriceTiers { get; set; }
            = new();
    }
}