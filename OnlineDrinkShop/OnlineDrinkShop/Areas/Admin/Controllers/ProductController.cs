using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineDrinkShop.Data;
using OnlineDrinkShop.Models;

namespace OnlineDrinkShop.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public ActionResult Index()
        {
            return View(_db.Products.Include(c => c.Tag).ToList());
        }
        [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? largeAmount)
        {
            var obj = _db.Products.Include(c => c.Tag)
                .Where(c => c.BigPrice >= lowAmount && c.BigPrice <= largeAmount).ToList();

            if (lowAmount == null || largeAmount == null)
            {
                obj = _db.Products.Include(c => c.Tag).ToList();
            }
            return View(obj);
        }

        public ActionResult Create()
        {
            ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name");
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
                    ViewBag.message = "此商品已經存在!";
                    ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name");
                    return View(obj);
                }

                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    using (var stream = new FileStream(name, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    obj.Image = "Images/" + image.FileName;
                }
                else
                {
                    obj.Image = "Images/noimage.PNG";
                }

                _db.Products.Add(obj);
                await _db.SaveChangesAsync();
                TempData["save"] = "Product type has been saved";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name");

                return View(obj);
            }
        }

        public ActionResult Edit(int? id)
        {
            ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name");

            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product obj, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    obj.Image = "Images/" + image.FileName;
                }
                else
                {
                    obj.Image = "Images/noimage.PNG";
                }

                _db.Products.Update(obj);
                await _db.SaveChangesAsync();
                TempData["update"] = "Product type has been updated";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["tagId"] = new SelectList(_db.Tags.ToList(), "Id", "Tag_Name");

                return View(obj);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Products.Include(c => c.Tag).FirstOrDefault(c => c.Id == id);
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

            var obj = _db.Products.Include(c => c.Tag).Where(c => c.Id == id).FirstOrDefault();
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

            var obj = _db.Products.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _db.Products.Remove(obj);
            await _db.SaveChangesAsync();
            TempData["remove"] = "Product type has been removed";
            return RedirectToAction(nameof(Index));
        }
    }
}
