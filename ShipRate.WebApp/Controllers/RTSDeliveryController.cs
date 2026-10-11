using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.ViewModels;
using ShipRate.Common.Utilities;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class RtsDeliveryController : Controller
    {
        private readonly IContainerActionService _containerActionService;
        private readonly IContainerService _containerService;
        private readonly IBaseDataService _baseDataService;
        public RtsDeliveryController(IContainerActionService containerActionService, IBaseDataService baseDataService, IContainerService containerService)
        {
            _containerActionService = containerActionService;
            _baseDataService = baseDataService;
            _containerService = containerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchRts(string rtsNo)
        {
            rtsNo = rtsNo?.Trim();

            if (string.IsNullOrWhiteSpace(rtsNo))
            {
                return PartialView("_RtsSearchMessage", "Please enter an RTS number.");
            }

            var summary = await _containerActionService.GetRtsDeliverySummaryAsync(rtsNo);

            if (summary == null)
            {
                return PartialView("_RtsSearchMessage", $"RTS '{rtsNo}' was not found.");
            }
            return PartialView("_RtsSummaryDetails", summary);
        }
        [HttpGet]
        public async Task<IActionResult> SearchRtsByInstructionId(Guid rtsInstructionId)
        {
            var summary = await _containerActionService.GetRtsDeliverySummaryByInstructionIdAsync(rtsInstructionId);
            return PartialView("~/Views/Booking/RTS/_RtsSummaryDetails.cshtml", summary);
        }
        [HttpGet]
        public async Task<IActionResult> LoadIssueModal(Guid rtsId, int sizeTypeId, Guid bookingId)
        {
            var sizetypes = await _baseDataService.GetContainerSizeTypesAsync();
            var sizeTypeName = sizetypes
                .Where(x => x.Id == sizeTypeId)
                .FirstOrDefault()
                .ShortName;

            if (sizeTypeName == null)
            {
                return Content("""
                    <div class="modal-header">
                        <h5 class="modal-title">Issue Empty Container</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-danger mb-0">Container size/type was not found.</div>
                    </div>
                    """, "text/html");
            }

            var command = new IssueContainerCommand
            {
                RTSInstructionId = rtsId,
                ContainerSizeTypeId = sizeTypeId,
                BookingId = bookingId,
                ActionDateTime = DateTime.Now
            };

            ViewBag.SizeTypeName = sizeTypeName;

            return PartialView("_IssueContainerModal", command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueContainer(IssueContainerCommand command)
        {
            if (string.IsNullOrEmpty(command.ContainerNumber) || !Iso6346.ValidateContainerNumber(command.ContainerNumber))
            {
                ModelState.AddModelError(
                    nameof(command.ContainerNumber),
                    $"Container No '{command.ContainerNumber}' is not a valid ISO 6346 number."
                );
            }

            var sizeTypeName = "555555";
            //var sizeTypeName = await _context.ContainerSizeTypes
            //    .Where(x => x.Id == command.ContainerSizeTypeId)
            //    .Select(x => x.Name)
            //    .FirstOrDefaultAsync() ?? "N/A";
            ViewBag.SizeTypeName = sizeTypeName;

            if (!ModelState.IsValid)
            {
                return PartialView("_IssueContainerModal", command);
            }



            // 1. اگر کانتینر جدید بود، ابتدا در جدول کانتینرها ثبت شود
            if (command.IsNewContainer)
            {
                ContainerFormVM vm = new ContainerFormVM
                {
                    Id = Guid.CreateVersion7(),
                    ContainerNumber = command.ContainerNumber,
                    ContainerSizeTypeId = command.ContainerSizeTypeId,
                    TareWeight = (int)command.NewContainerTareWeight,
                    CntOwnTypeId = 1,
                    IsActive = true,
                    MaxPayLoad = (int)command.NewContainerMaxPayload,
                    OwnLeaseTypeId = 1,
                    ResponsibilityByUs = false,
                };
                var newId = await _containerService.CreateContainer(vm);
            }

            var companyId = GetCurrentCompanyId();
            var result = await _containerActionService.IssueContainerAsync(command, companyId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return PartialView("_IssueContainerModal", command);
            }
            Response.Headers["HX-Trigger"] = "{\"rtsUpdated\":true,\"rtsIssueSaved\":{\"message\":\"Container issued successfully.\"}}";

            return Content(string.Empty);
        }

        private int GetCurrentCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            return int.TryParse(companyIdClaim, out var companyId) ? companyId : 1;
        }
    }
}
