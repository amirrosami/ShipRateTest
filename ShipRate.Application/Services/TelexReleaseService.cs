using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class TelexReleaseService : ITelexReleaseService
    {
        private readonly ApplicationDbContext _db;

        public TelexReleaseService(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<TelexReleaseIndexViewModel> GetIndexAsync(TelexReleaseSearchViewModel search)
        {
            var result = await SearchAsync(search);

            return new TelexReleaseIndexViewModel
            {
                Search = search,
                Result = result,
                Statuses = await GetStatusSelectListAsync()
            };
        }

        public async Task<PagedResultDto<TelexReleaseListItemViewModel>> SearchAsync(TelexReleaseSearchViewModel search)
        {
            var query = from t in _db.TelexReleaseRequests
                        join s in _db.TelexReleaseStatus on t.TelexReleaseStatusId equals s.Id
                        join b in _db.BLs on t.BLId equals b.Id into blJoin
                        from b in blJoin.DefaultIfEmpty()
                        join cu in _db.Users on t.CreateUserId equals cu.Id
                        join au in _db.Users on t.ApprovedByUserId equals au.Id into auJoin
                        from au in auJoin.DefaultIfEmpty()

                        where !t.IsDeleted
                        select new TelexReleaseListItemViewModel
                        {
                            Id = t.Id,
                            BLNo = b != null ? b.BLNo : "",
                            StatusName = s.TelexReleaseStatusName,
                            RequestDate = t.RequestDate,
                            ApprovalDate = t.ApprovalDate,
                            CompletedAt = t.CompletedAt,
                            Remarks = t.Remarks,
                            //IsActive = t.IsActive,
                            CreateDate = t.CreatedAt,
                            CreatedByUserName = cu.UserName,
                            ApproveByUserName = au.UserName,
                            RequestNo = t.RequestNo
                        };

            if (!string.IsNullOrWhiteSpace(search.BLNo))
            {
                query = query.Where(x => x.BLNo != null && x.BLNo.Contains(search.BLNo));
            }

            if (search.TelexReleaseStatusId.HasValue)
            {
                query = query.Where(x => _db.TelexReleaseRequests
                    .Any(t => t.Id == x.Id && t.TelexReleaseStatusId == search.TelexReleaseStatusId.Value));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestDate)
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return new PagedResultDto<TelexReleaseListItemViewModel>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = search.PageNumber,
                PageSize = search.PageSize
            };
        }

        public async Task<TelexReleaseCreateEditViewModel> GetCreateModelAsync(Guid? blId)
        {
            return new TelexReleaseCreateEditViewModel
            {
                RequestDate = DateTime.Now,
                IsActive = true,
                //Statuses = await GetStatusSelectListAsync(),
                LineForwarderTypes = await GetLineForwarderTypeSelectListAsync(),
                Companies = await GetCompanySelectListAsync(),
                Carriers = await GetCarrierSelectListAsync(blId),
                BLId = blId
            };
        }

        public async Task<TelexReleaseCreateEditViewModel?> GetEditModelAsync(Guid id)
        {
            var entity = await _db.TelexReleaseRequests
                .Include(x => x.Customer)
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (entity == null)
                return null;

            var blNo = await _db.BLs
                .Where(x => x.Id == entity.BLId)
                .Select(x => x.BLNo)
                .FirstOrDefaultAsync();

            return new TelexReleaseCreateEditViewModel
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                ShipmentId = entity.ShipmentId,
                BLId = entity.BLId,
                BLNo = blNo,
                CustomerId = entity.CustomerId,
                ShippingLineId = entity.ShippingLineId,
                LineForwarderTypeId = entity.LineForwarderTypeId,
                RequestDate = entity.RequestDate,
                TelexReleaseStatusId = entity.TelexReleaseStatusId,
                BLSurrenderId = entity.BLSurrenderId,
                ApprovedByUserId = entity.ApprovedByUserId,
                ApprovalDate = entity.ApprovalDate,
                CompletedAt = entity.CompletedAt,
                Remarks = entity.Remarks,
                //IsActive = entity.IsActive,
                //Statuses = await GetStatusSelectListAsync(),
                LineForwarderTypes = await GetLineForwarderTypeSelectListAsync(),
                Carriers = await GetCarrierSelectListAsync(entity.BLId),
                Companies = await GetCompanySelectListAsync(),
                CustomerName = entity.Customer?.PersonName,
                RequestNo = entity.RequestNo
            };
        }

        public async Task<bool> CreateAsync(TelexReleaseCreateEditViewModel model, Guid userId)
        {
            var entity = new TelexReleaseRequest
            {
                Id = Guid.CreateVersion7(),
                CompanyId = model.CompanyId,
                ShipmentId = model.ShipmentId,
                BLId = model.BLId,
                CustomerId = model.CustomerId,
                ShippingLineId = model.ShippingLineId,
                LineForwarderTypeId = model.LineForwarderTypeId,
                RequestDate = model.RequestDate,
                TelexReleaseStatusId = 10,
                BLSurrenderId = model.BLSurrenderId,
                ApprovedByUserId = model.ApprovedByUserId,
                ApprovalDate = model.ApprovalDate,
                CompletedAt = model.CompletedAt,
                Remarks = model.Remarks,
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId,
                RequestNo = model.RequestNo
            };

            _db.TelexReleaseRequests.Add(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(TelexReleaseCreateEditViewModel model, Guid userId)
        {
            if (!model.Id.HasValue)
                return false;

            var entity = await _db.TelexReleaseRequests
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value && !x.IsDeleted);

            if (entity == null)
                return false;

            entity.CompanyId = model.CompanyId;
            entity.ShipmentId = model.ShipmentId;
            entity.BLId = model.BLId;
            entity.CustomerId = model.CustomerId;
            entity.ShippingLineId = model.ShippingLineId;
            entity.LineForwarderTypeId = model.LineForwarderTypeId;

            entity.RequestNo = model.RequestNo;
            entity.RequestDate = model.RequestDate;

            //entity.TelexReleaseStatusId = model.TelexReleaseStatusId;
            entity.BLSurrenderId = model.BLSurrenderId;
            entity.ApprovedByUserId = model.ApprovedByUserId;
            entity.ApprovalDate = model.ApprovalDate;
            entity.CompletedAt = model.CompletedAt;
            entity.Remarks = model.Remarks;
            //entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<TelexReleaseDetailsViewModel?> GetDetailsAsync(Guid id)
        {
            var query = from t in _db.TelexReleaseRequests
                        join s in _db.TelexReleaseStatus on t.TelexReleaseStatusId equals s.Id
                        join l in _db.LineForwarderTypes on t.LineForwarderTypeId equals l.Id
                        join cmp in _db.Companies on t.CompanyId equals cmp.Id
                        join p in _db.Persons on cmp.PersonId equals p.Id
                        join b in _db.BLs on t.BLId equals b.Id into blJoin
                        from b in blJoin.DefaultIfEmpty()
                        join c in _db.Persons on t.CustomerId equals c.Id into custJoin
                        from c in custJoin.DefaultIfEmpty()
                        join carrier in _db.Carriers on t.ShippingLineId equals carrier.Id into carrierJoin
                        from carrier in carrierJoin.DefaultIfEmpty()

                        where t.Id == id && !t.IsDeleted
                        select new TelexReleaseDetailsViewModel
                        {
                            Id = t.Id,
                            CompanyId = t.CompanyId,
                            ShipmentId = t.ShipmentId,
                            BLId = t.BLId,
                            BLNo = b != null ? b.BLNo : "",
                            CustomerId = t.CustomerId,
                            ShippingLineId = t.ShippingLineId,
                            LineForwarderTypeId = t.LineForwarderTypeId,
                            LineForwarderTypeName = l.LineForwarderTypeName,
                            RequestDate = t.RequestDate,
                            TelexReleaseStatusId = t.TelexReleaseStatusId,
                            StatusName = s.TelexReleaseStatusName,
                            BLSurrenderId = t.BLSurrenderId,
                            ApprovedByUserId = t.ApprovedByUserId,
                            ApprovalDate = t.ApprovalDate,
                            CompletedAt = t.CompletedAt,
                            Remarks = t.Remarks,
                            //IsActive = t.IsActive,
                            IsDeleted = t.IsDeleted,
                            CreatedAt = t.CreatedAt,
                            UpdatedAt = t.UpdatedAt,
                            CreateUserId = t.CreateUserId,
                            EditUserId = t.EditUserId,
                            CustomerName = c.PersonName,
                            CarrierName = carrier.Person == null ? "" : carrier.Person.PersonName,
                            CompanyName = p.PersonName
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TelexReleaseCreateEditViewModel?> GetDeleteModelAsync(Guid id)
        {
            return await GetEditModelAsync(id);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var entity = await _db.TelexReleaseRequests
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = userId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<BLLookupViewModel> SearchBLAsync(string? searchTerm)
        {
            var query = _db.BLs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.BLNo.Contains(searchTerm));
            }

            var items = await query
                .OrderBy(x => x.BLNo)
                .Take(20)
                .Select(x => new BLItemLookupViewModel
                {
                    Id = x.Id,
                    BLNo = x.BLNo
                })
                .ToListAsync();

            return new BLLookupViewModel
            {
                SearchTerm = searchTerm,
                Items = items
            };
        }

        public async Task<List<SelectListItem>> GetStatusSelectListAsync()
        {
            return await _db.TelexReleaseStatus
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.TelexReleaseStatusName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TelexReleaseStatusName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCompanySelectListAsync()
        {
            return await _db.Companies
                .Include(x => x.Person)
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetCarrierSelectListAsync(Guid? blId)
        {
            var query = _db.Carriers
                .Include(x => x.Person)
                .Where(x => !x.IsDeleted);

            if (blId != null && blId.HasValue && blId != Guid.Empty)
            {
                var jobId = await _db.BLs
                       .AsNoTracking()
                       .Where(bl => bl.Id == blId.Value)
                       .Select(bl => bl.JobId)
                       .FirstOrDefaultAsync();
                query = query.Where(person =>
                    _db.JobParties.Any(jobParty =>
                        jobParty.JobId == jobId &&
                        jobParty.PartyId == person.Id));
            }
            return await query
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetLineForwarderTypeSelectListAsync()
        {
            return await _db.LineForwarderTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LineForwarderTypeName
                })
                .ToListAsync();
        }

        public async Task<TelexReleaseCreateEditViewModel?> GetApproveModelAsync(Guid id)
        {
            return await GetEditModelAsync(id);
        }

        public async Task<bool> ApproveAsync(Guid id, Guid userId)
        {
            var entity = await _db.TelexReleaseRequests
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return false;

            entity.TelexReleaseStatusId = 50;
            entity.ApprovalDate = DateTime.Now;
            entity.ApprovedByUserId = userId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasBLConfirmedTelexAsync(Guid blId)
        {
            return await _db.TelexReleaseRequests.Where(x => x.BLId == blId && x.TelexReleaseStatusId == 50 && !x.IsDeleted).AnyAsync();
        }
    }
}
