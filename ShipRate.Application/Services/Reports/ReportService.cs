using Dapper;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShipRate.Application.DTOs.Reports;
using ShipRate.Application.Interfaces.Reports;
using ShipRate.Application.JobsViewModels;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Infrastructure.Data;
using System.Data;
using System.Drawing;

namespace ShipRate.Application.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;


        public ReportService(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        public async Task<ReportsIndexViewModel> GetAuthorizedReportsAsync(int companyId)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == companyId && x.IsActive && !x.IsDeleted);

            if (company == null)
            {
                return new ReportsIndexViewModel
                {
                    SelectedCompanyId = companyId,
                    SelectedCompanyName = null,
                    Categories = new List<ReportCategoryCardViewModel>()
                };
            }

            var companyRoleTypeIds = await _context.CompanyRoles
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .Select(x => x.CompanyRoleTypeId)
                .Distinct()
                .ToListAsync();

            if (!companyRoleTypeIds.Any())
            {
                return new ReportsIndexViewModel
                {
                    SelectedCompanyId = companyId,
                    SelectedCompanyName = $"Company #{companyId}",
                    Categories = new List<ReportCategoryCardViewModel>()
                };
            }

            var categories = await _context.ReportCategories
                .AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted)
                .Select(c => new ReportCategoryCardViewModel
                {
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    CategoryLocalName = c.CategoryLocalName,
                    Description = c.Description,
                    IconClass = c.IconClass,
                    ColorClass = c.ColorClass,
                    SortOrder = c.SortOrder,
                    Reports = c.Reports
                        .Where(r => r.IsActive && !r.IsDeleted &&
                                    r.ReportCompanyRoleTypes.Any(rr =>
                                        rr.IsActive && !rr.IsDeleted &&
                                        companyRoleTypeIds.Contains(rr.CompanyRoleTypeId)))
                        .OrderBy(r => r.SortOrder)
                        .Select(r => new ReportItemViewModel
                        {
                            Id = r.Id,
                            ReportKey = r.ReportKey,
                            ReportName = r.ReportName,
                            ReportLocalName = r.ReportLocalName,
                            Description = r.Description,
                            ControllerName = r.ControllerName,
                            ActionName = r.ActionName,
                            FilterViewName = r.FilterViewName,
                            ResultViewName = r.ResultViewName,
                            IconClass = r.IconClass,
                            CategoryId = r.CategoryId,
                            SortOrder = r.SortOrder
                        })
                        .ToList()
                })
                .Where(c => c.Reports.Any())
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return new ReportsIndexViewModel
            {
                SelectedCompanyId = companyId,
                SelectedCompanyName = $"Company #{companyId}",
                Categories = categories
            };
        }
        public async Task<ReportsIndexViewModel> GetAllReportsAsync()
        {
            var categories = await _context.ReportCategories
                .Include(x => x.Reports)
                .AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted)
                .Select(c => new ReportCategoryCardViewModel
                {
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    CategoryLocalName = c.CategoryLocalName,
                    Description = c.Description,
                    IconClass = c.IconClass,
                    ColorClass = c.ColorClass,
                    SortOrder = c.SortOrder,
                    Reports = c.Reports
                        .Where(r => r.IsActive && !r.IsDeleted)
                        .OrderBy(r => r.SortOrder)
                        .Select(r => new ReportItemViewModel
                        {
                            Id = r.Id,
                            ReportKey = r.ReportKey,
                            ReportName = r.ReportName,
                            ReportLocalName = r.ReportLocalName,
                            Description = r.Description,
                            ControllerName = r.ControllerName,
                            ActionName = r.ActionName,
                            FilterViewName = r.FilterViewName,
                            ResultViewName = r.ResultViewName,
                            IconClass = r.IconClass,
                            CategoryId = r.CategoryId,
                            SortOrder = r.SortOrder
                        })
                        .ToList()
                })
                .Where(c => c.Reports.Any())
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return new ReportsIndexViewModel
            {
                Categories = categories
            };
        }

        public async Task<List<Rpt_ContainerDto>> GetContainersAsync()
        {
            var connectionString = _configuration.GetConnectionString("ShipRateConnection");
            //"Data Source=.\\Sql2025;Initial Catalog=ShipRate;Trusted_Connection=True;TrustServerCertificate=True"
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM vwContainers";
            var result = await connection.QueryAsync<Rpt_ContainerDto>(sql);
            return result.ToList();
        }

        public async Task<RateRequestReportDto?> GetRateRequestReportAsync(Guid? rateRequestId)
        {
            var connectionString = _configuration.GetConnectionString("ShipRateConnection");
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@RateRequestId", rateRequestId, DbType.Guid);

            using var multi = await connection.QueryMultipleAsync(
                "rpt.spRateRequestReport",
                parameters,
                commandType: CommandType.StoredProcedure);

            var header = await multi.ReadFirstOrDefaultAsync<RateRequestReportDto>();
            if (header == null)
                return null;

            header.PackageTypes = (await multi.ReadAsync<RateRequestPackageTypeDto>()).ToList();
            header.ContainerTypes = (await multi.ReadAsync<RateRequestContainerTypeDto>()).ToList();
            return header;
        }

        public async Task<RateRequestReportDto?> GetRateRequestReportsAsync()
        {
            var connectionString = _configuration.GetConnectionString("ShipRateConnection");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@RateRequestId", Guid.Empty, DbType.Guid);

            using var multi = await connection.QueryMultipleAsync(
                "rpt.spRateRequestReport",
                parameters,
                commandType: CommandType.StoredProcedure);

            var header = await multi.ReadFirstOrDefaultAsync<RateRequestReportDto>();
            if (header == null)
                return null;

            header.PackageTypes = (await multi.ReadAsync<RateRequestPackageTypeDto>()).ToList();
            header.ContainerTypes = (await multi.ReadAsync<RateRequestContainerTypeDto>()).ToList();
            return header;
        }

        public async Task<RTSInstructionDetailsVm> GetRTSInstructionLetterAsync(Guid id)
        {
            var connectionString = _configuration.GetConnectionString("ShipRateConnection");
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Guid);

            using var multi = await connection.QueryMultipleAsync(
                "rpt.spRTSInstructionLetterReport",
                parameters,
                commandType: CommandType.StoredProcedure);

            var header = await multi.ReadFirstOrDefaultAsync<RTSInstructionDetailsVm>();
            if (header == null)
                return null;

            header.Containers = (await multi.ReadAsync<RTSInstructionDetailsContainerVm>()).ToList();
            return header;
        }



        //public async Task<byte[]> PrintRTSInstructionLetterPdfAsync(Guid id)
        //{
        //    var connectionString = _configuration.GetConnectionString("ShipRateConnection");

        //    await using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    using var multi = await connection.QueryMultipleAsync(
        //        "rpt.spRTSInstructionLetterReport",
        //        new { Id = id },
        //        commandType: CommandType.StoredProcedure);

        //    var headerRows = (await multi.ReadAsync()).ToList();
        //    var containerRows = (await multi.ReadAsync()).ToList();

        //    if (!headerRows.Any())
        //        return null;

        //    var dataSet = new DataSet("RTSData");

        //    var headerTable = new DataTable("Header");
        //    headerTable.Columns.Add("Id", typeof(Guid));
        //    headerTable.Columns.Add("RTSInstructionNo", typeof(string));
        //    headerTable.Columns.Add("RTSInstructionDate", typeof(DateTime));
        //    headerTable.Columns.Add("RTSInstructionLocalDate", typeof(string));
        //    headerTable.Columns.Add("CarrierName", typeof(string));
        //    headerTable.Columns.Add("CarrierLocalName", typeof(string));
        //    headerTable.Columns.Add("ShipperName", typeof(string));
        //    headerTable.Columns.Add("ShipperLocalName", typeof(string));

        //    foreach (var item in headerRows)
        //    {
        //        headerTable.Rows.Add(
        //            item.Id,
        //            item.RTSInstructionNo,
        //            item.RTSInstructionDate ?? (object)DBNull.Value,
        //            item.RTSInstructionLocalDate ?? string.Empty,
        //            item.CarrierName ?? string.Empty,
        //            item.CarrierLocalName ?? string.Empty,
        //            item.ShipperName ?? string.Empty,
        //            item.ShipperLocalName ?? string.Empty
        //        );
        //    }

        //    var containersTable = new DataTable("Containers");
        //    containersTable.Columns.Add("Id", typeof(Guid));
        //    containersTable.Columns.Add("RTSInstructionId", typeof(Guid));
        //    containersTable.Columns.Add("ContainerSizeTypeName", typeof(string));
        //    containersTable.Columns.Add("ContainerCounts", typeof(int));
        //    containersTable.Columns.Add("Remarks", typeof(string));

        //    foreach (var item in containerRows)
        //    {
        //        containersTable.Rows.Add(
        //            item.Id,
        //            item.RTSInstructionId,
        //            item.ContainerSizeTypeName ?? string.Empty,
        //            item.ContainerCounts,
        //            item.Remarks ?? string.Empty
        //        );
        //    }

        //    dataSet.Tables.Add(headerTable);
        //    dataSet.Tables.Add(containersTable);

        //    var reportPath = Path.Combine(_env.ContentRootPath, "Reports", "RTSInstructionLetter.frx");

        //    using var report = new Report();
        //    report.Load(reportPath);
        //    report.RegisterData(dataSet, "RTSData");

        //    report.GetDataSource("Header").Enabled = true;
        //    report.GetDataSource("Containers").Enabled = true;

        //    report.Prepare();

        //    using var pdfExport = new PDFSimpleExport();
        //    using var stream = new MemoryStream();

        //    report.Export(pdfExport, stream);
        //    return stream.ToArray();
        //}





        public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices
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
                InvoiceNo = invoice.InvoiceNo ?? "-",
                //InvoiceDatePersian = invoice.InvoiceDate?.() ?? "-", // متد کمکی خودتان
                InvoiceDatePersian = "1600",
                InvoiceModelName = invoice.InvoiceModel?.InvoiceModelName ?? "",
                CurrencyName = invoice.Currency?.CurrencyName ?? "",
                TotalFirst = invoice.FirstTotal,
                TotalDiscount = invoice.Discount,
                TotalTax = invoice.TaxAmount,
                FinalAmount = invoice.TotalAmount,
                Items = invoice.InvoiceItems.OrderBy(x => x.SequenceNumber).Select(x => new InvoiceItemPrintDto
                {
                    Row = x.SequenceNumber,
                    ChargeTitle = x.ChargeType?.ChargeTypeName + " " + x.ChargeTypeSupplementRemarks,
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

            // لود فایل FRX و تولید PDF
            using var report = new Report();
            string reportPath = Path.Combine(_env.ContentRootPath, "Reports", "OfficialInvoice.frx");
            report.Load(reportPath);

            // پاس دادن داده‌ها به گزارش
            report.RegisterData(new List<InvoicePrintDto> { dto }, "Header");
            report.RegisterData(dto.Items, "Items");



            var logoPath = Path.Combine(
                   _env.WebRootPath,
                   "logos",
                   "logo-1.png");

            var logoObject = report.FindObject("CompanyLogo") as PictureObject;

            if (logoObject is not null &&
                System.IO.File.Exists(logoPath))
            {
                using var sourceImage = Image.FromFile(logoPath);
                logoObject.Image = new Bitmap(sourceImage);
            }
            report.Prepare();
            using var ms = new MemoryStream();
            var pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, ms);
            return ms.ToArray();
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
    }
}
