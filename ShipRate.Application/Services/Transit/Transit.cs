using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Transit;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.TransitOperation;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Transit
{

    public class TIRCarnetService : ITIRCarnetService
    {
        private const int CompanyId = 1;

        private const int StatusDraft = -1;
        private const int StatusInUse = 1;
        private const int StatusClosed = 2;
        private const int StatusReturned = 3;
        private const int StatusSettled = 4;
        private const int StatusExpired = 5;
        private const int StatusCanceled = 9;

        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public TIRCarnetService(
            ApplicationDbContext db,
            ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        // --------------------------------------------------------------------
        // Index / Search
        // --------------------------------------------------------------------

        public async Task<TIRCarnetIndexVm> GetIndexVmAsync(TIRCarnetSearchVm search)
        {
            search.PageNumber = search.PageNumber <= 0 ? 1 : search.PageNumber;
            search.PageSize = NormalizePageSize(search.PageSize);

            await FillSearchLookupsAsync(search);

            var result = await SearchAsync(search);

            return new TIRCarnetIndexVm
            {
                Search = search,
                Result = result
            };
        }

        public async Task<PagedResultDto<TIRCarnetListItemVm>> SearchAsync(TIRCarnetSearchVm search)
        {
            search.PageNumber = search.PageNumber <= 0 ? 1 : search.PageNumber;
            search.PageSize = NormalizePageSize(search.PageSize);

            var query =
                from c in _db.TIRCarnets.AsNoTracking()
                join country in _db.Countries.AsNoTracking()
                    on c.IssuingCountryId equals country.Id
                join association in _db.Associations.AsNoTracking()
                    on c.IssuingAssociationId equals association.Id
                join currency in _db.Currencies.AsNoTracking()
                    on c.CurrencyId equals currency.Id
                join status in _db.TIRCarnetStatus.AsNoTracking()
                    on c.StatusId equals status.Id
                where c.CompanyId == CompanyId
                      && !c.IsDeleted
                select new
                {
                    Carnet = c,
                    CountryName = country.CountryName,
                    AssociationName = association.AssociationName,
                    CurrencyName = currency.CurrencyCode + " - " + currency.CurrencyName,
                    StatusTitle = status.TIRCarnetStatusTitle
                };

            if (!string.IsNullOrWhiteSpace(search.TIRCarnetNumber))
            {
                var term = search.TIRCarnetNumber.Trim();
                query = query.Where(x => x.Carnet.TIRCarnetNumber.Contains(term));
            }

            if (search.IssuingCountryId.HasValue)
            {
                query = query.Where(x => x.Carnet.IssuingCountryId == search.IssuingCountryId.Value);
            }

            if (search.IssuingAssociationId.HasValue)
            {
                query = query.Where(x => x.Carnet.IssuingAssociationId == search.IssuingAssociationId.Value);
            }

            if (search.StatusId.HasValue)
            {
                query = query.Where(x => x.Carnet.StatusId == search.StatusId.Value);
            }

            if (search.IssueDateFrom.HasValue)
            {
                query = query.Where(x => x.Carnet.IssueDate >= search.IssueDateFrom.Value);
            }

            if (search.IssueDateTo.HasValue)
            {
                var to = search.IssueDateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.Carnet.IssueDate < to);
            }

            if (search.ExpiryDateFrom.HasValue)
            {
                query = query.Where(x => x.Carnet.ExpiryDate >= search.ExpiryDateFrom.Value);
            }

            if (search.ExpiryDateTo.HasValue)
            {
                var to = search.ExpiryDateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.Carnet.ExpiryDate < to);
            }

            //if (search.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.Carnet.IsActive == search.IsActive.Value);
            //}

            query = ApplySorting(query, search.SortColumn, search.SortDirection);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new TIRCarnetListItemVm
                {
                    Id = x.Carnet.Id,
                    TIRCarnetNumber = x.Carnet.TIRCarnetNumber,

                    IssuingCountryId = x.Carnet.IssuingCountryId,
                    IssuingCountryName = x.CountryName,

                    IssuingAssociationId = x.Carnet.IssuingAssociationId,
                    IssuingAssociationName = x.AssociationName,

                    BuyDate = x.Carnet.BuyDate,
                    IssueDate = x.Carnet.IssueDate.Value,
                    ExpiryDate = x.Carnet.ExpiryDate.Value,
                    ClosedDate = x.Carnet.ClosedDate,

                    VoletsCount = x.Carnet.VoletsCount,
                    GuaranteeChainName = x.Carnet.GuaranteeChainName,
                    GuaranteeAmount = x.Carnet.GuaranteeAmount,

                    CurrencyId = x.Carnet.CurrencyId.Value,
                    CurrencyName = x.CurrencyName,

                    ReceivedFromDriverAt = x.Carnet.ReceivedFromDriverAt,
                    SubmittedToChamberAt = x.Carnet.SubmittedToChamberAt,
                    SettledAt = x.Carnet.SettledAt,

                    StatusId = x.Carnet.StatusId,
                    StatusTitle = x.StatusTitle,

                    Remarks = x.Carnet.Remarks,
                    //IsActive = x.Carnet.IsActive,

                    AssignmentsCount = _db.TIRAssignments.Count(a =>
                        a.TIRCarnetId == x.Carnet.Id &&
                        !a.IsDeleted),

                    UsagesCount = _db.TIRUsages.Count(u =>
                        !u.IsDeleted &&
                        _db.TIRAssignments.Any(a =>
                            a.Id == u.TIRAssignmentId &&
                            a.TIRCarnetId == x.Carnet.Id &&
                            !a.IsDeleted))
                })
                .ToListAsync();

            return new PagedResultDto<TIRCarnetListItemVm>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = search.PageNumber,
                PageSize = search.PageSize,
                SearchTerm = search.TIRCarnetNumber,
                SortColumn = search.SortColumn,
                SortDirection = search.SortDirection
            };
        }

        // --------------------------------------------------------------------
        // TIR Carnet CRUD
        // --------------------------------------------------------------------

        public async Task<TIRCarnetCreateEditVm> GetCreateModelAsync()
        {
            var model = new TIRCarnetCreateEditVm
            {
                Id = null,
                IssueDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddMonths(6),
                StatusId = StatusDraft,
                IsActive = true
            };

            await FillCarnetLookupsAsync(model);
            return model;
        }

        public async Task<TIRCarnetCreateEditVm?> GetEditModelAsync(Guid id)
        {
            var entity = await _db.TIRCarnets
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return null;

            var model = new TIRCarnetCreateEditVm
            {
                Id = entity.Id,
                TIRCarnetNumber = entity.TIRCarnetNumber,
                IssuingCountryId = entity.IssuingCountryId,
                IssuingAssociationId = entity.IssuingAssociationId,
                BuyDate = entity.BuyDate,
                IssueDate = entity.IssueDate.Value,
                ExpiryDate = entity.ExpiryDate.Value,
                ClosedDate = entity.ClosedDate,
                VoletsCount = entity.VoletsCount,
                GuaranteeChainName = entity.GuaranteeChainName,
                GuaranteeAmount = entity.GuaranteeAmount,
                CurrencyId = entity.CurrencyId.Value,
                ReceivedFromDriverAt = entity.ReceivedFromDriverAt,
                SubmittedToChamberAt = entity.SubmittedToChamberAt,
                SettledAt = entity.SettledAt,
                StatusId = entity.StatusId,
                Remarks = entity.Remarks,
                //IsActive = entity.IsActive
            };

            await FillCarnetLookupsAsync(model);
            return model;
        }

        public async Task<TIRCarnetDeleteVm?> GetDeleteModelAsync(Guid id)
        {
            return await _db.TIRCarnets
                .AsNoTracking()
                .Where(x =>
                    x.Id == id &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted)
                .Select(x => new TIRCarnetDeleteVm
                {
                    Id = x.Id,
                    TIRCarnetNumber = x.TIRCarnetNumber
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateAsync(TIRCarnetCreateEditVm model)
        {
            await ValidateCarnetAsync(model);

            var now = DateTime.Now;
            var userId = _currentUserService.UserId;

            var entity = new TIRCarnet
            {
                Id = Guid.CreateVersion7(),
                CompanyId = 1,  //بعدا تغییر داده شود

                TIRCarnetNumber = model.TIRCarnetNumber.Trim(),
                IssuingCountryId = model.IssuingCountryId,
                IssuingAssociationId = model.IssuingAssociationId.Value,
                BuyDate = model.BuyDate,
                IssueDate = model.IssueDate,
                ExpiryDate = model.ExpiryDate,
                ClosedDate = model.ClosedDate,

                VoletsCount = model.VoletsCount,
                GuaranteeChainName = model.GuaranteeChainName.Trim(),
                GuaranteeAmount = model.GuaranteeAmount,
                CurrencyId = model.CurrencyId,

                ReceivedFromDriverAt = model.ReceivedFromDriverAt,
                SubmittedToChamberAt = model.SubmittedToChamberAt,
                SettledAt = model.SettledAt,

                StatusId = ResolveCarnetStatus(model),
                Remarks = model.Remarks,

                //IsActive = model.IsActive,
                IsDeleted = false,

                CreatedAt = now,
                UpdatedAt = null,
                CreateUserId = userId,
                EditUserId = null
            };

            _db.TIRCarnets.Add(entity);
            await _db.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(TIRCarnetCreateEditVm model)
        {
            if (!model.Id.HasValue)
                return false;

            await ValidateCarnetAsync(model);

            var entity = await _db.TIRCarnets
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id.Value &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            entity.TIRCarnetNumber = model.TIRCarnetNumber.Trim();
            entity.IssuingCountryId = model.IssuingCountryId;
            entity.IssuingAssociationId = model.IssuingAssociationId.Value;
            entity.BuyDate = model.BuyDate;
            entity.IssueDate = model.IssueDate;
            entity.ExpiryDate = model.ExpiryDate;
            entity.ClosedDate = model.ClosedDate;

            entity.VoletsCount = model.VoletsCount;
            entity.GuaranteeChainName = model.GuaranteeChainName.Trim();
            entity.GuaranteeAmount = model.GuaranteeAmount;
            entity.CurrencyId = model.CurrencyId;

            entity.ReceivedFromDriverAt = model.ReceivedFromDriverAt;
            entity.SubmittedToChamberAt = model.SubmittedToChamberAt;
            entity.SettledAt = model.SettledAt;

            entity.StatusId = ResolveCarnetStatus(model);
            entity.Remarks = model.Remarks;
            //entity.IsActive = model.IsActive;

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.TIRCarnets
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(Guid id, int statusId)
        {
            if (!IsValidStatus(statusId))
                return false;

            var entity = await _db.TIRCarnets
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            entity.StatusId = statusId;

            if (statusId == StatusClosed && entity.ClosedDate == null)
                entity.ClosedDate = DateTime.Now;

            if (statusId == StatusReturned && entity.ReceivedFromDriverAt == null)
                entity.ReceivedFromDriverAt = DateTime.Now;

            if (statusId == StatusSettled && entity.SettledAt == null)
                entity.SettledAt = DateTime.Now;

            //if (statusId == StatusCanceled)
            //    entity.IsActive = false;

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        // --------------------------------------------------------------------
        // Assignments
        // --------------------------------------------------------------------

        public async Task<AssignmentsPanelVm?> GetAssignmentsPanelAsync(Guid tirCarnetId)
        {
            var carnet = await _db.TIRCarnets
                .AsNoTracking()
                .Where(x =>
                    x.Id == tirCarnetId &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.TIRCarnetNumber
                })
                .FirstOrDefaultAsync();

            if (carnet == null)
                return null;

            var items = await
                (
                    from a in _db.TIRAssignments.AsNoTracking()
                    join t in _db.Trucks.AsNoTracking()
                        on a.TruckId equals t.Id
                    where a.CompanyId == CompanyId
                          && a.TIRCarnetId == tirCarnetId
                          && !a.IsDeleted
                    orderby a.TIRAssignmentAssignDate descending
                    select new TIRAssignmentListItemVm
                    {
                        Id = a.Id,
                        TIRCarnetId = a.TIRCarnetId,
                        TruckId = a.TruckId,
                        TruckDisplay = t.TruckReferenceTitle + " / " + t.TruckPlateNumber,
                        TIRAssignmentAssignDate = a.TIRAssignmentAssignDate,
                        TIRAssignmentReleaseDate = a.TIRAssignmentReleaseDate,
                        Remarks = a.Remarks,
                        //IsActive = a.IsActive
                    }
                )
                .ToListAsync();

            return new AssignmentsPanelVm
            {
                TIRCarnetId = carnet.Id,
                TIRCarnetNumber = carnet.TIRCarnetNumber,
                Items = items
            };
        }

        public async Task<TIRAssignmentCreateEditVm?> GetAssignmentCreateModelAsync(Guid tirCarnetId)
        {
            var carnet = await _db.TIRCarnets
                .AsNoTracking()
                .Where(x =>
                    x.Id == tirCarnetId &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.TIRCarnetNumber
                })
                .FirstOrDefaultAsync();

            if (carnet == null)
                return null;

            var model = new TIRAssignmentCreateEditVm
            {
                Id = null,
                TIRCarnetId = carnet.Id,
                TIRCarnetNumber = carnet.TIRCarnetNumber,
                TIRAssignmentAssignDate = DateTime.Now,
                IsActive = true
            };

            await FillAssignmentLookupsAsync(model);
            return model;
        }

        public async Task<TIRAssignmentCreateEditVm?> GetAssignmentEditModelAsync(Guid id)
        {
            var data = await
                (
                    from a in _db.TIRAssignments.AsNoTracking()
                    join c in _db.TIRCarnets.AsNoTracking()
                        on a.TIRCarnetId equals c.Id
                    where a.Id == id
                          && a.CompanyId == CompanyId
                          && c.CompanyId == CompanyId
                          && !a.IsDeleted
                          && !c.IsDeleted
                    select new
                    {
                        Assignment = a,
                        c.TIRCarnetNumber
                    }
                )
                .FirstOrDefaultAsync();

            if (data == null)
                return null;

            var model = new TIRAssignmentCreateEditVm
            {
                Id = data.Assignment.Id,
                TIRCarnetId = data.Assignment.TIRCarnetId,
                TIRCarnetNumber = data.TIRCarnetNumber,
                TruckId = data.Assignment.TruckId,
                TIRAssignmentAssignDate = data.Assignment.TIRAssignmentAssignDate,
                TIRAssignmentReleaseDate = data.Assignment.TIRAssignmentReleaseDate,
                Remarks = data.Assignment.Remarks,
                //IsActive = data.Assignment.IsActive
            };

            await FillAssignmentLookupsAsync(model);
            return model;
        }

        public async Task<TIRAssignmentDeleteVm?> GetAssignmentDeleteModelAsync(Guid id)
        {
            return await
                (
                    from a in _db.TIRAssignments.AsNoTracking()
                    join t in _db.Trucks.AsNoTracking()
                        on a.TruckId equals t.Id
                    where a.Id == id
                          && a.CompanyId == CompanyId
                          && !a.IsDeleted
                    select new TIRAssignmentDeleteVm
                    {
                        Id = a.Id,
                        TIRCarnetId = a.TIRCarnetId,
                        TruckDisplay = t.TruckReferenceTitle + " / " + t.TruckPlateNumber
                    }
                )
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateAssignmentAsync(TIRAssignmentCreateEditVm model)
        {
            var carnet = await _db.TIRCarnets
                .FirstOrDefaultAsync(x =>
                    x.Id == model.TIRCarnetId &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (carnet == null)
                throw new InvalidOperationException("TIR Carnet was not found.");

            var truckExists = await _db.Trucks.AnyAsync(x =>
                x.Id == model.TruckId &&
                !x.IsDeleted
                //&& x.IsActive
                );

            if (!truckExists)
                throw new InvalidOperationException("Selected truck is not valid.");

            var activeAssignmentExists = await _db.TIRAssignments.AnyAsync(x =>
                x.TIRCarnetId == model.TIRCarnetId &&
                x.TruckId == model.TruckId &&
                //x.IsActive &&
                !x.IsDeleted);

            if (activeAssignmentExists)
                throw new InvalidOperationException("This truck already has an active assignment for this TIR Carnet.");

            var now = DateTime.Now;
            var userId = _currentUserService.UserId;

            var entity = new TIRAssignment
            {
                Id = Guid.CreateVersion7(),
                CompanyId = CompanyId,
                TIRCarnetId = model.TIRCarnetId,
                TruckId = model.TruckId,
                TIRAssignmentAssignDate = model.TIRAssignmentAssignDate,
                TIRAssignmentReleaseDate = model.TIRAssignmentReleaseDate,
                Remarks = model.Remarks,
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = now,
                UpdatedAt = null,
                CreateUserId = userId,
                EditUserId = null
            };

            _db.TIRAssignments.Add(entity);

            // Auto status
            if (carnet.StatusId == StatusDraft)
                carnet.StatusId = StatusInUse;

            carnet.UpdatedAt = now;
            carnet.EditUserId = userId;

            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAssignmentAsync(TIRAssignmentCreateEditVm model)
        {
            if (!model.Id.HasValue)
                return false;

            var entity = await _db.TIRAssignments
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id.Value &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            var truckExists = await _db.Trucks.AnyAsync(x =>
                x.Id == model.TruckId &&
                x.CompanyId == CompanyId &&
                !x.IsDeleted
                //&&                 x.IsActive
                );

            if (!truckExists)
                throw new InvalidOperationException("Selected truck is not valid.");

            entity.TruckId = model.TruckId;
            entity.TIRAssignmentAssignDate = model.TIRAssignmentAssignDate;
            entity.TIRAssignmentReleaseDate = model.TIRAssignmentReleaseDate;
            entity.Remarks = model.Remarks;
            //entity.IsActive = model.IsActive;

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAssignmentAsync(Guid id)
        {
            var entity = await _db.TIRAssignments
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        // --------------------------------------------------------------------
        // Usages by Assignment
        // --------------------------------------------------------------------

        public async Task<UsagesPanelVm?> GetUsagesPanelAsync(Guid tirAssignmentId)
        {
            var assignment = await
                (
                    from a in _db.TIRAssignments.AsNoTracking()
                    join c in _db.TIRCarnets.AsNoTracking()
                        on a.TIRCarnetId equals c.Id
                    join t in _db.Trucks.AsNoTracking()
                        on a.TruckId equals t.Id
                    where a.Id == tirAssignmentId
                          && a.CompanyId == CompanyId
                          && c.CompanyId == CompanyId
                          && !a.IsDeleted
                          && !c.IsDeleted
                    select new
                    {
                        AssignmentId = a.Id,
                        CarnetId = c.Id,
                        c.TIRCarnetNumber,
                        TruckDisplay = t.TruckReferenceTitle + " / " + t.TruckPlateNumber
                    }
                )
                .FirstOrDefaultAsync();

            if (assignment == null)
                return null;

            var items = await
                (
                    from u in _db.TIRUsages.AsNoTracking()
                    join io in _db.InOut.AsNoTracking()
                        on u.InOutId equals io.Id
                    join border0 in _db.Locations.AsNoTracking()
                        on u.BorderId equals border0.Id into borderJoin
                    from border in borderJoin.DefaultIfEmpty()
                    join customs0 in _db.Locations.AsNoTracking()
                        on u.CustomsOfficeId equals customs0.Id into customsJoin
                    from customs in customsJoin.DefaultIfEmpty()
                    where u.CompanyId == CompanyId
                          && u.TIRAssignmentId == tirAssignmentId
                          && !u.IsDeleted
                    orderby u.TIRUsageUsedAt descending
                    select new TIRUsageListItemVm
                    {
                        Id = u.Id,
                        TIRAssignmentId = u.TIRAssignmentId,
                        TIRCarnetId = assignment.CarnetId,

                        TIRCarnetNumber = assignment.TIRCarnetNumber,
                        TruckDisplay = assignment.TruckDisplay,

                        BorderId = u.BorderId,
                        BorderName = border != null ? border.LocationName : null,

                        CustomsOfficeId = u.CustomsOfficeId,
                        CustomsOfficeName = customs != null ? customs.LocationName : null,

                        InOutId = u.InOutId,
                        InOutName = io.InOutName,

                        VoletNo = u.VoletNo,
                        TIRUsageUsedAt = u.TIRUsageUsedAt,
                        Remarks = u.Remarks,
                        //IsActive = u.IsActive
                    }
                )
                .ToListAsync();

            return new UsagesPanelVm
            {
                TIRAssignmentId = assignment.AssignmentId,
                TIRCarnetId = assignment.CarnetId,
                TIRCarnetNumber = assignment.TIRCarnetNumber,
                TruckDisplay = assignment.TruckDisplay,
                Items = items
            };
        }

        public async Task<TIRUsageCreateEditVm?> GetUsageCreateModelAsync(Guid tirAssignmentId)
        {
            var data = await GetAssignmentHeaderAsync(tirAssignmentId);

            if (data == null)
                return null;

            var model = new TIRUsageCreateEditVm
            {
                Id = null,
                TIRAssignmentId = data.TIRAssignmentId,
                TIRCarnetId = data.TIRCarnetId,
                TIRCarnetNumber = data.TIRCarnetNumber,
                TruckDisplay = data.TruckDisplay,
                TIRUsageUsedAt = DateTime.Now,
                IsActive = true
            };

            await FillUsageLookupsAsync(model);
            return model;
        }

        public async Task<TIRUsageCreateEditVm?> GetUsageEditModelAsync(Guid id)
        {
            var data = await
                (
                    from u in _db.TIRUsages.AsNoTracking()
                    join a in _db.TIRAssignments.AsNoTracking()
                        on u.TIRAssignmentId equals a.Id
                    join c in _db.TIRCarnets.AsNoTracking()
                        on a.TIRCarnetId equals c.Id
                    join t in _db.Trucks.AsNoTracking()
                        on a.TruckId equals t.Id
                    where u.Id == id
                          && u.CompanyId == CompanyId
                          && a.CompanyId == CompanyId
                          && c.CompanyId == CompanyId
                          && !u.IsDeleted
                          && !a.IsDeleted
                          && !c.IsDeleted
                    select new
                    {
                        Usage = u,
                        TIRCarnetId = c.Id,
                        c.TIRCarnetNumber,
                        TruckDisplay = t.TruckReferenceTitle + " / " + t.TruckPlateNumber
                    }
                )
                .FirstOrDefaultAsync();

            if (data == null)
                return null;

            var model = new TIRUsageCreateEditVm
            {
                Id = data.Usage.Id,
                TIRAssignmentId = data.Usage.TIRAssignmentId,
                TIRCarnetId = data.TIRCarnetId,
                TIRCarnetNumber = data.TIRCarnetNumber,
                TruckDisplay = data.TruckDisplay,

                BorderId = data.Usage.BorderId,
                CustomsOfficeId = data.Usage.CustomsOfficeId,
                InOutId = data.Usage.InOutId,
                VoletNo = data.Usage.VoletNo,
                TIRUsageUsedAt = data.Usage.TIRUsageUsedAt,
                Remarks = data.Usage.Remarks,
                //IsActive = data.Usage.IsActive
            };

            await FillUsageLookupsAsync(model);
            return model;
        }

        public async Task<TIRUsageDeleteVm?> GetUsageDeleteModelAsync(Guid id)
        {
            return await
                (
                    from u in _db.TIRUsages.AsNoTracking()
                    join a in _db.TIRAssignments.AsNoTracking()
                        on u.TIRAssignmentId equals a.Id
                    join c in _db.TIRCarnets.AsNoTracking()
                        on a.TIRCarnetId equals c.Id
                    join t in _db.Trucks.AsNoTracking()
                        on a.TruckId equals t.Id
                    where u.Id == id
                          && u.CompanyId == CompanyId
                          && !u.IsDeleted
                          && !a.IsDeleted
                          && !c.IsDeleted
                    select new TIRUsageDeleteVm
                    {
                        Id = u.Id,
                        TIRAssignmentId = u.TIRAssignmentId,
                        TIRCarnetId = c.Id,
                        TIRCarnetNumber = c.TIRCarnetNumber,
                        TruckDisplay = t.TruckReferenceTitle + " / " + t.TruckPlateNumber,
                        VoletNo = u.VoletNo
                    }
                )
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateUsageAsync(TIRUsageCreateEditVm model)
        {
            var assignment = await _db.TIRAssignments
                .Include(x => x.TIRCarnet)
                .FirstOrDefaultAsync(x =>
                    x.Id == model.TIRAssignmentId &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (assignment == null)
                throw new InvalidOperationException("Assignment was not found.");

            var now = DateTime.Now;
            var userId = _currentUserService.UserId;

            var entity = new TIRUsage
            {
                Id = Guid.CreateVersion7(),
                CompanyId = CompanyId,
                TIRAssignmentId = model.TIRAssignmentId,
                BorderId = model.BorderId,
                CustomsOfficeId = model.CustomsOfficeId,
                InOutId = model.InOutId,
                VoletNo = model.VoletNo.Trim(),
                TIRUsageUsedAt = model.TIRUsageUsedAt,
                Remarks = model.Remarks,
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = now,
                UpdatedAt = null,
                CreateUserId = userId,
                EditUserId = null
            };

            _db.TIRUsages.Add(entity);

            // Auto status
            var carnet = await _db.TIRCarnets.FirstOrDefaultAsync(x =>
                x.Id == assignment.TIRCarnetId &&
                x.CompanyId == CompanyId &&
                !x.IsDeleted);

            if (carnet != null && carnet.StatusId == StatusDraft)
            {
                carnet.StatusId = StatusInUse;
                carnet.UpdatedAt = now;
                carnet.EditUserId = userId;
            }

            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateUsageAsync(TIRUsageCreateEditVm model)
        {
            if (!model.Id.HasValue)
                return false;

            var entity = await _db.TIRUsages
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id.Value &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            entity.BorderId = model.BorderId;
            entity.CustomsOfficeId = model.CustomsOfficeId;
            entity.InOutId = model.InOutId;
            entity.VoletNo = model.VoletNo.Trim();
            entity.TIRUsageUsedAt = model.TIRUsageUsedAt;
            entity.Remarks = model.Remarks;
            //entity.IsActive = model.IsActive;

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUsageAsync(Guid id)
        {
            var entity = await _db.TIRUsages
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == CompanyId &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
            return true;
        }

        // --------------------------------------------------------------------
        // Lookups
        // --------------------------------------------------------------------

        private async Task FillSearchLookupsAsync(TIRCarnetSearchVm model)
        {
            model.Countries = await _db.Countries
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.CountryName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CountryName
                })
                .ToListAsync();

            model.Associations = await _db.Associations
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.AssociationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.AssociationName
                })
                .ToListAsync();

            model.Statuses = await _db.TIRCarnetStatus
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TIRCarnetStatusTitle
                })
                .ToListAsync();
        }

        private async Task FillCarnetLookupsAsync(TIRCarnetCreateEditVm model)
        {
            model.Countries = await _db.Countries
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.CountryName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CountryName
                })
                .ToListAsync();

            model.Associations = await _db.Associations
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.AssociationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.AssociationName
                })
                .ToListAsync();

            model.Currencies = await _db.Currencies
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Sort)
                .ThenBy(x => x.CurrencyCode)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode + " - " + x.CurrencyName
                })
                .ToListAsync();

            model.Statuses = await _db.TIRCarnetStatus
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TIRCarnetStatusTitle
                })
                .ToListAsync();
        }

        private async Task FillAssignmentLookupsAsync(TIRAssignmentCreateEditVm model)
        {
            model.Trucks = await _db.Trucks
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TruckReferenceTitle)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TruckReferenceTitle + " / " + x.TruckPlateNumber
                })
                .ToListAsync();
        }

        private async Task FillUsageLookupsAsync(TIRUsageCreateEditVm model)
        {
            model.Borders = await _db.Locations
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.SupportsTransit == true)
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();

            model.CustomsOffices = await _db.Locations
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.SupportsTransit == true)
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();

            model.InOutItems = await _db.InOut
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.InOutName
                })
                .ToListAsync();
        }

        // --------------------------------------------------------------------
        // Helpers
        // --------------------------------------------------------------------

        private int NormalizePageSize(int pageSize)
        {
            return pageSize switch
            {
                10 => 10,
                20 => 20,
                50 => 50,
                100 => 100,
                _ => 10
            };
        }

        private static IQueryable<dynamic> ApplySortingDynamic(
            IQueryable<dynamic> query,
            string? sortColumn,
            string? sortDirection)
        {
            return query;
        }

        private static IQueryable<T> ApplySorting<T>(
            IQueryable<T> query,
            string? sortColumn,
            string? sortDirection)
        {
            // This generic helper is not used for strongly typed anonymous projection.
            return query;
        }

        private static IQueryable<dynamic> Dummy()
        {
            throw new NotImplementedException();
        }

        //private static IQueryable<SearchProjection> ApplySorting(
        //    IQueryable<SearchProjection> query,
        //    string? sortColumn,
        //    string? sortDirection)
        //{
        //    throw new NotImplementedException();
        //}

        private IQueryable<dynamic> ApplySorting(
            IQueryable<dynamic> query,
            string? sortColumn,
            string? sortDirection,
            bool dummy = false)
        {
            return query;
        }

        private int ResolveCarnetStatus(TIRCarnetCreateEditVm model)
        {
            if (model.StatusId == StatusCanceled)
                return StatusCanceled;

            if (model.SettledAt.HasValue)
                return StatusSettled;

            if (model.ClosedDate.HasValue)
                return StatusClosed;

            if (model.ReceivedFromDriverAt.HasValue)
                return StatusReturned;

            if (model.ExpiryDate.Date < DateTime.Today)
                return StatusExpired;

            if (IsValidStatus(model.StatusId))
                return model.StatusId;

            return StatusDraft;
        }

        private bool IsValidStatus(int statusId)
        {
            return statusId is
                StatusDraft or
                StatusInUse or
                StatusClosed or
                StatusReturned or
                StatusSettled or
                StatusExpired or
                StatusCanceled;
        }

        private async Task ValidateCarnetAsync(TIRCarnetCreateEditVm model)
        {
            if (string.IsNullOrWhiteSpace(model.TIRCarnetNumber))
                throw new InvalidOperationException("TIR Carnet Number is required.");

            if (model.ExpiryDate.Date < model.IssueDate.Date)
                throw new InvalidOperationException("Expiry Date cannot be earlier than Issue Date.");

            var duplicateExists = await _db.TIRCarnets.AnyAsync(x =>
                x.CompanyId == CompanyId &&
                x.TIRCarnetNumber == model.TIRCarnetNumber.Trim() &&
                !x.IsDeleted &&
                (!model.Id.HasValue || x.Id != model.Id.Value));

            if (duplicateExists)
                throw new InvalidOperationException("TIR Carnet Number already exists.");
        }

        private async Task<AssignmentHeaderVm?> GetAssignmentHeaderAsync(Guid tirAssignmentId)
        {
            return await
                (
                    from a in _db.TIRAssignments.AsNoTracking()
                    join c in _db.TIRCarnets.AsNoTracking()
                        on a.TIRCarnetId equals c.Id
                    join t in _db.Trucks.AsNoTracking()
                        on a.TruckId equals t.Id
                    where a.Id == tirAssignmentId
                          && a.CompanyId == CompanyId
                          && c.CompanyId == CompanyId
                          && !a.IsDeleted
                          && !c.IsDeleted
                    select new AssignmentHeaderVm
                    {
                        TIRAssignmentId = a.Id,
                        TIRCarnetId = c.Id,
                        TIRCarnetNumber = c.TIRCarnetNumber,
                        TruckDisplay = t.TruckReferenceTitle + " / " + t.TruckPlateNumber
                    }
                )
                .FirstOrDefaultAsync();
        }

        private sealed class AssignmentHeaderVm
        {
            public Guid TIRAssignmentId { get; set; }
            public Guid TIRCarnetId { get; set; }
            public string TIRCarnetNumber { get; set; } = string.Empty;
            public string TruckDisplay { get; set; } = string.Empty;
        }
    }
}
