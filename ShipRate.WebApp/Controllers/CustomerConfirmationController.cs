using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.CustomersVms;
namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CustomerConfirmationController : Controller
    {
        private readonly ApplicationDbContext _db;
        public readonly ICurrentUserService _currentUserService;

        public CustomerConfirmationController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [Authorize("CRM.CustomerConfirmations.View")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("CRM.CustomerConfirmations.View")]
        public async Task<IActionResult> List(string? searchName, string? searchStatus, int page = 1)
        {
            int pageSize = 10;

            var query = _db.CustomerConfirmations
                .Include(c => c.Person)
                .Where(c => !c.IsDeleted);

            // Filter by person name
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(c =>
                    (c.Person.PersonName).Contains(searchName));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(searchStatus) && searchStatus != "All")
            {
                query = query.Where(c => c.Status == searchStatus);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerConfirmationListItem
                {
                    Id = c.Id,
                    PersonId = c.PersonId,
                    PersonName = c.Person.PersonName,
                    Remarks = c.Remarks,
                    ConfirmRemarks = c.COnfirmRemarks,
                    Status = c.Status,
                    ConfirmUserName = c.ConfirmUser != null
                        ? c.ConfirmUser.UserName
                        : "—",
                    CreatedAt = c.CreatedAt,
                    ConfirmDate = c.ConfirmDate
                })
                .ToListAsync();

            var vm = new CustomerConfirmationListViewModel
            {
                Items = items,
                SearchName = searchName,
                SearchStatus = searchStatus,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return PartialView("_List", vm);
        }

        [HttpGet]
        [Authorize("CRM.CustomerConfirmations.View")]
        public async Task<IActionResult> SearchPerson(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(new object[] { });

            var results = await _db.Persons
                .Where(p => !p.IsDeleted &&
                    (p.PersonName).Contains(term))
                .Take(15)
                .Select(p => new PersonSearchResult
                {
                    Id = p.Id,
                    FullName = p.PersonName
                })
                .ToListAsync();

            return Json(results);
        }

        [HttpGet]
        [Authorize("CRM.CustomerConfirmations.Edit")]
        public IActionResult Create()
        {
            var vm = new CustomerConfirmationCreateViewModel();
            return PartialView("_CreateForm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.CustomerConfirmations.View")]
        public async Task<IActionResult> Create(CustomerConfirmationCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateForm", vm);

            // Check: already has a pending or confirmed record?
            var existing = await _db.CustomerConfirmations
                .AnyAsync(c => c.PersonId == vm.PersonId && !c.IsDeleted &&
                    (c.Status == "In Progress" || c.Status == "Confirmed"));

            if (existing)
            {
                ModelState.AddModelError("PersonId",
                    "This person already has a pending or confirmed request.");
                return PartialView("_CreateForm", vm);
            }

            // Check: already a Customer?
            var isCustomer = await _db.Customers.AnyAsync(c => c.Id == vm.PersonId);
            if (isCustomer)
            {
                ModelState.AddModelError("PersonId",
                    "This person is already a customer.");
                return PartialView("_CreateForm", vm);
            }

            // TODO: Replace with actual current user id
            var currentUserId = _currentUserService.UserId;

            var entity = new CustomerConfirmation
            {
                Id = Guid.CreateVersion7(),
                PersonId = vm.PersonId!.Value,
                Remarks = vm.Remarks,
                Status = "In Progress",
                ConfirmUserId = currentUserId,
                COnfirmRemarks = string.Empty,
                ConfirmDate = DateTime.MinValue,
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            _db.CustomerConfirmations.Add(entity);
            await _db.SaveChangesAsync();

            // Return updated list
            return await List(null, null, 1);
        }

        [HttpGet]
        [Authorize("CRM.CustomerConfirmations.View")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _db.CustomerConfirmations
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (entity == null) return NotFound();

            // Only editable when In Progress
            if (entity.Status != "In Progress")
            {
                return Content("<div class='alert alert-warning'>Only 'In Progress' records can be edited.</div>");
            }

            var vm = new CustomerConfirmationEditViewModel
            {
                Id = entity.Id,
                PersonId = entity.PersonId,
                PersonName = entity.Person.PersonName,
                Remarks = entity.Remarks,
                Status = entity.Status
            };

            return PartialView("_EditForm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.CustomerConfirmations.View")]
        public async Task<IActionResult> Edit(CustomerConfirmationEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditForm", vm);

            var entity = await _db.CustomerConfirmations
                .FirstOrDefaultAsync(c => c.Id == vm.Id && !c.IsDeleted);

            if (entity == null) return NotFound();

            if (entity.Status != "In Progress")
            {
                return Content("<div class='alert alert-warning'>Only 'In Progress' records can be edited.</div>");
            }

            var currentUserId = _currentUserService.UserId;

            entity.Remarks = vm.Remarks;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();

            return await List(null, null, 1);
        }

        [HttpGet]
        [Authorize("CRM.CustomerConfirmations.ConfirmOrReject")]
        public async Task<IActionResult> ConfirmReject(Guid id)
        {
            var entity = await _db.CustomerConfirmations
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (entity == null) return NotFound();

            if (entity.Status != "In Progress")
            {
                return Content("<div class='alert alert-warning'>This request has already been processed.</div>");
            }

            var vm = new CustomerConfirmationActionViewModel
            {
                Id = entity.Id,
                PersonName = entity.Person.PersonName,
                Remarks = entity.Remarks,
                CurrentStatus = entity.Status,
                ConfirmRemarks = string.Empty
            };

            return PartialView("_ConfirmRejectForm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.CustomerConfirmations.ConfirmOrReject")]
        public async Task<IActionResult> ConfirmReject(CustomerConfirmationActionViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_ConfirmRejectForm", vm);

            var entity = await _db.CustomerConfirmations
                .FirstOrDefaultAsync(c => c.Id == vm.Id && !c.IsDeleted);

            if (entity == null) return NotFound();

            if (entity.Status != "In Progress")
            {
                return Content("<div class='alert alert-warning'>This request has already been processed.</div>");
            }

            var currentUserId = _currentUserService.UserId;

            entity.COnfirmRemarks = vm.ConfirmRemarks ?? string.Empty;
            entity.ConfirmDate = DateTime.Now;
            entity.ConfirmUserId = currentUserId;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            if (vm.Action == "Confirmed")
            {
                entity.Status = "Confirmed";

                // ★ Insert into Customers table
                var customerExists = await _db.Customers.AnyAsync(c => c.Id == entity.PersonId);
                if (!customerExists)
                {
                    var customer = new Customer
                    {
                        Id = entity.PersonId,
                        CreateUserId = currentUserId
                    };
                    _db.Customers.Add(customer);
                }
            }
            else if (vm.Action == "Rejected")
            {
                entity.Status = "Rejected";
            }

            await _db.SaveChangesAsync();

            return await List(null, null, 1);
        }
        [HttpGet]
        [Authorize("CRM.CustomerConfirmations.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _db.CustomerConfirmations
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (entity == null) return NotFound();

            ViewBag.PersonName = entity.Person.PersonName;
            ViewBag.EntityId = entity.Id;

            return PartialView("_DeleteConfirm");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.CustomerConfirmations.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await _db.CustomerConfirmations
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return await List(null, null, 1);
        }
    }
}
