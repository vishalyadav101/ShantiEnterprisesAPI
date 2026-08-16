namespace ShantiEnterprises.API.DTOs.Inventory
{
    public class StockAdjustmentDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string? Remarks { get; set; }
    }
}