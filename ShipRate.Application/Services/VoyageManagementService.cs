using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class VoyageManagementService : IVoyageManagementService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public VoyageManagementService(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _db = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<VoyageManagementIndexViewModel> GetIndexModelAsync()
        {
            var model = new VoyageManagementIndexViewModel();

            //model.Ports = await _db.Locations
            //    .AsNoTracking()
            //    .OrderBy(x => x.LocationName)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.LocationName
            //    })
            //    .ToListAsync();

            model.Vessels = await _db.Vessels
                .AsNoTracking()
                .OrderBy(x => x.VesselName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VesselName
                })
                .ToListAsync();

            model.ShippingLines = await _db.Carriers
                .Include(x => x.Person)
                .AsNoTracking()
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();

            model.EventTypes = await _db.EventTypes
                .AsNoTracking()
                .Where(x => x.Id == VoyageEventTypeIds.VesselEtaUpdated
                         || x.Id == VoyageEventTypeIds.ArrivalToPort
                         || x.Id == VoyageEventTypeIds.VesselBerthed
                         || x.Id == VoyageEventTypeIds.OperationsCommenced
                         || x.Id == VoyageEventTypeIds.OperationsCompleted
                         || x.Id == VoyageEventTypeIds.VoyageCompleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeName
                })
                .ToListAsync();

            return model;
        }

        public async Task<PagedResultDto<VoyageMonitoringRowDto>> SearchAsync(VoyageSearchRequestDto request)
        {
            var query = _db.VoyagePorts
                .AsNoTracking()
                .Select(x => new VoyageMonitoringRowDto
                {
                    VoyagePortId = x.Id,
                    InboundVoyageId = x.InboundVoyageId,
                    OutboundVoyageId = x.OutboundVoyageId,

                    VesselName = x.InboundVoyage != null && x.InboundVoyage.Vessel != null
                        ? x.InboundVoyage.Vessel.VesselName
                        : string.Empty,

                    ShippingLineName = x.InboundVoyage != null && x.InboundVoyage.ShippingLine != null
                        ? x.InboundVoyage.ShippingLine.Person.PersonName
                        : string.Empty,

                    VoyageNoIn = x.InboundVoyage != null
                        ? x.InboundVoyage.VoyageNo
                        : string.Empty,

                    VoyageNoOut = x.OutboundVoyage != null
                        ? x.OutboundVoyage.VoyageNo
                        : string.Empty,

                    DirectionCode = x.InboundVoyage != null
                        ? x.InboundVoyage.DirectionCode ?? string.Empty
                        : string.Empty,

                    PortName = x.Port != null ? x.Port.LocationName : string.Empty,
                    TerminalName = x.Terminal != null ? x.Terminal.LocationName : string.Empty,
                    VoyagePortSequence = x.VoyagePortSequence,

                    ETA = x.ETA,
                    ATA = x.ATA,

                    BerthingDate = x.VoyagePortEvents
                        .Where(e => e.EventTypeId == VoyageEventTypeIds.VesselBerthed)
                        .OrderByDescending(e => e.VoyagePortEventDateTime)
                        .Select(e => (DateTime?)e.VoyagePortEventDateTime)
                        .FirstOrDefault(),

                    OperationsCommencedDate = x.VoyagePortEvents
                        .Where(e => e.EventTypeId == VoyageEventTypeIds.OperationsCommenced)
                        .OrderByDescending(e => e.VoyagePortEventDateTime)
                        .Select(e => (DateTime?)e.VoyagePortEventDateTime)
                        .FirstOrDefault(),

                    OperationsCompletedDate = x.VoyagePortEvents
                        .Where(e => e.EventTypeId == VoyageEventTypeIds.OperationsCompleted)
                        .OrderByDescending(e => e.VoyagePortEventDateTime)
                        .Select(e => (DateTime?)e.VoyagePortEventDateTime)
                        .FirstOrDefault(),

                    VoyageCompletedDate = x.VoyagePortEvents
                        .Where(e => e.EventTypeId == VoyageEventTypeIds.VoyageCompleted)
                        .OrderByDescending(e => e.VoyagePortEventDateTime)
                        .Select(e => (DateTime?)e.VoyagePortEventDateTime)
                        .FirstOrDefault(),

                    LastEventName = x.VoyagePortEvents
                        .OrderByDescending(e => e.VoyagePortEventDateTime)
                        .Select(e => e.VoyageEventType != null ? e.VoyageEventType.EventTypeName : string.Empty)
                        .FirstOrDefault() ?? string.Empty,

                    LastEventDate = x.VoyagePortEvents
                        .OrderByDescending(e => e.VoyagePortEventDateTime)
                        .Select(e => (DateTime?)e.VoyagePortEventDateTime)
                        .FirstOrDefault(),

                    CurrentStage =
                        x.VoyagePortEvents.Any(e => e.EventTypeId == VoyageEventTypeIds.VoyageCompleted) ? "Voyage Completed" :
                        x.VoyagePortEvents.Any(e => e.EventTypeId == VoyageEventTypeIds.OperationsCompleted) ? "Operations Completed" :
                        x.VoyagePortEvents.Any(e => e.EventTypeId == VoyageEventTypeIds.OperationsCommenced) ? "Under Operations" :
                        x.VoyagePortEvents.Any(e => e.EventTypeId == VoyageEventTypeIds.VesselBerthed) ? "Berthed" :
                        x.ATA.HasValue ? "Arrived" :
                        x.ETA.HasValue ? "Expected" : "Planned"
                });

            if (!string.IsNullOrWhiteSpace(request.VoyageNo))
            {
                var voyageNo = request.VoyageNo.Trim();

                query = query.Where(x =>
                    x.VoyageNoIn.Contains(voyageNo) ||
                    x.VoyageNoOut.Contains(voyageNo));
            }

            if (request.PortId.HasValue)
            {
                query = query.Where(x => _db.VoyagePorts
                    .Where(vp => vp.Id == x.VoyagePortId)
                    .Select(vp => vp.PortId)
                    .FirstOrDefault() == request.PortId.Value);
            }

            if (request.VesselId.HasValue)
            {
                query = query.Where(x => _db.VoyagePorts
                    .Where(vp => vp.Id == x.VoyagePortId)
                    .Select(vp => vp.InboundVoyage != null ? vp.InboundVoyage.VesselId : Guid.Empty)
                    .FirstOrDefault() == request.VesselId.Value);
            }

            if (request.ShippingLineId.HasValue)
            {
                query = query.Where(x => _db.VoyagePorts
                    .Where(vp => vp.Id == x.VoyagePortId)
                    .Select(vp => vp.InboundVoyage != null ? vp.InboundVoyage.ShippingLineId : Guid.Empty)
                    .FirstOrDefault() == request.ShippingLineId.Value);
            }

            if (request.EtaFrom.HasValue)
            {
                query = query.Where(x => x.ETA >= request.EtaFrom.Value);
            }

            if (request.EtaTo.HasValue)
            {
                query = query.Where(x => x.ETA <= request.EtaTo.Value);
            }

            if (request.AtaFrom.HasValue)
            {
                query = query.Where(x => x.ATA >= request.AtaFrom.Value);
            }

            if (request.AtaTo.HasValue)
            {
                query = query.Where(x => x.ATA <= request.AtaTo.Value);
            }

            if (request.EventTypeId.HasValue || request.EventDateFrom.HasValue || request.EventDateTo.HasValue)
            {
                query = query.Where(x => _db.VoyagePortEvents.Any(e =>
                    e.VoyagePortId == x.VoyagePortId &&
                    (!request.EventTypeId.HasValue || e.EventTypeId == request.EventTypeId.Value) &&
                    (!request.EventDateFrom.HasValue || e.VoyagePortEventDateTime >= request.EventDateFrom.Value) &&
                    (!request.EventDateTo.HasValue || e.VoyagePortEventDateTime <= request.EventDateTo.Value)));
            }

            query = ApplySorting(query, request);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResultDto<VoyageMonitoringRowDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<VoyageMonitoringDetailsDto?> GetTimelineAsync(Guid voyagePortId)
        {
            return await _db.VoyagePorts
                .AsNoTracking()
                .Where(x => x.Id == voyagePortId)
                .Select(x => new VoyageMonitoringDetailsDto
                {
                    VoyagePortId = x.Id,
                    VesselName = x.InboundVoyage != null && x.InboundVoyage.Vessel != null
                        ? x.InboundVoyage.Vessel.VesselName
                        : string.Empty,

                    ShippingLineName = x.InboundVoyage != null && x.InboundVoyage.ShippingLine != null
                        ? x.InboundVoyage.ShippingLine.Person.PersonName
                        : string.Empty,

                    PortName = x.Port != null ? x.Port.LocationName : string.Empty,
                    TerminalName = x.Terminal != null ? x.Terminal.LocationName : string.Empty,

                    VoyageNoIn = x.InboundVoyage != null ? x.InboundVoyage.VoyageNo : string.Empty,
                    VoyageNoOut = x.OutboundVoyage != null ? x.OutboundVoyage.VoyageNo : string.Empty,

                    ETA = x.ETA,
                    ATA = x.ATA,
                    ETD = x.ETD,
                    ATD = x.ATD,

                    Events = x.VoyagePortEvents
                        .OrderBy(e => e.VoyagePortEventDateTime)
                        .Select(e => new VoyageEventItemDto
                        {
                            EventTypeId = e.EventTypeId,
                            EventTypeName = e.VoyageEventType != null ? e.VoyageEventType.EventTypeName : string.Empty,
                            EventDateTime = e.VoyagePortEventDateTime,
                            Remarks = e.VoyagePortEventRemarks
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task ExecuteActionAsync(VoyageActionRequestDto request)
        {
            if (request.VoyagePortId == Guid.Empty)
            {
                throw new ArgumentException("VoyagePortId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ActionCode))
            {
                throw new ArgumentException("ActionCode is required.");
            }
            var voyagePort = await _db.VoyagePorts
                .FirstOrDefaultAsync(x => x.Id == request.VoyagePortId);

            if (voyagePort == null)
            {
                throw new InvalidOperationException("Voyage port not found.");
            }

            switch (request.ActionCode.Trim().ToUpperInvariant())
            {
                case "REGISTER_ARRIVAL":
                    voyagePort.ATA = request.ActionDateTime ?? DateTime.Now;
                    _db.VoyagePortEvents.Add(new VoyagePortEvent
                    {
                        VoyagePortId = voyagePort.Id,
                        EventTypeId = VoyageEventTypeIds.ArrivalToPort,
                        VoyagePortEventDateTime = request.ActionDateTime ?? DateTime.Now,
                        VoyagePortEventRemarks = request.Remarks ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId

                    });
                    break;

                case "REGISTER_BERTHING":
                    _db.VoyagePortEvents.Add(new VoyagePortEvent
                    {
                        VoyagePortId = voyagePort.Id,
                        EventTypeId = VoyageEventTypeIds.VesselBerthed,
                        VoyagePortEventDateTime = request.ActionDateTime ?? DateTime.Now,
                        VoyagePortEventRemarks = request.Remarks ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    });
                    break;

                case "REGISTER_OPS_COMMENCED":
                    _db.VoyagePortEvents.Add(new VoyagePortEvent
                    {
                        VoyagePortId = voyagePort.Id,
                        EventTypeId = VoyageEventTypeIds.OperationsCommenced,
                        VoyagePortEventDateTime = request.ActionDateTime ?? DateTime.Now,
                        VoyagePortEventRemarks = request.Remarks ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    });
                    break;

                case "REGISTER_OPS_COMPLETED":
                    _db.VoyagePortEvents.Add(new VoyagePortEvent
                    {
                        VoyagePortId = voyagePort.Id,
                        EventTypeId = VoyageEventTypeIds.OperationsCompleted,
                        VoyagePortEventDateTime = request.ActionDateTime ?? DateTime.Now,
                        VoyagePortEventRemarks = request.Remarks ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    });
                    break;

                case "MARK_COMPLETED":
                    _db.VoyagePortEvents.Add(new VoyagePortEvent
                    {
                        VoyagePortId = voyagePort.Id,
                        EventTypeId = VoyageEventTypeIds.VoyageCompleted,
                        VoyagePortEventDateTime = request.ActionDateTime ?? DateTime.Now,
                        VoyagePortEventRemarks = request.Remarks ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    });
                    break;

                default:
                    throw new InvalidOperationException("Unsupported action.");
            }

            await _db.SaveChangesAsync();
        }

        private static IQueryable<VoyageMonitoringRowDto> ApplySorting(
            IQueryable<VoyageMonitoringRowDto> query,
            VoyageSearchRequestDto request)
        {
            var sortBy = request.SortBy?.Trim().ToLowerInvariant() ?? "eta";

            return sortBy switch
            {
                "vessel" => request.SortDesc
                    ? query.OrderByDescending(x => x.VesselName)
                    : query.OrderBy(x => x.VesselName),

                "voyagein" => request.SortDesc
                    ? query.OrderByDescending(x => x.VoyageNoIn)
                    : query.OrderBy(x => x.VoyageNoIn),

                "voyageout" => request.SortDesc
                    ? query.OrderByDescending(x => x.VoyageNoOut)
                    : query.OrderBy(x => x.VoyageNoOut),

                "port" => request.SortDesc
                    ? query.OrderByDescending(x => x.PortName)
                    : query.OrderBy(x => x.PortName),

                "ata" => request.SortDesc
                    ? query.OrderByDescending(x => x.ATA)
                    : query.OrderBy(x => x.ATA),

                "lasteventdate" => request.SortDesc
                    ? query.OrderByDescending(x => x.LastEventDate)
                    : query.OrderBy(x => x.LastEventDate),

                _ => request.SortDesc
                    ? query.OrderByDescending(x => x.ETA)
                    : query.OrderBy(x => x.ETA)
            };
        }
        public async Task<VoyageMonitoringRowDto?> GetMonitoringRowAsync(Guid voyagePortId)
        {
            return await _db.VoyagePorts
                .AsNoTracking()
                .Where(x => x.Id == voyagePortId)
                .Select(x => new VoyageMonitoringRowDto
                {
                    VoyagePortId = x.Id,
                    VesselName = x.InboundVoyage.Vessel.VesselName,
                    ShippingLineName = x.InboundVoyage.ShippingLine.Person.PersonName,
                    VoyageNoIn = x.InboundVoyage.VoyageNo,
                    VoyageNoOut = x.OutboundVoyage.VoyageNo,
                    PortName = x.Port.LocationName,
                    TerminalName = x.Terminal != null ? x.Terminal.LocationName : "",
                    VoyagePortSequence = x.VoyagePortSequence,
                    ETA = x.ETA,
                    ATA = x.ATA,
                    CurrentStage = ""
                })
                .FirstOrDefaultAsync();
        }


        public async Task<IReadOnlyList<VoyageLookupItemVm>> SearchForSelectAsync(
            string search,
            int take,
            CancellationToken cancellationToken)
        {
            search = search.Trim();

            return await _db.Voyages
                .Include(x => x.Vessel)
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            (x.Vessel.VesselName.Contains(search) ||
                             x.VoyageNo.Contains(search)))
                .OrderBy(x => x.Vessel.VesselName)
                .ThenBy(x => x.VoyageNo)
                .Take(take)
                .Select(x => new VoyageLookupItemVm
                {
                    Id = x.Id,
                    VesselName = x.Vessel.VesselName,
                    VoyageNo = x.VoyageNo
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<NLookupDto>> SearchPortsAsync(string? term, int take = 20)
        {
            // محدود کردن سقف رکوردها جهت امنیت و پرفورمنس
            if (take <= 0) take = 20;
            if (take > 100) take = 100;

            var query = _db.Locations
                .AsNoTracking()
                .AsQueryable();

            // فیلتر کردن بر اساس عبارت جستجو شده
            if (!string.IsNullOrWhiteSpace(term))
            {
                var trimmedTerm = term.Trim();

                // جستجو همزمان روی PortCode (مانند CNSHA یا IRBND) و PortName
                query = query.Where(p =>
                    p.LocationName.Contains(trimmedTerm) ||
                    (p.LocationCode != null && p.LocationCode.Contains(trimmedTerm)));
            }

            return await query
                .OrderBy(p => p.LocationName)
                .Take(take)
                .Select(p => new NLookupDto
                {
                    Value = p.Id.ToString(), // تبدیل Id (چه Guid چه int) به string
                    Text = !string.IsNullOrEmpty(p.LocationCode)
                        ? $"{p.LocationCode} - {p.LocationName}"
                        : p.LocationName
                })
                .ToListAsync();
        }


    }
}