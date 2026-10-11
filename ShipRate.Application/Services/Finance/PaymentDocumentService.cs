using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Finance
{
    public class PaymentDocumentService : IPaymentDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private const int ChequePaymentMethodId = 3;
        public PaymentDocumentService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<PagedResultDto<PaymentDocumentListItemVm>> GetPagedAsync(PaymentDocumentSearchVm searchModel)
        {
            searchModel ??= new PaymentDocumentSearchVm();

            var query = _context.PaymentDocuments
                .AsNoTracking()
                //.Include(x => x.Party)
                //.Include(x => x.Company)
                //.Include(x => x.Currency)
                //.Include(x => x.FinanceDirection)
                //.Include(x => x.PaymentMethod)
                //.Include(x => x.Cheque)
                .Where(x => !x.IsDeleted)
                .AsQueryable();


            if (searchModel.FinanceDirectionId != null)
            {
                query = query.Where(x => x.FinanceDirectionId == searchModel.FinanceDirectionId);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.DocumentNumber))
            {
                var docNo = searchModel.DocumentNumber.Trim();
                query = query.Where(x => x.DocumentNo != null && x.DocumentNo.Contains(docNo));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.PersonName))
            {
                var personName = searchModel.PersonName.Trim();
                query = query.Where(x => x.Party != null && x.Party.PersonName != null && x.Party.PersonName.Contains(personName));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.TrackingNumber))
            {
                var tracking = searchModel.TrackingNumber.Trim();
                query = query.Where(x => x.TrackingNo != null && x.TrackingNo.Contains(tracking));
            }

            if (searchModel.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == searchModel.CompanyId.Value);
            }

            if (searchModel.PaymentMethodId.HasValue)
            {
                query = query.Where(x => x.PaymentMethodId == searchModel.PaymentMethodId.Value);
            }

            if (searchModel.CurrencyId.HasValue)
            {
                query = query.Where(x => x.CurrencyId == searchModel.CurrencyId.Value);
            }

            if (searchModel.FinanceDirectionId.HasValue)
            {
                query = query.Where(x => x.FinanceDirectionId == searchModel.FinanceDirectionId.Value);
            }

            if (searchModel.ChequePurposeId.HasValue)
            {
                query = query.Where(x => x.Cheque.ChequePurposeId == searchModel.ChequePurposeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.FromDateShamsi))
            {
                var fromDate = ParseShamsiToGregorian(searchModel.FromDateShamsi);
                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.DocumentDate >= fromDate.Value.Date);
                }
            }
            if (searchModel.PersonId.HasValue)
            {
                query = query.Where(x => x.PartyId == searchModel.PersonId);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ToDateShamsi))
            {
                var toDate = ParseShamsiToGregorian(searchModel.ToDateShamsi);
                if (toDate.HasValue)
                {
                    var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.DocumentDate <= endOfDay);
                }
            }

            if (searchModel.OnlyHasRemaining)
            {
                query = query.Where(x => (x.Amount - x.AllocatedAmount) > 0);
            }


            var totalCount = await query.CountAsync();

            var page = searchModel.Page <= 0 ? 1 : searchModel.Page;
            var pageSize = searchModel.PageSize <= 0 ? 10 : searchModel.PageSize;

            var items = await query
                .OrderByDescending(x => x.SystemId)
                .ThenByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PaymentDocumentListItemVm
                {
                    Id = x.Id,
                    DocumentNumber = x.DocumentNo,
                    DocumentDateGregorian = x.DocumentDate,
                    DocumentDateShamsi = x.DocumentDate != null
                        ? PersianDateHelper.ToShamsi(x.DocumentDate)
                        : null,
                    PersonName = x.Party != null ? x.Party.PersonName : null,
                    CompanyName = x.Company != null ? x.Company.Person.PersonName : null,
                    CurrencyTitle = x.Currency != null ? x.Currency.CurrencyCode : null,
                    FinanceDirectionTitle = x.FinanceDirection != null ? x.FinanceDirection.PaymentDocumentContextName : null,
                    PaymentMethodTitle = x.PaymentMethod != null ? x.PaymentMethod.PaymentMethodName : null,
                    Amount = x.Amount,
                    AllocatedAmount = x.AllocatedAmount,
                    UnAllocatedAmount = x.UnallocatedAmount,
                    TrackingNumber = x.TrackingNo,
                    HasCheque = x.Cheque != null && !x.Cheque.IsDeleted,
                    SystemId = x.SystemId,
                    CreatedAt = x.CreatedAt,
                    CreateUserName = x.CreateUser.FullName,
                    UpdatedAt = x.UpdatedAt,
                    UpdateUserName = x.EditUser.FullName,
                    HasVaultTransaction = _context.VaultMovements.Any(y => y.SourceEntityId == x.Id && y.VaultMovementSourceTypeId == 1)
                })
                .ToListAsync();

            return new PagedResultDto<PaymentDocumentListItemVm>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaymentDocumentCreateEditVm> GetCreateModelAsync()
        {
            var model = new PaymentDocumentCreateEditVm
            {
                DocumentDateShamsi = PersianDateHelper.ToShamsi(DateTime.Now),
                Cheque = new ChequeFormVm
                {
                    ChequeDateShamsi = PersianDateHelper.ToShamsi(DateTime.Now)
                }
            };

            await FillLookupListsAsync(model);
            return model;
        }

        public async Task<PaymentDocumentCreateEditVm?> GetEditModelAsync(Guid id)
        {
            var entity = await _context.PaymentDocuments
                .AsNoTracking()
                .Include(x => x.Cheque)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            var model = new PaymentDocumentCreateEditVm
            {
                Id = entity.Id,
                DocumentNumber = entity.DocumentNo ?? string.Empty,
                DocumentDateGregorian = entity.DocumentDate,
                DocumentDateShamsi = entity.DocumentDate != null
                    ? PersianDateHelper.ToShamsi(entity.DocumentDate)
                    : string.Empty,
                CompanyId = entity.CompanyId,
                PersonId = entity.PartyId,
                CurrencyId = entity.CurrencyId,
                FinanceDirectionId = entity.FinanceDirectionId,
                PaymentMethodId = entity.PaymentMethodId,
                Amount = entity.Amount,
                TrackingNumber = entity.TrackingNo,
                Description = entity.Description,
                BranchId = entity.BranchId,
                Cheque = new ChequeFormVm()
            };

            if (entity.PartyId.HasValue)
            {
                model.PersonName = await _context.Persons
                    .AsNoTracking()
                    .Where(x => x.Id == entity.PartyId.Value && !x.IsDeleted)
                    .Select(x => x.PersonName)
                    .FirstOrDefaultAsync();
            }

            if (entity.Cheque != null && !entity.Cheque.IsDeleted)
            {
                model.Cheque = new ChequeFormVm
                {
                    Id = entity.Cheque.Id,
                    ChequeNumber = entity.Cheque.ChequeNo,
                    ChequeDateGregorian = entity.Cheque.DueDate,
                    ChequeDateShamsi = entity.Cheque.DueDate != null
                        ? PersianDateHelper.ToShamsi(entity.Cheque.DueDate)
                        : null,
                    ChequeTypeId = entity.Cheque.ChequeTypeId,
                    BankId = entity.Cheque.BankId,
                    BankAccountId = entity.Cheque.BankAccountId,
                    AccountHolderName = entity.Cheque.IssuerName,
                    Description = entity.Cheque.Remarks,
                    ChequePurposeId = entity.Cheque.ChequePurposeId,
                    BankAccountNo = entity.Cheque.BankAccountNo,
                    SayadNo = entity.Cheque.SayadNo,
                    IBAN = entity.Cheque.IBAN,
                };
            }

            await FillLookupListsAsync(model);
            return model;
        }

        public async Task<Guid> CreateAsync(PaymentDocumentCreateEditVm model, string? currentUserId = null)
        {
            NormalizeModel(model);
            await FillLookupListsAsync(model);

            var validationErrors = await ValidateBusinessAsync(model);
            if (validationErrors.Any())
            {
                throw new InvalidOperationException(string.Join(" | ", validationErrors));
            }

            //var documentDate = ParseShamsiToGregorian(model.DocumentDateShamsi);
            //if (!documentDate.HasValue)
            //{
            //    throw new InvalidOperationException("Document date is invalid.");
            //}

            var entity = new PaymentDocument
            {
                CompanyId = model.CompanyId.Value,
                BranchId = model.BranchId,
                DocumentNo = model.DocumentNumber?.Trim(),
                DocumentDate = model.DocumentDateGregorian.Value,
                PartyId = model.PersonId,
                CurrencyId = model.CurrencyId.Value,
                FinanceDirectionId = model.FinanceDirectionId.Value,
                PaymentMethodId = model.PaymentMethodId.Value,
                Amount = model.Amount,
                TrackingNo = model.TrackingNumber?.Trim(),
                Description = model.Description?.Trim(),
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
            };

            if (RequiresCheque(model.PaymentMethodId))
            {
                entity.Cheque = BuildChequeEntityFromModel(model.Cheque, _currentUserService.UserId.ToString());
                entity.Cheque.CurrencyId = model.CurrencyId.Value;
            }

            _context.PaymentDocuments.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(PaymentDocumentCreateEditVm model)
        {
            NormalizeModel(model);
            await FillLookupListsAsync(model);

            var validationErrors = await ValidateBusinessAsync(model);
            if (validationErrors.Any())
            {
                throw new InvalidOperationException(string.Join(" | ", validationErrors));
            }

            var currentuserId = _currentUserService.UserId;

            var entity = await _context.PaymentDocuments
                .Include(x => x.Cheque)
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

            if (entity == null)
                return false;

            var documentDate = ParseShamsiToGregorian(model.DocumentDateShamsi);
            if (!documentDate.HasValue)
            {
                throw new InvalidOperationException("Document date is invalid.");
            }

            entity.DocumentNo = model.DocumentNumber?.Trim();
            entity.DocumentDate = documentDate.Value;
            entity.CompanyId = model.CompanyId.Value;
            entity.PartyId = model.PersonId.Value;
            entity.CurrencyId = model.CurrencyId.Value;
            entity.FinanceDirectionId = model.FinanceDirectionId.Value;
            entity.PaymentMethodId = model.PaymentMethodId.Value;
            entity.Amount = model.Amount;
            entity.TrackingNo = model.TrackingNumber?.Trim();
            entity.Description = model.Description?.Trim();
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentuserId;

            if (RequiresCheque(model.PaymentMethodId))
            {
                if (entity.Cheque == null || entity.Cheque.IsDeleted)
                {
                    entity.Cheque = BuildChequeEntityFromModel(model.Cheque, currentuserId.ToString());
                }
                else
                {
                    UpdateChequeEntityFromModel(entity.Cheque, model.Cheque, currentuserId.ToString());
                }
            }
            else
            {
                if (entity.Cheque != null && !entity.Cheque.IsDeleted)
                {
                    entity.Cheque.IsDeleted = true;
                    entity.Cheque.UpdatedAt = DateTime.Now;
                    //entity.Cheque.ModifiedBy = currentUserId;
                }
            }




            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, string? currentUserId = null)
        {
            var entity = await _context.PaymentDocuments
                .Include(x => x.Cheque)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return false;








            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            //entity.EditUserId = currentUserId;

            if (entity.Cheque != null && !entity.Cheque.IsDeleted)
            {
                entity.Cheque.IsDeleted = true;
                entity.Cheque.UpdatedAt = DateTime.Now;
                //entity.Cheque.ModifiedBy = currentUserId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentDocumentDetailsVm?> GetDetailsAsync(Guid id)
        {
            var entity = await _context.PaymentDocuments
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .Include(x => x.Currency)
                .Include(x => x.FinanceDirection)
                .Include(x => x.PaymentMethod)
                .Include(x => x.Cheque)
                    .ThenInclude(x => x.Bank)
                .Include(x => x.Cheque)
                    .ThenInclude(x => x.BankAccount)
                .Include(x => x.Cheque)
                    .ThenInclude(x => x.ChequeType)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            return new PaymentDocumentDetailsVm
            {
                Id = entity.Id,
                DocumentNumber = entity.DocumentNo,
                DocumentDateGregorian = entity.DocumentDate,
                DocumentDateShamsi = entity.DocumentDate != null
                    ? PersianDateHelper.ToShamsi(entity.DocumentDate)
                    : null,
                CompanyName = entity.Company?.Person.PersonName,
                PersonName = entity.Party?.PersonName,
                CurrencyTitle = entity.Currency?.CurrencyName,
                FinanceDirectionTitle = entity.FinanceDirection?.FinanceDirectionName,
                PaymentMethodTitle = entity.PaymentMethod?.PaymentMethodName,
                Amount = entity.Amount,
                TrackingNumber = entity.TrackingNo,
                Description = entity.Description,
                Cheque = entity.Cheque != null && !entity.Cheque.IsDeleted
                    ? new ChequeDetailsVm
                    {
                        Id = entity.Cheque.Id,
                        ChequeNumber = entity.Cheque.ChequeNo,
                        ChequeDateGregorian = entity.Cheque.DueDate,
                        ChequeDateShamsi = entity.Cheque.DueDate != null
                            ? PersianDateHelper.ToShamsi(entity.Cheque.DueDate)
                            : null,
                        ChequeTypeTitle = entity.Cheque.ChequeType?.ChequeTypeName,
                        BankName = entity.Cheque.Bank?.BankName,
                        BankAccountNumber = entity.Cheque.BankAccountNo,
                        AccountHolderName = entity.Cheque.IssuerName,
                        Description = entity.Cheque.Remarks

                    }
                    : null
            };
        }

        public async Task<List<PersonLookupVm>> SearchPersonsAsync(string? term, int take = 10)
        {
            take = take <= 0 ? 10 : take;

            var query = _context.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();

                query = query.Where(x =>
                    (x.PersonName != null && x.PersonName.Contains(term)) ||
                    (x.NationalID != null && x.NationalID.Contains(term)));
            }

            return await query
                .OrderBy(x => x.PersonName)
                .Take(take)
                .Select(x => new PersonLookupVm
                {
                    Id = x.Id,
                    FullName = x.PersonName ?? string.Empty,
                    NationalCode = x.NationalID
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetBankAccountsByBankIdAsync(Guid? bankId)
        {
            if (!bankId.HasValue)
                return new List<SelectListItem>();

            return await _context.BankAccounts
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.BankId == bankId.Value)
                .OrderBy(x => x.BankAccountTitle)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BankAccountTitle
                })
                .ToListAsync();
        }

        private async Task FillLookupListsAsync(PaymentDocumentCreateEditVm model)
        {
            model.Companies = await _context.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName,
                    Selected = model.CompanyId.HasValue && model.CompanyId.Value == x.Id
                })
                .ToListAsync();


            model.Branches = await _context.Branches
                .Include(x => x.Parent)
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Parent.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BranchName + "(" + x.Parent.PersonName + ")",
                    Selected = model.BranchId.HasValue && model.BranchId == x.Id
                })
                .ToListAsync();




            model.Currencies = await _context.Currencies
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyName,
                    Selected = model.CurrencyId.HasValue && model.CurrencyId.Value == x.Id
                })
                .ToListAsync();

            model.FinanceDirections = await _context.FinanceDirections
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PaymentDocumentContextName,
                    Selected = model.FinanceDirectionId.HasValue && model.FinanceDirectionId.Value == x.Id
                })
                .ToListAsync();

            model.PaymentMethods = await _context.PaymentMethods
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PaymentMethodName,
                    Selected = model.PaymentMethodId.HasValue && model.PaymentMethodId.Value == x.Id
                })
                .ToListAsync();

            model.Banks = await _context.Banks
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.BankName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BankName,
                    Selected = model.Cheque.BankId.HasValue && model.Cheque.BankId == x.Id
                })
                .ToListAsync();

            model.ChequeTypes = await _context.ChequeTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChequeTypeName + "-" + x.ChequeTypeLocalName,
                    Selected = model.Cheque.ChequeTypeId.HasValue && model.Cheque.ChequeTypeId.Value == x.Id
                })
                .ToListAsync();


            model.ChequePurposes = await _context.ChequePurposes
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChequePurposeName + "-" + x.ChequePurposeLocalName,
                    Selected = model.Cheque.ChequePurposeId.HasValue && model.Cheque.ChequePurposeId.Value == x.Id
                })
                .ToListAsync();


            model.BankAccounts = await GetBankAccountsByBankIdAsync(model.Cheque.BankId);
            if (model.Cheque.BankAccountId.HasValue)
            {
                foreach (var item in model.BankAccounts)
                {
                    item.Selected = item.Value == model.Cheque.BankAccountId.Value.ToString();
                }
            }
        }

        private async Task<List<string>> ValidateBusinessAsync(PaymentDocumentCreateEditVm model)
        {
            var errors = new List<string>();

            //if (string.IsNullOrWhiteSpace(model.DocumentDateShamsi) || !ParseShamsiToGregorian(model.DocumentDateShamsi).HasValue)
            //{
            //    errors.Add("Document date is invalid.");
            //}

            if (model.CompanyId.HasValue)
            {
                var companyExists = await _context.Companies.AnyAsync(x => x.Id == model.CompanyId.Value && !x.IsDeleted);
                if (!companyExists)
                    errors.Add("Selected company is invalid.");
            }

            if (model.PersonId.HasValue)
            {
                var personExists = await _context.Persons.AnyAsync(x => x.Id == model.PersonId.Value && !x.IsDeleted);
                if (!personExists)
                    errors.Add("Selected person is invalid.");
            }

            if (model.CurrencyId.HasValue)
            {
                var currencyExists = await _context.Currencies.AnyAsync(x => x.Id == model.CurrencyId.Value && !x.IsDeleted);
                if (!currencyExists)
                    errors.Add("Selected currency is invalid.");
            }

            if (model.FinanceDirectionId.HasValue)
            {
                var directionExists = await _context.FinanceDirections.AnyAsync(x => x.Id == model.FinanceDirectionId.Value && !x.IsDeleted);
                if (!directionExists)
                    errors.Add("Selected finance direction is invalid.");
            }

            if (model.PaymentMethodId.HasValue)
            {
                var paymentMethodExists = await _context.PaymentMethods.AnyAsync(x => x.Id == model.PaymentMethodId.Value && !x.IsDeleted);
                if (!paymentMethodExists)
                    errors.Add("Selected payment method is invalid.");
            }

            if (RequiresCheque(model.PaymentMethodId))
            {
                if (string.IsNullOrWhiteSpace(model.Cheque.ChequeDateShamsi) || !ParseShamsiToGregorian(model.Cheque.ChequeDateShamsi).HasValue)
                {
                    errors.Add("Cheque date is invalid.");
                }

                if (model.Cheque.BankId.HasValue)
                {
                    var bankExists = await _context.Banks.AnyAsync(x => x.Id == model.Cheque.BankId.Value && !x.IsDeleted);
                    if (!bankExists)
                        errors.Add("Selected bank is invalid.");
                }

                if (model.Cheque.BankAccountId.HasValue)
                {
                    var bankAccountExists = await _context.BankAccounts.AnyAsync(x =>
                        x.Id == model.Cheque.BankAccountId.Value &&
                        !x.IsDeleted &&
                        (!model.Cheque.BankId.HasValue || x.BankId == model.Cheque.BankId.Value));

                    if (!bankAccountExists)
                        errors.Add("Selected bank account is invalid.");
                }

                if (model.Cheque.ChequeTypeId.HasValue)
                {
                    var chequeTypeExists = await _context.ChequeTypes.AnyAsync(x => x.Id == model.Cheque.ChequeTypeId.Value && !x.IsDeleted);
                    if (!chequeTypeExists)
                        errors.Add("Selected cheque type is invalid.");
                }
            }
            return errors;
        }
        private static void NormalizeModel(PaymentDocumentCreateEditVm model)
        {
            model.DocumentNumber = model.DocumentNumber?.Trim() ?? string.Empty;
            model.DocumentDateShamsi = model.DocumentDateShamsi?.Trim() ?? string.Empty;
            model.TrackingNumber = model.TrackingNumber?.Trim();
            model.Description = model.Description?.Trim();
            model.PersonName = model.PersonName?.Trim();

            model.Cheque ??= new ChequeFormVm();
            model.Cheque.ChequeNumber = model.Cheque.ChequeNumber?.Trim();
            model.Cheque.ChequeDateShamsi = model.Cheque.ChequeDateShamsi?.Trim();
            model.Cheque.AccountHolderName = model.Cheque.AccountHolderName?.Trim();
            model.Cheque.Description = model.Cheque.Description?.Trim();
        }
        private static bool RequiresCheque(int? paymentMethodId)
        {
            return paymentMethodId.HasValue && paymentMethodId.Value == ChequePaymentMethodId;
        }
        private static DateTime? ParseShamsiToGregorian(string? shamsiDate)
        {
            if (string.IsNullOrWhiteSpace(shamsiDate))
                return null;

            try
            {
                return PersianDateHelper.ToGregorian(shamsiDate);
            }
            catch
            {
                return null;
            }
        }
        private Cheque BuildChequeEntityFromModel(ChequeFormVm model, string? currentUserId)
        {
            var chequeDate = ParseShamsiToGregorian(model.ChequeDateShamsi);
            return new Cheque
            {
                ChequeNo = model.ChequeNumber?.Trim(),
                DueDate = chequeDate.GetValueOrDefault(),
                ChequeTypeId = model.ChequeTypeId.Value,
                ChequePurposeId = model.ChequePurposeId.Value,
                BankId = model.BankId.Value,
                BankAccountNo = model.BankAccountNo,
                IssuerName = model.AccountHolderName?.Trim(),
                SayadNo = model.SayadNo,
                IBAN = model.IBAN,
                Remarks = "-",
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                StatusId = 10  //Draft
            };
        }
        private void UpdateChequeEntityFromModel(Cheque entity, ChequeFormVm model, string? currentUserId)
        {
            var chequeDate = ParseShamsiToGregorian(model.ChequeDateShamsi);

            entity.ChequeNo = model.ChequeNumber?.Trim();
            entity.DueDate = chequeDate.GetValueOrDefault();
            entity.ChequeTypeId = model.ChequeTypeId.Value;
            entity.ChequePurposeId = model.ChequePurposeId.Value;
            entity.BankId = model.BankId.Value;
            //entity.BankAccountId = model.BankAccountId;
            entity.BankAccountNo = model.BankAccountNo;
            entity.IssuerName = model.AccountHolderName?.Trim();
            entity.Remarks = "-";
            entity.IsDeleted = false;
            entity.SayadNo = model.SayadNo;
            entity.IBAN = model.IBAN;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = Guid.Parse(currentUserId);
        }
        public async Task<IEnumerable<PaymentDocumentListItemVm>> GetEligibleAsync(int companyId, Guid partyId, int treasuryDocumentTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AvailablePaymentDocDto>> GetAvailablePaymentDocsByPartyAsync(Guid partyId)
        {
            var availablePaymentDocs = await _context.PaymentDocuments
                //.Where(p.UnallocatedAmount > 0)
                .Where(x => x.PartyId == partyId)
                .Select(p => new AvailablePaymentDocDto
                {
                    PaymentDocumentId = p.Id,
                    DocumentNo = p.DocumentNo,
                    DocumentDate = p.DocumentDate,
                    TotalAmount = p.Amount,
                    UnallocatedAmount = p.UnallocatedAmount,
                    CurrencyId = p.CurrencyId,
                    CurrencyCode = p.Currency.CurrencyCode,
                    PaymentMethodName = p.PaymentMethod.PaymentMethodName
                })
                .ToListAsync();
            return availablePaymentDocs;
        }
        public async Task<List<AvailablePaymentDocDto>> GetAvailablePaymentDocumentsAsync(
            Guid partyId,
            int? currencyId = null,
            int? companyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.PaymentDocuments
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.PartyId == partyId);
            // در صورتی که ارز مشخص شده باشد، اسناد با همان ارز فیلتر می‌شوند
            if (currencyId.HasValue && currencyId.Value > 0)
            {
                query = query.Where(p => p.CurrencyId == currencyId.Value);
            }
            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(p => p.CompanyId == companyId.Value);
            }
            // فیلتر اسنادی که مانده قابل تخصیص دارند
            // (از آنجایی که AllocatedAmount را Sync نگه می‌داریم، محاسبه به صورت Amount - AllocatedAmount است)
            query = query.Where(p => (p.Amount - p.AllocatedAmount) > 0);

            var result = await query
                .OrderByDescending(p => p.DocumentDate)
                .Select(p => new AvailablePaymentDocDto
                {
                    PaymentDocumentId = p.Id,
                    DocumentNo = p.DocumentNo,
                    DocumentDate = p.DocumentDate,
                    PaymentMethodName = p.PaymentMethod != null ? p.PaymentMethod.PaymentMethodName : "N/A",
                    TotalAmount = p.Amount,
                    UnallocatedAmount = p.Amount - p.AllocatedAmount,
                    CurrencyId = p.CurrencyId,
                    CurrencyCode = p.Currency != null ? p.Currency.CurrencyCode : ""
                })
                .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<bool> CanBeDeletedPaymentDocumentAsync(Guid id)
        {
            if (await _context.SettlementItemPaymentAllocations.AnyAsync(x => x.PaymentDocumentId == id) || await _context.ReturnCashRequests.AnyAsync(x => x.PaymentDocumentId == id)
                 || await _context.TreasuryDocumentItems.AnyAsync(x => x.PaymentDocumentId == id)) return false;
            if (await _context.VaultMovements.AnyAsync(x => x.SourceEntityId == id && x.VaultMovementSourceTypeId == 1)) return false;
            return true;
        }


        public async Task<PaymentDocumentUsagePanelVm?> GetUsagePanelAsync(
                Guid paymentDocumentId,
                CancellationToken cancellationToken = default)
        {
            var payment = await _context.PaymentDocuments
                .AsNoTracking()
                .Where(x => x.Id == paymentDocumentId)
                .Select(x => new PaymentDocumentUsagePanelVm
                {
                    PaymentDocumentId = x.Id,
                    PaymentDocumentNumber = x.DocumentNo,
                    Amount = x.Amount,
                    CurrencyTitle = x.Currency.CurrencyCode
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (payment is null)
                return null;

            payment.Items = await _context.SettlementItemPaymentAllocations
                .AsNoTracking()
                .Where(x => x.PaymentDocumentId == paymentDocumentId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new PaymentDocumentUsageItemVm
                {
                    UsageType = "Settlement",

                    ReferenceNumber = x.Settlement.SettlementNo.ToString(),
                    ReferenceDate = x.Settlement.CreatedAt.ToString(),
                    PartyName = x.Settlement.Party.PersonName,
                    AllocatedAmount = x.AllocatedAmount,
                    CurrencyTitle = payment.CurrencyTitle,
                    Description = x.Remarks
                })
                .ToListAsync(cancellationToken);

            payment.AllocatedAmount = payment.Items.Sum(x => x.AllocatedAmount);
            payment.RemainingAmount = payment.Amount - payment.AllocatedAmount;
            for (var index = 0; index < payment.Items.Count; index++)
            {
                payment.Items[index].RowNumber = index + 1;
            }
            return payment;
        }

        public async Task<List<PaymentDocumentLookupDto>> GetAvailablePaymentDocumentsForVaultAsync()
        {
            return await _context.PaymentDocuments
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.PaymentMethodId == 1 &&
                            !_context.VaultMovements.Any(vm => vm.SourceEntityId == x.Id))
                .OrderByDescending(x => x.DocumentDate)
                .Select(x => new PaymentDocumentLookupDto
                {
                    Id = x.Id,
                    DocumentNo = x.DocumentNo ?? x.TrackingNo,
                    PayerName = x.Party != null ? x.Party.PersonName : "-",
                    DocumentDate = x.DocumentDate,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.Currency != null ? x.Currency.CurrencyCode : "N/A"
                })
                .ToListAsync();
        }


    }
}
