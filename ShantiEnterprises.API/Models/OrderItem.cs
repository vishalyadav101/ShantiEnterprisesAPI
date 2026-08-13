namespace ShantiEnterprises.API.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal GSTPercentage { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}