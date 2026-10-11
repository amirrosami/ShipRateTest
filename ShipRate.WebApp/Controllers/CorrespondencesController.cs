using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CorrespondencesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        public CorrespondencesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        [Authorize("Utilities.Correspondences.View")]
        public async Task<IActionResult> Index()
        {
            await LoadDropdowns();
            return View();
        }

        [Authorize("Utilities.Correspondences.View")]
        public async Task<IActionResult> List([FromQuery] CorrespondenceFilterDto filter)
        {
            var query = _context.Correspondences
                .Include(c => c.Company)
                .Include(c => c.InOut)
                .Include(c => c.SenderReceiver)
                .Include(c => c.Subject)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            if (filter.SubjectId.HasValue)
                query = query.Where(c => c.SubjectId == filter.SubjectId);

            if (!string.IsNullOrEmpty(filter.CorrespondenceNo))
                query = query.Where(c => c.CorrespondenceNo.Contains(filter.CorrespondenceNo));

            if (filter.DateFrom.HasValue)
                query = query.Where(c => c.CorrespondenceDate >= filter.DateFrom);

            if (filter.DateTo.HasValue)
                query = query.Where(c => c.CorrespondenceDate <= filter.DateTo);

            if (!string.IsNullOrEmpty(filter.Keywords))
                query = query.Where(c => c.Keywords.Contains(filter.Keywords) ||
                                        c.SubjectFreeText.Contains(filter.Keywords));

            var items = await query
                .OrderByDescending(c => c.CorrespondenceDate)
                .Select(c => new CorrespondenceDto
                {
                    Id = c.Id,
                    CompanyId = c.CompanyId,
                    CompanyName = "",
                    InOutId = c.InOutId,
                    InOutName = c.InOut!.InOutName,
                    SenderReceiverId = c.SenderReceiverId,
                    SenderReceiverName = c.SenderReceiver != null ? c.SenderReceiver.PersonName : null,
                    SenderReceiverTitle = c.SenderReceiverTitle,
                    CorrespondenceNo = c.CorrespondenceNo,
                    CorrespondenceDate = c.CorrespondenceDate,
                    SubjectId = c.SubjectId,
                    SubjectName = c.Subject != null ? c.Subject.SubjectTitle : null,
                    SubjectFreeText = c.SubjectFreeText,
                    Keywords = c.Keywords,
                })
                .ToListAsync();

            return PartialView("_List", items);
        }

        [Authorize("Utilities.Correspondences.Edit")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return PartialView("_Create", new CorrespondenceCreateDto());
        }

        [Authorize("Utilities.Correspondences.Edit")]
        [HttpPost]
        public async Task<IActionResult> Create(CorrespondenceCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return PartialView("_Create", dto);
            }

            var userId = _currentUserService.UserId;
            var entity = new Correspondence
            {
                Id = Guid.CreateVersion7(),
                CompanyId = dto.CompanyId,
                InOutId = dto.InOutId,
                SenderReceiverId = dto.SenderReceiverId,
                SenderReceiverTitle = dto.SenderReceiverTitle,
                CorrespondenceNo = dto.CorrespondenceNo,
                CorrespondenceDate = dto.CorrespondenceDate,
                SubjectId = dto.SubjectId,
                SubjectFreeText = dto.SubjectFreeText,
                SystemObjectTypeId = dto.SystemObjectTypeId,
                ObjectId = dto.ObjectId,
                Keywords = dto.Keywords,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            _context.Correspondences.Add(entity);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "مکاتبه با موفقیت ثبت شد";
            ViewBag.CreatedId = entity.Id;
            await LoadDropdowns();

            return PartialView("_Create", new CorrespondenceCreateDto());
        }

        [Authorize("Utilities.Correspondences.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _context.Correspondences.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new CorrespondenceCreateDto
            {
                CompanyId = entity.CompanyId,
                InOutId = entity.InOutId,
                SenderReceiverId = entity.SenderReceiverId,
                SenderReceiverTitle = entity.SenderReceiverTitle,
                CorrespondenceNo = entity.CorrespondenceNo,
                CorrespondenceDate = entity.CorrespondenceDate,
                SubjectId = entity.SubjectId,
                SubjectFreeText = entity.SubjectFreeText,
                SystemObjectTypeId = entity.SystemObjectTypeId,
                ObjectId = entity.ObjectId,
                Keywords = entity.Keywords
            };

            await LoadDropdowns();
            ViewBag.EditId = id;
            return PartialView("_Edit", dto);
        }

        [Authorize("Utilities.Correspondences.Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, CorrespondenceCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                ViewBag.EditId = id;
                return PartialView("_Edit", dto);
            }

            var entity = await _context.Correspondences.FindAsync(id);
            if (entity == null) return NotFound();

            entity.CompanyId = dto.CompanyId;
            entity.InOutId = dto.InOutId;
            entity.SenderReceiverId = dto.SenderReceiverId;
            entity.SenderReceiverTitle = dto.SenderReceiverTitle;
            entity.CorrespondenceNo = dto.CorrespondenceNo;
            entity.CorrespondenceDate = dto.CorrespondenceDate;
            entity.SubjectId = dto.SubjectId;
            entity.SubjectFreeText = dto.SubjectFreeText;
            entity.SystemObjectTypeId = dto.SystemObjectTypeId;
            entity.ObjectId = dto.ObjectId;
            entity.Keywords = dto.Keywords;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _context.SaveChangesAsync();

            Response.Headers.Add("HX-Trigger", "correspondenceUpdated");
            return Content("ویرایش با موفقیت انجام شد");
        }

        [Authorize("Utilities.Correspondences.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _context.Correspondences
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null) return NotFound();

            return PartialView("_Delete", entity);
        }

        [Authorize("Utilities.Correspondences.Edit")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await _context.Correspondences.FindAsync(id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _context.SaveChangesAsync();

            Response.Headers.Add("HX-Trigger", "correspondenceDeleted");
            return Content("حذف با موفقیت انجام شد");
        }

        private async Task LoadDropdowns()
        {
            ViewBag.Companies = await _context.Companies
                .Include(x => x.Person)
                .Where(c => c.IsActive)
                .Select(c => new { c.Id, c.Person.PersonName })
                .ToListAsync();

            ViewBag.InOuts = await _context.InOut
                .Select(i => new { i.Id, i.InOutName })
                .ToListAsync();

            ViewBag.Subjects = await _context.CorrespondenceSubjects
                .Where(s => s.IsActive)
                .Select(s => new { s.Id, s.SubjectTitle })
                .ToListAsync();

            ViewBag.Persons = await _context.Persons
                .Select(p => new { p.Id, p.PersonName })
                .ToListAsync();
        }
    }
}
