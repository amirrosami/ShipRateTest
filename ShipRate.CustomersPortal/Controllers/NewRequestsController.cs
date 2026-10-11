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
    public class NewRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ICurrentUserService _currentUser;
        public NewRequestsController(ApplicationDbContext context ,ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUser = currentUserService;
        }


        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
