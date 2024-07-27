using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;
using System.Data;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _he;
        public ProductController(ApplicationDbContext db, Microsoft.AspNetCore.Hosting.IWebHostEnvironment he)
        {
            _db = db;
            _he = he;
        }
        // GET: ProductController
        public IActionResult Index()
        {
            return View(_db.Products.Include(c => c.Tag).ToList()); //回傳Products資料並轉成清單
        }
        [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? largeAmount)
        {
            var obj = _db.Products.Include(c => c.Tag)
                                  .Where(c => (c.SmallPrice >= lowAmount && c.SmallPrice <= largeAmount) ||
                                        (c.BigPrice >= lowAmount && c.BigPrice <= largeAmount))
                                  .ToList(); //設定搜尋條件

            if (lowAmount == null || largeAmount == null) //如果都有填值的話，回傳指定搜尋條件內容
            {
                obj = _db.Products.Include(c => c.Tag).ToList();
            }
            return View(obj);
        }

        public IActionResult Create()
        {
            ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name"); //宣告Tag的SelectList
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product obj, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                //檢查是否重複新增商品
                var searchProductName = _db.Products.FirstOrDefault(c => c.ProductName == obj.ProductName);
                if (searchProductName != null)
                {
                    ViewBag.ProductError = "此商品已經存在!";
                    ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name");
                    return View(obj);
                }

                if (image != null)
                {
                    var SelectTag = _db.Tags.FirstOrDefault(c => c.Id == obj.TagId); //取得所選的TAG

                    if (SelectTag != null) 
                    {
                        //照片存取
                        var name = Path.Combine(_he.WebRootPath + "/Images/" + SelectTag.Tag_Name, Path.GetFileName(image.FileName)); //設定路徑
                        using (var stream = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        {
                            await image.CopyToAsync(stream);
                        }
                        obj.Image = "Images/" + SelectTag.Tag_Name + "/" + image.FileName;
                    }
                    else
                    {
                        obj.Image = "Images/noimage.PNG";
                    }
                }
                else
                {
                    obj.Image = "Images/noimage.PNG";
                }

                _db.Products.Add(obj);
                await _db.SaveChangesAsync();
                TempData["save"] = "產品已被儲存!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name"); //宣告Tag的SelectList

                return View(obj);
            }
        }

        public IActionResult Edit(int? id)
        {
            ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name"); //宣告Tag的SelectList

            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == id); //利用ID搜尋指定產品
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product obj, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                if (image == null)
                {
                    obj.Image = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == obj.Id).Image;
                    _db.ChangeTracker.Clear(); //取消追蹤_db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == obj.Id)
                }
                else
                {
                    var SelectTag = _db.Tags.FirstOrDefault(c => c.Id == obj.TagId); //取得所選的TAG
                    //照片存取
                    var name = Path.Combine(_he.WebRootPath + "/Images/" + SelectTag.Tag_Name, Path.GetFileName(image.FileName)); //設定路徑
                    await image.CopyToAsync(new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                    obj.Image = "Images/" + SelectTag.Tag_Name + "/" + image.FileName;
                }

                _db.Products.Update(obj);
                await _db.SaveChangesAsync();
                TempData["update"] = "產品已被更新!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                obj.Image = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == obj.Id).Image;
                ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name"); //宣告Tag的SelectList

                return View(obj);
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == id); //利用ID搜尋指定產品
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

            var obj = _db.Products.Include(c => c.Tag).Where(c => c.Id == id).FirstOrDefault(); //利用ID搜尋指定產品
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

            var obj = _db.Products.FirstOrDefault(c => c.Id == id); //利用ID搜尋指定產品
            if (obj == null)
            {
                return NotFound();
            }

            _db.Products.Remove(obj); //刪除指定產品
            await _db.SaveChangesAsync(); //儲存資料庫
            TempData["remove"] = "產品已被移除!";
            return RedirectToAction(nameof(Index));
        }
    }
}
