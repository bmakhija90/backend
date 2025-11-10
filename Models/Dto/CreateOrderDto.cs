namespace EcommerceAPI.Models.Dto
{
    public class CreateOrderDto
    {
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
