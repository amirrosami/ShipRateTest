using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Finance
{
    public class SettlementService : ISettlementService
    {
        private readonly ApplicationDbContext _context;

        public SettlementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<SettlementListItemDto>> SearchAsync(SettlementSearchDto input)
        {
            if (input.Page <= 0)
                input.Page = 1;

            if (input.PageSize is not (10 or 20 or 50))
                input.PageSize = 10;

            var query = _context.Settlements
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (input.SettlementNo.HasValue)
            {
                query = query.Where(x => x.SettlementNo == input.SettlementNo.Value);
            }

            if (input.SettlementDateFrom.HasValue)
            {
                var from = input.SettlementDateFrom.Value.Date;
                query = query.Where(x => x.SettlementDate >= from);
            }

            if (input.SettlementDateTo.HasValue)
            {
                var to = input.SettlementDateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.SettlementDate <= to);
            }

            if (input.PartyId.HasValue)
            {
                query = query.Where(x => x.PartyId == input.PartyId.Value);
            }

            if (input.StatusId.HasValue)
            {
                query = query.Where(x => x.StatusId == input.StatusId.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.InvoiceNo))
            {
                var invoiceNo = input.InvoiceNo.Trim();

                query = query.Where(x =>
                    _context.SettlementItems.Any(si =>
                        si.SettlementId == x.Id &&
                        si.InvoiceId != null &&
                        _context.Invoices.Any(inv =>
                            inv.Id == si.InvoiceId &&
                            inv.InvoiceNo.Contains(invoiceNo))));
            }

            if (!string.IsNullOrWhiteSpace(input.PaymentDocumentNo))
            {
                var docNo = input.PaymentDocumentNo.Trim();

                query = query.Where(x =>
                    _context.SettlementItemPaymentAllocations.Any(pa =>
                        pa.SettlementId == x.Id &&
                        _context.PaymentDocuments.Any(pd =>
                            pd.Id == pa.PaymentDocumentId &&
                            pd.DocumentNo.Contains(docNo))));
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.SettlementDate)
                .ThenByDescending(x => x.SettlementNo)
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new SettlementListItemDto
                {
                    Id = x.Id,
                    SettlementNo = x.SettlementNo,
                    SettlementDate = x.SettlementDate,
                    CompanyId = x.CompanyId,
                    CompanyName = _context.Companies
                        .Include(x => x.Person)
                        .Where(c => c.Id == x.CompanyId)
                        .Select(c => c.Person.PersonName)
                        .FirstOrDefault() ?? "",

                    PartyId = x.PartyId,
                    PartyName = _context.Persons
                        .Where(p => p.Id == x.PartyId)
                        .Select(p => p.PersonName)
                        .FirstOrDefault() ?? "",

                    CurrencyId = x.CurrencyId,
                    CurrencyName = _context.Currencies
                        .Where(cu => cu.Id == x.CurrencyId)
                        .Select(cu => cu.CurrencyName)
                        .FirstOrDefault() ?? "",

                    StatusId = x.StatusId,
                    StatusName = _context.SettlementStatuses
                        .Where(st => st.Id == x.StatusId)
                        .Select(st => st.SettlementStatusName)
                        .FirstOrDefault() ?? "",

                    TotalSettlementAmount = _context.SettlementItems
                        .Where(si => si.SettlementId == x.Id && !si.IsDeleted)
                        .Sum(si => (decimal?)si.Amount) ?? 0,

                    TotalAllocatedAmount = _context.SettlementItemPaymentAllocations
                        .Where(pa => pa.SettlementId == x.Id && !pa.IsDeleted)
                        .Sum(pa => (decimal?)pa.AllocatedAmount) ?? 0,

                    Remarks = x.Remarks,
                    CreateDate = x.CreatedAt,
                    CreateUserName = x.CreateUser.FullName
                })
                .ToListAsync();

            return new PagedResultDto<SettlementListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = input.Page,
                PageSize = input.PageSize
            };
        }

        public Task<SettlementHeaderCreateDto> GetCreateModelAsync()
        {
            return Task.FromResult(new SettlementHeaderCreateDto
            {
                SettlementDate = DateTime.Today,
                StatusId = 1
            });
        }

        public async Task<Guid> CreateHeaderAsync(SettlementHeaderCreateDto input, Guid userId)
        {
            var settlementNo = await GenerateSettlementNoAsync(input.CompanyId);

            var entity = new Settlement
            {
                Id = Guid.CreateVersion7(),
                CompanyId = input.CompanyId,
                SettlementNo = settlementNo,
                SettlementDate = input.SettlementDate,
                PartyId = input.PartyId,
                CurrencyId = input.CurrencyId,
                StatusId = 1,
                Remarks = input.Remarks ?? "",
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            _context.Settlements.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<SettlementDetailsDto?> GetDetailsAsync(Guid id)
        {
            var header = await GetHeaderForEditAsync(id);

            if (header == null)
                return null;

            return new SettlementDetailsDto
            {
                Header = header,
                Items = await GetItemsAsync(id),
                PaymentAllocations = await GetPaymentAllocationsAsync(id)
            };
        }

        public async Task<SettlementHeaderEditDto?> GetHeaderForEditAsync(Guid id)
        {
            return await _context.Settlements
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new SettlementHeaderEditDto
                {
                    Id = x.Id,
                    SettlementNo = x.SettlementNo,
                    CompanyId = x.CompanyId,
                    SettlementDate = x.SettlementDate,
                    PartyId = x.PartyId,
                    PartyName = _context.Persons
                        .Where(p => p.Id == x.PartyId)
                        .Select(p => p.PersonName)
                        .FirstOrDefault(),
                    CurrencyId = x.CurrencyId,
                    StatusId = x.StatusId,
                    Remarks = x.Remarks
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateHeaderAsync(SettlementHeaderEditDto input, Guid userId)
        {
            var entity = await _context.Settlements
                .FirstOrDefaultAsync(x => x.Id == input.Id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Settlement not found.");

            entity.CompanyId = input.CompanyId;
            entity.SettlementDate = input.SettlementDate;
            entity.PartyId = input.PartyId;
            entity.CurrencyId = input.CurrencyId;
            //entity.StatusId = input.StatusId;
            entity.Remarks = input.Remarks ?? "";
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;
            await _context.SaveChangesAsync();
        }
        public async Task<List<SettlementItemDto>> GetItemsAsync(Guid settlementId)
        {
            return await _context.SettlementItems
                .AsNoTracking()
                .Where(x => x.SettlementId == settlementId && !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new SettlementItemDto
                {
                    Id = x.Id,
                    SettlementId = x.SettlementId,
                    InvoiceId = x.InvoiceId,
                    InvoiceNo = x.InvoiceId == null
                        ? null
                        : _context.Invoices
                            .Where(i => i.Id == x.InvoiceId)
                            .Select(i => i.InvoiceNo)
                            .FirstOrDefault(),

                    InvoiceItemId = x.InvoiceItemId,
                    ChargeId = x.ChargeId,
                    ChargeDescription = x.ChargeId == null
                        ? null
                        : _context.Charges
                            .Where(c => c.Id == x.ChargeId)
                            .Select(c => c.ChargeDescription)
                            .FirstOrDefault(),

                    Amount = x.Amount,
                    AmountInBaseCurrency = x.AmountInBaseCurrency,
                    BaseExchangeRate = x.BaseExchangeRate,
                    Remarks = x.Remarks
                })
                .ToListAsync();
        }

        public async Task<Guid> AddItemAsync(SettlementItemCreateDto input, Guid userId)
        {
            var settlementExists = await _context.Settlements
                .AnyAsync(x => x.Id == input.SettlementId && !x.IsDeleted);

            if (!settlementExists)
                throw new Exception("Settlement not found.");

            if (input.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            if (input.InvoiceId == null && input.InvoiceItemId == null && input.ChargeId == null)
                throw new Exception("Invoice, Invoice Item or Charge is required.");

            var entity = new SettlementItem
            {
                Id = Guid.CreateVersion7(),
                SettlementId = input.SettlementId,
                InvoiceId = input.InvoiceId,
                InvoiceItemId = input.InvoiceItemId,
                ChargeId = input.ChargeId,
                Amount = input.Amount,
                AmountInBaseCurrency = input.AmountInBaseCurrency,
                BaseExchangeRate = input.BaseExchangeRate,
                Remarks = input.Remarks ?? "",
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            _context.SettlementItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var entity = await _context.SettlementItems.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            var hasAllocation = await _context.SettlementItemPaymentAllocations
                .AnyAsync(x => x.SettlementItemId == id && !x.IsDeleted);

            if (hasAllocation)
                throw new Exception("This item has payment allocations and cannot be deleted.");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<PaymentAllocationDto>> GetPaymentAllocationsAsync(Guid settlementId)
        {
            return await _context.SettlementItemPaymentAllocations
                .AsNoTracking()
                .Where(x => x.SettlementId == settlementId && !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new PaymentAllocationDto
                {
                    Id = x.Id,
                    SettlementId = x.SettlementId,
                    SettlementItemId = x.SettlementItemId,
                    PaymentDocumentId = x.PaymentDocumentId,

                    PaymentDocumentNo = _context.PaymentDocuments
                        .Where(p => p.Id == x.PaymentDocumentId)
                        .Select(p => p.DocumentNo)
                        .FirstOrDefault() ?? "",

                    PaymentDocumentDate = _context.PaymentDocuments
                        .Where(p => p.Id == x.PaymentDocumentId)
                        .Select(p => p.DocumentDate)
                        .FirstOrDefault(),

                    PaymentDocumentAmount = _context.PaymentDocuments
                        .Where(p => p.Id == x.PaymentDocumentId)
                        .Select(p => p.Amount)
                        .FirstOrDefault(),

                    AllocatedAmount = x.AllocatedAmount,
                    SettledAmountInItemCurrency = x.SettledAmountInItemCurrency,
                    ExchangeRateValue = x.ExchangeRateValue,
                    AllocatedAmountInBaseCurrency = x.AllocatedAmountInBaseCurrency,
                    Remarks = x.Remarks
                })
                .ToListAsync();
        }

        public async Task AddPaymentAllocationAsync(PaymentAllocationCreateDto input, Guid userId)
        {
            if (input.AllocatedAmount <= 0)
                throw new Exception("Allocated amount must be greater than zero.");

            var item = await _context.SettlementItems
                .FirstOrDefaultAsync(x => x.Id == input.SettlementItemId && !x.IsDeleted);

            if (item == null)
                throw new Exception("Settlement item not found.");

            if (item.SettlementId != input.SettlementId)
                throw new Exception("Invalid settlement item.");

            var payment = await _context.PaymentDocuments
                .FirstOrDefaultAsync(x => x.Id == input.PaymentDocumentId && !x.IsDeleted);

            if (payment == null)
                throw new Exception("Payment document not found.");

            var alreadyAllocated = await _context.SettlementItemPaymentAllocations
                .Where(x => x.PaymentDocumentId == input.PaymentDocumentId && !x.IsDeleted)
                .SumAsync(x => (decimal?)x.AllocatedAmount) ?? 0;

            if (alreadyAllocated + input.AllocatedAmount > payment.Amount)
                throw new Exception("Allocated amount is greater than available payment amount.");

            var itemRemaining = item.Amount -
                await _context.SettlementItemPaymentAllocations
                .Where(x => x.SettlementItemId == item.Id && !x.IsDeleted)
                .SumAsync(x => (decimal?)x.AllocatedAmount) ?? 0;

            if (input.AllocatedAmount > itemRemaining)
            {
                throw new Exception("Allocated amount exceeds remaining settlement item amount.");
            }



            var entity = new SettlementItemPaymentAllocation
            {
                Id = Guid.CreateVersion7(),
                SettlementId = input.SettlementId,
                SettlementItemId = input.SettlementItemId,
                PaymentDocumentId = input.PaymentDocumentId,
                AllocatedAmount = input.AllocatedAmount,
                SettledAmountInItemCurrency = input.SettledAmountInItemCurrency,
                ExchangeRateId = input.ExchangeRateId,
                ExchangeRateBaseId = input.ExchangeRateBaseId,
                ExchangeRateDate = input.ExchangeRateDate,
                ExchangeRateValue = input.ExchangeRateValue,
                AllocatedAmountInBaseCurrency = input.AllocatedAmountInBaseCurrency,
                Remarks = input.Remarks ?? "",
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            _context.SettlementItemPaymentAllocations.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentAllocationAsync(Guid id)
        {
            var entity = await _context.SettlementItemPaymentAllocations.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<DTOs.Finance.LookupDto>> SearchPartiesAsync(string term)
        {
            term = term?.Trim() ?? "";

            if (term.Length < 2)
                return new List<DTOs.Finance.LookupDto>();

            return await _context.Persons
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    (
                        x.PersonName.Contains(term) ||
                        x.PersonPersianName.Contains(term) ||
                        x.NationalID.Contains(term) ||
                        x.Mobile.Contains(term)
                    ))
                .OrderBy(x => x.PersonName)
                .Take(20)
                .Select(x => new LookupDto
                {
                    Id = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();
        }

        public async Task<List<LookupDto>> SearchInvoicesAsync(Guid? partyId, string term)
        {
            term = term?.Trim() ?? "";

            var query = _context.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (partyId.HasValue)
                query = query.Where(x => x.PartyId == partyId.Value);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(x => x.InvoiceNo.Contains(term));

            return await query
                .OrderByDescending(x => x.InvoiceDate)
                .Take(20)
                .Select(x => new LookupDto
                {
                    Id = x.Id.ToString(),
                    Text = x.InvoiceNo + " - " + x.TotalAmount
                })
                .ToListAsync();
        }

        public async Task<List<LookupDto>> SearchPaymentDocumentsAsync(Guid? partyId, string term)
        {
            term = term?.Trim() ?? "";

            var query = _context.PaymentDocuments
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (partyId.HasValue)
                query = query.Where(x => x.PartyId == partyId.Value);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(x => x.DocumentNo.Contains(term));

            return await query
                .OrderByDescending(x => x.DocumentDate)
                .Take(20)
                .Select(x => new LookupDto
                {
                    Id = x.Id.ToString(),
                    Text = x.DocumentNo + " - " + x.Amount
                })
                .ToListAsync();
        }

        public async Task<List<LookupDto>> SearchChargesAsync(Guid? partyId, string term)
        {
            term = term?.Trim() ?? "";

            var query = _context.Charges
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (partyId.HasValue)
                query = query.Where(x => x.PartyId == partyId.Value);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(x => x.ChargeDescription.Contains(term));

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .Select(x => new LookupDto
                {
                    Id = x.Id.ToString(),
                    Text = x.ChargeDescription + " - " + x.Amount
                })
                .ToListAsync();
        }

        private async Task<int> GenerateSettlementNoAsync(int companyId)
        {
            var lastNo = await _context.Settlements
                .Where(x => x.CompanyId == companyId)
                .MaxAsync(x => (int?)x.SettlementNo) ?? 0;

            return lastNo + 1;
        }


        public async Task<PaymentAllocationFormDto> GetPaymentAllocationForm(Guid settlementId)
        {
            var items = await _context.SettlementItems
                .Where(x => x.SettlementId == settlementId && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.Amount,
                    InvoiceNo = _context.Invoices
                        .Where(i => i.Id == x.InvoiceId)
                        .Select(i => i.InvoiceNo)
                        .FirstOrDefault(),

                    ChargeDescription = _context.Charges
                        .Where(c => c.Id == x.ChargeId)
                        .Select(c => c.ChargeDescription)
                        .FirstOrDefault(),

                    Allocated = _context.SettlementItemPaymentAllocations
                        .Where(p => p.SettlementItemId == x.Id && !p.IsDeleted)
                        .Sum(p => (decimal?)p.AllocatedAmount) ?? 0
                })
                .ToListAsync();

            var list = items.Select(x => new SettlementItemLookupDto
            {
                Id = x.Id,
                Description = (x.InvoiceNo ?? "") + " " + (x.ChargeDescription ?? ""),
                ItemAmount = x.Amount,
                AllocatedAmount = x.Allocated,
                RemainingAmount = x.Amount - x.Allocated
            }).ToList();

            return new PaymentAllocationFormDto
            {
                SettlementId = settlementId,
                SettlementItems = list,
                Input = new PaymentAllocationCreateDto
                {
                    SettlementId = settlementId,
                    ExchangeRateValue = 1
                }
            };
        }


        public async Task<List<JobSettlementGridRowDto>> GetJobSettlementsAsync(Guid jobId)
        {
            var query = await (
                from si in _context.SettlementItems
                join s in _context.Settlements on si.SettlementId equals s.Id

                join i0 in _context.Invoices on si.InvoiceId equals i0.Id into invoiceGroup
                from i in invoiceGroup.DefaultIfEmpty()

                join c0 in _context.Charges on si.ChargeId equals c0.Id into chargeGroup
                from c in chargeGroup.DefaultIfEmpty()

                join pa0 in _context.SettlementItemPaymentAllocations
                    .Where(x => !x.IsDeleted)
                    on si.Id equals pa0.SettlementItemId into allocationGroup
                from pa in allocationGroup.DefaultIfEmpty()

                join pd0 in _context.PaymentDocuments
                    on pa.PaymentDocumentId equals pd0.Id into paymentDocGroup
                from pd in paymentDocGroup.DefaultIfEmpty()

                where !si.IsDeleted
                      && !s.IsDeleted
                      && i != null
                      && i.JobId == jobId

                select new JobSettlementGridRowDto
                {
                    SettlementItemId = si.Id,
                    SettlementId = s.Id,
                    SettlementNo = s.SettlementNo.ToString(),
                    InvoiceNo = i != null ? i.InvoiceNo : "",
                    ChargeName = c != null ? c.ChargeDescription : "",
                    PaymentDocumentNo = pd != null ? pd.DocumentNo : "",
                    SettledAmount = pa != null ? pa.SettledAmountInItemCurrency : 0
                })
                .ToListAsync();

            return query;
        }

        public async Task<List<JobSettlementDetailDto>> GetSettlementItemDetailsAsync(Guid settlementItemId)
        {
            var result = await (
                from si in _context.SettlementItems

                join s in _context.Settlements
                    on si.SettlementId equals s.Id

                join i0 in _context.Invoices
                    on si.InvoiceId equals i0.Id into invoiceGroup
                from i in invoiceGroup.DefaultIfEmpty()

                join c0 in _context.Charges
                    on si.ChargeId equals c0.Id into chargeGroup
                from c in chargeGroup.DefaultIfEmpty()

                join pa0 in _context.SettlementItemPaymentAllocations.Where(x => !x.IsDeleted)
                    on si.Id equals pa0.SettlementItemId into paGroup
                from pa in paGroup.DefaultIfEmpty()

                from pd in _context.PaymentDocuments
                    .Where(x => pa != null
                                && x.Id == pa.PaymentDocumentId
                                && !x.IsDeleted)
                    .DefaultIfEmpty()

                where si.Id == settlementItemId
                      && !si.IsDeleted
                      && !s.IsDeleted

                select new JobSettlementDetailDto
                {
                    SettlementId = s.Id,
                    SettlementItemId = si.Id,
                    SettlementNo = s.SettlementNo.ToString(),
                    SettlementDate = s.SettlementDate,

                    InvoiceNo = i != null ? i.InvoiceNo : "",
                    ChargeName = c != null ? c.ChargeDescription : "",

                    ItemAmount = si.Amount,

                    ItemType = i != null ? "Invoice" : "Charge",
                    ItemTitle = i != null
                    ? i.InvoiceNo
                    : c != null
                        ? c.ChargeDescription
                        : "",


                    PaymentDocumentNo = pd != null ? pd.DocumentNo : "",

                    AllocatedAmount = pa != null ? pa.AllocatedAmount : 0,
                    SettledAmount = pa != null ? pa.SettledAmountInItemCurrency : 0,
                    ExchangeRate = pa != null ? pa.ExchangeRateValue : 0,
                    Remarks = pa != null ? pa.Remarks : ""
                })
                .ToListAsync();

            return result;
        }

        public async Task<OperationResult> ApproveSettlementAsync(Guid settlementId, Guid currentUserId, CancellationToken cancellationToken = default)
        {
            // ۱. دریافت رکورد تسویه به همراه تراکنش/وابستگی‌های مرتبط در صورت نیاز
            var settlement = await _context.Settlements
                .Include(s => s.Items) // در صورت وجود آیتم‌ها یا فاکتورهای متصل
                .FirstOrDefaultAsync(s => s.Id == settlementId, cancellationToken);

            if (settlement is null)
            {
                return OperationResult.Failure("Settlement Not Found!!!!");
            }

            // ۲. اعتبارسنجی وضعیت (فقط تسویه‌های Draft یا وضعیت 1 قابل Approve هستند)
            if (settlement.StatusId != 1) // یا SettlementStatus.Draft
            {
                return OperationResult.Failure("Only Draft Settlements Can Be Approved");
            }


            try
            {
                // ۴. تغییر وضعیت به Approved (2)
                settlement.StatusId = 2; // SettlementStatus.Approved
                //settlement.ApprovedAt = DateTime.Now;
                //settlement.ApprovedByUserId = currentUserId;

                // در صورت نیاز به ردیابی Auditing:
                settlement.UpdatedAt = DateTime.Now;
                settlement.EditUserId = currentUserId;

                // ۵. در صورت نیاز به به‌روزرسانی فاکتورها، تراکنش‌های بانکی یا Ledger
                /*
                foreach (var item in settlement.SettlementItems)
                {
                    // مثلاً تغییر وضعیت فاکتور مربوطه به Paid یا Settled
                }
                */

                await _context.SaveChangesAsync(cancellationToken);

                return OperationResult.Success("Settlement Approved Successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure("Concurrency Error");
            }
            catch (Exception ex)
            {
                // لاگ کردن خطا
                return OperationResult.Failure("An Error Occurred While Approving Settlement");
            }
        }




        public async Task<SettlementReceiptReportDto?> GetSettlementReceiptReportDataAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var settlement = await _context.Settlements
                .AsNoTracking()
                .Include(s => s.Company).ThenInclude(c => c.Person)
                .Include(s => s.Party)
                .Include(s => s.Currency)
                .Include(s => s.Status)
                .Include(s => s.Items).ThenInclude(i => i.Invoice)
                .Include(s => s.Items).ThenInclude(i => i.Charge)
                .Include(s => s.Items).ThenInclude(i => i.Detention)
                .Include(s => s.Items).ThenInclude(i => i.PaymentAllocations)
                .Include(s => s.PaymentAllocations).ThenInclude(p => p.PaymentDocument).ThenInclude(x => x.PaymentMethod)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

            if (settlement == null)
                return null;

            var totalItemsAmount = settlement.Items.Sum(x => x.Amount);
            var totalAllocatedAmount = settlement.PaymentAllocations.Sum(x => x.AllocatedAmount);
            var totalSettledInItemCurrency = settlement.PaymentAllocations.Sum(x => x.SettledAmountInItemCurrency);

            var dto = new SettlementReceiptReportDto
            {
                CompanyId = settlement.CompanyId,
                CompanyName = settlement.Company?.Person?.PersonName ?? "شرکت حمل و نقل بین‌المللی",
                SettlementNo = settlement.SettlementNo,
                SettlementDate = settlement.SettlementDate.ToString("yyyy/MM/dd"),
                PartyName = settlement.Party?.PersonName ?? "-",
                CurrencyName = settlement.Currency?.CurrencyCode ?? "-",
                StatusName = settlement.Status?.SettlementStatusName ?? "-",
                Remarks = settlement.Remarks ?? string.Empty,
                TotalAmount = totalItemsAmount,
                TotalAllocated = totalAllocatedAmount,
                RemainingAmount = totalItemsAmount - totalSettledInItemCurrency,
                Items = settlement.Items.Select((x, idx) => new SettlementReceiptItemDto
                {
                    RowIndex = idx + 1,
                    InvoiceNo = x.Invoice?.InvoiceNo ?? x.Detention?.DetentionNo.ToString() ?? "-",
                    //ChargeTitle = x.Charge?.ChargeDescription ?? (!string.IsNullOrEmpty(x.Remarks) ? x.Remarks : "بابت فاکتور / هزینه"),
                    ChargeTitle = x.Invoice != null ? "فاکتور" : x.Detention != null ? "دیتنشن" : "-",
                    Amount = x.Amount,
                    CoveredAmount = x.CoveredAmountInItemCurrency,
                    RemainingAmount = x.RemainingAmountInItemCurrency,
                    Remarks = x.Remarks ?? string.Empty
                }).ToList(),
                Payments = settlement.PaymentAllocations.Select((p, idx) => new SettlementReceiptPaymentDto
                {
                    RowIndex = idx + 1,
                    DocumentNo = p.PaymentDocument?.DocumentNo ?? "-",
                    DocumentDate = p.PaymentDocument?.DocumentDate.ToString("yyyy/MM/dd") ?? "-",
                    PaymentMethod = p.PaymentDocument?.PaymentMethod.PaymentMethodName ?? "سند مالی / حواله",
                    AllocatedAmount = p.AllocatedAmount,
                    SettledInItemCurrency = p.SettledAmountInItemCurrency,
                    ExchangeRate = p.ExchangeRateValue,
                    Remarks = p.Remarks ?? string.Empty
                }).ToList()
            };

            return dto;
        }

        public async Task<List<InvoiceSettlementDto>> GetInvoicesForSettlementAsync(List<Guid> invoiceIds)
        {
            // استفاده از Projection برای واکشی فقط ستون‌های مورد نیاز جهت افزایش سرعت
            return await _context.Invoices
                .Where(x => invoiceIds.Contains(x.Id))
                .Select(x => new InvoiceSettlementDto
                {
                    Id = x.Id,
                    InvoiceNo = x.SerialDisplay,
                    TotalAmount = x.TotalAmount,
                    // اگر RemainingAmount فیلد محاسبه شده در دیتابیس نیست، 
                    // در اینجا می‌توانید از محاسبه دستی استفاده کنید: (x.TotalAmount - x.PaidAmount)
                    RemainingAmount = x.TotalAmount,
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.Currency.CurrencyCode, // فرض بر این است که Navigation Property برقرار است
                    PartyId = x.PartyId.Value,
                    PartyName = x.Party.PersonName,
                    CompanyId = x.CompanyId
                })
                .ToListAsync();
        }
        public async Task SyncSettlementBalancesAsync(IEnumerable<Guid> invoiceIds, IEnumerable<Guid> paymentDocIds, IEnumerable<Guid> detentionIds)
        {
            // ۱. به‌روزرسانی فاکتورها
            if (invoiceIds.Any())
            {
                var invoices = await _context.Invoices
                    .Where(i => invoiceIds.Contains(i.Id))
                    .ToListAsync();

                var settledSums = await _context.SettlementItemPaymentAllocations
                    .Where(a => a.SettlementItem.InvoiceId != null
                             && invoiceIds.Contains(a.SettlementItem.InvoiceId.Value)
                             && a.Settlement.StatusId != 10)
                    .GroupBy(a => a.SettlementItem.InvoiceId.Value)
                    .Select(g => new { Id = g.Key, Total = g.Sum(x => x.SettledAmountInItemCurrency) })
                    .ToDictionaryAsync(x => x.Id, x => x.Total);

                foreach (var inv in invoices)
                {
                    inv.SettledAmount = settledSums.GetValueOrDefault(inv.Id, 0m);
                    inv.InvoiceStatusId = inv.SettledAmount >= inv.TotalAmount ? 5 : (inv.SettledAmount > 0 ? 4 : inv.InvoiceStatusId);
                }
            }

            // ۲. به‌روزرسانی Detentionها (جدید)
            if (detentionIds.Any())
            {
                var detentions = await _context.Detentions // فرض بر وجود جدول Detentions
                    .Where(d => detentionIds.Contains(d.Id))
                    .ToListAsync();

                var settledSums = await _context.SettlementItemPaymentAllocations
                    .Where(a => a.SettlementItem.DetentionId != null // فرض بر وجود این فیلد
                             && detentionIds.Contains(a.SettlementItem.DetentionId.Value)
                             && a.Settlement.StatusId != 10)
                    .GroupBy(a => a.SettlementItem.DetentionId.Value)
                    .Select(g => new { Id = g.Key, Total = g.Sum(x => x.SettledAmountInItemCurrency) })
                    .ToDictionaryAsync(x => x.Id, x => x.Total);

                foreach (var det in detentions)
                {
                    det.SettledAmount = settledSums.GetValueOrDefault(det.Id, 0m);
                }
            }

            // ۳. به‌روزرسانی اسناد پرداخت
            if (paymentDocIds.Any())
            {
                var paymentDocs = await _context.PaymentDocuments
                    .Where(p => paymentDocIds.Contains(p.Id))
                    .ToListAsync();

                var allocatedSums = await _context.SettlementItemPaymentAllocations
                    .Where(a => paymentDocIds.Contains(a.PaymentDocumentId)
                             && a.Settlement.StatusId != 10)
                    .GroupBy(a => a.PaymentDocumentId)
                    .Select(g => new { DocId = g.Key, TotalAllocated = g.Sum(x => x.AllocatedAmount) })
                    .ToDictionaryAsync(x => x.DocId, x => x.TotalAllocated);

                foreach (var doc in paymentDocs)
                {
                    doc.AllocatedAmount = allocatedSums.GetValueOrDefault(doc.Id, 0m);
                }
            }

            await _context.SaveChangesAsync();
        }

        //Task ISettlementService.AddItemAsync(SettlementItemCreateDto input, Guid userId)
        //{
        //    return AddItemAsync(input, userId);
        //}

        //public async Task SyncSettlementBalancesAsync(IEnumerable<Guid> invoiceIds, IEnumerable<Guid> paymentDocIds)
        //{
        //    // ۱. به‌روزرسانی فاکتورها
        //    if (invoiceIds.Any())
        //    {
        //        var invoices = await _context.Invoices
        //            .Where(i => invoiceIds.Contains(i.Id))
        //            .ToListAsync();

        //        var settledSums = await _context.SettlementItemPaymentAllocations
        //            .Where(a => a.SettlementItem.InvoiceId != null
        //                     && invoiceIds.Contains(a.SettlementItem.InvoiceId.Value)
        //                     && a.Settlement.StatusId != 10)  //لغو شده
        //            .GroupBy(a => a.SettlementItem.InvoiceId.Value)
        //            .Select(g => new { InvoiceId = g.Key, TotalSettled = g.Sum(x => x.SettledAmountInItemCurrency) })
        //            .ToDictionaryAsync(x => x.InvoiceId, x => x.TotalSettled);

        //        foreach (var inv in invoices)
        //        {
        //            inv.SettledAmount = settledSums.ContainsKey(inv.Id) ? settledSums[inv.Id] : 0m;
        //            if (inv.SettledAmount >= inv.TotalAmount)
        //                inv.InvoiceStatusId = 5; //پرداخت کامل انجام شده
        //            else if (inv.SettledAmount > 0)
        //                inv.InvoiceStatusId = 4; //بخشی پرداخت شده
        //        }
        //    }
        //    // ۲. به‌روزرسانی اسناد پرداخت
        //    if (paymentDocIds.Any())
        //    {
        //        var paymentDocs = await _context.PaymentDocuments
        //            .Where(p => paymentDocIds.Contains(p.Id))
        //            .ToListAsync();

        //        var allocatedSums = await _context.SettlementItemPaymentAllocations
        //            .Where(a => paymentDocIds.Contains(a.PaymentDocumentId)
        //                     && a.Settlement.StatusId != 10)  //لغو
        //            .GroupBy(a => a.PaymentDocumentId)
        //            .Select(g => new { DocId = g.Key, TotalAllocated = g.Sum(x => x.AllocatedAmount) })
        //            .ToDictionaryAsync(x => x.DocId, x => x.TotalAllocated);

        //        foreach (var doc in paymentDocs)
        //        {
        //            doc.AllocatedAmount = allocatedSums.ContainsKey(doc.Id) ? allocatedSums[doc.Id] : 0m;
        //        }
        //    }
        //    await _context.SaveChangesAsync();
        //}
    }
}
