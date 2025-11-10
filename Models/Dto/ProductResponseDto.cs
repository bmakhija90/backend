namespace EcommerceAPI.Models.Dto
{
    public class ProductResponseDto
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public decimal basePrice { get; set; }
        public string category { get; set; } = string.Empty;
        public int stock { get; set; }
        public List<string> sizes { get; set; } = new();
        public bool isActive { get; set; }
        public List<string> imagePaths { get; set; } = new();
    }
}
