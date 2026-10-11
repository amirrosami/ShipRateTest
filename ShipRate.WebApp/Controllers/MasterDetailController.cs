using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.WebApp.Models;
using ShipRate.WebApp.TagHelpers;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class MasterDetailController : Controller
    {
        [HttpPost]
        public IActionResult Index(OrderViewModel orderViewModel)
        {
            return View(orderViewModel);
        }
        public IActionResult Index()
        {
            return View(new OrderViewModel()
            {

            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new OrderViewModel
            {
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.CreateVersion7().ToString("N")[..4].ToUpper()}",
                OrderDate = DateTime.Today,
                OrderDetails = new List<OrderDetailViewModel>()
            };

            LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            //await _orderService.CreateOrderAsync(model);
            TempData["Success"] = "Order created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new OrderViewModel();  //await _orderService.GetOrderByIdAsync(id);
            if (model == null) return NotFound();

            LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            // await _orderService.UpdateOrderAsync(model);
            TempData["Success"] = "Order updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        private void LoadDropdowns()
        {
            // Load products for detail dropdown
            ViewBag.Products = new List<SelectListData>
            {
                new SelectListData { Value = "1", Text = "Laptop" },
                new SelectListData { Value = "2", Text = "Mouse" },
                new SelectListData { Value = "3", Text = "Keyboard" },
                new SelectListData { Value = "4", Text = "Monitor" },
                new SelectListData { Value = "5", Text = "Headset" },
            };
        }

    }
}
