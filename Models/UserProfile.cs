using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Net;

namespace EcommerceAPI.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("avatarUrl")]
        public string AvatarUrl { get; set; } = string.Empty;

        [BsonElement("memberSince")]
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        [BsonElement("addresses")]
        public List<Address> Addresses { get; set; } = new List<Address>();

        [BsonElement("orders")]
        public List<Order> Orders { get; set; } = new List<Order>();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

