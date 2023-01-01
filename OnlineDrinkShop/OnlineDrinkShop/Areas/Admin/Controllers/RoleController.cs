using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineDrinkShop.Areas.Admin.Models;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;
using System.Data;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _db;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList(); //取得Roles資料
            ViewBag.Roles = roles;
            
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            IdentityRole role = new IdentityRole();

            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name); //檢查新增Role名稱是否已存在
            if (isExist) //如果存在
            {
                ViewBag.RoleError = "\"" + name + "\"" + " 已經存在!";
                return View();
            }

            var result = await _roleManager.CreateAsync(role); //送出新增
            if (result.Succeeded) //新增成功
            {
                TempData["save"] = "Role建置成功!";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id); //藉由id找到相對應的role

            if (role == null) //檢查role是否存在
            {
                return NotFound();
            }

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id); //藉由id找到相對應的role

            if (role == null) //檢查role是否存在
            {
                return NotFound();
            }
            role.Name = name;

            var isExist = await _roleManager.RoleExistsAsync(role.Name); //檢查role.name是否存在
            if (isExist) //如果存在
            {
                ViewBag.msg = "\"" + name + "\"" + " 已經存在!";
                return View();
            }

            var result = await _roleManager.UpdateAsync(role); //送出更新
            if (result.Succeeded) //更新成功
            {
                TempData["update"] = "Role更新成功!";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id); //藉由id找到相對應的role

            if (role == null) //檢查role是否存在
            {
                return NotFound();
            }

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View();
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id); //藉由id找到相對應的role
            if (role == null) //檢查role是否存在
            {
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role); //送出刪除
            if (result.Succeeded) //刪除成功
            {
                TempData["remove"] = "Role刪除成功!";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult Assign()
        {
            //取得User名稱(如果被鎖定就過濾掉)及Role名稱並轉換成SelectList
            ViewData["UserId"] = new SelectList(_db.ApplicationUsers.Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null).ToList(), "Id", "UserName");
            ViewData["RoleName"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleVm userRole)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == userRole.UserId); //取得要被Assign的user

            if(user != null)
            {
                var isCheckRoleAssign = await _userManager.IsInRoleAsync(user, userRole.RoleName); //確認user是否已被Assign過相同的Role
                if (isCheckRoleAssign) //如果已被Assign
                {
                    //取得User名稱(如果被鎖定就過濾掉)及Role名稱並轉換成SelectList
                    ViewData["UserId"] = new SelectList(_db.ApplicationUsers.Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null).ToList(), "Id", "UserName");
                    ViewData["RoleName"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                    ViewBag.RoleError = "此使用者已被此Role指派過";
                    return View();
                }
                
                var role = await _userManager.AddToRoleAsync(user, userRole.RoleName); //user RoleAssign
                if (role.Succeeded) //成功指派
                {
                    TempData["save"] = "使用者Role指派成功!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        public ActionResult AssignUserRole()
        {
            var result = from ur in _db.UserRoles //取得UserRoles資料
                         join r in _db.Roles on ur.RoleId equals r.Id //加入Roles資料，條件:UserRoles.RoleId == Roles.Id
                         join a in _db.ApplicationUsers on ur.UserId equals a.Id //加入ApplicationUsers資料，條件:UserRoles.UserId == ApplicationUsers.Id
                         select new UserRoleMaping
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };
            ViewBag.UserRoles = result;

            return View();
        }
    }
}
