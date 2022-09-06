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

        public string OrderNo { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string PhoneNo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        public int Total { get; set; }
        public virtual List<OrderDetail> OrderDetail { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public bool OrderIsComplete { get; set; } = false;
    }
}
