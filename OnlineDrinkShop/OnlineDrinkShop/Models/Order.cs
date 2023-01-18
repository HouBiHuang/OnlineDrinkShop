using System.ComponentModel.DataAnnotations;

namespace OnlineDrinkShop.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetail = new List<OrderDetail>();
        }
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入訂購人姓名")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入電話號碼")]
        public string PhoneNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "請輸入Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入地址")]
        public string Address { get; set; } = string.Empty;

        public int Total { get; set; }
        public virtual List<OrderDetail> OrderDetail { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public bool OrderIsComplete { get; set; } = false;
        public bool PointsHaveBeenGifted { get; set; } = false;
    }
}
