using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;//For accessing wwwroot folder
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> list = _UnitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(list);
        }
        public IActionResult Create()

        {
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = CategoryList
            };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");
                    using (var fileStreams = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName;

                }
                _UnitOfWork.Product.Add(productVM.Product);
                _UnitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _UnitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(productVM);
            }
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _UnitOfWork.Product.Get(u => u.Id == id);
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            ProductVM productVM = new()
            {
                Product = product,
                CategoryList = CategoryList
            };
            if (productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");
                    var oldImage = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                    using (var fileStreams = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName;
                }
                if(obj.Product.Id == 0)
                {
                    _UnitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _UnitOfWork.Product.Update(obj.Product);
                }
                _UnitOfWork.Product.Update(obj.Product);
                _UnitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        #region
        //API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> list = _UnitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = list });
        }
        public IActionResult Delete(int?id)
        {
            var product = _UnitOfWork.Product.Get(u => u.Id == id);
            if (product==null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            _UnitOfWork.Product.Remove(product);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
