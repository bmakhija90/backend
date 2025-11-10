using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Dto
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
