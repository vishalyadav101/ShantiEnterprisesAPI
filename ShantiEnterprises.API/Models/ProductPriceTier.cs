namespace ShantiEnterprises.API.Models
{
    public class ProductPriceTier
    {
        public int ProductPriceTierId { get; set; }

        public int ProductId { get; set; }

        public int MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public decimal Price { get; set; }

        public Product? Product { get; set; }
    }
}