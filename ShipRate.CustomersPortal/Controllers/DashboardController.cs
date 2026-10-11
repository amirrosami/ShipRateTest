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
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ICurrentUserService _currentUser;
        public DashboardController(ApplicationDbContext context ,ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUser = currentUserService;
        }



        public async Task<IActionResult> Index()
        {
            var currentCustomer = await _currentUser.GetPersonIdAsync();
            var query = _context.RateRequests.Where(x => x.CustomerId != null && x.CustomerId == currentCustomer)
                .OrderByDescending(x=>x.CreatedAt).Take(3).AsQueryable();


            var notifs = await _context.Notifications
                .Take(6)
                .ToListAsync();

            var factors = await _context.Invoices.Where(x => x.PartyId == currentCustomer).Include(x=>x.InvoiceStatus).Include(x=>x.Company).ThenInclude(x=>x.Person).Take(5).ToListAsync();
            var items = await query.Select(r => new RateRequestItemViewModel
            {
                Id = r.Id,
                CustomerName = r.CustomerName,
                ContactName = r.ContactName,
                MobileNo = r.MobileNo,
                EmailAddress = r.EmailAddress,
                OriginName = r.Origin != null ? r.Origin.LocationName : null,
                DestinationName = r.Destination != null ? r.Destination.LocationName : null,
                CargoReadinessDate = r.CargoReadinessDate.Value,
                StatusName = r.RateRequestStatus != null ? r.RateRequestStatus.RateRequestStatusName : null,
                CreatedAt = r.CreatedAt,
                RequestNo = r.RequestNumber,
                RequestDate = r.RequestDate,
                CreatedByUserName = r.CreatedByUser.FullName,
                LastEditedByUserName = r.LastEditedByUser.FullName,
            })
                .ToListAsync();
            return View(new DashboardViewModel { RateRequests = items , Invoices = factors ,Notifications = notifs});
        }
    }
}
