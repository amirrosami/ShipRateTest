using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Infrastructure.Data;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class InventoryController : Controller
    {
        private readonly IInventoryService _service;
        private readonly ApplicationDbContext _db;

        public InventoryController(IInventoryService service, ApplicationDbContext db)
        {
            _service = service;
            _db = db;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        [Authorize("Inventory.Inventories.View")]
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetPagedAsync(new PagedRequestDto());
            return View(result);
        }


        [HttpGet]
        [Authorize("Inventory.Inventories.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await _service.GetPagedAsync(request);
            return PartialView("_List", result);
        }


        public async Task GetDropDowns()
        {
            var companies = await _db.Companies
                .Include(x => x.Person)
                .Where(c => c.IsActive)
                .Select(c => new { c.Id, c.Person.PersonName })
                .ToListAsync();
            ViewBag.Companies = new SelectList(companies, "Id", "PersonName");
        }



        [HttpGet]
        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> Create()
        {
            await GetDropDowns();
            return PartialView("_FormCreate", new InventoryCreateDto { });
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> Create(InventoryCreateDto dto)
        {
            // Check unique code
            //if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode))
            //{
            //    ModelState.AddModelError("Code", "This code already exists.");
            //}

            //if (!ModelState.IsValid)
            //{
            //    Response.Headers.Append("HX-Retarget", "#modal-body");
            //    Response.Headers.Append("HX-Reswap", "innerHTML");
            //    return PartialView("_FormCreate", dto);
            //}

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                await GetDropDowns();
                return PartialView("_FormCreate", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await GetDropDowns();
                return PartialView("_FormCreate", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshInventoryList = new { },
                showToast = new { message = "Inventory created successfully.", type = "success" }
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await GetDropDowns();
            var dto = new InventoryUpdateDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                InventoryCode = entity.InventoryCode,
                InventoryName = entity.InventoryName,
                Remarks = entity.Remarks
            };
            return PartialView("_FormEdit", dto);
        }
        // POST: /PackageTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> Edit(InventoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                await GetDropDowns();
                return PartialView("_FormEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await GetDropDowns();
                return PartialView("_FormEdit", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshInventoryList = new { },
                showToast = new { message = "Inventory updated successfully.", type = "success" }
            });
            return Content("");
        }

        // GET: /PackageTypes/DeleteConfirm/5

        [HttpGet]
        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    closeModal = new { },
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshInventoryList = new { },
                showToast = new { message = "Inventory deleted successfully.", type = "success" }
            });

            return Content("");
        }

        [HttpPost]
        [Authorize("Inventory.Inventories.Edit")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var (success, error) = await _service.ToggleActiveAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                refreshList = new { },
                showToast = new { message = "Status toggled.", type = "info" }
            });

            return Content("");
        }
    }
}
