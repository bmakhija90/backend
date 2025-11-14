namespace EcommerceAPI.Models.Dto
{
    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public decimal Total { get; set; }
        public string? TrackingNumber { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = "standard";
        public decimal ShippingCost { get; set; }
        public decimal Vat { get; set; }
        public decimal FinalTotal { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "paid";
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class CreateOrderRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public decimal Total { get; set; }
        public string? TrackingNumber { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = "standard";
        public decimal ShippingCost { get; set; }
        public decimal Vat { get; set; }
        public decimal FinalTotal { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "paid";
        public DateTime? EstimatedDelivery { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
    }
}
