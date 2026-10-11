using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;

namespace ShipRate.CustomersPortal.Controllers
{
    [Authorize]
    public class FakePaymentGWController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ICurrentUserService _currentUser;
        public FakePaymentGWController(ApplicationDbContext context ,ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUser = currentUserService;
        }


        public async Task<IActionResult> Index(FakePaymentViewModel model)
        {
            return View(model);
        }


        public async Task<IActionResult> FinalizePayment(FakePaymentViewModel model)
        {
            var redirectUrl = $"https://localhost:5001/OnlinePayment/SamanGatewayCallback?token={model.Token}&ResNum={model.ResNum}&State=OK&Amount={model.Amount}";
            return Redirect(redirectUrl);
        }
    }
}
