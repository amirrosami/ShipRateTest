using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.DetentionCalculation;
using ShipRate.Infrastructure.Data;
using System.Data;

namespace ShipRate.Application.Services
{
    public class DetentionService : IDetentionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DetentionService(ApplicationDbContext context, ICurrentUserService currentUserService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _currentUserService = currentUserService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<PagedResultDto<DetentionListItemVm>> GetPagedListAsync(DetentionSearchVm search)
        {
            search.Normalize();

            var query = _context.Detentions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search.BLNo))
                query = query.Where(x => x.BL.BLNo.Contains(search.BLNo));

            if (!string.IsNullOrEmpty(search.BookingNo))
                query = query.Where(x => x.Booking.BookingNo.Contains(search.BookingNo));


            if (!string.IsNullOrEmpty(search.JobFileNo))
                query = query.Where(x => x.BL.Job.JobFileNumber == search.JobFileNo || x.Booking.Job.JobFileNumber == search.JobFileNo);

            if (!search.showInactive)
            {
                query = query.Where(x => x.DetentionStatusId != -1);
            }
            if (!string.IsNullOrEmpty(search.PartyName))
                query = query.Where(x => x.Party.PersonName.Contains(search.PartyName));

            if (search.IsFinal.HasValue)
            {
                query = query.Where(c => c.IsFinal == search.IsFinal.Value);
            }

            if (search.DetentionStatusId.HasValue)
                query = query.Where(x => x.DetentionStatusId == search.DetentionStatusId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new DetentionListItemVm
                {
                    Id = x.Id,
                    BLNo = x.BL != null ? x.BL.BLNo : "-",
                    BookingNo = x.Booking != null ? x.Booking.BookingNo : "-",
                    StatusName = x.DetentionStatus.DetentionStatusName,
                    PartyName = x.Party.PersonName,
                    Amount = x.Amount,
                    Currency = x.MainCurrency.CurrencyCode,
                    DetentionToDate = x.DetentionToDate,
                    IsFinal = x.IsFinal,
                    CompanyName = x.Company.Person.PersonName,
                    StatusId = x.DetentionStatusId,
                    FinanceDirectionName = x.FinanceDirection.InvoiceContextName,
                    FinalAmount = x.FinalPayableAmount,
                    FinalCurrency = x.PaymentCurrency.CurrencyCode,
                    RemainAmount = x.RemainingAmount,
                    SettledAmount = x.SettledAmount,
                    VATAmount = x.VATAmount,
                    ExchangeRateValue = x.ExchangeRateValue

                }).ToListAsync();
            return new PagedResultDto<DetentionListItemVm>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = search.Page,
                PageSize = search.PageSize
            };
        }

        public async Task<List<DetentionItemListItemVm>> GetItemsAsync(Guid detentionId)
        {
            return await _context.DetentionItems
                .AsNoTracking()
                .Where(x => x.DetentionId == detentionId)
                .Select(x => new DetentionItemListItemVm
                {
                    Id = x.Id,
                    ContainerNo = x.ContainerNumber,
                    ContainerType = x.ContainerSizeType.ShortName,
                    StartDate = x.DetentionItemStartDate,
                    EndDate = x.DetentionItemEndDate,
                    TotalDays = x.DetentionItemTotalDays,
                    FreeDays = x.DetentionItemFreeDays,
                    LateDays = x.DetentionItemLateDays,
                    Amount = x.DetentionItemAmount
                }).ToListAsync();
        }

        public async Task<List<DetentionItemDetailListItemVm>> GetItemDetailsAsync(Guid detentionItemId)
        {
            return await _context.DetentionItemDetails
                .AsNoTracking()
                .Where(x => x.DetentionItemId == detentionItemId)
                .OrderBy(x => x.DetentionItemDetailFromDate)
                .Select(x => new DetentionItemDetailListItemVm
                {
                    FromDate = x.DetentionItemDetailFromDate,
                    ToDate = x.DetentionItemDetailToDate,
                    TotalDays = x.DetentionItemDetailTotalDays,
                    RatePerDay = x.DetentionItemDetailRatePerDay,
                    Amount = x.DetentionItemDetailAmount
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDetentionStatusesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DetentionStatuses
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DetentionStatusName
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<DetentionCalculationResultVm> CreateAndCalculateAsync(CreateDetentionVm model, CancellationToken cancellationToken = default)
        {
            var initialStatusId = 1;
            var detention = new Detention
            {
                Id = Guid.CreateVersion7(),
                CompanyId = model.CompanyId,
                PartyId = model.PartyId,
                BLId = model.BLId,
                BookingId = model.BookingId,
                TariffBookId = model.TariffBookId,

                TradeTypeId = model.TradeTypeId,
                DetentionStatusId = initialStatusId,

                DetentionToDate = model.DetentionToDate.Date,

                MainCurrencyId = model.MainCurrencyId,
                FinalCurrencyId = model.FinalCurrencyId,


                ExchangeRateId = model.ExchangeRateId,
                ExchangeRateValue = model.ExchangeRateValue,
                Remarks = model.Remarks?.Trim(),

                GrossAmount = 0,
                VATAmount = 0,
                Amount = 0,
                FinalPayableAmount = 0,
                FinalVATAmount = 0,
                DetentionUpdateById = 2,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,
                IsFinal = false,
                FinanceDirectionId = model.FinanceDirectionId
            };

            _context.Detentions.Add(detention);

            await _context.SaveChangesAsync(cancellationToken);
            await RunDetentionCalculationBlackBoxAsync(
                detention.Id,
                model.BLId,
                model.BookingId,
                cancellationToken);

            return await GetCalculationResultAsync(
                detention.Id,
                cancellationToken);
        }

        private async Task RunDetentionCalculationBlackBoxAsync(
            Guid detentionId,
            Guid? blId,
            Guid? bookingId,
            CancellationToken cancellationToken = default)
        {
            if (bookingId.HasValue && bookingId != Guid.Empty)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_CalcDetentionItems_For_Booking @DetentionId = {detentionId}", cancellationToken);
            }
            else if (blId.HasValue && blId != Guid.Empty)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.sp_CalcDetentionItems_For_BL @DetentionId = {detentionId}", cancellationToken);
            }
        }
        private async Task<DetentionCalculationResultVm> GetCalculationResultAsync(
    Guid detentionId,
    CancellationToken cancellationToken)
        {
            var detention = await _context.Detentions
                .AsNoTracking()
                .Where(x => x.Id == detentionId)
                .Select(x => new DetentionCalculationResultVm
                {
                    DetentionId = x.Id,
                    GrossAmount = x.GrossAmount,
                    VATAmount = x.VATAmount,
                    Amount = x.Amount,
                    MainCurrencyCode = x.MainCurrency.CurrencyCode,
                    DetentionToDate = x.DetentionToDate,
                    PayableAmount = x.FinalPayableAmount,
                    PaymentCurrencyCode = x.PaymentCurrency.CurrencyCode,
                    Items = x.Items
                        .OrderBy(i => i.DetentionItemStartDate)
                        .Select(i => new DetentionItemListItemVm
                        {
                            Id = i.Id,
                            ContainerNo = i.ContainerNumber,
                            StartDate = i.DetentionItemStartDate,
                            EndDate = i.DetentionItemEndDate,
                            TotalDays = i.DetentionItemTotalDays,
                            FreeDays = i.DetentionItemFreeDays,
                            LateDays = i.DetentionItemLateDays,
                            Amount = i.DetentionItemAmount
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (detention is null)
            {
                throw new KeyNotFoundException("Detention was not found.");
            }

            return detention;
        }
        public async Task<List<SelectListItem>> GetBLSelectListAsync(Guid? blId)
        {
            var query = _context.BLs
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (blId != null && blId.HasValue && blId != Guid.Empty)
            {
                query = query.Where(x => x.Id == blId);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLNo
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetBookingSelectListAsync(Guid? bookingId)
        {
            var query = _context.Bookings
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (bookingId != null && bookingId.HasValue && bookingId != Guid.Empty)
            {
                query = query.Where(x => x.Id == bookingId);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BookingNo
                })
                .ToListAsync();
        }



        public async Task<DetentionStartBaseViewModel?> GetDetentionStartBaseAsync(Guid? blId, Guid? bookingId, int companyId)
        {
            var query = _context.DetentionStartBases.Where(x => x.CompanyId == companyId)
                      .AsQueryable();


            //var entity = await _context.DetentionStartBases
            //    .FirstOrDefaultAsync(x => x.BLId == blId && x.CompanyId == companyId);

            if (blId.HasValue && blId != Guid.Empty)
            {
                query = query.Where(x => x.BLId == blId);
            }
            else if (bookingId.HasValue && bookingId != Guid.Empty)
            {
                query = query.Where(x => x.BookingId == bookingId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return null;

            return new DetentionStartBaseViewModel
            {
                BLId = blId.HasValue && blId != Guid.Empty ? entity.BLId : null,
                BookingId = bookingId.HasValue && bookingId != Guid.Empty ? entity.BookingId : null,
                CompanyId = entity.CompanyId,
                DetentionStartBaseDateForBuy = entity.DetentionStartBaseDateForBuy,
                DetentionStartBaseDateForSell = entity.DetentionStartBaseDateForSell
            };
        }
        public async Task SaveDetentionStartBaseAsync(DetentionStartBaseViewModel model)
        {
            var entity = await _context.DetentionStartBases
                .FirstOrDefaultAsync(x => (x.BLId == model.BLId || x.BookingId == model.BookingId) && x.CompanyId == model.CompanyId);
            if (entity != null)
            {
                entity.DetentionStartBaseDateForBuy = model.DetentionStartBaseDateForBuy.Value;
                entity.DetentionStartBaseDateForSell = model.DetentionStartBaseDateForSell.Value;
                entity.EditUserId = _currentUserService.UserId;
                entity.UpdatedAt = DateTime.Now;
                _context.Update(entity);
            }
            else
            {
                var newEntity = new DetentionStartBase
                {
                    BLId = model.BLId.HasValue && model.BLId != Guid.Empty ? model.BLId : null,
                    BookingId = model.BookingId.HasValue && model.BookingId != Guid.Empty ? model.BookingId : null,
                    CompanyId = model.CompanyId,
                    DetentionStartBaseDateForBuy = model.DetentionStartBaseDateForBuy.Value,
                    DetentionStartBaseDateForSell = model.DetentionStartBaseDateForSell.Value,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                await _context.DetentionStartBases.AddAsync(newEntity);
            }
            await _context.SaveChangesAsync();
        }



        public async Task<byte[]> GenerateDetentionReportPdfAsync(Guid detentionId, CancellationToken cancellationToken = default)
        {
            // ۱. خواندن اطلاعات کامل از دیتابیس
            var detention = await _context.Detentions
                .AsNoTracking()
                .Include(d => d.Company)
                .Include(d => d.Party)
                .Include(d => d.FinanceDirection)
                .Include(d => d.BL)
                .Include(d => d.Booking)
                .Include(d => d.TariffBook)
                .Include(d => d.TradeType)
                .Include(d => d.DetentionStatus)
                .Include(d => d.MainCurrency)
                .Include(d => d.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.ContainerSizeType)
                .FirstOrDefaultAsync(d => d.Id == detentionId && !d.IsDeleted, cancellationToken);

            if (detention == null)
                throw new KeyNotFoundException($"Detention record with Id '{detentionId}' not found.");

            //// ۲. تعیین مسیر لوگوی شرکت
            //var logoFileName = $"logo-{detention.CompanyId}.png";
            //var fullLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", logoFileName);
            //if (!File.Exists(fullLogoPath))
            //{
            //    // در صورت نبود لوگوی اختصاصی، مسیر پیش‌فرض یا خالی
            //    fullLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", "default-logo.png");
            //    if (!File.Exists(fullLogoPath)) fullLogoPath = string.Empty;
            //}



            // ۳. نگاشت مستر
            var masterDto = new DetentionReportMasterDto
            {
                DetentionId = detention.Id,
                BLNo = detention.BL?.BLNo ?? "-",
                BookingNo = detention.Booking?.BookingNo ?? "-",
                CompanyName = detention.Company?.Person?.LastName ?? string.Empty,
                IssuerPartyName = detention.Party?.PersonName ?? detention.Party?.PersonName ?? "-",
                IssuedToPartyName = "-",
                TariffBookName = detention.TariffBook?.Name ?? "-",
                TradeTypeName = detention.TradeType?.TradeTypeName ?? "-",
                StatusName = detention.DetentionStatus?.DetentionStatusName ?? "-",
                DetentionToDate = detention.DetentionToDate,
                GrossAmount = detention.GrossAmount,
                VATAmount = detention.VATAmount,
                Amount = detention.Amount,
                CurrencyCode = detention.MainCurrency?.CurrencyCode ?? "USD",
                Remarks = detention.Remarks,
                PrintDate = DateTime.Now
            };

            var logoPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", $"logo-{detention.CompanyId}.png");
            if (File.Exists(logoPhysicalPath))
            {
                masterDto.LogoData = await File.ReadAllBytesAsync(logoPhysicalPath);
            }




            // ۴. نگاشت اقلام کانتینرها
            var itemsDtoList = detention.Items.Select(i => new DetentionReportItemDto
            {
                ContainerNumber = i.ContainerNumber,
                ContainerSizeTypeName = i.ContainerSizeType?.ShortName ?? i.ContainerSizeTypeId.ToString(),
                StartDate = i.DetentionItemStartDate,
                EndDate = i.DetentionItemEndDate,
                TotalDays = i.DetentionItemTotalDays,
                FreeDays = i.DetentionItemFreeDays,
                LateDays = i.DetentionItemLateDays,
                Amount = i.DetentionItemAmount
            }).ToList();

            // ۵. تبدیل به DataTable برای رجیستر در TableDataSource
            var dtMaster = new DataTable("Master");
            dtMaster.Columns.Add("BLNo", typeof(string));
            dtMaster.Columns.Add("BookingNo", typeof(string));
            dtMaster.Columns.Add("CompanyName", typeof(string));
            dtMaster.Columns.Add("IssuerPartyName", typeof(string));
            dtMaster.Columns.Add("IssuedToPartyName", typeof(string));
            dtMaster.Columns.Add("TariffBookName", typeof(string));
            dtMaster.Columns.Add("TradeTypeName", typeof(string));
            dtMaster.Columns.Add("DetentionToDate", typeof(DateTime));
            dtMaster.Columns.Add("GrossAmount", typeof(decimal));
            dtMaster.Columns.Add("VATAmount", typeof(decimal));
            dtMaster.Columns.Add("Amount", typeof(decimal));
            dtMaster.Columns.Add("CurrencyCode", typeof(string));
            dtMaster.Columns.Add("Remarks", typeof(string));
            dtMaster.Columns.Add("LogoData", typeof(byte[]));
            dtMaster.Columns.Add("PrintDate", typeof(DateTime));

            dtMaster.Rows.Add(
                masterDto.BLNo,
                masterDto.BookingNo,
                masterDto.CompanyName,
                masterDto.IssuerPartyName,
                masterDto.IssuedToPartyName,
                masterDto.TariffBookName,
                masterDto.TradeTypeName,
                masterDto.DetentionToDate,
                masterDto.GrossAmount,
                masterDto.VATAmount,
                masterDto.Amount,
                masterDto.CurrencyCode,
                masterDto.Remarks ?? "",
                masterDto.LogoData,
                masterDto.PrintDate
            );

            var dtItems = new DataTable("Items");
            dtItems.Columns.Add("ContainerNumber", typeof(string));
            dtItems.Columns.Add("ContainerSizeTypeName", typeof(string));
            dtItems.Columns.Add("StartDate", typeof(DateTime));
            dtItems.Columns.Add("EndDate", typeof(DateTime));
            dtItems.Columns.Add("TotalDays", typeof(int));
            dtItems.Columns.Add("FreeDays", typeof(int));
            dtItems.Columns.Add("LateDays", typeof(int));
            dtItems.Columns.Add("Amount", typeof(decimal));

            foreach (var itm in itemsDtoList)
            {
                dtItems.Rows.Add(
                    itm.ContainerNumber,
                    itm.ContainerSizeTypeName,
                    itm.StartDate,
                    itm.EndDate,
                    itm.TotalDays,
                    itm.FreeDays,
                    itm.LateDays,
                    itm.Amount
                );
            }

            var ds = new DataSet("DetentionData");
            ds.Tables.Add(dtMaster);
            ds.Tables.Add(dtItems);

            // ۶. بارگذاری گزارش FastReport و Export به PDF
            var reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Detentions", "DetentionReport_EN.frx");

            using var report = new Report();
            report.Load(reportPath);
            report.RegisterData(ds, "DetentionData");

            report.Prepare();

            using var memoryStream = new MemoryStream();
            var pdfExport = new PDFSimpleExport(); // یا PDFExport
            report.Export(pdfExport, memoryStream);

            return memoryStream.ToArray();
        }




        public async Task<byte[]> GenerateDetentionReportPdfFarsiAsync(Guid detentionId, CancellationToken cancellationToken = default)
        {
            // ۱. خواندن اطلاعات کامل از دیتابیس
            var detention = await _context.Detentions
                .AsNoTracking()
                .Include(d => d.Company)
                .Include(d => d.Party)
                .Include(d => d.BL)
                .Include(d => d.Booking)
                .Include(d => d.TariffBook)
                .Include(d => d.TradeType)
                .Include(d => d.DetentionStatus)
                .Include(d => d.MainCurrency)
                .Include(d => d.PaymentCurrency)
                .Include(d => d.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.ContainerSizeType)
                .FirstOrDefaultAsync(d => d.Id == detentionId && !d.IsDeleted, cancellationToken);

            if (detention == null)
                throw new KeyNotFoundException($"Detention record with Id '{detentionId}' not found.");

            //// ۲. تعیین مسیر لوگوی شرکت
            //var logoFileName = $"logo-{detention.CompanyId}.png";
            //var fullLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", logoFileName);
            //if (!File.Exists(fullLogoPath))
            //{
            //    // در صورت نبود لوگوی اختصاصی، مسیر پیش‌فرض یا خالی
            //    fullLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", "default-logo.png");
            //    if (!File.Exists(fullLogoPath)) fullLogoPath = string.Empty;
            //}

            // ۳. نگاشت مستر
            var masterDto = new DetentionReportMasterDto
            {
                DetentionId = detention.Id,
                BLNo = detention.BL?.BLNo ?? "-",
                BookingNo = detention.Booking?.BookingNo ?? "-",
                CompanyName = detention.Company?.Person?.LastName ?? string.Empty,
                IssuerPartyName = "-",
                IssuedToPartyName = detention.Party?.PersonName ?? detention.Party?.PersonName ?? "-",
                TariffBookName = detention.TariffBook?.Name ?? "-",
                TradeTypeName = detention.TradeType?.TradeTypeName ?? "-",
                StatusName = detention.DetentionStatus?.DetentionStatusName ?? "-",
                DetentionToDate = detention.DetentionToDate,
                GrossAmount = detention.GrossAmount,
                VATAmount = detention.VATAmount,
                Amount = detention.Amount,
                CurrencyCode = detention.MainCurrency?.CurrencyCode ?? "USD",
                Remarks = detention.Remarks,
                PrintDate = DateTime.Now,
                PreviousPaid = detention.PreviousPaidAmount,
                FinalPayable = detention.FinalPayableAmount,
                FinalCurrencyCode = detention.PaymentCurrency.CurrencyCode,
                DetentionNo = detention.DetentionNo,
            };

            var logoPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", $"logo-{detention.CompanyId}.png");
            if (File.Exists(logoPhysicalPath))
            {
                masterDto.LogoData = await File.ReadAllBytesAsync(logoPhysicalPath);
            }


            // ۴. نگاشت اقلام کانتینرها
            var itemsDtoList = detention.Items.Select(i => new DetentionReportItemDto
            {
                ContainerNumber = i.ContainerNumber,
                ContainerSizeTypeName = i.ContainerSizeType?.ShortName ?? i.ContainerSizeTypeId.ToString(),
                StartDate = i.DetentionItemStartDate,
                EndDate = i.DetentionItemEndDate,
                TotalDays = i.DetentionItemTotalDays,
                FreeDays = i.DetentionItemFreeDays,
                LateDays = i.DetentionItemLateDays,
                Amount = i.DetentionItemAmount,
                ReturnDate = i.DetentionItemReturnDate
            }).ToList();

            // ۵. تبدیل به DataTable برای رجیستر در TableDataSource
            var dtMaster = new DataTable("Master");
            dtMaster.Columns.Add("BLNo", typeof(string));
            dtMaster.Columns.Add("BookingNo", typeof(string));
            dtMaster.Columns.Add("CompanyName", typeof(string));
            dtMaster.Columns.Add("IssuerPartyName", typeof(string));
            dtMaster.Columns.Add("IssuedToPartyName", typeof(string));
            dtMaster.Columns.Add("TariffBookName", typeof(string));
            dtMaster.Columns.Add("TradeTypeName", typeof(string));
            dtMaster.Columns.Add("DetentionToDate", typeof(DateTime));
            dtMaster.Columns.Add("GrossAmount", typeof(decimal));
            dtMaster.Columns.Add("VATAmount", typeof(decimal));
            dtMaster.Columns.Add("Amount", typeof(decimal));
            dtMaster.Columns.Add("CurrencyCode", typeof(string));
            dtMaster.Columns.Add("Remarks", typeof(string));
            dtMaster.Columns.Add("LogoData", typeof(byte[]));
            dtMaster.Columns.Add("PrintDate", typeof(DateTime));
            dtMaster.Columns.Add("PreviousPaid", typeof(decimal));
            dtMaster.Columns.Add("FinalPayable", typeof(decimal));
            dtMaster.Columns.Add("FinalCurrencyCode", typeof(string));
            dtMaster.Columns.Add("DetentionNo", typeof(long));


            dtMaster.Rows.Add(
                masterDto.BLNo,
                masterDto.BookingNo,
                masterDto.CompanyName,
                masterDto.IssuerPartyName,
                masterDto.IssuedToPartyName,
                masterDto.TariffBookName,
                masterDto.TradeTypeName,
                masterDto.DetentionToDate,
                masterDto.GrossAmount,
                masterDto.VATAmount,
                masterDto.Amount,
                masterDto.CurrencyCode,
                masterDto.Remarks ?? "",
                masterDto.LogoData,
                masterDto.PrintDate,
                masterDto.PreviousPaid,
                masterDto.FinalPayable,
                masterDto.FinalCurrencyCode,
                masterDto.DetentionNo
            );

            var dtItems = new DataTable("Items");
            dtItems.Columns.Add("ContainerNumber", typeof(string));
            dtItems.Columns.Add("ContainerSizeTypeName", typeof(string));
            dtItems.Columns.Add("StartDate", typeof(DateTime));
            dtItems.Columns.Add("EndDate", typeof(DateTime));
            dtItems.Columns.Add("TotalDays", typeof(int));
            dtItems.Columns.Add("FreeDays", typeof(int));
            dtItems.Columns.Add("LateDays", typeof(int));
            dtItems.Columns.Add("Amount", typeof(decimal));
            dtItems.Columns.Add("ReturnDate", typeof(DateTime));

            foreach (var itm in itemsDtoList)
            {
                dtItems.Rows.Add(
                    itm.ContainerNumber,
                    itm.ContainerSizeTypeName,
                    itm.StartDate,
                    itm.EndDate,
                    itm.TotalDays,
                    itm.FreeDays,
                    itm.LateDays,
                    itm.Amount,
                    itm.ReturnDate
                );
            }
            var ds = new DataSet("DetentionData");
            ds.Tables.Add(dtMaster);
            ds.Tables.Add(dtItems);

            // ۶. بارگذاری گزارش FastReport و Export به PDF
            var reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Detentions", "DetentionReport_FA.frx");

            using var report = new Report();
            report.Load(reportPath);
            report.RegisterData(ds, "DetentionData");

            report.Prepare();

            using var memoryStream = new MemoryStream();
            var pdfExport = new PDFSimpleExport(); // یا PDFExport
            report.Export(pdfExport, memoryStream);
            return memoryStream.ToArray();
        }




        public async Task<byte[]> GenerateDetentionStatementPdfAsync(Guid detentionId, CancellationToken cancellationToken = default)
        {
            // ۱. واکشی موجودیت ۳ سطحی
            var detention = await _context.Detentions
                .Include(d => d.Company).ThenInclude(c => c.Person)
                .Include(d => d.BL)
                .Include(d => d.Booking)
                .Include(d => d.Party)
                .Include(d => d.TariffBook)
                .Include(d => d.TradeType)
                .Include(d => d.DetentionStatus)
                .Include(d => d.PaymentCurrency)
                .Include(d => d.Items)
                    .ThenInclude(i => i.ContainerSizeType)
                .Include(d => d.Items)
                    .ThenInclude(i => i.Details)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == detentionId, cancellationToken);

            if (detention == null)
                throw new KeyNotFoundException($"Detention with ID '{detentionId}' not found.");

            // ۲. آماده‌سازی لوگو
            var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", $"logo-{detention.CompanyId}.png");
            if (!File.Exists(logoPath))
            {
                logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "logos", "default-logo.png");
            }

            byte[]? logoBytes = null;
            if (File.Exists(logoPath))
            {
                logoBytes = await File.ReadAllBytesAsync(logoPath, cancellationToken);
            }

            // ۳. ساخت دیتاست و جداول برای FastReport
            var dataSet = new DataSet("DetentionDS");

            // Master Table
            var dtMaster = new DataTable("Master");
            dtMaster.Columns.Add("DetentionId", typeof(Guid));
            dtMaster.Columns.Add("BLNo", typeof(string));
            dtMaster.Columns.Add("BookingNo", typeof(string));
            dtMaster.Columns.Add("CompanyName", typeof(string));
            dtMaster.Columns.Add("IssuerPartyName", typeof(string));
            dtMaster.Columns.Add("IssuedToPartyName", typeof(string));
            dtMaster.Columns.Add("TariffBookName", typeof(string));
            dtMaster.Columns.Add("TradeTypeName", typeof(string));
            dtMaster.Columns.Add("StatusName", typeof(string));
            dtMaster.Columns.Add("DetentionToDate", typeof(DateTime));
            dtMaster.Columns.Add("GrossAmount", typeof(decimal));
            dtMaster.Columns.Add("VATAmount", typeof(decimal));
            dtMaster.Columns.Add("Amount", typeof(decimal));
            dtMaster.Columns.Add("CurrencyCode", typeof(string));
            dtMaster.Columns.Add("Remarks", typeof(string));
            dtMaster.Columns.Add("LogoData", typeof(byte[]));
            dtMaster.Columns.Add("PrintDate", typeof(DateTime));

            var currencyCode = detention.PaymentCurrency?.CurrencyCode ?? "USD";
            var companyName = detention.Company?.Person != null
                ? $"{detention.Company.Person.FirstName} {detention.Company.Person.LastName}".Trim()
                : "Shipping Agency";

            dtMaster.Rows.Add(
                detention.Id,
                detention.BL?.BLNo ?? "-",
                detention.Booking?.BookingNo ?? "-",
                companyName,
                "-",
                detention.Party?.PersonName ?? "-",
                detention.TariffBook?.Name ?? "-",
                detention.TradeType?.TradeTypeName ?? "-",
                detention.DetentionStatus?.DetentionStatusName ?? "Active",
                detention.DetentionToDate,
                detention.GrossAmount,
                detention.VATAmount,
                detention.Amount,
                currencyCode,
                detention.Remarks ?? string.Empty,
                (object?)logoBytes ?? DBNull.Value,
                DateTime.Now
            );

            // Items Table
            var dtItems = new DataTable("Items");
            dtItems.Columns.Add("ItemId", typeof(Guid));
            dtItems.Columns.Add("DetentionId", typeof(Guid));
            dtItems.Columns.Add("ContainerNumber", typeof(string));
            dtItems.Columns.Add("ContainerSizeType", typeof(string));
            dtItems.Columns.Add("StartDate", typeof(DateTime));
            dtItems.Columns.Add("EndDate", typeof(DateTime));
            dtItems.Columns.Add("TotalDays", typeof(int));
            dtItems.Columns.Add("FreeDays", typeof(int));
            dtItems.Columns.Add("LateDays", typeof(int));
            dtItems.Columns.Add("Amount", typeof(decimal));

            // Details (Tiers) Table
            var dtDetails = new DataTable("Details");
            dtDetails.Columns.Add("DetailId", typeof(Guid));
            dtDetails.Columns.Add("ItemId", typeof(Guid));
            dtDetails.Columns.Add("FromDate", typeof(DateTime));
            dtDetails.Columns.Add("ToDate", typeof(DateTime));
            dtDetails.Columns.Add("TotalDays", typeof(int));
            dtDetails.Columns.Add("RatePerDay", typeof(decimal));
            dtDetails.Columns.Add("Amount", typeof(decimal));

            foreach (var item in detention.Items)
            {
                dtItems.Rows.Add(
                    item.Id,
                    item.DetentionId,
                    item.ContainerNumber,
                    item.ContainerSizeType?.ShortName ?? "-",
                    item.DetentionItemStartDate,
                    item.DetentionItemEndDate,
                    item.DetentionItemTotalDays,
                    item.DetentionItemFreeDays,
                    item.DetentionItemLateDays,
                    item.DetentionItemAmount
                );

                if (item.Details != null)
                {
                    foreach (var detail in item.Details.OrderBy(x => x.DetentionItemDetailFromDate))
                    {
                        dtDetails.Rows.Add(
                            detail.Id,
                            detail.DetentionItemId,
                            detail.DetentionItemDetailFromDate,
                            detail.DetentionItemDetailToDate,
                            detail.DetentionItemDetailTotalDays,
                            detail.DetentionItemDetailRatePerDay,
                            detail.DetentionItemDetailAmount
                        );
                    }
                }
            }

            dataSet.Tables.Add(dtMaster);
            dataSet.Tables.Add(dtItems);
            dataSet.Tables.Add(dtDetails);

            // تعریف Relation بین Master -> Items و Items -> Details در DataSet
            var relMasterItems = new DataRelation("Master_Items", dtMaster.Columns["DetentionId"]!, dtItems.Columns["DetentionId"]!);
            var relItemDetails = new DataRelation("Item_Details", dtItems.Columns["ItemId"]!, dtDetails.Columns["ItemId"]!);
            dataSet.Relations.Add(relMasterItems);
            dataSet.Relations.Add(relItemDetails);

            // ۴. بارگذاری فایل FRX و رندر گزارش
            var reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Detentions", "DetentionStatement_EN.frx");
            if (!File.Exists(reportPath))
            {
                throw new FileNotFoundException($"FastReport template not found at {reportPath}");
            }

            using var report = new Report();
            report.Load(reportPath);
            report.RegisterData(dataSet, "DetentionDS");

            // فعال‌سازی تمام TableDataSourceها
            foreach (var c in report.Dictionary.AllObjects)
            {
                if (c is FastReport.Data.TableDataSource tds)
                {
                    tds.Enabled = true;
                }
            }

            report.Prepare();

            // ۵. خروجی PDF
            using var stream = new MemoryStream();
            var pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, stream);
            return stream.ToArray();
        }
        public async Task<bool> ValidateTariffRulesAsync(Guid blId, Guid tariffBookId)
        {
            var blContainerTypes = await _context.BLContainers
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .Select(x => x.ContainerSizeTypeId)
                .Distinct()
                .ToListAsync();

            var tariffRuleTypes = await _context.TariffRules
                .Where(x => x.TariffBookId == tariffBookId && !x.IsDeleted)
                .Select(x => x.ContainerSizeTypeId)
                .Distinct()
                .ToListAsync();

            var missingTypes = blContainerTypes.Except(tariffRuleTypes).ToList();

            if (missingTypes.Any())
            {
                return false;
            }
            return true;
        }

        public async Task<List<string>> GetMissingContainerTypesAsync(Guid blId, Guid tariffBookId)
        {
            var blContainerTypes = await _context.BLContainers
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .Select(x => x.ContainerSizeTypeId)
                .Distinct()
                .ToListAsync();

            var tariffRuleTypes = await _context.TariffRules
                .Where(x => x.TariffBookId == tariffBookId && !x.IsDeleted)
                .Select(x => x.ContainerSizeTypeId)
                .ToListAsync();


            var missingIds = blContainerTypes.Except(tariffRuleTypes).ToList();

            if (!missingIds.Any())
                return new List<string>();

            return await _context.ContainerSizeTypes
                .Where(x => missingIds.Contains(x.Id))
                .Select(x => x.ShortName)
                .ToListAsync();
        }

        public async Task<DetentionListItemVm> GetDetentionAsync(Guid id)
        {
            return await _context.Detentions
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new DetentionListItemVm
                {
                    Id = x.Id,
                    BLNo = x.BL != null ? x.BL.BLNo : "-",
                    BookingNo = x.Booking != null ? x.Booking.BookingNo : "-",
                    StatusName = x.DetentionStatus != null ? x.DetentionStatus.DetentionStatusName : "-",
                    PartyName = x.Party != null ? x.Party.PersonName : "-",
                    Amount = x.Amount,
                    Currency = x.MainCurrency != null ? x.MainCurrency.CurrencyCode : "-",
                    DetentionToDate = x.DetentionToDate,
                    IsFinal = x.IsFinal,
                    CompanyName = x.Company != null && x.Company.Person != null ? x.Company.Person.PersonName : "-",
                    StatusId = x.DetentionStatusId,
                    FinanceDirectionName = x.FinanceDirection != null ? x.FinanceDirection.InvoiceContextName : "-",
                    FinalAmount = x.FinalPayableAmount,
                    FinalCurrency = x.PaymentCurrency != null ? x.PaymentCurrency.CurrencyCode : "-",
                    RemainAmount = x.RemainingAmount,
                    SettledAmount = x.SettledAmount,
                    VATAmount = x.VATAmount,
                    ExchangeRateValue = x.ExchangeRateValue
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> ValidateToDeleteAsync(Guid id)
        {
            var detention = await GetDetentionAsync(id);
            if (detention == null) return false;
            var hasnext = await _context.Detentions.Where(x => x.IsDeleted == false && x.PrevDetentionId == id).AnyAsync();
            if (detention.SettledAmount > 0 || detention.StatusId != 1 || hasnext)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var detention = await _context.Detentions.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (detention == null) return false;
            detention.DetentionStatusId = -1;
            detention.IsDeleted = true;
            detention.DeleteUserId = _currentUserService.UserId;
            detention.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateTariffRulesForBookingAsync(Guid bookingId, Guid tariffBookId)
        {
            var bookingContainerTypes = await _context.ContainerActions.Include(x => x.Container)
                .Where(x => x.BookingId == bookingId && !x.IsDeleted)
                .Select(x => x.Container.ContainerSizeTypeId)
                .Distinct()
                .ToListAsync();

            var tariffRuleTypes = await _context.TariffRules
                .Where(x => x.TariffBookId == tariffBookId && !x.IsDeleted)
                .Select(x => x.ContainerSizeTypeId.Value)
                .Distinct()
                .ToListAsync();

            var missingTypes = bookingContainerTypes.Except(tariffRuleTypes).ToList();

            if (missingTypes.Any())
            {
                return false;
            }
            return true;
        }
        public async Task<List<string>> GetMissingContainerTypesForBookingAsync(Guid bookingId, Guid tariffBookId)
        {
            var bookingContainerTypes = await _context.ContainerActions.Include(x => x.Container)
                .Where(x => x.BookingId == bookingId && !x.IsDeleted)
                .Select(x => x.Container.ContainerSizeTypeId)
                .Distinct()
                .ToListAsync();

            var tariffRuleTypes = await _context.TariffRules
                .Where(x => x.TariffBookId == tariffBookId && !x.IsDeleted)
                .Select(x => x.ContainerSizeTypeId.Value)
                .ToListAsync();


            var missingIds = bookingContainerTypes.Except(tariffRuleTypes).ToList();

            if (!missingIds.Any())
                return new List<string>();

            return await _context.ContainerSizeTypes
                .Where(x => missingIds.Contains(x.Id))
                .Select(x => x.ShortName)
                .ToListAsync();
        }
    }
}
