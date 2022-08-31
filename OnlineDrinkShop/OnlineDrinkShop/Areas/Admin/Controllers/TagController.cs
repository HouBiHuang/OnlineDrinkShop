using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private ApplicationDbContext _db;

        public TagController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: TagController
        public IActionResult Index()
        {
            var data = _db.Tags.ToList();
            return View(data); //回傳Tag資料
        }

        // GET: TagController/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag obj)
        {
            if (ModelState.IsValid)
            {
                //檢查是否重複新增Tag
                var searchTagName = _db.Tags.FirstOrDefault(c => c.Tag_Name == obj.Tag_Name);
                if (searchTagName != null)
                {
                    ViewBag.message = "此Tag已經存在!";
                    return View(obj);
                }

                //如果沒重複Tag，就儲存
                _db.Tags.Add(obj);
                await _db.SaveChangesAsync();
                TempData["save"] = "Tag已被儲存!";
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Tags.Find(id); //利用ID尋找TAG
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tag obj)
        {
            if (ModelState.IsValid)
            {
                _db.Tags.Update(obj); //更新
                await _db.SaveChangesAsync(); //資料庫儲存
                TempData["update"] = "Tag已被更新!";
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

            var obj = _db.Tags.Find(id); //利用ID尋找TAG
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Tags.Find(id); //利用ID尋找TAG
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, Tag obj)
        {
            if (id == null || id != obj.Id)
            {
                return NotFound();
            }

            var obj_content = _db.Tags.Find(id); //利用ID尋找TAG
            if (obj_content == null)
            {
                return NotFound();
            }

            _db.Tags.Remove(obj_content); //刪除TAG
            await _db.SaveChangesAsync(); //資料庫儲存
            TempData["remove"] = "Tag已被刪除!";
            return RedirectToAction(nameof(Index));
        }
    }
}
