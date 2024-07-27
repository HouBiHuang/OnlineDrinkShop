using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineDrinkShop.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public string OrderNo { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SugarLevel { get; set; } = string.Empty;
        public string IceLevel { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Price { get; set; }
        public string? Remark { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}
