namespace OnlineDrinkShop.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string ProductName { get; set; }
        public string SugarLevel { get; set; }
        public string IceLevel { get; set; }
        public string Size { get; set; }
        public int Price { get; set; }
        public string? Remark { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}
