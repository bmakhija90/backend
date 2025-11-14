//EcommerceAPI.Models
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceAPI.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;

        [BsonElement("items")]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("trackingNumber")]
        public string? TrackingNumber { get; set; }

        [BsonElement("deliveryAddress")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [BsonElement("shippingMethod")]
        public string ShippingMethod { get; set; } = "standard";

        [BsonElement("shippingCost")]
        public decimal ShippingCost { get; set; }

        [BsonElement("vat")]
        public decimal Vat { get; set; }

        [BsonElement("finalTotal")]
        public decimal FinalTotal { get; set; }

        [BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;

        [BsonElement("paymentStatus")]
        public string PaymentStatus { get; set; } = "paid";

        [BsonElement("estimatedDelivery")]
        public DateTime? EstimatedDelivery { get; set; }

        [BsonElement("deliveredDate")]
        public DateTime? DeliveredDate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderItem
    {
        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("size")]
        public string Size { get; set; } = string.Empty;

        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;
    }
}