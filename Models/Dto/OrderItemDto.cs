namespace EcommerceAPI.Models.Dto
{
    public class OrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string SizeLabel { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
