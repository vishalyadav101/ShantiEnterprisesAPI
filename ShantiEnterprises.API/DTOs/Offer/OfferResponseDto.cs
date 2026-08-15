namespace ShantiEnterprises.API.DTOs.Offer
{
    public class OfferResponseDto
    {
        public int OfferId { get; set; }

        public string OfferName { get; set; }
            = string.Empty;

        public string Description { get; set; }
            = string.Empty;

        public decimal DiscountPercentage { get; set; }

        public decimal? MinimumOrderAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}