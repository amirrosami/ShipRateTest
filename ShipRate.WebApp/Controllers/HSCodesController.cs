using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Validations.Domain;
using ShipRate.Common.Exceptions;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class HSCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 20;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJobService _jobService;

        public HSCodesController(ApplicationDbContext context, ICurrentUserService currentUserService, IJobService jobService)
        {
            this._currentUserService = currentUserService;
            _context = context;
            _jobService = jobService;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        // GET: HSCodes
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto { });
            return View(result);
        }


        private async Task<PagedResult<HSCodeListItem>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.HSCodes
                .Where(x => !x.IsDeleted)
                .Include(x => x.Parent)
                .Include(x => x.HSLevel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.HSCodeCode.Contains(dto.SearchTerm) || x.HSCodeName.Contains(dto.SearchTerm) || x.Parent.HSCodeCode.Contains(dto.SearchTerm) || x.HSLevel.HSLevelName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new HSCodeListItem
            {
                Id = x.Id,
                HSCodeCode = x.HSCodeCode,
                HSCodeName = x.HSCodeName,
                HSLevelName = x.HSLevel.HSLevelName,
                ParentCode = x.Parent.HSCodeCode,
                //IsActive = x.IsActive
            }).OrderBy(x => x.HSCodeCode).ToListAsync();

            return new PagedResult<HSCodeListItem>()
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

        // GET: HSCodes/LoadGrid
        public async Task<IActionResult> LoadGrid(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "HSCodes/LoadGrid",
                GridPartialName = "_HSCodeGrid",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>()
            {
                new GridColumn("HSCodeCode","Code") ,new GridColumn("HSCodeName","HSCode Name"),
                new GridColumn("HSLevelName","Level"), new GridColumn("ParentCode","Parent HSCode")
            };
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new HSCodeFormViewModel
            {
                IsActive = true,
                HSLevels = await GetHSLevels(),
                ParentHSCodes = await GetParentHSCodes()
            };
            ViewBag.CreateEdit = "Create";
            return PartialView("_HSCodeForm", viewModel);
        }

        // POST: HSCodes/Create
        [HttpPost]
        public async Task<IActionResult> Create(HSCodeFormViewModel model)
        {

            ModelState.Clear();

            var hsCode = new HSCode
            {
                Id = Guid.CreateVersion7(),
                HSCodeCode = model.HSCodeCode,
                HSCodeName = model.HSCodeName,
                HSLevelId = model.HSLevelId,
                ParentId = model.ParentId,
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };
            var validation = new HSCodeValidation().Validate(hsCode);
            if (!validation.IsValid)
            {
                foreach (var err in validation.Errors)
                {
                    ModelState.AddModelError("", err.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                model.HSLevels = await GetHSLevels();
                model.ParentHSCodes = await GetParentHSCodes();
                ViewBag.CreateEdit = "Create";
                return PartialView("_HSCodeForm", model);
            }

            _context.HSCodes.Add(hsCode);
            await _context.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var hsCode = await _context.HSCodes.FindAsync(id);
            if (hsCode == null) return NotFound();

            var viewModel = new HSCodeFormViewModel
            {
                Id = hsCode.Id,
                HSCodeCode = hsCode.HSCodeCode,
                HSCodeName = hsCode.HSCodeName,
                HSLevelId = hsCode.HSLevelId,
                ParentId = hsCode.ParentId == Guid.Empty ? null : hsCode.ParentId,
                //IsActive = hsCode.IsActive,
                HSLevels = await GetHSLevels(),
                ParentHSCodes = await GetParentHSCodes()
            };

            return PartialView("_HSCodeForm", viewModel);
        }

        // POST: HSCodes/Edit/id
        [HttpPost]
        public async Task<IActionResult> Edit(HSCodeFormViewModel model)
        {

            ModelState.Clear();
            var hsCode = await _context.HSCodes.FindAsync(model.Id);
            if (hsCode == null) return NotFound();

            hsCode.HSCodeCode = model.HSCodeCode;
            hsCode.HSCodeName = model.HSCodeName;
            hsCode.HSLevelId = model.HSLevelId;
            hsCode.ParentId = model.ParentId ?? Guid.Empty;
            //hsCode.IsActive = model.IsActive;
            hsCode.UpdatedAt = DateTime.Now;
            hsCode.EditUserId = _currentUserService.UserId;


            var validation = new HSCodeValidation().Validate(hsCode);
            if (!validation.IsValid)
            {
                foreach (var err in validation.Errors)
                {
                    ModelState.AddModelError("", err.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                model.HSLevels = await GetHSLevels();
                model.ParentHSCodes = await GetParentHSCodes();
                return PartialView("_HSCodeForm", model);
            }
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteHSCodeConfirm", id);
        }

        // DELETE: HSCodes/Delete/id
        [HttpPost]
        public async Task<IActionResult> DeleteHSCodeConfirmed(Guid id)
        {
            var hsCode = await _context.HSCodes.FindAsync(id);
            if (hsCode == null) return NotFound();

            hsCode.IsDeleted = true;
            hsCode.UpdatedAt = DateTime.Now;
            hsCode.EditUserId = Guid.Empty; // Replace with actual user ID

            await _context.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Deleted Sussefully"
            });
            return Ok();
        }

        // GET: HSCodes/LoadCountryHSCodes/id
        public async Task<IActionResult> LoadCountryHSCodes(Guid hsCodeId)
        {
            var Hscode = _context.HSCodes.Where(x => x.Id == hsCodeId).FirstOrDefault();
            if (Hscode == null)
            {
                return Content("");
            }
            var countryHSCodes = _context.CountryHSCodes
                .Include(c => c.Country)
                .Where(c => c.HSCodeId == hsCodeId && !c.IsDeleted)
                .Select(c => new CountryHSCodeListItem
                {
                    Id = c.Id,
                    CountryName = c.Country.CountryIsoCode2,
                    CHSCode = c.CHSCode,
                    CHSName = c.CHSName,
                    LocalName = c.LocalName,
                    ValidFrom = c.CountryHSCodeValidFrom,
                    ValidTo = c.CountryHSCodeValidTo,
                    DutyRate = c.DutyRate,
                    CommercialBenefitRate = c.CommercialBenefitRate,
                    IsActive = c.IsActive
                }).OrderBy(x => x.CHSCode);


            var result = new DetailGridViewModel
            {
                Controller = "HSCodes",
                MasterTitle = "HSCode Countries",
                MasterDisplayValue = $"HSCode Name: {Hscode.HSCodeName}",
                MasterId = hsCodeId,
                Columns = new List<GridColumn> {
                    new GridColumn { DisplayName = "Country", Name = "CountryName" },
                new GridColumn("CHSCode","HS Code") ,new GridColumn("CHSName","HS Name"),new GridColumn("LocalName","Local Name") ,new GridColumn("ValidFrom","Valid From"),new GridColumn("ValidTo","Valid To") ,new GridColumn("DutyRate","DutyRate"),new GridColumn("CommercialBenefitRate","Commercial Benefit Rate")},

                Data = await countryHSCodes.Select(x => (object)x).ToListAsync(),
                ListUrl = "HScodes/LoadCountryHSCodes",
                DetailGridPartialName = "_CountryHSCodeGrid",
                CreateAction = "CreateCountryHSCode"
            };
            return PartialView("_DetailGrid", result);
        }

        public async Task<IActionResult> CreateCountryHSCode(Guid hsCodeId)
        {
            var hscode = _context.HSCodes.Where(x => x.Id == hsCodeId).FirstOrDefault();
            if (hscode == null)
            {
                throw new GeneralAppException("error occured");
            }
            var viewModel = new CountryHSCodeFormViewModel
            {
                HSCodeId = hsCodeId,
                IsActive = true,
                CountryHSCodeValidTo = DateTime.Now.AddYears(10),
                Countries = await GetCountries()
            };
            ViewBag.HSCodeId = hsCodeId;
            ViewBag.HSCodeName = hscode.HSCodeName;
            ViewBag.CreateEdit = "Create";
            return PartialView("_CountryHSCodeForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountryHSCode(CountryHSCodeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HSCodeId = model.HSCodeId;
                ViewBag.HSCodeName = model.HSCodeName;
                ViewBag.CreateEdit = "Create";
                model.Countries = await GetCountries();
                return PartialView("_CountryHSCodeForm", model);
            }

            var countryHSCode = new CountryHSCode
            {
                Id = Guid.CreateVersion7(),
                HSCodeId = model.HSCodeId,
                CountryId = model.CountryId,
                CHSCode = model.CHSCode,
                CHSName = model.CHSName,
                LocalName = model.LocalName,
                CountryHSCodeValidFrom = model.CountryHSCodeValidFrom,
                CountryHSCodeValidTo = model.CountryHSCodeValidTo,
                DutyRate = model.DutyRate,
                CommercialBenefitRate = model.CommercialBenefitRate,
                IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };

            _context.CountryHSCodes.Add(countryHSCode);
            await _context.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }

        // GET: HSCodes/EditCountryHSCode/id
        public async Task<IActionResult> EditCountryHSCode(Guid id)
        {
            var countryHSCode = await _context.CountryHSCodes.Include(x => x.HSCode)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (countryHSCode == null) return NotFound();

            var viewModel = new CountryHSCodeFormViewModel
            {
                Id = countryHSCode.Id,
                HSCodeId = countryHSCode.HSCodeId,
                CountryId = countryHSCode.CountryId,
                CHSCode = countryHSCode.CHSCode,
                CHSName = countryHSCode.CHSName,
                LocalName = countryHSCode.LocalName,
                CountryHSCodeValidFrom = countryHSCode.CountryHSCodeValidFrom,
                CountryHSCodeValidTo = countryHSCode.CountryHSCodeValidTo,
                DutyRate = countryHSCode.DutyRate,
                CommercialBenefitRate = countryHSCode.CommercialBenefitRate,
                IsActive = countryHSCode.IsActive,
                Countries = await GetCountries()
            };
            ViewBag.HSCodeId = countryHSCode.HSCodeId;
            ViewBag.HSCodeName = countryHSCode.HSCode.HSCodeName;
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CountryHSCodeForm", viewModel);
        }

        // POST: HSCodes/EditCountryHSCode/id
        [HttpPost]
        public async Task<IActionResult> EditCountryHSCode(CountryHSCodeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Countries = await GetCountries();
                ViewBag.HSCodeId = model.HSCodeId;
                ViewBag.HSCodeName = model.HSCodeName;
                ViewBag.CreateEdit = "Edit";
                return PartialView("_CountryHSCodeForm", model);
            }

            var countryHSCode = await _context.CountryHSCodes.FindAsync(model.Id);
            if (countryHSCode == null) return NotFound();

            countryHSCode.CountryId = model.CountryId;
            countryHSCode.CHSCode = model.CHSCode;
            countryHSCode.CHSName = model.CHSName;
            countryHSCode.LocalName = model.LocalName;
            countryHSCode.CountryHSCodeValidFrom = model.CountryHSCodeValidFrom;
            countryHSCode.CountryHSCodeValidTo = model.CountryHSCodeValidTo;
            countryHSCode.DutyRate = model.DutyRate;
            countryHSCode.CommercialBenefitRate = model.CommercialBenefitRate;
            countryHSCode.IsActive = model.IsActive;
            countryHSCode.UpdatedAt = DateTime.Now;
            countryHSCode.EditUserId = Guid.Empty;

            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> DeleteCountryHSCode(Guid id)
        {
            return PartialView("_DeleteHSCodeCountryConfirm", id);
        }
        // DELETE: HSCodes/DeleteCountryHSCode/id
        [HttpPost]
        public async Task<IActionResult> DeleteHsCodeCountryConfirmed(Guid id)
        {
            var countryHSCode = await _context.CountryHSCodes.FindAsync(id);
            if (countryHSCode == null) return NotFound();

            countryHSCode.IsDeleted = true;
            countryHSCode.UpdatedAt = DateTime.Now;
            countryHSCode.EditUserId = Guid.Empty;

            await _context.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                DetailChanged = true,
                toastSuccess = "Deleted Sussefully"
            });
            return Content("");
        }

        // Helper Methods
        private async Task<List<HSLevelOption>> GetHSLevels()
        {
            return await _context.HSLevels
                .OrderBy(h => h.HSLevelName)
                .Select(h => new HSLevelOption
                {
                    Id = h.Id,
                    Name = h.HSLevelName
                })
                .ToListAsync();
        }

        private async Task<List<HSCodeOption>> GetParentHSCodes()
        {
            return await _context.HSCodes
                .Where(h => !h.IsDeleted)
                .OrderBy(h => h.HSCodeCode)
                .Select(h => new HSCodeOption
                {
                    Id = h.Id,
                    Code = h.HSCodeCode,
                    Name = h.HSCodeName
                })
                .ToListAsync();
        }

        private async Task<List<CountryOption>> GetCountries()
        {
            return await _context.Countries
                .OrderBy(c => c.CountryName)
                .Select(c => new CountryOption
                {
                    Id = c.Id,
                    Name = c.CountryName
                })
                .ToListAsync();
        }


        [HttpGet]
        public async Task<IActionResult> SearchHSCodes(
            string? term,
            string targetTextInputId = "HSCode",
            CancellationToken cancellationToken = default)
        {
            var items = await _jobService.SearchHSCodesAsync(term);
            ViewBag.TargetTextInputId = targetTextInputId;
            return PartialView("_HSCodeSearchResults", items);
        }



    }
}
