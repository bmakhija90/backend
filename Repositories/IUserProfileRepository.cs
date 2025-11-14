using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<UserProfile> CreateAsync(UserProfile profile);
        Task<UserProfile> UpdateAsync(string id, UserProfile profile);
        Task<bool> DeleteAsync(string id);

        // Address operations
        Task<UserProfile?> AddAddressAsync(string userId, Address address);
        Task<UserProfile?> UpdateAddressAsync(string userId, string addressId, Address address);
        Task<UserProfile?> DeleteAddressAsync(string userId, string addressId);
        Task<UserProfile?> SetDefaultAddressAsync(string userId, string addressId);

        // Order operations
        Task<UserProfile?> AddOrderAsync(string userId, Order order);
        Task<UserProfile?> UpdateOrderStatusAsync(string userId, string orderId, string status, string? trackingNumber = null);
        Task<List<Order>> GetUserOrdersAsync(string userId);
    }
}
