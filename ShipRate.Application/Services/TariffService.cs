using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Infrastructure.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;
namespace ShipRate.Application.Services
{
    public class TariffService : ITariffService
    {
        private const int CoverageChargeTypeId = 555;

        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public TariffService(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<TariffIndexVm> GetIndexVmAsync(
            TariffBookSearchVm search,
            CancellationToken cancellationToken = default)
        {

            if (search.TariffTypes.Count == 0) search.TariffTypes = await GetTariffTypeSelectListAsync(cancellationToken);
            //if (search.ChargeTypes.Count == 0) search.ChargeTypes = await GetChargeTypeSelectListAsync(tariffTy  cancellationToken);
            if (search.Statuses.Count == 0) search.Statuses = await GetTariffStatusSelectListAsync(cancellationToken);
            return new TariffIndexVm
            {
                Search = search,
                //TariffTypes = await GetTariffTypeSelectListAsync(cancellationToken),
                //ChargeTypes = await GetChargeTypeSelectListAsync(cancellationToken),
                //Statuses = await GetTariffStatusSelectListAsync(cancellationToken)
            };
        }

        public async Task<PagedResultDto<TariffBookListItemVm>> GetTariffBooksAsync(
            TariffBookSearchVm search,
            CancellationToken cancellationToken = default)
        {
            if (search.Page <= 0)
                search.Page = 1;

            if (search.PageSize <= 0)
                search.PageSize = 10;

            var query = _db.TariffBooks
                .AsNoTracking()
                .Where(x => !x.IsDeleted);


            if (search.TariffTypeId.HasValue)
                query = query.Where(x => x.TariffTypeId == search.TariffTypeId.Value);



            if (!string.IsNullOrWhiteSpace(search.SearchText))
            {
                var term = search.SearchText.Trim();

                query = query.Where(x =>
                    x.Code.Contains(term) ||
                    x.Name.Contains(term) ||
                    (x.Description != null && x.Description.Contains(term)) ||
                    (x.Party != null && x.Party.PersonName.Contains(term)) ||
                    (x.TariffType != null && x.TariffType.TariffTypeName.Contains(term)) ||
                    (x.Status != null && x.Status.TariffStatusName.Contains(term)));
            }

            if (search.TariffTypeId.HasValue)
            {
                query = query.Where(x => x.TariffTypeId == search.TariffTypeId.Value);
            }

            if (search.PartyId.HasValue)
            {
                query = query.Where(x => x.PartyId == search.PartyId.Value);
            }

            if (search.StatusId.HasValue)
            {
                query = query.Where(x => x.StatusId == search.StatusId.Value);
            }

            if (search.ChargeTypeId.HasValue)
            {
                query = query.Where(x =>
                    _db.TariffRules.Any(r =>
                        !r.IsDeleted &&
                        r.TariffBookId == x.Id &&
                        r.ChargeTypeId == search.ChargeTypeId.Value));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Code)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new TariffBookListItemVm
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    TariffTypeId = x.TariffTypeId,
                    TariffTypeName = x.TariffType != null ? x.TariffType.TariffTypeName : "",
                    PartyId = x.PartyId,
                    PartyName = x.Party != null ? x.Party.PersonName : "",
                    StatusId = x.StatusId,
                    StatusName = x.Status != null ? x.Status.TariffStatusName : "",
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    //IsActive = x.IsActive,
                    CurrencyId = x.CurrencyId,
                    CurrencyName = x.Currency != null ? x.Currency.CurrencyName : ""
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TariffBookListItemVm>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = search.Page,
                PageSize = search.PageSize
            };
        }

        public async Task<PagedResultDto<TariffRuleListItemVm>> GetTariffRulesAsync(
            Guid tariffBookId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            var query = _db.TariffRules
                .Include(x => x.Origin)
                .Include(x => x.TradeType)
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.TariffBookId == tariffBookId);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.ChargeTypeId)
                .ThenBy(x => x.TradeTypeId)
                .ThenBy(x => x.ContainerSizeTypeId)
                .ThenByDescending(x => x.FullMTId)
                .ThenBy(x => x.MinQuantity)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TariffRuleListItemVm
                {
                    Id = x.Id,
                    TariffBookId = x.TariffBookId,
                    ChargeTypeId = x.ChargeTypeId,
                    ChargeTypeName = x.ChargeType != null ? x.ChargeType.ChargeTypeName : "",
                    CalculationById = x.CalculationById,
                    CalculationByName = x.CalculationBy != null ? x.CalculationBy.CalculationByName : "",
                    CalculationTypeId = x.CalculationTypeId,
                    CalculationTypeName = x.CalculationType != null ? x.CalculationType.CalculationTypeName : "",
                    FromValue = x.MinQuantity,
                    ToValue = x.MaxQuantity,
                    Amount = x.Amount,
                    CurrencyCode = x.Currency.CurrencyCode,
                    EffectiveFrom = x.EffectiveFrom,
                    EffectiveTo = x.EffectiveTo,
                    //IsActive = x.IsActive,
                    ContainerSizeTypeName = x.ContainerSizeType != null ? x.ContainerSizeType.ShortName : "",
                    OriginName = x.Origin != null ? x.Origin.LocationName : "All",
                    DestinationName = x.Destination != null ? x.Destination.LocationName : "All",
                    TradeTypeName = x.TradeType != null ? x.TradeType.TradeTypeName : "-",
                    FullMTName = x.FullMT != null ? x.FullMT.FullMTName : "-",
                    IsDG = x.IsDG != null ? x.IsDG.Value : false,
                    IsOverSized = x.IsOverSized != null ? x.IsOverSized.Value : false,
                    IsOverWeight = x.IsOverWeight != null ? x.IsOverWeight.Value : false
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TariffRuleListItemVm>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<List<TariffRuleCoverageListItemVm>> GetTariffRuleCoveragesAsync(
            Guid tariffRuleId,
            CancellationToken cancellationToken = default)
        {
            return await _db.TariffRuleCoverages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.TariffRuleId == tariffRuleId)
                .OrderBy(x => x.Id)
                .Select(x => new TariffRuleCoverageListItemVm
                {
                    Id = x.Id,
                    TariffRuleId = x.TariffRuleId,
                    //CountryId = x.CountryId,
                    //CountryName = x.Country != null ? x.Country.Name : "",
                    //LocationId = x.LocationId,
                    //LocationName = x.Location != null ? x.Location.Name : "",
                    //CoverageCode = x.CoverageCode,
                    //CoverageName = x.CoverageName,
                    //IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> RuleCanShowCoveragesAsync(
            Guid tariffRuleId,
            CancellationToken cancellationToken = default)
        {
            return await _db.TariffRules
                .AsNoTracking()
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Id == tariffRuleId &&
                    x.ChargeTypeId == CoverageChargeTypeId,
                    cancellationToken);
        }


        public async Task<TariffBookFormVm> GetCreateTariffBookVmAsync(
                CancellationToken cancellationToken = default)
        {
            var model = new TariffBookFormVm
            {
                ValidFrom = DateTime.Today,
                IsActive = true
            };

            await FillTariffBookLookupsAsync(model, cancellationToken);

            return model;
        }

        public async Task<TariffBookFormVm?> GetEditTariffBookVmAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _db.TariffBooks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id)
                .Select(x => new TariffBookFormVm
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    TariffTypeId = x.TariffTypeId,
                    PartyId = x.PartyId,
                    PartyDisplayName = x.Party != null ? x.Party.PersonName : "",
                    StatusId = x.StatusId,
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    //IsActive = x.IsActive,
                    CurrencyId = x.CurrencyId,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                return null;

            await FillTariffBookLookupsAsync(entity, cancellationToken);

            return entity;
        }

        public async Task<Guid> CreateTariffBookAsync(
            TariffBookFormVm model,
            CancellationToken cancellationToken = default)
        {
            var entity = new TariffBook
            {
                Id = Guid.CreateVersion7(),
                Code = model.Code.Trim(),
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                TariffTypeId = model.TariffTypeId,
                PartyId = model.PartyId,
                StatusId = -1,  //Draft
                ValidFrom = model.ValidFrom,
                ValidTo = model.ValidTo,
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                CurrencyId = model.CurrencyId,
            };

            _db.TariffBooks.Add(entity);

            await _db.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task<bool> UpdateTariffBookAsync(
            TariffBookFormVm model,
            CancellationToken cancellationToken = default)
        {
            if (!model.Id.HasValue)
                return false;

            var entity = await _db.TariffBooks
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id.Value, cancellationToken);

            if (entity == null)
                return false;

            entity.Code = model.Code.Trim();
            entity.Name = model.Name.Trim();
            entity.Description = model.Description?.Trim();
            entity.TariffTypeId = model.TariffTypeId;
            entity.PartyId = model.PartyId;
            entity.ValidFrom = model.ValidFrom;
            entity.ValidTo = model.ValidTo;
            //entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            entity.CurrencyId = model.CurrencyId;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SoftDeleteTariffBookAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _db.TariffBooks
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, cancellationToken);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;

            var rules = await _db.TariffRules
                .Where(x => !x.IsDeleted && x.TariffBookId == id)
                .ToListAsync(cancellationToken);

            foreach (var rule in rules)
            {
                rule.IsDeleted = true;
                //rule.IsActive = false;
                rule.UpdatedAt = DateTime.Now;
            }

            var ruleIds = rules.Select(x => x.Id).ToList();

            var coverages = await _db.TariffRuleCoverages
                .Where(x => !x.IsDeleted && ruleIds.Contains(x.TariffRuleId))
                .ToListAsync(cancellationToken);

            foreach (var coverage in coverages)
            {
                coverage.IsDeleted = true;
                //coverage.IsActive = false;
                coverage.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }


        public async Task<TariffRuleFormVm> GetCreateTariffRuleVmAsync(
                int tariffTypeId,
                Guid tariffBookId,
                CancellationToken cancellationToken = default)
        {
            var model = new TariffRuleFormVm
            {
                TariffBookId = tariffBookId,
                EffectiveFrom = DateTime.Today,
                IsActive = true
            };
            var book = await GetEditTariffBookVmAsync(tariffBookId, cancellationToken);
            model.CurrencyId = book.CurrencyId;
            model.EffectiveFrom = book.ValidFrom;
            model.EffectiveTo = book.ValidTo;
            await FillTariffRuleLookupsAsync(tariffTypeId, model, cancellationToken);
            return model;
        }

        public async Task<TariffRuleFormVm?> GetEditTariffRuleVmAsync(
            int tariffTypeId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _db.TariffRules
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id)
                .Select(x => new TariffRuleFormVm
                {
                    Id = x.Id,
                    TariffBookId = x.TariffBookId,
                    ChargeTypeId = x.ChargeTypeId,
                    CalculationById = x.CalculationById,
                    CalculationTypeId = x.CalculationTypeId,
                    FromValue = x.MinQuantity,
                    ToValue = x.MaxQuantity,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    EffectiveFrom = x.EffectiveFrom,
                    EffectiveTo = x.EffectiveTo,
                    //IsActive = x.IsActive,
                    ExtraAttributes = x.ExtraAttributes,
                    ContainerSizeId = x.ContainerSizeId,
                    ContainerSizeTypeId = x.ContainerSizeTypeId,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                return null;

            await FillTariffRuleLookupsAsync(tariffTypeId, entity, cancellationToken);

            return entity;
        }

        public async Task<Guid> CreateTariffRuleAsync(
            TariffRuleFormVm model,
            CancellationToken cancellationToken = default)
        {
            var entity = new TariffRule
            {
                Id = Guid.CreateVersion7(),
                TariffBookId = model.TariffBookId,
                ChargeTypeId = model.ChargeTypeId,
                CalculationById = model.CalculationById.Value,
                CalculationTypeId = model.CalculationTypeId.Value,
                MinQuantity = model.FromValue,
                MaxQuantity = model.ToValue,
                Amount = model.Amount,
                CurrencyId = model.CurrencyId,
                EffectiveFrom = model.EffectiveFrom,
                EffectiveTo = model.EffectiveTo,
                //IsActive = model.IsActive,
                IsDeleted = false,
                ExtraAttributes = model.ExtraAttributes,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                ContainerSizeId = model.ContainerSizeId,
                ContainerSizeTypeId = model.ContainerSizeTypeId,
            };

            _db.TariffRules.Add(entity);

            await _db.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task<bool> UpdateTariffRuleAsync(
            TariffRuleFormVm model,
            CancellationToken cancellationToken = default)
        {
            if (!model.Id.HasValue)
                return false;

            var entity = await _db.TariffRules
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id.Value, cancellationToken);

            if (entity == null)
                return false;

            entity.ChargeTypeId = model.ChargeTypeId;
            entity.CalculationById = model.CalculationById.Value;
            entity.CalculationTypeId = model.CalculationTypeId.Value;
            entity.MinQuantity = model.FromValue;
            entity.MaxQuantity = model.ToValue;
            entity.Amount = model.Amount;
            entity.CurrencyId = model.CurrencyId;
            entity.EffectiveFrom = model.EffectiveFrom;
            entity.EffectiveTo = model.EffectiveTo;
            //entity.IsActive = model.IsActive;
            entity.ExtraAttributes = model.ExtraAttributes;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            entity.ContainerSizeId = model.ContainerSizeId;
            entity.ContainerSizeTypeId = model.ContainerSizeTypeId;
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> SoftDeleteTariffRuleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _db.TariffRules
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, cancellationToken);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;

            var coverages = await _db.TariffRuleCoverages
                .Where(x => !x.IsDeleted && x.TariffRuleId == id)
                .ToListAsync(cancellationToken);

            foreach (var coverage in coverages)
            {
                coverage.IsDeleted = true;
                //coverage.IsActive = false;
                coverage.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<TariffRuleCoverageFormVm> GetCreateCoverageVmAsync(
                Guid tariffRuleId,
                CancellationToken cancellationToken = default)
        {
            var model = new TariffRuleCoverageFormVm
            {
                TariffRuleId = tariffRuleId,
                IsActive = true
            };

            await FillCoverageLookupsAsync(model, cancellationToken);

            return model;
        }

        public async Task<TariffRuleCoverageFormVm?> GetEditCoverageVmAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _db.TariffRuleCoverages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id)
                .Select(x => new TariffRuleCoverageFormVm
                {
                    Id = x.Id,
                    TariffRuleId = x.TariffRuleId,
                    //CountryId = x.CountryId,
                    //LocationId = x.LocationId,
                    //CoverageCode = x.CoverageCode,
                    //CoverageName = x.CoverageName,
                    //IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                return null;

            await FillCoverageLookupsAsync(entity, cancellationToken);

            return entity;
        }

        public async Task<Guid> CreateCoverageAsync(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken = default)
        {
            var entity = new TariffRuleCoverage
            {
                Id = Guid.CreateVersion7(),
                TariffRuleId = model.TariffRuleId,
                //CountryId = model.CountryId,
                //LocationId = model.LocationId,
                //CoverageCode = model.CoverageCode?.Trim(),
                //CoverageName = model.CoverageName?.Trim(),
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _db.TariffRuleCoverages.Add(entity);

            await _db.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task<bool> UpdateCoverageAsync(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken = default)
        {
            if (!model.Id.HasValue)
                return false;

            var entity = await _db.TariffRuleCoverages
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id.Value, cancellationToken);

            if (entity == null)
                return false;

            //entity.CountryId = model.CountryId;
            //entity.LocationId = model.LocationId;
            //entity.CoverageCode = model.CoverageCode?.Trim();
            //entity.CoverageName = model.CoverageName?.Trim();
            //entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> SoftDeleteCoverageAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _db.TariffRuleCoverages
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, cancellationToken);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<PartyLookupItemVm>> SearchPartiesAsync(
                string? term,
                CancellationToken cancellationToken = default)
        {
            term = term?.Trim();

            var query = _db.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.PersonName.Contains(term) ||
                    x.PersonPersianName.Contains(term));
            }

            return await query
                .OrderBy(x => x.PersonName)
                .Take(20)
                .Select(x => new PartyLookupItemVm
                {
                    Id = x.Id,
                    DisplayName = x.PersonName + " - " + x.PersonPersianName
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<List<SelectListItem>> GetTariffTypeSelectListAsync(
                CancellationToken cancellationToken = default)
        {
            return await _db.TariffTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TariffTypeName + " - " + x.TariffTypeLocalName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SelectListItem>> GetChargeTypeSelectListAsync(int tariffTypeId,
            CancellationToken cancellationToken = default)
        {

            var query = _db.ChargeTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive);

            if (tariffTypeId == 2)   //Local Charges
            {
                query = query.Where(x => x.ChargeTypeCategoryId == 2);
            }
            else if (tariffTypeId == 3)   //DETENTION
            {
                query = query.Where(x => x.Id == 160);
            }

            var list = await query
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName + "-" + x.ChargeTypeLocalName
                })
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<List<SelectListItem>> GetTariffStatusSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.TariffStatus
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.TariffStatusName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TariffStatusName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetCalculationBySelectListAsync(int tariffTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _db.CalculationBy
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.CalculationByName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CalculationByName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetCalculationTypeSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.CalculationTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CalculationTypeName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetCountrySelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.Countries
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.CountryName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CountryName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetLocationSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync(cancellationToken);
        }



        private async Task<List<SelectListItem>> GetContainerSizesListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.ContainerSizes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ContainerSizeName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SelectListItem>> GetContainerSizeTypesListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.ContainerSizeTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShortName
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<SelectListItem>> GetFullMTsListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.FullMT
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderByDescending(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync(cancellationToken);
        }



        private async Task<List<SelectListItem>> GetCurrencySelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.Currencies
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode
                })
                .ToListAsync(cancellationToken);
        }

        private async Task FillTariffBookLookupsAsync(
                TariffBookFormVm model,
                CancellationToken cancellationToken = default)
        {
            model.Currencies = await GetCurrencySelectListAsync(cancellationToken);
        }

        private async Task FillTariffRuleLookupsAsync(
            int tariffTypeId,
            TariffRuleFormVm model,
            CancellationToken cancellationToken = default)
        {
            model.ChargeTypes = await GetChargeTypeSelectListAsync(tariffTypeId, cancellationToken);




            model.CalculationByItems = await GetCalculationBySelectListAsync(tariffTypeId, cancellationToken);
            model.CalculationTypes = await GetCalculationTypeSelectListAsync(cancellationToken);
            model.Currencies = await GetCurrencySelectListAsync(cancellationToken);

            model.ContainerSizes = await GetContainerSizesListAsync(cancellationToken);
            model.ContainerSizeTypes = await GetContainerSizeTypesListAsync(cancellationToken);

            //model.ChargeTypes.Insert(0, new SelectListItem
            //{
            //    Value = "",
            //    Text = "-- Select Charge Type --"
            //});

            //model.CalculationByItems.Insert(0, new SelectListItem
            //{
            //    Value = "",
            //    Text = "-- Select Calculation By --"
            //});

            //model.CalculationTypes.Insert(0, new SelectListItem
            //{
            //    Value = "",
            //    Text = "-- Select Calculation Type --"
            //});
        }

        private async Task FillCoverageLookupsAsync(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken = default)
        {
            model.Countries = await GetCountrySelectListAsync(cancellationToken);
            model.Locations = await GetLocationSelectListAsync(cancellationToken);

            //model.Countries.Insert(0, new SelectListItem
            //{
            //    Value = "",
            //    Text = "-- Select Country --"
            //});

            //model.Locations.Insert(0, new SelectListItem
            //{
            //    Value = "",
            //    Text = "-- Select Location --"
            //});
        }



        public async Task ReplaceDemurrageRulesAsync(DemurrageRuleBatchFormVm vm)
        {
            var tariffBook = await _db.TariffBooks
                .FirstAsync(x => x.Id == vm.TariffBookId);

            var existingRules = await _db.TariffRules
                .Where(x =>
                    x.TariffBookId == vm.TariffBookId &&
                    x.ContainerSizeTypeId == vm.ContainerSizeTypeId &&
                    x.ChargeTypeId == 25 &&
                    x.CalculationById == 10 &&
                    x.CalculationTypeId == 2 &&
                    !x.IsDeleted)
                .ToListAsync();

            foreach (var rule in existingRules)
            {
                rule.IsDeleted = true;
                //rule.IsActive = false;
            }

            foreach (var step in vm.Steps.OrderBy(x => x.From))
            {
                _db.TariffRules.Add(new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    ContainerSizeTypeId = vm.ContainerSizeTypeId,

                    ChargeTypeId = 25,
                    CalculationById = 10,
                    CalculationTypeId = 2,

                    CurrencyId = tariffBook.CurrencyId,
                    EffectiveFrom = tariffBook.ValidFrom,
                    EffectiveTo = tariffBook.ValidTo,

                    MinQuantity = step.From,
                    MaxQuantity = step.To,
                    Amount = step.Amount,

                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,

                    //IsActive = true,
                    IsDeleted = false
                });
            }

            await _db.SaveChangesAsync();
        }



        public async Task<DemurrageRuleBatchFormVm> BuildDemurrageRuleBatchFormAsync(
            Guid tariffBookId,
            int? containerSizeTypeId = null)
        {
            var tariffBook = await _db.TariffBooks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == tariffBookId);

            if (tariffBook == null)
                throw new InvalidOperationException("Tariff book not found.");

            if (tariffBook.TariffTypeId != 3)
                throw new InvalidOperationException("Selected tariff book is not a demurrage tariff book.");

            var vm = new DemurrageRuleBatchFormVm
            {
                TariffBookId = tariffBookId,
                ContainerSizeTypeId = containerSizeTypeId,
                TariffBookTitle = tariffBook.Name,
                CurrencyTitle = tariffBook.Currency?.CurrencyCode + "-" + tariffBook.Currency?.CurrencyName,
                ValidFrom = tariffBook.ValidFrom,
                ValidTo = tariffBook.ValidTo,
                ContainerSizeTypes = await _db.ContainerSizeTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.ShortName
                    })
                    .ToListAsync()
            };

            if (!containerSizeTypeId.HasValue)
                return vm;

            vm.Steps = await _db.TariffRules
                .AsNoTracking()
                .Where(x =>
                    x.TariffBookId == tariffBookId &&
                    x.ContainerSizeTypeId == containerSizeTypeId.Value &&
                    x.ChargeTypeId == 25 &&
                    x.CalculationById == 10 &&
                    x.CalculationTypeId == 2 &&
                    !x.IsDeleted)
                .OrderBy(x => x.MinQuantity)
                .Select(x => new DemurrageRuleStepVm
                {
                    From = x.MinQuantity ?? 0,
                    To = x.MaxQuantity ?? 0,
                    Amount = x.Amount
                })
                .ToListAsync();

            if (!vm.Steps.Any())
            {
                vm.Steps.Add(new DemurrageRuleStepVm
                {
                    From = 1,
                    To = 9999,
                    Amount = 0
                });
            }

            return vm;
        }
        public async Task<int> GetTariffTypeIdAsync(Guid tariffBookId, CancellationToken cancellationToken = default)
        {
            var tariffbook = await _db.TariffBooks.Where(x => x.Id == tariffBookId).FirstOrDefaultAsync();
            return tariffbook.TariffTypeId;
        }

        public async Task<string> GetTariffTitleAsync(Guid tariffBookId, CancellationToken cancellationToken = default)
        {
            var tariffbook = await _db.TariffBooks.Where(x => x.Id == tariffBookId).FirstOrDefaultAsync();
            return string.Format("Code:{0} Title:{1}", tariffbook.Code, tariffbook.Name);
        }

        public async Task<OceanFreightRuleBatchFormVm> BuildOceanFreightRuleBatchFormAsync(
            Guid tariffBookId,
            Guid? originId = null,
            Guid? destinationId = null,
            Guid? partyId = null,
            OceanFreightRuleBatchFormVm? postedModel = null)
        {
            var model = postedModel ?? new OceanFreightRuleBatchFormVm();

            var tariffBook = await _db.TariffBooks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == tariffBookId);

            if (tariffBook == null)
                throw new Exception("Tariff book not found.");

            model.TariffBookId = tariffBook.Id;
            model.TariffTypeId = 1;

            model.TariffBookTitle = tariffBook.Name;
            model.CurrencyId = tariffBook.CurrencyId;
            model.EffectiveFrom = tariffBook.ValidFrom;
            model.EffectiveTo = tariffBook.ValidTo;

            model.OriginId = originId ?? model.OriginId;
            model.DestinationId = destinationId ?? model.DestinationId;
            model.PartyId = partyId ?? model.PartyId;

            await PopulateOceanFreightRuleBatchLookupsAsync(model);

            if (postedModel == null &&
                model.OriginId.HasValue &&
                model.DestinationId.HasValue &&
                model.PartyId.HasValue)
            {
                model.Rows = await _db.TariffRules
                    .AsNoTracking()
                    .Where(x =>
                        x.TariffBookId == tariffBookId &&
                        x.OriginId == model.OriginId &&
                        x.DestinationId == model.DestinationId &&
                        x.PartyId == model.PartyId &&
                        x.IsDeleted == false)
                    .OrderBy(x => x.ContainerSizeTypeId)
                    .ThenBy(x => x.ChargeTypeId)
                    .Select(x => new OceanFreightRateRowVm
                    {
                        ContainerSizeTypeId = x.ContainerSizeTypeId,
                        ChargeTypeId = x.ChargeTypeId,
                        Amount = x.Amount,
                        ExtraAttributes = x.ExtraAttributes
                    })
                    .ToListAsync();
            }

            if (model.Rows == null || !model.Rows.Any())
            {
                model.Rows = new List<OceanFreightRateRowVm>
        {
            new OceanFreightRateRowVm()
        };
            }

            return model;
        }


        public async Task<OceanFreightRuleBatchFormVm> AddOceanFreightRuleRowAsync(
            OceanFreightRuleBatchFormVm model)
        {
            model.Rows ??= new List<OceanFreightRateRowVm>();
            model.Rows.Add(new OceanFreightRateRowVm());

            await PopulateOceanFreightRuleBatchLookupsAsync(model);

            return model;
        }

        public async Task<OceanFreightRuleBatchFormVm> RemoveOceanFreightRuleRowAsync(
            OceanFreightRuleBatchFormVm model,
            int rowIndex)
        {
            model.Rows ??= new List<OceanFreightRateRowVm>();

            if (rowIndex >= 0 && rowIndex < model.Rows.Count)
                model.Rows.RemoveAt(rowIndex);

            if (!model.Rows.Any())
                model.Rows.Add(new OceanFreightRateRowVm());

            await PopulateOceanFreightRuleBatchLookupsAsync(model);

            return model;
        }

        public async Task<OceanFreightRuleBatchFormVm> PasteOceanFreightRuleRowsAsync(
            OceanFreightRuleBatchFormVm model)
        {
            await PopulateOceanFreightRuleBatchLookupsAsync(model);

            model.Rows ??= new List<OceanFreightRateRowVm>();

            var pastedRows = ParseOceanFreightRows(model.PastedRowsText);

            model.Rows.AddRange(pastedRows);

            model.Rows = model.Rows
                .Where(x =>
                    x.ContainerSizeTypeId.HasValue ||
                    x.ChargeTypeId.HasValue ||
                    x.Amount.HasValue ||
                    !string.IsNullOrWhiteSpace(x.ExtraAttributes))
                .ToList();

            if (!model.Rows.Any())
                model.Rows.Add(new OceanFreightRateRowVm());

            return model;
        }




        public async Task<ServiceResult> SaveOceanFreightRulesAsync(
            OceanFreightRuleBatchFormVm model,
            Guid currentUserId)
        {
            var result = ValidateOceanFreightRules(model);

            if (!result.Succeeded)
                return result;

            var tariffBookExists = await _db.TariffBooks
                .AsNoTracking()
                .AnyAsync(x => x.Id == model.TariffBookId);

            if (!tariffBookExists)
                return ServiceResult.Failure("", "Tariff book not found.");

            var oldRules = await _db.TariffRules
                .Where(x =>
                    x.TariffBookId == model.TariffBookId &&
                    x.OriginId == model.OriginId &&
                    x.DestinationId == model.DestinationId &&
                    x.PartyId == model.PartyId &&
                    x.ScopeTypeId == model.ScopeTypeId &&
                    x.IsDeleted == false)
                .ToListAsync();

            foreach (var rule in oldRules)
            {
                rule.IsDeleted = true;
                //rule.IsActive = false;
                rule.DeletedAt = DateTime.Now;
                rule.DeleteUserId = _currentUserService.UserId;
                rule.UpdatedAt = DateTime.Now;
                rule.EditUserId = _currentUserService.UserId;
            }

            var rows = model.Rows
                .Where(x =>
                    x.ContainerSizeTypeId.HasValue &&
                    x.ChargeTypeId.HasValue &&
                    x.Amount.HasValue)
                .ToList();

            foreach (var row in rows)
            {
                var rule = new TariffRule
                {
                    TariffBookId = model.TariffBookId,
                    ChargeTypeId = row.ChargeTypeId.Value,

                    OriginCountryId = model.OriginCountryId,
                    DestinationCountryId = model.DestinationCountryId,
                    OriginId = model.OriginId,
                    DestinationId = model.DestinationId,
                    PartyId = model.PartyId,

                    CalculationById = 1,      // TODO: مقدار واقعی Container
                    CalculationTypeId = 1,    // TODO: مقدار واقعی Per Container / Fixed

                    ContainerSizeTypeId = row.ContainerSizeTypeId,

                    Amount = row.Amount.Value,
                    CurrencyId = model.CurrencyId,

                    EffectiveFrom = model.EffectiveFrom.Value,
                    EffectiveTo = model.EffectiveTo,

                    ExtraAttributes = row.ExtraAttributes,
                    ScopeTypeId = model.ScopeTypeId,

                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };

                _db.TariffRules.Add(rule);
            }

            await _db.SaveChangesAsync();

            return ServiceResult.Success("Ocean freight rates saved successfully.");
        }
        private ServiceResult ValidateOceanFreightRules(OceanFreightRuleBatchFormVm model)
        {
            var result = new ServiceResult
            {
                Succeeded = true
            };

            if (!model.OriginId.HasValue)
                result.AddError(nameof(model.OriginId), "Origin is required.");

            if (!model.DestinationId.HasValue)
                result.AddError(nameof(model.DestinationId), "Destination is required.");

            if (model.OriginId.HasValue &&
                model.DestinationId.HasValue &&
                model.OriginId.Value == model.DestinationId.Value)
                result.AddError(nameof(model.DestinationId), "Origin and destination cannot be the same.");

            if (!model.PartyId.HasValue)
                result.AddError(nameof(model.PartyId), "Carrier / shipping line is required.");

            if (!model.ScopeTypeId.HasValue)
                result.AddError(nameof(model.ScopeTypeId), "Scope type is required.");

            if (!model.CurrencyId.HasValue)
                result.AddError(nameof(model.CurrencyId), "Currency is required.");

            if (!model.EffectiveFrom.HasValue)
                result.AddError(nameof(model.EffectiveFrom), "Effective from is required.");

            model.Rows ??= new List<OceanFreightRateRowVm>();

            var validRows = model.Rows
                .Where(x =>
                    x.ContainerSizeTypeId.HasValue ||
                    x.ChargeTypeId.HasValue ||
                    x.Amount.HasValue ||
                    !string.IsNullOrWhiteSpace(x.ExtraAttributes))
                .ToList();

            if (!validRows.Any())
                result.AddError("", "At least one rate row is required.");

            for (var i = 0; i < model.Rows.Count; i++)
            {
                var row = model.Rows[i];

                var hasAnyValue =
                    row.ContainerSizeTypeId.HasValue ||
                    row.ChargeTypeId.HasValue ||
                    row.Amount.HasValue ||
                    !string.IsNullOrWhiteSpace(row.ExtraAttributes);

                if (!hasAnyValue)
                    continue;

                if (!row.ContainerSizeTypeId.HasValue)
                    result.AddError($"Rows[{i}].ContainerSizeTypeId", "Container size type is required.");

                if (!row.ChargeTypeId.HasValue)
                    result.AddError($"Rows[{i}].ChargeTypeId", "Charge type is required.");

                if (!row.Amount.HasValue)
                    result.AddError($"Rows[{i}].Amount", "Amount is required.");

                if (row.Amount.HasValue && row.Amount.Value < 0)
                    result.AddError($"Rows[{i}].Amount", "Amount cannot be negative.");
            }

            var duplicates = validRows
                .Where(x => x.ContainerSizeTypeId.HasValue && x.ChargeTypeId.HasValue)
                .GroupBy(x => new { x.ContainerSizeTypeId, x.ChargeTypeId })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
                result.AddError("", "Duplicate container and charge type rows are not allowed.");

            return result;
        }
        public async Task PopulateOceanFreightRuleBatchLookupsAsync(
            OceanFreightRuleBatchFormVm model)
        {
            model.Origins = await _db.Locations
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();

            model.Destinations = model.Origins.ToList();

            model.Parties = await _db.Persons
                .AsNoTracking()
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();

            model.ScopeTypes = await _db.ScopeTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ScopeTypeName
                })
                .ToListAsync();

            model.ContainerSizeTypes = await _db.ContainerSizeTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.ShortName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShortName
                })
                .ToListAsync();

            model.ChargeTypes = await _db.ChargeTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync();

            if (model.CurrencyId.HasValue)
            {
                model.CurrencyTitle = await _db.Currencies
                    .AsNoTracking()
                    .Where(x => x.Id == model.CurrencyId.Value)
                    .Select(x => x.CurrencyCode)
                    .FirstOrDefaultAsync();
            }
        }


        private List<OceanFreightRateRowVm> ParseOceanFreightRows(string? pastedRows)
        {
            var result = new List<OceanFreightRateRowVm>();

            if (string.IsNullOrWhiteSpace(pastedRows))
                return result;

            var lines = pastedRows.Split(
                new[] { "\r\n", "\n", "\r" },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var columns = line.Split('\t');

                if (columns.Length < 3)
                    columns = line.Split(',', StringSplitOptions.TrimEntries);

                if (columns.Length < 3)
                    continue;

                int.TryParse(columns[0].Trim(), out var containerSizeTypeId);
                int.TryParse(columns[1].Trim(), out var chargeTypeId);
                decimal.TryParse(columns[2].Trim(), out var amount);

                result.Add(new OceanFreightRateRowVm
                {
                    ContainerSizeTypeId = containerSizeTypeId > 0 ? containerSizeTypeId : null,
                    ChargeTypeId = chargeTypeId > 0 ? chargeTypeId : null,
                    Amount = amount,
                    ExtraAttributes = columns.Length > 3 ? columns[3].Trim() : null
                });
            }

            return result;
        }


        public async Task<OceanFreightRuleBatchFormVm> LoadOceanFreightRulesForHeaderAsync(
            OceanFreightRuleBatchFormVm model)
        {
            await PopulateOceanFreightRuleBatchLookupsAsync(model);

            model.Rows = new List<OceanFreightRateRowVm>();

            var hasCompleteHeader =
                model.OriginId.HasValue &&
                model.DestinationId.HasValue &&
                model.PartyId.HasValue &&
                model.ScopeTypeId.HasValue;

            if (!hasCompleteHeader)
            {
                model.Rows.Add(new OceanFreightRateRowVm());
                return model;
            }

            model.Rows = await _db.TariffRules
                .AsNoTracking()
                .Where(x =>
                    x.TariffBookId == model.TariffBookId &&
                    x.OriginId == model.OriginId &&
                    x.DestinationId == model.DestinationId &&
                    x.PartyId == model.PartyId &&
                    x.ScopeTypeId == model.ScopeTypeId &&
                    x.IsDeleted == false)
                .OrderBy(x => x.ContainerSizeTypeId)
                .ThenBy(x => x.ChargeTypeId)
                .Select(x => new OceanFreightRateRowVm
                {
                    ContainerSizeTypeId = x.ContainerSizeTypeId,
                    ChargeTypeId = x.ChargeTypeId,
                    Amount = x.Amount,
                    ExtraAttributes = x.ExtraAttributes
                })
                .ToListAsync();

            if (!model.Rows.Any())
                model.Rows.Add(new OceanFreightRateRowVm());

            return model;
        }
        public async Task<bool> ActivateTariffBookAsync(Guid tariffBookId)
        {
            var tariffBook = await _db.TariffBooks.FindAsync(tariffBookId);
            if (tariffBook == null) return false;
            tariffBook.StatusId = 1;
            await _db.SaveChangesAsync();
            return true;
        }



        public async Task<TariffThcRuleBatchFormVm> BuildThcRuleBatchFormAsync(
            Guid tariffBookId,
            CancellationToken cancellationToken = default)
        {
            var tariffBook = await _db.TariffBooks
                .AsNoTracking()
                .Include(x => x.TariffRules.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(x => x.Id == tariffBookId, cancellationToken);

            if (tariffBook == null)
                throw new KeyNotFoundException($"TariffBook with Id '{tariffBookId}' not found.");

            var tradeTypesList = await GetTradeTypesSelectListAsync(cancellationToken);
            //var allowedTradeTypeIds = new HashSet<string> { "1", "2", "3", "4" };
            //tradeTypesList = tradeTypesList
            //    .Where(x => allowedTradeTypeIds.Contains(x.Value))
            //    .ToList();

            var currenciesList = await GetCurrenciesSelectListAsync(cancellationToken);
            var containerSizesList = await GetContainerSizeTypesListAsync(cancellationToken);
            var fullMTsList = await GetFullMTsListAsync(cancellationToken);

            var vm = new TariffThcRuleBatchFormVm
            {
                TariffBookId = tariffBook.Id,
                TariffBookTitle = $"{tariffBook.Code} - {tariffBook.Name}",
                CurrencyId = tariffBook.CurrencyId,
                TradeTypes = tradeTypesList,
                Currencies = currenciesList,
                ContainerSizeTypes = containerSizesList,
                FullMTs = fullMTsList
            };

            var existingRules = tariffBook.TariffRules.ToList();
            if (existingRules.Any())
            {
                var first = existingRules.First();
                vm.LocationId = first.OriginId ?? first.DestinationId;
                vm.CurrencyId = first.CurrencyId ?? tariffBook.CurrencyId;

                // اگر لوکیشن وجود دارد نام آن را هم برای اینپوت جستجو بخوانید
                if (vm.LocationId.HasValue)
                {
                    var loc = await _db.Locations.AsNoTracking()
                        .FirstOrDefaultAsync(l => l.Id == vm.LocationId.Value, cancellationToken);
                    vm.LocationName = loc != null ? $"{loc.LocationCode} - {loc.LocationName}" : null;
                }

                vm.Rows = existingRules.OrderBy(x => x.TradeTypeId).ThenBy(x => x.ContainerSizeTypeId).ThenByDescending(x => x.FullMTId).Select(r => new TariffThcRuleRowVm
                {
                    Id = r.Id,
                    TradeTypeId = r.TradeTypeId,
                    ContainerSizeTypeId = r.ContainerSizeTypeId,
                    FullMTId = r.FullMTId,
                    IsDG = r.IsDG ?? false,
                    IsOverSized = r.IsOverSized ?? false,
                    IsOverWeight = r.IsOverWeight ?? false,
                    Amount = r.Amount,
                    Remarks = r.ExtraAttributes
                }).ToList();
            }
            else
            {
                // پیش‌فرض: ایجاد ردیف‌های ماتریسی برای هر TradeType × هر سایز کانتینر
                var containerSizes = await _db.ContainerSizeTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .ToListAsync(cancellationToken);
                var fullMTs = await _db.FullMT
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .ToListAsync(cancellationToken);


                var rows = new List<TariffThcRuleRowVm>();

                var allowedTradeTypeIds = new HashSet<string> { "1", "2", "3", "4" };
                tradeTypesList = tradeTypesList
                    .Where(x => allowedTradeTypeIds.Contains(x.Value))
                    .ToList();



                // ایجاد ردیف به ازای هر نوع تجارت موجود (مثلاً Import و Export)
                foreach (var trade in tradeTypesList)
                {
                    if (int.TryParse(trade.Value, out var tradeId))
                    {
                        foreach (var c in containerSizes)
                        {
                            foreach (var f in fullMTs)
                            {
                                rows.Add(new TariffThcRuleRowVm
                                {
                                    TradeTypeId = tradeId,
                                    ContainerSizeTypeId = c.Id,
                                    ContainerSizeTypeName = c.ShortName,
                                    FullMTId = f.Id,
                                    FullMTName = f.FullMTName,
                                    Amount = 0,
                                    IsDG = false,
                                    IsOverSized = false,
                                    IsOverWeight = false
                                });
                            }
                        }
                    }
                }

                vm.Rows = rows;
            }

            return vm;
        }

        public async Task<TariffThcRuleBatchFormVm> RebuildThcRuleBatchFormAsync(
            TariffThcRuleBatchFormVm model,
            CancellationToken cancellationToken = default)
        {
            model.TradeTypes = await GetTradeTypesSelectListAsync(cancellationToken);
            model.Currencies = await GetCurrenciesSelectListAsync(cancellationToken);
            model.ContainerSizeTypes = await GetContainerSizeTypesListAsync(cancellationToken);
            model.FullMTs = await GetFullMTsListAsync(cancellationToken);

            var tariffBook = await _db.TariffBooks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == model.TariffBookId, cancellationToken);

            if (tariffBook != null)
            {
                model.TariffBookTitle = $"{tariffBook.Code} - {tariffBook.Name}";
            }

            return model;
        }

        public List<TariffThcRuleRowVm> ParseThcRulePastedText(string? pastedText)
        {
            var result = new List<TariffThcRuleRowVm>();
            if (string.IsNullOrWhiteSpace(pastedText))
                return result;

            var lines = pastedText
                .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l));

            foreach (var line in lines)
            {
                // فرمت اکسل: [TradeTypeId, ContainerSizeTypeId, Amount, DG(0/1), OOG(0/1), OW(0/1), Remarks]
                var parts = line.Split(new[] { '\t', ';', ',' }, StringSplitOptions.None)
                                .Select(p => p.Trim())
                                .ToArray();

                if (parts.Length == 0) continue;

                var row = new TariffThcRuleRowVm();

                // 1. TradeTypeId
                if (parts.Length > 0 && int.TryParse(parts[0], out var tradeTypeId))
                    row.TradeTypeId = tradeTypeId;

                // 2. ContainerSizeTypeId
                if (parts.Length > 1 && int.TryParse(parts[1], out var sizeTypeId))
                    row.ContainerSizeTypeId = sizeTypeId;



                // 3. Amount
                if (parts.Length > 2 && decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                    row.Amount = amount;

                // 4. Flags (DG, OOG, OW)
                if (parts.Length > 3)
                    row.IsDG = parts[3] == "1" || parts[3].Equals("true", StringComparison.OrdinalIgnoreCase);

                if (parts.Length > 4)
                    row.IsOverSized = parts[4] == "1" || parts[4].Equals("true", StringComparison.OrdinalIgnoreCase);

                if (parts.Length > 5)
                    row.IsOverWeight = parts[5] == "1" || parts[5].Equals("true", StringComparison.OrdinalIgnoreCase);

                // 5. Remarks
                if (parts.Length > 6)
                    row.Remarks = parts[6];

                result.Add(row);
            }

            return result;
        }

        public async Task<bool> SaveThcRulesAsync(
            TariffThcRuleBatchFormVm model,
            CancellationToken cancellationToken = default)
        {
            var tariffBook = await _db.TariffBooks
                .Include(x => x.TariffRules)
                .FirstOrDefaultAsync(x => x.Id == model.TariffBookId, cancellationToken);

            if (tariffBook == null)
                return false;

            var chargeTypeId = 155; // Default THC Charge Type
            var currentUserId = _currentUserService.UserId;
            var now = DateTime.Now;

            // فیلتر ردیف‌های معتبر که فیلدهای الزامی دارند
            var validRows = model.Rows?
                .Where(r => r.TradeTypeId.HasValue && r.ContainerSizeTypeId.HasValue && r.FullMTId.HasValue && r.Amount > 0)
                .ToList() ?? new List<TariffThcRuleRowVm>();

            var incomingRuleIds = validRows
                .Where(r => r.Id.HasValue && r.Id.Value != Guid.Empty)
                .Select(r => r.Id!.Value)
                .ToHashSet();

            // 1. مدیریت حذف نرم (Soft Delete) برای قوانینی که از فرم حذف شده‌اند
            var existingRules = tariffBook.TariffRules.Where(r => !r.IsDeleted).ToList();
            foreach (var rule in existingRules)
            {
                if (!incomingRuleIds.Contains(rule.Id))
                {
                    rule.IsDeleted = true;
                    rule.DeletedAt = now;
                    rule.DeleteUserId = currentUserId;
                }
            }

            // 2. افزودن یا به‌روزرسانی ردیف‌ها
            foreach (var row in validRows)
            {
                TariffRule? rule = null;

                if (row.Id.HasValue && row.Id.Value != Guid.Empty)
                {
                    rule = existingRules.FirstOrDefault(r => r.Id == row.Id.Value);
                }

                if (rule == null)
                {
                    rule = new TariffRule
                    {
                        Id = Guid.CreateVersion7(),
                        TariffBookId = tariffBook.Id,
                        CreatedAt = now,
                        CreateUserId = currentUserId
                    };
                    _db.TariffRules.Add(rule);
                }
                else
                {
                    rule.UpdatedAt = now;
                    rule.EditUserId = currentUserId;
                }

                // انتساب مقادیر هدر و ردیف
                rule.ChargeTypeId = chargeTypeId;
                rule.TradeTypeId = row.TradeTypeId;               // مقدار از هر سطر
                rule.ContainerSizeTypeId = row.ContainerSizeTypeId;// مقدار از هر سطر
                rule.FullMTId = row.FullMTId;
                rule.Amount = row.Amount;
                rule.IsDG = row.IsDG;
                rule.IsOverSized = row.IsOverSized;
                rule.IsOverWeight = row.IsOverWeight;
                //rule.ExtraAttributes = row.Remarks;
                // مقادیر عمومی و پیش‌فرض
                rule.CalculationById = 1;   // Per Container
                rule.CalculationTypeId = 1; // Fixed Amount
                rule.CurrencyId = model.CurrencyId ?? tariffBook.CurrencyId;
                rule.EffectiveFrom = tariffBook.ValidFrom;
                rule.EffectiveTo = tariffBook.ValidTo;
                // لوکیشن انتخاب‌شده در هدر (مبدأ یا مقصد بسته به سناریو؛ معمولاً OriginId)
                rule.OriginId = model.LocationId;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }


        public async Task<List<LocationLookupVm>> SearchLocationsAsync(
            string? term,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<LocationLookupVm>();

            term = term.Trim().ToLower();

            return await _db.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && (x.LocationName.ToLower().Contains(term) || x.LocationCode.ToLower().Contains(term)))
                .Take(20)
                .Select(x => new LocationLookupVm
                {
                    Id = x.Id,
                    Code = x.LocationCode,
                    Name = x.LocationName
                })
                .ToListAsync(cancellationToken);
        }

        // =========================================================================
        // Helper SelectLists
        // =========================================================================

        //public async Task<List<SelectListItem>> GetContainerSizeTypesListAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _db.ContainerSizeTypes
        //        .AsNoTracking()
        //        .OrderBy(x => x.ShortName)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.Id.ToString(),
        //            Text = $"{x.ShortName}"
        //        })
        //        .ToListAsync(cancellationToken);
        //}
        public async Task<List<SelectListItem>> GetTradeTypesSelectListAsync(CancellationToken cancellationToken = default)
        {
            return await _db.TradeTypes
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TradeTypeName
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<SelectListItem>> GetFullMTsSelectListAsync(CancellationToken cancellationToken = default)
        {
            return await _db.FullMT
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync(cancellationToken);
        }


        private async Task<List<SelectListItem>> GetCurrenciesSelectListAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Currencies
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.CurrencyCode} - {x.CurrencyName}"
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<TariffRuleForSaoiVM> BuildLocalCntSaoiFormAsync(Guid tariffBookId, CancellationToken cancellationToken = default)
        {
            var tariffRules = await _db.TariffRules.Where(x => x.TariffBookId == tariffBookId).ToListAsync();
            var vm = new TariffRuleForSaoiVM
            {
                TariffBookId = tariffBookId,
            };

            //پروانه ورود موقت 
            var tariff1 = tariffRules.Where(x => x.ChargeTypeId == 101).FirstOrDefault();
            if (tariff1 != null) { vm.Row1TempFeeValue = (int)tariff1.Amount; }

            //ترخیصیه
            // مرتب‌سازی بر اساس MinQuantity و تبدیل به لیست یا آرایه
            var stepRules = tariffRules
                .Where(x => x.ChargeTypeId == 100)
                .OrderBy(x => x.MinQuantity)
                .ToList();

            // پله اول (ایندکس 0)
            var step1 = stepRules.ElementAtOrDefault(0);
            if (step1 != null)
            {
                vm.Row2_Issue_1_From = step1.MinQuantity != null ? (int)step1.MinQuantity : 0;
                vm.Row2_Issue_1_To = step1.MaxQuantity != null ? (int)step1.MaxQuantity : 0;
                vm.Row2_Issue_1_Value = step1.Amount != null ? (int)step1.Amount : 0;
            }

            // پله دوم (ایندکس 1)
            var step2 = stepRules.ElementAtOrDefault(1);
            if (step2 != null)
            {
                vm.Row2_Issue_2_From = step2.MinQuantity != null ? (int)step2.MinQuantity : 0;
                vm.Row2_Issue_2_To = step2.MaxQuantity != null ? (int)step2.MaxQuantity : 0;
                vm.Row2_Issue_2_Value = step2.Amount != null ? (int)step2.Amount : 0;
            }

            // پله سوم (ایندکس 2)
            var step3 = stepRules.ElementAtOrDefault(2);
            if (step3 != null)
            {
                vm.Row2_Issue_3_From = step3.MinQuantity != null ? (int)step3.MinQuantity : 0;
                vm.Row2_Issue_3_To = step3.MaxQuantity != null ? (int)step3.MaxQuantity : 0;
                vm.Row2_Issue_3_Value = step3.Amount != null ? (int)step3.Amount : 0;
            }

            //خدمات اداری اصلاحیه 
            var tariff3 = tariffRules.Where(x => x.ChargeTypeId == 127).FirstOrDefault();
            if (tariff3 != null) { vm.Row3_AmmendAdmin_Value = (int)tariff3.Amount; }


            //تجمیع و تفکیک
            var tariff4 = tariffRules.Where(x => x.ChargeTypeId == 128).FirstOrDefault();
            if (tariff4 != null) { vm.Row4_MergeManifest_Value = (int)tariff4.Amount; }

            //ترانشیپ
            var tariff5 = tariffRules.Where(x => x.ChargeTypeId == 109).FirstOrDefault();
            if (tariff5 != null) { vm.Row5_TranshipValue_Value = (int)tariff5.Amount; }


            //آپلود
            var tariff6 = tariffRules.Where(x => x.ChargeTypeId == 126).FirstOrDefault();
            if (tariff6 != null) { vm.Row6_Upload_Value = (int)tariff6.Amount; }

            //پلمپ
            var tariff7 = tariffRules.Where(x => x.ChargeTypeId == 108).FirstOrDefault();
            if (tariff7 != null) { vm.Row7_Seal_Value = (int)tariff7.Amount; }

            //برچسب زدایی
            var tariff8 = tariffRules.Where(x => x.ChargeTypeId == 107).FirstOrDefault();
            if (tariff8 != null) { vm.Row8_labelRemoval_Value = (int)tariff8.Amount; }

            //بارگذاری و تبادل اسناد کالای خطرناک
            var tariff9 = tariffRules.Where(x => x.ChargeTypeId == 110).FirstOrDefault();
            if (tariff9 != null) { vm.Row9_Danger_Value = (int)tariff9.Amount; }

            //کنسلی کانتنینرهای صادراتی
            var tariff10 = tariffRules.Where(x => x.ChargeTypeId == 111).FirstOrDefault();
            if (tariff10 != null) { vm.Row10_ExportCancel_Value = (int)tariff10.Amount; }

            //شیفتینگ چابهار
            var tariff11 = tariffRules.Where(x => x.ChargeTypeId == 112).FirstOrDefault();
            if (tariff11 != null) { vm.Row11_Shifting_Value = (int)tariff11.Amount; }

            //اچ اس کد مازاد
            var tariff12 = tariffRules.Where(x => x.ChargeTypeId == 104).FirstOrDefault();
            if (tariff12 != null) { vm.Row12_Extra_HsCode_Value = (int)tariff12.Amount; }

            return vm;
        }

        public async Task<bool> SaveTariffRuleForSaoiAsync(TariffRuleForSaoiVM vm, CancellationToken cancellationToken)
        {
            if (vm == null) return false;
            var tariffRules = await _db.TariffRules
                            .Where(x => x.TariffBookId == vm.TariffBookId).ToListAsync();

            //پروانه ورود موقت 
            var tariff1 = tariffRules.Where(x => x.ChargeTypeId == 101).FirstOrDefault();
            if (tariff1 != null)
            { tariff1.Amount = vm.Row1TempFeeValue; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row1TempFeeValue,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 101
                };
                _db.TariffRules.Add(newRule);
            }


            //ترخیصیه
            // مرتب‌سازی بر اساس MinQuantity و تبدیل به لیست یا آرایه
            var stepRules = tariffRules
                .Where(x => x.ChargeTypeId == 100)
                .OrderBy(x => x.MinQuantity)
                .ToList();

            // پله اول (ایندکس 0)
            var step1 = stepRules.ElementAtOrDefault(0);
            if (step1 != null)
            {
                step1.MinQuantity = vm.Row2_Issue_1_From;
                step1.MaxQuantity = vm.Row2_Issue_1_To;
                step1.Amount = vm.Row2_Issue_1_Value;
            }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row2_Issue_1_Value,
                    MinQuantity = vm.Row2_Issue_1_From,
                    MaxQuantity = vm.Row2_Issue_1_To,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    ChargeTypeId = 100,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                _db.TariffRules.Add(newRule);
            }



            // پله دوم (ایندکس 1)
            var step2 = stepRules.ElementAtOrDefault(1);
            if (step2 != null)
            {
                step2.MinQuantity = vm.Row2_Issue_2_From;
                step2.MaxQuantity = vm.Row2_Issue_2_To;
                step2.Amount = vm.Row2_Issue_2_Value;
            }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row2_Issue_2_Value,
                    MinQuantity = vm.Row2_Issue_2_From,
                    MaxQuantity = vm.Row2_Issue_2_To,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    ChargeTypeId = 100,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                _db.TariffRules.Add(newRule);
            }

            // پله سوم (ایندکس 2)
            var step3 = stepRules.ElementAtOrDefault(2);
            if (step3 != null)
            {
                step3.MinQuantity = vm.Row2_Issue_3_From;
                step3.MaxQuantity = vm.Row2_Issue_3_To;
                step3.Amount = vm.Row2_Issue_3_Value;
            }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row2_Issue_3_Value,
                    MinQuantity = vm.Row2_Issue_3_From,
                    MaxQuantity = vm.Row2_Issue_3_To,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    ChargeTypeId = 100,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                _db.TariffRules.Add(newRule);
            }

            //خدمات اداری اصلاحیه 
            var tariff3 = tariffRules.Where(x => x.ChargeTypeId == 127).FirstOrDefault();
            if (tariff3 != null)
            { tariff3.Amount = vm.Row3_AmmendAdmin_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row3_AmmendAdmin_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 127
                };
                _db.TariffRules.Add(newRule);
            }


            //تجمیع و تفکیک
            var tariff4 = tariffRules.Where(x => x.ChargeTypeId == 128).FirstOrDefault();
            if (tariff4 != null)
            { tariff4.Amount = vm.Row4_MergeManifest_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row4_MergeManifest_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 128
                };
                _db.TariffRules.Add(newRule);
            }

            //ترانشیپ
            var tariff5 = tariffRules.Where(x => x.ChargeTypeId == 109).FirstOrDefault();
            if (tariff5 != null)
            { tariff5.Amount = vm.Row5_TranshipValue_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row5_TranshipValue_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 109
                };
                _db.TariffRules.Add(newRule);
            }


            //آپلود
            var tariff6 = tariffRules.Where(x => x.ChargeTypeId == 126).FirstOrDefault();
            if (tariff6 != null)
            { tariff6.Amount = vm.Row6_Upload_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row6_Upload_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 126
                };
                _db.TariffRules.Add(newRule);
            }

            //پلمپ
            var tariff7 = tariffRules.Where(x => x.ChargeTypeId == 108).FirstOrDefault();
            if (tariff7 != null)
            { tariff7.Amount = vm.Row7_Seal_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row7_Seal_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 108
                };
                _db.TariffRules.Add(newRule);
            }

            //برچسب زدایی
            var tariff8 = tariffRules.Where(x => x.ChargeTypeId == 107).FirstOrDefault();
            if (tariff8 != null)
            { tariff8.Amount = vm.Row8_labelRemoval_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row8_labelRemoval_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 107
                };
                _db.TariffRules.Add(newRule);
            }

            //بارگذاری و تبادل اسناد کالای خطرناک
            var tariff9 = tariffRules.Where(x => x.ChargeTypeId == 110).FirstOrDefault();
            if (tariff9 != null)
            { tariff9.Amount = vm.Row9_Danger_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row9_Danger_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 110
                };
                _db.TariffRules.Add(newRule);
            }

            //کنسلی کانتنینرهای صادراتی
            var tariff10 = tariffRules.Where(x => x.ChargeTypeId == 111).FirstOrDefault();
            if (tariff10 != null)
            { tariff10.Amount = vm.Row10_ExportCancel_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row10_ExportCancel_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 111
                };
                _db.TariffRules.Add(newRule);
            }

            //شیفتینگ چابهار
            var tariff11 = tariffRules.Where(x => x.ChargeTypeId == 112).FirstOrDefault();
            if (tariff11 != null)
            { tariff11.Amount = vm.Row11_Shifting_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row11_Shifting_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 112
                };
                _db.TariffRules.Add(newRule);
            }

            //اچ اس کد مازاد
            var tariff12 = tariffRules.Where(x => x.ChargeTypeId == 104).FirstOrDefault();
            if (tariff12 != null)
            { tariff12.Amount = vm.Row12_Extra_HsCode_Value; }
            else
            {
                var newRule = new TariffRule
                {
                    TariffBookId = vm.TariffBookId,
                    Amount = vm.Row12_Extra_HsCode_Value,
                    CalculationById = 1,
                    CalculationTypeId = 1,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    ChargeTypeId = 104
                };
                _db.TariffRules.Add(newRule);
            }
            await _db.SaveChangesAsync();
            return true;
        }

        //private async Task<List<SelectListItem>> GetFullMTsListAsync(
        //   CancellationToken cancellationToken = default)
        //{
        //    return await _db.FullMT
        //        .AsNoTracking()
        //        .Where(x => !x.IsDeleted && x.IsActive)
        //        .OrderBy(x => x.Id)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.Id.ToString(),
        //            Text = x.FullMTName
        //        })
        //        .ToListAsync(cancellationToken);
        //}
    }
}