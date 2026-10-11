using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.JobsServices
{
    public class JobPartyService : IJobPartyService
    {
        private readonly ApplicationDbContext _db;
        public IActivityLogService _activityLogService;

        public JobPartyService(ApplicationDbContext db, IActivityLogService activityLogService)
        {
            _db = db;
            _activityLogService = activityLogService;
        }

        public async Task<PagedResultDto<JobPartyListItemVm>> GetListAsync(JobPartyFilterVm filter)
        {
            var query = _db.JobParties
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.RoleType)
                .Where(x => !x.IsDeleted && x.JobId == filter.JobId)
                .OrderBy(x => x.SequenceNo)
                .ThenBy(x => x.CreatedAt);

            var items = await query
                .Select(x => new JobPartyListItemVm
                {
                    Id = x.Id,
                    SequenceNo = x.SequenceNo,
                    PartyId = x.PartyId,
                    PartyName = x.Party.PersonName,
                    RoleTypeId = x.RoleTypeId,
                    RoleTypeName = x.RoleType.RoleTypeName,
                    IsPrimaryRole = x.IsPrimaryRole,
                    ContactName = x.ContactName,
                    ContactEmail = x.ContactEmail,
                    ContactPhone = x.ContactPhone,
                    //IsActive = x.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<JobPartyListItemVm>
            {
                Items = items,
                TotalCount = items.Count
            };
        }

        public async Task<JobPartyFormVm> BuildCreateVmAsync(Guid jobId)
        {
            var vm = new JobPartyFormVm
            {
                JobId = jobId,
                IsActive = true,
                SequenceNo = await GetNextSequenceNoAsync(jobId)
            };

            await FillLookupsAsync(vm);
            return vm;
        }

        public async Task<JobPartyFormVm?> BuildEditVmAsync(Guid id)
        {
            var entity = await _db.JobParties
                .Include(x => x.Party)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            var vm = new JobPartyFormVm
            {
                Id = entity.Id,
                JobId = entity.JobId,
                SequenceNo = entity.SequenceNo,
                PartyId = entity.PartyId,
                PartyName = entity.Party.PersonName,
                RoleTypeId = entity.RoleTypeId,
                IsPrimaryRole = entity.IsPrimaryRole,
                ContactName = entity.ContactName,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                //IsActive = entity.IsActive
            };

            await FillLookupsAsync(vm);
            return vm;
        }

        public async Task<JobPartyDetailsVm?> GetDetailsAsync(Guid id)
        {
            return await _db.JobParties
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.RoleType)
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new JobPartyDetailsVm
                {
                    Id = x.Id,
                    JobId = x.JobId,
                    SequenceNo = x.SequenceNo,
                    PartyId = x.PartyId,
                    PartyName = x.Party.PersonName,
                    RoleTypeId = x.RoleTypeId,
                    RoleTypeName = x.RoleType.RoleTypeName,
                    IsPrimaryRole = x.IsPrimaryRole,
                    ContactName = x.ContactName,
                    ContactEmail = x.ContactEmail,
                    ContactPhone = x.ContactPhone,
                    //IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<JobPartyDeleteVm?> GetDeleteVmAsync(Guid id)
        {
            return await _db.JobParties
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.RoleType)
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new JobPartyDeleteVm
                {
                    Id = x.Id,
                    JobId = x.JobId,
                    PartyName = x.Party.PersonName,
                    RoleTypeName = x.RoleType.RoleTypeName,
                    SequenceNo = x.SequenceNo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string? Error, Guid Id)> CreateAsync(JobPartyFormVm vm, Guid currentUserId)
        {
            var jobExists = await _db.Jobs.AnyAsync(x => x.Id == vm.JobId);
            if (!jobExists)
                return (false, "Invalid Job.", Guid.Empty);

            var partyExists = vm.PartyId.HasValue &&
                              await _db.Persons.AnyAsync(x => x.Id == vm.PartyId.Value && !x.IsDeleted);
            if (!partyExists)
                return (false, "Invalid Party.", Guid.Empty);

            var roleTypeExists = vm.RoleTypeId.HasValue &&
                                 await _db.RoleTypes.AnyAsync(x => x.Id == vm.RoleTypeId.Value && !x.IsDeleted && x.IsActive);
            if (!roleTypeExists)
                return (false, "Invalid Role Type.", Guid.Empty);

            if (await ExistsDuplicateAsync(vm.JobId, vm.PartyId!.Value, vm.RoleTypeId!.Value, null))
                return (false, "This party with the selected role already exists for this job.", Guid.Empty);

            if (vm.ContactEmail?.Trim() == "") { vm.ContactEmail = null; }

            var entity = new JobParty
            {
                Id = Guid.CreateVersion7(),
                JobId = vm.JobId,
                SequenceNo = vm.SequenceNo,
                PartyId = vm.PartyId.Value,
                RoleTypeId = vm.RoleTypeId.Value,
                IsPrimaryRole = vm.IsPrimaryRole,
                ContactName = vm.ContactName?.Trim(),
                ContactEmail = vm.ContactEmail?.Trim(),
                ContactPhone = vm.ContactPhone?.Trim(),
                //IsActive = vm.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            _db.JobParties.Add(entity);
            await _db.SaveChangesAsync();

            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Party Created",
                    Description = vm.PartyName,
                    EntityTypeId = 1,
                    EntityId = vm.JobId,
                    PerformedByUserId = currentUserId,
                    IsSystemGenerated = true,
                }
                );


            return (true, null, entity.Id);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(JobPartyFormVm vm, Guid currentUserId)
        {
            if (!vm.Id.HasValue)
                return (false, "Invalid Id.");

            var entity = await _db.JobParties
                .FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted);

            if (entity == null)
                return (false, "Record not found.");

            var partyExists = vm.PartyId.HasValue &&
                              await _db.Persons.AnyAsync(x => x.Id == vm.PartyId.Value && !x.IsDeleted);
            if (!partyExists)
                return (false, "Invalid Party.");

            var roleTypeExists = vm.RoleTypeId.HasValue &&
                                 await _db.RoleTypes.AnyAsync(x => x.Id == vm.RoleTypeId.Value && !x.IsDeleted && x.IsActive);
            if (!roleTypeExists)
                return (false, "Invalid Role Type.");

            if (await ExistsDuplicateAsync(entity.JobId, vm.PartyId!.Value, vm.RoleTypeId!.Value, entity.Id))
                return (false, "This party with the selected role already exists for this job.");

            if (vm.ContactEmail?.Trim() == "") { vm.ContactEmail = null; }

            entity.SequenceNo = vm.SequenceNo;
            entity.PartyId = vm.PartyId.Value;
            entity.RoleTypeId = vm.RoleTypeId.Value;
            entity.IsPrimaryRole = vm.IsPrimaryRole;
            entity.ContactName = vm.ContactName?.Trim();
            entity.ContactEmail = vm.ContactEmail?.Trim();
            entity.ContactPhone = vm.ContactPhone?.Trim();
            //entity.IsActive = vm.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id, Guid currentUserId)
        {
            var entity = await _db.JobParties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return (false, "Record not found.");

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task FillLookupsAsync(JobPartyFormVm vm)
        {
            vm.Parties = await _db.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();

            vm.RoleTypes = await _db.RoleTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.RoleTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RoleTypeName
                })
                .ToListAsync();
        }

        private async Task<int> GetNextSequenceNoAsync(Guid jobId)
        {
            var max = await _db.JobParties
                .Where(x => x.JobId == jobId && !x.IsDeleted)
                .Select(x => (int?)x.SequenceNo)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        private async Task<bool> ExistsDuplicateAsync(Guid jobId, Guid partyId, int roleTypeId, Guid? excludedId)
        {
            return await _db.JobParties.AnyAsync(x =>
                !x.IsDeleted &&
                x.JobId == jobId &&
                x.PartyId == partyId &&
                x.RoleTypeId == roleTypeId &&
                (!excludedId.HasValue || x.Id != excludedId.Value));
        }
    }
}
