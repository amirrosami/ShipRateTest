using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.ChargeTypeVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class ChargeTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChargeTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ChargeTypes
        [Authorize("Finance.ChargeTypes.View")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Set<ChargeTypeCategory>()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new ChargeTypeCategoryItem
                {
                    Id = c.Id,
                    ChargeTypeCategoryName = c.ChargeTypeCategoryName,
                    ChargeTypeCategoryLocalName = c.ChargeTypeCategoryLocalName,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            var model = new ChargeTypeListViewModel
            {
                Categories = categories,
                ChargeTypes = new List<ChargeTypeItem>()
            };

            return View(model);
        }

        // GET: /ChargeTypes/CategoryList
        [Authorize("Finance.ChargeTypes.View")]
        public async Task<IActionResult> CategoryList(string searchTerm)
        {
            var query = _context.Set<ChargeTypeCategory>()
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // Filter by search term if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var categoryIds = await _context.Set<ChargeType>()
                    .Where(ct => !ct.IsDeleted &&
                           ct.ChargeTypeName.Contains(searchTerm))
                    .Select(ct => ct.ChargeTypeCategoryId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(c => categoryIds.Contains(c.Id));
            }

            var categories = await query
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new ChargeTypeCategoryItem
                {
                    Id = c.Id,
                    ChargeTypeCategoryName = c.ChargeTypeCategoryName,
                    ChargeTypeCategoryLocalName = c.ChargeTypeCategoryLocalName,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            var model = new ChargeTypeListViewModel
            {
                SearchTerm = searchTerm,
                Categories = categories
            };

            return PartialView("_CategoryGrid", model);
        }

        // GET: /ChargeTypes/List
        [Authorize("Finance.ChargeTypes.View")]
        public async Task<IActionResult> List(int? categoryId, string searchTerm)
        {
            if (!categoryId.HasValue)
            {
                return PartialView("_ChargeTypeGrid", new ChargeTypeListViewModel
                {
                    ChargeTypes = new List<ChargeTypeItem>()
                });
            }

            var query = _context.Set<ChargeType>()
                .Include(ct => ct.ChargeTypeCategory)
                .Include(ct => ct.Currency)
                .Include(ct => ct.CalculationBy)
                .Where(ct => !ct.IsDeleted && ct.ChargeTypeCategoryId == categoryId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(ct => ct.ChargeTypeName.Contains(searchTerm));
            }

            var chargeTypes = await query
                .OrderBy(ct => ct.Id)
                .Select(ct => new ChargeTypeItem
                {
                    Id = ct.Id,
                    ChargeTypeCode = ct.ChargeTypeCode,
                    ChargeTypeCode2 = ct.ChargeTypeCode2,
                    ChargeTypeName = ct.ChargeTypeName,
                    ChargeTypeLocalName = ct.ChargeTypeLocalName,
                    CategoryName = ct.ChargeTypeCategory.ChargeTypeCategoryName,
                    CurrencyCode = ct.Currency != null ? ct.Currency.CurrencyCode : "",
                    CalculationByName = ct.CalculationBy.CalculationByName,
                    IsTaxable = ct.IsTaxable.Value,
                    IsActive = ct.IsActive,
                    SystemDefined = ct.SystemDefined
                })
                .ToListAsync();

            var model = new ChargeTypeListViewModel
            {
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm,
                ChargeTypes = chargeTypes
            };
            return PartialView("_ChargeTypeGrid", model);
        }

        // GET: /ChargeTypes/Form
        [Authorize("Finance.ChargeTypes.Edit")]
        public async Task<IActionResult> Form(int? id, int? categoryId)
        {
            var model = new ChargeTypeFormViewModel();

            if (id.HasValue)
            {
                var entity = await _context.Set<ChargeType>()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null) return NotFound();

                model.Id = entity.Id;
                model.ChargeTypeCode = entity.ChargeTypeCode;
                model.ChargeTypeName = entity.ChargeTypeName;
                model.ChargeTypeLocalName = entity.ChargeTypeLocalName;
                model.ChargeTypeCategoryId = entity.ChargeTypeCategoryId;
                model.IsTaxable = entity.IsTaxable.Value;
                model.AllowManualAmount = entity.AllowManualAmount is null ? false : entity.AllowManualAmount.Value;
                //model.AllowRateBased = entity.AllowRateBased is null ? false : entity.AllowRateBased.Value;
                model.CurrencyId = entity.CurrencyId;
                model.CalculationById = entity.CalculationById.Value;
                model.AffectProfit = entity.AffectProfit is null ? false : entity.AffectProfit.Value;
                model.DependsOnChargeTypeId = entity.DependsOnChargeTypeId;
                model.IsActive = entity.IsActive;
                model.Remarks = entity.Remarks;
                model.SystemDefined = entity.SystemDefined;
            }
            else if (categoryId.HasValue)
            {
                model.ChargeTypeCategoryId = categoryId.Value;
                model.IsActive = true;
            }

            model.Categories = await _context.Set<ChargeTypeCategory>()
                .Where(x => !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeCategoryName
                }).ToListAsync();

            model.Currencies = await _context.Set<Currency>()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode
                }).ToListAsync();

            model.CalculationByList = await _context.Set<CalculationBy>()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CalculationByName
                }).ToListAsync();

            model.ChargeTypesList = await _context.Set<ChargeType>()
                .Where(x => !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                }).ToListAsync();

            return PartialView("_ChargeTypeForm", model);
        }

        [HttpPost]
        [Authorize("Finance.ChargeTypes.Edit")]
        public async Task<IActionResult> Save(ChargeTypeFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChargeTypeForm", model);

            ChargeType entity;

            if (model.Id.HasValue)
            {
                entity = await _context.Set<ChargeType>().FindAsync(model.Id.Value);
                if (entity == null) return NotFound();
            }
            else
            {
                entity = new ChargeType();
                if (_context.ChargeTypes.Any())
                { entity.Id = _context.ChargeTypes.Max(x => x.Id) + 1; }
                else
                { entity.Id = 1; }
                _context.Add(entity);
            }

            entity.ChargeTypeCode = model.ChargeTypeCode;
            entity.ChargeTypeName = model.ChargeTypeName;
            entity.ChargeTypeLocalName = model.ChargeTypeLocalName;
            entity.ChargeTypeCategoryId = model.ChargeTypeCategoryId;
            entity.IsTaxable = model.IsTaxable;
            entity.AllowManualAmount = model.AllowManualAmount;
            //entity.AllowRateBased = model.AllowRateBased;
            entity.CurrencyId = model.CurrencyId;
            entity.CalculationById = model.CalculationById;
            entity.AffectProfit = model.AffectProfit;
            entity.DependsOnChargeTypeId = model.DependsOnChargeTypeId;
            //entity.MultiplierRate = model.MultiplierRate;
            entity.IsActive = model.IsActive;
            entity.Remarks = model.Remarks;
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "chargeTypeChanged";
            return Content("");
        }
        [HttpPost]
        [Authorize("Finance.ChargeTypes.Edit")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Set<ChargeType>().FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "chargeTypeChanged";
            return Content("");
        }
    }
}