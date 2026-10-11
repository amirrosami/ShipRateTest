using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.InventoryViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    [Route("inventory/product-requests")]
    public class ProductRequestsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _cuurentUserService;

        public ProductRequestsController(ApplicationDbContext db, ICurrentUserService user)
        {
            _db = db;
            _cuurentUserService = user;
        }
        [HttpGet]
        [Authorize("Inventory.ProductRequests.View")]
        public async Task<IActionResult> Index(string? search, int? statusId, int page = 1)
        {
            var q = _db.ProductRequests
                .Include(r => r.RequestedByUser)
                .Include(r => r.ApprovedByManager)
                .Include(r => r.DeliveredByStoreKeeper)
                .Include(r => r.ProductRequestStatus)
                .Include(r => r.Items)
                .Where(r => !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(r => r.RequestNo.ToString().Contains(search) ||
                                 r.RequestedByUser.UserName.Contains(search));

            if (statusId.HasValue) q = q.Where(r => r.ProductRequestStatusId == statusId);


            int total = await q.CountAsync();
            const int pageSize = 15;

            var items = await q.OrderByDescending(r => r.RequestDate)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(r => new ProductRequestListVM
                {
                    Id = r.Id,
                    RequestNo = r.RequestNo,
                    RequestDate = r.RequestDate.ToString("yyyy/MM/dd"),
                    RequestedBy = r.RequestedByUser.UserName,
                    ApprovedBy = r.ApprovedByManager.UserName,
                    DeliveredBy = r.DeliveredByStoreKeeper.UserName,
                    DeliveredAt = r.DeliveredAt.Value.ToString("yyyy/MM/dd"),
                    ItemCount = r.Items.Count(i => !i.IsDeleted),
                    Status = r.ProductRequestStatus.ProductRequestStatusName,
                    StatusId = r.ProductRequestStatusId,
                    Remarks = r.Remarks
                }).ToListAsync();

            ViewBag.Statuses = await _db.ProductRequestStatus.ToListAsync();
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.StatusId = statusId;

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_Table", items);

            return View(items);
        }
        [HttpGet("create")]
        [Authorize("Inventory.ProductRequests.Edit")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _db.Products.Where(p => p.IsActive && !p.IsDeleted)
                .Select(p => new { p.Id, p.ProductName }).ToListAsync();
            return PartialView("_Form", new ProductRequestFormVM());
        }

        [HttpPost("create")]
        [Authorize("Inventory.ProductRequests.Edit")]
        public async Task<IActionResult> Create(ProductRequestFormVM vm)
        {
            if (!ModelState.IsValid) return BadRequest();

            var lastNo = await _db.ProductRequests.MaxAsync(r => (int?)r.RequestNo) ?? 0;

            var request = new ProductRequest
            {
                Id = Guid.CreateVersion7(),
                CompanyId = 1,
                RequestNo = lastNo + 1,
                RequestDate = DateTime.Now,
                RequestedByUserId = _cuurentUserService.UserId,
                ProductRequestStatusId = -1, // Draft
                Remarks = vm.Remarks,
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _cuurentUserService.UserId,
                Items = vm.Items.Select(i => new ProductRequestItem
                {
                    Id = Guid.CreateVersion7(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _cuurentUserService.UserId
                }).ToList()
            };

            _db.ProductRequests.Add(request);
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("<div class='alert alert-success'>Request Registered.</div>", "text/html");
        }

        // ── EDIT MODAL ────────────────────────────────────────
        [HttpGet("{id}/edit")]
        [Authorize("Inventory.ProductRequests.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var r = await _db.ProductRequests.Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (r == null) return NotFound();

            ViewBag.Products = await _db.Products.Where(p => p.IsActive && !p.IsDeleted)
                .Select(p => new { p.Id, p.ProductName }).ToListAsync();

            var vm = new ProductRequestFormVM
            {
                Id = r.Id,
                Remarks = r.Remarks,
                Items = r.Items.Where(i => !i.IsDeleted).Select(i => new ProductRequestItemFormVM
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
            return PartialView("_Form", vm);
        }

        [HttpPost("{id}/edit")]
        [Authorize("Inventory.ProductRequests.Edit")]
        public async Task<IActionResult> Edit(Guid id, ProductRequestFormVM vm)
        {
            var r = await _db.ProductRequests.Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();

            r.Remarks = vm.Remarks;
            r.UpdatedAt = DateTime.Now;
            r.EditUserId = _cuurentUserService.UserId;

            // sync items
            foreach (var item in r.Items) item.IsDeleted = true;
            foreach (var i in vm.Items)
            {
                var existing = i.Id.HasValue ? r.Items.FirstOrDefault(x => x.Id == i.Id) : null;
                if (existing != null) { existing.Quantity = i.Quantity; existing.IsDeleted = false; }
                else r.Items.Add(new ProductRequestItem
                {
                    Id = Guid.CreateVersion7(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    ProductRequestId = id,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _cuurentUserService.UserId
                });
            }

            await _db.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("<div class='alert alert-success'>ویرایش ذخیره شد.</div>", "text/html");
        }

        // ── Send Request───────────────────────────────────────────
        [HttpPost("{id}/sendrequest")]
        [Authorize("Inventory.ProductRequests.Edit")]
        public async Task<IActionResult> SendRequest(Guid id)
        {
            var r = await _db.ProductRequests.FindAsync(id);
            if (r == null) return NotFound();

            r.ProductRequestStatusId = 1; // Pending
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("<div class='alert alert-success'>Request Sent.</div>", "text/html");
        }

        // ── Reject Request───────────────────────────────────────────
        [HttpPost("{id}/reject")]
        [Authorize("Inventory.ProductRequests.ApproveReject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var r = await _db.ProductRequests.FindAsync(id);
            if (r == null) return NotFound();

            r.ProductRequestStatusId = 9; // Rejected
            r.ApprovedByManagerId = _cuurentUserService.UserId;
            r.ApprovedAt = DateTime.Now;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("<div class='alert alert-success'>Request Rejected.</div>", "text/html");
        }



        // ── APPROVE ───────────────────────────────────────────
        [HttpPost("{id}/approve")]
        [Authorize("Inventory.ProductRequests.ApproveReject")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var r = await _db.ProductRequests.FindAsync(id);
            if (r == null) return NotFound();

            r.ProductRequestStatusId = 2; // Approved
            r.ApprovedByManagerId = _cuurentUserService.UserId;
            r.ApprovedAt = DateTime.Now;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("<div class='alert alert-success'>Request Approved.</div>", "text/html");
        }

        // ── DELIVER ───────────────────────────────────────────
        [HttpPost("{id}/deliver")]
        [Authorize("Inventory.ProductRequests.Deliver")]
        public async Task<IActionResult> Deliver(Guid id)
        {
            var r = await _db.ProductRequests.FindAsync(id);
            if (r == null) return NotFound();

            r.ProductRequestStatusId = 3; // Delivered
            r.DeliveredByStoreKeeperId = _cuurentUserService.UserId;
            r.DeliveredAt = DateTime.Now;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("<div class='alert alert-success'>Request Delivered.</div>", "text/html");
        }
        [HttpDelete("{id}")]
        [Authorize("Inventory.ProductRequests.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var r = await _db.ProductRequests.FindAsync(id);
            if (r == null) return NotFound();

            r.IsDeleted = true;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "refreshTable";
            return Content("", "text/html");
        }
        [HttpGet("{id}/detail")]
        [Authorize("Inventory.ProductRequests.View")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var r = await _db.ProductRequests
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByManager)
                .Include(x => x.DeliveredByStoreKeeper)
                .Include(x => x.ProductRequestStatus)
                .Include(x => x.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (r == null) return NotFound();
            return PartialView("_Detail", r);
        }
    }
}
