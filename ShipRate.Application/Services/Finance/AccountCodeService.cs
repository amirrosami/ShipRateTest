using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Finance
{
    public class AccountCodeService : IAccountCodeService
    {
        private readonly ApplicationDbContext _context;

        public AccountCodeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<AccountCodeListItemVm>> GetPagedAsync(AccountCodeSearchVm input)
        {
            var query = _context.AccountCodes
                .AsNoTracking()
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .Include(x => x.Person)
                .Include(x => x.ChargeType)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(input.Search))
            {
                var search = input.Search.Trim();

                query = query.Where(x =>
                    x.AccCode.Contains(search) ||
                    x.Company.Person.PersonName.Contains(search) ||
                    (x.Person != null &&
                        x.Person.PersonName.Contains(search)
                    ) ||
                    (x.ChargeType != null && x.ChargeType.ChargeTypeName.Contains(search))
                );
            }

            if (input.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == input.CompanyId.Value);

            if (input.ChargeTypeId.HasValue)
                query = query.Where(x => x.ChargeTypeId == input.ChargeTypeId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.AccCode)
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new AccountCodeListItemVm
                {
                    Id = x.Id,
                    AccCode = x.AccCode,
                    BuyAccCode = x.BuyAccCode,
                    SellAccCode = x.SellAccCode,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Person.PersonName,
                    PersonId = x.PersonId,
                    PersonName = x.Person != null ? (x.Person.PersonName) : null,
                    ChargeTypeId = x.ChargeTypeId,
                    ChargeTypeTitle = x.ChargeType != null ? x.ChargeType.ChargeTypeName : null
                })
                .ToListAsync();

            return new PagedResultDto<AccountCodeListItemVm>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = input.Page,
                PageSize = input.PageSize
            };
        }

        public async Task<AccountCodeCreateEditVm> GetCreateVmAsync()
        {
            var vm = new AccountCodeCreateEditVm();
            return await FillLookupsAsync(vm);
        }

        public async Task<AccountCodeCreateEditVm?> GetByIdForEditAsync(Guid id)
        {
            var entity = await _context.AccountCodes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            var vm = new AccountCodeCreateEditVm
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                PersonId = entity.PersonId,
                ChargeTypeId = entity.ChargeTypeId,
                AccCode = entity.AccCode,
                BuyAccCode = entity.BuyAccCode,
                SellAccCode = entity.SellAccCode,
            };

            return await FillLookupsAsync(vm);
        }

        public async Task<AccountCodeCreateEditVm> FillLookupsAsync(AccountCodeCreateEditVm vm)
        {
            vm.Companies = await _context.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();

            vm.Persons = await _context.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();

            vm.ChargeTypes = await _context.ChargeTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync();

            return vm;
        }

        public async Task<(bool Succeeded, string Message)> CreateAsync(AccountCodeCreateEditVm vm, string? userId)
        {
            var validation = await ValidateBusinessRulesAsync(vm);
            if (!validation.Succeeded)
                return validation;



            var duplicate = await _context.AccountCodes
                .AnyAsync(x => !x.IsDeleted &&
                               x.AccCode == vm.AccCode &&
                               x.CompanyId == vm.CompanyId &&
                               x.PersonId == vm.PersonId &&
                               x.ChargeTypeId == vm.ChargeTypeId);

            if (duplicate)
                return (false, "Duplicate account code record exists.");

            var entity = new AccountCode
            {
                Id = Guid.NewGuid(),
                CompanyId = vm.CompanyId,
                PersonId = vm.PersonId,
                ChargeTypeId = vm.ChargeTypeId,
                AccCode = vm.AccCode.Trim(),
                CreatedAt = DateTime.Now,
                CreateUserId = Guid.Parse(userId),
                IsDeleted = false,
                BuyAccCode = vm.BuyAccCode,
                SellAccCode = vm.SellAccCode,
            };

            _context.AccountCodes.Add(entity);
            await _context.SaveChangesAsync();

            return (true, "Account code created successfully.");
        }

        public async Task<(bool Succeeded, string Message)> UpdateAsync(AccountCodeCreateEditVm vm, string? userId)
        {
            if (!vm.Id.HasValue)
                return (false, "Invalid record id.");

            var entity = await _context.AccountCodes
                .FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted);

            if (entity == null)
                return (false, "Record not found.");


            var validation = await ValidateBusinessRulesAsync(vm);
            if (!validation.Succeeded)
                return validation;

            var duplicate = await _context.AccountCodes
                .AnyAsync(x => !x.IsDeleted &&
                               x.Id != vm.Id.Value &&
                               x.AccCode == vm.AccCode &&
                               x.CompanyId == vm.CompanyId &&
                               x.PersonId == vm.PersonId &&
                               x.ChargeTypeId == vm.ChargeTypeId);

            if (duplicate)
                return (false, "Duplicate account code record exists.");

            entity.CompanyId = vm.CompanyId;
            entity.PersonId = vm.PersonId;
            entity.ChargeTypeId = vm.ChargeTypeId;

            entity.AccCode = vm.AccCode.Trim();
            entity.BuyAccCode = vm.BuyAccCode.Trim();
            entity.SellAccCode = vm.SellAccCode.Trim();

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = Guid.Parse(userId);


            await _context.SaveChangesAsync();

            return (true, "Account code updated successfully.");
        }

        public async Task<(bool Succeeded, string Message)> DeleteAsync(Guid id, string? userId)
        {
            var entity = await _context.AccountCodes
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return (false, "Record not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = Guid.Parse(userId);

            await _context.SaveChangesAsync();

            return (true, "Account code deleted successfully.");
        }


        private async Task<(bool Succeeded, string Message)> ValidateBusinessRulesAsync(AccountCodeCreateEditVm vm)
        {
            var hasPerson = vm.PersonId.HasValue;
            var hasCharge = vm.ChargeTypeId.HasValue;

            if (!hasPerson && !hasCharge)
                return (false, "Either Person or Charge Type must be selected.");

            if (hasPerson && hasCharge)
                return (false, "Only one of Person or Charge Type can be selected.");

            if (hasPerson)
            {
                var personDuplicate = await _context.AccountCodes
                    .AnyAsync(x =>
                        !x.IsDeleted &&
                        x.Id != (vm.Id ?? Guid.Empty) &&
                        x.CompanyId == vm.CompanyId &&
                        x.PersonId == vm.PersonId);

                if (personDuplicate)
                    return (false, "This person already has an account code in the selected company.");
            }

            if (hasCharge)
            {
                var chargeDuplicate = await _context.AccountCodes
                    .AnyAsync(x =>
                        !x.IsDeleted &&
                        x.Id != (vm.Id ?? Guid.Empty) &&
                        x.CompanyId == vm.CompanyId &&
                        x.ChargeTypeId == vm.ChargeTypeId);

                if (chargeDuplicate)
                    return (false, "This charge type already has an account code in the selected company.");
            }
            return (true, string.Empty);
        }
    }
}
