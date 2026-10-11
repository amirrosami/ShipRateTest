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
    public class CustomerGroupingController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public CustomerGroupingController(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _db = context;
            _currentUser = currentUser;
        }
        [Authorize("CRM.CustomerGroupings.View")]
        public async Task<IActionResult> Index()
        {
            var bases = await _db.CustomerGroupingBases
                .Where(x => !x.IsDeleted)
                .Select(x => new CustomerGroupingBaseVM
                {
                    Id = x.Id,
                    CustomerGroupingBaseName = x.CustomerGroupingBaseName,
                    //IsActive = x.IsActive,
                    GroupCount = x.CustomerGroups.Count(g => !g.IsDeleted)
                })
                .OrderBy(x => x.CustomerGroupingBaseName)
                .ToListAsync();

            return View(bases);
        }

        [Authorize("CRM.CustomerGroupings.View")]
        [HttpGet]
        public async Task<IActionResult> BasesGrid()
        {
            var bases = await _db.CustomerGroupingBases
                .Where(x => !x.IsDeleted)
                .Select(x => new CustomerGroupingBaseVM
                {
                    Id = x.Id,
                    CustomerGroupingBaseName = x.CustomerGroupingBaseName,
                    //IsActive = x.IsActive,
                    GroupCount = x.CustomerGroups.Count(g => !g.IsDeleted)
                })
                .OrderBy(x => x.CustomerGroupingBaseName)
                .ToListAsync();

            return PartialView("_BasesGrid", bases);
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public IActionResult CreateBase()
        {
            return PartialView("_BaseForm", new CustomerGroupingBaseFormVM());
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> EditBase(Guid id)
        {
            var entity = await _db.CustomerGroupingBases.FindAsync(id);
            if (entity == null) return NotFound();

            var vm = new CustomerGroupingBaseFormVM
            {
                Id = entity.Id,
                CustomerGroupingBaseName = entity.CustomerGroupingBaseName,
                //IsActive = entity.IsActive
            };
            return PartialView("_BaseForm", vm);
        }

        [HttpPost]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> SaveBase(CustomerGroupingBaseFormVM vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_BaseForm", vm);

            if (vm.Id == null || vm.Id == Guid.Empty)
            {
                var entity = new CustomerGroupingBase
                {
                    Id = Guid.CreateVersion7(),
                    CustomerGroupingBaseName = vm.CustomerGroupingBaseName,
                    //IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUser.UserId
                };
                _db.CustomerGroupingBases.Add(entity);
            }
            else
            {
                var entity = await _db.CustomerGroupingBases.FindAsync(vm.Id);
                if (entity == null) return NotFound();

                entity.CustomerGroupingBaseName = vm.CustomerGroupingBaseName;
                //entity.IsActive = vm.IsActive;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUser.UserId;
            }

            await _db.SaveChangesAsync();
            return Content("OK");
        }

        [HttpDelete]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> DeleteBase(Guid id)
        {
            var entity = await _db.CustomerGroupingBases.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;
            await _db.SaveChangesAsync();

            return Content("OK");
        }

        [Authorize("CRM.CustomerGroupings.View")]
        [HttpGet]
        public async Task<IActionResult> GroupsGrid(Guid baseId)
        {
            var groups = await _db.CustomerGroups
                .Where(x => !x.IsDeleted && x.CustomerGroupingBaseId == baseId)
                .Select(x => new CustomerGroupVM
                {
                    Id = x.Id,
                    CustomerGroupingBaseId = x.CustomerGroupingBaseId,
                    CustomerGroupName = x.CustomerGroupName,
                    //IsActive = x.IsActive,
                    MemberCount = x.CustomerBelongsToGroups.Count(m => !m.IsDeleted)
                })
                .OrderBy(x => x.CustomerGroupName)
                .ToListAsync();

            ViewBag.SelectedBaseId = baseId;

            // Get base name for header
            var baseName = await _db.CustomerGroupingBases
                .Where(x => x.Id == baseId)
                .Select(x => x.CustomerGroupingBaseName)
                .FirstOrDefaultAsync();
            ViewBag.BaseName = baseName;

            return PartialView("_GroupsGrid", groups);
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public IActionResult CreateGroup(Guid baseId)
        {
            var vm = new CustomerGroupFormVM { CustomerGroupingBaseId = baseId };
            return PartialView("_GroupForm", vm);
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> EditGroup(Guid id)
        {
            var entity = await _db.CustomerGroups.FindAsync(id);
            if (entity == null) return NotFound();

            var vm = new CustomerGroupFormVM
            {
                Id = entity.Id,
                CustomerGroupingBaseId = entity.CustomerGroupingBaseId,
                CustomerGroupName = entity.CustomerGroupName,
                //IsActive = entity.IsActive
            };
            return PartialView("_GroupForm", vm);
        }

        [HttpPost]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> SaveGroup(CustomerGroupFormVM vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_GroupForm", vm);

            if (vm.Id == null || vm.Id == Guid.Empty)
            {
                var entity = new CustomerGroup
                {
                    Id = Guid.CreateVersion7(),
                    CustomerGroupingBaseId = vm.CustomerGroupingBaseId,
                    CustomerGroupName = vm.CustomerGroupName,
                    //IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUser.UserId
                };
                _db.CustomerGroups.Add(entity);
            }
            else
            {
                var entity = await _db.CustomerGroups.FindAsync(vm.Id);
                if (entity == null) return NotFound();

                entity.CustomerGroupName = vm.CustomerGroupName;
                //entity.IsActive = vm.IsActive;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUser.UserId;
            }

            await _db.SaveChangesAsync();
            return Content("OK");
        }

        [HttpDelete]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            var entity = await _db.CustomerGroups.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;
            await _db.SaveChangesAsync();

            return Content("OK");
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.View")]
        public async Task<IActionResult> MembersGrid(Guid groupId, int page = 1, int pageSize = 10)
        {
            var query = _db.CustomerBelongsToGroups.Include(x => x.Customer)
                .ThenInclude(x => x.Person)
                .Where(x => !x.IsDeleted && x.CustomerGroupId == groupId);

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var members = await query
                .OrderByDescending(x => x.EnterDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CustomerBelongsToGroupVM
                {
                    Id = x.Id,
                    CustomerGroupId = x.CustomerGroupId,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.Person.PersonName, // adjust based on your Customer entity
                    EnterDate = x.EnterDate,
                    ExitDate = x.ExitDate,
                    EnterRemarks = x.EnterRemarks,
                    ExitRemarks = x.ExitRemarks,
                    //IsActive = x.IsActive
                })
                .ToListAsync();

            ViewBag.SelectedGroupId = groupId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            // Get group name for header
            var groupName = await _db.CustomerGroups
                .Where(x => x.Id == groupId)
                .Select(x => x.CustomerGroupName)
                .FirstOrDefaultAsync();
            ViewBag.GroupName = groupName;

            return PartialView("_MembersGrid", members);
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public IActionResult CreateMember(Guid groupId)
        {
            var vm = new CustomerBelongsToGroupFormVM
            {
                CustomerGroupId = groupId,
                EnterDate = DateTime.Now
            };
            return PartialView("_MemberForm", vm);
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> EditMember(Guid id)
        {
            var entity = await _db.CustomerBelongsToGroups
                .Include(x => x.Customer)
                .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            var vm = new CustomerBelongsToGroupFormVM
            {
                Id = entity.Id,
                CustomerGroupId = entity.CustomerGroupId,
                CustomerId = entity.CustomerId,
                EnterDate = entity.EnterDate,
                ExitDate = entity.ExitDate,
                EnterRemarks = entity.EnterRemarks,
                ExitRemarks = entity.ExitRemarks,
                //IsActive = entity.IsActive
            };

            ViewBag.CustomerName = entity.Customer?.Person.PersonName;
            return PartialView("_MemberForm", vm);
        }

        [HttpPost]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> SaveMember(CustomerBelongsToGroupFormVM vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_MemberForm", vm);

            if (vm.Id == null || vm.Id == Guid.Empty)
            {
                var entity = new CustomerBelongsToGroup
                {
                    Id = Guid.CreateVersion7(),
                    CustomerGroupId = vm.CustomerGroupId,
                    CustomerId = vm.CustomerId,
                    EnterDate = vm.EnterDate,
                    ExitDate = vm.ExitDate ?? DateTime.MaxValue,
                    EnterRemarks = vm.EnterRemarks ?? "",
                    ExitRemarks = vm.ExitRemarks ?? "",
                    //IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUser.UserId
                };
                _db.CustomerBelongsToGroups.Add(entity);
            }
            else
            {
                var entity = await _db.CustomerBelongsToGroups.FindAsync(vm.Id);
                if (entity == null) return NotFound();

                entity.CustomerId = vm.CustomerId;
                entity.EnterDate = vm.EnterDate;
                entity.ExitDate = vm.ExitDate ?? DateTime.MaxValue;
                entity.EnterRemarks = vm.EnterRemarks ?? "";
                entity.ExitRemarks = vm.ExitRemarks ?? "";
                //entity.IsActive = vm.IsActive;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUser.UserId;
            }

            await _db.SaveChangesAsync();
            return Content("OK");
        }

        [HttpDelete]
        [Authorize("CRM.CustomerGroupings.Edit")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            var entity = await _db.CustomerBelongsToGroups.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;
            await _db.SaveChangesAsync();

            return Content("OK");
        }

        [HttpGet]
        [Authorize("CRM.CustomerGroupings.View")]
        public async Task<IActionResult> SearchCustomers(Guid groupId, string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(new List<CustomerSearchResultVM>());

            // Get existing customer IDs in this group
            var existingIds = await _db.CustomerBelongsToGroups
                .Include(x => x.Customer)
                .ThenInclude(x => x.Person)
                .Where(x => x.CustomerGroupId == groupId && !x.IsDeleted)
                .Select(x => x.CustomerId)
                .ToListAsync();

            var customers = await _db.Customers
                .Where(x => !x.IsDeleted
                    && !existingIds.Contains(x.Id)
                    && x.Person.PersonName.Contains(term))
                .Take(15)
                .Select(x => new CustomerSearchResultVM
                {
                    Id = x.Id,
                    FullName = x.Person.PersonName
                })
                .ToListAsync();

            return Json(customers);
        }
    }
}
