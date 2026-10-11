using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TIPController : Controller
    {
        private readonly ITipService _tipService;
        private readonly IJobLookupService _lookupService;
        private readonly ICurrentUserService _currentUserService;


        public TIPController(ITipService tipService, IJobLookupService lookupService, ICurrentUserService currentUserService)
        {
            _tipService = tipService;
            _lookupService = lookupService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize("ImportSpecific.TIPs.View")]
        public async Task<IActionResult> Index(TipFilterDto filter)
        {
            var tips = await _tipService.GetPagedTipsAsync(filter);
            return View(tips);
        }

        [HttpGet]
        [Authorize("ImportSpecific.TIPs.View")]
        public async Task<IActionResult> ListTips(TipFilterDto filter)
        {
            var tips = await _tipService.GetPagedTipsAsync(filter);
            return PartialView("_TipGrid", tips);
        }

        [Authorize("ImportSpecific.TIPs.View")]
        [HttpGet]
        public async Task<IActionResult> ListTipUsages(Guid tipId)
        {
            var usages = await _tipService.GetTipUsagesByTipAsync(tipId);
            return PartialView("_TipUsageGrid", usages);
        }

        [HttpGet]
        [Authorize("ImportSpecific.TIPs.View")]
        public async Task<IActionResult> ListTipUsageContainers(Guid tipUsageId)
        {
            var containers = await _tipService.GetContainersByTipUsageAsync(tipUsageId);
            return PartialView("_TipUsageContainersGrid", containers);
        }

        [HttpGet]
        [Authorize("ImportSpecific.TIPs.Edit")]
        public async Task<IActionResult> Create()
        {
            var dto = new TipEditDto
            {
                TIPDate = DateTime.Today,
                TIPExpiryDate = DateTime.Today.AddDays(30),
                IsActive = true
            };

            await FillTipDropdownsAsync(dto);
            return PartialView("_TipFormModal", dto);
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.TIPs.Edit")]
        public async Task<IActionResult> Create(TipEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                await FillTipDropdownsAsync(dto);
                await FillVoyageDisplayTextAsync(dto);
                Response.StatusCode = 400;
                return PartialView("_TipFormModal", dto);
            }

            var currentUserId = _currentUserService.UserId;
            await _tipService.CreateTipAsync(dto, currentUserId);

            var filter = new TipFilterDto { PageNumber = 1 };
            var tips = await _tipService.GetPagedTipsAsync(filter);
            Response.Headers["HX-Trigger"] = "tipSaved";
            //return PartialView("_TipGrid", tips);
            return PartialView("_TipSaveSuccess", tips);
        }
        // Edit GET
        [HttpGet]
        [Authorize("ImportSpecific.TIPs.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _tipService.GetTipForEditAsync(id);
            if (dto == null)
                return NotFound();

            await FillTipDropdownsAsync(dto);
            return PartialView("_TipFormModal", dto);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.TIPs.Edit")]
        public async Task<IActionResult> Edit(TipEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                await FillTipDropdownsAsync(dto);
                await FillVoyageDisplayTextAsync(dto);
                Response.StatusCode = 400;
                return PartialView("_TipFormModal", dto);
            }

            var currentUserId = _currentUserService.UserId;
            await _tipService.UpdateTipAsync(dto, currentUserId);

            var result = await _tipService.GetPagedTipsAsync(new TipFilterDto { PageNumber = 1 });
            return PartialView("_TipSaveSuccess", result);
        }        // Delete GET (confirm modal)
        [HttpGet]
        [Authorize("ImportSpecific.TIPs.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _tipService.GetTipForEditAsync(id);
            if (dto == null)
                return NotFound();

            return PartialView("_TipDeleteConfirmModal", dto);
        }

        // Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.TIPs.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var currentUserId = _currentUserService.UserId;

            await _tipService.DeleteTipAsync(id, currentUserId);

            var filter = new TipFilterDto
            {
                PageNumber = 1
            };

            var tips = await _tipService.GetPagedTipsAsync(filter);

            Response.Headers["HX-Trigger"] = "closeTipFormModal";

            return PartialView("_TipGrid", tips);
        }

        // Details (فقط نمایش)
        [HttpGet]
        [Authorize("ImportSpecific.TIPs.View")]
        public async Task<IActionResult> Details(Guid id)
        {
            var dto = await _tipService.GetTipForEditAsync(id);
            if (dto == null)
                return NotFound();

            return PartialView("_TipDetailsModal", dto);
        }

        // Lookup VoyagePort Modal
        //[HttpGet]
        //public async Task<IActionResult> LookupVoyagePort(VoyagePortLookupFilterDto filter)
        //{
        //    var result = await _tipService.LookupVoyagePortsAsync(filter);
        //    return PartialView("_VoyagePortLookupModal", result);
        //}

        [HttpGet]
        public async Task<IActionResult> LookupVoyagePort(VoyagePortLookupFilterDto filter, string targetPrefix = "tip-voyageport")
        {
            var result = await _tipService.LookupVoyagePortsAsync(filter);
            ViewBag.TargetPrefix = targetPrefix;
            return PartialView("_VoyagePortLookupModal", result);
        }
        [HttpGet]
        public async Task<IActionResult> LookupVoyagePortResults(VoyagePortLookupFilterDto filter, string targetPrefix = "tip-voyageport")
        {
            var result = await _tipService.LookupVoyagePortsAsync(filter);
            ViewBag.TargetPrefix = targetPrefix;
            return PartialView("_VoyagePortLookupResults", result);
        }

        [HttpGet]
        public async Task<IActionResult> SelectVoyagePort(Guid voyagePortId, string targetPrefix = "tip-voyageport")
        {
            var voyagePort = await _tipService.GetVoyagePortByIdAsync(voyagePortId);
            if (voyagePort == null)
            {
                return NotFound();
            }

            var model = new VoyagePortSelectorDto
            {
                VoyagePortId = voyagePort.VoyagePortId,
                VoyageDisplayText = $"{voyagePort.VoyageNo} / {voyagePort.VesselName} / {voyagePort.PortName}",
                TargetPrefix = targetPrefix
            };

            Response.Headers["HX-Trigger"] = "closeLookupModal";

            return PartialView("_VoyagePortSelector", model);
        }

        private async Task FillVoyageDisplayTextAsync(TipEditDto dto)
        {
            if (dto.VoyagePortId != Guid.Empty)
            {
                var vp = await _tipService.GetVoyagePortByIdAsync(dto.VoyagePortId);
                if (vp != null)
                {
                    dto.VoyageDisplayText = vp.DisplayText;
                }
            }
        }

        private async Task FillTipDropdownsAsync(TipEditDto dto)
        {
            dto.Companies = await _lookupService.GetCompaniesAsync();
            dto.ShippingLines = await _lookupService.GetCarriersAsync(null);
            dto.CustomsOffices = await _lookupService.GetCustomOfficesAsync();
        }

        [HttpGet]
        public async Task<IActionResult> PrintTipLetter(Guid blid)
        {
            var dto = await _tipService.GetTipUsageByBlIdAsync(blid);
            try
            {
                byte[] pdfBytes = await _tipService.GenerateTipLetterPdfAsync(dto);
                string fileName = $"TIP_Letter_{dto.TIPNo}_{DateTime.Now:yyyyMMdd}.pdf";
                // برای باز شدن مستقیم در مرورگر (Inline View)
                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                // مدیریت خطا متناسب با الگوهای پروژه شما
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }



    }
}
