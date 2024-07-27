using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order obj)
        {
            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart") ?? new List<Cart>(); //取得購物車清單
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null) //如果有登入
            {
                int InputBonusPoints = HttpContext.Session.Get<int>("InputBonusPoints");
                user.BonusPoints = user.BonusPoints - InputBonusPoints; //使用者的紅利點數 - 輸入的紅利點數
                await _userManager.UpdateAsync(user); //儲存使用者資訊

                obj.UserId = user.Id;
            }

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
                    obj.OrderDetail.Add(orderDetail);
                }
            }
            
            _db.Orders.Add(obj); //新增訂單資訊

            await _db.SaveChangesAsync(); //儲存訂單資訊
            HttpContext.Session.Set("cart", new List<Cart>()); //將購物車設為空

            return RedirectToAction(nameof(SubmitOrderSuccess));
        }

        public string GetOrderNo()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            //產生8碼亂數並存入stringChars
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            
            var returnOrderNo = new String(stringChars); //把stringChars轉成String，然後存進returnOrderNo

            var searchOrderNo = _db.Orders.FirstOrDefault(c => c.OrderNo == returnOrderNo);//檢查是否重複的OrderNo
            while (searchOrderNo != null) //重複的話，再重新產生
            {
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                returnOrderNo = new String(stringChars);
                searchOrderNo = _db.Orders.FirstOrDefault(c => c.OrderNo == returnOrderNo);
            }

            return returnOrderNo;
        }

        public IActionResult SubmitOrderSuccess()
        {
            return View();
        }
    }
}
