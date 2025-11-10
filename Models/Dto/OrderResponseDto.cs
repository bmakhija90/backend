namespace EcommerceAPI.Models.Dto
{
    public class OrderResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
