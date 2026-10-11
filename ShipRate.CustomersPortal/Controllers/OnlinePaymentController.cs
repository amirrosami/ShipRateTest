using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Services;
using ShipRate.Application.ViewModels;
using ShipRate.Common.ExternalServices.PaymentGateways;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;

namespace ShipRate.CustomersPortal.Controllers
{
    [Authorize]
    public class OnlinePaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ICurrentUserService _currentUser;
        private readonly PaymentService _paymentService;
        public OnlinePaymentController(ApplicationDbContext context ,ICurrentUserService currentUserService , PaymentService paymentService)
        {
            this._paymentService = paymentService;
            _context = context;
            _currentUser = currentUserService;
        }


        public async Task<IActionResult> SamanGatewayCallback(SamanPaymentCallBackResponseDto dto)
        {
            var vm = new GatewayCallbackResultVM();
            var result = await _paymentService.HandleCallback(GatewayType.SAMAN, dto);
            if (!result.Succeeded) 
            {
                vm.IsSuccess = false;
            }
            vm.IsSuccess = true;
            vm.Amount = result.Data.Amount;
            return View("Callback",vm);
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _paymentService.GetPagedOnlinePayments(new OnlinePaymentFilterVm { });
            return View(vm.Data);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] OnlinePaymentFilterVm filter)
        {
            var vm = await _paymentService.GetPagedOnlinePayments(filter);
            return PartialView("_List", vm.Data);
        }
    }
}
