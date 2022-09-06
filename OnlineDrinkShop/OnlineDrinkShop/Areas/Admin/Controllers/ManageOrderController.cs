using Microsoft.AspNetCore.Mvc;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageOrderController : Controller
    {
        private ApplicationDbContext _db;

        public ManageOrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Orders.ToList()); //回傳Products資料並轉成清單
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Orders.FirstOrDefault(c => c.Id == id); //利用ID搜尋指定訂單
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Order obj)
        {
            if (ModelState.IsValid)
            {
                _db.Orders.Update(obj); //更新
                await _db.SaveChangesAsync(); //資料庫儲存
                TempData["update"] = "訂單已被更新!";
                return RedirectToAction(nameof(Index));
            }
            
            return View(obj);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Orders.FirstOrDefault(c => c.Id == id); //利用ID搜尋指定訂單

            if (obj == null)
            {
                return NotFound();
            }

            var Detail = from d in _db.OrderDetails
                         where d.OrderNo == obj.OrderNo
                         select d; //利用OrderNo找到訂單細節
            ViewBag.OrderDetail = Detail;

            return View(obj);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Orders.FirstOrDefault(c => c.Id == id); //利用ID搜尋指定訂單
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Orders.FirstOrDefault(c => c.Id == id); //利用ID搜尋指定訂單
            if (obj == null)
            {
                return NotFound();
            }
            //var data = _db.OrderDetails.AsQueryable();
            //data = data.Where(c => c.OrderNo == obj.OrderNo);
            var details = _db.OrderDetails.Where(c => c.OrderNo == obj.OrderNo);
            foreach (var data in details)
            {
                _db.OrderDetails.Remove(data);
            }

            _db.Orders.Remove(obj); //刪除指定訂單
            await _db.SaveChangesAsync(); //儲存資料庫
            TempData["remove"] = "訂單已被移除!";
            return RedirectToAction(nameof(Index));
        }
    }
}
