namespace ShantiEnterprises.API.DTOs.FAQ
{
    public class CreateFAQDto
    {
        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}