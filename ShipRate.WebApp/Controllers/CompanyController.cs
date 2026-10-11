using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.CompaniesViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CompanyController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var companies = await _db.Companies
                .Include(x => x.Person)
                .Where(c => !c.IsDeleted)
                .Include(c => c.Person)
                .Select(c => new CompanyListViewModel
                {
                    Id = c.Id,
                    PersonName = c.Person.PersonName, // فرض بر وجود فیلد Name در Persons
                    AccountingAddress = c.AccountingAddress,
                    IsActive = c.IsActive,
                    ParentId = c.ParentId,
                    CompanyAbb = c.CompanyAbb
                })
                .ToListAsync();

            return View(companies);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Settings(int id)
        {
            var company = await _db.Companies
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (company == null)
                return NotFound();

            var settings = await _db.SystemSettings
                .Where(s => s.CompanyId == id)
                .Select(s => new SettingItemViewModel
                {
                    Id = s.Id,
                    SettingKey = s.SettingKey,
                    SettingValue = s.SettingValue,
                    Description = s.Description
                })
                .ToListAsync();

            var model = new CompanySettingsViewModel
            {
                CompanyId = company.Id,
                CompanyName = company.Person.PersonName,
                Settings = settings
            };

            return View(model);
        }

        // POST: Companies/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Settings(CompanySettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingSettings = _db.SystemSettings
                .Where(s => s.CompanyId == model.CompanyId);
            _db.SystemSettings.RemoveRange(existingSettings);

            foreach (var item in model.Settings.Where(s => !string.IsNullOrWhiteSpace(s.SettingKey)))
            {
                _db.SystemSettings.Add(new SystemSetting
                {
                    CompanyId = model.CompanyId,
                    SettingKey = item.SettingKey.Trim(),
                    SettingValue = item.SettingValue?.Trim(),
                    Description = item.Description?.Trim()
                });
            }

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Settings saved successfully.";
            return RedirectToAction(nameof(Settings), new { id = model.CompanyId });
        }





        // GET: Companies/Numbering/5

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Numbering(int id)
        {
            var company = await _db.Companies
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (company == null)
                return NotFound();

            var numbering = await _db.NumberingSystems
                .FirstOrDefaultAsync(n => n.CompanyId == id && !n.IsDeleted);

            var model = new NumberingSystemViewModel
            {
                CompanyId = company.Id,
                CompanyName = company.Person.PersonName,
                IsActive = true,
                CurrentYear = DateTime.Now.Year,
                PadLength = 4
            };

            if (numbering != null)
            {
                model.Id = numbering.Id;
                model.EntityType = numbering.EntityType;
                model.Prefix = numbering.Prefix;
                model.Suffix = numbering.Suffix;
                model.PadLength = numbering.PadLedngth;
                model.ResetYearly = numbering.ResetYearly;
                model.CurrentSeq = numbering.CurrentSeq;
                model.CurrentYear = numbering.CurrentYear;
                model.IsActive = numbering.IsActive;
            }

            return View(model);
        }

        // POST: Companies/Numbering
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Numbering(NumberingSystemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _db.NumberingSystems
                .FirstOrDefaultAsync(n => n.CompanyId == model.CompanyId && !n.IsDeleted);

            if (existing != null)
            {
                // Update
                existing.EntityType = model.EntityType.Trim();
                existing.Prefix = model.Prefix.Trim();
                existing.Suffix = model.Suffix.Trim();
                existing.PadLedngth = model.PadLength;
                existing.ResetYearly = model.ResetYearly;
                existing.CurrentSeq = model.CurrentSeq;
                existing.CurrentYear = model.CurrentYear;
                existing.IsActive = model.IsActive;
            }
            else
            {
                // Create
                _db.NumberingSystems.Add(new NumberingSystem
                {
                    CompanyId = model.CompanyId,
                    EntityType = model.EntityType.Trim(),
                    Prefix = model.Prefix.Trim(),
                    Suffix = model.Suffix.Trim(),
                    PadLedngth = model.PadLength,
                    ResetYearly = model.ResetYearly,
                    CurrentSeq = model.CurrentSeq,
                    CurrentYear = model.CurrentYear,
                    IsActive = model.IsActive,
                    IsDeleted = false
                });
            }

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Numbering settings saved successfully.";
            return RedirectToAction(nameof(Numbering), new { id = model.CompanyId });
        }






        // GET: Company/NumberingList/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NumberingList(int id)
        {
            var company = await _db.Companies
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (company == null)
                return NotFound();

            var numberings = await _db.NumberingSystems
                .Where(n => n.CompanyId == id && !n.IsDeleted)
                .Select(n => new NumberingSystemItemViewModel
                {
                    Id = n.Id,
                    CompanyId = n.CompanyId,
                    CompanyName = company.Person.PersonName,
                    EntityType = n.EntityType,
                    Prefix = n.Prefix,
                    Suffix = n.Suffix,
                    PadLength = n.PadLedngth,
                    ResetYearly = n.ResetYearly,
                    CurrentSeq = n.CurrentSeq,
                    CurrentYear = n.CurrentYear,
                    IsActive = n.IsActive
                })
                .ToListAsync();

            var model = new CompanyNumberingListViewModel
            {
                CompanyId = company.Id,
                CompanyName = company.Person.PersonName,
                Items = numberings
            };

            return View(model);
        }

        // GET: Company/EditNumbering/10
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditNumbering(int id)
        {
            var numbering = await _db.NumberingSystems
                .Include(n => n.Company)
                .ThenInclude(c => c.Person)
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

            if (numbering == null)
                return NotFound();

            var model = new NumberingSystemItemViewModel
            {
                Id = numbering.Id,
                CompanyId = numbering.CompanyId,
                CompanyName = numbering.Company?.Person?.PersonName,
                EntityType = numbering.EntityType,
                Prefix = numbering.Prefix,
                Suffix = numbering.Suffix,
                PadLength = numbering.PadLedngth,
                ResetYearly = numbering.ResetYearly,
                CurrentSeq = numbering.CurrentSeq,
                CurrentYear = numbering.CurrentYear,
                IsActive = numbering.IsActive
            };

            return View(model);
        }

        // POST: Company/EditNumbering
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditNumbering(NumberingSystemItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _db.NumberingSystems
                .FirstOrDefaultAsync(n => n.Id == model.Id && !n.IsDeleted);

            if (existing == null)
                return NotFound();

            // EntityType ویرایش نمی‌شود
            existing.Prefix = model.Prefix?.Trim() ?? string.Empty;
            existing.Suffix = model.Suffix?.Trim() ?? string.Empty;
            existing.PadLedngth = model.PadLength;
            existing.ResetYearly = model.ResetYearly;
            existing.CurrentSeq = model.CurrentSeq;
            existing.CurrentYear = model.CurrentYear;
            existing.IsActive = model.IsActive;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Numbering settings for '{existing.EntityType}' updated successfully.";
            return RedirectToAction(nameof(NumberingList), new { id = existing.CompanyId });
        }
    }
}
