using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.PettyCashControl;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.FinanceVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CostCenterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private const int PageSize = 10;

        public CostCenterController(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [Authorize("PettyCash.CostCenters.View")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("PettyCash.CostCenters.View")]
        public async Task<IActionResult> Grid(string? search, int page = 1)
        {
            var query = _context.CostCenters
                .Include(c => c.Company)
                .ThenInclude(c => c.Person)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // ---------- Search ----------
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(term) ||
                    c.Code.ToLower().Contains(term) ||
                    c.Company.Person.PersonName.ToLower().Contains(term));
            }

            // ---------- Count & Pagination ----------
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var items = await query
                .OrderBy(c => c.Code)
                .ThenBy(c => c.Title)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new CostCenterVM
                {
                    Id = c.Id,
                    Code = c.Code,
                    Title = c.Title,
                    CompanyName = c.Company.Person.PersonName
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;

            return PartialView("_Grid", items);
        }

        [Authorize("PettyCash.CostCenters.Edit")]
        public IActionResult Create()
        {
            var vm = new CostCenterFormVM { IsActive = true };
            PopulateViewBags();
            return PartialView("_Form", vm);
        }

        [Authorize("PettyCash.CostCenters.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _context.CostCenters
                .Where(c => !c.IsDeleted && c.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();

            var vm = new CostCenterFormVM
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Code = entity.Code,
                Title = entity.Title
            };

            PopulateViewBags();
            return PartialView("_Form", vm);
        }

        [Authorize("PettyCash.CostCenters.Edit")]
        [HttpPost]
        public async Task<IActionResult> Save(CostCenterFormVM vm)
        {
            // ========== Server-side Validation ==========
            if (string.IsNullOrWhiteSpace(vm.Title))
                ModelState.AddModelError("Title", "Title is required.");
            else if (vm.Title.Trim().Length > 50)
                ModelState.AddModelError("Title", "Title cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(vm.Code))
                ModelState.AddModelError("Code", "Code is required.");
            else if (vm.Code.Trim().Length > 20)
                ModelState.AddModelError("Code", "Code cannot exceed 20 characters.");

            if (vm.CompanyId <= 0)
                ModelState.AddModelError("CompanyId", "Please select a company.");

            // ---- Duplicate Title check (same company, not deleted) ----
            if (!string.IsNullOrWhiteSpace(vm.Title))
            {
                var duplicateTitle = await _context.CostCenters.AnyAsync(c =>
                    !c.IsDeleted &&
                    c.CompanyId == vm.CompanyId &&
                    c.Title.ToLower() == vm.Title.Trim().ToLower() &&
                    c.Id != (vm.Id ?? Guid.Empty));

                if (duplicateTitle)
                    ModelState.AddModelError("Title", "A cost center with this title already exists for the selected company.");
            }

            // ---- Duplicate Code check (same company, not deleted) ----
            if (!string.IsNullOrWhiteSpace(vm.Code))
            {
                var duplicateCode = await _context.CostCenters.AnyAsync(c =>
                    !c.IsDeleted &&
                    c.CompanyId == vm.CompanyId &&
                    c.Code.ToLower() == vm.Code.Trim().ToLower() &&
                    c.Id != (vm.Id ?? Guid.Empty));

                if (duplicateCode)
                    ModelState.AddModelError("Code", "A cost center with this code already exists for the selected company.");
            }

            // ---- If validation failed, return form with errors ----
            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return PartialView("_Form", vm);
            }

            // ========== Save ==========
            if (vm.Id == null || vm.Id == Guid.Empty)
            {
                // ---- CREATE ----
                var entity = new CostCenter
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = vm.CompanyId,
                    Code = vm.Code.Trim(),
                    Title = vm.Title.Trim(),
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUser.UserId
                };
                _context.CostCenters.Add(entity);
            }
            else
            {
                // ---- UPDATE ----
                var entity = await _context.CostCenters
                    .FirstOrDefaultAsync(c => c.Id == vm.Id && !c.IsDeleted);

                if (entity == null)
                    return NotFound();

                entity.CompanyId = vm.CompanyId;
                entity.Code = vm.Code.Trim();
                entity.Title = vm.Title.Trim();
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUser.UserId;
            }

            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "closeModalAndRefresh";
            return Content("");
        }

        [Authorize("PettyCash.CostCenters.Edit")]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _context.CostCenters
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (entity == null)
                return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;

            await _context.SaveChangesAsync();

            return await Grid(null, 1);
        }

        private void PopulateViewBags()
        {
            ViewBag.Companies = _context.Companies
                .Include(x => x.Person)
                .OrderBy(c => c.Person.PersonName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Person.PersonName
                })
                .ToList();
        }
    }
}
