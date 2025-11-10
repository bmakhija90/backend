namespace EcommerceAPI.Models
{
    public class SizeOption
    {
        public string SizeLabel { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal? PriceAdjustment { get; set; }
    }
}
