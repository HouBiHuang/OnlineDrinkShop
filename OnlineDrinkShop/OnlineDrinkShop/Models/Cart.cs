namespace OnlineDrinkShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SugarLevel { get; set; } = string.Empty;
        public string IceLevel { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Price { get; set; }
        public string? Remark { get; set; }
    }
}
