using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Finance
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateRepository _repository;
        private readonly ApplicationDbContext _db;
        public ExchangeRateService(IExchangeRateRepository repository, ApplicationDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<PagedResultDto<ExchangeRateDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                //query = query.Where(x =>
                //    x.ExchangeRateCode.ToLower().Contains(term) ||
                //    x.ExchangeRateName.ToLower().Contains(term));
            }

            //// Filter by IsActive
            //if (request.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == request.IsActive.Value);
            //}

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<ExchangeRateDto>(items, totalCount, request.PageNumber, request.PageSize);
        }





        public async Task<ExchangeRateDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithRelationsAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(ExchangeRateCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.ExchangeRateBaseId == dto.ExchangeRateBaseId && x.FromCurrencyId == dto.FromCurrencyId && x.ToCurrencyId == dto.ToCurrencyId && x.ExchangeRateDate == dto.ExchangeRateDate))
                return (false, $"Specified Exchange Rate  already exists.");


            var entity = new ExchangeRate
            {
                ExchangeRateBaseId = dto.ExchangeRateBaseId,
                ExchangeRateDate = dto.ExchangeRateDate,
                ExchangeRateSource = dto.ExchangeRateSource,
                ToCurrencyId = dto.ToCurrencyId,
                FromCurrencyId = dto.FromCurrencyId,
                ExchangeRateValidToDate = dto.ExchangeRateDate,
                ExchangeRateValue = dto.ExchangeRateValue,
                //IsActive = true,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(ExchangeRateUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Exchange Rate not found.");


            entity.ExchangeRateBaseId = dto.ExchangeRateBaseId;
            entity.ExchangeRateDate = dto.ExchangeRateDate;
            entity.ExchangeRateSource = dto.ExchangeRateSource;
            entity.ToCurrencyId = dto.ToCurrencyId;
            entity.FromCurrencyId = dto.FromCurrencyId;
            entity.ExchangeRateValidToDate = dto.ExchangeRateDate;
            entity.ExchangeRateValue = dto.ExchangeRateValue;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);
        }





        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static ExchangeRateDto MapToDto(ExchangeRate entity)
        {
            return new ExchangeRateDto
            {
                Id = entity.Id,
                ExchangeRateBaseId = entity.ExchangeRateBaseId,
                ExchangeRateDate = entity.ExchangeRateDate,
                ExchangeRateSource = entity.ExchangeRateSource,
                ToCurrencyId = entity.ToCurrencyId,
                FromCurrencyId = entity.FromCurrencyId,
                ExchangeRateBaseName = entity.ExchangeRateBase.ExchangeRateBaseName,
                ExchangeRateValue = entity.ExchangeRateValue,
                ExchangeRatePersianDate = entity.ExchangeRateDate.getSolarDate10(),
                FromCurrencyName = entity.FromCurrency.CurrencyName,
                ToCurrencyName = entity.ToCurrency.CurrencyName,
                //IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
            };
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "ExchangeRate not found.");

            entity.IsDeleted = true;
            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "ExchangeRate not found.");

            //entity.IsActive = !entity.IsActive;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
        public async Task<decimal> GetFormalPortsExchangeRateValueAsync(int fromCurrId, int toCurrId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var rateRow = await _db.ExchangeRates
                .Where(x => !x.IsDeleted
                         && x.FromCurrencyId == fromCurrId
                         && x.ToCurrencyId == toCurrId
                         && x.ExchangeRateBaseId == 10
                         // مقایسه بازه‌ای (بهینه برای Index)
                         && x.ExchangeRateDate >= startDate
                         && x.ExchangeRateDate < endDate)
                .FirstOrDefaultAsync();

            if (rateRow == null) return 1;
            return rateRow.ExchangeRateValue;
        }
        public async Task<decimal> GetFreeMarketExchangeRateValueAsync(int fromCurrId, int toCurrId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var rateRow = await _db.ExchangeRates
                .Where(x => !x.IsDeleted
                         && x.FromCurrencyId == fromCurrId
                         && x.ToCurrencyId == toCurrId
                         && x.ExchangeRateBaseId == 20
                         && x.ExchangeRateDate >= startDate
                         && x.ExchangeRateDate < endDate)
                .FirstOrDefaultAsync();

            if (rateRow == null) return 1;
            return rateRow.ExchangeRateValue;
        }
    }
}