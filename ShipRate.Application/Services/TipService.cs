using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using System.Data;

namespace ShipRate.Application.Services
{
    public class TipService : ITipService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TipService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<PagedResultDto<TipListDto>> GetPagedTipsAsync(TipFilterDto filter)
        {
            var query = _context.TIPs
                .Include(t => t.VoyagePort)
                    .ThenInclude(vp => vp.InboundVoyage)
                        .ThenInclude(v => v.Vessel)
                .Include(t => t.VoyagePort)
                    .ThenInclude(vp => vp.Port)
                .Include(t => t.TIPStatus)
                .Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.TIPNo))
            {
                query = query.Where(t => t.TIPNo.Contains(filter.TIPNo));
            }

            if (!string.IsNullOrWhiteSpace(filter.VoyageNo))
            {
                query = query.Where(t => t.VoyagePort.InboundVoyage.VoyageNo.Contains(filter.VoyageNo));
            }

            if (!string.IsNullOrWhiteSpace(filter.VesselName))
            {
                query = query.Where(t => t.VoyagePort.InboundVoyage.Vessel.VesselName.Contains(filter.VesselName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Port))
            {
                query = query.Where(t => t.VoyagePort.Port.LocationName.Contains(filter.Port)
                                      || t.VoyagePort.Port.LocationCode.Contains(filter.Port));
            }

            var totalCount = await query.CountAsync();

            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TipListDto
                {
                    Id = t.Id,
                    TIPNo = t.TIPNo,
                    TIPDate = t.TIPDate,
                    TIPExpiryDate = t.TIPExpiryDate,
                    VoyageNo = t.VoyagePort.InboundVoyage.VoyageNo,
                    VesselName = t.VoyagePort.InboundVoyage.Vessel.VesselName,
                    PortName = t.VoyagePort.Port.LocationName,
                    TIPStatusName = t.TIPStatus.TIPStatusName
                })
                .ToListAsync();

            return new PagedResultDto<TipListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<TipEditDto> GetTipForEditAsync(Guid id)
        {
            var tip = await _context.TIPs
                .Include(t => t.VoyagePort)
                    .ThenInclude(vp => vp.InboundVoyage)
                        .ThenInclude(v => v.Vessel)
                .Include(t => t.VoyagePort)
                    .ThenInclude(vp => vp.Port)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (tip == null) return null;

            var dto = new TipEditDto
            {
                Id = tip.Id,
                CompanyId = tip.CompanyId,
                ShippingLineId = tip.ShippingLineId,
                VoyagePortId = tip.VoyagePortId,
                TIPNo = tip.TIPNo,
                TIPDate = tip.TIPDate,
                TIPExpiryDate = tip.TIPExpiryDate,
                CustomsOfficeId = tip.CustomsOfficeId,
                TIPStatusId = tip.TIPStatusId,
                //IsActive = tip.IsActive,
                Remarks = tip.Remarks,

                VoyageDisplayText = tip.VoyagePort != null
                    ? $"{tip.VoyagePort.InboundVoyage.VoyageNo} - {tip.VoyagePort.InboundVoyage.Vessel.VesselName} - {tip.VoyagePort.Port.LocationName}"
                    : string.Empty
            };
            return dto;
        }
        public async Task<Guid> CreateTipAsync(TipEditDto dto, Guid currentUserId)
        {
            var tip = new TIP
            {
                Id = Guid.CreateVersion7(),
                CompanyId = dto.CompanyId,
                ShippingLineId = dto.ShippingLineId,
                VoyagePortId = dto.VoyagePortId,
                TIPNo = dto.TIPNo,
                TIPDate = dto.TIPDate,
                TIPExpiryDate = dto.TIPExpiryDate,
                CustomsOfficeId = dto.CustomsOfficeId,
                TIPStatusId = 1,
                //IsActive = true,
                IsDeleted = false,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };
            _context.TIPs.Add(tip);
            await _context.SaveChangesAsync();

            return tip.Id;
        }

        public async Task UpdateTipAsync(TipEditDto dto, Guid currentUserId)
        {
            var tip = await _context.TIPs.FirstOrDefaultAsync(t => t.Id == dto.Id && !t.IsDeleted);
            if (tip == null)
                throw new Exception("TIP not found");

            tip.CompanyId = dto.CompanyId;
            tip.ShippingLineId = dto.ShippingLineId;
            tip.VoyagePortId = dto.VoyagePortId;
            tip.TIPNo = dto.TIPNo;
            tip.TIPDate = dto.TIPDate;
            tip.TIPExpiryDate = dto.TIPExpiryDate;
            tip.CustomsOfficeId = dto.CustomsOfficeId;
            //tip.IsActive = dto.IsActive;
            tip.Remarks = dto.Remarks;
            tip.UpdatedAt = DateTime.Now;
            tip.EditUserId = currentUserId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTipAsync(Guid id, Guid currentUserId)
        {
            var tip = await _context.TIPs.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (tip == null)
                return;

            tip.IsDeleted = true;
            //tip.IsActive = false;
            tip.UpdatedAt = DateTime.Now;
            tip.EditUserId = currentUserId;

            await _context.SaveChangesAsync();
        }

        public async Task<List<TipUsageListDto>> GetTipUsagesByTipAsync(Guid tipId)
        {
            var query = _context.TIPUsages
                .Include(u => u.Customer)
                .Where(u => u.TIPId == tipId && !u.IsDeleted);

            var list = await query
                .OrderBy(u => u.LetterDate)
                .Select(u => new TipUsageListDto
                {
                    Id = u.Id,
                    TIPId = u.TIPId,
                    BLNo = u.BLNo,
                    CustomerName = u.Customer.PersonName,
                    LetterNo = u.LetterNo,
                    LetterDate = u.LetterDate,
                    F20 = u.F20,
                    F40 = u.F40,
                })
                .ToListAsync();




            return list;
        }

        public async Task<List<TipUsageContainerListDto>> GetContainersByTipUsageAsync(Guid tipUsageId)
        {
            var query = _context.TIPUsageContainers
                .Include(c => c.SizeType)
                .Where(c => c.TIPUsageId == tipUsageId && !c.IsDeleted);

            var list = await query
                .OrderBy(c => c.ContainerNumber)
                .Select(c => new TipUsageContainerListDto
                {
                    Id = c.Id,
                    TIPUsageId = c.TIPUsageId,
                    ContainerNumber = c.ContainerNumber,
                    SizeTypeShortName = c.SizeType.ShortName
                })
                .ToListAsync();

            return list;
        }

        public async Task<PagedResultDto<VoyagePortLookupDto>> LookupVoyagePortsAsync(VoyagePortLookupFilterDto filter)
        {
            var query = _context.VoyagePorts
                .Include(vp => vp.InboundVoyage)
                    .ThenInclude(v => v.Vessel)
                .Include(vp => vp.Port)
                .Where(vp => !vp.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.VoyageNo))
            {
                query = query.Where(vp => vp.InboundVoyage.VoyageNo.Contains(filter.VoyageNo));
            }

            if (!string.IsNullOrWhiteSpace(filter.VesselName))
            {
                query = query.Where(vp => vp.InboundVoyage.Vessel.VesselName.Contains(filter.VesselName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Port))
            {
                query = query.Where(vp => vp.Port.LocationName.Contains(filter.Port)
                                       || vp.Port.LocationCode.Contains(filter.Port));
            }

            var totalCount = await query.CountAsync();

            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var items = await query
                .OrderBy(vp => vp.InboundVoyage.VoyageNo)
                .ThenBy(vp => vp.Port.LocationName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(vp => new VoyagePortLookupDto
                {
                    VoyagePortId = vp.Id,
                    VoyageNo = vp.InboundVoyage.VoyageNo,
                    VesselName = vp.InboundVoyage.Vessel.VesselName,
                    PortName = vp.Port.LocationName
                })
                .ToListAsync();

            return new PagedResultDto<VoyagePortLookupDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<VoyagePortLookupDto> GetVoyagePortByIdAsync(Guid voyagePortId)
        {
            var vp = await _context.VoyagePorts
                .Include(x => x.InboundVoyage)
                    .ThenInclude(v => v.Vessel)
                .Include(x => x.Port)
                .Where(x => x.Id == voyagePortId && !x.IsDeleted)
                .Select(x => new VoyagePortLookupDto
                {
                    VoyagePortId = x.Id,
                    VoyageNo = x.InboundVoyage.VoyageNo,
                    VesselName = x.InboundVoyage.Vessel.VesselName,
                    PortName = x.Port.LocationName
                })
                .FirstOrDefaultAsync();

            return vp;
        }

        public async Task<int> GetNewSerialNoAsync(int companyId, CancellationToken cancellationToken = default)
        {
            var maxSerial = await _context.TIPUsages
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .MaxAsync(x => (int?)x.Serial, cancellationToken);
            return (maxSerial ?? 0) + 1;
        }

        public async Task<byte[]> GenerateTipLetterPdfAsync(TipUsageListDto dto)
        {
            // ۱. ساخت DataTable والد و تک‌مقداری (مربوط به اطلاعات نامه)
            DataTable dtTip = new DataTable("TipUsage");
            dtTip.Columns.Add("BLNo", typeof(string));
            dtTip.Columns.Add("CustomerName", typeof(string));
            dtTip.Columns.Add("TIPNo", typeof(string));
            dtTip.Columns.Add("LetterNo", typeof(string));
            dtTip.Columns.Add("LetterLocalDate", typeof(string));
            dtTip.Columns.Add("VesselName", typeof(string));
            dtTip.Columns.Add("VoyageNo", typeof(string));
            dtTip.Columns.Add("CustomsOfficeName", typeof(string));
            dtTip.Columns.Add("F20PlusF40", typeof(int));

            dtTip.Rows.Add(
                dto.BLNo ?? "",
                dto.CustomerName ?? "",
                dto.TIPNo ?? "",
                dto.LetterNo ?? "",
                dto.LetterLocalDate ?? "",
                dto.VesselName ?? "",
                dto.VoyageNo ?? "",
                dto.CustomsOfficeName ?? "",
                dto.F20 + dto.F40
            );

            // ۲. ساخت DataTable فرزند (مربوط به لیست کانتینرها)
            DataTable dtContainers = new DataTable("Containers");
            dtContainers.Columns.Add("RowNo", typeof(int));
            dtContainers.Columns.Add("ContainerNumber", typeof(string));
            dtContainers.Columns.Add("SizeTypeShortName", typeof(string));

            if (dto.Containers != null && dto.Containers.Any())
            {
                int index = 1;
                foreach (var c in dto.Containers)
                {
                    dtContainers.Rows.Add(index++, c.ContainerNumber ?? "", c.SizeTypeShortName ?? "");
                }
            }

            // ۳. بارگذاری در FastReport و خروجی PDF
            using (Report report = new Report())
            {
                var path = Path.Combine(_env.ContentRootPath, "Reports", "DO", "TIPLetter.frx");
                report.Load(path);

                // ثبت TableDataSourceها در گزارش
                report.RegisterData(dtTip, "TipUsage");
                report.RegisterData(dtContainers, "Containers");

                // فعال‌سازی منابع داده در FastReport
                report.GetDataSource("TipUsage").Enabled = true;
                report.GetDataSource("Containers").Enabled = true;
                report.Prepare();

                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    report.Export(pdfExport, ms);
                    return ms.ToArray();
                }
            }
        }
        public async Task<TipUsageListDto> GetTipUsageByBlIdAsync(Guid blId)
        {
            var tipu = await _context.TIPUsages
                .Include(x => x.TIP)
                    .ThenInclude(x => x.CustomsOffice)
                .Include(x => x.TIP)
                    .ThenInclude(x => x.VoyagePort)
                        .ThenInclude(x => x.InboundVoyage)
                            .ThenInclude(x => x.Vessel)
                .Include(u => u.Customer)
                .Where(u => u.BLId == blId && !u.IsDeleted)
                .Select(u => new TipUsageListDto
                {
                    Id = u.Id,
                    TIPId = u.TIPId,
                    BLNo = u.BLNo,
                    CustomerName = u.Customer.PersonName,
                    LetterNo = u.LetterNo,
                    LetterDate = u.LetterDate,
                    F20 = u.F20,
                    F40 = u.F40,
                    TIPNo = u.TIP.TIPNo,
                    CustomsOfficeName = u.TIP.CustomsOffice.LocationLocalName,
                    VesselName = u.TIP.VoyagePort.InboundVoyage.VoyageNo,
                    VoyageNo = u.TIP.VoyagePort.InboundVoyage.VoyageNo,
                    LetterLocalDate = DateHelper.getSolarDate10(u.LetterDate)
                })
                .FirstOrDefaultAsync();
            tipu.Containers = await GetContainersByTipUsageAsync(tipu.Id);
            return tipu;
        }
    }
}
