using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Persons;
using ShipRate.Application.Services.Persons;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly CustomerService customerService;

        public CustomerController(CustomerService customerService)
        {
            this.customerService = customerService;
        }

        public async Task<IActionResult> Index(int page = 1, string sortColumn = "ContactName", bool sortDesc = false)
        {
            // var customers = await customerService.getAllAsync();
            var customers = new List<CustomerDto> {
            new CustomerDto{ContactName = "amir" , PersonDto = new PersonDto{ PersonName = "amirrr" ,PersonPersianName = "امیر"} } , new CustomerDto{ContactName = "ali" ,PersonDto = new PersonDto{ PersonName = "aliiii" ,PersonPersianName = "علی"}},
            new CustomerDto{ContactName = "hassan"} , new CustomerDto{ContactName = "alireza"},
            new CustomerDto{ContactName = "reza"} , new CustomerDto{ContactName = "hossein"},
            };

            ViewBag.Page = page;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDesc = sortDesc;
            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerDto customerDto)
        {
            await customerService.AddCustomer(customerDto);
            return View(customerDto);
        }

        [HttpGet]
        public async Task<IActionResult> AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task UpdateCustomer(CustomerDto customerDto)
        {
            await customerService.UpdateCustomer(customerDto);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCustomer()
        {
            return View();
        }

        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await customerService.DeleteCustomer(id);
            return View();
        }
    }
}
