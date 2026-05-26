using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    //public interface IWishListRepository : IBaseRepository<Wishlist>
    //{
    //    Task<List<Wishlist>> GetWishListsByUserIdAsync(Guid userId);
    //    Task<Wishlist?> GetWishList(Guid id);
    //    Task<List<Wishlist>> GetWishLists(WishListSearch search);
    //}
    //public class WishListSearch
    //{
    //    public Guid? ItemId { get; set; }
    //    public WishlistItemType? ItemType { get; set; }
    //}

    public interface IWishListRepository : IBaseRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId);
        Task<Wishlist?> GetDuplicateAsync(Guid userId, Guid itemId);
        Task<Wishlist?> GetByUserAndItemAsync(Guid userId, Guid itemId);
        Task<bool> UserExistsAsync(Guid userId);
        Task<bool> ServiceExistsAsync(Guid serviceId);
        Task<bool> PackageExistsAsync(Guid packageId);
        Task<bool> AgencyExistsAsync(Guid agencyId);
        Task<bool> ProviderExistsAsync(Guid providerId);
        Task<bool> ItineraryExistsAsync(Guid itineraryId);
    }

}
