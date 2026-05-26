namespace TouRest.Application.DTOs.WishList
{
    public class WishListCheckResponse
    {
        public bool IsFavorited { get; set; }
        public Guid? WishlistId { get; set; }
    }
}
