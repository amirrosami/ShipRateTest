using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using System.Security.Claims;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ProductStockTransactionsController : Controller
    {
        private readonly IProductStockTransactionService _service;

        public ProductStockTransactionsController(IProductStockTransactionService service)
        {
            _service = service;
        }
        [Authorize("Inventory.ProductStockTransactions.View")]
        public async Task<IActionResult> Index(ProductStockTransactionFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _service.GetPagedAsync(filter, cancellationToken);

            ViewBag.Filter = filter;
            ViewBag.Inventories = await _service.GetInventorySelectListAsync(cancellationToken);
            ViewBag.Products = await _service.GetProductSelectListAsync(cancellationToken);

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_List", result);

            return View(result);
        }

        [HttpGet]
        [Authorize("Inventory.ProductStockTransactions.Edit")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var vm = await _service.GetCreateVmAsync(cancellationToken);
            return PartialView("_Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.ProductStockTransactions.Edit")]
        public async Task<IActionResult> Create(
            ProductStockTransactionCreateVm vm,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var createVm = await _service.GetCreateVmAsync(cancellationToken);
                createVm.Input = vm.Input;
                return PartialView("_Create", createVm);
            }

            try
            {
                await _service.CreateAsync(vm.Input, GetCurrentUserId(), cancellationToken);

                Response.Headers["HX-Trigger"] = "productStockTransactionChanged";

                return PartialView("_CreateSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var createVm = await _service.GetCreateVmAsync(cancellationToken);
                createVm.Input = vm.Input;

                return PartialView("_Create", createVm);
            }
        }

        [Authorize("Inventory.ProductStockTransactions.View")]
        public async Task<IActionResult> InventoryStocks(
            InventoryProductStockFilterDto filter,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetInventoryStocksAsync(filter, cancellationToken);

            ViewBag.Filter = filter;
            ViewBag.Inventories = await _service.GetInventorySelectListAsync(cancellationToken);
            ViewBag.Products = await _service.GetProductSelectListAsync(cancellationToken);

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_InventoryStocksList", result);

            return View(result);
        }

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(value, out var userId))
                throw new InvalidOperationException("Current user id was not found.");

            return userId;
        }

        [HttpGet]
        [Authorize("Inventory.ProductStockTransactions.Deliver")]
        public async Task<IActionResult> Deliver(Guid id, CancellationToken cancellationToken)
        {
            var vm = await _service.GetDeliverVmAsync(id, cancellationToken);
            return PartialView("_Deliver", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.ProductStockTransactions.Deliver")]
        public async Task<IActionResult> Deliver(DeliverProductRequestVm vm, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeliverAsync(vm.Input, GetCurrentUserId(), cancellationToken);

                Response.Headers["HX-Trigger"] = "productRequestDelivered";
                return PartialView("_DeliverSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var newVm = await _service.GetDeliverVmAsync(vm.Input.ProductRequestId, cancellationToken);
                newVm.Input = vm.Input;

                return PartialView("_Deliver", newVm);
            }

        }
    }
}