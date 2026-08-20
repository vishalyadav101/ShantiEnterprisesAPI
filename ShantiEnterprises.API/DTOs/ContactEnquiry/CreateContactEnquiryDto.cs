namespace ShantiEnterprises.API.DTOs.ContactEnquiry
{
    public class CreateContactEnquiryDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}