using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class ExportDocsReceiptService : IExportDocsReceiptService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IContainerActionService _containerActionService;

        public ExportDocsReceiptService(ApplicationDbContext context, ICurrentUserService currentUserService, IContainerActionService containerActionService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _containerActionService = containerActionService;
        }

        public async Task<PagedResultDto<ExportDocsReceiptListItemVm>> GetPagedAsync(ExportDocsReceiptIndexVm input)
        {
            var query = _context.ExportDocsReceipts
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.Booking)
                .Include(x => x.ReceiptDirection)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(input.ReceiptNo))
            {
                var receiptNo = input.ReceiptNo.Trim();
                query = query.Where(x => x.ReceiptNo.Contains(receiptNo));
            }

            if (input.PartyId.HasValue)
                query = query.Where(x => x.PartyId == input.PartyId.Value);

            if (input.BookingId.HasValue)
                query = query.Where(x => x.BookingId == input.BookingId.Value);

            if (input.ReceiptDirectionId.HasValue)
                query = query.Where(x => x.ReceiptDirectionId == input.ReceiptDirectionId.Value);

            if (input.FromDate.HasValue)
                query = query.Where(x => x.ReceiptDate.Date >= input.FromDate.Value.Date);

            if (input.ToDate.HasValue)
                query = query.Where(x => x.ReceiptDate.Date <= input.ToDate.Value.Date);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.ReceiptDate)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new ExportDocsReceiptListItemVm
                {
                    Id = x.Id,
                    ReceiptNo = x.ReceiptNo,
                    ReceiptDate = x.ReceiptDate,
                    PartyName = x.Party != null ? x.Party.PersonName : string.Empty,
                    BookingNo = x.Booking != null ? x.Booking.BookingNo : string.Empty,
                    ReceiptDirectionName = x.ReceiptDirection != null ? x.ReceiptDirection.ReceiptDirectionLocalName : string.Empty,
                    PersonPersonName = x.PersonPersonName,
                    Remarks = x.Remarks
                })
                .ToListAsync();
            return new PagedResultDto<ExportDocsReceiptListItemVm>(items, totalCount, input.PageNumber, input.PageSize);
        }

        public async Task<ExportDocsReceiptFormVm> GetCreateModelAsync(Guid? bookingId)
        {
            var model = new ExportDocsReceiptFormVm
            {
                ReceiptDate = DateTime.Today,
            };
            if (bookingId.HasValue && bookingId != Guid.Empty) model.BookingId = bookingId.Value;
            await FillLookupsAsync(model);
            return model;
        }

        public async Task<ExportDocsReceiptFormVm?> GetEditModelAsync(Guid id)
        {
            var entity = await _context.ExportDocsReceipts
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.Booking)
                .Include(x => x.ReceiptDirection)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            var documents = await _context.ExportDocsReceiptItems
                .AsNoTracking()
                .Include(x => x.Document)
                .Where(x => x.ExportDocsReceiptId == id)
                .OrderBy(x => x.Document.DocumentTitle)
                .Select(x => new ExportDocsReceiptDocumentRowVm
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    DocumentName = x.Document != null ? x.Document.DocumentTitle : string.Empty
                })
                .ToListAsync();

            var containers = await _context.ExportDocsReceiptContainers
                .AsNoTracking()
                .Include(x => x.Container)
                .Include(x => x.ContainerUse)
                .Where(x => x.ExportDocsReceiptId == id)
                .OrderByDescending(x => x.RBSDate)
                .Select(x => new ExportDocsReceiptContainerRowVm
                {
                    Id = x.Id,
                    ContainerId = x.ContainerId,
                    ContainerUseId = x.ContainerUseId,
                    ContainerNo = x.Container != null
                        ? x.Container.ContainerNumber
                        : x.ContainerUse != null && x.ContainerUse.Container != null
                            ? x.ContainerUse.Container.ContainerNumber
                            : string.Empty,
                    RBSDate = x.RBSDate
                })
                .ToListAsync();

            var model = new ExportDocsReceiptFormVm
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                ReceiptDirectionId = entity.ReceiptDirectionId,
                PartyId = entity.PartyId,
                BookingId = entity.BookingId,
                ReceiptNo = entity.ReceiptNo,
                ReceiptDate = entity.ReceiptDate,
                PersonPersonName = entity.PersonPersonName,
                Remarks = entity.Remarks,
                Documents = documents,
                Containers = containers
            };

            await FillLookupsAsync(model);
            return model;
        }

        public async Task<Guid> CreateAsync(ExportDocsReceiptFormVm input)
        {
            var entity = new ExportDocsReceipt
            {
                Id = Guid.CreateVersion7(),
                CompanyId = input.CompanyId,
                ReceiptDirectionId = input.ReceiptDirectionId,
                PartyId = input.PartyId,
                BookingId = input.BookingId,
                ReceiptNo = input.ReceiptNo.Trim(),
                ReceiptDate = input.ReceiptDate,
                PersonPersonName = string.IsNullOrWhiteSpace(input.PersonPersonName) ? null : input.PersonPersonName.Trim(),
                Remarks = string.IsNullOrWhiteSpace(input.Remarks) ? null : input.Remarks.Trim()
            };

            _context.ExportDocsReceipts.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(ExportDocsReceiptFormVm input)
        {
            if (!input.Id.HasValue)
                throw new InvalidOperationException("Receipt id is required for update.");

            var entity = await _context.ExportDocsReceipts
                .FirstOrDefaultAsync(x => x.Id == input.Id.Value);

            if (entity == null)
                throw new InvalidOperationException("Receipt not found.");

            entity.CompanyId = input.CompanyId;
            entity.ReceiptDirectionId = input.ReceiptDirectionId;
            entity.PartyId = input.PartyId;
            entity.BookingId = input.BookingId;
            entity.ReceiptNo = input.ReceiptNo.Trim();
            entity.ReceiptDate = input.ReceiptDate;
            entity.PersonPersonName = string.IsNullOrWhiteSpace(input.PersonPersonName) ? null : input.PersonPersonName.Trim();
            entity.Remarks = string.IsNullOrWhiteSpace(input.Remarks) ? null : input.Remarks.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ExportDocsReceipts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            var items = await _context.ExportDocsReceiptItems
                .Where(x => x.ExportDocsReceiptId == id)
                .ToListAsync();

            var containers = await _context.ExportDocsReceiptContainers
                .Where(x => x.ExportDocsReceiptId == id)
                .ToListAsync();

            if (items.Count > 0)
                _context.ExportDocsReceiptItems.RemoveRange(items);

            if (containers.Count > 0)
                _context.ExportDocsReceiptContainers.RemoveRange(containers);

            _context.ExportDocsReceipts.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<AddExportDocsReceiptDocumentVm> GetAddDocumentModelAsync(Guid exportDocsReceiptId)
        {
            var model = new AddExportDocsReceiptDocumentVm
            {
                ExportDocsReceiptId = exportDocsReceiptId,
                Documents = await _context.Documents
                    .AsNoTracking()
                    .OrderBy(x => x.DocumentTitle)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.DocumentTitle
                    })
                    .ToListAsync()
            };

            return model;
        }

        public async Task AddDocumentAsync(AddExportDocsReceiptDocumentVm input)
        {
            var exists = await _context.ExportDocsReceiptItems
                .AnyAsync(x => x.ExportDocsReceiptId == input.ExportDocsReceiptId && x.DocumentId == input.DocumentId);

            if (exists)
                return;

            var entity = new ExportDocsReceiptItem
            {
                Id = Guid.CreateVersion7(),
                ExportDocsReceiptId = input.ExportDocsReceiptId,
                DocumentId = input.DocumentId
            };

            _context.ExportDocsReceiptItems.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDocumentAsync(Guid exportDocsReceiptItemId)
        {
            var entity = await _context.ExportDocsReceiptItems
                .FirstOrDefaultAsync(x => x.Id == exportDocsReceiptItemId);

            if (entity == null)
                return;

            _context.ExportDocsReceiptItems.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<AddExportDocsReceiptContainerVm> GetAddContainerModelAsync(Guid exportDocsReceiptId)
        {
            var doc = await _context.ExportDocsReceipts.Where(x => x.Id == exportDocsReceiptId).FirstOrDefaultAsync();
            var bookingId = doc.BookingId;
            var containerIds = await _context.ContainerActions.Where(x => !x.IsDeleted && x.BookingId == bookingId && x.CntActionTypeId == 1).Select(x => x.ContainerId).ToListAsync();
            var model = new AddExportDocsReceiptContainerVm
            {
                ExportDocsReceiptId = exportDocsReceiptId,
                RBSDate = DateTime.Today,
                Containers = await _context.Containers
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && containerIds.Contains(x.Id))
                    .OrderBy(x => x.ContainerNumber)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.ContainerNumber
                    })
                    .ToListAsync()
            };
            return model;
        }
        public async Task AddContainerAsync(AddExportDocsReceiptContainerVm input)
        {
            var entity = new ExportDocsReceiptContainer
            {
                Id = Guid.CreateVersion7(),
                ExportDocsReceiptId = input.ExportDocsReceiptId,
                ContainerId = input.ContainerId,
                //ContainerUseId = input.ContainerUseId,
                RBSDate = input.RBSDate
            };
            _context.ExportDocsReceiptContainers.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveContainerAsync(Guid exportDocsReceiptContainerId)
        {
            var entity = await _context.ExportDocsReceiptContainers
                .FirstOrDefaultAsync(x => x.Id == exportDocsReceiptContainerId);

            if (entity == null)
                return;

            _context.ExportDocsReceiptContainers.Remove(entity);
            await _context.SaveChangesAsync();
        }

        private async Task FillLookupsAsync(ExportDocsReceiptFormVm model)
        {
            model.ReceiptDirections = await _context.ReceiptDirections
                .AsNoTracking()
                .OrderBy(x => x.ReceiptDirectionLocalName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ReceiptDirectionLocalName
                })
                .ToListAsync();

            model.Parties = await _context.Persons
                .AsNoTracking()
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();


            model.Companies = await _context.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();




            if (model.BookingId != null && model.BookingId != Guid.Empty)
            {
                model.Bookings = await _context.Bookings
                    .AsNoTracking()
                    .Where(x => x.Id == model.BookingId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.BookingNo
                    })
                    .ToListAsync();
            }
            else
            {
                model.Bookings = await _context.Bookings
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.BookingNo
                    })
                    .ToListAsync();
            }
        }
        public async Task<bool> ConfirmAsync(Guid id)
        {
            var receipt = await _context.ExportDocsReceipts.Include(x => x.Containers).Where(x => !x.IsConfirmed && !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (receipt == null)
            {
                return false;
            }
            receipt.ConfirmDate = DateTime.Now;
            receipt.IsConfirmed = true;
            receipt.ConfirmUserId = _currentUserService.UserId;
            bool b1 = await _containerActionService.RBSReceiptAsync(receipt);
            if (b1)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
