namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductImageResponseDto
    {
        public int ProductImageId { get; set; }

        public int ProductId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}