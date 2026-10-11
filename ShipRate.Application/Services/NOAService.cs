using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Services.SMS;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.NOA;
using ShipRate.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace ShipRate.Application.Services
{
    public sealed class NOAService : INOAService
    {
        private const string PriceCalcSp = "dbo.sp_NOA_CalculatePrices";
        private const string NoaDocType = "NOA";
        private readonly ISmsQueueService _smsQueue;

        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _userService;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;


        public NOAService(
            ApplicationDbContext db,
            ICurrentUserService userService,
            IWebHostEnvironment env,
            ISmsQueueService smsQueue,
            IEmailService emailService)
        {
            _db = db;
            _userService = userService;
            _env = env;
            _smsQueue = smsQueue;
            _emailService = emailService;
        }

        // ---------- شماره‌گذاری اتمیک (بدون race) ----------
        private async Task<(int Sequence, string Number)> NextNumberAsync(int companyId, int inOutId, CancellationToken ct)
        {
            //await EnsureSequenceRowAsync(companyId, ct);
            if (inOutId == 1) return (1, "Others");

            FormattableString sql = $@"
                    UPDATE cmp.NumberingSystems
                    SET CurrentSeq = CurrentSeq + 1
                    OUTPUT INSERTED.CurrentSeq AS Value
                    WHERE CompanyId = {companyId}
                      AND EntityType = {NoaDocType}
                      AND IsDeleted = 0;";


            var sequenceList = await _db.Database
                .SqlQuery<int>(sql)
                .ToListAsync(ct); // استفاده از List برای بررسی تعداد

            if (sequenceList.Count == 0)
                throw new InvalidOperationException("No numbering sequence found for this company/type.");
            int seq = sequenceList[0];
            return (seq, $"{NoaDocType}-{companyId}-{seq:D5}");
        }
        // ---------- جستجوی بارنامه (باید فقط یک رکورد بدهد) ----------
        public async Task<BLMatchResultDto> SearchBLAsync(string searchTerm, CancellationToken ct)
        {
            var q = searchTerm?.Trim();
            if (string.IsNullOrWhiteSpace(q))
                return new BLMatchResultDto { Message = "لطفاً شماره بارنامه را وارد کنید." };

            var query = _db.BLs
                .Where(x => !x.IsDeleted && x.BLNo.Contains(q));
            //.Include(x => x.Voyage)
            //.Include(x => x.Consignee)
            /*                .Include(x => x.NotifyParty)*/

            var count = await query.CountAsync(ct);

            if (count == 0) return new BLMatchResultDto { Message = $"بارنامه‌ای با «{q}» یافت نشد." };
            if (count > 1) return new BLMatchResultDto { Multiple = true, Message = $"بیش از یک رکورد برای «{q}» یافت شد. عبارت دقیق‌تری وارد کنید." };

            var bl = await query.FirstAsync(ct);

            // اگر NOA فعال برای همین BL هست → ویرایش، وگرنه ساخت جدید
            var existing = await _db.NOAs
                .FirstOrDefaultAsync(x => x.BLId == bl.Id && !x.IsDeleted, ct);

            var form = existing is not null
                ? MapToForm(existing)
                : MapBLToNewForm(bl);

            return new BLMatchResultDto { Found = true, Form = form };
        }

        public async Task<NOAFormViewModel> GetNewFormForBLAsync(Guid blId, CancellationToken ct)
        {
            var bl = await _db.BLs
                .FirstOrDefaultAsync(x => x.Id == blId && !x.IsDeleted, ct)
                ?? throw new InvalidOperationException("BL not found.");
            var form = MapBLToNewForm(bl);
            var refData = await GetReferenceDataAsync(blId, ct);
            return new NOAFormViewModel { Form = form, Ref = refData };
        }

        public async Task<NOAFormViewModel> GetFormAsync(Guid id, CancellationToken ct)
        {
            var noa = await _db.NOAs
                .Include(x => x.BL)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
                ?? throw new InvalidOperationException("NOA not found.");

            var form = MapToForm(noa);
            var refData = await GetReferenceDataAsync(noa.BLId, ct);

            var priceGrid = await GetPricesByNOAIdAsync(id, ct);

            priceGrid.ClearanceTypes = await GetClearanceTypesAsync();
            priceGrid.ChargeTypes = await GetChargeTypesAsync();

            return new NOAFormViewModel
            {
                Form = form,
                Ref = refData,
                PriceGrid = priceGrid
            };
        }

        // ---------- لیست صفحه‌بندی ----------
        public async Task<PagedResultDto<NOAListDto>> GetPageAsync(NOAFilterDto filter, CancellationToken ct)
        {
            var companyId = filter.CompanyId > 0 ? filter.CompanyId : 1;

            var query = _db.NOAs
                .Include(x => x.BL)
                .Include(x => x.CreatedBy)
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.Trim();
                query = query.Where(x =>
                    x.NOANumber.Contains(s) ||
                    (x.BL != null && x.BL.BLNo.Contains(s)) ||
                    (x.VesselName != null && x.VesselName.Contains(s)));
            }
            if (!string.IsNullOrWhiteSpace(filter.BlNumber))
            {
                var s = filter.BlNumber.Trim();
                query = query.Where(x =>
                    (x.BL != null && x.BL.BLNo.Contains(s)) ||
                    (x.VesselName != null && x.VesselName.Contains(s)));
            }

            if (filter.FromDate.HasValue) query = query.Where(x => x.IssuedDate >= filter.FromDate);
            if (filter.ToDate.HasValue) query = query.Where(x => x.IssuedDate <= filter.ToDate);

            if (filter.BLId.HasValue && filter.BLId != null && filter.BLId != Guid.Empty)
            {
                query = query.Where(x => x.BLId <= filter.BLId);
            }
            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new NOAListDto
                {
                    Id = x.Id,
                    NOANumber = x.NOANumber,
                    SequenceNumber = x.SequenceNumber,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.CompanyAbb,
                    InOutId = x.InOutId,
                    BLNumber = x.BL != null ? x.BL.BLNo : null,
                    MasterBLNumber = x.BL != null ? x.BL.MasterBLNo : null,
                    VesselName = x.VesselName,
                    InOutName = x.InOutId == 1 ? "In" : "Out",
                    VoyageNo = x.VoyageNo,
                    IssuedDate = x.IssuedDate,
                    AvailableForPickupDate = x.AvailableForPickupDate,
                    HasMaster = x.HasMaster,
                    MasterNOANo = x.MasterNOANo,
                    ConsigneeName = x.Consignee != null ? x.Consignee.PersonName : null,
                    CreatedByUserName = x.CreatedBy.UserName,
                    CreatedAt = x.CreatedAt,
                    ArrivalDate = x.ArrivalDate,
                    MasterNOAReceiveDate = x.MasterNOAReceiveDate
                })
                .ToListAsync(ct);

            var pages = (int)Math.Ceiling((double)total / filter.PageSize);
            return new PagedResultDto<NOAListDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = filter.Page,
                PageSize = filter.PageSize
            };
        }

        // ---------- ذخیره (ایجاد/ویرایش) ----------
        public async Task<SaveNOAResult> SaveAsync(NOAFormDto dto, CancellationToken ct)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            try
            {
                NOA savedNoa = null!;

                await strategy.ExecuteAsync(async () =>
                {
                    await using var tx = await _db.Database.BeginTransactionAsync(ct);
                    NOA noa;
                    if (dto.Id is null || dto.Id == Guid.Empty)
                    {
                        if (dto.BLId == Guid.Empty)
                            throw new InvalidOperationException("BL is not Valid.");

                        var (seq, number) = await NextNumberAsync(dto.CompanyId, dto.InOutId, ct);
                        noa = new NOA
                        {
                            Id = Guid.CreateVersion7(),
                            CompanyId = dto.CompanyId,
                            SequenceNumber = seq,
                            NOANumber = number,
                            BLId = dto.BLId,
                            IssuedDate = dto.IssuedDate ?? DateTime.Now,
                            InOutId = dto.InOutId,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _userService.UserId
                        };
                        _db.NOAs.Add(noa);
                    }
                    else
                    {
                        noa = await _db.NOAs.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, ct);
                    }
                    // Snapshot از Voyage در صورت خالی بودن
                    if (string.IsNullOrWhiteSpace(dto.VesselName) || string.IsNullOrWhiteSpace(dto.VoyageNo))
                    {
                        var voyage = await _db.Voyages.Include(x => x.Vessel).FirstOrDefaultAsync(x => x.Id == dto.VoyageId, ct);
                        if (voyage is not null)
                        {
                            if (string.IsNullOrWhiteSpace(dto.VesselName)) dto.VesselName = voyage.Vessel?.VesselName;
                            if (string.IsNullOrWhiteSpace(dto.VoyageNo)) dto.VoyageNo = voyage.VoyageNo;
                        }
                    }

                    // اعمال HasMaster
                    if (!dto.HasMaster)
                    {
                        dto.MasterNOANo = null;
                        dto.MasterNOAReceiveDate = null;
                    }

                    noa.HasMaster = dto.HasMaster;
                    noa.MasterNOANo = dto.MasterNOANo;
                    noa.MasterNOAReceiveDate = dto.MasterNOAReceiveDate;
                    noa.CarrierId = dto.CarrierId;
                    noa.ArrivalDate = dto.ArrivalDate;
                    noa.AgentId = dto.AgentId;
                    noa.VoyageId = dto.VoyageId;
                    noa.VesselName = dto.VesselName;
                    noa.VoyageNo = dto.VoyageNo;
                    noa.ArrivalPortId = dto.ArrivalPortId;
                    noa.DischargeTerminalId = dto.DischargeTerminalId;
                    noa.AvailableForPickupDate = dto.AvailableForPickupDate;
                    noa.ConsigneeId = dto.ConsigneeId;
                    noa.NotifyPartyId = dto.NotifyPartyId;
                    noa.NOAFormatId = dto.NOAFormatId;
                    noa.SpecialInstructions = dto.SpecialInstructions;
                    noa.Remarks = dto.Remarks;
                    noa.CalcRate = dto.CalcRate;

                    if (dto.IssuedDate.HasValue) noa.IssuedDate = dto.IssuedDate;
                    await _db.SaveChangesAsync(ct);
                    await tx.CommitAsync(ct);

                    savedNoa = noa;
                });
                // ۲. حذف قیمت‌ها/هزینه‌های قبلی در صورت وجود
                var existingPrices = await _db.NOAPrices
                    .Where(p => p.NOAId == savedNoa.Id)
                    .ToListAsync(ct);

                if (existingPrices.Any())
                {
                    _db.NOAPrices.RemoveRange(existingPrices);
                    await _db.SaveChangesAsync(ct);
                }

                // اجرای SP محاسبه قیمت پس از Commit قطعی ترنزکشن
                try
                {
                    var prices = await CalculateAndGetPricesAsync(savedNoa.Id, ct);
                    return new SaveNOAResult
                    {
                        Success = true,
                        Id = savedNoa.Id,
                        NOANumber = savedNoa.NOANumber,
                        Prices = prices
                    };
                }
                catch (Exception ex)
                {
                    return new SaveNOAResult
                    {
                        Success = true,
                        Id = savedNoa.Id,
                        NOANumber = savedNoa.NOANumber,
                        SpError = ex.Message, // سند ذخیره شد ولی SP خطا داد
                    };
                }
            }
            catch (Exception ex)
            {
                return new SaveNOAResult { Error = ex.Message };
            }
        }

        // ---------- اجرای SP محاسبه قیمت ----------
        public async Task<List<NOAPriceDto>> CalculateAndGetPricesAsync(Guid noaId, CancellationToken ct)
        {

            var userId = _userService.UserId;
            // فراخوانی SP شما؛ SPR باید خودش رکوردهای NOAPrice را بنویسد
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC {PriceCalcSp} @NOAId = {noaId},@UserId={userId}", ct);

            return await GetPricesAsync(noaId, ct);
        }

        public async Task<List<NOAPriceDto>> GetPricesAsync(Guid noaId, CancellationToken ct)
        {
            return await _db.NOAPrices
                .Where(p => p.NOAId == noaId && !p.IsDeleted)
                .Include(p => p.ChargeType)
                .Include(p => p.Currency)
                .Include(p => p.ClearanceType)
                .OrderBy(p => p.ClearanceTypeId)
                .ThenBy(x => x.SequenceNo)
                .Select(p => new NOAPriceDto
                {
                    Id = p.Id,
                    NOAId = p.NOAId,
                    ChargeTypeId = p.ChargeTypeId,
                    ChargeTypeName = $"{p.ChargeType.ChargeTypeName}({p.ChargeType.ChargeTypeLocalName})",
                    InitialValue = p.InitialValue,
                    VATAmount = p.VATAmount,
                    TotalValue = p.TotalValue,
                    SysCalcInitialValue = p.SysCalcInitialValue,
                    SysCalcVATAmount = p.SysCalcVATAmount,
                    SysCalcTotalValue = p.SysCalcTotalValue,
                    CurrencyId = p.CurrencyId,
                    CurrencyCode = p.Currency.CurrencyCode,
                    ClearanceTypeId = p.ClearanceTypeId,
                    ClearanceTypeName = p.ClearanceType.ClearanceTypeName
                })
                .ToListAsync(ct);
        }

        // ---------- حذف نرم ----------
        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var noa = await _db.NOAs.FirstAsync(x => x.Id == id && !x.IsDeleted, ct);
            noa.IsDeleted = true;
            noa.DeletedAt = DateTime.Now;
            noa.DeleteUserId = _userService.UserId;
            await _db.SaveChangesAsync(ct);
        }

        // ---------- ارسال SMS / Email ----------
        public async Task<NOASendResultDto> SendAsync(Guid noaId, int sendToolTypeId, string receiver, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(receiver))
                return new NOASendResultDto { Message = "گیرنده مشخص نشده است." };

            var send = new NOASend
            {
                Id = Guid.CreateVersion7(),
                NOAId = noaId,
                SendToolTypeId = sendToolTypeId,
                NOASender = "----",
                NOAReceiver = receiver.Trim(),
                SendDateTime = DateTime.Now,
            };

            _db.NOASends.Add(send);
            await _db.SaveChangesAsync(ct);

            // TODO: اینجا سرویس واقعی SMS/Email را صدا بزنید.
            // await _smsService.SendAsync(...) / _emailService.SendAsync(...);

            return new NOASendResultDto { Success = true, SendId = send.Id };
        }

        public async Task<List<NOASendDto>> GetSendHistoryAsync(Guid noaId, CancellationToken ct)
        {
            return await _db.NOASends
                .Where(s => s.NOAId == noaId && !s.IsDeleted)
                .Include(s => s.SendToolType)
                .OrderByDescending(s => s.SendDateTime)
                .Select(s => new NOASendDto
                {
                    Id = s.Id,
                    SendToolTypeId = s.SendToolTypeId,
                    SendToolTypeName = s.SendToolType.SendToolTypeName,
                    Sender = s.NOASender,
                    Receiver = s.NOAReceiver,
                    SendDateTime = s.SendDateTime,
                    ReceiveDateTime = s.ReceiveDateTime,
                })
                .ToListAsync(ct);
        }

        // ---------- لاگ فعالیت ----------
        private void LogActivity(NOA noa, string action)
            => _db.ActivityLogs.Add(new ActivityLog
            {
                Id = Guid.CreateVersion7(),
                EntityTypeId = 33,
                EntityId = noa.Id,
                CreatedAt = DateTime.Now,
                PerformedByUserId = _userService.UserId
            });

        public async Task<List<NLookupDto>> GetChargeTypesAsync()
        {
            var chargeTypes = await _db.ChargeTypes.Where(c => !c.IsDeleted)
                .Select(c => new NLookupDto { Value = c.Id.ToString(), Text = $"{c.ChargeTypeName}({c.ChargeTypeLocalName})" }).ToListAsync();
            return chargeTypes;
        }
        public async Task<List<NLookupDto>> GetClearanceTypesAsync()
        {
            var list = await _db.ClearanceTypes.Where(c => !c.IsDeleted)
                .Select(c => new NLookupDto
                {
                    Value = c.Id.ToString(),
                    Text = c.Id == 3 ? "Both(هر دو)" : $"{c.ClearanceTypeName}"
                })
                .ToListAsync();
            return list;
        }

        // ---------- Reference Data ----------
        public async Task<ReferenceDataDto> GetReferenceDataAsync(Guid blId, CancellationToken ct)
        {
            var bl = await _db.BLs.Where(x => x.Id == blId).FirstOrDefaultAsync();
            var jobId = bl.JobId;

            var companyIds = await _db.JobCompanyRoles
                            .Where(x => !x.IsDeleted && x.JobId == jobId)
                            .Select(x => x.CompanyId)
                            .Distinct()
                            .ToListAsync();

            var personIds = await _db.JobParties
                            .Where(x => !x.IsDeleted && x.JobId == jobId)
                            .Select(x => x.PartyId)
                            .Distinct()
                            .ToListAsync();

            var blPorts = await _db.BLs
                .Where(x => !x.IsDeleted && x.Id == blId)
                .Select(x => new { x.PODId, x.PODelId })
                .FirstOrDefaultAsync(ct);

            var portIds = new List<Guid>();
            if (blPorts != null)
            {
                if (blPorts.PODId.HasValue && blPorts.PODId.Value != Guid.Empty)
                    portIds.Add(blPorts.PODId.Value);

                if (blPorts.PODelId.HasValue && blPorts.PODelId.Value != Guid.Empty)
                    portIds.Add(blPorts.PODelId.Value);
            }

            return new ReferenceDataDto
            {
                Companies = await _db.Companies.Include(x => x.Person).Where(c => !c.IsDeleted && companyIds.Contains(c.Id))
                    .Select(c => new NLookupDto { Value = c.Id.ToString(), Text = c.Person.PersonName }).ToListAsync(ct),

                Carriers = await _db.Carriers.Include(x => x.Person).Where(x => !x.IsDeleted && personIds.Contains(x.Id))
                    .Select(c => new NLookupDto { Value = c.Id.ToString(), Text = c.Person.PersonName }).ToListAsync(ct),

                Agents = await _db.Agents.Include(x => x.Person).Where(x => !x.IsDeleted && personIds.Contains(x.Id))
                    .Select(c => new NLookupDto { Value = c.Id.ToString(), Text = c.Person.PersonName }).ToListAsync(ct),

                ArrivalPorts = await _db.Locations.Where(x => !x.IsDeleted && x.LocationTypeId == 2 && portIds.Contains(x.Id))
                    .Select(c => new NLookupDto { Value = c.Id.ToString(), Text = c.LocationName }).ToListAsync(ct),
                DischargeTerminals = await _db.Locations.Where(x => !x.IsDeleted && x.LocationTypeId == 6)
                    .Select(c => new NLookupDto { Value = c.Id.ToString(), Text = c.LocationName }).ToListAsync(ct),
                Persons = await _db.Persons.Where(x => !x.IsDeleted && personIds.Contains(x.Id))
                    .Select(p => new NLookupDto { Value = p.Id.ToString(), Text = p.PersonName }).ToListAsync(ct),
                SendToolTypes = await _db.SendToolTypes.Where(x => x.IsActive)
                    .Select(t => new NLookupDto { Value = t.Id.ToString(), Text = t.SendToolTypeName }).ToListAsync(ct),
                InOuts = await _db.InOut.Where(x => x.IsActive)
                    .Select(t => new NLookupDto { Value = t.Id.ToString(), Text = t.InOutName }).ToListAsync(ct),
            };
        }
        // ---------- Mapping ----------
        private static NOAFormDto MapToForm(NOA noa) => new()
        {
            Id = noa.Id,
            NOANumber = noa.NOANumber,
            InOutId = noa.InOutId,
            ArrivalDate = noa.ArrivalDate,
            CalcRate = (long)noa.CalcRate,
            SequenceNumber = noa.SequenceNumber,
            CompanyId = noa.CompanyId,
            HasMaster = noa.HasMaster,
            MasterNOANo = noa.MasterNOANo,
            MasterNOAReceiveDate = noa.MasterNOAReceiveDate,
            CarrierId = noa.CarrierId,
            AgentId = noa.AgentId,
            BLId = noa.BLId,
            BLNumber = noa.BL?.BLNo,
            VoyageId = noa.VoyageId,
            VesselName = noa.VesselName,
            VoyageNo = noa.VoyageNo,
            ArrivalPortId = noa.ArrivalPortId,
            DischargeTerminalId = noa.DischargeTerminalId,
            AvailableForPickupDate = noa.AvailableForPickupDate,
            ConsigneeId = noa.ConsigneeId,
            NotifyPartyId = noa.NotifyPartyId,
            NOAFormatId = noa.NOAFormatId,
            SpecialInstructions = noa.SpecialInstructions,
            Remarks = noa.Remarks,
            IssuedDate = noa.IssuedDate,
        };

        private static NOAFormDto MapBLToNewForm(BL bl) => new()
        {
            BLId = bl.Id,
            BLNumber = bl.BLNo,
            //VoyageId = bl.VoyageId,
            //VesselName = bl.Voyage?.VesselName ?? bl.VesselName,
            //VoyageNo = bl.Voyage?.VoyageNo ?? bl.VoyageNo,
            //ArrivalPortId = bl.ArrivalPortId,
            //DischargeTerminalId = bl.DischargeTerminalId,
            //ConsigneeId = bl.ConsigneeId,
            //NotifyPartyId = bl.NotifyPartyId,
            //CarrierId = bl.CarrierId,
            //AgentId = bl.AgentId,
        };

        public async Task<NOAPrintViewModel?> GetPrintDataAsync(
                Guid noaId,
                CancellationToken ct = default)
        {






            // 1. واکشی داده‌های NOA به همراه روابط کلیدی
            var rawData = await _db.NOAs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == noaId)
                .Select(x => new
                {
                    x.NOANumber,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Person.PersonName,
                    CompanyAbb = x.Company.CompanyAbb,
                    x.IssuedDate,
                    x.ArrivalDate,
                    x.AvailableForPickupDate,
                    x.MasterNOANo,

                    ConsigneeName = x.Consignee != null ? x.Consignee.PersonName : string.Empty,
                    ConsigneeAddress = x.Consignee != null ? x.Consignee.Address : string.Empty,
                    ConsigneePhone = x.Consignee != null ? (x.Consignee.Phone ?? x.Consignee.Mobile ?? string.Empty) : string.Empty,

                    NotifyPartyName = x.NotifyParty != null ? x.NotifyParty.PersonName : string.Empty,
                    NotifyPartyAddress = x.NotifyParty != null ? x.NotifyParty.Address : string.Empty,
                    NotifyPartyPhone = x.NotifyParty != null ? (x.NotifyParty.Phone ?? x.NotifyParty.Mobile ?? string.Empty) : string.Empty,

                    BLId = x.BLId,
                    BLNumber = x.BL.BLNo,
                    VesselName = x.VesselName ?? string.Empty,
                    VoyageNo = x.VoyageNo ?? string.Empty,
                    GoodsDescription = x.BL.GoodsDescription ?? string.Empty,
                    MarksAndNumbers = x.BL.MarksAndNumbers ?? string.Empty,
                    PackageTypeText = x.BL.PackageTypeText ?? string.Empty,
                    TotalPackages = x.BL.TotalPackages,
                    GrossWeightKg = x.BL.GrossWeightKg,
                    VolumeCBM = x.BL.VolumeCBM,
                    TotalContainers = x.BL.TotalContainers,

                    ArrivalPortName = x.ArrivalPort != null ? x.ArrivalPort.LocationName : string.Empty,
                    DischargeTerminalName = x.DischargeTerminal != null ? x.DischargeTerminal.LocationName : string.Empty,

                    SpecialInstructions = x.SpecialInstructions ?? string.Empty,
                    Remarks = x.Remarks ?? string.Empty,

                    CashDepositTip = x.Prices
                                    .Where(p => !p.IsDeleted && p.ChargeTypeId == 141 && p.ClearanceTypeId == 1)
                                    .Sum(p => (decimal?)p.TotalValue) ?? 0m,

                    CashDepositStrip = x.Prices
                                    .Where(p => !p.IsDeleted && p.ChargeTypeId == 141 && p.ClearanceTypeId == 2)
                                    .Sum(p => (decimal?)p.TotalValue) ?? 0m,

                    ChequeGuaranteeTip = x.Prices
                                    .Where(p => !p.IsDeleted && p.ChargeTypeId == 140 && p.ClearanceTypeId == 1)
                                    .Sum(p => (decimal?)p.TotalValue) ?? 0m,

                    ChequeGuaranteeStrip = x.Prices
                                    .Where(p => !p.IsDeleted && p.ChargeTypeId == 140 && p.ClearanceTypeId == 2)
                                    .Sum(p => (decimal?)p.TotalValue) ?? 0m,
                    PriceLines = x.Prices
                        .Where(p => !p.IsDeleted && p.ChargeTypeId != 140 && p.ChargeTypeId != 141)
                        .OrderBy(p => p.SequenceNo)
                        .Select(p => new NOAPriceLineViewModel
                        {
                            SequenceNo = p.SequenceNo,
                            ChargeTypeName = p.ChargeType.ChargeTypeName,
                            ChargeTypeLocalName = p.ChargeType.ChargeTypeLocalName,
                            InitialValue = p.InitialValue,
                            VATAmount = p.VATAmount,
                            TotalValue = p.TotalValue,
                            CurrencyCode = p.Currency.CurrencyCode,
                            CurrencyName = p.Currency.CurrencyName,
                            CurrencySymbol = p.Currency.Symbol ?? p.Currency.CurrencyCode,
                            ClearanceTypeId = p.ClearanceTypeId.Value
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (rawData is null)
                return null;

            // 2. واکشی داده‌های Shipper از جدول BLParties با RoleTypeId = 51
            var shipper = await _db.BLParties
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.BLId == rawData.BLId && p.RoleTypeId == 51)
                .Select(p => new
                {
                    Name = p.Party != null ? p.Party.PersonName : (!string.IsNullOrEmpty(p.PartyTextInBL) ? p.PartyTextInBL : string.Empty),
                    Address = p.Party != null ? p.Party.Address : string.Empty,
                    Phone = p.Party != null ? (p.Party.Phone ?? p.Party.Mobile ?? string.Empty) : (!string.IsNullOrEmpty(p.PartyTelephone) ? p.PartyTelephone : p.PartyMobile ?? string.Empty)
                })
                .FirstOrDefaultAsync(ct);
            // 3. لود بایت‌های لوگوی شرکت
            byte[]? logoBytes = null;
            var logoPath = Path.Combine(_env.WebRootPath, "logos", $"logo-local-{rawData.CompanyId}.png");
            if (File.Exists(logoPath))
            {
                logoBytes = await File.ReadAllBytesAsync(logoPath, ct);
            }

            // 4. نگاشت به ViewModel نهایی
            var vm = new NOAPrintViewModel
            {
                NOANumber = rawData.NOANumber,
                CompanyName = rawData.CompanyName,
                CompanyAbb = rawData.CompanyAbb,
                CompanyLogo = logoBytes,
                IssuedDate = rawData.IssuedDate,
                ArrivalDate = rawData.ArrivalDate,
                AvailableForPickupDate = rawData.AvailableForPickupDate,
                MasterNOANo = rawData.MasterNOANo,

                ConsigneeName = rawData.ConsigneeName,
                ConsigneeAddress = rawData.ConsigneeAddress,
                ConsigneePhone = rawData.ConsigneePhone,

                NotifyPartyName = rawData.NotifyPartyName,
                NotifyPartyAddress = rawData.NotifyPartyAddress,
                NotifyPartyPhone = rawData.NotifyPartyPhone,

                ShipperName = shipper?.Name ?? string.Empty,
                ShipperAddress = shipper?.Address ?? string.Empty,
                ShipperPhone = shipper?.Phone ?? string.Empty,

                BLNumber = rawData.BLNumber,
                VesselName = rawData.VesselName,
                VoyageNo = rawData.VoyageNo,
                GoodsDescription = rawData.GoodsDescription,
                MarksAndNumbers = rawData.MarksAndNumbers,
                PackageTypeText = rawData.PackageTypeText,
                TotalPackages = rawData.TotalPackages,
                GrossWeightKg = rawData.GrossWeightKg,
                VolumeCBM = rawData.VolumeCBM,
                TotalContainers = rawData.TotalContainers,

                ArrivalPortName = rawData.ArrivalPortName,
                DischargeTerminalName = rawData.DischargeTerminalName,

                SpecialInstructions = rawData.SpecialInstructions,
                Remarks = rawData.Remarks,


                CashDepositTip = rawData.CashDepositTip,
                CashDepositStrip = rawData.CashDepositStrip,
                ChequeGuaranteeTip = rawData.ChequeGuaranteeTip,
                ChequeGuaranteeStrip = rawData.ChequeGuaranteeStrip,

                PriceLines = rawData.PriceLines,
                ClosingText = "",
                FooterText = "",
                PageFooterText = "",
                TotalAmount = rawData.PriceLines.Sum(p => p.TotalValue),
                CurrencySymbol = rawData.PriceLines.FirstOrDefault()?.CurrencySymbol ?? string.Empty
            };

            var setting01 = await _db.SystemSettings.Where(x => x.CompanyId == rawData.CompanyId && x.SettingKey.CompareTo("NOA.FooterText") == 0).FirstOrDefaultAsync();
            vm.FooterText = setting01.SettingValue;
            var setting02 = await _db.SystemSettings.Where(x => x.CompanyId == rawData.CompanyId && x.SettingKey.CompareTo("NOA.PageFooterText") == 0).FirstOrDefaultAsync();
            vm.PageFooterText = setting02.SettingValue;
            return vm;
        }



        public async Task<PriceGridVM> SavePricesAsync(PriceGridVM vm, CancellationToken ct = default)
        {
            // دریافت رکوردهای فعلی از دیتابیس
            var existingPrices = await _db.NOAPrices
                .Where(p => p.NOAId == vm.NOAId)
                .ToListAsync(ct);

            foreach (var dto in vm.Prices)
            {
                var entity = existingPrices.FirstOrDefault(x => x.Id == dto.Id || (dto.Id == Guid.Empty && x.ChargeTypeId == dto.ChargeTypeId));

                if (entity != null)
                {
                    // آپدیت فیلدهای ویرایش‌شده
                    if (entity.InitialValue != dto.InitialValue || entity.VATAmount != dto.VATAmount || entity.TotalValue != dto.InitialValue + dto.VATAmount)
                    {
                        entity.InitialValue = dto.InitialValue;
                        entity.VATAmount = dto.VATAmount;
                        entity.TotalValue = dto.InitialValue + dto.VATAmount;
                        entity.EditUserId = _userService.UserId;
                        entity.UpdatedAt = DateTime.Now;
                    }
                }
                else
                {
                    // اگر رکورد جدیدی بود
                    var newPrice = new NOAPrice
                    {
                        Id = Guid.CreateVersion7(),
                        NOAId = vm.NOAId,
                        ChargeTypeId = dto.ChargeTypeId,
                        InitialValue = dto.InitialValue,
                        VATAmount = dto.VATAmount,
                        TotalValue = dto.InitialValue + dto.VATAmount,
                        SysCalcInitialValue = dto.SysCalcInitialValue,
                        SysCalcVATAmount = dto.SysCalcVATAmount,
                        SysCalcTotalValue = dto.SysCalcTotalValue,
                        CurrencyId = dto.CurrencyId,
                    };
                    await _db.NOAPrices.AddAsync(newPrice, ct);
                }
            }

            await _db.SaveChangesAsync(ct);

            return vm;
        }
        public async Task<PriceGridVM> GetPricesByNOAIdAsync(Guid nOAId, CancellationToken cancellationToken)
        {
            var prices = await GetPricesAsync(nOAId, cancellationToken);
            var vm = new PriceGridVM();
            vm.NOAId = nOAId;
            vm.Prices = prices;
            return vm;
        }

        public async Task<NOAFormDto> GetNOAHeaderAsync(Guid id, CancellationToken ct)
        {
            return await _db.NOAs.Where(x => x.Id == id)
                .AsNoTracking()
                .Select(x => new NOAFormDto
                {
                    Id = x.Id,
                    InOutId = x.InOutId,
                    ArrivalDate = x.ArrivalDate,
                    BLNumber = x.BL.BLNo
                }).FirstOrDefaultAsync();

        }
        public async Task<Guid> CreateOutFromInAsync(Guid inNoaId, CancellationToken ct)
        {
            var inNoa = await _db.NOAs
                .Include(n => n.Prices.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(n => n.Id == inNoaId && !n.IsDeleted, ct);

            if (inNoa == null)
                throw new KeyNotFoundException("NOA not found");

            using var transaction = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var bl = await _db.BLs.FirstOrDefaultAsync(b => b.Id == inNoa.BLId, ct);
                var blNo = bl?.BLNo ?? "BL";
                var (seq, numnber) = await NextNumberAsync(inNoa.CompanyId, 2, ct);

                // ساخت اعلامیه خروجی جدید
                var outNoa = new NOA
                {
                    Id = Guid.CreateVersion7(),
                    InOutId = 2, // خروجی به مشتری
                    SequenceNumber = seq,
                    NOANumber = numnber,
                    CompanyId = inNoa.CompanyId,
                    BLId = inNoa.BLId,
                    HasMaster = true,
                    MasterNOANo = inNoa.NOANumber,
                    MasterNOAReceiveDate = inNoa.MasterNOAReceiveDate ?? inNoa.IssuedDate,
                    ArrivalDate = inNoa.ArrivalDate,
                    CarrierId = inNoa.CarrierId,
                    AgentId = inNoa.AgentId,
                    VoyageId = inNoa.VoyageId,
                    VesselName = inNoa.VesselName,
                    VoyageNo = inNoa.VoyageNo,
                    ArrivalPortId = inNoa.ArrivalPortId,
                    DischargeTerminalId = inNoa.DischargeTerminalId,
                    AvailableForPickupDate = inNoa.AvailableForPickupDate,
                    ConsigneeId = inNoa.ConsigneeId,
                    NotifyPartyId = inNoa.NotifyPartyId,
                    NOAFormatId = inNoa.NOAFormatId,
                    CalcRate = inNoa.CalcRate,
                    SpecialInstructions = inNoa.SpecialInstructions,
                    Remarks = inNoa.Remarks,
                    IssuedDate = DateTime.Now,
                    CreateUserId = _userService.UserId,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
                // کپی قیمت‌ها
                if (inNoa.Prices != null && inNoa.Prices.Any())
                {
                    foreach (var sp in inNoa.Prices)
                    {
                        outNoa.Prices.Add(new NOAPrice
                        {
                            Id = Guid.CreateVersion7(),
                            NOAId = outNoa.Id,
                            SequenceNo = sp.SequenceNo,
                            ChargeTypeId = sp.ChargeTypeId,
                            ClearanceTypeId = sp.ClearanceTypeId,
                            CurrencyId = sp.CurrencyId,
                            BankAccountId = sp.BankAccountId,
                            SysCalcInitialValue = sp.SysCalcInitialValue,
                            SysCalcVATAmount = sp.SysCalcVATAmount,
                            SysCalcTotalValue = sp.SysCalcTotalValue,
                            InitialValue = sp.InitialValue,
                            VATAmount = sp.VATAmount,
                            TotalValue = sp.TotalValue,
                            CreateUserId = _userService.UserId,
                            CreatedAt = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                }

                await _db.NOAs.AddAsync(outNoa, ct);
                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return outNoa.Id;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }



        public async Task AddPriceAsync(Guid noaId, NOAPriceDto dto, CancellationToken ct)
        {
            var noa = await _db.NOAs
            .FirstOrDefaultAsync(x => x.Id == noaId && !x.IsDeleted, ct);

            if (noa == null)
                throw new KeyNotFoundException("NOA not found");

            // SequenceNo بعدی
            var maxSeq = await _db.NOAPrices
            .Where(p => p.NOAId == noaId && !p.IsDeleted)
            .MaxAsync(p => (int?)p.SequenceNo, ct) ?? 0;

            var seq = maxSeq + 1;

            // VAT (نمونه: 10%)
            var vatRate = 0.10m;
            var initial = (decimal)dto.InitialValue;
            var vat = Math.Round(initial * (decimal)vatRate, 0);

            var price = new NOAPrice
            {
                Id = Guid.CreateVersion7(),
                NOAId = noaId,
                SequenceNo = seq,
                ClearanceTypeId = dto.ClearanceTypeId!.Value == 3 ? 1 : dto.ClearanceTypeId!.Value,
                ChargeTypeId = dto.ChargeTypeId,
                CurrencyId = 2,

                // شما این فیلدها را دارید:
                InitialValue = (long)initial,
                VATAmount = (long)vat,
                TotalValue = (long)(initial + vat),

                // اگر SysCalc هم می‌خواهید برابر initial شروع شود:
                //SysCalcInitialValue = 0,
                //SysCalcVATAmount = 0,
                //SysCalcTotalValue = 0,
                CreatedAt = DateTime.Now,
                CreateUserId = _userService.UserId
            };
            _db.NOAPrices.Add(price);



            if (dto.ClearanceTypeId!.Value == 3)
            {
                var price2 = new NOAPrice
                {
                    Id = Guid.CreateVersion7(),
                    NOAId = noaId,
                    SequenceNo = seq + 1,
                    ClearanceTypeId = 2,
                    ChargeTypeId = dto.ChargeTypeId,
                    CurrencyId = 2,

                    // شما این فیلدها را دارید:
                    InitialValue = (long)initial,
                    VATAmount = (long)vat,
                    TotalValue = (long)(initial + vat),

                    // اگر SysCalc هم می‌خواهید برابر initial شروع شود:
                    //SysCalcInitialValue = 0,
                    //SysCalcVATAmount = 0,
                    //SysCalcTotalValue = 0,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _userService.UserId
                };
                _db.NOAPrices.Add(price2);
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task DispatchNOANotificationAsync(NOASendRequestDto dto, string currentUserId)
        {
            Guid? smsMessageId = null;
            DateTime? receiveTime = null;
            if (dto.SendToolTypeId == 2)
            {
                var cleanedMobile = CleanAndValidateMobile(dto.Receiver);
                if (string.IsNullOrEmpty(cleanedMobile))
                {
                    throw new ArgumentException("Receiver's Mobile is not correct!!!!!.");
                }
                smsMessageId = await _smsQueue.EnqueueSingleAsync(
                        mobile: dto.Receiver,
                        body: $"{dto.MessageText}",
                        tag: "NOA_DISPATCH"
                    );
            }
            else if (dto.SendToolTypeId == 1) // Email Service (همراه با پیوست PDF)
            {

                if (string.IsNullOrWhiteSpace(dto.Receiver) || !new EmailAddressAttribute().IsValid(dto.Receiver))
                {
                    throw new ArgumentException("The Email Address is not Valid!!!!!.");
                }
                var noaInfo = await _db.NOAs
                    .Where(x => x.Id == dto.NOAId)
                    .Select(x => new { x.NOANumber, BLNumber = x.BL.BLNo, Consignee = x.Consignee.PersonName, x.ArrivalDate, x.VesselName, x.VoyageNo })
                    .FirstOrDefaultAsync();
                var subject = $"Notice of Arrival - BL: {noaInfo?.BLNumber} - NOA: {noaInfo?.NOANumber}";


                string htmlBody = $@"
        <div dir='ltr' style='font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px; overflow: hidden;'>
            <div style='background-color: #004a99; padding: 20px; text-align: center;'>
                <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>Notice of Arrival</h1>
            </div>
            <div style='padding: 25px; background-color: #ffffff;'>
                <p>Dear <strong>{noaInfo.Consignee}</strong>,</p>
                <p>Please be advised that the following shipment is scheduled to arrive at the destination port. Attached to this email, you will find the formal <strong>Notice of Arrival (NOA)</strong> document.</p>
                
                <div style='background-color: #f8f9fa; border-left: 4px solid #004a99; padding: 15px; margin: 20px 0;'>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr><td style='padding: 5px; color: #666;'>B/L Number:</td><td style='font-weight: bold;'>{noaInfo?.BLNumber}</td></tr>
                        <tr><td style='padding: 5px; color: #666;'>Vessel / Voyage:</td><td style='font-weight: bold;'>{noaInfo?.VesselName} / {noaInfo?.VoyageNo}</td></tr>
                        <tr><td style='padding: 5px; color: #666;'>Arrival:</td><td style='font-weight: bold;'>{noaInfo?.ArrivalDate?.ToString("yyyy-MM-dd")}</td></tr>
                        <tr><td style='padding: 5px; color: #666;'>NOA Ref:</td><td style='font-weight: bold;'>{noaInfo?.NOANumber}</td></tr>
                    </table>
                </div>
                <p style='margin-bottom: 25px;'>{dto.MessageText}</p>
                <div style='text-align: center;'>
                    <a href='https://portal.Shiprate.com' style='background-color: #004a99; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>View in Customer Portal</a>
                </div>
            </div>
            <div style='background-color: #f1f1f1; padding: 15px; text-align: center; font-size: 12px; color: #777;'>
                <p style='margin: 0;'>ShipRate Logistics Management System</p>
                <p style='margin: 5px 0;'>This is an automated notification. Please do not reply directly to this email.</p>
            </div>
        </div>";
                var (pdfBytes, pdfFileName) = await GenerateNoaPdfBytesAsync(dto.NOAId, default);
                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    await _emailService.SendEmailWithAttachmentAsync(
                        to: dto.Receiver.Trim(),
                        subject: subject,
                        body: htmlBody, // متن HTML حرفه‌ای جایگزین شد
                        attachmentBytes: pdfBytes,
                        attachmentFileName: pdfFileName ?? $"NOA-{noaInfo?.NOANumber}.pdf"
                    );
                }
                else
                {
                    await _emailService.SendEmailViaMailKitAsync(
                        toList: new List<string> { dto.Receiver.Trim() },
                        subject: subject,
                        body: dto.MessageText,
                        isHtml: true
                    );
                }

                // چون ایمیل تحویل سرور SMTP شد، زمان دریافت ثبت می‌شود
                receiveTime = DateTime.Now;
            }
            else if (dto.SendToolTypeId == 3) // Customer Portal
            {
                receiveTime = DateTime.Now;
            }




            // ۲. ثبت رویداد در جدول لاگ دامنه NOASend
            var noaSend = new NOASend
            {
                Id = Guid.CreateVersion7(),
                NOAId = dto.NOAId,
                SendToolTypeId = dto.SendToolTypeId,
                NOASender = dto.Sender,
                NOAReceiver = dto.Receiver,
                SendDateTime = DateTime.Now,
                SmsMessageId = smsMessageId
            };
            _db.NOASends.Add(noaSend);
            await _db.SaveChangesAsync();
        }

        private static string? CleanAndValidateMobile(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return null;

            // حذف فاصله‌ها، خط تیره و پرانتزها
            var cleaned = Regex.Replace(mobile.Trim(), @"[\s\-\(\)]+", "");

            // تبدیل ارقام فارسی یا عربی به انگلیسی در صورت وجود
            cleaned = cleaned
                .Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9")
                .Replace("٠", "0").Replace("١", "1").Replace("٢", "2").Replace("٣", "3").Replace("٤", "4")
                .Replace("٥", "5").Replace("٦", "6").Replace("٧", "7").Replace("٨", "8").Replace("٩", "9");

            // تبدیل پیش‌شماره‌های بین‌المللی به 0
            if (cleaned.StartsWith("+98"))
                cleaned = "0" + cleaned[3..];
            else if (cleaned.StartsWith("0098"))
                cleaned = "0" + cleaned[4..];
            else if (cleaned.StartsWith("98") && cleaned.Length == 12)
                cleaned = "0" + cleaned[2..];
            else if (!cleaned.StartsWith("0") && cleaned.Length == 10 && cleaned.StartsWith("9"))
                cleaned = "0" + cleaned;

            // الگوی استاندارد شماره موبایل ایران: 09xxxxxxxxx (11 رقم)
            var match = Regex.Match(cleaned, @"^09\d{9}$");
            if (match.Success)
            {
                return cleaned;
            }

            return null; // نامعتبر
        }


        public async Task<NOASendHubViewModel> GetSendHubViewModelAsync(Guid noaId)
        {
            // ۱. واکشی اطلاعات NOA همراه با بارنامه، طرفین پرونده و تاریخچه ارسال‌ها
            var noa = await _db.NOAs
                .AsNoTracking()
                .Include(n => n.BL)
                .Include(x => x.Consignee)
                .Include(x => x.NotifyParty)
                .Include(n => n.Sends)
                    .ThenInclude(s => s.SendToolType)
                .Include(n => n.Sends)
                    .ThenInclude(s => s.SmsMessage)
                        .ThenInclude(s => s.Recipients)
                .FirstOrDefaultAsync(n => n.Id == noaId);

            if (noa == null)
                throw new KeyNotFoundException($"Notice of Arrival with ID '{noaId}' not found.");

            // ۲. استخراج هوشمند اطلاعات پیش‌فرض گیرنده (اولویت با Consignee، سپس Notify Party)
            var defaultMobile = noa.Consignee?.Mobile
                                ?? noa.NotifyParty?.Mobile
                                ?? string.Empty;

            var defaultEmail = noa.Consignee?.EmailAddress
                               ?? noa.NotifyParty?.EmailAddress
                               ?? string.Empty;

            var defaultPortalUser = noa.Consignee?.PersonName
                                    ?? string.Empty;

            // ۳. نگاشت لاگ‌های ارسال به NOASendDto (مرتب‌شده از جدیدترین به قدیمی‌ترین)
            var historyList = noa.Sends
                .OrderByDescending(s => s.SendDateTime ?? s.CreatedAt)
                .Select(s => new NOASendDto
                {
                    Id = s.Id,
                    SendToolTypeId = s.SendToolTypeId,
                    SendToolTypeName = s.SendToolType != null ? s.SendToolType.SendToolTypeName : "Unknown",
                    Sender = s.NOASender,
                    Receiver = s.NOAReceiver,
                    SendDateTime = s.SendDateTime,
                    ReceiveDateTime = s.ReceiveDateTime,
                    SMSMessageId = s.SmsMessageId,
                    SmsStatus = s.SmsMessage is null ? null : s.SmsMessage.Recipients?.FirstOrDefault()?.Status
                })
                .ToList();
            // ۴. ساخت و بازگرداندن ViewModel
            return new NOASendHubViewModel
            {
                NOAId = noa.Id,
                NOANumber = noa.NOANumber ?? "-",
                BLNumber = noa.BL?.BLNo ?? "-",
                DefaultSender = "System / Line SMS",
                DefaultMobile = defaultMobile,
                DefaultEmail = defaultEmail,
                DefaultPortalUser = defaultPortalUser,
                History = historyList
            };
        }
        public async Task<(byte[]? FileBytes, string? FileName)> GenerateNoaPdfBytesAsync(Guid id, CancellationToken ct)
        {
            var noa = await GetNOAHeaderAsync(id, ct);
            if (noa == null || noa.InOutId == 1) return (null, null);

            var model = await GetPrintDataAsync(id, ct);
            if (model == null) return (null, null);

            var matrixPriceLines = model.PriceLines
                .GroupBy(p => p.ChargeTypeLocalName)
                .Select(g => new NOAPriceMatrixReportDto
                {
                    SequenceNo = g.Min(x => x.SequenceNo),
                    ChargeTypeName = g.Key,
                    ChargeTypeLocalName = g.Key,
                    TipInitialValue = g.FirstOrDefault(x => x.ClearanceTypeId == 1)?.InitialValue ?? 0,
                    TipVATAmount = g.FirstOrDefault(x => x.ClearanceTypeId == 1)?.VATAmount ?? 0,
                    TipTotalValue = g.FirstOrDefault(x => x.ClearanceTypeId == 1)?.TotalValue ?? 0,
                    StripInitialValue = g.FirstOrDefault(x => x.ClearanceTypeId == 2)?.InitialValue ?? 0,
                    StripVATAmount = g.FirstOrDefault(x => x.ClearanceTypeId == 2)?.VATAmount ?? 0,
                    StripTotalValue = g.FirstOrDefault(x => x.ClearanceTypeId == 2)?.TotalValue ?? 0
                })
                .OrderBy(x => x.SequenceNo)
                .ToList();

            var dtHeader = new DataTable("NOA");
            dtHeader.Columns.Add("NOANumber", typeof(string));
            dtHeader.Columns.Add("CompanyName", typeof(string));
            dtHeader.Columns.Add("CompanyAbb", typeof(string));
            dtHeader.Columns.Add("CompanyLogo", typeof(byte[]));
            dtHeader.Columns.Add("IssuedDate", typeof(DateTime));
            dtHeader.Columns.Add("ArrivalDate", typeof(DateTime));
            dtHeader.Columns.Add("AvailableForPickupDate", typeof(DateTime));
            dtHeader.Columns.Add("MasterNOANo", typeof(string));
            dtHeader.Columns.Add("ConsigneeName", typeof(string));
            dtHeader.Columns.Add("ConsigneeAddress", typeof(string));
            dtHeader.Columns.Add("ConsigneePhone", typeof(string));
            dtHeader.Columns.Add("NotifyPartyName", typeof(string));
            dtHeader.Columns.Add("NotifyPartyAddress", typeof(string));
            dtHeader.Columns.Add("NotifyPartyPhone", typeof(string));
            dtHeader.Columns.Add("ShipperName", typeof(string));
            dtHeader.Columns.Add("ShipperAddress", typeof(string));
            dtHeader.Columns.Add("ShipperPhone", typeof(string));
            dtHeader.Columns.Add("BLNumber", typeof(string));
            dtHeader.Columns.Add("VesselName", typeof(string));
            dtHeader.Columns.Add("VoyageNo", typeof(string));
            dtHeader.Columns.Add("GoodsDescription", typeof(string));
            dtHeader.Columns.Add("MarksAndNumbers", typeof(string));
            dtHeader.Columns.Add("PackageTypeText", typeof(string));
            dtHeader.Columns.Add("TotalPackages", typeof(int));
            dtHeader.Columns.Add("GrossWeightKg", typeof(decimal));
            dtHeader.Columns.Add("VolumeCBM", typeof(decimal));
            dtHeader.Columns.Add("TotalContainers", typeof(int));
            dtHeader.Columns.Add("ArrivalPortName", typeof(string));
            dtHeader.Columns.Add("DischargeTerminalName", typeof(string));
            dtHeader.Columns.Add("SpecialInstructions", typeof(string));
            dtHeader.Columns.Add("Remarks", typeof(string));
            dtHeader.Columns.Add("ClosingText", typeof(string));
            dtHeader.Columns.Add("FooterText", typeof(string));
            dtHeader.Columns.Add("PageFooterText", typeof(string));
            dtHeader.Columns.Add("TotalAmount", typeof(decimal));
            dtHeader.Columns.Add("CurrencySymbol", typeof(string));

            dtHeader.Columns.Add("CashDepositTip", typeof(decimal));
            dtHeader.Columns.Add("CashDepositStrip", typeof(decimal));
            dtHeader.Columns.Add("ChequeGuaranteeTip", typeof(decimal));
            dtHeader.Columns.Add("ChequeGuaranteeStrip", typeof(decimal));

            var headerRow = dtHeader.NewRow();
            headerRow["NOANumber"] = model.NOANumber ?? string.Empty;
            headerRow["CompanyName"] = model.CompanyName ?? string.Empty;
            headerRow["CompanyAbb"] = model.CompanyAbb ?? string.Empty;
            headerRow["CompanyLogo"] = model.CompanyLogo != null ? (object)model.CompanyLogo : DBNull.Value;
            headerRow["IssuedDate"] = model.IssuedDate.HasValue ? (object)model.IssuedDate.Value : DBNull.Value;
            headerRow["ArrivalDate"] = model.ArrivalDate.HasValue ? (object)model.ArrivalDate.Value : DBNull.Value;
            headerRow["AvailableForPickupDate"] = model.AvailableForPickupDate.HasValue ? (object)model.AvailableForPickupDate.Value : DBNull.Value;
            headerRow["MasterNOANo"] = model.MasterNOANo ?? string.Empty;
            headerRow["ConsigneeName"] = model.ConsigneeName ?? string.Empty;
            headerRow["ConsigneeAddress"] = model.ConsigneeAddress ?? string.Empty;
            headerRow["ConsigneePhone"] = model.ConsigneePhone ?? string.Empty;
            headerRow["NotifyPartyName"] = model.NotifyPartyName ?? string.Empty;
            headerRow["NotifyPartyAddress"] = model.NotifyPartyAddress ?? string.Empty;
            headerRow["NotifyPartyPhone"] = model.NotifyPartyPhone ?? string.Empty;
            headerRow["ShipperName"] = model.ShipperName ?? string.Empty;
            headerRow["ShipperAddress"] = model.ShipperAddress ?? string.Empty;
            headerRow["ShipperPhone"] = model.ShipperPhone ?? string.Empty;
            headerRow["BLNumber"] = model.BLNumber ?? string.Empty;
            headerRow["VesselName"] = model.VesselName ?? string.Empty;
            headerRow["VoyageNo"] = model.VoyageNo ?? string.Empty;
            headerRow["GoodsDescription"] = model.GoodsDescription ?? string.Empty;
            headerRow["MarksAndNumbers"] = model.MarksAndNumbers ?? string.Empty;
            headerRow["PackageTypeText"] = model.PackageTypeText ?? string.Empty;
            headerRow["TotalPackages"] = model.TotalPackages;
            headerRow["GrossWeightKg"] = model.GrossWeightKg.HasValue ? (object)model.GrossWeightKg.Value : DBNull.Value;
            headerRow["VolumeCBM"] = model.VolumeCBM.HasValue ? (object)model.VolumeCBM.Value : DBNull.Value;
            headerRow["TotalContainers"] = model.TotalContainers;
            headerRow["ArrivalPortName"] = model.ArrivalPortName ?? string.Empty;
            headerRow["DischargeTerminalName"] = model.DischargeTerminalName ?? string.Empty;
            headerRow["SpecialInstructions"] = model.SpecialInstructions ?? string.Empty;
            headerRow["Remarks"] = model.Remarks ?? string.Empty;
            headerRow["ClosingText"] = model.ClosingText ?? string.Empty;
            headerRow["FooterText"] = model.FooterText ?? string.Empty;
            headerRow["PageFooterText"] = model.PageFooterText ?? string.Empty;
            headerRow["TotalAmount"] = model.TotalAmount;
            headerRow["CurrencySymbol"] = model.CurrencySymbol ?? string.Empty;

            headerRow["CashDepositTip"] = model.CashDepositTip;
            headerRow["CashDepositStrip"] = model.CashDepositStrip;
            headerRow["ChequeGuaranteeTip"] = model.ChequeGuaranteeTip;
            headerRow["ChequeGuaranteeStrip"] = model.ChequeGuaranteeStrip;


            dtHeader.Rows.Add(headerRow);

            var dtPrices = new DataTable("PriceLines");
            dtPrices.Columns.Add("SequenceNo", typeof(int));
            dtPrices.Columns.Add("ChargeTypeName", typeof(string));
            dtPrices.Columns.Add("ChargeTypeLocalName", typeof(string));
            dtPrices.Columns.Add("TipInitialValue", typeof(decimal));
            dtPrices.Columns.Add("TipVATAmount", typeof(decimal));
            dtPrices.Columns.Add("TipTotalValue", typeof(decimal));
            dtPrices.Columns.Add("StripInitialValue", typeof(decimal));
            dtPrices.Columns.Add("StripVATAmount", typeof(decimal));
            dtPrices.Columns.Add("StripTotalValue", typeof(decimal));

            foreach (var item in matrixPriceLines)
            {
                var row = dtPrices.NewRow();
                row["SequenceNo"] = item.SequenceNo;
                row["ChargeTypeName"] = item.ChargeTypeName ?? string.Empty;
                row["ChargeTypeLocalName"] = item.ChargeTypeLocalName ?? string.Empty;
                row["TipInitialValue"] = item.TipInitialValue;
                row["TipVATAmount"] = item.TipVATAmount;
                row["TipTotalValue"] = item.TipTotalValue;
                row["StripInitialValue"] = item.StripInitialValue;
                row["StripVATAmount"] = item.StripVATAmount;
                row["StripTotalValue"] = item.StripTotalValue;
                dtPrices.Rows.Add(row);
            }
            using var report = new Report();
            var reportPath = Path.Combine(_env.ContentRootPath, "Reports", "NOA_Report.frx");
            report.Load(reportPath);

            report.RegisterData(dtHeader, "NOA");
            report.RegisterData(dtPrices, "PriceLines");

            var noaSource = report.GetDataSource("NOA");
            if (noaSource != null) noaSource.Enabled = true;

            var priceSource = report.GetDataSource("PriceLines");
            if (priceSource != null) priceSource.Enabled = true;

            report.Prepare();

            using var ms = new MemoryStream();



            var pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, ms);
            return (ms.ToArray(), $"NOA-{model.NOANumber}.pdf");
        }
    }
}