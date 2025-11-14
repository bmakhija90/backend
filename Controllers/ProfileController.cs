using EcommerceAPI.Models.Dto;
using EcommerceAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(string userId)
        {
            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
            {
                return NotFound($"Profile for user {userId} not found");
            }
            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<UserProfileDto>> CreateProfile(CreateProfileRequest request)
        {
            try
            {
                var profile = await _profileService.CreateProfileAsync(request);
                return CreatedAtAction(nameof(GetProfile), new { userId = profile.UserId }, profile);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating profile: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile(string userId, UpdateProfileRequest request)
        {
            var profile = await _profileService.UpdateProfileAsync(userId, request);
            if (profile == null)
            {
                return NotFound($"Profile for user {userId} not found");
            }
            return Ok(profile);
        }

        // Address Endpoints
        [HttpPost("{userId}/addresses")]
        public async Task<ActionResult<UserProfileDto>> AddAddress(string userId, CreateAddressRequest request)
        {
            var profile = await _profileService.AddAddressAsync(userId, request);
            if (profile == null)
            {
                return NotFound($"Profile for user {userId} not found");
            }
            return Ok(profile);
        }

        [HttpPut("{userId}/addresses/{addressId}")]
        public async Task<ActionResult<UserProfileDto>> UpdateAddress(string userId, string addressId, CreateAddressRequest request)
        {
            var profile = await _profileService.UpdateAddressAsync(userId, addressId, request);
            if (profile == null)
            {
                return NotFound($"Address {addressId} not found for user {userId}");
            }
            return Ok(profile);
        }

        [HttpDelete("{userId}/addresses/{addressId}")]
        public async Task<ActionResult<UserProfileDto>> DeleteAddress(string userId, string addressId)
        {
            var profile = await _profileService.DeleteAddressAsync(userId, addressId);
            if (profile == null)
            {
                return NotFound($"Address {addressId} not found for user {userId}");
            }
            return Ok(profile);
        }

        [HttpPut("{userId}/addresses/{addressId}/default")]
        public async Task<ActionResult<UserProfileDto>> SetDefaultAddress(string userId, string addressId)
        {
            var profile = await _profileService.SetDefaultAddressAsync(userId, addressId);
            if (profile == null)
            {
                return NotFound($"Address {addressId} not found for user {userId}");
            }
            return Ok(profile);
        }

        // Order Endpoints
        [HttpPost("{userId}/orders")]
        public async Task<ActionResult<UserProfileDto>> AddOrder(string userId, CreateOrderRequest request)
        {
            var profile = await _profileService.AddOrderAsync(userId, request);
            if (profile == null)
            {
                return NotFound($"Profile for user {userId} not found");
            }
            return Ok(profile);
        }

        [HttpPut("{userId}/orders/{orderId}/status")]
        public async Task<ActionResult<UserProfileDto>> UpdateOrderStatus(string userId, string orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var profile = await _profileService.UpdateOrderStatusAsync(userId, orderId, request.Status, request.TrackingNumber);
            if (profile == null)
            {
                return NotFound($"Order {orderId} not found for user {userId}");
            }
            return Ok(profile);
        }

        [HttpGet("{userId}/orders")]
        public async Task<ActionResult<List<OrderDto>>> GetUserOrders(string userId)
        {
            var orders = await _profileService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
    }
}
