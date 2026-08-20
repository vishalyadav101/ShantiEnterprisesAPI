namespace ShantiEnterprises.API.DTOs.ContactEnquiry
{
    public class ContactEnquiryResponseDto
    {
        public int ContactEnquiryId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? AdminReply { get; set; }

        public DateTime? RepliedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}