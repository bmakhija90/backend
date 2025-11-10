using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceAPI.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public List<ImageFile> Images { get; set; } = new();
        public string Category { get; set; } = string.Empty;
        public List<string> Sizes { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool isActive { get; set; }
        public int Stock { get; set; }

    }
}
