using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;
using OnlineDrinkShop.Utility;
using System.Diagnostics;
using X.PagedList;

namespace OnlineDrinkShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        //茗品系列View
        public IActionResult Tea(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag.Tag_Name == "茗品系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();

            return View(tea.ToPagedList(page ?? 1,16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        //奶茶系列View
        public IActionResult MilkTea(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag.Tag_Name == "奶茶系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();

            return View(tea.ToPagedList(page ?? 1, 16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        //季節鮮果系列View
        public IActionResult SeasonalFreshFruit(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag.Tag_Name == "季節鮮果系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();
             
            return View(tea.ToPagedList(page ?? 1, 16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == id); //利用id找尋指定Product
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        public IActionResult CartPage()
        {
            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart"); //購物車清單
            if (objs == null)
            {
                objs = new List<Cart>();
            }
            return View(objs);
        }

        [HttpPost]
        public ActionResult AddItemToCart(int? id, string select_size, string select_iceLevel,
            string select_sugarLevel, int inputCount, string inputRemark)
        {
            //檢查id是否存在
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == id); //取得產品
            //檢查產品是否存在
            if (obj == null)
            {
                return NotFound();
            }

            List<Cart> objs = new List<Cart>(); //objs:Product type list
            Cart item = new Cart(); //item:購物車物件
            objs = HttpContext.Session.Get<List<Cart>>("cart"); //購物車清單
            if (objs == null)
            {
                objs = new List<Cart>();
            }

            //將產品細項儲存至購物車
            item.Id = obj.Id;
            item.ProductName = obj.ProductName;
            item.Size = select_size;
            item.IceLevel = select_iceLevel;
            item.SugarLevel = select_sugarLevel;
            if(select_size == "Big")
            {
                item.Price = obj.BigPrice;
            }
            else
            {
                item.Price = obj.SmallPrice;
            }
            item.Remark = inputRemark;

            //購買多個同樣商品
            for (int i = 0; i < inputCount; i++)
            {
                objs.Add(item);
                HttpContext.Session.Set("cart", objs);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [ActionName("Remove")]
        public IActionResult RemoveToCart(int? id)
        {
            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart"); //購物車清單
            if (objs != null)
            {
                var obj = objs.FirstOrDefault(c => c.Id == id);
                if (obj != null)
                {
                    objs.Remove(obj); //將產品從購物車移除
                    HttpContext.Session.Set("cart", objs); //儲存
                }
            }
            return RedirectToAction(nameof(CartPage));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}