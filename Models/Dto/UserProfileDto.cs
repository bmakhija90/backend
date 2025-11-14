namespace EcommerceAPI.Models.Dto
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }

    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
