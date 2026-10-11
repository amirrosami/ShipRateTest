using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class PostalDispatchesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public PostalDispatchesController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [Authorize("Utilities.PostalDispatch.View")]
        public async Task<IActionResult> Index()
        {
            await LoadLookups();
            return View(new PostalDispatchSearchViewModel());
        }

        [HttpGet]
        [Authorize("Utilities.PostalDispatch.View")]
        public async Task<IActionResult> DispatchGrid([FromQuery] PostalDispatchSearchViewModel search)
        {
            var query = _db.PostalDispatches
                .Include(d => d.Sender)
                .Include(d => d.Receiver)
                .Include(d => d.CourierCompany)
                .ThenInclude(d => d.Person)
                .Include(d => d.CourierMethod)
                .Include(d => d.PostalDispatchStatus)
                .Where(d => !d.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.TrackingNumber))
                query = query.Where(d => d.TrackingNumber.Contains(search.TrackingNumber));

            if (!string.IsNullOrWhiteSpace(search.SenderName))
                query = query.Where(d => d.Sender.PersonName.Contains(search.SenderName));

            if (!string.IsNullOrWhiteSpace(search.ReceiverName))
                query = query.Where(d => d.Receiver.PersonName.Contains(search.ReceiverName));

            if (search.StatusId.HasValue)
                query = query.Where(d => d.StatusId == search.StatusId.Value);

            if (search.FromDate.HasValue)
                query = query.Where(d => d.DispatchDate >= search.FromDate.Value);

            if (search.ToDate.HasValue)
                query = query.Where(d => d.DispatchDate <= search.ToDate.Value);

            // Search in items too
            if (!string.IsNullOrWhiteSpace(search.DocumentDescription))
                query = query.Where(d => d.Items.Any(i =>
                    !i.IsDeleted && i.DocumentDescription.Contains(search.DocumentDescription)));

            if (!string.IsNullOrWhiteSpace(search.ItemReference))
                query = query.Where(d => d.Items.Any(i =>
                    !i.IsDeleted && i.ItemReference.Contains(search.ItemReference)));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.DispatchDate)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(d => new DispatchGridRow
                {
                    Id = d.Id,
                    DispatchDate = d.DispatchDate,
                    SenderName = d.Sender.PersonName,
                    ReceiverName = d.Receiver.PersonName,
                    CourierCompanyName = d.CourierCompany != null ? d.CourierCompany.Person.PersonName : "-",
                    TrackingNumber = d.TrackingNumber,
                    CourierMethodName = d.CourierMethod.CourierMethodName,
                    StatusName = d.PostalDispatchStatus.StatusName,
                    StatusId = d.StatusId,
                    //IsActive = d.IsActive
                })
                .ToListAsync();

            var result = new PagedResult<DispatchGridRow>
            {
                Items = items,
                TotalCount = totalCount,
                Page = search.Page,
                PageSize = search.PageSize
            };

            return PartialView("_DispatchGrid", result);
        }

        [HttpGet]
        [Authorize("Utilities.PostalDispatch.View")]
        public async Task<IActionResult> ItemGrid(Guid dispatchId)
        {
            var items = await _db.PostalDispatchItems
                .Where(i => i.PostalDispatchId == dispatchId && !i.IsDeleted)
                .Select(i => new ItemGridRow
                {
                    Id = i.Id,
                    PostalDispatchId = i.PostalDispatchId,
                    DocumentDescription = i.DocumentDescription,
                    ItemReference = i.ItemReference,
                    IsOriginal = i.IsOriginal,
                    //IsActive = i.IsActive
                })
                .ToListAsync();

            return PartialView("_ItemGrid", items);
        }

        [HttpGet]
        [Authorize("Utilities.PostalDispatch.Edit")]
        public async Task<IActionResult> Create()
        {
            await LoadLookups();
            return PartialView("_CreateEditModal", new PostalDispatchViewModel());
        }

        [HttpGet]
        [Authorize("Utilities.PostalDispatch.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dispatch = await _db.PostalDispatches
                .Include(d => d.Items.Where(i => !i.IsDeleted))
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (dispatch == null) return NotFound();

            var vm = new PostalDispatchViewModel
            {
                Id = dispatch.Id,
                DispatchDate = dispatch.DispatchDate,
                SenderId = dispatch.SenderId,
                ReceiverId = dispatch.ReceiverId,
                CourierCompanyId = dispatch.CourierCompanyId,
                TrackingNumber = dispatch.TrackingNumber,
                CourierMethodId = dispatch.CourierMethodId,
                StatusId = dispatch.StatusId,
                Items = dispatch.Items.Select(i => new PostalDispatchItemViewModel
                {
                    Id = i.Id,
                    ObjectId = i.ObjectId,
                    IsOriginal = i.IsOriginal,
                    DocumentDescription = i.DocumentDescription,
                    ItemReference = i.ItemReference
                }).ToList()
            };

            await LoadLookups();
            return PartialView("_CreateEditModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Utilities.PostalDispatch.Edit")]
        public async Task<IActionResult> Save(PostalDispatchViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookups();
                Response.Headers["HX-Retarget"] = "#modal-container";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateEditModal", vm);
            }

            var userId = _currentUserService.UserId;
            var now = DateTime.Now;

            if (vm.Id == null || vm.Id == Guid.Empty)
            {
                // Create
                var dispatch = new PostalDispatch
                {
                    Id = Guid.CreateVersion7(),
                    DispatchDate = vm.DispatchDate,
                    SenderId = vm.SenderId,
                    ReceiverId = vm.ReceiverId,
                    CourierCompanyId = vm.CourierCompanyId,
                    TrackingNumber = vm.TrackingNumber,
                    CourierMethodId = vm.CourierMethodId,
                    StatusId = vm.StatusId,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreateUserId = userId
                };

                foreach (var item in vm.Items)
                {
                    dispatch.Items.Add(new PostalDispatchItem
                    {
                        Id = Guid.CreateVersion7(),
                        PostalDispatchId = dispatch.Id,
                        IsOriginal = item.IsOriginal,
                        DocumentDescription = item.DocumentDescription,
                        ItemReference = item.ItemReference,
                        //IsActive = true,
                        IsDeleted = false,
                        CreatedAt = now,
                        CreateUserId = userId
                    });
                }

                _db.PostalDispatches.Add(dispatch);
            }
            else
            {
                // Update
                var dispatch = await _db.PostalDispatches
                    .Include(d => d.Items.Where(i => !i.IsDeleted))
                    .FirstOrDefaultAsync(d => d.Id == vm.Id && !d.IsDeleted);

                if (dispatch == null) return NotFound();

                dispatch.DispatchDate = vm.DispatchDate;
                dispatch.SenderId = vm.SenderId;
                dispatch.ReceiverId = vm.ReceiverId;
                dispatch.CourierCompanyId = vm.CourierCompanyId;
                dispatch.TrackingNumber = vm.TrackingNumber;
                dispatch.CourierMethodId = vm.CourierMethodId;
                dispatch.StatusId = vm.StatusId;
                dispatch.UpdatedAt = now;
                dispatch.EditUserId = userId;

                // Soft-delete removed items
                var submittedIds = vm.Items
                    .Where(i => i.Id.HasValue && i.Id != Guid.Empty)
                    .Select(i => i.Id!.Value)
                    .ToHashSet();

                foreach (var existing in dispatch.Items)
                {
                    if (!submittedIds.Contains(existing.Id))
                    {
                        existing.IsDeleted = true;
                        existing.UpdatedAt = now;
                        existing.EditUserId = userId;
                    }
                }

                foreach (var item in vm.Items)
                {
                    if (item.Id.HasValue && item.Id != Guid.Empty)
                    {
                        var existing = dispatch.Items.First(i => i.Id == item.Id);
                        existing.ObjectId = item.ObjectId;
                        existing.IsOriginal = item.IsOriginal;
                        existing.DocumentDescription = item.DocumentDescription;
                        existing.ItemReference = item.ItemReference;
                        existing.UpdatedAt = now;
                        existing.EditUserId = userId;
                    }
                    else
                    {
                        dispatch.Items.Add(new PostalDispatchItem
                        {
                            Id = Guid.CreateVersion7(),
                            PostalDispatchId = dispatch.Id,
                            ObjectId = item.ObjectId,
                            IsOriginal = item.IsOriginal,
                            DocumentDescription = item.DocumentDescription,
                            ItemReference = item.ItemReference,
                            //IsActive = true,
                            IsDeleted = false,
                            CreatedAt = now,
                            CreateUserId = userId
                        });
                    }
                }
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshGrid, closeModal";
            return Ok();
        }

        [HttpDelete]
        [Authorize("Utilities.PostalDispatch.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dispatch = await _db.PostalDispatches
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (dispatch == null) return NotFound();

            var now = DateTime.Now;
            var userId = _currentUserService.UserId;

            dispatch.IsDeleted = true;
            dispatch.UpdatedAt = now;
            dispatch.EditUserId = userId;

            foreach (var item in dispatch.Items)
            {
                item.IsDeleted = true;
                item.UpdatedAt = now;
                item.EditUserId = userId;
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshGrid";
            return Ok();
        }

        // GET: Confirm modal
        [HttpGet]
        [Authorize("Utilities.PostalDispatch.Edit")]
        public IActionResult ConfirmModal(Guid id)
        {
            var vm = new PostalDispatchConfirmViewModel
            {
                PostalDispatchId = id,
                ConfirmationDate = DateTime.Now
            };
            return PartialView("_ConfirmModal", vm);
        }

        // POST: Confirm receipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Utilities.PostalDispatch.Edit")]
        public async Task<IActionResult> ConfirmReceipt(PostalDispatchConfirmViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                Response.Headers["HX-Retarget"] = "#modal-container";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_ConfirmModal", vm);
            }

            var userId = _currentUserService.UserId;
            var now = DateTime.Now;

            var confirmation = new PostalDispatchConfirmation
            {
                Id = Guid.CreateVersion7(),
                PostalDispatchId = vm.PostalDispatchId,
                ConfirmationDate = vm.ConfirmationDate,
                ConditionOnReceipt = vm.ConditionOnReceipt,
                Remarks = vm.Remarks,
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = now,
                CreateUserId = userId
            };

            _db.PostalDispatchConfirmations.Add(confirmation);

            // Update status to 5 (Delivered/Confirmed)
            var dispatch = await _db.PostalDispatches.FindAsync(vm.PostalDispatchId);
            if (dispatch != null)
            {
                dispatch.StatusId = 5;
                dispatch.UpdatedAt = now;
                dispatch.EditUserId = userId;
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshGrid, closeModal";
            return Ok();
        }

        private async Task LoadLookups()
        {
            ViewBag.Senders = new SelectList(
                await _db.Persons.Where(p => !p.IsDeleted)
                    .Select(p => new { p.Id, Name = p.PersonName })
                    .ToListAsync(), "Id", "Name");

            ViewBag.Receivers = new SelectList(
                await _db.Persons.Where(p => !p.IsDeleted)
                    .Select(p => new { p.Id, Name = p.PersonName })
                    .ToListAsync(), "Id", "Name");

            ViewBag.CourierCompanies = new SelectList(
                await _db.CourierCompanies.Include(x => x.Person).Where(c => !c.IsDeleted)
                    .Select(c => new { c.Id, Name = c.Person.PersonName })
                    .ToListAsync(), "Id", "Name");

            ViewBag.CourierMethods = new SelectList(
                await _db.CourierMethods.Where(m => !m.IsDeleted)
                    .Select(m => new { m.Id, Name = m.CourierMethodName })
                    .ToListAsync(), "Id", "Name");

            ViewBag.Statuses = new SelectList(
                await _db.PostalDispatchStatuses.Where(s => !s.IsDeleted)
                    .Select(s => new { s.Id, s.StatusName })
                    .ToListAsync(), "Id", "StatusName");
        }
    }
}
