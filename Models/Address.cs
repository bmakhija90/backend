//EcommerceAPI.Models

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceAPI.Models
{
    public class Address
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty; // Home, Work, etc.

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("address1")]
        public string Address1 { get; set; } = string.Empty;

        [BsonElement("address2")]
        public string Address2 { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("county")]
        public string County { get; set; } = string.Empty;

        [BsonElement("postcode")]
        public string Postcode { get; set; } = string.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = "United Kingdom";

        [BsonElement("isDefault")]
        public bool IsDefault { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}