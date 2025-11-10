namespace EcommerceAPI.Models.Dto
{
    public class SizeOptionDto
    {
        public string SizeLabel { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal? PriceAdjustment { get; set; }
    }
}
