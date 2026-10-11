using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class VesselsController : Controller
    {
        private readonly IVesselService _service;
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _context;

        public VesselsController(IVesselService service, IBaseDataService basedataservice, ApplicationDbContext context)
        {
            _service = service;
            _basedataservice = basedataservice;
            _context = context;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            //Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
            Response.Headers.Append("HX-Trigger", JsonSerializer.Serialize(triggers));
        }


        private async Task<PagedResult<VesselDto>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.Vessels
                .Where(x => !x.IsDeleted)
                .Include(x => x.VesselType)
                .Include(x => x.OwnType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.VesselCode.Contains(dto.SearchTerm) || x.VesselName.Contains(dto.SearchTerm) || x.VesselType.VesselTypeName.Contains(dto.SearchTerm) || x.OwnType.VesselOwnTypeName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new VesselDto
            {
                Id = x.Id,
                VesselCode = x.VesselCode,
                VesselName = x.VesselName,
                VesselTypeName = x.VesselType.VesselTypeName,
                OwnTypeName = x.OwnType.VesselOwnTypeName
            }).ToListAsync();

            return new PagedResult<VesselDto>()
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

        [Authorize("BaseInfo.Vessels.View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Vessels/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> { new GridColumn("VesselCode", "VesselCode"), new GridColumn("VesselName", "Vessel Name"), new GridColumn("VesselTypeName", "VesselTypeName"), new GridColumn("OwnTypeName", "Own Type Name") };
        }


        public async Task CreateViewBags()
        {
            var countries = await _basedataservice.GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "CountryName");

            var vesseltypes = await _basedataservice.GetVesselTypesAsync();
            ViewBag.VesselTypes = new SelectList(vesseltypes, "Id", "VesselTypeName");

            var shippinglines = await _basedataservice.GetCarriers(1);
            ViewBag.ShippingLines = new SelectList(shippinglines, "Id", "Person.PersonName");

            var vesselowntypes = await _basedataservice.GetVesselOwnTypesAsync();
            ViewBag.VesselOwnTypes = new SelectList(vesselowntypes, "Id", "VesselOwnTypeName");

        }


        [HttpGet]
        [Authorize(Policy = "BaseInfo.Vessels.Edit")]
        public async Task<IActionResult> Create()
        {
            await CreateViewBags();
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new VesselCreateEditDto { });
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BaseInfo.Vessels.Edit")]
        public async Task<IActionResult> Create(VesselCreateEditDto dto)
        {

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                await CreateViewBags();
                ViewBag.CreateEdit = "Create";
                return PartialView("_CreateEdit", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await CreateViewBags();
                ViewBag.CreateEdit = "Create";
                return PartialView("_CreateEdit", dto);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });

            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Vessels.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await CreateViewBags();

            var dto = new VesselCreateEditDto
            {
                Id = entity.Id,
                BuiltYear = entity.BuiltYear,
                CallSign = entity.CallSign,
                CapacityTEU = entity.CapacityTEU,
                DWT = entity.DWT,
                FlagCountryId = entity.FlagCountryId,
                GrossTonnage = entity.GrossTonnage,
                IMONumber = entity.IMONumber,
                NetTonnage = entity.NetTonnage,
                OwnerId = entity.OwnerId,
                OwnTypeId = entity.OwnTypeId,
                ShippingLineId = entity.ShippingLineId,
                VesselCode = entity.VesselCode,
                VesselName = entity.VesselName,
                VesselTypeId = entity.VesselTypeId,
                MMSI = entity.MMSI
            };
            ViewBag.CreateEdit = "Edit";

            return PartialView("_CreateEdit", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BaseInfo.Vessels.View")]
        public async Task<IActionResult> Edit(VesselCreateEditDto dto)
        {
            //if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode, dto.Id))
            //{
            //    ModelState.AddModelError("Code", "This code already exists.");
            //}

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Edit";
                return PartialView("_CreateEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Edit";
                return PartialView("_CreateEdit", dto);
            }
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });

            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Vessels.Edit")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            //var item = await _service.GetByIdAsync(id);
            // if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", id);
        }


        [Authorize(Policy = "BaseInfo.Vessels.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {

                HxTrigger(new
                {
                    gridChanged = false,
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
    }
}
