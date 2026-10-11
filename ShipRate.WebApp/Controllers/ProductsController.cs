using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Linq.Dynamic.Core;
using System.Text.Json;
namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ProductsController(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _context = context;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        [Authorize("Inventory.Products.View")]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }

        [HttpPost]
        [Authorize("Inventory.Products.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);

            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Products/List",
                GridPartialName = "_List",
                columns = GetGridColumns()
            };

            return PartialView("_Grid", gridVm);
        }


        private async Task<ViewModels.PagedResult<ProductDto>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.Products
                .Include(x => x.ProductCategory)
                .Where(t => !t.IsDeleted);


            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.ProductName.Contains(dto.SearchTerm) || x.ProductCategory.ProductCategoryName.Contains(dto.SearchTerm) || x.ProductCode.Contains(dto.SearchTerm));
            }

            var select = data.Select(t => new ProductDto
            {
                Id = t.Id,
                ProductCode = t.ProductCode,
                ProductName = t.ProductName,
                ProductMinStock = t.ProductMinStock,
                ProductCategoryName = t.ProductCategory.ProductCategoryName,
                IsActive = t.IsActive,
            });

            int total = await select.CountAsync();

            if (!string.IsNullOrEmpty(dto.SortColumn) && !string.IsNullOrEmpty(dto.SortDirection))
            {
                select = select.OrderBy($"{dto.SortColumn}  {dto.SortDirection}");
            }
            var items = await select.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync();


            return new ViewModels.PagedResult<ProductDto>()
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

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
            new GridColumn("ProductCategoryName","Category"),
            new GridColumn("ProductCode","Product Code"),
            new GridColumn("ProductName","Product Name"),
            new GridColumn("ProductMinStock","Min Stock"),
            new GridColumn("IsActive","IsActive")
            };
        }


        private async Task PopulateViewBags()
        {
            var categories = await _context.ProductCategory.Select(x => new { Id = x.Id, ProductCategoryName = x.ProductCategoryName }).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "ProductCategoryName");
        }

        [HttpGet]
        [Authorize("Inventory.Products.Edit")]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            ViewBag.CreateEdit = "Create";
            return PartialView("_Form", new ProductDto { });
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.Products.Edit")]
        public async Task<IActionResult> Create(ProductDto dto)
        {

            ModelState.Clear();
            var entity = new Product
            {
                Id = Guid.CreateVersion7(),
                ProductCategoryId = dto.ProductCategoryId,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                ProductMinStock = dto.ProductMinStock,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };
            var validation = new ProductValidation().Validate(entity);
            if (!validation.IsValid)
            {
                foreach (var item in validation.Errors)
                {
                    ModelState.AddModelError("", item.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.CreateEdit = "Create";
                await PopulateViewBags();
                return PartialView("_Form");
            }
            _context.Add(entity);
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Content("");
        }

        [Authorize("Inventory.Products.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _context.Products.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            await PopulateViewBags();
            ViewBag.CreateEdit = "Edit";
            var dto = new ProductDto
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                ProductCategoryId = entity.ProductCategoryId,
                ProductName = entity.ProductName,
                ProductCode = entity.ProductCode,
                ProductMinStock = entity.ProductMinStock
            };
            return PartialView("_Form", dto);
        }
        // POST: /PackageTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.Products.Edit")]
        public async Task<IActionResult> Edit(ProductDto dto)
        {
            var entity = await _context.Products.Where(x => x.Id == dto.Id && !x.IsDeleted).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            entity.IsActive = dto.IsActive;
            entity.ProductMinStock = dto.ProductMinStock;
            entity.ProductCode = dto.ProductCode;
            entity.ProductName = dto.ProductName;
            entity.ProductCategoryId = dto.ProductCategoryId;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            var validation = new ProductValidation().Validate(entity);
            if (!validation.IsValid)
            {
                foreach (var item in validation.Errors)
                {
                    ModelState.AddModelError("", item.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.CreateEdit = "Edit";
                await PopulateViewBags();
                return PartialView("_Form");
            }
            _context.Update(entity);
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }

        // GET: /PackageTypes/DeleteConfirm/5

        [HttpGet]
        [Authorize("Inventory.Products.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Inventory.Products.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            product.IsDeleted = true;
            product.EditUserId = _currentUserService.UserId;
            product.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Deleted Sussefully"
            });

            return Content("");
        }
    }
}
