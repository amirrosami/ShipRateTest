using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Contract;
using ShipRate.Infrastructure.Data;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class FACContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;
        public ICurrentUserService _currentUserService { get; set; }

        public FACContractsController(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        [Authorize("BaseOperations.FACContracts.View")]
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var query = _context.FACContracts
                .Include(x => x.Carrier)
                .ThenInclude(x => x.Person)
                .Include(x => x.Customer)
                .ThenInclude(x => x.Person)
                .Include(x => x.Currency)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.FacContractTitle.Contains(search));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            ViewBag.Search = search;

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_TablePartial", items);

            return View(items);
        }

        [Authorize("BaseOperations.FACContracts.Edit")]
        public IActionResult Create()
        {
            return PartialView("_Form", new FACContract());
        }

        [HttpPost]
        [Authorize("BaseOperations.FACContracts.View")]
        public async Task<IActionResult> Create(FACContract model)
        {
            var filledCount = (model.FACPercent.HasValue ? 1 : 0) +
                             (model.FACPerCBM.HasValue ? 1 : 0) +
                             (model.FACPerTEU.HasValue ? 1 : 0);

            if (filledCount != 1)
                ModelState.AddModelError("", "Only one of FACPercent, FACPerCBM or FACPerTEU must be filled.");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            model.Id = Guid.CreateVersion7();
            model.CreatedAt = DateTime.Now;
            model.CreateUserId = _currentUserService.UserId;
            model.IsDeleted = false;
            model.CompanyId = 1;

            _context.Add(model);
            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "closeModal,refreshTable";
            return Ok();
        }

        [Authorize("BaseOperations.FACContracts.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _context.FACContracts
                .Include(x => x.Carrier)
                .ThenInclude(x => x.Person)
                .Include(x => x.Customer)
                .ThenInclude(x => x.Person)
                .Include(x => x.Currency)
                .FirstOrDefaultAsync(x => x.Id == id);

            return PartialView("_Form", item);
        }

        [HttpPost]
        [Authorize("BaseOperations.FACContracts.Edit")]
        public async Task<IActionResult> Edit(FACContract model)
        {
            var filledCount = (model.FACPercent.HasValue ? 1 : 0) +
                             (model.FACPerCBM.HasValue ? 1 : 0) +
                             (model.FACPerTEU.HasValue ? 1 : 0);

            if (filledCount != 1)
                ModelState.AddModelError("", "Only one of FACPercent, FACPerCBM or FACPerTEU must be filled.");

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            model.UpdatedAt = DateTime.Now;
            _context.Update(model);
            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "closeModal,refreshTable";
            return Ok();
        }

        [HttpPost]
        [Authorize("BaseOperations.FACContracts.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.FACContracts.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Ok();
        }

        public async Task<IActionResult> SearchCarrier(string term)
        {
            var data = await _context.Carriers
                .Include(x => x.Person)
                .Where(x => !x.IsDeleted && x.Person.PersonName.Contains(term))
                .OrderBy(x => x.Person.PersonName)
                .Take(10)
                .ToListAsync();

            return PartialView("_CarrierResults", data);
        }

        public async Task<IActionResult> SearchForwarder(string term)
        {
            var data = await _context.Customers
                .Include(x => x.Person)
                .Where(x => !x.IsDeleted && x.Person.PersonName.Contains(term))
                .OrderBy(x => x.Person.PersonName)
                .Take(10)
                .ToListAsync();

            return PartialView("_ForwarderResults", data);
        }
        public async Task<IActionResult> SearchCurrency(string term)
        {
            var data = await _context.Currencies
                .Where(x => x.CurrencyCode.Contains(term) || x.CurrencyName.Contains(term))
                .OrderBy(x => x.CurrencyCode)
                .Take(10)
                .ToListAsync();

            return PartialView("_CurrencyResults", data);
        }
        public async Task<IActionResult> SelectCarrier(Guid id)
        {
            var carrier = await _context.Carriers.Include(x => x.Person).FirstOrDefaultAsync(x => x.Id == id);
            return PartialView("_CarrierSelected", carrier);
        }
        public async Task<IActionResult> SelectForwarder(Guid id)
        {
            var item = await _context.Customers.Include(x => x.Person).FirstOrDefaultAsync(x => x.Id == id);
            return PartialView("_ForwarderSelected", item);
        }
        public async Task<IActionResult> SelectCurrency(int id)
        {
            var item = await _context.Currencies.FindAsync(id);
            return PartialView("_CurrencySelected", item);
        }
    }
}
