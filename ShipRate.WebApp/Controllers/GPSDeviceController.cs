using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.TransitOperation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.TransitViewModels;
using System.Linq.Dynamic.Core;
using System.Text.Json;
namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class GPSDeviceController : Controller
    {
        private readonly ApplicationDbContext _db;
        public readonly ICurrentUserService _currentUserService;
        private const int PageSize = 10;

        public GPSDeviceController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        // ───────── Helpers ─────────
        private int GetCompanyId() => 1;
        private Guid GetUserId() => _currentUserService.UserId;

        private async Task<List<GPSProvider>> GetProviders()
        {
            return await _db.GPSProviders
                .Include(x => x.Person)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Person.PersonName)
                .ToListAsync();
        }

        private async Task<List<GPSDeviceStatus>> GetStatuses()
        {
            return await _db.GPSDeviceStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        private IActionResult HtmxSuccess(string trigger)
        {
            Response.Headers.Add("HX-Trigger", trigger);
            return Ok();
        }

        [Authorize("Transit.GPSDevices.View")]
        public async Task<IActionResult> Index()
        {

            var result = await getFilteredList(new PagedRequestDto(), Guid.Empty, 0);
            var Providers = await GetProviders();
            var Statuses = await GetStatuses();
            ViewBag.Providers = new SelectList(Providers.Select(x => new { Id = x.Id, Name = x.Person.PersonName }), "Id", "Name");
            ViewBag.Statuses = new SelectList(Statuses.Select(x => new { Id = x.Id, Name = x.GPSDeviceStatusName }), "Id", "Name");

            return View(result);
        }

        [Authorize("Transit.GPSDevices.View")]
        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request, Guid searchProviderId, int searchStatusId)
        {
            var result = await getFilteredList(request, searchProviderId, searchStatusId);

            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "GPSDevice/List",
                GridPartialName = "_List",
                columns = GetGridColumns()
            };

            return PartialView("_Grid", gridVm);
        }

        private List<ViewModels.GridColumn> GetGridColumns()
        {
            return new List<ViewModels.GridColumn> {
            new ViewModels.GridColumn("GPSDeviceSerialNumber","Serial Number"),
            new ViewModels.GridColumn("ProviderName","Provider"),
            new ViewModels.GridColumn("Status","Status"),
            new ViewModels.GridColumn("Remarks","Remarks")
            //new ViewModels.GridColumn("IsActive","IsActive")
            };
        }

        private async Task<ViewModels.PagedResult<GPSDeviceListItem>> getFilteredList(PagedRequestDto dto, Guid searchProviderId, int searchStatusId)
        {
            var companyId = GetCompanyId();

            var query = _db.GPSDevices
                .Include(d => d.GPSProvider)
                .ThenInclude(x => x.Person)
                .Include(d => d.GPSDeviceStatus)
                .Where(d => !d.IsDeleted && d.CompanyId == companyId);


            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                query = query.Where(x => x.GPSDeviceSerialNumber.Contains(dto.SearchTerm) || x.Remarks.Contains(dto.SearchTerm));
            }
            if (searchProviderId != Guid.Empty)
            {
                query = query.Where(x => x.GPSProviderId == searchProviderId);
            }
            if (searchStatusId > 0)
            {
                query = query.Where(x => x.GPSDeviceStatusId == searchStatusId);
            }

            var selectquery = query.Select(d => new GPSDeviceListItem
            {
                Id = d.Id,
                GPSDeviceSerialNumber = d.GPSDeviceSerialNumber,
                ProviderName = d.GPSProvider.Person.PersonName,
                StatusName = d.GPSDeviceStatus.GPSDeviceStatusName ?? d.GPSDeviceStatus.GPSDeviceStatusName,
                GPSDeviceStatusId = d.GPSDeviceStatusId,
                Remarks = d.Remarks,
                //IsActive = d.IsActive
            });

            int total = await selectquery.CountAsync();

            if (!string.IsNullOrEmpty(dto.SortColumn) && !string.IsNullOrEmpty(dto.SortDirection))
            {
                selectquery = selectquery.OrderBy($"{dto.SortColumn}  {dto.SortDirection}");
            }
            selectquery = selectquery.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);



            return new ViewModels.PagedResult<GPSDeviceListItem>()
            {
                Items = await selectquery.ToListAsync(),
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection,
                TotalCount = total,

            };

        }


        [Authorize("Transit.GPSDevices.Edit")]
        public async Task<IActionResult> Create()
        {
            var providers = await GetProviders();
            var statuses = await GetStatuses();
            var model = new GPSDeviceFormViewModel
            {
                Providers = providers.Select(x => new SelectListItem(x.Person.PersonName, x.Id.ToString())).ToList(),
                Statuses = statuses.Select(x => new SelectListItem(x.GPSDeviceStatusName, x.Id.ToString())).ToList()
            };
            ViewBag.CreateEdit = "Create";

            return PartialView("_Form", model);
        }

        [HttpPost]
        [Authorize("Transit.GPSDevices.Edit")]
        public async Task<IActionResult> Create(GPSDeviceFormViewModel model)
        {


            ModelState.Clear();
            var entity = new GPSDevice
            {
                Id = Guid.CreateVersion7(),
                CompanyId = GetCompanyId(),
                GPSDeviceSerialNumber = model.GPSDeviceSerialNumber.Trim(),
                GPSProviderId = model.GPSProviderId,
                GPSDeviceStatusId = model.GPSDeviceStatusId,
                Remarks = model.Remarks?.Trim() ?? "",
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = GetUserId()
            };


            // validate:
            var validation = new GPSDeviceValidation().Validate(entity);
            if (!validation.IsValid)
            {
                foreach (var item in validation.Errors)
                {
                    ModelState.AddModelError("", item.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                var providers = await GetProviders();
                var statuses = await GetStatuses();
                ViewBag.CreateEdit = "Create";
                model.Providers = providers.Select(x => new SelectListItem(x.Person.PersonName, x.Id.ToString())).ToList();
                model.Statuses = statuses.Select(x => new SelectListItem(x.GPSDeviceStatusName, x.Id.ToString())).ToList();
                return PartialView("_Form", model);
            }
            _db.GPSDevices.Add(entity);
            await _db.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        // ───────── Edit (GET) ─────────
        [Authorize("Transit.GPSDevices.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _db.GPSDevices.FindAsync(id);
            if (entity == null) return NotFound();

            var model = new GPSDeviceFormViewModel
            {
                Id = entity.Id,
                GPSDeviceSerialNumber = entity.GPSDeviceSerialNumber,
                GPSProviderId = entity.GPSProviderId,
                GPSDeviceStatusId = entity.GPSDeviceStatusId,
                Remarks = entity.Remarks,
                //IsActive = entity.IsActive
            };
            var providers = await GetProviders();
            var statuses = await GetStatuses();
            ViewBag.CreateEdit = "Edit";

            model.Providers = providers.Select(x => new SelectListItem(x.Person.PersonName, x.Id.ToString())).ToList();
            model.Statuses = statuses.Select(x => new SelectListItem(x.GPSDeviceStatusName, x.Id.ToString())).ToList();
            return PartialView("_Form", model);
        }

        // ───────── Edit (POST) ─────────
        [HttpPost]
        [Authorize("Transit.GPSDevices.Edit")]
        public async Task<IActionResult> Edit(GPSDeviceFormViewModel model)
        {
            ModelState.Clear();
            var entity = await _db.GPSDevices.FindAsync(model.Id);
            if (entity == null) return NotFound();

            entity.GPSDeviceSerialNumber = model.GPSDeviceSerialNumber.Trim();
            entity.GPSProviderId = model.GPSProviderId;
            entity.GPSDeviceStatusId = model.GPSDeviceStatusId;
            entity.Remarks = model.Remarks?.Trim() ?? "";
            //entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = GetUserId();

            var validation = new GPSDeviceValidation().Validate(entity);
            if (!validation.IsValid)
            {
                foreach (var item in validation.Errors)
                {
                    ModelState.AddModelError("", item.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.CreateEdit = "Edit";
                var providers = await GetProviders();
                var statuses = await GetStatuses();
                model.Providers = providers.Select(x => new SelectListItem(x.Person.PersonName, x.Id.ToString())).ToList();
                model.Statuses = statuses.Select(x => new SelectListItem(x.GPSDeviceStatusName, x.Id.ToString())).ToList();
                return PartialView("_Form", model);
            }
            await _db.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }

        [HttpGet]
        [Authorize("Transit.GPSDevices.Edit")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        [Authorize("Transit.GPSDevices.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await _db.GPSDevices.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = GetUserId();

            await _db.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });

            return Ok();
        }
    }
}