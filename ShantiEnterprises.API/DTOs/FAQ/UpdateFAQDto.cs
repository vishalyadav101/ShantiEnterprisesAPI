namespace ShantiEnterprises.API.DTOs.FAQ
{
    public class UpdateFAQDto
    {
        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}