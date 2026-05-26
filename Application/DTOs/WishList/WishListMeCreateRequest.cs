using System.ComponentModel.DataAnnotations;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.WishList
{
    public class WishListMeCreateRequest
    {
        [Required]
        public WishlistItemType ItemType { get; set; }

        [Required]
        public Guid ItemId { get; set; }
    }
}
