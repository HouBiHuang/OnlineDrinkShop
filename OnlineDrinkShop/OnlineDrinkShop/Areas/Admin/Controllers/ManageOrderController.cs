using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _db;

        public ManageOrderController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Orders.ToList()); //回傳Products資料並轉成清單
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) //檢查有無該筆資料
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
            if(obj.UserId == null) //因不允許UserId為null時存入Database，故設為string.Empty
            {
                obj.UserId = string.Empty;
            }

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == obj.UserId);
                if(user != null && obj.OrderIsComplete && obj.PointsHaveBeenGifted == false) //如果有user、訂單完成、點數未送出
                {
                    user.BonusPoints = user.BonusPoints + (int)(obj.Total / 100); //使用者的紅利點數 + (總金額/100取整數)*滿100送1點
                    obj.PointsHaveBeenGifted = true; //點數已送出
                }

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
