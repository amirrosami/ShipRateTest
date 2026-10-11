using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.VoyageVms;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers;

[Authorize]
public class VoyageController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly IContainerActionService _containerActionService;
    private const int PageSize = 15;

    public VoyageController(ApplicationDbContext db, ICurrentUserService currentUserService, IContainerActionService containerActionService)
    {
        _db = db;
        _currentUserService = currentUserService;
        _containerActionService = containerActionService;
    }

    // ══════════════════════════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════════════════════════

    private int GetCompanyId() => 1;


    private Guid GetUserId() =>
        _currentUserService.UserId;

    /// <summary>
    /// Returns a small HTML success message + sets HX-Trigger header
    /// so the client-side JS refreshes the relevant grid.
    /// </summary>
    /// 
    private void HxTrigger(object triggers)
    {
        Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
    }

    private IActionResult HtmxSuccess(string triggerEvent)
    {
        Response.Headers["HX-Trigger"] = triggerEvent;
        return Content(
            "<div class='text-center text-success py-4'>" +
            "<i class='bi bi-check-circle-fill fs-1'></i>" +
            "<p class='mt-2 mb-0'>Operation completed successfully!</p>" +
            "</div>",
            "text/html");
    }

    // ── Dropdown Population ──

    private async Task PopulateVoyageDropdowns(VoyageFormViewModel vm)
    {
        var companyId = GetCompanyId();

        vm.Vessels = await _db.Vessels
            .Where(v => !v.IsDeleted)
            .OrderBy(v => v.VesselName)
            .Select(v => new SelectListItem(v.VesselName, v.Id.ToString()))
            .ToListAsync();

        vm.ShippingLines = await _db.Carriers
            .Include(X => X.Person)
            .Where(s => !s.IsDeleted && s.CarrierTypeId == 1)
            .OrderBy(s => s.Person.PersonName)
            .Select(s => new SelectListItem(s.Person.PersonName, s.Id.ToString()))
            .ToListAsync();

        //var ports = await _db.Locations
        //    .Where(p => !p.IsDeleted && p.IsActive && p.LocationTypeId == 2)
        //    .OrderBy(p => p.LocationName)
        //    .Select(p => new SelectListItem(p.LocationName, p.Id.ToString()))
        //    .ToListAsync();

    }

    private async Task PopulateVoyagePortDropdowns(VoyagePortFormViewModel vm)
    {
        var companyId = GetCompanyId();

        //vm.Ports = await _db.Locations
        //    .Where(p => !p.IsDeleted && p.IsActive && p.LocationTypeId == 2)
        //    .OrderBy(p => p.LocationName)
        //    .Select(p => new SelectListItem(p.LocationName, p.Id.ToString()))
        //    .ToListAsync();

        vm.Terminals = await _db.Locations
            .Where(t => !t.IsDeleted && t.IsActive && t.LocationTypeId == 6)
            .OrderBy(t => t.LocationName)
            .Select(t => new SelectListItem(t.LocationName, t.Id.ToString()))
            .ToListAsync();

        vm.OutboundVoyages = await _db.Voyages
            .Where(v => !v.IsDeleted && v.CompanyId == companyId)
            .OrderByDescending(v => v.CreatedAt)
            .Select(v => new SelectListItem(
                $"{v.VoyageNo} ({v.Vessel!.VesselName})", v.Id.ToString()))
            .ToListAsync();


        //vm.ShippingLineAgents = await _db.A
        //    .Where(a => !a.IsDeleted && a.IsActive && a.CompanyId == companyId)
        //    .OrderBy(a => a.AgentName)
        //    .Select(a => new SelectListItem(a.AgentName, a.Id.ToString()))
        //    .ToListAsync();
    }

    private async Task PopulateEventDropdowns(VoyagePortEventFormViewModel vm)
    {
        vm.EventTypes = await _db.EventTypes
            .Where(e => !e.IsDeleted && e.IsActive && e.CategoryId == 1)
            .OrderBy(e => e.Id)
            .Select(e => new SelectListItem(e.EventTypeName, e.Id.ToString()))
            .ToListAsync();
    }

    [Authorize("BaseOperations.Voyages.View")]
    public async Task<IActionResult> Index()
    {
        var result = await getFilteredList(new PagedRequestDto { });
        return View(result);
    }


    private async Task<PagedResult<VoyageListItem>> getFilteredList(PagedRequestDto dto)
    {
        var data = _db.Voyages
            .Where(x => !x.IsDeleted)
            .Include(x => x.Vessel)
            .Include(x => x.ShippingLine)
            .Include(x => x.ShippingLine.Person)
            .Include(x => x.DestinationPort)
            .Include(x => x.OriginPort)
            .AsQueryable();

        if (!string.IsNullOrEmpty(dto.SearchTerm))
        {
            data = data.Where(x => x.VoyageNo.Contains(dto.SearchTerm) || x.Vessel.VesselName.Contains(dto.SearchTerm) || x.ShippingLine.Person.PersonName.Contains(dto.SearchTerm));
        }

        int total = await data.CountAsync();

        data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

        var items = await data.Select(x => new VoyageListItem
        {
            Id = x.Id,
            VoyageNo = x.VoyageNo,
            ShippingLineName = x.ShippingLine.Person.PersonName,
            VesselName = x.Vessel.VesselName,
            TransitTime = x.TransitTime,
            VoyageEndDate = x.VoyageEndDate,
            VoyageStartDate = x.VoyageStartDate,
            OriginPortName = x.OriginPort.LocationName,
            DestinationPortName = x.DestinationPort.LocationName,
            VoyageStatusId = x.VoyageStatusId,
            //IsActive = x.IsActive
        }).ToListAsync();

        return new PagedResult<VoyageListItem>()
        {
            Items = items,
            Page = dto.Page,
            PageSize = dto.PageSize,
            SearchTerm = dto.SearchTerm,
            CustomFilterValues = new { },
            SortColumn = dto.SortColumn,
            SortDirection = dto.SortDirection,
            TotalCount = total,
        };
    }

    [Authorize("BaseOperations.Voyages.View")]
    public async Task<IActionResult> LoadGrid(PagedRequestDto request)
    {
        var result = await getFilteredList(request);
        var gridVm = new GridViewModel
        {
            Data = result.toObject(),
            ListUrl = "Voyage/LoadGrid",
            GridPartialName = "_VoyageGrid",
            columns = GetGridColumns()

        };
        return PartialView("_Grid", gridVm);
    }

    private List<GridColumn> GetGridColumns()
    {
        return new List<GridColumn>()
            {
                new GridColumn("VoyageNo","Voyage No"),
                new GridColumn("VesselName","Vessel") ,
                new GridColumn("ShippingLineName","Shipping Line"),
                new GridColumn("OriginPortName","Origin Port"),
                new GridColumn("DestinationPortName","Destination Port"),
                new GridColumn("VoyageStartDate","Start Date"),
                new GridColumn("VoyageEndDate","End Date"),
                new GridColumn("TransitTime","End Date")
            };
    }


    [HttpGet]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> CreateVoyage()
    {
        var vm = new VoyageFormViewModel { CompanyId = GetCompanyId() };
        await PopulateVoyageDropdowns(vm);
        ViewBag.CreateEdit = "Create";
        return PartialView("_VoyageForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> CreateVoyage(VoyageFormViewModel vm)
    {
        ModelState.Clear();
        await _db.Database.BeginTransactionAsync();
        var entity = new Voyage
        {
            Id = Guid.CreateVersion7(),
            CompanyId = GetCompanyId(),
            VesselId = vm.VesselId,
            VoyageNo = vm.VoyageNo,
            ShippingLineId = vm.ShippingLineId,
            OriginPortId = vm.OriginPortId,
            DestinationPortId = vm.DestinationPortId,
            VoyageStatusId = 1,
            VoyageStartDate = vm.VoyageStartDate,
            VoyageEndDate = vm.VoyageEndDate,
            TransitTime = vm.TransitTime,
            Remarks = string.Empty,
            //IsActive = true,
            CreatedAt = DateTime.Now,
            CreateUserId = GetUserId()
        };

        var validation = new VoyageValidation().Validate(entity);
        if (!validation.IsValid)
        {
            foreach (var err in validation.Errors)
            {
                ModelState.AddModelError("", err.ErrorMessage);
            }
            ViewBag.CreateEdit = "Create";
            await PopulateVoyageDropdowns(vm);
            return PartialView("_VoyageForm", vm);
        }

        _db.Voyages.Add(entity);
        try
        {
            await _db.SaveChangesAsync();
            await _db.Database.CommitTransactionAsync();
            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                DetailDetailChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }
        catch (Exception)
        {
            await _db.Database.RollbackTransactionAsync();
            ModelState.AddModelError("", "error in save data");
            ViewBag.CreateEdit = "Create";
            await PopulateVoyageDropdowns(vm);
            return PartialView("_VoyageForm", vm);
        }

    }


    [HttpGet]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> EditVoyage(Guid id)
    {
        var companyId = GetCompanyId();
        var entity = await _db.Voyages
            .Include(x => x.OriginPort)
            .Include(x => x.DestinationPort)
            .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId && !v.IsDeleted);

        if (entity == null) return NotFound();

        var vm = new VoyageFormViewModel
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            VesselId = entity.VesselId,
            VoyageNo = entity.VoyageNo,
            ShippingLineId = entity.ShippingLineId,
            OriginPortId = entity.OriginPortId,
            DestinationPortId = entity.DestinationPortId,
            VoyageStatusId = entity.VoyageStatusId,
            VoyageStartDate = entity.VoyageStartDate,
            VoyageEndDate = entity.VoyageEndDate,
            TransitTime = entity.TransitTime,
            DestinationPortName = entity.DestinationPort.LocationName,
            OriginPortName = entity.OriginPort.LocationName,
            //VoyageRemarks = entity.VoyageRemarks,
            //IsActive = entity.IsActive
        };

        await PopulateVoyageDropdowns(vm);
        return PartialView("_VoyageForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> EditVoyage(VoyageFormViewModel vm)
    {
        ModelState.Clear();

        var companyId = GetCompanyId();
        var entity = await _db.Voyages
            .FirstOrDefaultAsync(v => v.Id == vm.Id && v.CompanyId == companyId && !v.IsDeleted);

        if (entity == null) return NotFound();

        entity.VesselId = vm.VesselId;
        entity.VoyageNo = vm.VoyageNo;
        entity.ShippingLineId = vm.ShippingLineId;
        entity.OriginPortId = vm.OriginPortId;
        entity.DestinationPortId = vm.DestinationPortId;
        entity.VoyageStatusId = vm.VoyageStatusId ?? 1;
        entity.VoyageStartDate = vm.VoyageStartDate;
        entity.VoyageEndDate = vm.VoyageEndDate;
        entity.TransitTime = vm.TransitTime;
        entity.UpdatedAt = DateTime.Now;
        entity.EditUserId = GetUserId();
        var validation = new VoyageValidation().Validate(entity);
        if (!validation.IsValid)
        {
            foreach (var item in validation.Errors)
            {
                ModelState.AddModelError("", item.ErrorMessage);
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateVoyageDropdowns(vm);
            ViewBag.CreateEdit = "Edit";
            return PartialView("_VoyageForm", vm);
        }
        await _db.SaveChangesAsync();
        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Edited Sussefully"
        });
        return Ok();
    }

    // ── Delete Voyage (Soft Delete) ──


    [HttpGet]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> DeleteVoyage(Guid id)
    {
        return PartialView("_DeleteConfirm", id);
    }

    // DELETE: HSCodes/Delete/id
    [HttpPost]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> DeleteVoyageConfirmed(Guid id)
    {
        var hsCode = await _db.Voyages.FindAsync(id);
        if (hsCode == null) return NotFound();

        hsCode.IsDeleted = true;
        hsCode.UpdatedAt = DateTime.Now;
        hsCode.EditUserId = Guid.Empty; // Replace with actual user ID

        await _db.SaveChangesAsync();

        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Deleted Sussefully"
        });
        return Ok();
    }

    [HttpGet]
    [Authorize("BaseOperations.Voyages.View")]
    public async Task<IActionResult> LoadVoyagePorts(Guid voyageId)
    {
        var voyage = _db.Voyages.Where(x => x.Id == voyageId).FirstOrDefault();
        if (voyage == null)
        {
            return Content("");
        }
        var voyageports = _db.VoyagePorts
            .Include(c => c.Port)
            .Include(x => x.Terminal)
            .Include(x => x.OutboundVoyage)
            .Where(c => c.InboundVoyageId == voyageId && !c.IsDeleted)
            .Select(c => new VoyagePortListItem
            {
                Id = c.Id,
                InboundVoyageId = voyageId,
                PortName = c.Port.LocationName,
                ATA = c.ATA,
                ATD = c.ATD,
                ETA = c.ETA,
                ETD = c.ETD,
                VoyagePortSequence = c.VoyagePortSequence,
                TerminalName = c.Terminal.LocationName,
                //IsActive = c.IsActive,
                OutboundVoyageNo = c.OutboundVoyage.VoyageNo
            });


        var result = new DetailGridViewModel
        {
            Controller = "Voyage",
            MasterTitle = "Voyage Ports",
            MasterDisplayValue = $"Voyage No: {voyage.VoyageNo}",
            MasterId = voyageId,
            Columns = new List<GridColumn> {new GridColumn("VoyagePortSequence", "Port Sequence"),
            new GridColumn("PortName","PortName"),new GridColumn("TerminalName","Terminal"),
            new GridColumn("ETA","ETA"),new GridColumn("ETD","ETD"),new GridColumn("ATA","ATA"),
            new GridColumn("ATD","ATD") ,new GridColumn("OutboundVoyageNo","Outbound Voyage No")},

            Data = await voyageports.Select(x => (object)x).ToListAsync(),
            ListUrl = "Voyages/LoadVoyagePorts",
            DetailGridPartialName = "_VoyagePortGrid",
            CreateAction = "CreateVoyagePort"
        };
        return PartialView("_DetailGrid", result);
    }



    [HttpGet]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> CreateVoyagePort(Guid voyageId)
    {
        var vm = new VoyagePortFormViewModel
        {
            InboundVoyageId = voyageId,
            VoyagePortSequence = await GetNextPortSequence(voyageId)
        };
        ViewBag.CreateEdit = "Create";
        await PopulateVoyagePortDropdowns(vm);
        return PartialView("_VoyagePortForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> CreateVoyagePort(VoyagePortFormViewModel vm)
    {
        ModelState.Clear();

        var entity = new VoyagePort
        {
            Id = Guid.CreateVersion7(),
            InboundVoyageId = vm.InboundVoyageId,
            VoyagePortSequence = vm.VoyagePortSequence,
            PortId = vm.PortId,
            TerminalId = vm.TerminalId,
            ETA = vm.ETA,
            ETD = vm.ETD,
            ATA = vm.ATA,
            ATD = vm.ATD,
            OutboundVoyageId = vm.OutboundVoyageId,
            ShippingLineAgentId = Guid.NewGuid(),
            //IsActive = vm.IsActive,
            CreatedAt = DateTime.Now,
            CreateUserId = GetUserId(),

        };
        var validation = new VoyagePortValidation().Validate(entity);
        if (!validation.IsValid)
        {
            foreach (var item in validation.Errors)
            {
                ModelState.AddModelError("", item.ErrorMessage);
            }

        }
        if (!ModelState.IsValid)
        {
            await PopulateVoyagePortDropdowns(vm);
            ViewBag.CreateEdit = "Create";
            return PartialView("_VoyagePortForm", vm);
        }
        _db.VoyagePorts.Add(entity);
        await _db.SaveChangesAsync();
        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Created Sussefully"
        });
        return Ok();
    }

    // ── Edit VoyagePort ──

    [HttpGet]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> EditVoyagePort(Guid id)
    {
        var entity = await _db.VoyagePorts
            .Include(x => x.Port)
            .FirstOrDefaultAsync(vp => vp.Id == id && !vp.IsDeleted);

        if (entity == null) return NotFound();

        var vm = new VoyagePortFormViewModel
        {
            Id = entity.Id,
            InboundVoyageId = entity.InboundVoyageId,
            VoyagePortSequence = entity.VoyagePortSequence,
            PortId = entity.PortId,
            PortName = entity.Port.LocationName,
            TerminalId = entity.TerminalId,
            ETA = entity.ETA,
            ETD = entity.ETD,
            ATA = entity.ATA,
            ATD = entity.ATD,
            OutboundVoyageId = entity.OutboundVoyageId,
            ShippingLineAgentId = entity.ShippingLineAgentId,
            //IsActive = entity.IsActive
        };
        ViewBag.CreateEdit = "Edit";

        await PopulateVoyagePortDropdowns(vm);
        return PartialView("_VoyagePortForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> EditVoyagePort(VoyagePortFormViewModel vm)
    {
        ModelState.Clear();

        var entity = await _db.VoyagePorts
            .FirstOrDefaultAsync(vp => vp.Id == vm.Id && !vp.IsDeleted);

        if (entity == null) return NotFound();

        entity.VoyagePortSequence = vm.VoyagePortSequence;
        entity.PortId = vm.PortId;
        entity.TerminalId = vm.TerminalId;
        entity.ETA = vm.ETA;
        entity.ETD = vm.ETD;
        entity.ATA = vm.ATA;
        entity.ATD = vm.ATD;
        entity.OutboundVoyageId = vm.OutboundVoyageId;
        entity.ShippingLineAgentId = vm.ShippingLineAgentId;
        //entity.IsActive = vm.IsActive;
        entity.UpdatedAt = DateTime.Now;
        entity.EditUserId = GetUserId();
        var validation = new VoyagePortValidation().Validate(entity);
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
            await PopulateVoyagePortDropdowns(vm);
            return PartialView("_VoyagePortForm", vm);
        }

        await _db.SaveChangesAsync();
        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Edited Sussefully"
        });
        return Ok();
    }

    [HttpGet]
    [Authorize("BaseOperations.Voyages.Edit")]
    public async Task<IActionResult> DeleteVoyagePort(Guid id)
    {
        return PartialView("_DeleteVoyagePortConfirm", id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteVoyagePortConfirmed(Guid id)
    {
        var entity = await _db.VoyagePorts
            .FirstOrDefaultAsync(vp => vp.Id == id && !vp.IsDeleted);

        if (entity == null) return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.Now;
        entity.EditUserId = GetUserId();

        await _db.SaveChangesAsync();

        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Deleted Sussefully"
        });

        return Ok();
    }

    // ── Helper: Next Sequence ──

    private async Task<int> GetNextPortSequence(Guid voyageId)
    {
        var maxSeq = await _db.VoyagePorts
            .Where(vp => vp.InboundVoyageId == voyageId && !vp.IsDeleted)
            .MaxAsync(vp => (int?)vp.VoyagePortSequence);

        return (maxSeq ?? 0) + 1;
    }

    [HttpGet]
    [Authorize("BaseOperations.Voyages.View")]
    public async Task<IActionResult> LoadVoyagePortEvents(Guid voyagePortId)
    {
        var voyagePort = _db.VoyagePorts.Include(x => x.Port).Where(x => x.Id == voyagePortId).FirstOrDefault();

        if (voyagePort == null)
        {
            return Content("");
        }
        var items = _db.VoyagePortEvents
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(e => !e.IsDeleted && e.VoyagePortId == voyagePortId)
            .Include(e => e.VoyageEventType)
            .OrderByDescending(e => e.VoyagePortEventDateTime)
            .Select(e => new VoyagePortEventListItem
            {
                Id = e.Id,
                VoyagePortId = e.VoyagePortId,
                EventTypeName = e.VoyageEventType!.EventTypeName,
                VoyagePortEventDateTime = e.VoyagePortEventDateTime,
                VoyagePortEventRemarks = e.VoyagePortEventRemarks,
                //IsActive = e.IsActive,
                CreateUserName = e.CreateUser.UserName,
                UpdateUserName = e.UpdateUser.UserName,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
            });
        ViewBag.VoyagePortId = voyagePortId;
        var result = new DetailGridViewModel
        {
            Controller = "Voyage",
            MasterTitle = "Voyage Port Events",
            MasterDisplayValue = $"Port Name: {voyagePort.Port.LocationName}",
            MasterId = voyagePortId,
            Columns = new List<GridColumn> {new GridColumn("EventTypeName", "Event Type"),
              new GridColumn("VoyagePortEventDateTime","Event Date Time")
            , new GridColumn("VoyagePortEventRemarks","Remarks")
            , new GridColumn("CreateUserName","Create User")
            , new GridColumn("CreatedAt","Created At")
            , new GridColumn("UpdateUserName","Update User")
            , new GridColumn("UpdatedAt","Updated At")
            },
            Data = await items.Select(x => (object)x).ToListAsync(),
            ListUrl = "Voyages/LoadVoyagePortEvents",
            DetailGridPartialName = "_VoyagePortEventGrid",
            CreateAction = "CreateVoyagePortEvent"
        };
        return PartialView("_DetailDetailGrid", result);
    }

    // ── Create VoyagePortEvent ──

    [HttpGet]
    [Authorize("BaseOperations.Voyages.EditPortEvents")]
    public async Task<IActionResult> CreateVoyagePortEvent(Guid voyagePortId)
    {
        var vm = new VoyagePortEventFormViewModel
        {
            VoyagePortId = voyagePortId,
            VoyagePortEventDateTime = DateTime.Now
        };

        var vp = await _db.VoyagePorts
            .Include(x => x.InboundVoyage)
            .Include(x => x.OutboundVoyage)
            .Include(x => x.Port)
            .Where(x => x.Id == vm.VoyagePortId)
            .FirstOrDefaultAsync();

        ViewBag.VoyageNo = vp.InboundVoyage.VoyageNo;
        ViewBag.OutboundVoyageNo = vp.OutboundVoyage?.VoyageNo;
        ViewBag.PortName = vp.Port.LocationName;
        ViewBag.CreateEdit = "Create";
        await PopulateEventDropdowns(vm);
        return PartialView("_VoyagePortEventForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize("BaseOperations.Voyages.EditPortEvents")]
    public async Task<IActionResult> CreateVoyagePortEvent(VoyagePortEventFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CreateEdit = "Create";
            await PopulateEventDropdowns(vm);
            return PartialView("_VoyagePortEventForm", vm);
        }

        if (await _db.VoyagePortEvents.AnyAsync(x => x.VoyagePortId == vm.VoyagePortId && x.EventTypeId == vm.EventTypeId))
        {
            ModelState.AddModelError("DuplicateEvent", "The event already exists!!!!");
            ViewBag.CreateEdit = "Create";
            await PopulateEventDropdowns(vm);
            return PartialView("_VoyagePortEventForm", vm);
        }




        var (isValidSequence, sequenceError) = await ValidateEventSequenceAsync(
               vm.VoyagePortId,
               vm.EventTypeId,
               vm.VoyagePortEventDateTime);

        if (!isValidSequence)
        {
            ModelState.AddModelError(nameof(vm.EventTypeId), sequenceError);
            ViewBag.CreateEdit = "Create";
            await PopulateEventDropdowns(vm);
            return PartialView("_VoyagePortEventForm", vm);
        }









        var entity = new VoyagePortEvent
        {
            Id = Guid.CreateVersion7(),
            VoyagePortId = vm.VoyagePortId,
            EventTypeId = vm.EventTypeId,
            VoyagePortEventDateTime = vm.VoyagePortEventDateTime,
            VoyagePortEventRemarks = vm.VoyagePortEventRemarks,
            //IsActive = vm.IsActive,
            CreatedAt = DateTime.Now,
            CreateUserId = GetUserId()
        };

        _db.VoyagePortEvents.Add(entity);
        await _db.SaveChangesAsync();
        if (entity.EventTypeId == 121)   //پایان تخلیه کشتی در بندر 
        {
            var b1 = await _containerActionService.RegisterDischargeActionsForFinalPortVoyageAsync(entity.VoyagePortId);
        }
        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Created Sussefully"
        });
        return Ok();
    }

    // ── Edit VoyagePortEvent ──

    [HttpGet]
    [Authorize("BaseOperations.Voyages.EditPortEvents")]
    public async Task<IActionResult> EditVoyagePortEvent(Guid id)
    {
        var entity = await _db.VoyagePortEvents
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        if (entity == null) return NotFound();

        var vm = new VoyagePortEventFormViewModel
        {
            Id = entity.Id,
            VoyagePortId = entity.VoyagePortId,
            EventTypeId = entity.EventTypeId,
            VoyagePortEventDateTime = entity.VoyagePortEventDateTime,
            VoyagePortEventRemarks = entity.VoyagePortEventRemarks,
            //IsActive = entity.IsActive
        };
        ViewBag.CreateEdit = "Edit";
        await PopulateEventDropdowns(vm);
        return PartialView("_VoyagePortEventForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize("BaseOperations.Voyages.EditPortEvents")]
    public async Task<IActionResult> EditVoyagePortEvent(VoyagePortEventFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEventDropdowns(vm);
            ViewBag.CreateEdit = "Edit";
            return PartialView("_VoyagePortEventForm", vm);
        }

        var entity = await _db.VoyagePortEvents
            .FirstOrDefaultAsync(e => e.Id == vm.Id && !e.IsDeleted);

        if (entity == null) return NotFound();

        entity.EventTypeId = vm.EventTypeId;
        entity.VoyagePortEventDateTime = vm.VoyagePortEventDateTime;
        entity.VoyagePortEventRemarks = vm.VoyagePortEventRemarks;
        //entity.IsActive = vm.IsActive;
        entity.UpdatedAt = DateTime.Now;
        entity.EditUserId = GetUserId();

        await _db.SaveChangesAsync();
        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Edited Sussefully"
        });

        return Ok();
    }

    [HttpGet]
    [Authorize("BaseOperations.Voyages.EditPortEvents")]
    public async Task<IActionResult> DeleteVoyagePortEvent(Guid id)
    {
        return PartialView("_DeleteVoyagePortEventConfirm", id);
    }

    [HttpPost]
    [Authorize("BaseOperations.Voyages.EditPortEvents")]
    public async Task<IActionResult> DeleteVoyagePortEventConfirmed(Guid id)
    {
        var entity = await _db.VoyagePortEvents
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        if (entity == null) return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.Now;
        entity.EditUserId = GetUserId();

        await _db.SaveChangesAsync();
        HxTrigger(new
        {
            GridChanged = true,
            DetailChanged = true,
            DetailDetailChanged = true,
            toastSuccess = "Deleted Sussefully"
        });
        return Ok();
    }





    private async Task<(bool IsValid, string ErrorMessage)> ValidateEventSequenceAsync(
        Guid voyagePortId,
        int newEventTypeId,
        DateTime newEventDateTime)
    {
        // ۱. لیست تمام رویدادهای ثبت‌شده قبلی برای این پورت
        var existingEvents = await _db.VoyagePortEvents
            .Where(x => x.VoyagePortId == voyagePortId)
            .Select(x => new { x.EventTypeId, x.VoyagePortEventDateTime })
            .ToListAsync();

        var existingEventTypes = existingEvents.ToDictionary(x => x.EventTypeId, x => x.VoyagePortEventDateTime);

        // ۲. بررسی پیش‌نیازهای مستقیم (Prerequisite Checks)
        switch (newEventTypeId)
        {
            case 110: // Berthed نیاز به Arrival دارد
                if (!existingEventTypes.ContainsKey(105))
                    return (false, "ابتدا باید رویداد Arrival To Port (105) ثبت شود.");
                break;

            case 115: // Operations Commenced
            case 116: // Discharge Commenced
            case 117: // Loading Commenced
                if (!existingEventTypes.ContainsKey(110) && !existingEventTypes.ContainsKey(105))
                    return (false, "قبل از شروع عملیات، ثبت رویداد Vessel Berthed (110) یا Arrival الزامی است.");
                break;

            case 120: // Operations Completed
                if (!existingEventTypes.ContainsKey(115))
                    return (false, "رویداد Operations Completed بدون ثبت Operations Commenced (115) امکان‌پذیر نیست.");
                break;

            case 121: // Discharge Completed
                if (!existingEventTypes.ContainsKey(116))
                    return (false, "رویداد Discharge Completed بدون ثبت Discharge Commenced (116) امکان‌پذیر نیست.");
                break;

            case 122: // Loading Completed
                if (!existingEventTypes.ContainsKey(117))
                    return (false, "رویداد Loading Completed بدون ثبت Loading Commenced (117) امکان‌پذیر نیست.");
                break;

            case 125: // Voyage Completed
                      // در صورت وجود عملیات، باید Completed شده باشند
                if (existingEventTypes.ContainsKey(116) && !existingEventTypes.ContainsKey(121))
                    return (false, "تخلیه شروع شده اما هنوز Discharge Completed ثبت نشده است.");
                if (existingEventTypes.ContainsKey(117) && !existingEventTypes.ContainsKey(122))
                    return (false, "بارگیری شروع شده اما هنوز Loading Completed ثبت نشده است.");
                break;
        }

        // ۳. بررسی تقدم و تأخر زمانی با رویدادهای موجود (Chronological Checks)
        // ۳.الف: زوج‌های مستقیم (Start vs Complete)
        var pairedEvents = new (int StartId, int EndId, string StartName, string EndName)[]
        {
        (105, 110, "Arrival To Port", "Vessel Berthed"),
        (115, 120, "Operations Commenced", "Operations Completed"),
        (116, 121, "Discharge Commenced", "Discharge Completed"),
        (117, 122, "Loading Commenced", "Loading Completed")
        };

        foreach (var pair in pairedEvents)
        {
            // اگر رویداد جدید End است و Start از قبل وجود دارد -> نباید تاریخش قبل از Start باشد
            if (newEventTypeId == pair.EndId && existingEventTypes.TryGetValue(pair.StartId, out var startDate))
            {
                if (newEventDateTime < startDate)
                    return (false, $"زمان {pair.EndName} نمی‌تواند قبل از {pair.StartName} ({startDate:yyyy-MM-dd HH:mm}) باشد.");
            }

            // اگر رویداد جدید Start است و End از قبل ثبت شده -> نباید تاریخش بعد از End باشد
            if (newEventTypeId == pair.StartId && existingEventTypes.TryGetValue(pair.EndId, out var endDate))
            {
                if (newEventDateTime > endDate)
                    return (false, $"زمان {pair.StartName} نمی‌تواند بعد از {pair.EndName} ({endDate:yyyy-MM-dd HH:mm}) باشد.");
            }
        }

        // ۳.ب: رویدادهای پس از پهلوگیری (115, 116, 117 نباید قبل از Berthed باشند)
        if (newEventTypeId is 115 or 116 or 117)
        {
            if (existingEventTypes.TryGetValue(110, out var berthedDate) && newEventDateTime < berthedDate)
                return (false, $"زمان شروع عملیات نمی‌تواند قبل از Vessel Berthed ({berthedDate:yyyy-MM-dd HH:mm}) باشد.");
        }

        // ۳.ج: پایان سفر (125 نباید قبل از هیچ رویداد دیگری باشد)
        if (newEventTypeId == 125)
        {
            var maxPreviousDate = existingEvents.Max(x => (DateTime?)x.VoyagePortEventDateTime);
            if (maxPreviousDate.HasValue && newEventDateTime < maxPreviousDate.Value)
                return (false, $"زمان Voyage Completed نمی‌تواند قبل از سایر رویدادهای ثبت‌شده ({maxPreviousDate.Value:yyyy-MM-dd HH:mm}) باشد.");
        }

        return (true, string.Empty);
    }









}
