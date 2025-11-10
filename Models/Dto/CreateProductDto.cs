namespace EcommerceAPI.Models.Dto
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<IFormFile> Images { get; set; } = new();
        public List<string> Sizes { get; set; } = new();
        public bool isActive { get; set; }
        public int Stock { get; set; }
    }
}
