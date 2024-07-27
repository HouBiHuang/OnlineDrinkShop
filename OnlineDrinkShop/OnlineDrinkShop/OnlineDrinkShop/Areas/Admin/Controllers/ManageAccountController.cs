using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;
using System.Data;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageAccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _db;
        public ManageAccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList()); //回傳使用者資料並轉成List
        }

        public IActionResult Edit(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id); //取得指定使用者
            if (user == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id); //取得指定使用者
            if (userInfo == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            userInfo.UserFullName = user.UserFullName; //更新UserFullName
            userInfo.PhoneNumber = user.PhoneNumber; //更新PhoneNumber
            userInfo.BonusPoints = user.BonusPoints;//更新BonusPoints

            var result = await _userManager.UpdateAsync(userInfo); //送出更新
            if (result.Succeeded)
            {
                TempData["update"] = "更新成功!";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult Details(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id); //取得指定使用者
            if (user == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Lockout(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id); //取得指定使用者
            if (user == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Lockout(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id); //取得指定使用者
            if (userInfo == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            userInfo.LockoutEnd = DateTime.Now.AddYears(100); //封鎖時間+100年
            int rowAffected = await _db.SaveChangesAsync(); //送出封鎖資訊，rowAffected:變動欄位有幾個
            if (rowAffected > 0)
            {
                TempData["lockout"] = "使用者已被封鎖!";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }

        public IActionResult Active(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id); //取得指定使用者
            if (user == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Active(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id); //取得指定使用者
            if (userInfo == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            userInfo.LockoutEnd = DateTime.Now.AddDays(-1); //設定解鎖，當今時間-1天
            int rowAffected = await _db.SaveChangesAsync(); //送出解鎖要求，rowAffected:變動欄位有幾個
            if (rowAffected > 0)
            {
                TempData["active"] = "使用者已被解鎖!";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }

        public IActionResult Delete(string id)
        { 
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id); //取得指定使用者
            if (user == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id); //取得指定使用者
            if (userInfo == null) //檢查指定使用者是否為空
            {
                return NotFound();
            }

             _db.ApplicationUsers.Remove(userInfo); //移除使用者
            int rowAffected = await _db.SaveChangesAsync(); //資料庫儲存
            if (rowAffected > 0)
            {
                TempData["remove"] = "使用者已被刪除!";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }
    }
}
