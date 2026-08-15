namespace ShantiEnterprises.API.DTOs.BulkEnquiry
{
    public class BulkEnquiryCreateDto
    {
        public int? UserId { get; set; }

        public string CustomerName { get; set; }
            = string.Empty;

        public string Mobile { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public int? ProductId { get; set; }

        public int Quantity { get; set; }

        public string Message { get; set; }
            = string.Empty;
    }
}