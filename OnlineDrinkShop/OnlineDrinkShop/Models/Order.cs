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

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PhoneNo { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public int Total { get; set; }
        public virtual List<OrderDetail> OrderDetail { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public bool OrderIsComplete { get; set; } = false;
    }
}
