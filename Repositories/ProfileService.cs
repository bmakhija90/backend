using EcommerceAPI.Models;
using EcommerceAPI.Models.Dto;

namespace EcommerceAPI.Repositories
{
    public class ProfileService : IProfileService
    {
        private readonly IUserProfileRepository _repository;

        public ProfileService(IUserProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<UserProfileDto> CreateProfileAsync(CreateProfileRequest request)
        {
            var profile = new UserProfile
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                MemberSince = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdProfile = await _repository.CreateAsync(profile);
            return MapToDto(createdProfile);
        }

        public async Task<UserProfileDto?> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var existingProfile = await _repository.GetByUserIdAsync(userId);
            if (existingProfile == null) return null;

            existingProfile.FirstName = request.FirstName;
            existingProfile.LastName = request.LastName;
            existingProfile.Email = request.Email;
            existingProfile.Phone = request.Phone;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            var updatedProfile = await _repository.UpdateAsync(existingProfile.Id, existingProfile);
            return MapToDto(updatedProfile);
        }

        public async Task<UserProfileDto?> AddAddressAsync(string userId, CreateAddressRequest request)
        {
            var address = new Address
            {
                Type = request.Type,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address1 = request.Address1,
                Address2 = request.Address2,
                City = request.City,
                County = request.County,
                Postcode = request.Postcode,
                Country = request.Country,
                IsDefault = request.IsDefault
            };

            var profile = await _repository.AddAddressAsync(userId, address);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<UserProfileDto?> UpdateAddressAsync(string userId, string addressId, CreateAddressRequest request)
        {
            var address = new Address
            {
                Id = addressId,
                Type = request.Type,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address1 = request.Address1,
                Address2 = request.Address2,
                City = request.City,
                County = request.County,
                Postcode = request.Postcode,
                Country = request.Country,
                IsDefault = request.IsDefault
            };

            var profile = await _repository.UpdateAddressAsync(userId, addressId, address);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<UserProfileDto?> DeleteAddressAsync(string userId, string addressId)
        {
            var profile = await _repository.DeleteAddressAsync(userId, addressId);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<UserProfileDto?> SetDefaultAddressAsync(string userId, string addressId)
        {
            var profile = await _repository.SetDefaultAddressAsync(userId, addressId);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<UserProfileDto?> AddOrderAsync(string userId, CreateOrderRequest request)
        {
            var order = new Order
            {
                OrderId = request.OrderId,
                Date = request.Date,
                Status = request.Status,
                Total = request.Total,
                TrackingNumber = request.TrackingNumber,
                DeliveryAddress = request.DeliveryAddress,
                Items = request.Items.Select(item => new OrderItem
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    ProductId = item.ProductId
                }).ToList()
            };

            var profile = await _repository.AddOrderAsync(userId, order);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<UserProfileDto?> UpdateOrderStatusAsync(string userId, string orderId, string status, string? trackingNumber = null)
        {
            var profile = await _repository.UpdateOrderStatusAsync(userId, orderId, status, trackingNumber);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _repository.GetUserOrdersAsync(userId);
            return orders.Select(order => MapToDto(order)).ToList();
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderId = order.OrderId,
                UserId = order.UserId,
                Date = order.Date,
                Status = order.Status,
                Total = order.Total,
                TrackingNumber = order.TrackingNumber,
                DeliveryAddress = order.DeliveryAddress,
                ShippingMethod = order.ShippingMethod,
                ShippingCost = order.ShippingCost,
                Vat = order.Vat,
                FinalTotal = order.FinalTotal,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                EstimatedDelivery = order.EstimatedDelivery,
                DeliveredDate = order.DeliveredDate,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    Size = item.Size,
                    Color = item.Color,
                    Category = item.Category
                }).ToList()
            };
        }

        private UserProfileDto MapToDto(UserProfile profile)
        {
            return new UserProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email,
                Phone = profile.Phone,
                AvatarUrl = profile.AvatarUrl,
                MemberSince = profile.MemberSince,
                Addresses = profile.Addresses.Select(a => new AddressDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Address1 = a.Address1,
                    Address2 = a.Address2,
                    City = a.City,
                    County = a.County,
                    Postcode = a.Postcode,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToList(),
                Orders = profile.Orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    Date = o.Date,
                    Status = o.Status,
                    Total = o.Total,
                    TrackingNumber = o.TrackingNumber,
                    DeliveryAddress = o.DeliveryAddress,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Name = i.Name,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        ImageUrl = i.ImageUrl,
                        ProductId = i.ProductId
                    }).ToList()
                }).ToList()
            };
        }
    }
}
