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
    public class ExpenseCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUser;
        private const int PageSize = 10;
        public ExpenseCategoryController(ApplicationDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        [Authorize("PettyCash.ExpenseCategories.View")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize("PettyCash.ExpenseCategories.View")]
        public async Task<IActionResult> Grid(string? search, int page = 1)
        {
            var query = _db.ExpenseCategories
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .Where(x => !x.IsDeleted);

            // ---- Search ----
            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(x =>
                    x.Title.ToLower().Contains(term) ||
                    x.AccountCode.ToLower().Contains(term) ||
                    x.Company.Person.PersonName.ToLower().Contains(term));
            }

            // ---- Count & Pagination ----
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var items = await query
                .OrderBy(x => x.Title)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new ExpenseCategoryVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    MaxAllowedAmount = x.MaxAllowedAmount,
                    AccountCode = x.AccountCode,
                    CompanyName = x.Company.Person.PersonName,
                    //IsActive = x.IsActive
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search ?? "";

            return PartialView("_Grid", items);
        }

        [Authorize("PettyCash.ExpenseCategories.Edit")]
        public async Task<IActionResult> Create()
        {
            var vm = new ExpenseCategoryFormVM
            {
                IsActive = true,
                MaxAllowedAmount = 0
            };

            await PopulateViewBags();
            return PartialView("_Form", vm);
        }

        [Authorize("PettyCash.ExpenseCategories.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _db.ExpenseCategories
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (entity == null)
                return Content("<div class='alert alert-danger'>Record not found.</div>");

            var vm = new ExpenseCategoryFormVM
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Title = entity.Title,
                MaxAllowedAmount = entity.MaxAllowedAmount,
                AccountCode = entity.AccountCode,
                //IsActive = entity.IsActive
            };

            await PopulateViewBags();
            return PartialView("_Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.ExpenseCategories.Edit")]
        public async Task<IActionResult> Save(ExpenseCategoryFormVM vm)
        {
            // ---- Server-side validation ----
            if (string.IsNullOrWhiteSpace(vm.Title))
                ModelState.AddModelError("Title", "Title is required.");

            if (string.IsNullOrWhiteSpace(vm.AccountCode))
                ModelState.AddModelError("AccountCode", "Account Code is required.");

            if (vm.CompanyId <= 0)
                ModelState.AddModelError("CompanyId", "Please select a company.");

            if (vm.MaxAllowedAmount < 0)
                ModelState.AddModelError("MaxAllowedAmount", "Amount cannot be negative.");

            // ---- Duplicate check ----
            bool duplicateTitle = await _db.ExpenseCategories
                .AnyAsync(x => !x.IsDeleted
                    && x.Title.ToLower() == vm.Title.Trim().ToLower()
                    && x.CompanyId == vm.CompanyId
                    && x.Id != (vm.Id ?? Guid.Empty));

            if (duplicateTitle)
                ModelState.AddModelError("Title", "This title already exists for the selected company.");

            if (!ModelState.IsValid)
            {
                await PopulateViewBags();
                return PartialView("_Form", vm);
            }

            // ---- Create or Update ----
            if (vm.Id == null || vm.Id == Guid.Empty)
            {
                // CREATE
                var entity = new ExpenseCategory
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = vm.CompanyId,
                    Title = vm.Title.Trim(),
                    MaxAllowedAmount = vm.MaxAllowedAmount,
                    AccountCode = vm.AccountCode.Trim(),
                    //IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUser.UserId
                };

                _db.ExpenseCategories.Add(entity);
            }
            else
            {
                // UPDATE
                var entity = await _db.ExpenseCategories
                    .FirstOrDefaultAsync(x => x.Id == vm.Id && !x.IsDeleted);

                if (entity == null)
                    return Content("<div class='alert alert-danger'>Record not found.</div>");

                entity.CompanyId = vm.CompanyId;
                entity.Title = vm.Title.Trim();
                entity.MaxAllowedAmount = vm.MaxAllowedAmount;
                entity.AccountCode = vm.AccountCode.Trim();
                //entity.IsActive = vm.IsActive;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUser.UserId;
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "closeModalAndRefresh";
            return Content("OK");
        }

        [Authorize("PettyCash.ExpenseCategories.Edit")]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _db.ExpenseCategories
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return NotFound();

            // Soft Delete
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            // Return refreshed grid
            return await Grid(null, 1);
        }
        private async Task PopulateViewBags()
        {
            var companies = await _db.Companies
                .Include(x => x.Person)
                .OrderBy(c => c.Person.PersonName)
                .Select(c => new { c.Id, c.Person.PersonName })
                .ToListAsync();

            ViewBag.Companies = new SelectList(companies, "Id", "PersonName");
        }
    }
}

