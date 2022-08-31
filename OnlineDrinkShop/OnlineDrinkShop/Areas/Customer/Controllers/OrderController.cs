using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;
using OnlineDrinkShop.Utility;

namespace OnlineDrinkShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order obj)
        {
            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart");
            obj.OrderNo = GetOrderNo(); //取得訂單編號

            if (objs != null)
            {
                foreach (var item in objs) //新增OrderDetail資料
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderNo = obj.OrderNo;
                    orderDetail.ProductName = item.ProductName;
                    orderDetail.SugarLevel = item.SugarLevel;
                    orderDetail.IceLevel = item.IceLevel;
                    orderDetail.Remark = item.Remark;
                    orderDetail.Price = item.Price;
                    orderDetail.Size = item.Size;
                    _db.OrderDetails.Add(orderDetail);
                }
            }

            _db.Orders.Add(obj); //新增訂單資訊
            await _db.SaveChangesAsync(); //儲存
            HttpContext.Session.Set("cart", new List<Cart>()); //將購物車設為空

            return RedirectToAction(nameof(SubmitOrderSuccess));
        }

        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }

        public IActionResult SubmitOrderSuccess()
        {
            return View();
        }
    }
}
