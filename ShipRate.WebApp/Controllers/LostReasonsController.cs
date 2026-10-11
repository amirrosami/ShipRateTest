using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.CustomersVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class LostReasonsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LostReasonsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize("Marketing.LostReasons.View")]
        public async Task<IActionResult> Index()
        {
            var categories = await _db.LostReasonCategories
                .Where(c => !c.IsDeleted)
                .Select(c => new LostReasonCategoryListItem
                {
                    Id = c.Id,
                    LostReasonCategoryName = c.LostReasonCategoryName,
                    LostReasonCategoryLocalName = c.LostReasonCategoryLocalName,
                    IsActive = c.IsActive,
                    ReasonsCount = _db.LostReasons.Count(r => r.LostReasonCategoryId == c.Id && !r.IsDeleted)
                })
                .ToListAsync();

            var model = new LostReasonCategoryListViewModel
            {
                Categories = categories,
                SelectedCategoryId = null
            };

            return View(model);
        }

        [Authorize("Marketing.LostReasons.View")]
        [HttpGet]
        public async Task<IActionResult> CategoryGrid()
        {
            var categories = await _db.LostReasonCategories
                .Where(c => !c.IsDeleted)
                .Select(c => new LostReasonCategoryListItem
                {
                    Id = c.Id,
                    LostReasonCategoryName = c.LostReasonCategoryName,
                    LostReasonCategoryLocalName = c.LostReasonCategoryLocalName,
                    IsActive = c.IsActive,
                    ReasonsCount = _db.LostReasons.Count(r => r.LostReasonCategoryId == c.Id && !r.IsDeleted)
                })
                .ToListAsync();

            return PartialView("_CategoryGrid", categories);
        }

        [HttpGet]
        [Authorize("Marketing.LostReasons.Edit")]
        public IActionResult CreateCategory()
        {
            return PartialView("_CategoryForm", new LostReasonCategoryViewModel());
        }

        [HttpGet]
        [Authorize("Marketing.LostReasons.Edit")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var cat = await _db.LostReasonCategories.FindAsync(id);
            if (cat == null || cat.IsDeleted) return NotFound();

            var vm = new LostReasonCategoryViewModel
            {
                Id = cat.Id,
                LostReasonCategoryName = cat.LostReasonCategoryName,
                LostReasonCategoryLocalName = cat.LostReasonCategoryLocalName,
                IsActive = cat.IsActive
            };

            return PartialView("_CategoryForm", vm);
        }

        [HttpPost]
        [Authorize("Marketing.LostReasons.Edit")]
        public async Task<IActionResult> SaveCategory(LostReasonCategoryViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_CategoryForm", vm);

            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

            if (vm.Id == 0)
            {
                var entity = new LostReasonCategory
                {
                    LostReasonCategoryName = vm.LostReasonCategoryName,
                    LostReasonCategoryLocalName = vm.LostReasonCategoryLocalName,
                    IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreateDatetime = DateTime.Now,
                    UpdateDatetime = DateTime.Now,
                    CreateUserId = userId
                };
                _db.LostReasonCategories.Add(entity);
            }
            else
            {
                var entity = await _db.LostReasonCategories.FindAsync(vm.Id);
                if (entity == null || entity.IsDeleted) return NotFound();

                entity.LostReasonCategoryName = vm.LostReasonCategoryName;
                entity.LostReasonCategoryLocalName = vm.LostReasonCategoryLocalName;
                entity.IsActive = vm.IsActive;
                entity.UpdateDatetime = DateTime.Now;
                entity.EditUserId = userId;
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "categoryChanged";
            return Content("");
        }

        [HttpDelete]
        [Authorize("Marketing.LostReasons.Edit")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var entity = await _db.LostReasonCategories.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdateDatetime = DateTime.Now;

            // soft delete reasons too
            var reasons = await _db.LostReasons
                .Where(r => r.LostReasonCategoryId == id && !r.IsDeleted)
                .ToListAsync();
            foreach (var r in reasons) r.IsDeleted = true;

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "categoryChanged";
            return Content("");
        }

        [HttpGet]
        [Authorize("Marketing.LostReasons.View")]
        public async Task<IActionResult> ReasonGrid(int categoryId)
        {
            var category = await _db.LostReasonCategories.FindAsync(categoryId);
            if (category == null || category.IsDeleted)
                return PartialView("_ReasonGrid", new LostReasonListViewModel());

            var reasons = await _db.LostReasons
                .Where(r => r.LostReasonCategoryId == categoryId && !r.IsDeleted)
                .Select(r => new LostReasonListItem
                {
                    Id = r.Id,
                    LostReasonName = r.LostReasonName,
                    LostReasonLocalName = r.LostReasonLocalName,
                    IsActive = r.IsActive,
                    LostReasonCategoryId = r.LostReasonCategoryId
                })
                .ToListAsync();

            var model = new LostReasonListViewModel
            {
                CategoryId = categoryId,
                CategoryName = category.LostReasonCategoryName,
                Reasons = reasons
            };

            return PartialView("_ReasonGrid", model);
        }

        [HttpGet]
        [Authorize("Marketing.LostReasons.Edit")]
        public IActionResult CreateReason(int categoryId)
        {
            var vm = new LostReasonViewModel { LostReasonCategoryId = categoryId };
            return PartialView("_ReasonForm", vm);
        }

        [HttpGet]
        [Authorize("Marketing.LostReasons.Edit")]
        public async Task<IActionResult> EditReason(int id)
        {
            var reason = await _db.LostReasons.FindAsync(id);
            if (reason == null || reason.IsDeleted) return NotFound();

            var vm = new LostReasonViewModel
            {
                Id = reason.Id,
                LostReasonCategoryId = reason.LostReasonCategoryId,
                LostReasonName = reason.LostReasonName,
                LostReasonLocalName = reason.LostReasonLocalName,
                IsActive = reason.IsActive
            };

            return PartialView("_ReasonForm", vm);
        }

        [HttpPost]
        [Authorize("Marketing.LostReasons.Edit")]
        public async Task<IActionResult> SaveReason(LostReasonViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_ReasonForm", vm);

            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

            if (vm.Id == 0)
            {
                var entity = new LostReason
                {
                    LostReasonCategoryId = vm.LostReasonCategoryId,
                    LostReasonName = vm.LostReasonName,
                    LostReasonLocalName = vm.LostReasonLocalName,
                    IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreateDatetime = DateTime.Now,
                    UpdateDatetime = DateTime.Now,
                    CreateUserId = userId
                };
                _db.LostReasons.Add(entity);
            }
            else
            {
                var entity = await _db.LostReasons.FindAsync(vm.Id);
                if (entity == null || entity.IsDeleted) return NotFound();

                entity.LostReasonName = vm.LostReasonName;
                entity.LostReasonLocalName = vm.LostReasonLocalName;
                entity.IsActive = vm.IsActive;
                entity.UpdateDatetime = DateTime.Now;
                entity.EditUserId = userId;
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "reasonChanged";
            return Content("");
        }

        [HttpDelete]
        [Authorize("Marketing.LostReasons.Edit")]
        public async Task<IActionResult> DeleteReason(int id)
        {
            var entity = await _db.LostReasons.FindAsync(id);
            if (entity == null) return NotFound();

            var categoryId = entity.LostReasonCategoryId;
            entity.IsDeleted = true;
            entity.UpdateDatetime = DateTime.Now;

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "reasonChanged";
            return Content("");
        }
    }
}