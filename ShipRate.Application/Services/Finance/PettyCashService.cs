using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Domain.Entities.PettyCashControl;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Finance
{
    public sealed class PettyCashService : IPettyCashService
    {
        private const int ExpenseStatusApproved = 3;
        private const int RechargeStatusCharged = 4;

        private readonly ApplicationDbContext _db;

        public PettyCashService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultDto<PettyCashListItemDto>> GetPettyCashesAsync(PettyCashIndexSearchDto search, CancellationToken cancellationToken = default)
        {
            var page = search.Page <= 0 ? 1 : search.Page;
            var pageSize = search.PageSize is 10 or 20 or 50 ? search.PageSize : 10;
            var term = search.Term?.Trim();

            var query =
                from pc in _db.PettyCashes.AsNoTracking()
                join p0 in _db.Persons.AsNoTracking() on pc.PersonId equals p0.Id into pg
                from p in pg.DefaultIfEmpty()
                join u0 in _db.Users.AsNoTracking() on pc.CustodianUserId equals u0.Id into ug
                from u in ug.DefaultIfEmpty()
                where !pc.IsDeleted
                select new
                {
                    pc,
                    PersonName = p == null ? null : p.PersonName,
                    CustodianUserName = u == null ? null : u.FullName
                };

            if (search.CompanyId.HasValue)
                query = query.Where(x => x.pc.CompanyId == search.CompanyId.Value);

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.pc.Code.Contains(term) ||
                    x.pc.Title.Contains(term) ||
                    (x.PersonName != null && x.PersonName.Contains(term)) ||
                    (x.CustodianUserName != null && x.CustodianUserName.Contains(term)) ||
                    _db.PettyCashExpenses.Any(e =>
                        !e.IsDeleted &&
                        e.PettyCashId == x.pc.Id &&
                        (e.Description.Contains(term) ||
                         _db.ExpenseCategories.Any(c => c.Id == e.ExpenseCategoryId && c.Title.Contains(term)) ||
                         _db.CostCenters.Any(cc => cc.Id == e.CostCenterId && cc.Title.Contains(term)))) ||
                    _db.PettyCashRecharges.Any(r =>
                        !r.IsDeleted &&
                        r.PettyCashId == x.pc.Id &&
                        r.Description.Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.pc.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PettyCashListItemDto
                {
                    Id = x.pc.Id,
                    CompanyId = x.pc.CompanyId,
                    Code = x.pc.Code,
                    Title = x.pc.Title,
                    InitialAmount = x.pc.InitialAmount,
                    CurrentBalance = x.pc.CurrentBalance,
                    PersonId = x.pc.PersonId,
                    PersonName = x.PersonName,
                    CustodianUserId = x.pc.CustodianUserId,
                    CustodianUserName = x.CustodianUserName,
                    //IsActive = x.pc.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<PettyCashListItemDto>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public Task<PettyCashUpsertDto> GetPettyCashForCreateAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PettyCashUpsertDto
            {
                Id = null,
                IsActive = true,
                InitialAmount = 0,
                CurrentBalance = 0
            });
        }

        public async Task<PettyCashUpsertDto> GetPettyCashForEditAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _db.PettyCashes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            return new PettyCashUpsertDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Code = entity.Code,
                Title = entity.Title,
                InitialAmount = entity.InitialAmount,
                CurrentBalance = entity.CurrentBalance,
                PersonId = entity.PersonId,
                CustodianUserId = entity.CustodianUserId,
                //IsActive = entity.IsActive
            };
        }

        public async Task SavePettyCashAsync(PettyCashUpsertDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            if (dto.PersonId == null && dto.CustodianUserId == null)
                throw new InvalidOperationException("Person or Custodian User is required.");

            if (dto.Id == null || dto.Id == Guid.Empty)
            {
                var entity = new PettyCash
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = dto.CompanyId,
                    Code = dto.Code.Trim(),
                    Title = dto.Title.Trim(),
                    InitialAmount = dto.InitialAmount,
                    CurrentBalance = dto.InitialAmount,
                    PersonId = dto.PersonId,
                    CustodianUserId = dto.CustodianUserId,
                    //IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = userId
                };

                _db.PettyCashes.Add(entity);
            }
            else
            {
                var entity = await _db.PettyCashes
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value && !x.IsDeleted, cancellationToken)
                    ?? throw new InvalidOperationException("Petty cash not found.");

                entity.CompanyId = dto.CompanyId;
                entity.Code = dto.Code.Trim();
                entity.Title = dto.Title.Trim();
                entity.InitialAmount = dto.InitialAmount;
                entity.PersonId = dto.PersonId;
                entity.CustodianUserId = dto.CustodianUserId;
                //entity.IsActive = dto.IsActive;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = userId;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePettyCashAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var entity = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            var hasExpense = await _db.PettyCashExpenses.AnyAsync(x => x.PettyCashId == id && !x.IsDeleted, cancellationToken);
            var hasRecharge = await _db.PettyCashRecharges.AnyAsync(x => x.PettyCashId == id && !x.IsDeleted, cancellationToken);

            if (hasExpense || hasRecharge)
                throw new InvalidOperationException("Petty cash has expenses or recharges and cannot be deleted.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PettyCashExpenseListItemDto>> GetExpensesAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var query =
                from e in _db.PettyCashExpenses.AsNoTracking()
                join c in _db.ExpenseCategories.AsNoTracking() on e.ExpenseCategoryId equals c.Id
                join cc in _db.CostCenters.AsNoTracking() on e.CostCenterId equals cc.Id
                join s in _db.PettyCashExpenseStatus.AsNoTracking() on e.StatusId equals s.Id
                where !e.IsDeleted && e.PettyCashId == pettyCashId
                orderby e.ExpenseDate descending
                select new PettyCashExpenseListItemDto
                {
                    Id = e.Id,
                    PettyCashId = e.PettyCashId,
                    ExpenseDate = e.ExpenseDate,
                    Amount = e.Amount,
                    CategoryTitle = c.Title,
                    CostCenterTitle = cc.Title,
                    Description = e.Description,
                    StatusId = e.StatusId,
                    StatusName = s.StatusName
                };

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<PettyCashExpenseUpsertDto> GetExpenseForCreateAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var pettyCash = await _db.PettyCashes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pettyCashId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            return new PettyCashExpenseUpsertDto
            {
                CompanyId = pettyCash.CompanyId,
                PettyCashId = pettyCash.Id,
                ExpenseDate = DateTime.Today,
                StatusId = 1,
                CurrentBalance = pettyCash.CurrentBalance
            };
        }

        public async Task<PettyCashExpenseUpsertDto> GetExpenseForEditAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _db.PettyCashExpenses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Expense not found.");

            var balance = await _db.PettyCashes
                .Where(x => x.Id == entity.PettyCashId)
                .Select(x => x.CurrentBalance)
                .FirstAsync(cancellationToken);

            var maxAllowed = await _db.ExpenseCategories
                .Where(x => x.Id == entity.ExpenseCategoryId)
                .Select(x => (decimal?)x.MaxAllowedAmount)
                .FirstOrDefaultAsync(cancellationToken);

            return new PettyCashExpenseUpsertDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                PettyCashId = entity.PettyCashId,
                ExpenseDate = entity.ExpenseDate,
                Amount = entity.Amount,
                ExpenseCategoryId = entity.ExpenseCategoryId,
                CostCenterId = entity.CostCenterId,
                Description = entity.Description,
                StatusId = entity.StatusId,
                CurrentBalance = balance,
                CategoryMaxAllowedAmount = maxAllowed
            };
        }

        public async Task SaveExpenseAsync(PettyCashExpenseUpsertDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var pettyCash = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == dto.PettyCashId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            var categoryMaxAmount = await _db.ExpenseCategories
                .Where(x => x.Id == dto.ExpenseCategoryId)
                .Select(x => x.MaxAllowedAmount)
                .FirstOrDefaultAsync(cancellationToken);

            if (categoryMaxAmount > 0 && dto.Amount > categoryMaxAmount)
            {
                // MaxAllowedAmount is only a warning according to business rule.
            }

            if (dto.Id == null || dto.Id == Guid.Empty)
            {
                var entity = new PettyCashExpense
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = pettyCash.CompanyId,
                    PettyCashId = dto.PettyCashId,
                    ExpenseDate = dto.ExpenseDate,
                    Amount = dto.Amount,
                    ExpenseCategoryId = dto.ExpenseCategoryId,
                    CostCenterId = dto.CostCenterId,
                    Description = dto.Description.Trim(),
                    StatusId = 1,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = userId
                };

                _db.PettyCashExpenses.Add(entity);
                pettyCash.CurrentBalance -= dto.Amount;
            }
            else
            {
                var entity = await _db.PettyCashExpenses
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value && !x.IsDeleted, cancellationToken)
                    ?? throw new InvalidOperationException("Expense not found.");

                if (entity.StatusId == ExpenseStatusApproved)
                    throw new InvalidOperationException("Approved expense cannot be edited.");

                var oldAmount = entity.Amount;

                entity.ExpenseDate = dto.ExpenseDate;
                entity.Amount = dto.Amount;
                entity.ExpenseCategoryId = dto.ExpenseCategoryId;
                entity.CostCenterId = dto.CostCenterId;
                entity.Description = dto.Description.Trim();
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = userId;

                pettyCash.CurrentBalance += oldAmount;
                pettyCash.CurrentBalance -= dto.Amount;
            }

            pettyCash.UpdatedAt = DateTime.Now;
            pettyCash.EditUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteExpenseAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var entity = await _db.PettyCashExpenses
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Expense not found.");

            if (entity.StatusId == ExpenseStatusApproved)
                throw new InvalidOperationException("Approved expense cannot be deleted.");

            var pettyCash = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == entity.PettyCashId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            pettyCash.CurrentBalance += entity.Amount;
            pettyCash.UpdatedAt = DateTime.Now;
            pettyCash.EditUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PettyCashRechargeListItemDto>> GetRechargesAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var query =
                from r in _db.PettyCashRecharges.AsNoTracking()
                join s in _db.PettyCashRechargeStatuses.AsNoTracking() on r.StatusId equals s.Id
                where !r.IsDeleted && r.PettyCashId == pettyCashId
                orderby r.RechargeDate descending
                select new PettyCashRechargeListItemDto
                {
                    Id = r.Id,
                    PettyCashId = r.PettyCashId,
                    RechargeAmount = r.RechargeAmount,
                    RechargeDate = r.RechargeDate,
                    Description = r.Description,
                    StatusId = r.StatusId,
                    StatusName = s.StatusName
                };

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<PettyCashRechargeUpsertDto> GetRechargeForCreateAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var pettyCash = await _db.PettyCashes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pettyCashId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            return new PettyCashRechargeUpsertDto
            {
                CompanyId = pettyCash.CompanyId,
                PettyCashId = pettyCash.Id,
                RechargeDate = DateTime.Today,
                StatusId = 1
            };
        }

        public async Task<PettyCashRechargeUpsertDto> GetRechargeForEditAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _db.PettyCashRecharges
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Recharge not found.");

            return new PettyCashRechargeUpsertDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                PettyCashId = entity.PettyCashId,
                RechargeAmount = entity.RechargeAmount,
                RechargeDate = entity.RechargeDate,
                Description = entity.Description,
                StatusId = entity.StatusId
            };
        }

        public async Task SaveRechargeAsync(PettyCashRechargeUpsertDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var pettyCash = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == dto.PettyCashId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Petty cash not found.");

            if (dto.Id == null || dto.Id == Guid.Empty)
            {
                var entity = new PettyCashRecharge
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = pettyCash.CompanyId,
                    PettyCashId = dto.PettyCashId,
                    RechargeAmount = dto.RechargeAmount,
                    RechargeDate = dto.RechargeDate,
                    Description = dto.Description.Trim(),
                    StatusId = dto.StatusId,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = userId
                };

                _db.PettyCashRecharges.Add(entity);

                if (dto.StatusId == RechargeStatusCharged)
                {
                    pettyCash.CurrentBalance += dto.RechargeAmount;
                    entity.ChargedByUserId = userId;
                }
            }
            else
            {
                var entity = await _db.PettyCashRecharges
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value && !x.IsDeleted, cancellationToken)
                    ?? throw new InvalidOperationException("Recharge not found.");

                if (entity.StatusId == RechargeStatusCharged)
                    throw new InvalidOperationException("Charged recharge cannot be edited.");

                var oldStatusId = entity.StatusId;

                entity.RechargeAmount = dto.RechargeAmount;
                entity.RechargeDate = dto.RechargeDate;
                entity.Description = dto.Description.Trim();
                entity.StatusId = dto.StatusId;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = userId;

                if (oldStatusId != RechargeStatusCharged && dto.StatusId == RechargeStatusCharged)
                {
                    pettyCash.CurrentBalance += dto.RechargeAmount;
                    entity.ChargedByUserId = userId;
                }
            }

            pettyCash.UpdatedAt = DateTime.Now;
            pettyCash.EditUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRechargeAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var entity = await _db.PettyCashRecharges
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Recharge not found.");

            if (entity.StatusId == RechargeStatusCharged)
                throw new InvalidOperationException("Charged recharge cannot be deleted.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<PettyCashLookupDto> GetLookupsAsync(int? companyId = null, CancellationToken cancellationToken = default)
        {
            var companies = await _db.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem(x.Person.PersonName.ToString(), x.Id.ToString()))
                .ToListAsync(cancellationToken);

            var persons = await _db.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem(x.PersonName, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            var users = await _db.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem(x.FullName ?? (x.FirstName + " " + x.LastName), x.Id.ToString()))
                .ToListAsync(cancellationToken);

            var categoriesQuery = _db.ExpenseCategories
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            var costCentersQuery = _db.CostCenters
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (companyId.HasValue)
            {
                categoriesQuery = categoriesQuery.Where(x => x.CompanyId == companyId.Value);
                costCentersQuery = costCentersQuery.Where(x => x.CompanyId == companyId.Value);
            }

            var categories = await categoriesQuery
                .OrderBy(x => x.Title)
                .Select(x => new SelectListItem(x.Title, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            var costCenters = await costCentersQuery
                .OrderBy(x => x.Title)
                .Select(x => new SelectListItem(x.Title, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            var expenseStatuses = await _db.PettyCashExpenseStatus
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem(x.StatusName, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            var rechargeStatuses = await _db.PettyCashRechargeStatuses
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem(x.StatusName, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            return new PettyCashLookupDto
            {
                Companies = companies,
                Persons = persons,
                Users = users,
                ExpenseCategories = categories,
                CostCenters = costCenters,
                ExpenseStatuses = expenseStatuses,
                RechargeStatuses = rechargeStatuses
            };
        }
        public async Task ApproveExpenseAsync(Guid id)
        {
            var expense = await _db.PettyCashExpenses
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (expense == null)
                throw new Exception("Expense not found.");

            if (expense.StatusId == 3)
                return;

            if (expense.StatusId == 9)
                throw new Exception("Rejected expense cannot be approved.");

            var pettyCash = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == expense.PettyCashId && !x.IsDeleted);

            if (pettyCash == null)
                throw new Exception("Petty cash not found.");

            expense.StatusId = 3;

            //pettyCash.CurrentBalance -= expense.Amount;

            await _db.SaveChangesAsync();
        }

        public async Task RejectExpenseAsync(Guid id)
        {
            var expense = await _db.PettyCashExpenses
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (expense == null)
                throw new Exception("Expense not found.");

            if (expense.StatusId == 9)
                return;

            var pettyCash = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == expense.PettyCashId && !x.IsDeleted);

            if (pettyCash == null)
                throw new Exception("Petty cash not found.");

            if (expense.StatusId == 3)
                throw new Exception("Approved expense cannot be rejected.");

            if (expense.StatusId == 3)
            {
                pettyCash.CurrentBalance += expense.Amount;
            }
            expense.StatusId = 9;

            await _db.SaveChangesAsync();
        }



        public async Task ApproveRechargeAsync(Guid id)
        {
            var recharge = await _db.PettyCashRecharges
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (recharge == null)
                throw new Exception("Recharge not found.");

            if (recharge.StatusId == 4)
                throw new Exception("Charged recharge cannot be approved.");

            if (recharge.StatusId == 9)
                throw new Exception("Rejected recharge cannot be approved.");

            recharge.StatusId = 3;

            await _db.SaveChangesAsync();
        }

        public async Task RejectRechargeAsync(Guid id)
        {
            var recharge = await _db.PettyCashRecharges
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (recharge == null)
                throw new Exception("Recharge not found.");

            if (recharge.StatusId == 4)
                throw new Exception("Charged recharge cannot be rejected.");

            recharge.StatusId = 9;

            await _db.SaveChangesAsync();
        }

        public async Task ChargeRechargeAsync(Guid id)
        {
            var recharge = await _db.PettyCashRecharges
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (recharge == null)
                throw new Exception("Recharge not found.");

            if (recharge.StatusId == 4)
                return;

            if (recharge.StatusId == 9)
                throw new Exception("Rejected recharge cannot be charged.");

            var pettyCash = await _db.PettyCashes
                .FirstOrDefaultAsync(x => x.Id == recharge.PettyCashId && !x.IsDeleted);

            if (pettyCash == null)
                throw new Exception("Petty cash not found.");

            recharge.StatusId = 4;
            pettyCash.CurrentBalance += recharge.RechargeAmount;

            await _db.SaveChangesAsync();
        }




    }
}
