using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.TerritoryVms;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TerritoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService currentUser;
        public TerritoryController(ApplicationDbContext db, ICurrentUserService userService)
        {
            _db = db;
            this.currentUser = userService;
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        // ══════════════════════════════════════
        //  INDEX — صفحه اصلی
        // ══════════════════════════════════════
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }


        private async Task<PagedResult<TerritoryListViewModel>> getFilteredList(PagedRequestDto dto)
        {
            var data = _db.Territories
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.TerritoryName.Contains(dto.SearchTerm) || x.Remarks.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new TerritoryListViewModel
            {
                Id = x.Id,
                TerritoryName = x.TerritoryName,
                Remarks = x.Remarks,
                CityCount = _db.TerritoryCities.Where(c => c.TerritoryId == x.Id).Count(),
                //IsActive = x.IsActive
            }).ToListAsync();

            return new PagedResult<TerritoryListViewModel>()
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


        // ══════════════════════════════════════
        //  TERRITORY GRID — گرید مناطق
        // ══════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> TerritoryGrid(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Territory/TerritoryGrid",
                GridPartialName = "_TerritoryGrid",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>()
            {
                new GridColumn("TerritoryName","Territory Name") ,new GridColumn("Remarks","Remarks"),
                new GridColumn("CityCount" ,"City Count")
            };
        }

        // ══════════════════════════════════════
        //  CITIES GRID — گرید شهرهای منطقه
        // ══════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> CitiesGrid(Guid territoryId, int cityPage = 1, int cityPageSize = 10)
        {
            if (territoryId == Guid.Empty)
            {
                return Content("");
            }
            var territory = _db.Territories.Where(x => x.Id == territoryId).FirstOrDefault();
            if (territory == null)
            {
                return Content("");
            }
            var query = _db.TerritoryCities
                .Where(tc => tc.TerritoryId == territoryId && !tc.IsDeleted)
                .Select(tc => new TerritoryCityRowViewModel
                {
                    Id = tc.Id,
                    CityId = tc.CityId,
                    LocationName = tc.City.LocationName,
                    //IsActive = tc.IsActive
                });

            var result = new DetailGridViewModel
            {
                Controller = "Territory",
                MasterTitle = "Cities",
                MasterDisplayValue = $"Territory Name : {territory.TerritoryName}",
                MasterId = territoryId,
                Columns = new List<GridColumn> {
                    new GridColumn { DisplayName = "Location Name", Name = "LocationName" }},
                Data = await query.Select(x => (object)x).ToListAsync(),
                ListUrl = "Territory/CitiesGrid",
                DetailGridPartialName = "_CitiesGrid",
                CreateAction = "AddCity"
            };
            return PartialView("_DetailGrid", result);
        }

        // ══════════════════════════════════════
        //  CREATE TERRITORY — فرم + ذخیره
        // ══════════════════════════════════════
        [HttpGet]
        public IActionResult CreateTerritory()
        {
            ViewBag.CreateEdit = "Create";
            return PartialView("_TerritoryForm", new TerritoryEditViewModel() {  });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTerritory(TerritoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CreateEdit = "Create";
                return PartialView("_TerritoryForm", model);
            }
            var entity = new Territory
            {
                Id = Guid.CreateVersion7(),
                CompanyId = 1,
                TerritoryName = model.TerritoryName,
                Remarks = model.Remarks ?? "",
                //IsActive = model.IsActive,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUser.UserId // TODO: از User بگیرید
            };

            _db.Territories.Add(entity);
            await _db.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Content("");
        }

        // ══════════════════════════════════════
        //  EDIT TERRITORY — فرم + ذخیره
        // ══════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> EditTerritory(Guid id)
        {
            var entity = await _db.Territories
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (entity == null) return NotFound();

            var model = new TerritoryEditViewModel
            {
                Id = entity.Id,
                TerritoryName = entity.TerritoryName,
                Remarks = entity.Remarks,
                //IsActive = entity.IsActive
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_TerritoryForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTerritory(TerritoryEditViewModel model)
        {
            ModelState.Clear();


            var entity = await _db.Territories
                .FirstOrDefaultAsync(t => t.Id == model.Id && !t.IsDeleted);

            if (entity == null) return NotFound();

            entity.TerritoryName = model.TerritoryName;
            entity.Remarks = model.Remarks ?? "";
            //entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUser.UserId;

            var validation = new TerritoryValidation().Validate(entity);
            if (!validation.IsValid)
            {
                foreach (var err in validation.Errors)
                {
                    ModelState.AddModelError("", err.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.CreateEdit = "Edit";
                return PartialView("_TerritoryForm", model);
            }

            await _db.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }

        // ══════════════════════════════════════
        //  DELETE TERRITORY — Soft Delete
        // ══════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> DeleteTerritory(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTerritoryConfirmed(Guid id)
        {
            var entity = await _db.Territories
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;

            // شهرهای مرتبط هم soft delete
            var cities = await _db.TerritoryCities
                .Where(tc => tc.TerritoryId == id && !tc.IsDeleted)
                .ToListAsync();
            foreach (var c in cities)
            {
                c.IsDeleted = true;
                c.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Deleted Sussefully"
            });
            return Content("");
        }

        // ══════════════════════════════════════
        //  ADD CITY — فرم + ذخیره
        // ══════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> AddCity(Guid territoryId)
        {
            var territory = await _db.Territories.Where(x => x.Id == territoryId).FirstOrDefaultAsync();
            ViewBag.TerritoryId = territoryId;
            ViewBag.TerritoryName = territory.TerritoryName;
            ViewBag.CreateEdit = "Create";
            return PartialView("_AddCityForm", new TerritoryCityCreateEditDto { TerritoryId = territoryId });
        }

        [HttpGet]
        public IActionResult EditCity(Guid id)
        {
            var city = _db.TerritoryCities.Where(x => x.Id == id).Include(x => x.Territory).Include(x => x.City).FirstOrDefault();
            if (city == null) return NotFound();
            ViewBag.TerritoryId = city.TerritoryId;
            ViewBag.TerritoryName = city.Territory.TerritoryName;
            ViewBag.CreateEdit = "Edit";
            var cityDto = new TerritoryCityCreateEditDto
            {
                Id = city.Id,
                CityId = city.CityId,
                CityName = city.City.LocationName,
                TerritoryId = city.TerritoryId
            };
            return PartialView("_AddCityForm", cityDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(TerritoryCityCreateEditDto dto)
        {
            ModelState.Clear();
            if (dto.TerritoryId == Guid.Empty)
            {
                ModelState.AddModelError("error", "an error occured!!\n please contact support team");
            }


            var entity = new TerritoryCity
            {
                Id = Guid.CreateVersion7(),
                TerritoryId = dto.TerritoryId,
                CityId = dto.CityId,
                //IsActive = true,
                CreatedAt = DateTime.Now,
                CreateUserId = Guid.Empty // TODO
            };
            var validator = new TerritoryCityValidation().Validate(entity);
            if (!validator.IsValid)
            {
                foreach (var err in validator.Errors)
                {
                    ModelState.AddModelError("", err.ErrorMessage);
                }
            }
            // بررسی تکراری نبودن
            var exists = await _db.TerritoryCities
                .AnyAsync(tc => tc.TerritoryId == dto.TerritoryId
                             && tc.CityId == dto.CityId
                             && !tc.IsDeleted);

            if (exists)
            {
                ModelState.AddModelError("", "This city is already added to this territory");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TerritoryId = dto.TerritoryId;
                ViewBag.CityId = dto.CityId;
                ViewBag.TerritoryName = "";
                ViewBag.CreateEdit = "Create";
                return PartialView("_AddCityForm", dto);
            }



            _db.TerritoryCities.Add(entity);
            await _db.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Added Sussefully"
            });
            return Content("");
        }

        // ══════════════════════════════════════
        //  DELETE CITY — Soft Delete
        // ══════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            return PartialView("_DeleteCityConfirm", id);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCityConfirmed(Guid id)
        {

            var entity = await _db.TerritoryCities
                .FirstOrDefaultAsync(tc => tc.Id == id && !tc.IsDeleted);

            var success = false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUser.UserId;
            try
            {
                await _db.SaveChangesAsync();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            if (!success)
            {
                HxTrigger(new
                {
                    gridChanged = false,
                    toastError = "An error occured during delete."
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Deleted Sussefully"
            });

            return Content("");
        }

        // ══════════════════════════════════════
        //  SEARCH LOCATIONS — جستجوی شهر (API)
        // ══════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> SearchLocations(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(new List<LocationSearchResult>());

            var results = await _db.Locations
                .Where(l => l.LocationTypeId == 2
                        && l.LocationName.Contains(term))
                .OrderBy(l => l.LocationName)
                .Take(15)
                .Select(l => new LocationSearchResult
                {
                    Id = l.Id,
                    LocationName = l.LocationName
                })
                .ToListAsync();

            return Json(results);
        }
    }
}
