namespace ShantiEnterprises.API.DTOs.ContactEnquiry
{
    public class UpdateContactEnquiryDto
    {
        public string Status { get; set; } = string.Empty;

        public string? AdminReply { get; set; }
    }
}