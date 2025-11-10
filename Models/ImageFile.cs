using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceAPI.Models
{
    public class ImageFile
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "image/jpeg";
        [BsonRepresentation(BsonType.Binary)]
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
