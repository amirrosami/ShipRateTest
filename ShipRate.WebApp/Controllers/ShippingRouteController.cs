using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.ShippingRouteVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class ShippingRouteController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public ShippingRouteController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize("BaseOperations.ServiceAndLoops.View")]
        public async Task<IActionResult> Index()
        {
            var carriers = await _db.Carriers
                .Where(c => c.CarrierTypeId == 1 && !c.IsDeleted)
                .Include(c => c.Person)
                .OrderBy(c => c.CarrierCode)
                .Select(c => new CarrierListItem
                {
                    CarrierId = c.Id,
                    Code = c.CarrierCode ?? "",
                    FullName = c.Person != null ? c.Person.PersonName : "N/A"
                })
                .ToListAsync();

            var vm = new ShippingRoutePageViewModel { Carriers = carriers };
            return View(vm);
        }

        [HttpGet]
        [Authorize("BaseOperations.ServiceAndLoops.View")]
        public async Task<IActionResult> ServiceGrid(Guid carrierId)
        {
            var carrier = await _db.Carriers
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == carrierId);

            var services = await _db.ShippingLineServices
                .Where(s => s.ShippingLineId == carrierId && !s.IsDeleted)
                .OrderBy(s => s.ShippingLineServiceName)
                .Select(s => new ServiceListItem
                {
                    ShippingLineServiceId = s.Id,
                    ServiceName = s.ShippingLineServiceName,
                    ServiceCode = s.ShippingLineServiceCode,
                    //IsActive = s.IsActive
                })
                .ToListAsync();

            var vm = new ServiceListViewModel
            {
                CarrierId = carrierId,
                CarrierName = carrier?.Person?.PersonName ?? "N/A",
                Services = services
            };

            return PartialView("_ServiceGrid", vm);
        }

        // GET: Create service form
        [HttpGet]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public IActionResult ServiceCreate(Guid carrierId)
        {
            var vm = new ServiceFormViewModel
            {
                CarrierId = carrierId,
                IsActive = true
            };
            return PartialView("_ServiceForm", vm);
        }

        // POST: Create service
        [HttpPost]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> ServiceCreate(ServiceFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_ServiceForm", vm);

            var entity = new ShippingLineService
            {
                ShippingLineId = vm.CarrierId,
                ShippingLineServiceName = vm.ServiceName,
                ShippingLineServiceCode = vm.ServiceCode,
                //IsActive = vm.IsActive,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
            };

            _db.ShippingLineServices.Add(entity);
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "serviceChanged";
            return Content("");
        }

        // GET: Edit service form
        [HttpGet]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> ServiceEdit(Guid id)
        {
            var entity = await _db.ShippingLineServices.FindAsync(id);
            if (entity == null) return NotFound();

            var vm = new ServiceFormViewModel
            {
                ShippingLineServiceId = entity.Id,
                CarrierId = entity.ShippingLineId,
                ServiceName = entity.ShippingLineServiceName,
                ServiceCode = entity.ShippingLineServiceCode,
                //IsActive = entity.IsActive
            };

            return PartialView("_ServiceForm", vm);
        }

        // POST: Edit service
        [HttpPost]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]

        public async Task<IActionResult> ServiceEdit(ServiceFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_ServiceForm", vm);

            var entity = await _db.ShippingLineServices.FindAsync(vm.ShippingLineServiceId);
            if (entity == null) return NotFound();

            entity.ShippingLineServiceName = vm.ServiceName;
            entity.ShippingLineServiceCode = vm.ServiceCode;
            //entity.IsActive = vm.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;


            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "serviceChanged";
            return Content("");
        }

        // DELETE: Delete service
        [HttpDelete]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]

        public async Task<IActionResult> ServiceDelete(Guid id)
        {
            var entity = await _db.ShippingLineServices.FindAsync(id);
            if (entity == null) return NotFound();

            // Check if any loops reference this service
            var hasLoops = await _db.Loops
                .AnyAsync(l => l.ShippingLineServiceId == id && !l.IsDeleted);

            if (hasLoops)
            {
                Response.StatusCode = 422;
                return Content("Cannot delete: This service has associated Loops. Remove them first.");
            }

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "serviceChanged";
            return Content("");
        }

        // ============================================================
        //  GRID 3: LOOPS
        // ============================================================

        [Authorize("BaseOperations.ServiceAndLoops.View")]
        [HttpGet]
        public async Task<IActionResult> LoopGrid(Guid serviceId, int page = 1)
        {
            const int pageSize = 10;

            var service = await _db.ShippingLineServices.FindAsync(serviceId);

            var query = _db.Loops
                .Where(l => l.ShippingLineServiceId == serviceId && !l.IsDeleted);

            var totalCount = await query.CountAsync();

            var loops = await query
                .OrderBy(l => l.LoopName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(l => l.Company)
                .ThenInclude(l => l.Person)
                .Select(l => new LoopListItem
                {
                    LoopId = l.Id,
                    LoopName = l.LoopName,
                    LoopCode = l.LoopCode,
                    CompanyName = l.Company != null ? l.Company.Person.PersonName : null,
                    IsActive = l.IsActive
                })
                .ToListAsync();

            var vm = new LoopListViewModel
            {
                ShippingLineServiceId = serviceId,
                ServiceName = service?.ShippingLineServiceName ?? "N/A",
                Loops = loops,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return PartialView("_LoopGrid", vm);
        }

        [HttpGet]
        public async Task<IActionResult> LoopCreate(Guid serviceId)
        {
            var entity = await _db.ShippingLineServices.FindAsync(serviceId);


            var vm = new LoopFormViewModel
            {
                ShippingLineServiceId = serviceId,
                ShippingLineId = entity.ShippingLineId,
                IsActive = true,
                Companies = await GetCompaniesDropdown()
            };
            return PartialView("_LoopForm", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> LoopCreate(LoopFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Companies = await GetCompaniesDropdown();
                return PartialView("_LoopForm", vm);
            }

            var entityMain = await _db.ShippingLineServices.FindAsync(vm.ShippingLineServiceId);



            var entity = new Loop
            {
                ShippingLineServiceId = vm.ShippingLineServiceId,
                ShippingLineId = entityMain.ShippingLineId,
                LoopName = vm.LoopName,
                LoopCode = vm.LoopCode,
                CompanyId = vm.CompanyId.Value,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };

            _db.Loops.Add(entity);
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "loopChanged";
            return Content("");
        }

        [HttpGet]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> LoopEdit(int id)
        {
            var entity = await _db.Loops.FindAsync(id);
            if (entity == null) return NotFound();

            var vm = new LoopFormViewModel
            {
                LoopId = entity.Id,
                ShippingLineServiceId = entity.ShippingLineServiceId.Value,
                LoopName = entity.LoopName,
                LoopCode = entity.LoopCode,
                CompanyId = entity.CompanyId,
                IsActive = entity.IsActive,
                Companies = await GetCompaniesDropdown()
            };

            return PartialView("_LoopForm", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> LoopEdit(LoopFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Companies = await GetCompaniesDropdown();
                return PartialView("_LoopForm", vm);
            }

            var entity = await _db.Loops.FindAsync(vm.LoopId);
            if (entity == null) return NotFound();

            entity.LoopName = vm.LoopName;
            entity.LoopCode = vm.LoopCode;
            entity.CompanyId = vm.CompanyId.Value;
            entity.IsActive = vm.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;


            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "loopChanged";
            return Content("");
        }

        [HttpDelete]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> LoopDelete(Guid id)
        {
            var entity = await _db.Loops.FindAsync(id);
            if (entity == null) return NotFound();

            var hasPorts = await _db.LoopPorts
                .AnyAsync(lp => lp.LoopId == id && !lp.IsDeleted);

            if (hasPorts)
            {
                Response.StatusCode = 422;
                return Content("Cannot delete: This loop has associated Ports. Remove them first.");
            }

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "loopChanged";
            return Content("");
        }

        // ============================================================
        //  GRID 4: LOOP PORTS (Inline Edit)
        // ============================================================

        [HttpGet]
        [Authorize("BaseOperations.ServiceAndLoops.View")]
        public async Task<IActionResult> LoopPortGrid(Guid loopId, Guid? editingPortId = null)
        {
            var loop = await _db.Loops.FindAsync(loopId);

            var ports = await _db.LoopPorts
                .Where(lp => lp.LoopId == loopId && !lp.IsDeleted)
                .Include(lp => lp.Port)
                .OrderBy(lp => lp.SequenceNo)
                .Select(lp => new LoopPortListItem
                {
                    LoopPortId = lp.Id,
                    LoopId = lp.LoopId,
                    LocationId = lp.PortId,
                    LocationName = lp.Port != null ? lp.Port.LocationName ?? "" : "",
                    LocationCode = lp.Port != null ? lp.Port.LocationCode : null,
                    SequenceNo = lp.SequenceNo
                })
                .ToListAsync();

            var locations = await GetLocationsDropdown();

            var vm = new LoopPortListViewModel
            {
                LoopId = loopId,
                LoopName = loop?.LoopName ?? "N/A",
                Ports = ports,
                AvailableLocations = locations,
                EditingPortId = editingPortId
            };

            return PartialView("_LoopPortGrid", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> LoopPortCreate(LoopPortFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableLocations = await GetLocationsDropdown();
                return PartialView("_LoopPortInlineForm", vm);
            }

            var entity = new LoopPort
            {
                LoopId = vm.LoopId,
                PortId = vm.LocationId,
                SequenceNo = vm.SequenceNo,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
            };

            _db.LoopPorts.Add(entity);
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "loopPortChanged";
            return Content("");
        }

        [HttpPost]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]

        public async Task<IActionResult> LoopPortEdit(LoopPortFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableLocations = await GetLocationsDropdown();
                return PartialView("_LoopPortInlineForm", vm);
            }

            var entity = await _db.LoopPorts.FindAsync(vm.LoopPortId);
            if (entity == null) return NotFound();

            entity.PortId = vm.LocationId;
            entity.SequenceNo = vm.SequenceNo;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "loopPortChanged";
            return Content("");
        }

        [HttpDelete]
        [Authorize("BaseOperations.ServiceAndLoops.Edit")]
        public async Task<IActionResult> LoopPortDelete(Guid id)
        {
            var entity = await _db.LoopPorts.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "loopPortChanged";
            return Content("");
        }

        // ============================================================
        //  HELPERS
        // ============================================================

        private async Task<List<CompanyDropdownItem>> GetCompaniesDropdown()
        {
            return await _db.Companies
                .Include(x => x.Person)
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Person.PersonName)
                .Select(c => new CompanyDropdownItem
                {
                    CompanyId = c.Id,
                    CompanyName = c.Person.PersonName ?? ""
                })
                .ToListAsync();
        }

        private async Task<List<LocationDropdownItem>> GetLocationsDropdown()
        {
            return await _db.Locations
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.LocationName)
                .Select(l => new LocationDropdownItem
                {
                    LocationId = l.Id,
                    LocationName = l.LocationName ?? "",
                    LocationCode = l.LocationCode
                })
                .ToListAsync();
        }
    }
}
