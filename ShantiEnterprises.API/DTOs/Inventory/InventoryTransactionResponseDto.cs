namespace ShantiEnterprises.API.DTOs.Inventory
{
    public class InventoryTransactionResponseDto
    {
        public int InventoryTransactionId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }
            = string.Empty;

        public int Quantity { get; set; }

        public string TransactionType { get; set; }
            = string.Empty;

        public int? ReferenceId { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}