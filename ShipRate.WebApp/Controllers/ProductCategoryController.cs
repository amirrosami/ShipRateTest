using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryService _service;
        private readonly ICurrentUserService _cuurentUserService;
        private readonly ApplicationDbContext _context;

        public ProductCategoryController(IProductCategoryService service, ICurrentUserService cuurentUserService, ApplicationDbContext context)
        {
            _service = service;
            _cuurentUserService = cuurentUserService;
            _context = context;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        [Authorize("Inventory.ProductCategories.View")]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }

        private async Task<ViewModels.PagedResult<ProductCategoryDto>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.ProductCategory
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.ProductCategoryName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            if (!string.IsNullOrEmpty(dto.SortColumn) && !string.IsNullOrEmpty(dto.SortDirection))
            {
                data = data.OrderBy($"{dto.SortColumn}  {dto.SortDirection}");
            }

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new ProductCategoryDto
            {
                ProductCategoryName = x.ProductCategoryName,
                IsActive = x.IsActive,
                Id = x.Id,
            }).ToListAsync();

            return new ViewModels.PagedResult<ProductCategoryDto>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection,
                TotalCount = total,
            };
        }

        [HttpPost]
        [Authorize("Inventory.ProductCategories.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "ProductCategory/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>
            {
                new GridColumn("ProductCategoryName","Category"),
                new GridColumn("IsActive","Is Active"),
            };
        }

        [HttpGet]
        [Authorize("Inventory.ProductCategories.Edit")]
        public IActionResult Create()
        {
            ViewBag.CreateEdit = "Create";
            return PartialView("_Form", new ProductCategoryDto { IsActive = true });
        }


        // POST: /ProductCategorys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.ProductCategories.Edit")]
        public async Task<IActionResult> Create(ProductCategoryDto dto)
        {

            ModelState.Clear();

            if (string.IsNullOrEmpty(dto.ProductCategoryName))
            {
                ViewBag.CreateEdit = "Create";
                ModelState.AddModelError("", "Category is required");
                return PartialView("_Form");
            }

            if (_context.ProductCategory.Any(x => x.ProductCategoryName == dto.ProductCategoryName && !x.IsDeleted))
            {
                ViewBag.CreateEdit = "Create";
                ModelState.AddModelError("", "This Category exists");
                return PartialView("_Form");
            }

            var entity = new ProductCategory()
            {
                Id = Guid.CreateVersion7(),
                ProductCategoryName = dto.ProductCategoryName,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _cuurentUserService.UserId,
                IsActive = dto.IsActive,
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }



        [Authorize("Inventory.ProductCategories.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _context.ProductCategory.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            var dto = new ProductCategoryDto
            {
                Id = entity.Id,
                ProductCategoryName = entity.ProductCategoryName,
                IsActive = entity.IsActive
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_Form", dto);
        }




        // POST: /ProductCategorys/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.ProductCategories.Edit")]
        public async Task<IActionResult> Edit(ProductCategoryUpdateDto dto)
        {
            var entity = await _context.ProductCategory.Where(x => x.Id == dto.Id && !x.IsDeleted).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (string.IsNullOrEmpty(dto.ProductCategoryName))
            {
                ViewBag.CreateEdit = "Edit";
                ModelState.AddModelError("", "Category is required");
                return PartialView("_Form");
            }

            if (_context.ProductCategory.Any(x => x.ProductCategoryName == dto.ProductCategoryName && x.ProductCategoryName != dto.ProductCategoryName && !x.IsDeleted))
            {
                ViewBag.CreateEdit = "Edit";
                ModelState.AddModelError("", "This Category exists");
                return PartialView("_Form");
            }
            entity.ProductCategoryName = dto.ProductCategoryName;
            entity.IsActive = dto.IsActive;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }

        // GET: /ProductCategorys/DeleteConfirm/5

        [HttpGet]
        [Authorize("Inventory.ProductCategories.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.ProductCategories.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChanged = false,
                    toastError = error
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Deleted Sussefully"
            });
            return Content("");
        }


        [HttpPost]
        [Authorize("Inventory.ProductCategories.Edit")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var (success, error) = await _service.ToggleActiveAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                refreshProductCategoryList = new { },
                showToast = new { message = "Status toggled.", type = "info" }
            });

            return Content("");
        }
    }
}