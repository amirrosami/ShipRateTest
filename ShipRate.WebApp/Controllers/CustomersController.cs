using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.CustomersVms;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ICurrentUserService _currentUserService { get; set; }

        public CustomersController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }



        [HttpGet]
        [Authorize("CRM.Customers.View")]
        public async Task<IActionResult> Index()
        {
            var vm = new CustomerSearchViewModel();
            ViewBag.Industries = await _db.Industries
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.IndustryName)
                .ToListAsync();

            ViewBag.SalesUsers = await _db.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(vm);
        }

        [Authorize("CRM.Customers.View")]
        [HttpGet]
        public async Task<IActionResult> List(CustomerSearchViewModel search)
        {
            if (search.Page < 1) search.Page = 1;
            if (search.PageSize < 5) search.PageSize = 10;

            var query = _db.Customers
                .Include(c => c.Person)
                .Include(c => c.Industry)
                .Include(c => c.SalesUser)
                .Include(c => c.CustomerTradeTypes.Where(ct => !ct.IsDeleted))
                    .ThenInclude(ct => ct.TradeType)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // ── Filters ──
            if (!string.IsNullOrWhiteSpace(search.PersonName))
                query = query.Where(c => c.Person.PersonName.Contains(search.PersonName));

            //if (search.Status == "active")
            //    query = query.Where(c => c.IsActive);
            //else if (search.Status == "inactive")
            //    query = query.Where(c => !c.IsActive);

            if (search.IndustryId.HasValue)
                query = query.Where(c => c.IndustryId == search.IndustryId);

            if (search.SalesUserId.HasValue)
                query = query.Where(c => c.SalesUserId == search.SalesUserId);

            // ── Sort + Paginate ──
            var sorted = query.OrderByDescending(c => c.CreatedAt)
                .Select(c => new CustomerListViewModel
                {
                    Id = c.Id,
                    PersonName = c.Person.PersonName,
                    LocalName = c.Person.PersonPersianName,
                    IndustryName = c.Industry != null ? c.Industry.IndustryName : "—",
                    SalesUserName = c.SalesUser != null ? c.SalesUser.FullName : "—",
                    //IsActive = c.IsActive,
                    Remarks = c.Remarks,
                    TradeTypeNames = c.CustomerTradeTypes
                        .Where(ct => !ct.IsDeleted)
                        .OrderBy(x => x.Id)
                        .Select(ct => ct.TradeType.TradeTypeName)
                        .ToList()
                });

            var paginatedList = await PaginatedList<CustomerListViewModel>
                .CreateAsync(sorted, search.Page, search.PageSize);

            // ── اطلاعات جستجو رو برای ساخت لینک‌های صفحه‌بندی پاس بده ──
            ViewBag.Search = search;

            return PartialView("_List", paginatedList);
        }
        [HttpGet]
        [Authorize("CRM.Customers.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {



            var customer = await _db.Customers
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            // فعلی‌های انتخاب شده
            var selectedTradeTypeIds = await _db.CustomerTradeTypes
                .Where(ct => ct.CustomerId == id && !ct.IsDeleted)
                .Select(ct => ct.TradeTypeId)
                .ToListAsync();

            var vm = new CustomerEditViewModel
            {
                Id = customer.Id,
                PersonName = customer.Person.PersonName,
                IndustryId = customer.IndustryId,
                Remarks = customer.Remarks,
                //IsActive = customer.IsActive,
                SelectedTradeTypeIds = selectedTradeTypeIds,

                Industries = await _db.Industries
                    .Where(i => !i.IsDeleted)
                    .OrderBy(i => i.IndustryName)
                    .Select(i => new IndustryDropdownItem { Id = i.Id, Name = i.IndustryName })
                    .ToListAsync(),

                TradeTypes = await _db.TradeTypes
                    .OrderBy(t => t.Id)
                    .Select(t => new TradeTypeDropdownItem { Id = t.Id, Name = t.TradeTypeName })
                    .ToListAsync()
            };

            return PartialView("_EditForm", vm);
        }
        [Authorize("CRM.Customers.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Industries = await _db.Industries
                    .Where(i => !i.IsDeleted)
                    .OrderBy(i => i.IndustryName)
                    .Select(i => new IndustryDropdownItem { Id = i.Id, Name = i.IndustryName })
                    .ToListAsync();

                vm.TradeTypes = await _db.TradeTypes
                    .OrderBy(t => t.TradeTypeName)
                    .Select(t => new TradeTypeDropdownItem { Id = t.Id, Name = t.TradeTypeName })
                    .ToListAsync();

                var person = await _db.Persons.FindAsync(vm.Id);
                vm.PersonName = person?.PersonName ?? "";

                Response.StatusCode = 422;
                return PartialView("_EditForm", vm);
            }


            var customer = await _db.Customers.FindAsync(vm.Id);
            if (customer == null) return NotFound();

            // ── Update Customer fields ──
            customer.IndustryId = vm.IndustryId;
            customer.Remarks = vm.Remarks ?? string.Empty;
            //customer.IsActive = vm.IsActive;
            customer.UpdatedAt = DateTime.Now;
            customer.EditUserId = _currentUserService.UserId;

            var existingTradeTypes = await _db.CustomerTradeTypes
                .Where(ct => ct.CustomerId == vm.Id)
                .ToListAsync();

            var selectedIds = vm.SelectedTradeTypeIds ?? new List<int>();

            // 1) آیتم‌هایی که قبلاً بوده ولی الان انتخاب نشده → Soft Delete
            foreach (var existing in existingTradeTypes)
            {
                if (!selectedIds.Contains(existing.TradeTypeId))
                {
                    existing.IsDeleted = true;
                    //existing.IsActive = false;
                    existing.UpdatedAt = DateTime.Now;
                    // existing.EditUserId = currentUserId;
                }
                else
                {
                    // 2) آیتم‌هایی که قبلاً بوده و هنوز انتخابه → Restore if deleted
                    existing.IsDeleted = false;
                    //existing.IsActive = true;
                    existing.UpdatedAt = DateTime.Now;
                }
            }

            // 3) آیتم‌های جدیدی که قبلاً وجود نداشته → Add
            var existingTradeTypeIds = existingTradeTypes.Select(e => e.TradeTypeId).ToList();
            foreach (var newTradeTypeId in selectedIds.Where(id => !existingTradeTypeIds.Contains(id)))
            {
                _db.CustomerTradeTypes.Add(new CustomerTradeType
                {
                    Id = Guid.CreateVersion7(),
                    CustomerId = vm.Id,
                    TradeTypeId = newTradeTypeId,


                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    // CreateUserId = currentUserId
                });
            }

            await _db.SaveChangesAsync();

            //Response.Headers["HX-Trigger"] = """{"closeModal":"","showToast":{"message":"Customer updated successfully","type":"success"},"refreshCustomersList":""}""";

            HxTrigger(new
            {
                //showToast = new { message = "Customer updated successfully.", type = "success" },
                closeModalRefreshCustomers = new { message = "Customer updated successfully.", type = "success" }
            });



            return Content("");
        }

        [HttpGet]
        [Authorize("CRM.Customers.AssignSalesUser")]
        public async Task<IActionResult> AssignSalesUser(Guid id)
        {
            var customer = await _db.Customers
                .Include(c => c.Person)
                .Include(c => c.SalesUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            var vm = new AssignSalesUserViewModel
            {
                CustomerId = customer.Id,
                PersonName = customer.Person.PersonName,
                CurrentSalesUserId = customer.SalesUserId,
                CurrentSalesUserName = customer.SalesUser?.FullName,
                SelectedSalesUserId = customer.SalesUserId,
                SalesUsers = await _db.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SalesUserDropdownItem { Id = u.Id, FullName = u.FullName })
                    .ToListAsync()
            };

            ViewBag.SalesUsers = await _db.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();


            return PartialView("_AssignSalesUser", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.Customers.AssignSalesUser")]
        public async Task<IActionResult> AssignSalesUser(AssignSalesUserViewModel vm)
        {
            var customer = await _db.Customers.FindAsync(vm.CustomerId);
            if (customer == null) return NotFound();

            customer.SalesUserId = vm.SelectedSalesUserId;
            customer.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            //Response.Headers["HX-Trigger"] = """{"closeModal":"","showToast":{"message":"Sales user assigned successfully","type":"success"},"refreshCustomersList":""}""";


            HxTrigger(new
            {
                closeSecondModalRefreshCustomers = new { },
                showToast = new { message = "Assigned successfully.", type = "success" }
            });



            return Content("");
        }

        [HttpGet]
        [Authorize("CRM.Customers.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _db.Customers
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.Customers.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            //Response.Headers["HX-Trigger"] = """{"closeModal":"","showToast":{"message":"Customer deleted successfully","type":"success"},"refreshCustomerList":""}""";

            Response.Headers["HX-Trigger"] = "closeSecondModalRefreshCustomers";
            return Content("");
        }
    }
}
