namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductPriceTierResponseDto
    {
        public int ProductPriceTierId { get; set; }

        public int ProductId { get; set; }

        public int MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public decimal Price { get; set; }
    }
}