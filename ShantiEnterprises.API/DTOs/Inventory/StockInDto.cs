namespace ShantiEnterprises.API.DTOs.Inventory
{
    public class StockInDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string? Remarks { get; set; }
    }
}