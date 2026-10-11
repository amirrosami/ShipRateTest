using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BillOfLadings;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.JobsViewModels;
using ShipRate.Application.Services.BillOfLadings;
using ShipRate.Application.Services.Finance;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;

namespace ShipRate.CustomersPortal.Controllers
{
    [Authorize]
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ICurrentUserService _currentUser;
        private IJobService _jobService;
        private BLService _blService;
        private readonly IShipmentService _shipmentService;
        private readonly InvoiceAppService _invoiceAppService;
        public TrackingController(ApplicationDbContext context ,ICurrentUserService currentUserService, IJobService jobService , BLService bLService ,IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
            _blService = bLService;
            _jobService = jobService;
            _context = context;
            _currentUser = currentUserService;
        }


        public async Task<IActionResult> Index(JobSearchVm vm)
        {
            var currentPersonId = await _currentUser.GetPersonIdAsync();
            vm.CustomerId = currentPersonId;
            var data = await _jobService.SearchAsync(vm);
            return View(data);
        }

        public async Task<IActionResult> List(JobSearchVm vm)
        {
            var currentPersonId = await _currentUser.GetPersonIdAsync();
            vm.CustomerId = currentPersonId;
            var data = await _jobService.SearchAsync(vm);
            return PartialView("_List",data);
        }

        public async Task<IActionResult> Detail(Guid id) 
        {
            var detail = await _jobService.GetDetailsAsync(id);
            var containers = await _jobService.GetContainersListAsync(id);
            var blInfo = await _blService.GetListAsync(new Application.DTOs.BillOfLadings.BLContextDto { JobId = id });
            var legs = await _shipmentService.GetShipmentLegListAsync(new ShipmentLegFilterVm { ShipmentId = await _jobService.GetFirstShipmentIdAsync(id) });


            var trackingDetailVm = new JobDetailTrackingVM
            {
                JobDetails = detail,
                BlInfos = blInfo,
                Containers = containers,
                Legs = legs.Items.ToList()
            };
            return PartialView("_ShipmentContent",trackingDetailVm);
        }
    }
}
