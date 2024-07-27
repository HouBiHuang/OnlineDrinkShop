using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager; 
        }

        public IActionResult Index()
        {
            return View();
        }

        //茗品系列View
        public IActionResult Tea(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag!.Tag_Name == "茗品系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();

            return View(tea.ToPagedList(page ?? 1,16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        //奶茶系列View
        public IActionResult MilkTea(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag!.Tag_Name == "奶茶系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();

            return View(tea.ToPagedList(page ?? 1, 16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        //季節鮮果系列View
        public IActionResult SeasonalFreshFruit(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag!.Tag_Name == "季節鮮果系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();
             
            return View(tea.ToPagedList(page ?? 1, 16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        //果醋系列View
        public IActionResult FruitVinegar(int? page)
        {
            var tea = (from t in _db.Products
                       where t.Tag!.Tag_Name == "果醋系列" && t.IsAvailable == true
                       select t)
                      .Include(a => a.Tag)
                      .ToList();

            return View(tea.ToPagedList(page ?? 1, 16)); //如果無指定Page，則從第1頁開始顯示，每頁16筆
        }

        public IActionResult Details(string? productName)
        {
            if (productName == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.ProductName == productName); //利用id找尋指定Product
            if (obj == null)
            {
                return NotFound();
            }

            //設定當前頁面屬於哪個系列
            switch (obj.Tag?.Tag_Name) //選取當前飲料歸類
            {
                case "茗品系列":
                    ViewBag.Current = "Tea";
                    break;
                case "奶茶系列":
                    ViewBag.Current = "MilkTea";
                    break;
                case "季節鮮果系列":
                    ViewBag.Current = "SeasonalFreshFruit";
                    break;
                case "果醋系列":
                    ViewBag.Current = "FruitVinegar";
                    break;
                default:
                    break;
            }

            return View(obj);
        }

        public async Task<IActionResult> CartPage()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                ViewBag.BonusPoints = user.BonusPoints;
            }
            else
            {
                ViewBag.BonusPoints = 0;
            }

            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart") ?? new List<Cart>(); //購物車清單

            return View(objs);
        }
        [HttpPost]
        public async Task<IActionResult> CartPage(int InputBonusPoints)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart") ?? new List<Cart>(); //取得購物車清單

            if (user != null) //如果有登入
            {
                if (InputBonusPoints > user.BonusPoints) //如果輸入的點數超過可用的點數
                {
                    ViewBag.InputBonusPointsError = "輸入的點數超過可用的點數";
                    ViewBag.BonusPoints = user.BonusPoints;

                    return View(objs);
                }
                else if(InputBonusPoints > objs.Sum(c => c.Price)) //如果輸入的點數超過總金額
                {
                    ViewBag.InputBonusPointsError = "輸入的點數超過總金額";
                    ViewBag.BonusPoints = user.BonusPoints;

                    return View(objs);
                }
                else
                {
                    HttpContext.Session.Set("InputBonusPoints", InputBonusPoints); //儲存輸入的點數
                    return RedirectToAction("Checkout", "Order");
                }
            } //如果沒登入
            else
            {
                if (InputBonusPoints > 0) //如果輸入的點數超過可用的點數
                {
                    ViewBag.InputBonusPointsError = "輸入的點數超過可用的點數";
                    ViewBag.BonusPoints = 0;

                    return View(objs);
                }
                else
                {
                    HttpContext.Session.Set("InputBonusPoints", InputBonusPoints); //儲存輸入的點數
                    return RedirectToAction("Checkout", "Order");
                }
            }
        }

        [HttpPost]
        public IActionResult AddItemToCart(string? productName, string select_size, string select_iceLevel,
            string select_sugarLevel, int inputCount, string inputRemark)
        {
            //檢查id是否存在
            if (productName == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.ProductName == productName); //取得產品
            //檢查產品是否存在
            if (obj == null)
            {
                return NotFound();
            }

            List<Cart> objs = new List<Cart>(); //objs:Product type list
            Cart item = new Cart(); //item:購物車物件
            objs = HttpContext.Session.Get<List<Cart>>("cart") ?? new List<Cart>(); //購物車清單

            //將產品細項儲存至購物車
            item.Id = obj.Id;
            item.ProductName = obj.ProductName;
            item.Size = select_size;
            item.IceLevel = select_iceLevel;
            item.SugarLevel = select_sugarLevel;
            if(select_size == "大")
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
            }
            HttpContext.Session.Set("cart", objs);

            return RedirectToAction("Details", new { productName = productName });
        }

        [ActionName("Remove")]
        public IActionResult RemoveToCart(int? id)
        {
            List<Cart> objs = HttpContext.Session.Get<List<Cart>>("cart") ?? new List<Cart>(); //購物車清單
            if (objs != new List<Cart>())
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