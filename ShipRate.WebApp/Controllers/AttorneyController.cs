// Controllers/AttorneyController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.CustomersVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class AttorneyController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const int PageSize = 10;
        private const int LawyerPageSize = 5;

        public AttorneyController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize("CRM.Attornies.View")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("CRM.Attornies.View")]
        public async Task<IActionResult> AttorneyList(AttorneySearchViewModel search)
        {
            var query = _db.Attorneys
                .Include(a => a.Client)
                .ThenInclude(a => a.Person)
                .Include(a => a.Company)
                    .ThenInclude(c => c.Person)
                .Where(a => !a.IsDeleted);

            // Search filters
            if (!string.IsNullOrWhiteSpace(search.ClientName))
                query = query.Where(a => a.Client.Person.PersonName.Contains(search.ClientName));

            if (!string.IsNullOrWhiteSpace(search.DocNo))
                query = query.Where(a => a.DocNo.Contains(search.DocNo));

            if (!string.IsNullOrWhiteSpace(search.LawyerName))
            {
                var lawyerAttorneyIds = await _db.AttorneyLawyers
                    .Where(al => !al.IsDeleted && al.Lawyer.PersonName.Contains(search.LawyerName))
                    .Select(al => al.AttorneyId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(a => lawyerAttorneyIds.Contains(a.Id));
            }

            var totalCount = await query.CountAsync();
            var page = search.Page < 1 ? 1 : search.Page;
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var list = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(a => new AttorneyIndexViewModel
                {
                    Id = a.Id,
                    DocNo = a.DocNo,
                    DocDate = a.DocDate,
                    ClientName = a.Client.Person.PersonName,
                    CompanyName = a.Company.Person.PersonName,
                    HasExpiryDate = a.HasExpiryDate,
                    ExpiryDate = a.ExpiryDate,
                    IsActive = a.IsActive,
                    LawyerCount = a.AttorneyLawyers.Count(l => !l.IsDeleted)
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return PartialView("_AttorneyList", list);
        }

        [Authorize("CRM.Attornies.Edit")]
        [HttpGet]
        public IActionResult CreateAttorney()
        {
            var model = new AttorneyEditViewModel
            {
                DocDate = DateTime.Now,
                IsActive = true
            };
            return PartialView("_AttorneyForm", model);
        }

        [Authorize("CRM.Attornies.Edit")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAttorney(AttorneyEditViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AttorneyForm", model);


            var customer = await _db.Customers
                .FirstOrDefaultAsync(a => a.Id == model.ClientId && !a.IsDeleted);
            if (customer == null) return NotFound();
            var entity = new Attorney
            {
                Id = Guid.CreateVersion7(),
                CompanyId = 1,
                ClientId = model.ClientId,
                DocNo = model.DocNo,
                DocDate = model.DocDate,
                HasExpiryDate = model.HasExpiryDate,
                ExpiryDate = model.HasExpiryDate ? model.ExpiryDate : null,
                Remarks = model.Remarks ?? "",
                IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = Guid.Empty
            };

            _db.Attorneys.Add(entity);
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "attorneyListChanged";
            Response.Headers["HX-Reswap"] = "none";
            Response.Headers["HX-Retarget"] = "this";

            return Content("");
        }

        [Authorize("CRM.Attornies.Edit")]
        [HttpGet]
        public async Task<IActionResult> EditAttorney(Guid id)
        {
            var entity = await _db.Attorneys
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (entity == null) return NotFound();

            var model = new AttorneyEditViewModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                ClientName = entity.Client.Person.PersonName,
                DocNo = entity.DocNo,
                DocDate = entity.DocDate,
                HasExpiryDate = entity.HasExpiryDate,
                ExpiryDate = entity.ExpiryDate,
                Remarks = entity.Remarks,
                IsActive = entity.IsActive
            };

            return PartialView("_AttorneyForm", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize("CRM.Attornies.Edit")]
        public async Task<IActionResult> EditAttorney(AttorneyEditViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AttorneyForm", model);

            var entity = await _db.Attorneys
                .FirstOrDefaultAsync(a => a.Id == model.Id && !a.IsDeleted);

            if (entity == null) return NotFound();

            entity.ClientId = model.ClientId;
            entity.DocNo = model.DocNo;
            entity.DocDate = model.DocDate;
            entity.HasExpiryDate = model.HasExpiryDate;
            entity.ExpiryDate = model.HasExpiryDate ? model.ExpiryDate : null;
            entity.Remarks = model.Remarks ?? "";
            entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = Guid.Empty;

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "attorneyListChanged";
            Response.Headers["HX-Reswap"] = "none";
            Response.Headers["HX-Retarget"] = "this";
            return Content("");
        }
        [HttpDelete]
        [Authorize("CRM.Attornies.Edit")]
        public async Task<IActionResult> DeleteAttorney(Guid id)
        {
            var entity = await _db.Attorneys
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;

            // Soft delete lawyers too
            var lawyers = await _db.AttorneyLawyers
                .Where(al => al.AttorneyId == id && !al.IsDeleted)
                .ToListAsync();

            foreach (var l in lawyers)
            {
                l.IsDeleted = true;
                l.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "attorneyListChanged";
            Response.Headers["HX-Reswap"] = "none";
            Response.Headers["HX-Retarget"] = "this";
            return Content("");
        }

        [Authorize("CRM.Attornies.View")]
        public async Task<IActionResult> LawyerList(Guid attorneyId, int page = 1)
        {
            var attorney = await _db.Attorneys
                .Include(a => a.Client)
                .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(a => a.Id == attorneyId && !a.IsDeleted);

            if (attorney == null) return NotFound();

            var query = _db.AttorneyLawyers
                .Include(al => al.Lawyer)
                .Where(al => al.AttorneyId == attorneyId && !al.IsDeleted);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)LawyerPageSize);
            page = page < 1 ? 1 : page;

            var list = await query
                .OrderBy(al => al.Lawyer.PersonName)
                .Skip((page - 1) * LawyerPageSize)
                .Take(LawyerPageSize)
                .Select(al => new AttorneyLawyerViewModel
                {
                    Id = al.Id,
                    AttorneyId = al.AttorneyId,
                    LawyerId = al.LawyerId,
                    LawyerName = al.Lawyer.PersonName,
                })
                .ToListAsync();

            ViewBag.AttorneyId = attorneyId;
            ViewBag.AttorneyDocNo = attorney.DocNo;
            ViewBag.ClientName = attorney.Client.Person.PersonName;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return PartialView("_LawyerList", list);
        }

        [Authorize("CRM.Attornies.Edit")]
        [HttpGet]
        public async Task<IActionResult> AddLawyer(Guid attorneyId)
        {
            var attorney = await _db.Attorneys
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.Id == attorneyId && !a.IsDeleted);

            if (attorney == null) return NotFound();

            ViewBag.AttorneyId = attorneyId;
            ViewBag.AttorneyDocNo = attorney.DocNo;

            return PartialView("_AddLawyerForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.Attornies.Edit")]
        public async Task<IActionResult> AddLawyer(Guid attorneyId, Guid lawyerId)
        {
            if (attorneyId == Guid.Empty || lawyerId == Guid.Empty)
            {
                return BadRequest("Invalid data.");
            }

            // Check duplicate
            var exists = await _db.AttorneyLawyers
                .AnyAsync(al => al.AttorneyId == attorneyId && al.LawyerId == lawyerId);

            if (exists)
            {
                return BadRequest("This lawyer is already assigned.");
            }

            var entity = new AttorneyLawyer
            {
                Id = Guid.CreateVersion7(),
                AttorneyId = attorneyId,
                LawyerId = lawyerId,
                IsDeleted = false
            };

            _db.AttorneyLawyers.Add(entity);
            await _db.SaveChangesAsync();

            // ✅ هدرهای مهم برای بستن مودال و رفرش لیست
            Response.Headers["HX-Trigger"] = "lawyerListChanged";
            Response.Headers["HX-Reswap"] = "none";
            Response.Headers["HX-Retarget"] = "this";

            return Content("");
        }
        [Authorize("CRM.Attornies.Edit")]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLawyer(Guid id)
        {
            var entity = await _db.AttorneyLawyers.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "lawyerListChanged";
            Response.Headers["HX-Reswap"] = "none";
            Response.Headers["HX-Retarget"] = "this";

            return Content("");
        }

        [HttpGet]
        [Authorize("CRM.Attornies.View")]
        public async Task<IActionResult> SearchPersons(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(Array.Empty<object>());

            var results = await _db.Persons
                .Where(p => p.PersonName.Contains(term))
                .Take(15)
                .Select(p => new { p.Id, p.PersonName })
                .ToListAsync();
            return Json(results);
        }
    }
}
