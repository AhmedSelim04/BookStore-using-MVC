using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            orderVM = new()
            {
                OrderHeader = _UnitOfWork.OrderHeader.Get(u => u.id == orderId, includeProperties: "AppUser"),
                OrderDetails = _UnitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var OrderHeaderFromDb = _UnitOfWork.OrderHeader.Get(u => u.id == orderVM.OrderHeader.id);
            if (OrderHeaderFromDb == null)
            {
                return NotFound();
            }
            OrderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            OrderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            OrderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            OrderHeaderFromDb.City = orderVM.OrderHeader.City;
            OrderHeaderFromDb.State = orderVM.OrderHeader.State;
            OrderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            if(!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                OrderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                OrderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }
            _UnitOfWork.OrderHeader.Update(OrderHeaderFromDb);
            _UnitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new {orderId= OrderHeaderFromDb.id});
        }
        
        #region
        //API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {

            IEnumerable<OrderHeader> objOrderHeaders ;
            if(User.IsInRole(SD.Role_Employee) || User.IsInRole(SD.Role_Admin))
            {
                objOrderHeaders = _UnitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            }
            else
            {
                var userId = User.Claims.FirstOrDefault(u => u.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _UnitOfWork.OrderHeader.GetAll(u => u.AppUserId == userId, includeProperties: "AppUser");
            }
            switch (status)
            {
                case "pending":
                    objOrderHeaders= objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }
            return Json(new { data = objOrderHeaders });
        }
        
        #endregion

    }
}
