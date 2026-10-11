using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Interfaces.BillOfLadings
{
    public interface IBLManagementService
    {
        Task<BLManagementPageViewModel> GetManagementPageDataAsync(BLManagementSearchRequest request);
        Task<bool> DeleteBLAsync(Guid blId, Guid userId);
        Task<MasterConsoleBLSummaryVm?> GetBLSummaryAsync(Guid blId, CancellationToken cancellationToken = default);
        Task<MasterConsoleVm> GetMasterConsoleAsync(Guid id);
        Task<List<InvoiceRowVm>> GetInvoicesByBlIdAsync(Guid blId, string filterType);
        Task<MasterConsoleDetentionsVm?> GetDetentionsByBlAsync(Guid blId, CancellationToken cancellationToken);
        Task<List<DoPricePreviewItemDto>> GetDoPricePreviewAsync(Guid blId, int clearanceTypeId, decimal taxRate);
    }
    public class BLManagementService : IBLManagementService
    {
        private readonly ApplicationDbContext _context;

        public BLManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BLManagementPageViewModel> GetManagementPageDataAsync(BLManagementSearchRequest request)
        {
            request ??= new BLManagementSearchRequest();
            var query = _context.BLs
                .Include(x => x.BLStatus).Include(x => x.BLType).Include(x => x.Company).Include(x => x.Job).AsNoTracking().AsQueryable();

            // فیلتر عدم نمایش موارد کاملاً حذف شده فیزیکی (در صورت وجود منطق سافت دلیت)
            query = query.Where(x => x.BLDeletedAt == null);

            // ۱. فیلتر جستجوی عمومی (Global Search)
            if (!string.IsNullOrWhiteSpace(request.GlobalSearch))
            {
                var term = request.GlobalSearch.Trim();
                query = query.Where(x =>
                    x.BLNo.Contains(term) ||
                    x.Job.JobNumber.Contains(term) ||
                    x.Job.JobFileNumber.Contains(term) ||
                    x.PORText.Contains(term) ||
                    x.POLText.Contains(term) ||
                    x.PODText.Contains(term) ||
                    x.PODelText.Contains(term) ||
                    x.BLParties.Any(p => p.PartyTextInBL.Contains(term)) ||
                    x.BLContainers.Any(c => c.ContainerNumber.Contains(term)) ||
                    x.BLFlights.Any(f => f.FlightNo != null && f.FlightNo.Contains(term)) ||
                    x.BLTrips.Any(t => t.TruckNo != null && t.TruckNo.Contains(term)) ||
                    x.BLRailJourneys.Any(r => r.TrainNo != null && r.TrainNo.Contains(term) || r.WagonNo != null && r.WagonNo.Contains(term))
                );
            }

            // ۲. اعمال فیلترهای اختصاصی
            if (request.TransportTypeId.HasValue)
                query = query.Where(x => x.TransportTypeId == request.TransportTypeId.Value);

            if (request.BLTypeId.HasValue)
                query = query.Where(x => x.BLTypeId == request.BLTypeId.Value);

            if (request.BLStatusId.HasValue)
                query = query.Where(x => x.BLStatusId == request.BLStatusId.Value);

            if (!string.IsNullOrWhiteSpace(request.BLNo))
                query = query.Where(x => x.BLNo.Contains(request.BLNo.Trim()));

            if (!string.IsNullOrWhiteSpace(request.JobNo))
                query = query.Where(x => x.Job.JobNumber.Contains(request.JobNo.Trim()));

            //if (!string.IsNullOrWhiteSpace(request.CustomerName))
            //    query = query.Where(x => x.BLParties.Any(p => p.RoleTypeId == BLPartyRole.Customer && p.PartyTextInBL.Contains(request.CustomerName.Trim())));

            if (!string.IsNullOrWhiteSpace(request.ShipperName))
                query = query.Where(x => x.BLParties.Any(p => p.RoleTypeId == (int)BLPartyRole.Shipper && p.PartyTextInBL.Contains(request.ShipperName.Trim())));

            if (!string.IsNullOrWhiteSpace(request.ConsigneeName))
                query = query.Where(x => x.BLParties.Any(p => p.RoleTypeId == (int)BLPartyRole.Consignee && p.PartyTextInBL.Contains(request.ConsigneeName.Trim())));

            if (!string.IsNullOrWhiteSpace(request.Origin))
                query = query.Where(x => x.PORText.Contains(request.Origin.Trim()) || x.POLText.Contains(request.Origin.Trim()));

            if (!string.IsNullOrWhiteSpace(request.Destination))
                query = query.Where(x => x.PODText.Contains(request.Destination.Trim()) || x.PODelText.Contains(request.Destination.Trim()));

            if (request.IssueDateFrom.HasValue)
                query = query.Where(x => x.BLIssueDate >= request.IssueDateFrom.Value);

            if (request.IssueDateTo.HasValue)
                query = query.Where(x => x.BLIssueDate <= request.IssueDateTo.Value);

            if (!string.IsNullOrWhiteSpace(request.ContainerNo))
                query = query.Where(x => x.BLContainers.Any(c => c.ContainerNumber.Contains(request.ContainerNo.Trim())));

            if (!string.IsNullOrWhiteSpace(request.VesselVoyage))
                query = query.Where(x => x.BLVoyages.Any(v => v.VesselName.Contains(request.VesselVoyage.Trim()) || v.VoyageNo.Contains(request.VesselVoyage.Trim())));

            if (!string.IsNullOrWhiteSpace(request.FlightNo))
                query = query.Where(x => x.BLFlights.Any(f => f.FlightNo != null && f.FlightNo.Contains(request.FlightNo.Trim())));

            if (!string.IsNullOrWhiteSpace(request.TruckNo))
                query = query.Where(x => x.BLTrips.Any(t => t.TruckNo != null && t.TruckNo.Contains(request.TruckNo.Trim())));

            if (!string.IsNullOrWhiteSpace(request.TrainNo))
                query = query.Where(x => x.BLRailJourneys.Any(r => r.TrainNo != null && r.TrainNo.Contains(request.TrainNo.Trim())));

            if (!string.IsNullOrWhiteSpace(request.WagonNo))
                query = query.Where(x => x.BLRailJourneys.Any(r => r.WagonNo != null && r.WagonNo.Contains(request.WagonNo.Trim())));

            // ۳. محاسبه مقادیر آماری (Summary Card) پیش از اعمال Paging
            var summary = new BLManagementSummaryDto
            {
                TotalBills = await query.CountAsync(),
                InProgressCount = await query.CountAsync(x => x.BLStatusId == 2), // فرض: ۲ وضعیت در جریان است
                IncompleteCount = await query.CountAsync(x => string.IsNullOrEmpty(x.BLNo) || x.BLIssueDate == null),
                MissingCarrierReferenceCount = await query.CountAsync(x => x.CarrierId == null)
            };

            // ۴. پیاده‌سازی Paging
            var totalCount = summary.TotalBills;
            var skip = (request.Page - 1) * request.PageSize;

            var rawItems = await query
                .OrderByDescending(x => x.CreatedAt) // مرتب‌سازی بر اساس زمان ایجاد
                .Skip(skip)
                .Take(request.PageSize)
                .Select(x => new
                {
                    x.Id,
                    x.BLNo,
                    JobNo = x.Job != null ? x.Job.JobNumber : string.Empty,
                    x.TransportTypeId,
                    BLTypeTitle = x.BLType != null ? x.BLType.BLTypeName : string.Empty,
                    DocumentStatusTitle = x.BLStatus != null ? x.BLStatus.BLStatusName : string.Empty,
                    ShipperName = x.BLParties.Where(p => p.RoleTypeId == (int)BLPartyRole.Shipper).Select(p => p.PartyTextInBL).FirstOrDefault() ?? string.Empty,
                    ConsigneeName = x.BLParties.Where(p => p.RoleTypeId == (int)BLPartyRole.Consignee).Select(p => p.PartyTextInBL).FirstOrDefault() ?? string.Empty,
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : string.Empty,
                    x.PORText,
                    x.POLText,
                    x.PODText,
                    x.PODelText,
                    x.BLIssueDate,
                    x.BLDeletedAt,
                    x.BLReleaseTypeId,
                    x.JobId,
                    x.Job.JobFileNumber,
                    x.TradeType.TradeTypeName,
                    x.TradeTypeId,
                    x.GrossWeightKg,
                    x.BLType.BLTypeCode,
                    SalesUserName = x.Job.SalesPerson.UserName,
                    // جزئیات ترانسپورت جهت فرمت‌دهی
                    FirstContainer = x.BLContainers.Select(c => c.ContainerNumber).FirstOrDefault(),
                    FirstVoyage = x.BLVoyages.Select(v => new { v.VesselName, v.VoyageNo }).FirstOrDefault(),
                    FirstFlight = x.BLFlights.Select(f => f.FlightNo).FirstOrDefault(),
                    FirstTruck = x.BLTrips.Select(t => t.TruckNo).FirstOrDefault(),
                    FirstTrain = x.BLRailJourneys.Select(r => new { r.TrainNo, r.WagonNo }).FirstOrDefault(),

                    // فلگ‌های منطقی مربوط به MultiModal
                    HasSea = x.BLVoyages.Any(),
                    HasRoad = x.BLTrips.Any(),
                    HasRail = x.BLRailJourneys.Any(),
                    HasAir = x.BLFlights.Any()
                })
                .ToListAsync();

            var rows = rawItems.Select(x => new BLManagementRowDto
            {
                BLId = x.Id,
                BLNo = x.BLNo,
                JobNo = x.JobNo,
                TransportTypeId = x.TransportTypeId,
                TransportTypeTitle = ((TransportTypeEnum)x.TransportTypeId).ToString(),
                BLTypeTitle = x.BLTypeCode,
                DocumentStatusTitle = x.DocumentStatusTitle,
                ShipperName = x.ShipperName,
                ConsigneeName = x.ConsigneeName,
                CarrierName = x.CarrierName,
                RouteText = BuildRouteText(x.PORText, x.POLText, x.PODText, x.PODelText),
                IssueDate = x.BLIssueDate,
                BLDeletedAt = x.BLDeletedAt,
                JobId = x.JobId,
                JobFileNumber = x.JobFileNumber,
                TradeTypeId = x.TradeTypeId,
                TradeTypeName = x.TradeTypeName,
                GrossWeightKg = x.GrossWeightKg != null ? x.GrossWeightKg : null,
                SalesUserName = x.SalesUserName,
                // مقداردهی هوشمند ستون‌های Mode Detail
                ModeDetailTitle = FormatModeDetailTitle(x.TransportTypeId, x.FirstContainer, x.FirstTruck, x.FirstTrain?.TrainNo, x.FirstFlight, x.HasSea, x.HasRoad, x.HasRail, x.HasAir),
                ModeDetailSubTitle = FormatModeDetailSubTitle(x.TransportTypeId, x.FirstVoyage?.VesselName, x.FirstVoyage?.VoyageNo, x.FirstTruck, x.FirstTrain?.WagonNo, x.FirstFlight, x.HasSea, x.HasRoad, x.HasRail, x.HasAir),
                // قوانین اکشن‌ها
                CanEdit = !x.BLDeletedAt.HasValue && x.BLReleaseTypeId == null,
                CanPrint = true,
                CanRelease = !x.BLDeletedAt.HasValue && x.BLReleaseTypeId == null,
                CanCancel = !x.BLDeletedAt.HasValue,
                CanDelete = !x.BLDeletedAt.HasValue
            }).ToList();

            return new BLManagementPageViewModel
            {
                Search = request,
                Summary = summary,
                PagedResult = new PagedResultDto<BLManagementRowDto>
                {
                    Items = rows,
                    TotalCount = totalCount,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                }
            };
        }

        public async Task<bool> DeleteBLAsync(Guid blId, Guid userId)
        {
            var bl = await _context.BLs.FindAsync(blId);
            if (bl == null) return false;

            bl.BLDeletedAt = DateTime.Now;
            bl.BLDeletedByUserId = userId;
            await _context.SaveChangesAsync();
            return true;
        }

        private static string BuildRouteText(string por, string pol, string pod, string podel)
        {
            var segments = new[] { por, pol, pod, podel }.Where(s => !string.IsNullOrWhiteSpace(s));
            return string.Join(" → ", segments);
        }

        private static string FormatModeDetailTitle(int typeId, string container, string truck, string train, string flight, bool hasSea, bool hasRoad, bool hasRail, bool hasAir)
        {
            return typeId switch
            {
                //1 => !string.IsNullOrEmpty(container) ? $"CNTR: {container}" : "Ocean Freight",
                1 => "Ocean Freight",
                2 => !string.IsNullOrEmpty(truck) ? $"Truck: {truck}" : "Road Transport",
                3 => !string.IsNullOrEmpty(train) ? $"Train: {train}" : "Rail Freight",
                4 => !string.IsNullOrEmpty(flight) ? $"Flight: {flight}" : "Air Freight",
                5 => string.Join(" + ", new List<string> { hasSea ? "Sea" : "", hasRoad ? "Road" : "", hasRail ? "Rail" : "", hasAir ? "Air" : "" }.Where(s => !string.IsNullOrEmpty(s))),
                _ => "Unknown"
            };
        }

        private static string FormatModeDetailSubTitle(int typeId, string vessel, string voyage, string truck, string wagon, string flight, bool hasSea, bool hasRoad, bool hasRail, bool hasAir)
        {
            return typeId switch
            {
                1 => !string.IsNullOrEmpty(vessel) ? $"{vessel} / {voyage}" : "-",
                2 => "Interstate Delivery",
                3 => !string.IsNullOrEmpty(wagon) ? $"Wagon: {wagon}" : "-",
                4 => "Direct Air Service",
                5 => $"Combined Leg Operations",
                _ => "-"
            };
        }


        public async Task<MasterConsoleBLSummaryVm?> GetBLSummaryAsync(Guid blId, CancellationToken cancellationToken = default)
        {
            var vm = await _context.BLs
                .Include(x => x.BLContainers)
                .AsNoTracking()
                .Where(x => x.Id == blId && !x.IsDeleted)
                .Select(x => new MasterConsoleBLSummaryVm
                {
                    BLId = x.Id,
                    BLNo = x.BLNo,
                    JobId = x.JobId,
                    JobFileNo = x.Job.JobFileNumber,
                    SalesUserName = x.Job.SalesPerson != null ? x.Job.SalesPerson.UserName : "",
                    //ReferenceNo = x.ReferenceNo,
                    //BookingNo = x.BookingNo,
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : string.Empty,
                    //VesselName = x.VesselName,
                    //VoyageNumber = x.VoyageNumber,
                    POL = x.POL != null ? x.POL.LocationName : string.Empty,
                    POD = x.POD != null ? x.POD.LocationName : string.Empty,
                    IssueDate = x.BLIssueDate,
                    GrossWeightKg = x.GrossWeightKg,
                    PackageCount = x.TotalPackages,
                    ContainerCount = x.BLContainers.Count(),
                    Containers = x.BLContainers
                                .OrderBy(x => x.ContainerNumber)
                                .Select(x => new MasterConsoleContainerVm
                                {
                                    ContainerNo = x.ContainerNumber,
                                    ContainerType = x.ContainerSizeType != null ? x.ContainerSizeType.ShortName : null,
                                    SealNo = x.SealNumber,
                                    GrossWeightKg = x.TotalGrossWeight,
                                    PackageCount = x.TotalPackageCount,
                                    ContainerUseId = x.ContainerUseId
                                })
                                .ToList(),
                    ConsigneeText = x.BLParties.Where(x => x.RoleTypeId == 52).Any() ? x.BLParties.Where(x => x.RoleTypeId == 52).FirstOrDefault().PartyTextInBL : "",
                    NotifyText = x.BLParties.Where(x => x.RoleTypeId == 53).Any() ? x.BLParties.Where(x => x.RoleTypeId == 53).FirstOrDefault().PartyTextInBL : "",
                    BookerClientName = x.Job.Parties.Where(x => x.RoleTypeId == 31).Any() ? x.Job.Parties.Where(x => x.RoleTypeId == 31).FirstOrDefault().Party.PersonName : "",
                    Cargos = x.CargoDetails
                            .Select(x => new MasterConsoleCargoVm
                            {
                                PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : null,
                                PackageCount = x.PackageCount,
                                GrossWeight = x.GrossWeight,
                                NetWeight = x.NetWeight,
                                Measurement = x.WeightUnit.MeasurementUnitName,
                                MarksAndNumbers = x.MarksAndNumbers,
                                DescriptionOfGoods = x.CargoDescription,
                                HSCode = x.HSCodeToSave,
                                Volume = x.Volume
                            }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (vm.Containers != null && vm.Containers.Any())
            {
                foreach (var item in vm.Containers)
                {
                    var allContainerActions = await _context.ContainerActions.Where(x => x.ContainerUseId == item.ContainerUseId).ToListAsync();
                    if (allContainerActions != null && allContainerActions.Any())
                    {

                        item.RTSDate = allContainerActions
                            .Where(a => a.CntActionTypeId == 1)
                            .OrderByDescending(a => a.ContainerActionDateTime)
                            .Select(a => (DateTime?)a.ContainerActionDateTime)
                            .FirstOrDefault();

                        item.RBSDate = allContainerActions
                            .Where(a => a.CntActionTypeId == 2)
                            .OrderByDescending(a => a.ContainerActionDateTime)
                            .Select(a => (DateTime?)a.ContainerActionDateTime)
                            .FirstOrDefault();


                        item.FirstLOBDate = allContainerActions
                            .Where(a => a.CntActionTypeId == 3)
                            .OrderBy(a => a.ContainerActionDateTime)
                            .Select(a => (DateTime?)a.ContainerActionDateTime)
                            .FirstOrDefault();

                        item.LastDischargeDate = allContainerActions
                            .Where(a => a.CntActionTypeId == 4)
                            .OrderByDescending(a => a.ContainerActionDateTime)
                            .Select(a => (DateTime?)a.ContainerActionDateTime)
                            .FirstOrDefault();

                        item.DTCDate = allContainerActions
                            .Where(a => a.CntActionTypeId == 5)
                            .OrderByDescending(a => a.ContainerActionDateTime)
                            .Select(a => (DateTime?)a.ContainerActionDateTime)
                            .FirstOrDefault();

                        item.RBCDate = allContainerActions
                            .Where(a => a.CntActionTypeId == 6)
                            .OrderByDescending(a => a.ContainerActionDateTime)
                            .Select(a => (DateTime?)a.ContainerActionDateTime)
                            .FirstOrDefault();
                    }
                }
            }
            return vm;
        }
        public async Task<MasterConsoleVm> GetMasterConsoleAsync(Guid id)
        {
            var vm = new MasterConsoleVm();
            vm.BLId = id;
            vm.Summary = await GetBLSummaryAsync(id, default);
            vm.JobId = vm.Summary.JobId;
            return vm;
        }
        public async Task<List<InvoiceRowVm>> GetInvoicesByBlIdAsync(Guid blId, string filterType)
        {
            var query = _context.Invoices
                .AsNoTracking()
                .Where(i => i.BLId == blId && i.InvoiceStatusId != 8);
            switch (filterType)
            {
                case "BuyF":
                    query = query.Where(x => x.FinanceDirectionId == 2 && x.InvoiceFNFId == 1 && x.InvoiceModelId == 1);
                    break;
                case "BuyNF":
                    query = query.Where(x => x.FinanceDirectionId == 2 && x.InvoiceFNFId == 2 && x.InvoiceModelId == 1);
                    break;
                case "SellF":
                    query = query.Where(x => x.FinanceDirectionId == 1 && x.InvoiceFNFId == 1 && x.InvoiceModelId == 1);
                    break;
                case "SellNF":
                    query = query.Where(x => x.FinanceDirectionId == 1 && x.InvoiceFNFId == 2 && x.InvoiceModelId == 1);
                    break;
                case "PreInvoices":
                    query = query.Where(x => x.InvoiceModelId == 2);
                    break;
                default:
                    break;
            }

            return await query
                .OrderBy(x => x.InvoiceModelId).ThenByDescending(x => x.FinanceDirectionId).ThenByDescending(x => x.InvoiceFNFId)
                .Select(i => new InvoiceRowVm
                {
                    InvoiceId = i.Id,
                    InvoiceNo = i.InvoiceNo,
                    InvoiceDate = i.InvoiceDate.Value,
                    PartyName = i.Party != null ? (i.Party.PersonName) : "Unknown",
                    TypeName = i.InvoiceType.InvoiceTypeName,
                    TotalAmount = i.TotalAmount,
                    CurrencyCode = i.Currency.CurrencyCode,
                    StatusName = i.InvoiceStatus.InvoiceStatusName,
                    StatusId = i.InvoiceStatusId,
                    IsPosted = i.IsPostedToAcc,
                    FNFName = i.InvoiceFNF.FNFName,
                    InvocieModelName = i.InvoiceModel.InvoiceModelName,
                    CompanyId = i.CompanyId,
                    CompanyName = i.Company.CompanyAbb,
                    DirectionName = i.FinanceDirection.InvoiceContextName,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    CreateUserName = i.CreateUser.UserName,
                    UpdateUserName = i.UpdateUser.UserName,
                    SettledAmount = i.SettledAmount,
                    RemainingAmount = i.RemainingAmount,
                    SubjectName = i.InvoiceSubject.InvoiceSubjectName,
                    Serial = i.Serial,
                    SerialPrefix = i.SerialPrefix,
                    DirectionId = i.FinanceDirectionId,
                    InvoiceModelId = i.InvoiceModelId
                })
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }
        public async Task<MasterConsoleDetentionsVm?> GetDetentionsByBlAsync(Guid blId, CancellationToken cancellationToken)
        {
            var items = await _context.Detentions
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.MainCurrency)
                .Include(x => x.PaymentCurrency)
                .Include(x => x.Company)
                .Include(x => x.FinanceDirection)
                .Include(x => x.Party)
                .Include(x => x.DetentionStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.EditUser)
                .Where(x => x.BLId == blId)
                .OrderByDescending(x => x.DetentionToDate)
                .Select(x => new DetentionRowVm
                {
                    Id = x.Id,
                    DetentionNo = x.DetentionNo,
                    DirectionName = x.FinanceDirection.InvoiceContextName,
                    CompanyName = x.Company.CompanyAbb,
                    IssueDate = x.CreatedAt,
                    StatusId = x.DetentionStatusId,
                    PartyName = x.Party != null ? x.Party.PersonName : null,
                    StatusTitle = x.DetentionStatus != null ? x.DetentionStatus.DetentionStatusName : null,
                    GrossAmount = x.GrossAmount,
                    VATAmount = x.VATAmount,
                    Amount = x.Amount,
                    DetentionToDate = x.DetentionToDate,
                    IsFinal = x.IsFinal,
                    Remarks = x.Remarks,
                    TariffTitle = x.TariffBook.Name,
                    FreeDays = x.Items.Max(x => x.DetentionItemFreeDays),
                    CreateUserName = x.CreateUser.FullName,
                    IssueDatePersian = PersianDateHelper.ToShamsi(x.CreatedAt),
                    FinalPayableAmount = x.FinalPayableAmount,
                    FinalCurrencyName = x.PaymentCurrency.CurrencyCode,
                    MainCurrencyName = x.MainCurrency.CurrencyCode,
                    Settled = x.SettledAmount,
                    Remain = x.RemainingAmount
                })
                .ToListAsync(cancellationToken);
            return new MasterConsoleDetentionsVm
            {
                BLId = blId,
                BLNo = "-",
                Items = items
            };
        }

        public async Task<List<DoPricePreviewItemDto>> GetDoPricePreviewAsync(Guid blId, int clearanceTypeId, decimal taxRate)
        {
            var sql = @"
                        SELECT 
                            Seq, ChargeTypeId, Description, Qty, UnitPrice, 
                            AmountBeforeDiscount, Discount, AmountAfterDiscount, 
                            TaxRatePercent, TaxAmount, TotalAmount
                        FROM [dbo].[fnGetIssueDOInvoiceItems](@blId, @clearanceTypeId, @taxRate)
                        ORDER BY Seq";
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            var items = await connection.QueryAsync<DoPricePreviewItemDto>(sql, new { blId, clearanceTypeId, taxRate });
            return items.ToList();
        }





    }
}