using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Wishlist
{
    public class AddWishlistItemDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}