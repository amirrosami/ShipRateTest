using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.DTOs.Reports;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Infrastructure.Data;
using ShipRate.Infrastructure.Repositories.Queries;
using System.Data;

namespace ShipRate.Application.Services.Finance
{
    public class InvoiceAppService : IInvoiceAppService
    {
        private readonly ApplicationDbContext _db;
        private readonly IRequestContext _requestContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly FinanceQueries financeQueries;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InvoiceAppService(ApplicationDbContext db, IRequestContext requestContext, ICurrentUserService currentUserService, IWebHostEnvironment webHostEnvironment, FinanceQueries financeQueries)
        {
            _db = db;
            _requestContext = requestContext;
            _currentUserService = currentUserService;
            this.financeQueries = financeQueries;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<InvoiceListPageVm> GetListPageAsync(InvoiceListFilterVm filter, CancellationToken cancellationToken = default)
        {
            filter.PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var invoices = _db.Invoices.AsNoTracking()
                //.Where(x => !x.IsDeleted && x.CompanyId == _requestContext.CompanyId);
                .Where(x => !x.IsDeleted);

            var isAgent = _currentUserService.IsInRole("External Party");
            if (isAgent)
            {
                var agentPersonId = await _currentUserService.GetPersonIdAsync();
                invoices = invoices.Where(x => x.CreateUserId == _currentUserService.UserId && x.PartyId == agentPersonId);
                //این هم میشه گذاشت 
            }
            if (!string.IsNullOrWhiteSpace(filter.InvoiceNo))
                invoices = invoices.Where(x => x.InvoiceNo.Contains(filter.InvoiceNo));

            if (filter.InvoiceTypeId.HasValue)
                invoices = invoices.Where(x => x.InvoiceTypeId == filter.InvoiceTypeId.Value);

            if (filter.JobId.HasValue)
                invoices = invoices.Where(x => x.JobId == filter.JobId.Value);

            if (filter.InvoiceStatusId.HasValue)
                invoices = invoices.Where(x => x.InvoiceStatusId == filter.InvoiceStatusId.Value);

            if (filter.FinanceDirectionId.HasValue)
                invoices = invoices.Where(x => x.FinanceDirectionId == filter.FinanceDirectionId.Value);

            if (filter.CurrencyId.HasValue)
                invoices = invoices.Where(x => x.CurrencyId == filter.CurrencyId.Value);

            if (filter.InvoiceDateFrom.HasValue)
                invoices = invoices.Where(x => x.InvoiceDate >= filter.InvoiceDateFrom.Value);

            if (filter.InvoiceDateTo.HasValue)
            {
                var toDate = filter.InvoiceDateTo.Value.Date.AddDays(1);
                invoices = invoices.Where(x => x.InvoiceDate < toDate);
            }

            var query =
                from i in invoices
                join p in _db.Persons.AsNoTracking().Where(x => !x.IsDeleted) on i.PartyId equals p.Id into partyJoin
                from p in partyJoin.DefaultIfEmpty()
                join t in _db.InvoiceTypes.AsNoTracking().Where(x => !x.IsDeleted) on i.InvoiceTypeId equals t.Id
                join fnf in _db.InvoiceFNFs.AsNoTracking().Where(x => !x.IsDeleted) on i.InvoiceFNFId equals fnf.Id
                join imodel in _db.InvoiceModels.AsNoTracking().Where(x => !x.IsDeleted) on i.InvoiceModelId equals imodel.Id
                join s in _db.InvoiceStatuses.AsNoTracking().Where(x => !x.IsDeleted) on i.InvoiceStatusId equals s.Id
                join subjects in _db.InvoiceSubjects.AsNoTracking().Where(x => !x.IsDeleted) on i.InvoiceSubjectId equals subjects.Id
                join f in _db.FinanceDirections.AsNoTracking().Where(x => !x.IsDeleted) on i.FinanceDirectionId equals f.Id
                join c in _db.Currencies.AsNoTracking().Where(x => !x.IsDeleted) on i.CurrencyId equals c.Id
                join u in _db.Users.AsNoTracking().Where(x => !x.IsDeleted) on i.CreateUserId equals u.Id
                join totalPaid in financeQueries.GetTotalInvoicePayment()
                 on i.Id equals totalPaid.InvoiceId
                join uu in _db.Users.AsNoTracking().Where(x => !x.IsDeleted) on i.EditUserId equals uu.Id into updateUser
                from uu in updateUser.DefaultIfEmpty()
                select new InvoiceListItemVm
                {
                    Id = i.Id,
                    InvoiceNo = i.InvoiceNo,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    PartyName = p != null ? p.PersonName : "",
                    InvoiceTypeName = t.InvoiceTypeName,
                    InvoiceStatusName = s.InvoiceStatusName,
                    FinanceDirectionName = f.InvoiceContextName,
                    CurrencyCode = c.CurrencyCode,
                    TotalAmount = i.TotalAmount,
                    FNFName = i.InvoiceFNF.FNFName,
                    CreateDateTime = i.CreatedAt,
                    CreateUserName = u.UserName,
                    UpdateDateTime = i.UpdatedAt,
                    UpdateUserName = uu.UserName,
                    InvoiceSubjectId = i.InvoiceSubjectId,
                    InvoiceSubjectName = subjects.InvoiceSubjectName,
                    InvoiceSubjectTitle = i.InvoiceSubjectTitle,
                    InvoiceModelId = i.InvoiceModelId,
                    InvoiceModelName = i.InvoiceModel.InvoiceModelName,
                    TotalPaid = totalPaid.TotalPaid ?? 0,
                    RemainPayment = i.TotalAmount - (totalPaid.TotalPaid ?? 0),
                    PartyId = i.PartyId.Value,
                    SettledAmount = i.SettledAmount,
                    RemainingAmount = i.RemainingAmount,
                    IsFullySettled = i.IsFullySettled,
                    BLNo = i.BL.BLNo,
                    JobFileNo = i.Job.JobFileNumber,
                    BookingNo = i.Booking.BookingNo,
                    Serial = i.Serial,
                    SerialPrefix = i.SerialPrefix,
                    CompanyName = i.Company.CompanyAbb,
                    DirectionId = i.FinanceDirectionId
                };
            if (!string.IsNullOrWhiteSpace(filter.PartyName))
                query = query.Where(x => x.PartyName.Contains(filter.PartyName));

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.InvoiceDate)
                .ThenByDescending(x => x.InvoiceNo)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);
            return new InvoiceListPageVm
            {
                Filter = filter,
                Result = new PagedResultVm<InvoiceListItemVm>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                },
                InvoiceTypes = await GetInvoiceTypesAsync(cancellationToken),
                InvoiceStatuses = await GetInvoiceStatusesAsync(cancellationToken),
                FinanceDirections = await GetFinanceDirectionsAsync(cancellationToken),
                Currencies = await GetCurrenciesAsync(cancellationToken)
            };
        }

        public async Task<InvoiceEditVm> GetCreateVmAsync(Guid? jobId, Guid? blId, Guid? bookingId, CancellationToken cancellationToken = default, string FilterType = "All")
        {
            var vm = new InvoiceEditVm
            {
                InvoiceDate = DateTime.Today,
                InvoicePDate = DateTime.Today,
                DueDate = DateTime.Today,
                ExchangeRate = 1,
                Remarks = string.Empty,
                Notes = string.Empty,
                InvoicePNo = string.Empty,
                InvoiceNo = string.Empty,
                FirstTotal = 0,
                Discount = 0,
                SubTotal = 0,
                TaxAmount = 0,
                TotalAmount = 0,
                JobId = jobId,
                BLId = blId,
                BookingId = bookingId
            };
            switch (FilterType)
            {
                case "BuyF":
                    vm.FNFId = 1;
                    vm.FinanceDirectionId = 2;
                    vm.InvoiceModelId = 1;
                    break;
                case "BuyNF":
                    vm.FNFId = 2;
                    vm.FinanceDirectionId = 2;
                    vm.InvoiceModelId = 1;
                    break;
                case "SellF":
                    vm.FNFId = 1;
                    vm.FinanceDirectionId = 1;
                    vm.InvoiceModelId = 1;
                    break;
                case "SellNF":
                    vm.FNFId = 2;
                    vm.FinanceDirectionId = 1;
                    vm.InvoiceModelId = 1;
                    break;
                case "PreInvoice":
                    vm.FNFId = 2;
                    vm.FinanceDirectionId = 1;
                    vm.InvoiceModelId = 2;
                    break;
                default:
                    break;
            }
            await FillHeaderLookupsAsync(vm, cancellationToken);
            return vm;
        }

        public async Task<InvoiceEditVm?> GetEditVmAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invoice = await _db.Invoices.AsNoTracking()
                //.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted && x.CompanyId == _requestContext.CompanyId, cancellationToken);
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                return null;

            string partyName = string.Empty;
            if (invoice.PartyId.HasValue)
            {
                partyName = await _db.Persons.AsNoTracking()
                    .Where(x => x.Id == invoice.PartyId.Value && !x.IsDeleted)
                    .Select(x => x.NationalID != null
                        ? "(" + x.NationalID + ")-" + x.PersonName + "-" + x.PersonPersianName
                        : x.PersonName)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
            }

            var vm = new InvoiceEditVm
            {
                Id = invoice.Id,
                CompanyId = invoice.CompanyId,
                BranchId = invoice.BranchId,
                FinanceDirectionId = invoice.FinanceDirectionId,
                InvoiceTypeId = invoice.InvoiceTypeId,
                InvoiceStatusId = invoice.InvoiceStatusId,
                InvoiceNo = invoice.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceSubjectId = invoice.InvoiceSubjectId,
                DueDate = invoice.DueDate,
                JobId = invoice.JobId,
                BLId = invoice.BLId,
                ParentInvoiceId = invoice.ParentInvoiceId,
                DetentionId = invoice.DetentionId,
                PartyId = invoice.PartyId,
                PartyName = partyName,
                CurrencyId = invoice.CurrencyId,
                ExchangeRate = invoice.ExchangeRate,
                Remarks = invoice.Remarks,
                Notes = invoice.Notes,
                FirstTotal = invoice.FirstTotal,
                Discount = invoice.Discount,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                FNFId = invoice.InvoiceFNFId,
                DefaultTaxPercent = invoice.DefaultTaxPercent,
                InvoiceModelId = invoice.InvoiceModelId,
                InvoiceSubjectTitle = invoice.InvoiceSubjectTitle
            };
            await FillHeaderLookupsAsync(vm, cancellationToken);
            return vm;
        }

        public async Task<Guid> SaveHeaderAsync(InvoiceEditVm vm, CancellationToken cancellationToken = default)
        {
            if (vm.Id.HasValue)
            {
                var invoice = await _db.Invoices
                    //.FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted && x.CompanyId == _requestContext.CompanyId, cancellationToken);
                    .FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted, cancellationToken);

                if (invoice == null)
                    throw new InvalidOperationException("Invoice not found.");

                invoice.FinanceDirectionId = vm.FinanceDirectionId;
                invoice.InvoiceTypeId = vm.FinanceDirectionId;
                invoice.InvoiceNo = vm.InvoiceNo.Trim();
                invoice.InvoiceModelId = vm.InvoiceModelId;
                invoice.InvoiceDate = vm.InvoiceDate;
                invoice.DueDate = vm.DueDate;
                invoice.PartyId = vm.PartyId;
                invoice.CurrencyId = vm.CurrencyId;
                invoice.ExchangeRate = vm.ExchangeRate;
                invoice.Remarks = vm.Remarks.Trim();
                invoice.Notes = vm.Notes?.Trim() ?? string.Empty;
                invoice.InvoiceFNFId = vm.FNFId;
                invoice.CompanyId = vm.CompanyId.Value;
                invoice.BranchId = vm.BranchId.Value;
                invoice.UpdatedAt = DateTime.Now;
                invoice.EditUserId = _requestContext.UserId;
                invoice.DefaultTaxPercent = vm.DefaultTaxPercent;
                invoice.InvoiceModelId = vm.InvoiceModelId;
                invoice.InvoiceSubjectId = vm.InvoiceSubjectId;
                invoice.InvoiceSubjectTitle = vm.InvoiceSubjectTitle;
                invoice.BLId = vm.BLId;
                invoice.BookingId = vm.BookingId;
                await RecalculateInvoiceTotalsInternalAsync(invoice.Id, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                await RecalculateInvoiceTotalsInternalAsync(invoice.Id, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                return invoice.Id;
            }
            else
            {

                await SetInvoiceSerialAsync(vm);
                var invoice = new Invoice
                {
                    Id = Guid.CreateVersion7(),
                    BranchId = vm.BranchId,
                    InvoiceFNFId = vm.FNFId,
                    CompanyId = vm.CompanyId.Value,
                    FinanceDirectionId = vm.FinanceDirectionId,
                    InvoiceTypeId = vm.FinanceDirectionId,
                    InvoiceNo = vm.InvoiceNo.Trim(),
                    InvoiceDate = vm.InvoiceDate,
                    DueDate = vm.DueDate,
                    JobId = vm.JobId,
                    BLId = vm.BLId,
                    BookingId = vm.BookingId,
                    ParentInvoiceId = vm.ParentInvoiceId,
                    DetentionId = vm.DetentionId,
                    PartyId = vm.PartyId,
                    CurrencyId = vm.CurrencyId,
                    InvoiceModelId = vm.InvoiceModelId,
                    ExchangeRate = vm.ExchangeRate,
                    FirstTotal = 0,
                    Discount = 0,
                    SubTotal = 0,
                    TaxAmount = 0,
                    TotalAmount = 0,
                    Remarks = vm.Remarks?.Trim() ?? string.Empty,
                    Notes = vm.Notes?.Trim() ?? string.Empty,
                    InvoiceStatusId = -1,
                    ApprovedById = null,
                    ApprovedDate = null,
                    IsPostedToAcc = false,
                    PostedToAcc = DateTime.Now,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    CreateUserId = _requestContext.UserId,
                    EditUserId = null,
                    InvoiceSubjectId = vm.InvoiceSubjectId,
                    InvoiceSubjectTitle = vm.InvoiceSubjectTitle,
                    DefaultTaxPercent = vm.DefaultTaxPercent,
                    Serial = vm.Serial,
                    SerialPrefix = vm.SerialPrefix
                };
                _db.Invoices.Add(invoice);
                await _db.SaveChangesAsync(cancellationToken);
                return invoice.Id;
            }
        }

        public async Task<int> SaveInvoiceDOAsync(InvoiceEditVm vm, CancellationToken cancellationToken = default)
        {
            if (vm.Id is null || vm.Id == Guid.Empty)
            {
                vm.Id = Guid.CreateVersion7();
                vm.Id2 = Guid.CreateVersion7();
            }
            var parameters = new[]
                {
                    new SqlParameter("@BLId", SqlDbType.UniqueIdentifier) { Value = vm.BLId },
                    new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) { Value = vm.Id },
                    new SqlParameter("@InvoiceId2", SqlDbType.UniqueIdentifier) { Value = vm.Id2 },
                    new SqlParameter("@CompanyId", SqlDbType.Int) { Value = vm.CompanyId },
                    new SqlParameter("@JobId", SqlDbType.UniqueIdentifier) { Value = vm.JobId },
                    new SqlParameter("@PartyId", SqlDbType.UniqueIdentifier) { Value = vm.PartyId },
                    new SqlParameter("@CurrencyId", SqlDbType.Int) { Value = 2 },  //Rials
                    new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = _currentUserService.UserId },
                    new SqlParameter("@DefaultTaxPercent", SqlDbType.Int) { Value = vm.DefaultTaxPercent },
                    new SqlParameter("@BranchId", SqlDbType.UniqueIdentifier) { Value = vm.BranchId },
                    new SqlParameter("@ClearanceTypeId", SqlDbType.Int) { Value = vm.ClearanceTypeId },
                    new SqlParameter("@Remarks", SqlDbType.NVarChar) { Value = vm.Remarks},
                };
            var rowsAffected = await _db.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[sp_IssueDoInvocie] @BLId, @InvoiceId, @CompanyId, @JobId, @PartyId, @CurrencyId, @UserId, @DefaultTaxPercent, @BranchId, @ClearanceTypeId,@Remarks,@InvoiceId2",
                parameters);
            return rowsAffected;
        }

        public async Task<(bool canBeDeleted, string message)> InvoiceCanBeDeleted(Guid id, CancellationToken cancellationToken = default)
        {
            var items = await _db.InvoiceItems.Where(x => x.InvoiceId == id && !x.IsDeleted).ToListAsync();
            if (items.Any())
            {
                return (false, "Invoice has some items and can not be deleted");
            }
            var settleitems = await _db.SettlementItems.Where(x => x.InvoiceId == id && !x.IsDeleted).ToListAsync();
            if (settleitems.Any())
            {
                return (false, "Invoice has some settlements and can not be deleted");
            }
            return (true, string.Empty);
        }


        public async Task<bool> SoftDeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invoice = await _db.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                return false;
            var allowStatus = await InvoiceCanBeDeleted(invoice.Id, cancellationToken);
            if (!allowStatus.canBeDeleted) return false;

            invoice.IsDeleted = true;
            //invoice.IsActive = false;
            invoice.DeletedAt = DateTime.Now;
            invoice.DeleteUserId = _requestContext.UserId;

            var items = await _db.InvoiceItems
                .Where(x => x.InvoiceId == id && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                item.IsDeleted = true;
                //item.IsActive = false;
                invoice.DeletedAt = DateTime.Now;
                invoice.DeleteUserId = _requestContext.UserId;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<InvoiceItemRowVm>> GetItemsAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var items =
                from ii in _db.InvoiceItems.AsNoTracking()
                where ii.InvoiceId == invoiceId && !ii.IsDeleted
                join ct in _db.ChargeTypes.AsNoTracking().Where(x => !x.IsDeleted) on ii.ChargeTypeId equals ct.Id
                join c in _db.Currencies.AsNoTracking().Where(x => !x.IsDeleted) on ii.CurrencyId equals c.Id
                join ch in _db.Charges.AsNoTracking().Where(x => !x.IsDeleted) on ii.ChargeId equals ch.Id into chargeJoin
                from ch in chargeJoin.DefaultIfEmpty()
                join u in _db.Users.AsNoTracking().Where(x => !x.IsDeleted) on ii.CreateUserId equals u.Id
                join uu in _db.Users.AsNoTracking().Where(x => !x.IsDeleted) on ii.EditUserId equals uu.Id into UpdateUserJoin
                from uu in UpdateUserJoin.DefaultIfEmpty()
                orderby ii.SequenceNumber

                select new InvoiceItemRowVm
                {
                    Id = ii.Id,
                    InvoiceId = ii.InvoiceId,
                    SequenceNumber = ii.SequenceNumber,
                    ChargeTypeName = ct.ChargeTypeName,
                    ChargeName = ch != null ? ch.ChargeDescription : "",
                    Description = ii.Description,
                    QTY = ii.QTY,
                    UnitPrice = ii.UnitPrice,
                    Discount = ii.Discount,
                    TaxPercent = ii.TaxPercent,
                    CurrencyCode = c.CurrencyCode,
                    AmountBeforeDiscount = ii.AmountBeforeDiscount,
                    AmountAfterDiscount = ii.AmountAfterDiscount,
                    TaxAmount = ii.TaxAmount,
                    TotalAmount = ii.TotalAmount,
                    CreateDateTime = ii.CreatedAt,
                    CreateUserName = u.UserName,
                    UpdateDateTime = ii.UpdatedAt,
                    UpdateUserName = uu.UserName
                };

            return await items.ToListAsync(cancellationToken);
        }

        public async Task<InvoiceItemEditVm> GetNewItemVmAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var invoice = await _db.Invoices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");

            var nextSequence = await _db.InvoiceItems.AsNoTracking()
                .Where(x => x.InvoiceId == invoiceId && !x.IsDeleted)
                .Select(x => (int?)x.SequenceNumber)
                .MaxAsync(cancellationToken) ?? 0;

            var vm = new InvoiceItemEditVm
            {
                InvoiceId = invoiceId,
                SequenceNumber = nextSequence + 1,
                CurrencyId = invoice.CurrencyId,
                QTY = 1,
                UnitPrice = 0,
                Discount = 0,
                TaxPercent = invoice.DefaultTaxPercent ?? 0,
                Description = "-",
                ChargeTypeSupplementRemarks = string.Empty
            };
            CalculateItem(vm);
            await FillItemLookupsAsync(vm, invoice.FinanceDirectionId, cancellationToken);
            return vm;
        }

        public async Task<InvoiceItemEditVm?> GetEditItemVmAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.InvoiceItems.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (item == null)
                return null;

            var invoice = await _db.Invoices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == item.InvoiceId && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                return null;

            var vm = new InvoiceItemEditVm
            {
                Id = item.Id,
                InvoiceId = item.InvoiceId,
                SequenceNumber = item.SequenceNumber,
                ChargeTypeId = item.ChargeTypeId,
                ChargeId = item.ChargeId,
                Description = item.Description,
                QTY = item.QTY,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                TaxPercent = item.TaxPercent,
                CurrencyId = item.CurrencyId,
                ChargeTypeSupplementRemarks = item.ChargeTypeSupplementRemarks,
                AmountBeforeDiscount = item.AmountBeforeDiscount,
                AmountAfterDiscount = item.AmountAfterDiscount,
                TaxAmount = item.TaxAmount,
                TotalAmount = item.TotalAmount
            };

            await FillItemLookupsAsync(vm, invoice.FinanceDirectionId, cancellationToken);
            return vm;
        }

        public async Task<Guid> SaveItemAsync(InvoiceItemEditVm vm, CancellationToken cancellationToken = default)
        {
            var invoice = await _db.Invoices
                .FirstOrDefaultAsync(x => x.Id == vm.InvoiceId && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");

            CalculateItem(vm);

            if (vm.Id.HasValue)
            {
                var item = await _db.InvoiceItems
                    .FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted, cancellationToken);

                if (item == null)
                    throw new InvalidOperationException("Invoice item not found.");

                item.ChargeTypeId = vm.ChargeTypeId;
                item.ChargeId = vm.ChargeId;
                item.Description = vm.Description.Trim();
                item.QTY = vm.QTY;
                item.UnitPrice = vm.UnitPrice;
                item.AmountBeforeDiscount = vm.AmountBeforeDiscount;
                item.Discount = vm.Discount;
                item.AmountAfterDiscount = vm.AmountAfterDiscount;
                item.TaxPercent = vm.TaxPercent;
                item.TaxAmount = vm.TaxAmount;
                item.TotalAmount = vm.TotalAmount;
                item.CurrencyId = invoice.CurrencyId;
                item.ChargeTypeSupplementRemarks = vm.ChargeTypeSupplementRemarks.Trim();
                item.UpdatedAt = DateTime.Now;
                item.EditUserId = _requestContext.UserId;
            }
            else
            {
                var item = new InvoiceItem
                {
                    Id = Guid.CreateVersion7(),
                    InvoiceId = vm.InvoiceId,
                    SequenceNumber = vm.SequenceNumber > 0 ? vm.SequenceNumber : await GetNextSequenceNumberAsync(vm.InvoiceId, cancellationToken),
                    ChargeTypeId = vm.ChargeTypeId,
                    ChargeId = vm.ChargeId,
                    Description = vm.Description.Trim(),
                    QTY = vm.QTY,
                    UnitPrice = vm.UnitPrice,
                    AmountBeforeDiscount = vm.AmountBeforeDiscount,
                    Discount = vm.Discount,
                    AmountAfterDiscount = vm.AmountAfterDiscount,
                    TaxPercent = vm.TaxPercent,
                    TaxAmount = vm.TaxAmount,
                    TotalAmount = vm.TotalAmount,
                    CurrencyId = invoice.CurrencyId,
                    ChargeTypeSupplementRemarks = vm.ChargeTypeSupplementRemarks.Trim(),
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    CreateUserId = _requestContext.UserId,
                    EditUserId = null

                };

                _db.InvoiceItems.Add(item);
                vm.Id = item.Id;
            }

            await RecalculateInvoiceTotalsInternalAsync(vm.InvoiceId, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await RecalculateInvoiceTotalsInternalAsync(vm.InvoiceId, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return vm.Id!.Value;
        }

        public async Task SoftDeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.InvoiceItems
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (item == null)
                return;

            var invoice = await _db.Invoices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == item.InvoiceId && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                return;

            item.IsDeleted = true;
            //item.IsActive = false;
            item.UpdatedAt = DateTime.Now;
            item.EditUserId = _requestContext.UserId;

            await RecalculateInvoiceTotalsInternalAsync(item.InvoiceId, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await RecalculateInvoiceTotalsInternalAsync(item.InvoiceId, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<InvoiceTotalsVm> GetTotalsAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var invoice = await _db.Invoices.AsNoTracking()
                //.FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted && x.CompanyId == _requestContext.CompanyId, cancellationToken);
                .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, cancellationToken);


            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");

            return new InvoiceTotalsVm
            {
                InvoiceId = invoice.Id,
                FirstTotal = invoice.FirstTotal,
                Discount = invoice.Discount,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount
            };
        }

        public async Task<PagedResultVm<PartyLookupRowVm>> GetPartyLookupAsync(PartyLookupFilterVm filter, Guid? jobId, CancellationToken cancellationToken = default)
        {
            filter.PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;





            var query = _db.Persons.AsNoTracking()
                .Where(x => !x.IsDeleted);


            // فیلتر بر اساس JobId در صورت وجود مقدار
            if (jobId.HasValue && jobId.Value != Guid.Empty)
            {
                query = query.Where(p => _db.JobParties.Any(jp =>
                    !jp.IsDeleted &&
                    jp.JobId == jobId.Value &&
                    jp.PartyId == p.Id));
            }
            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(x => x.PersonName.Contains(filter.Search));

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.PersonName)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new PartyLookupRowVm
                {
                    Id = x.Id,
                    PersonName = x.NationalID != null ? "(" + x.NationalID + ")-" + x.PersonName + "-" + x.PersonPersianName : x.PersonName
                })
                .ToListAsync(cancellationToken);

            return new PagedResultVm<PartyLookupRowVm>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<List<LookupItemVm>> GetChargesForItemAsync(int? currencyId, int? chargeTypeId, int? financeDirectionId, CancellationToken cancellationToken = default)
        {
            var query = _db.Charges.AsNoTracking()
                //.Where(x => !x.IsDeleted && x.CompanyId == _requestContext.CompanyId);
                .Where(x => !x.IsDeleted);


            if (currencyId.HasValue)
                query = query.Where(x => x.CurrencyId == currencyId.Value);

            if (chargeTypeId.HasValue)
                query = query.Where(x => x.ChargeTypeId == chargeTypeId.Value);

            if (financeDirectionId.HasValue)
                query = query.Where(x => x.DirectionId == financeDirectionId.Value);

            return await query
                .OrderBy(x => x.ChargeDescription)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeDescription
                })
                .ToListAsync(cancellationToken);
        }
        private async Task FillHeaderLookupsAsync(InvoiceEditVm vm, CancellationToken cancellationToken)
        {
            vm.InvoiceTypes = await GetInvoiceTypesAsync(cancellationToken);
            vm.InvoiceStatuses = await GetInvoiceStatusesAsync(cancellationToken);
            vm.FinanceDirections = await GetFinanceDirectionsAsync(cancellationToken);
            vm.Currencies = await GetCurrenciesAsync(cancellationToken);
            vm.FNFS = await GetFNFSAsync(cancellationToken);
            vm.Companies = await GetCompaniesAsync(cancellationToken);
            vm.InvoiceSubjects = await GetInvoiceSubjectsAsync(cancellationToken);
            vm.InvoiceModels = await GetInvoiceModelsAsync(cancellationToken);
            vm.Branches = await GetBranchesAsync(cancellationToken);
            vm.ClearanceTypes = await GetClearanceTypesAsync(cancellationToken);
            if (vm.JobId != null && vm.JobId.HasValue && vm.JobId != Guid.Empty)
            {
                vm.BLs = await GetBLsAsync(vm.JobId.Value, cancellationToken);
                vm.Bookings = await GetBookingsAsync(vm.JobId.Value, cancellationToken);

                //if (vm.BLId != null && vm.BLId.HasValue && vm.BLId != Guid.Empty)
                //{
                //    vm.BLId=
                //}
            }
        }
        private async Task FillItemLookupsAsync(InvoiceItemEditVm vm, int financeDirectionId, CancellationToken cancellationToken)
        {
            vm.ChargeTypes = await _db.ChargeTypes.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync(cancellationToken);

            vm.Currencies = await GetCurrenciesAsync(cancellationToken);

            vm.Charges = await GetChargesForItemAsync(
                vm.CurrencyId > 0 ? vm.CurrencyId : null,
                vm.ChargeTypeId > 0 ? vm.ChargeTypeId : null,
                financeDirectionId,
                cancellationToken);
        }

        private async Task<List<LookupItemVm>> GetInvoiceTypesAsync(CancellationToken cancellationToken)
        {
            return await _db.InvoiceTypes.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.InvoiceTypeName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<LookupItemVm>> GetInvoiceStatusesAsync(CancellationToken cancellationToken)
        {
            return await _db.InvoiceStatuses.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.InvoiceStatusName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<LookupItemVm>> GetFinanceDirectionsAsync(CancellationToken cancellationToken)
        {
            return await _db.FinanceDirections.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.Id != 3)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.InvoiceContextName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetCurrenciesAsync(CancellationToken cancellationToken)
        {
            return await _db.Currencies.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .ThenBy(x => x.CurrencyCode)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode + " - " + x.CurrencyName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetCompaniesAsync(CancellationToken cancellationToken)
        {
            return await _db.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetInvoiceSubjectsAsync(CancellationToken cancellationToken)
        {
            return await _db.InvoiceSubjects
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.InvoiceSubjectName + " - " + x.InvoiceSubjectLocalName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetInvoiceModelsAsync(CancellationToken cancellationToken)
        {
            return await _db.InvoiceModels
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.InvoiceModelName + " - " + x.InvoiceModelLocalName
                })
                .ToListAsync(cancellationToken);
        }


        private async Task<List<LookupItemVm>> GetClearanceTypesAsync(CancellationToken cancellationToken)
        {
            return await _db.ClearanceTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.ClearanceTypeName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetBranchesAsync(CancellationToken cancellationToken)
        {
            return await _db.Branches
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.BranchName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetBLsAsync(Guid jobId, CancellationToken cancellationToken)
        {
            return await _db.BLs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.JobId == jobId)
                .OrderBy(x => x.BLNo)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.BLNo
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<LookupItemVm>> GetBookingsAsync(Guid jobId, CancellationToken cancellationToken)
        {
            return await _db.Bookings
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.JobId == jobId)
                .OrderBy(x => x.BookingNo)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.BookingNo
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<LookupItemVm>> GetFNFSAsync(CancellationToken cancellationToken)
        {
            return await _db.InvoiceFNFs.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new LookupItemVm
                {
                    Value = x.Id.ToString(),
                    Text = x.FNFName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<int> GetNextSequenceNumberAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            var max = await _db.InvoiceItems
                .Where(x => x.InvoiceId == invoiceId && !x.IsDeleted)
                .Select(x => (int?)x.SequenceNumber)
                .MaxAsync(cancellationToken);

            return (max ?? 0) + 1;
        }

        private static void CalculateItem(InvoiceItemEditVm vm)
        {
            vm.AmountBeforeDiscount = Math.Round(vm.QTY * vm.UnitPrice, 2, MidpointRounding.AwayFromZero);
            vm.AmountAfterDiscount = Math.Round(vm.AmountBeforeDiscount - vm.Discount, 2, MidpointRounding.AwayFromZero);

            if (vm.AmountAfterDiscount < 0)
                vm.AmountAfterDiscount = 0;

            vm.TaxAmount = Math.Round(vm.AmountAfterDiscount * vm.TaxPercent / 100m, 2, MidpointRounding.AwayFromZero);
            vm.TotalAmount = Math.Round(vm.AmountAfterDiscount + vm.TaxAmount, 2, MidpointRounding.AwayFromZero);
        }

        private async Task RecalculateInvoiceTotalsInternalAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            var invoice = await _db.Invoices
                .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, cancellationToken);

            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");

            var items = await _db.InvoiceItems
                .Where(x => x.InvoiceId == invoiceId && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            invoice.FirstTotal = items.Sum(x => x.AmountBeforeDiscount);
            invoice.Discount = items.Sum(x => x.Discount);
            invoice.SubTotal = items.Sum(x => x.AmountAfterDiscount);
            invoice.TaxAmount = items.Sum(x => x.TaxAmount);
            invoice.TotalAmount = items.Sum(x => x.TotalAmount);
            invoice.UpdatedAt = DateTime.Now;
            invoice.EditUserId = _requestContext.UserId;
        }
        public async Task<string> GetInvoiceNoAsync(Guid invoiceId)
        {
            var invoice = await _db.Invoices.FindAsync(invoiceId);
            if (invoice == null) return "";
            return invoice.InvoiceNo;
        }
        public async Task<int> GetInvoiceCurrencyIdAsync(Guid invoiceId)
        {
            var inv = await _db.Invoices.FindAsync(invoiceId);
            return inv.CurrencyId;
        }


        public async Task<ServiceResult<CreateOnlinePaymentVm>> GetNewOnlinePayment(List<Guid> invoiceIds)
        {
            var currentPerson = await _currentUserService.GetPersonIdAsync();
            var invoices = await _db.Invoices.Where(x => invoiceIds.Contains(x.Id) && x.PartyId == currentPerson).ToListAsync();
            if (invoiceIds.Count != invoices.Count)
            {
                return ServiceResult<CreateOnlinePaymentVm>.Failed("invoice Ids are invalid");
            }

            // check remain payment for each invoice:

            var remainPaymentInvoices = await financeQueries.GetRemainInvoicePayment()
                .Where(x => invoiceIds.Contains(x.InvoiceId))
                .ToListAsync();

            if (remainPaymentInvoices.Any(x => x.RemainPayment <= 0))
            {
                return ServiceResult<CreateOnlinePaymentVm>.Failed("Some Invoices paid fully before !!");
            }

            var items = from i in invoices
                        join remainPayment in remainPaymentInvoices
                 on i.Id equals remainPayment.InvoiceId
                        select new CreateOnlinePaymentItemVm
                        {
                            InvoiceId = i.Id,
                            InvoiceNo = i.InvoiceNo,
                            RemainAmount = (long?)remainPayment.RemainPayment ?? 0,
                            PaymentAmount = (long?)remainPayment.RemainPayment ?? 0,
                            Total = (long)i.TotalAmount
                        };

            var createvm = new CreateOnlinePaymentVm(null, currentPerson.Value, items.ToList());
            return ServiceResult<CreateOnlinePaymentVm>.Success(createvm);
        }

        public async Task<bool> ExistsSaleDoInvoiceForBLAsync(Guid? bLId)
        {
            return await _db.Invoices.Where(x => x.BLId == bLId && x.FinanceDirectionId == 1 && x.InvoiceStatusId != 8 && x.IsDeleted == false && x.InvoiceSubjectId == 4).AnyAsync();
        }




        public async Task<byte[]> GenerateOfficialTaxInvoicePdfAsync(InvoicePrintDto invoiceDto, CancellationToken cancellationToken = default)
        {

            var logoPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "logos",
                        "logo-1.png");

            byte[] logoBytes = Array.Empty<byte>();

            if (File.Exists(logoPath))
            {
                logoBytes = await File.ReadAllBytesAsync(
                    logoPath,
                    cancellationToken);
            }



            using var report = new Report();

            // 1. ساخت DataTable برای هدر فاکتور
            var headerTable = new DataTable("InvoiceHeader");
            headerTable.Columns.Add("InvoiceNo", typeof(string));
            headerTable.Columns.Add("InvoiceDatePersian", typeof(string));
            headerTable.Columns.Add("InvoiceModelName", typeof(string));
            headerTable.Columns.Add("CurrencyName", typeof(string));
            headerTable.Columns.Add("SellerName", typeof(string));
            headerTable.Columns.Add("SellerEconomicCode", typeof(string));
            headerTable.Columns.Add("SellerNationalId", typeof(string));
            headerTable.Columns.Add("SellerAddress", typeof(string));
            headerTable.Columns.Add("SellerPhone", typeof(string));
            headerTable.Columns.Add("BuyerName", typeof(string));
            headerTable.Columns.Add("BuyerEconomicCode", typeof(string));
            headerTable.Columns.Add("BuyerNationalId", typeof(string));
            headerTable.Columns.Add("BuyerAddress", typeof(string));
            headerTable.Columns.Add("BuyerPhone", typeof(string));
            headerTable.Columns.Add("TotalFirst", typeof(decimal));
            headerTable.Columns.Add("TotalDiscount", typeof(decimal));
            headerTable.Columns.Add("TotalTax", typeof(decimal));
            headerTable.Columns.Add("FinalAmount", typeof(decimal));
            headerTable.Columns.Add("CompanyLogo", typeof(byte[]));

            var headerRow = headerTable.NewRow();
            headerRow["InvoiceNo"] = invoiceDto.InvoiceNo ?? "";
            headerRow["InvoiceDatePersian"] = invoiceDto.InvoiceDatePersian ?? "";
            headerRow["InvoiceModelName"] = invoiceDto.InvoiceModelName ?? "";
            headerRow["CurrencyName"] = invoiceDto.CurrencyName;
            headerRow["SellerName"] = invoiceDto.SellerName ?? "";
            headerRow["SellerEconomicCode"] = invoiceDto.SellerEconomicCode ?? "";
            headerRow["SellerNationalId"] = invoiceDto.SellerNationalId ?? "";
            headerRow["SellerAddress"] = invoiceDto.SellerAddress ?? "";
            headerRow["SellerPhone"] = invoiceDto.SellerPhone ?? "";
            headerRow["BuyerName"] = invoiceDto.BuyerName ?? "";
            headerRow["BuyerEconomicCode"] = invoiceDto.BuyerEconomicCode ?? "";
            headerRow["BuyerNationalId"] = invoiceDto.BuyerNationalId ?? "";
            headerRow["BuyerAddress"] = invoiceDto.BuyerAddress ?? "";
            headerRow["BuyerPhone"] = invoiceDto.BuyerPhone ?? "";
            headerRow["TotalFirst"] = invoiceDto.TotalFirst;
            headerRow["TotalDiscount"] = invoiceDto.TotalDiscount;
            headerRow["TotalTax"] = invoiceDto.TotalTax;
            headerRow["FinalAmount"] = invoiceDto.FinalAmount;
            headerRow["CompanyLogo"] = logoBytes;
            headerTable.Rows.Add(headerRow);

            // 2. ساخت DataTable برای اقلام فاکتور (Items)
            var itemsTable = new DataTable("InvoiceItems");
            itemsTable.Columns.Add("Row", typeof(int));
            itemsTable.Columns.Add("ChargeTitle", typeof(string));
            itemsTable.Columns.Add("Qty", typeof(decimal));
            itemsTable.Columns.Add("UnitPrice", typeof(decimal));
            itemsTable.Columns.Add("AmountBeforeDiscount", typeof(decimal));
            itemsTable.Columns.Add("Discount", typeof(decimal));
            itemsTable.Columns.Add("TaxAmount", typeof(decimal));
            itemsTable.Columns.Add("Total", typeof(decimal));

            int index = 1;
            foreach (var item in invoiceDto.Items)
            {
                var row = itemsTable.NewRow();
                row["Row"] = item.Row > 0 ? item.Row : index++;
                row["ChargeTitle"] = item.ChargeTitle ?? "";
                row["Qty"] = item.Qty;
                row["UnitPrice"] = item.UnitPrice;
                row["AmountBeforeDiscount"] = item.AmountBeforeDiscount;
                row["Discount"] = item.Discount;
                row["TaxAmount"] = item.TaxAmount;
                row["Total"] = item.Total;
                itemsTable.Rows.Add(row);
            }

            var dataSet = new DataSet("TaxInvoiceData");
            dataSet.Tables.Add(headerTable);
            dataSet.Tables.Add(itemsTable);

            // 3. ثبت دیتاست در FastReport و فعال‌سازی TableDataSource
            report.RegisterData(dataSet, "TaxInvoiceData");
            report.GetDataSource("InvoiceHeader").Enabled = true;
            report.GetDataSource("InvoiceItems").Enabled = true;

            // 4. بارگذاری تمپلیت فایل FastReport (.frx) یا طراحی آن
            string reportPath = "";
            switch (invoiceDto.InvoiceModelId)
            {
                case 1:
                    switch (invoiceDto.FNFId)
                    {
                        case 1:
                            reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Finance", "OfficialTaxInvoice.frx");
                            break;
                        case 2:
                            reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Finance", "NFInvoice.frx");
                            break;
                    }
                    break;
                case 2:
                    reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Finance", "PreInvoice.frx");
                    break;
            }
            if (File.Exists(reportPath))
            {
                report.Load(reportPath);
            }
            else
            {
                // ساخت خودکار تمپلیت در حافظه در صورتی که فایل frx هنوز تعریف نشده باشد
                BuildDynamicReportTemplate(report);
            }
            report.Prepare();
            using var pdfExport = new PDFSimpleExport();
            using var stream = new MemoryStream();
            pdfExport.Export(report, stream);

            return stream.ToArray();
        }

        private void BuildDynamicReportTemplate(Report report)
        {
            var page = new ReportPage();
            page.Name = "Page1";
            page.Landscape = false;
            page.PaperWidth = 210; // A4
            page.PaperHeight = 297;
            page.LeftMargin = 8;
            page.RightMargin = 8;
            page.TopMargin = 8;
            page.BottomMargin = 8;
            report.Pages.Add(page);
            // تمپلیت در لایه frx اختصاصی لود می‌شود
        }







        public async Task<InvoicePrintDto> GetInvoicePrintDtoAsync(Guid invoiceId)
        {
            var invoice = await _db.Invoices
                .Include(x => x.Company)
                    .ThenInclude(x => x.Person)
                .Include(x => x.Party)
                .Include(x => x.InvoiceModel)
                .Include(x => x.Currency)
                .Include(x => x.InvoiceItems).ThenInclude(i => i.ChargeType)
                .FirstOrDefaultAsync(x => x.Id == invoiceId);

            if (invoice == null) return null;

            var dto = new InvoicePrintDto
            {
                FinanceDirectionId = invoice.FinanceDirectionId,
                InvoiceModelId = invoice.InvoiceModelId,
                InvoiceNo = invoice.InvoiceNo ?? "-",
                InvoiceDatePersian = DateHelper.getSolarDate10(invoice.InvoiceDate.Value),
                InvoiceModelName = invoice.InvoiceModel?.InvoiceModelName ?? "",
                CurrencyName = invoice.Currency?.LocalName ?? "",
                TotalFirst = invoice.FirstTotal,
                TotalDiscount = invoice.Discount,
                TotalTax = invoice.TaxAmount,
                FinalAmount = invoice.TotalAmount,
                FNFId = invoice.InvoiceFNFId,
                Items = invoice.InvoiceItems.OrderBy(x => x.SequenceNumber).Select(x => new InvoiceItemPrintDto
                {
                    Row = x.SequenceNumber,
                    ChargeTitle = x.ChargeType?.ChargeTypeLocalName + " " + x.ChargeTypeSupplementRemarks,
                    Qty = x.QTY,
                    UnitPrice = x.UnitPrice,
                    AmountBeforeDiscount = x.AmountBeforeDiscount,
                    Discount = x.Discount,
                    TaxAmount = x.TaxAmount,
                    Total = x.TotalAmount
                }).ToList()
            };

            // منطق Direction: 1 = فروش (Company فروشنده)، 2 = خرید (Company خریدار)
            if (invoice.FinanceDirectionId == 1)
            {
                FillSellerFromCompany(dto, invoice.Company);
                FillBuyerFromParty(dto, invoice.Party);
            }
            else
            {
                FillSellerFromParty(dto, invoice.Party);
                FillBuyerFromCompany(dto, invoice.Company);
            }
            return dto;
        }


        private void FillSellerFromCompany(InvoicePrintDto dto, Company company)
        {
            if (company == null) return;

            dto.SellerName = company.Person.PersonPersianName;
            dto.SellerEconomicCode = company.Person.EcoCode ?? "-";
            dto.SellerNationalId = company.Person.NationalID ?? "-"; // یا هر فیلدی که شناسه ملی است
            dto.SellerAddress = company.Person.PersianAddress ?? "-";
            dto.SellerPhone = company.Person.Phone ?? "-";
        }

        private void FillBuyerFromParty(InvoicePrintDto dto, Person party)
        {
            if (party == null) return;

            dto.BuyerName = party.PersonPersianName;
            dto.BuyerEconomicCode = party.EcoCode ?? "-";
            dto.BuyerNationalId = party.NationalID ?? "-";
            dto.BuyerAddress = party.PersianAddress ?? "-";
            dto.BuyerPhone = party.Phone ?? "-";
        }
        private void FillSellerFromParty(InvoicePrintDto dto, Person party)
        {
            if (party == null) return;

            dto.SellerName = party.PersonPersianName;
            dto.SellerEconomicCode = party.EcoCode ?? "-";
            dto.SellerNationalId = party.NationalID ?? "-";
            dto.SellerAddress = party.Address ?? "-";
            dto.SellerPhone = party.Phone ?? "-";
        }
        private void FillBuyerFromCompany(InvoicePrintDto dto, Company company)
        {
            if (company == null) return;

            dto.BuyerName = company.Person.PersonPersianName;
            dto.BuyerEconomicCode = company.Person.EcoCode ?? "-";
            dto.BuyerNationalId = company.Person.NationalID ?? "-";
            dto.BuyerAddress = company.Person.PersianAddress ?? "-";
            dto.BuyerPhone = company.Person.Phone ?? "-";
        }
        public async Task<int> GetNextSerialAsync(int companyId, string serialPrefix, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(serialPrefix))
                throw new ArgumentException("Serial prefix cannot be empty.", nameof(serialPrefix));

            serialPrefix = serialPrefix.Trim().ToUpper();

            // گرفتن ماکسیمم شماره سریال برای پیشوند مشخص شده
            var maxSerial = await _db.Invoices
                .Where(x => x.SerialPrefix == serialPrefix && !x.IsDeleted && x.CompanyId == companyId)
                .MaxAsync(x => (int?)x.Serial, ct) ?? 0;
            return maxSerial + 1;
        }

        public async Task SetInvoiceSerialAsync(InvoiceEditVm vm)
        {
            if (vm.InvoiceModelId == 2)
            {
                vm.SerialPrefix = "P";
                vm.Serial = await GetNextSerialAsync(vm.CompanyId.Value, vm.SerialPrefix, default);
                return;
            }
            switch (vm.FinanceDirectionId)
            {
                case 1: //Sell
                    switch (vm.FNFId)
                    {
                        case 1:
                            vm.SerialPrefix = "A";
                            vm.Serial = await GetNextSerialAsync(vm.CompanyId.Value, vm.SerialPrefix, default);
                            break;
                        case 2:
                            vm.SerialPrefix = "N";
                            vm.Serial = await GetNextSerialAsync(vm.CompanyId.Value, vm.SerialPrefix, default);
                            break;
                        default:
                            break;
                    }
                    break;
                case 2: //Buy
                    vm.SerialPrefix = "Z";
                    vm.Serial = await GetNextSerialAsync(vm.CompanyId.Value, vm.SerialPrefix, default);
                    break;
                default:
                    break;
            }
        }
    }
}
