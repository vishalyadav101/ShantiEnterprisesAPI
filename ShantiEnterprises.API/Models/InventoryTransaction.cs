namespace ShantiEnterprises.API.Models
{
    public class InventoryTransaction
    {
        public int InventoryTransactionId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string TransactionType { get; set; }
            = string.Empty;

        public int? ReferenceId { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        // =========================
        // NAVIGATION
        // =========================

        public Product? Product { get; set; }
    }
}