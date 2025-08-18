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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;//For accessing wwwroot folder
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> list = _UnitOfWork.Company.GetAll().ToList();
            
            return View(list);
        }
        public IActionResult Create()
        {
            return View(new Company());
        }
        [HttpPost]
        public IActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {

                _UnitOfWork.Company.Add(company);
                _UnitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? company = _UnitOfWork.Company.Get(u => u.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        public IActionResult Edit(Company obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _UnitOfWork.Company.Add(obj);
                }
                _UnitOfWork.Company.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Company updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        #region
        //API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> list = _UnitOfWork.Company.GetAll().ToList();
            return Json(new { data = list });
        }
        public IActionResult Delete(int?id)
        {
            var Company = _UnitOfWork.Company.Get(u => u.Id == id);
            if (Company==null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            } 
            _UnitOfWork.Company.Remove(Company);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
