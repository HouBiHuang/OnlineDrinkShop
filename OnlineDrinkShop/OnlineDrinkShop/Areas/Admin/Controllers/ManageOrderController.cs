using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;
using System.Data;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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

        public IActionResult Edit(int? id)
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

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Orders.Include(c => c.OrderDetail).FirstOrDefault(c => c.Id == id); //利用ID搜尋指定訂單

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
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

            _db.Orders.Remove(obj); //刪除指定訂單
            await _db.SaveChangesAsync(); //儲存資料庫
            TempData["remove"] = "訂單已被移除!";
            return RedirectToAction(nameof(Index));
        }
    }
}
