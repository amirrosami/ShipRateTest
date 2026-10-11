using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.ContainerViewModels;

namespace ShipRate.WebApp.Controllers
{
    // Controllers/ContainerController.cs
    [Authorize]
    public class ContainerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IBaseDataService _baseDataService;
        private const int PageSize = 10;

        public ContainerController(ApplicationDbContext context, ICurrentUserService currentUser, IBaseDataService baseDataService)
        {
            _context = context;
            _currentUser = currentUser;
            _baseDataService = baseDataService;
        }



        [Authorize("Containers.Containers.View")]
        public async Task<IActionResult> Index(
            string? search, int? ownLeaseTypeId, int? cntOwnTypeId,
            Guid? ownerId, bool? isActive, bool? responsibilityByUs,
            int? responsibleCompanyId, int page = 1)
        {
            var query = _context.Containers
                .Include(c => c.ContainerSizeType)
                .Include(c => c.CntOwnType)
                .Include(c => c.CntOwnLeaseType)
                .Include(c => c.Owner)
                .Include(c => c.ResponsibleCompany)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.ContainerNumber.Contains(search));

            if (ownLeaseTypeId.HasValue)
                query = query.Where(c => c.OwnLeaseTypeId == ownLeaseTypeId);

            if (cntOwnTypeId.HasValue)
                query = query.Where(c => c.CntOwnTypeId == cntOwnTypeId);

            if (ownerId.HasValue)
                query = query.Where(c => c.OwnerId == ownerId);

            //if (isActive.HasValue)
            //    query = query.Where(c => c.IsActive == isActive);

            if (responsibilityByUs.HasValue)
                query = query.Where(c => c.ResponsibilityByUs == responsibilityByUs);

            if (responsibleCompanyId.HasValue)
                query = query.Where(c => c.ResponsibleCompanyId == responsibleCompanyId);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ContainerListVM
                {
                    Id = c.Id,
                    ContainerNumber = c.ContainerNumber,
                    SizeType = c.ContainerSizeType.ShortName,
                    OwnLeaseType = c.CntOwnLeaseType.CntOwnLeaseTypeName,
                    CntOwnType = c.CntOwnType.CntOwnTypeName,
                    OwnerName = c.Owner != null ? c.Owner.PersonName : null,
                    ResponsibilityByUs = c.ResponsibilityByUs,
                    ResponsibleCompany = c.ResponsibleCompany != null ? c.ResponsibleCompany.Person.PersonName : null,
                    MaxPayLoad = c.MaxPayLoad,
                    TareWeight = c.TareWeight
                })
                .ToListAsync();


            ViewBag.Search = search;
            ViewBag.OwnLeaseTypeId = ownLeaseTypeId;
            ViewBag.CntOwnTypeId = cntOwnTypeId;
            ViewBag.OwnerId = ownerId;
            ViewBag.IsActive = isActive;
            ViewBag.ResponsibilityByUs = responsibilityByUs;
            ViewBag.ResponsibleCompanyId = responsibleCompanyId;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            await PopulateViewBags();

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_Table", items);

            return View(items);
        }

        [Authorize("Containers.Containers.Edit")]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            return PartialView("_Form", new ContainerFormVM());
        }

        [HttpPost]
        [Authorize("Containers.Containers.Edit")]
        public async Task<IActionResult> Create(ContainerFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBags();
                return PartialView("_Form", vm);
            }

            var entity = new Container
            {
                Id = Guid.CreateVersion7(),
                ContainerNumber = vm.ContainerNumber,
                ContainerSizeTypeId = vm.ContainerSizeTypeId,
                CntOwnTypeId = vm.CntOwnTypeId,
                OwnLeaseTypeId = vm.OwnLeaseTypeId,
                MaxPayLoad = vm.MaxPayLoad,
                TareWeight = vm.TareWeight,
                OwnerId = vm.OwnerId,
                ResponsibilityByUs = vm.ResponsibilityByUs,
                ResponsibleCompanyId = vm.ResponsibleCompanyId,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUser.UserId
            };

            _context.Containers.Add(entity);
            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("");
        }

        [Authorize("Containers.Containers.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _context.Containers.FindAsync(id);
            if (entity == null) return NotFound();

            await PopulateViewBags();

            //ViewBag.OwnLeaseTypeId = entity.OwnLeaseTypeId;
            //ViewBag.CntOwnTypeId = entity.CntOwnTypeId;
            //ViewBag.OwnerId = entity.OwnerId;
            //ViewBag.IsActive = entity.IsActive;
            //ViewBag.ResponsibilityByUs = entity.ResponsibilityByUs;
            //ViewBag.ResponsibleCompanyId = entity.ResponsibleCompanyId;

            var vm = new ContainerFormVM
            {
                Id = entity.Id,
                ContainerNumber = entity.ContainerNumber,
                ContainerSizeTypeId = entity.ContainerSizeTypeId,
                CntOwnTypeId = entity.CntOwnTypeId,
                OwnLeaseTypeId = entity.OwnLeaseTypeId,
                MaxPayLoad = entity.MaxPayLoad,
                TareWeight = entity.TareWeight,
                OwnerId = entity.OwnerId,
                ResponsibilityByUs = entity.ResponsibilityByUs,
                ResponsibleCompanyId = entity.ResponsibleCompanyId
            };

            return PartialView("_Form", vm);
        }

        [HttpPost]
        [Authorize("Containers.Containers.Edit")]
        public async Task<IActionResult> Edit(ContainerFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBags();
                return PartialView("_Form", vm);
            }

            var entity = await _context.Containers.FindAsync(vm.Id);
            if (entity == null) return NotFound();

            entity.ContainerNumber = vm.ContainerNumber;
            entity.ContainerSizeTypeId = vm.ContainerSizeTypeId;
            entity.CntOwnTypeId = vm.CntOwnTypeId;
            entity.OwnLeaseTypeId = vm.OwnLeaseTypeId;
            entity.MaxPayLoad = vm.MaxPayLoad;
            entity.TareWeight = vm.TareWeight;
            entity.OwnerId = vm.OwnerId;
            entity.ResponsibilityByUs = vm.ResponsibilityByUs;
            entity.ResponsibleCompanyId = vm.ResponsibleCompanyId;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;

            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("");
        }

        [HttpDelete]
        [Authorize("Containers.Containers.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _context.Containers.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;

            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("");
        }

        private async Task PopulateViewBags()
        {
            ViewBag.SizeTypes = new SelectList(
                await _context.ContainerSizeTypes.ToListAsync(), "Id", "ShortName");

            //ViewBag.OwnLeaseTypes = new SelectList(
            //    await _context.CntOwnLeaseTypes.ToListAsync(), "Id", "CntOwnLeaseTypeName");


            ViewBag.OwnLeaseTypes = await _context.CntOwnLeaseTypes
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnLeaseTypeName
                })
                .ToListAsync();

            ViewBag.CntOwnTypes = await _context.CntOwnTypes
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();
            ViewBag.Owners = new SelectList(
                await _context.Persons.OrderBy(x => x.PersonName).ToListAsync(), "Id", "PersonName");
            ViewBag.Companies = new SelectList(
                await _context.Companies
                .Include(x => x.Person)
                .Select(x => new { x.Id, x.Person.PersonName })
                .ToListAsync(), "Id", "PersonName");
        }
        [HttpGet]
        public async Task<IActionResult> LookupContainer(string containerNumber, int? targetSizeTypeId)
        {
            if (string.IsNullOrWhiteSpace(containerNumber) || containerNumber.Trim().Length < 11)
            {
                return PartialView("_ContainerLookupResult", new ContainerLookupResultViewModel());
            }

            containerNumber = containerNumber.Trim().ToUpperInvariant();
            var container = await _context.Containers
                .Include(c => c.ContainerSizeType)
                .FirstOrDefaultAsync(c => c.ContainerNumber == containerNumber);

            var vm = new ContainerLookupResultViewModel
            {
                ContainerNumber = containerNumber
            };

            if (container != null)
            {
                vm.Exists = true;
                vm.ContainerId = container.Id;
                vm.SizeTypeName = container.ContainerSizeType?.ShortName;
                vm.TareWeight = container.TareWeight;
                vm.IsMismatchWithType = targetSizeTypeId.HasValue && container.ContainerSizeTypeId != targetSizeTypeId.Value;
            }
            else
            {
                vm.Exists = false;
                // پر کردن دراپ‌داون نوع کانتینرها (با پیش‌فرض سایز انتخابی RTS)
                var types = await _baseDataService.GetContainerSizeTypesAsync();
                vm.ContainerTypeSelectList = new SelectList(types, "Id", "ShortName", targetSizeTypeId);
            }

            return PartialView("_ContainerLookupResult", vm);
        }
    }
}
