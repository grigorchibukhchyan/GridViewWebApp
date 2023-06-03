using GridViewWebApp.Models;
using GridViewWebApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace GridViewWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerServices _customerServices;
        private readonly IStatusServices _statusServices;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ICustomerServices customerServices, IStatusServices statusServices, IWebHostEnvironment hostEnvironment)
        {
            _customerServices = customerServices;
            _statusServices = statusServices;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(string FName, string LName, int SearchStatusID, int sortBy, bool Asc=true, int page=1, int itemsInPage = 20)
        {
            AllModels models = new AllModels();
            ViewBag.PageSize = Math.Ceiling((double)_customerServices.GetCustomersCount(FName, LName, SearchStatusID) / (double)itemsInPage);
            ViewBag.ListOfStatus = _statusServices.GetStatuses().ToList();

            models.Customers_View_List = _customerServices.GetCustomers(FName, LName, SearchStatusID, sortBy, Asc,page,itemsInPage).ToList();

            return View(models);
        }

        public IActionResult Delete(int id)
        {
            _customerServices.DeleteCustomer(id);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Save(Customer customer, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                customer.Picture = "/images/" + fileName;

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _customerServices.InsertCustomer(customer);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditField(int customerID, string FName, string LName, int StatusID, IFormFile file)
        {
            Customer cus = new Customer
            {
                CustomerID = customerID,
                FName = FName,
                LName = LName,
                StatusID = StatusID
            };
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                cus.Picture = "/images/" + fileName;

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _customerServices.UpdateCustomer(cus);

            return Content("/Home/Index");
        }
    }
}