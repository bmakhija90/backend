using EcommerceAPI.Models.Dto;

namespace EcommerceAPI.Repositories
{
    public interface IProfileService
    {
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<UserProfileDto> CreateProfileAsync(CreateProfileRequest request);
        Task<UserProfileDto?> UpdateProfileAsync(string userId, UpdateProfileRequest request);

        // Address operations
        Task<UserProfileDto?> AddAddressAsync(string userId, CreateAddressRequest request);
        Task<UserProfileDto?> UpdateAddressAsync(string userId, string addressId, CreateAddressRequest request);
        Task<UserProfileDto?> DeleteAddressAsync(string userId, string addressId);
        Task<UserProfileDto?> SetDefaultAddressAsync(string userId, string addressId);

        // Order operations
        Task<UserProfileDto?> AddOrderAsync(string userId, CreateOrderRequest request);
        Task<UserProfileDto?> UpdateOrderStatusAsync(string userId, string orderId, string status, string? trackingNumber = null);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
    }

    public class CreateProfileRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
