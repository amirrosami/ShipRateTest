using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Marketting;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ShipRate.Application.Services
{
    public class LeadService : ILeadService
    {
        private readonly IRepository<Lead> _leadRepo;
        private readonly IRepository<LeadActivity> _activityRepo;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Industry> _industryRepo;
        private readonly IRepository<Currency> _currencyRepo;
        private readonly IRepository<LeadStatus> _statusRepo;
        private readonly IRepository<LeadSource> _sourceRepo;
        private readonly IRepository<TradeType> _tradeTypeRepo;
        private readonly IRepository<LostReason> _lostReasonRepo;
        private readonly IRepository<ActivityType> _activityTypeRepo;
        private readonly IRepository<ActivityResult> _activityResultRepo;
        private readonly IRepository<ActivityOutcome> _activityOutcomeRepo;

        public LeadService(
            IRepository<Lead> leadRepo,
            IRepository<LeadActivity> activityRepo,
            IRepository<ApplicationUser> userRepo,
            IRepository<Industry> industryRepo,
            IRepository<Currency> currencyRepo,
            IRepository<LeadStatus> statusRepo,
            IRepository<LeadSource> sourceRepo,
            IRepository<TradeType> tradeTypeRepo,
            IRepository<LostReason> lostReasonRepo,
            IRepository<ActivityType> activityTypeRepo,
            IRepository<ActivityResult> activityResultRepo,
            IRepository<ActivityOutcome> activityOutcomeRepo)
        {
            _leadRepo = leadRepo;
            _activityRepo = activityRepo;
            _userRepo = userRepo;
            _industryRepo = industryRepo;
            _currencyRepo = currencyRepo;
            _statusRepo = statusRepo;
            _sourceRepo = sourceRepo;
            _tradeTypeRepo = tradeTypeRepo;
            _lostReasonRepo = lostReasonRepo;
            _activityTypeRepo = activityTypeRepo;
            _activityResultRepo = activityResultRepo;
            _activityOutcomeRepo = activityOutcomeRepo;
        }

        public async Task<PagedResultDto<LeadDto>> GetLeadsAsync(LeadSearchDto filter)
        {
            var query = _leadRepo.GetQueryable()
                .Include(l => l.Industry)
                .Include(l => l.Currency)
                .Include(l => l.AssignedToUser)
                .Include(l => l.LeadSource)
                .Include(l => l.LeadStatus)
                .Include(l => l.TradeType)
                .Include(l => l.LostReason)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(l =>
                    l.CompanyName.ToLower().Contains(term) ||
                    l.ContactName.ToLower().Contains(term) ||
                    l.EmailAddress.ToLower().Contains(term) ||
                    l.Phone.Contains(term));
            }

            if (filter.LeadStatusId.HasValue)
                query = query.Where(l => l.LeadStatusId == filter.LeadStatusId.Value);

            if (filter.LeadSourceId.HasValue)
                query = query.Where(l => l.LeadSourceId == filter.LeadSourceId.Value);

            if (filter.AssignedToUserId.HasValue)
                query = query.Where(l => l.AssignedToUserId == filter.AssignedToUserId.Value);

            if (filter.IndustryId.HasValue)
                query = query.Where(l => l.IndustryId == filter.IndustryId.Value);

            if (filter.MinLeadScore.HasValue)
                query = query.Where(l => l.LeadScore >= filter.MinLeadScore.Value);

            if (filter.ActivityDateFrom.HasValue || filter.ActivityDateTo.HasValue)
            {
                var leadIdsWithActivity = _activityRepo.GetQueryable()
                    .Where(a => (!filter.ActivityDateFrom.HasValue || a.ActivityDate >= filter.ActivityDateFrom.Value) &&
                                (!filter.ActivityDateTo.HasValue || a.ActivityDate <= filter.ActivityDateTo.Value))
                    .Select(a => a.LeadId)
                    .Distinct();

                query = query.Where(l => leadIdsWithActivity.Contains(l.Id));
            }

            var total = await query.CountAsync();

            var leads = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(l => new LeadDto
                {
                    Id = l.Id,
                    CompanyName = l.CompanyName,
                    ContactName = l.ContactName,
                    EmailAddress = l.EmailAddress,
                    Phone = l.Phone,
                    IndustryId = l.IndustryId,
                    IndustryName = l.Industry.IndustryName,
                    EstimatedRevenue = l.EstimatedRevenue,
                    CurrencyId = l.CurrencyId,
                    CurrencyName = l.Currency.CurrencyName,
                    AssignedToUserId = l.AssignedToUserId,
                    AssignedToUserName = l.AssignedToUser.FullName,
                    LeadSourceId = l.LeadSourceId,
                    LeadSourceName = l.LeadSource.LeadSourceName,
                    LeadStatusId = l.LeadStatusId,
                    LeadStatusName = l.LeadStatus.LeadStatusName,
                    LeadScore = l.LeadScore,
                    TradeTypeId = l.TradeTypeId,
                    TradeTypeName = l.TradeType.TradeTypeName,
                    CargoType = l.CargoType,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            return new PagedResultDto<LeadDto>
            {
                Items = leads,
                TotalCount = total,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<LeadDto> GetLeadAsync(Guid id)
        {
            var lead = await _leadRepo.GetQueryable()
                .Include(x => x.AssignedToUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lead == null) return null;

            return new LeadDto
            {
                Id = lead.Id,
                CompanyName = lead.CompanyName,
                ContactName = lead.ContactName,
                EmailAddress = lead.EmailAddress,
                Phone = lead.Phone,
                AssignedToUserId = lead.AssignedToUserId,
                AssignedToUserName = lead.AssignedToUser?.FullName
            };
        }

        public async Task<LeadDto> CreateLeadAsync(LeadDto dto, Guid userId)
        {
            var lead = new Lead
            {
                Id = Guid.CreateVersion7(),
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                EmailAddress = dto.EmailAddress,
                Phone = dto.Phone,
                LeadSourceId = dto.LeadSourceId.Value,
                LeadStatusId = dto.LeadStatusId.Value,
                CreatedAt = DateTime.Now
            };

            await _leadRepo.AddAsync(lead);
            await _leadRepo.SaveChangesAsync();

            dto.Id = lead.Id;
            return dto;
        }

        public async Task<LeadDto> UpdateLeadAsync(LeadDto dto, Guid userId)
        {
            var lead = await _leadRepo.GetByGuidAsync(dto.Id.Value);

            lead.CompanyName = dto.CompanyName;
            lead.ContactName = dto.ContactName;
            lead.EmailAddress = dto.EmailAddress;
            lead.Phone = dto.Phone;
            lead.LeadStatusId = dto.LeadStatusId.Value;
            lead.UpdatedAt = DateTime.Now;

            await _leadRepo.UpdateAsync(lead);
            await _leadRepo.SaveChangesAsync();

            return dto;
        }

        public async Task DeleteLeadAsync(Guid id)
        {
            var lead = await _leadRepo.GetByGuidAsync(id);
            await _leadRepo.DeleteAsync(lead);
            await _leadRepo.SaveChangesAsync();
        }

        public async Task<AssignUserDto> GetAssignModelAsync(Guid leadId)
        {
            var lead = await _leadRepo.GetByGuidAsync(leadId);

            var users = await _userRepo.GetQueryable()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                }).ToListAsync();

            return new AssignUserDto
            {
                LeadId = lead.Id,
                CompanyName = lead.CompanyName,
                ContactName = lead.ContactName,
                CurrentAssignedUserId = lead.AssignedToUserId,
                Users = users
            };
        }

        public async Task AssignLeadAsync(Guid leadId, Guid userId, Guid performedByUserId)
        {
            var lead = await _leadRepo.GetByGuidAsync(leadId);

            lead.AssignedToUserId = userId;
            lead.UpdatedAt = DateTime.Now;

            await _leadRepo.UpdateAsync(lead);
            await _leadRepo.SaveChangesAsync();
        }

        public async Task<List<LeadActivityDto>> GetActivitiesAsync(Guid leadId)
        {
            return await _activityRepo.GetQueryable()
                .Include(x => x.ActivityType)
                .Include(x => x.ActivityResult)
                .Include(x => x.ActivityOutcome)
                .Where(x => x.LeadId == leadId)
                .OrderByDescending(x => x.ActivityDate)
                .Select(a => new LeadActivityDto
                {
                    Id = a.Id,
                    LeadId = a.LeadId,
                    ActivityTypeId = a.ActivityTypeId,
                    ActivityResultId = a.ActivityResultId,
                    ActivityOutcomeId = a.ActivityOutcomeId,
                    Description = a.Description,
                    DurationMinutes = a.DurationMinutes,
                    ActivityDate = a.ActivityDate,
                    FollowUpDate = a.FollowUpDate,
                    ActivityTypeName = a.ActivityType.ActivityTypeName,
                    ActivityResultName = a.ActivityResult.ActivityResultName,
                    ActivityOutcomeName = a.ActivityOutcome.ActivityOutcomeName
                }).ToListAsync();
        }

        public async Task<LeadActivityDto> GetActivityAsync(Guid id)
        {
            var a = await _activityRepo.GetByGuidAsync(id);

            return new LeadActivityDto
            {
                Id = a.Id,
                LeadId = a.LeadId,
                ActivityTypeId = a.ActivityTypeId,
                ActivityResultId = a.ActivityResultId,
                ActivityOutcomeId = a.ActivityOutcomeId,
                Description = a.Description,
                DurationMinutes = a.DurationMinutes,
                ActivityDate = a.ActivityDate,
                FollowUpDate = a.FollowUpDate
            };
        }

        public async Task<LeadActivityDto> CreateActivityAsync(LeadActivityDto dto, Guid userId)
        {
            var a = new LeadActivity
            {
                Id = Guid.CreateVersion7(),
                LeadId = dto.LeadId,
                ActivityTypeId = dto.ActivityTypeId,
                ActivityResultId = dto.ActivityResultId,
                ActivityOutcomeId = dto.ActivityOutcomeId,
                Description = dto.Description,
                DurationMinutes = dto.DurationMinutes,
                ActivityDate = dto.ActivityDate,
                FollowUpDate = dto.FollowUpDate,
                CreatedAt = DateTime.Now
            };

            await _activityRepo.AddAsync(a);
            await _activityRepo.SaveChangesAsync();

            dto.Id = a.Id;
            return dto;
        }

        public async Task<LeadActivityDto> UpdateActivityAsync(LeadActivityDto dto, Guid userId)
        {
            var a = await _activityRepo.GetByGuidAsync(dto.Id);

            a.ActivityTypeId = dto.ActivityTypeId;
            a.ActivityResultId = dto.ActivityResultId;
            a.ActivityOutcomeId = dto.ActivityOutcomeId;
            a.Description = dto.Description;
            a.DurationMinutes = dto.DurationMinutes;
            a.ActivityDate = dto.ActivityDate;
            a.FollowUpDate = dto.FollowUpDate;

            await _activityRepo.UpdateAsync(a);
            await _activityRepo.SaveChangesAsync();

            return dto;
        }

        public async Task DeleteActivityAsync(Guid id)
        {
            var a = await _activityRepo.GetByGuidAsync(id);
            await _activityRepo.DeleteAsync(a);
            await _activityRepo.SaveChangesAsync();
        }


        public async Task<List<SelectListItem>> GetUsersAsync()
            => await _userRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetLeadStatusesAsync()
            => await _statusRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.LeadStatusName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetLeadSourcesAsync()
            => await _sourceRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.LeadSourceName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetIndustriesAsync()
            => await _industryRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.IndustryName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetCurrenciesAsync()
            => await _currencyRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.CurrencyName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetTradeTypesAsync()
            => await _tradeTypeRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TradeTypeName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetLostReasonsAsync()
            => await _lostReasonRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.LostReasonName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetActivityTypesAsync()
            => await _activityTypeRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ActivityTypeName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetActivityResultsAsync()
            => await _activityResultRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ActivityResultName })
            .ToListAsync();

        public async Task<List<SelectListItem>> GetActivityOutcomesAsync()
            => await _activityOutcomeRepo.GetQueryable()
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ActivityOutcomeName })
            .ToListAsync();
    }

}



