using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Enums;
using ShipRate.Infrastructure.Data;
using System.Linq.Dynamic.Core;

namespace ShipRate.Application.Services
{
    public class TransportDocumentService : ITransportDocumentService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPersonService _personService;
        private readonly IContainerService _containerService;

        private const int ShipperRoleTypeId = 51;
        private const int ConsigneeRoleTypeId = 52;
        private const int NotifyRoleTypeId = 53;
        private const int AgentRoleTypeId = 3;


        private const int SeaTransportTypeId = 1;
        private const int RoadTransportTypeId = 2;
        private const int RailTransportTypeId = 3;
        private const int AirTransportTypeId = 4;
        private const int MultimodalTransportTypeId = 5;

        private const string DraftStatusCode = "Draft";
        private const string IssuedStatusCode = "Issued";
        private const string VoidStatusCode = "Void";

        public TransportDocumentService(ApplicationDbContext context, ICurrentUserService currentUserService, IPersonService personService, IContainerService containerService)
        {
            _db = context;
            _currentUserService = currentUserService;
            _personService = personService;
            _containerService = containerService;
        }

        public async Task<BLManagePageVm> GetManagePageAsync(BLContextDto context)
        {
            var items = await GetListAsync(context);

            return new BLManagePageVm
            {
                Context = context,
                Items = items
            };
        }

        public async Task<List<BLListItemDto>> GetListAsync(BLContextDto context)
        {
            var query = _db.BLs
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            query = ApplyContextFilter(query, context);

            var list = await query
                .OrderByDescending(x => x.BLIssueDate)
                .ThenByDescending(x => x.CreatedAt)
                .Select(x => new BLListItemDto
                {
                    Id = x.Id,
                    BLNo = x.BLNo,
                    BLIssueDate = x.BLIssueDate.Value,
                    TradeTypeId = x.TradeTypeId,
                    TradeTypeName = x.TradeType.TradeTypeName,
                    TransportTypeName = x.TransportType != null ? x.TransportType.TransportTypeName : "",
                    BLTypeName = x.BLType != null ? x.BLType.BLTypeName : "",
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : "",

                    POLText = !string.IsNullOrWhiteSpace(x.POLText)
                        ? x.POLText
                        : (x.POL != null ? x.POL.LocationName : ""),

                    PODText = !string.IsNullOrWhiteSpace(x.PODText) ? x.PODText
                        : (x.POD != null ? x.POD.LocationName : ""),

                    TotalContainers = x.BLContainers.Count(c => !c.IsDeleted),
                    TotalPackages = x.TotalPackages
                })
                .ToListAsync();
            return list;
        }
        public async Task<TransportDocumentUpsertVm> BuildCreateModelAsync(Guid jobId)
        {
            var job = await _db.Jobs
                .AsNoTracking()
                .Include(x => x.Containers)
                .Include(x => x.CargoDetails)
                .Where(x => x.Id == jobId).FirstOrDefaultAsync();
            var model = new TransportDocumentUpsertVm
            {
                JobId = jobId,
                JobNo = job.JobNumber + "-" + job.JobFileNumber,
                OriginalCount = 3,
                CopyCount = 3,
                IssuedByUs = false,
                BLIssueDate = DateTime.Today,
                TradeTypeId = job.TradeTypeId,
                TotalContainers = (int?)job.Containers?.Sum(x => x.Counts) ?? 0,
                VolumeCBM = job.CargoDetails?.Sum(x => x.Volume) ?? 0,
                PackageTypeId = job.CargoDetails?.FirstOrDefault()?.PackageTypeId,
            };

            if (!model.RailJourneys.Any())
            {
                model.RailJourneys.Add(new BLRailJourneyRowVm());
            }
            if (!model.Flights.Any())
            {
                model.Flights.Add(new BLFlightRowVm());
            }
            if (!model.Trips.Any())
            {
                model.Trips.Add(new BLTripRowVm());
            }
            if (!model.Voyages.Any())
            {
                model.Voyages.Add(new BLVoyageRowVm
                {
                    VoyageTypeId = 1
                });
            }

            await PrepareLookupsAsync(model);
            await PrepareBLReadyDataAsync(model);

            return model;
        }
        private async Task<TransportDocumentUpsertVm> PrepareBLReadyDataAsync(TransportDocumentUpsertVm model)
        {
            var jobId = model.JobId;
            var job = await _db.Jobs
                    .Include(x => x.Parties)
                    .Include(x => x.Routing)
                        .ThenInclude(x => x.OriginLocation)
                    .Include(x => x.Routing)
                        .ThenInclude(x => x.DestinationLocation)
                    .Include(x => x.CargoDetails)
                    .Where(x => x.Id == jobId)
                    .FirstOrDefaultAsync();
            //General
            model.TransportTypeId = job.TransportTypeId;
            model.IncotermId = job.IncotermId;
            model.LoadTypeId = job.LoadTypeId;
            model.MovementTypeId = job.Routing.FirstOrDefault()?.MovementTypeId;
            model.PaymentTypeId = job.FreightPaymentTypeId;



            //Persons
            model.ShipperPartyId = job.Parties.Where(x => x.RoleTypeId == 51).FirstOrDefault()?.PartyId;
            if (model.ShipperPartyId != null)
            {
                model.ShipperPartyName = await _personService.GetPersonNameAsync(model.ShipperPartyId.Value);
                model.ShipperTextInBL = await GetPartyTextInBLAsync(model.ShipperPartyId.Value);
            }

            model.ConsigneePartyId = job.Parties.Where(x => x.RoleTypeId == 52).FirstOrDefault()?.PartyId;
            if (model.ConsigneePartyId != null)
            {
                model.ConsigneePartyName = await _personService.GetPersonNameAsync(model.ConsigneePartyId.Value);
                model.ConsigneeTextInBL = await GetPartyTextInBLAsync(model.ConsigneePartyId.Value);
            }

            model.NotifyPartyId = job.Parties.Where(x => x.RoleTypeId == 53).FirstOrDefault()?.PartyId;
            if (model.NotifyPartyId != null)
            {
                model.NotifyPartyName = await _personService.GetPersonNameAsync(model.NotifyPartyId.Value);
                model.NotifyPartyTextInBL = await GetPartyTextInBLAsync(model.NotifyPartyId.Value);
            }
            model.AgentPartyId = job.Parties.Where(x => x.RoleTypeId == 3).FirstOrDefault()?.PartyId;
            if (model.AgentPartyId != null)
            {
                model.AgentPartyName = await _personService.GetPersonNameAsync(model.AgentPartyId.Value);
                model.AgentTextInBL = await GetPartyTextInBLAsync(model.AgentPartyId.Value);
            }

            model.CarrierId = job.Parties.Where(x => x.RoleTypeId == 1).FirstOrDefault()?.PartyId;
            model.IssuerPartyId = job.Parties.Where(x => x.RoleTypeId == 3).FirstOrDefault()?.PartyId;

            //Locations
            model.PORId = job.Routing.FirstOrDefault()?.OriginLocationId;
            model.PORText = job.Routing.FirstOrDefault()?.OriginLocation?.LocationName;
            model.PORName = job.Routing.FirstOrDefault()?.OriginLocation?.LocationName;

            model.PODelId = job.Routing.FirstOrDefault()?.DestinationLocationId;
            model.PODelText = job.Routing.FirstOrDefault()?.DestinationLocation?.LocationName;
            model.PODelName = job.Routing.FirstOrDefault()?.DestinationLocation?.LocationName;

            //Cargos
            model.PackageTypeId = job.CargoDetails.FirstOrDefault()?.PackageTypeId;
            model.PackageTypeText = job.CargoDetails.FirstOrDefault()?.PackageType?.PackageTypeName;
            model.TotalPackages = job.CargoDetails?.Sum(x => (int)x.PackageCount);
            model.GrossWeightKg = job.CargoDetails?.Sum(x => x.GrossWeight);
            model.NetWeightKg = job.CargoDetails?.Sum(x => x.NetWeight);
            model.VolumeCBM = job.CargoDetails?.Sum(x => x.Volume);
            model.ChargeableWeight = job.CargoDetails?.Sum(x => x.ChargeableWeight);
            model.GoodsDescription = job.CargoDetails != null
                                    ? string.Join(", ", job.CargoDetails.Select(x => x.CargoDescription))
                                    : string.Empty;
            return model;
        }
        public async Task<TransportDocumentUpsertVm> BuildEditModelAsync(Guid jobId, Guid id)
        {
            var bl = await _db.BLs
                .AsNoTracking()
                .Include(x => x.Job)
                .Include(x => x.BLStatus)
                .Include(x => x.BLParties)
                    .ThenInclude(x => x.Party)
                .Include(x => x.BLVoyages)
                    .ThenInclude(x => x.Voyage)
                        .ThenInclude(x => x.Vessel)
                .Include(x => x.BLFlights)
                    .ThenInclude(x => x.FlightDate)
                        .ThenInclude(x => x.Flight)
                .Include(x => x.BLFlights)
                    .ThenInclude(x => x.Airline)
                        .ThenInclude(x => x.Person)
                .Include(x => x.BLFlights)
                    .ThenInclude(x => x.Flight)
                        .ThenInclude(x => x.AirLine)
                            .ThenInclude(x => x.Person)
                .Include(x => x.BLTrips)
                    .ThenInclude(x => x.Trip)
                        .ThenInclude(x => x.Truck)
                .Include(x => x.BLTrips)
                    .ThenInclude(x => x.AllocatedContainers)
                .Include(x => x.BLRailJourneys)
                .Include(x => x.BLCharges)
                .Include(x => x.CargoDetails)
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.ContainerSizeType)
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.DimensionUnit)
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                 .Include(x => x.POL)
                 .Include(x => x.POD)
                 .Include(x => x.PODel)
                 .Include(x => x.POR)
                 .Include(x => x.IssuerParty)
                 .Include(x => x.TranshipmentPort)
                 .Include(x => x.PackageType)
                .FirstOrDefaultAsync(x => x.Id == id && x.JobId == jobId);

            if (bl == null)
                throw new InvalidOperationException("BL not found.");
            var shipper = bl.BLParties.FirstOrDefault(x => x.RoleTypeId == ShipperRoleTypeId);
            var consignee = bl.BLParties.FirstOrDefault(x => x.RoleTypeId == ConsigneeRoleTypeId);
            var notify = bl.BLParties.FirstOrDefault(x => x.RoleTypeId == NotifyRoleTypeId);
            var agent = bl.BLParties.FirstOrDefault(x => x.RoleTypeId == AgentRoleTypeId);

            //var sea = bl.BLVoyages.FirstOrDefault();
            //var air = bl.BLFlights.FirstOrDefault();
            //var road = bl.BLTrips.FirstOrDefault();
            //var rail = bl.BLRailJourneys.FirstOrDefault();



            var model = new TransportDocumentUpsertVm
            {
                Id = bl.Id,
                JobId = bl.JobId,
                JobNo = bl.Job.JobNumber + "-" + bl.Job.JobFileNumber,
                BookingId = bl.BookingId,
                ShipmentId = bl.ShipmentId,
                ShipmentLegId = bl.ShipmentLegId,
                CompanyId = bl.CompanyId,
                TransportTypeId = bl.TransportTypeId,
                BLTypeId = bl.BLTypeId,
                BLLevelId = bl.BLLevelId,
                IncotermId = bl.IncotermId,
                TradeTypeId = bl.TradeTypeId,

                BLNo = bl.BLNo,
                MasterBLNo = bl.MasterBLNo,
                BLIssueDate = bl.BLIssueDate,
                PlaceOfIssue = bl.PlaceOfIssue,

                CarrierId = bl.CarrierId,
                IssuerPartyId = bl.IssuerPartyId,
                IssuerPartyName = bl.IssuerParty?.PersonName,
                IssuedByUs = bl.IssuedByUs,


                BLReleaseTypeId = bl.BLReleaseTypeId,
                ReleaseDate = bl.ReleaseDate,

                OriginalCount = bl.OriginalCount,
                CopyCount = bl.CopyCount,


                BLStatusId = bl.BLStatusId,
                StatusName = bl.BLStatus != null ? bl.BLStatus.BLStatusName : null,

                ExportReference = bl.ExportReference,
                ForwardingAgentReference = bl.ForwardingAgentReference,
                SerialNo = bl.SerialNo,
                IsSwitched = bl.IsSwitched,

                ShipperPartyId = shipper?.PartyId,
                ShipperPartyName = shipper?.Party?.PersonName,
                ShipperTextInBL = shipper?.PartyTextInBL,

                ConsigneePartyId = consignee?.PartyId,
                ConsigneePartyName = consignee?.Party?.PersonName,
                ConsigneeTextInBL = consignee?.PartyTextInBL,
                IsConsigneeToOrder = consignee?.IsConsigneeToOrder ?? false,
                IsConsigneeToOrderOf = consignee?.IsConsigneeToOrderOf ?? false,
                IsConsigneeToBearer = consignee?.IsConsigneeToBearer ?? false,

                NotifyPartyId = notify?.PartyId,
                NotifyPartyName = notify?.Party?.PersonName,
                NotifyPartyTextInBL = notify?.PartyTextInBL,
                IsNotifySameAsConsignee = notify?.IsNotifySameAs ?? false,

                AgentPartyId = agent?.PartyId,
                AgentPartyName = agent?.Party?.PersonName,
                AgentTextInBL = agent?.PartyTextInBL,

                PORId = bl.PORId,
                PORName = bl.POR?.LocationCode + "-" + bl.POR?.LocationName,
                PORText = bl.PORText,

                POLId = bl.POLId,
                POLName = bl.POL?.LocationCode + "-" + bl.POL?.LocationName,
                POLText = bl.POLText,

                PODId = bl.PODId,
                PODName = bl.POD?.LocationCode + "-" + bl.POD?.LocationName,
                PODText = bl.PODText,

                PODelId = bl.PODelId,
                PODelName = bl.PODel?.LocationCode + "-" + bl.PODel?.LocationName,
                PODelText = bl.PODelText,

                FinalDestinationText = bl.FinalDestinationText,

                TranshipmentPortId = bl.TranshipmentPortId,
                TranshipmentPortName = bl.TranshipmentPort?.LocationName,

                PointandCountryOfOriginId = bl.PointandCountryOfOriginId,
                PointandCountryOfOriginText = bl.PointandCountryOfOriginText,

                OnwardInlandRoutingId = bl.OnwardInlandRoutingId,
                OnwardInlandRoutingText = bl.OnwardInlandRoutingText,

                LoadTypeId = bl.LoadTypeId,
                MovementTypeId = bl.MovementTypeId,

                MarksAndNumbers = bl.MarksAndNumbers,
                GoodsDescription = bl.GoodsDescription,

                GrossWeightKg = bl.GrossWeightKg,
                NetWeightKg = bl.NetWeightKg,
                VolumeCBM = bl.VolumeCBM,
                ChargeableWeight = bl.ChargeableWeight,

                TotalPackages = bl.TotalPackages,
                PackageTypeId = bl.PackageTypeId,
                PackageTypeText = bl.PackageTypeText,

                TotalContainers = bl.TotalContainers,

                PaymentTypeId = bl.PaymentTypeId,
                FreightTerms = bl.FreightTerms,

                POLDetentionTariffId = bl.POLDetentionTariffId,
                PODDetentionTariffId = bl.PODDetentionTariffId,

                FreightPaymentAtId = bl.FreightPaymentAtId,
                FreightPaymentAtText = bl.FreightPaymentAtText,

                ShippedOnBoardDate = bl.ShippedOnBoardDate,
                ReceivedForShipmentDate = bl.ReceivedForShipmentDate,

                ClauseRemarks = bl.ClauseRemarks,
                OnBoardNotation = bl.OnBoardNotation,
                RemarksInBL = bl.RemarksInBL,
                RemarksInManifest = bl.RemarksInManifest,
                InternalNotes = bl.InternalNotes,

                MRN = bl.MRN,
                PursuitCode = bl.PursuitCode,

                LCNo = bl.LCNo,
                LCBankName = bl.LCBankName,
                OrderRegistrationNo = bl.OrderRegistrationNo,
                PackageTypeName = bl.PackageType?.PackageTypeName,
                CargoDetails = bl.CargoDetails.Select(x => new CargoDetailVm
                {
                    Id = x.Id,
                    BLId = x.BLId,
                    CargoDescription = x.CargoDescription,
                    MarksAndNumbers = x.MarksAndNumbers,
                }).ToList(),
                Containers = bl.BLContainers
                .OrderBy(x => x.Seq)
                .Select(x => new BLContainerVm
                {
                    Seq = x.Seq,
                    Id = x.Id,
                    BLContainerPercent = x.BLContainerPercent,
                    ContainerNumber = x.ContainerNumber,
                    ContainerSizeTypeId = x.ContainerSizeTypeId,
                    ContainerTypeName = x.ContainerSizeType.ShortName,
                    TotalPackageCount = x.TotalPackageCount,
                    TotalGrossWeight = x.TotalGrossWeight,
                    TotalNetWeight = x.TotalNetWeight,
                    BLId = x.BLId,
                    OverHeight = x.OverHeight,
                    OverLength = x.OverLength,
                    OverWidth = x.OverWidth,
                    TotalVolumeCBM = x.TotalVolumeCBM,
                    DimensionUnitId = x.DimensionUnitId,
                    DimensionUnitName = x.DimensionUnit?.MeasurementUnitName,
                    SealNumber = x.SealNumber,
                    SealNumber2 = x.SealNumber2,
                    TareWeight = x.TareWeight,
                    CntOwnTypeId = x.CntOwnTypeId,
                    ContainerUseId = x.ContainerUseId,
                    LoadTypeId = x.LoadTypeId,
                    FullMTId = x.FullMTId,
                    ExportPermission = x.ExportPermissionNo,
                    CargoAllocations = x.CargoAllocations.Select(x => new BLContainerCargoAllocationVm
                    {
                        BLContainerId = x.BLContainerId,
                        CargoDetailId = x.CargoDetailId,
                        CargoDetailDescription = x.CargoDetail?.CargoDescription,
                        NetWeight = x.NetWeight,
                        GrossWeight = x.GrossWeight,
                        PackageCount = x.PackageCount,
                        Volume = x.Volume,
                        Id = x.Id,
                        Remarks = x.Remarks,
                        CargoDetailMarks = x.CargoDetail?.MarksAndNumbers,
                    }).ToList(),
                })
                .ToList(),
                Voyages = bl.BLVoyages
                .OrderBy(x => x.Id)
                .Select(x => new BLVoyageRowVm
                {
                    Id = x.Id,
                    VoyageTypeId = x.VoyageTypeId,
                    VesselName = x.VesselName,
                    VoyageNo = x.VoyageNo,
                    ShippedOnBoardDate = x.ShippedOnBoardDate,
                    ReceivedForShipmentDate = x.ReceivedForShipmentDate,
                    VoyageId = x.VoyageId,
                    SelectedVoyageText = x.Voyage.Vessel.VesselName + "-" + x.Voyage.VoyageNo,
                    IsFinal = x.IsFinal
                })
                .ToList(),
                Trips = bl.BLTrips
                .OrderBy(x => x.Id)
                .Select(x => new BLTripRowVm
                {
                    Id = x.Id,
                    TruckNo = x.TruckNo,
                    TrailerNo = x.TrailerNo,
                    DriverName = x.DriverName,
                    CMRNo = x.CMRNo,
                    ManifestNo = x.ManifestNo,
                    PackageCount = x.PackageCount,
                    GrossWeight = x.GrossWeight,
                    TIRNo = x.TIRNo,
                    TripId = x.TripId,
                    SelectedTripText = x.Trip?.Truck?.TruckPlateNumber + "-" + x.Trip?.Truck?.TrailerPlateNumber,
                    AllocatedContainersCount = x.AllocatedContainers.Count
                })
                .ToList(),
                RailJourneys = bl.BLRailJourneys
                .OrderBy(x => x.Id)
                .Select(x => new BLRailJourneyRowVm
                {
                    Id = x.Id,
                    TrainNo = x.TrainNo,
                    WagonNo = x.WagonNo
                })
                .ToList(),
                Flights = bl.BLFlights.Select(x => new BLFlightRowVm
                {
                    Id = x.Id,
                    AirlineId = x.AirlineId,
                    FlightNo = x.FlightNo,
                    FlightDateDate = x.FlightDateDate,
                    AirlineName = x.Airline != null ? x.Airline.Person.PersonName : null,
                    SelectedFlightText = x.Flight != null ? $"{x.Flight.AirLine.Person.PersonName} - {x.Flight.FlightNumber}" : null,
                    SelectedFlightDateText = x.FlightDate != null ? x.FlightDate.ETD?.ToString("yyyy-MM-dd HH:mm") : null,
                }).ToList()
                ,
                SummaryShipperName = !string.IsNullOrWhiteSpace(shipper?.PartyTextInBL)
                    ? shipper.PartyTextInBL
                    : "-",

                SummaryConsigneeName = !string.IsNullOrWhiteSpace(consignee?.PartyTextInBL)
                    ? consignee.PartyTextInBL
                    : "-",

                SummaryRoute = BuildRouteSummary(bl.POLText, bl.PODText),
                SummaryCargo = BuildCargoSummary(bl.TotalPackages, bl.PackageTypeText, bl.GrossWeightKg, bl.VolumeCBM),
                SummaryContainers = bl.TotalContainers.ToString(),

                SummaryTotalCharges = bl.BLCharges != null
                    ? bl.BLCharges.Sum(x => x.TotalAmount)
                    : 0
            };

            await PrepareLookupsAsync(model);

            if (!model.Voyages.Any())
            {
                model.Voyages.Add(new BLVoyageRowVm
                {
                    VoyageTypeId = 1
                });
            }
            if (!model.Trips.Any())
            {
                model.Trips.Add(new BLTripRowVm());
            }
            if (!model.Flights.Any())
            {
                model.Flights.Add(new BLFlightRowVm());
            }

            if (!model.RailJourneys.Any())
            {
                model.RailJourneys.Add(new BLRailJourneyRowVm());
            }

            return model;
        }
        public async Task<(bool Success, string Message)> CheckAndInsertMasterFileAsync(TransportDocumentUpsertVm model)
        {
            if (!model.Containers.Any()) return (true, string.Empty);
            foreach (var cnt in model.Containers)
            {
                if (string.IsNullOrEmpty(cnt.ContainerNumber) || !Iso6346.ValidateContainerNumber(cnt.ContainerNumber)) return (false, $"Container Numner {cnt.ContainerNumber} is invlaid");
                var item = _db.Containers.Where(x => x.ContainerNumber == cnt.ContainerNumber);
                if (!item.Any())
                {
                    ContainerFormVM vm = new ContainerFormVM
                    {
                        Id = Guid.CreateVersion7(),
                        ContainerNumber = cnt.ContainerNumber,
                        ContainerSizeTypeId = (int)cnt.ContainerSizeTypeId,
                        TareWeight = cnt.TareWeight != null ? (int)cnt.TareWeight : 0,
                        CntOwnTypeId = 1,
                        IsActive = true,
                        MaxPayLoad = 0,
                        OwnLeaseTypeId = 1,
                        ResponsibilityByUs = false,
                    };
                    var newId = await _containerService.CreateContainer(vm);
                }
            }
            return (true, string.Empty);
        }
        public async Task<Guid> SaveAsync(Guid jobId, TransportDocumentUpsertVm model)
        {
            (bool IsValid, string Message) check = await CheckAndInsertMasterFileAsync(model);
            if (!check.IsValid) { return Guid.Empty; }
            if (model.Id.HasValue && model.Id.Value != Guid.Empty)
                return await UpdateAsync(jobId, model);

            return await CreateAsync(jobId, model);
        }

        private async Task<Guid> CreateAsync(Guid jobId, TransportDocumentUpsertVm model)
        {
            var draftStatus = await GetStatusAsync(DraftStatusCode);

            var bl = new BL
            {
                Id = Guid.CreateVersion7(),

                CompanyId = model.CompanyId ?? 1,

                JobId = jobId,
                BookingId = model.BookingId,
                ShipmentId = model.ShipmentId,
                ShipmentLegId = model.ShipmentLegId,

                TransportTypeId = model.TransportTypeId ?? 0,
                BLTypeId = model.BLTypeId ?? 0,
                BLLevelId = model.BLLevelId ?? 0,
                TradeTypeId = model.TradeTypeId ?? 0,

                BLNo = model.BLNo ?? string.Empty,
                MasterBLNo = model.MasterBLNo ?? string.Empty,
                BLIssueDate = model.BLIssueDate,
                PlaceOfIssue = model.PlaceOfIssue,

                CarrierId = model.CarrierId,
                IssuerPartyId = model.IssuerPartyId,
                IssuedByUs = model.IssuedByUs,

                BLReleaseTypeId = model.BLReleaseTypeId,
                ReleaseDate = model.ReleaseDate,

                OriginalCount = model.OriginalCount ?? 0,
                CopyCount = model.CopyCount ?? 0,

                BLStatusId = draftStatus.Id,

                ExportReference = model.ExportReference,
                ForwardingAgentReference = model.ForwardingAgentReference,
                SerialNo = model.SerialNo,
                IsSwitched = model.IsSwitched,

                PORId = model.PORId,
                PORText = model.PORText ?? string.Empty,

                POLId = model.POLId,
                POLText = model.POLText ?? string.Empty,

                PODId = model.PODId,
                PODText = model.PODText ?? string.Empty,

                PODelId = model.PODelId,
                PODelText = model.PODelText ?? string.Empty,

                FinalDestinationText = model.FinalDestinationText ?? string.Empty,

                TranshipmentPortId = model.TranshipmentPortId,

                PointandCountryOfOriginId = model.PointandCountryOfOriginId,
                PointandCountryOfOriginText = model.PointandCountryOfOriginText,

                OnwardInlandRoutingId = model.OnwardInlandRoutingId,
                OnwardInlandRoutingText = model.OnwardInlandRoutingText,

                LoadTypeId = model.LoadTypeId,
                MovementTypeId = model.MovementTypeId,

                MarksAndNumbers = model.MarksAndNumbers,
                GoodsDescription = model.GoodsDescription,

                GrossWeightKg = model.GrossWeightKg,
                NetWeightKg = model.NetWeightKg,
                VolumeCBM = model.VolumeCBM,
                ChargeableWeight = model.ChargeableWeight,

                TotalPackages = model.TotalPackages ?? 0,
                PackageTypeId = model.PackageTypeId,
                PackageTypeText = model.PackageTypeText,

                TotalContainers = model.TotalContainers ?? 0,

                PaymentTypeId = model.PaymentTypeId,
                FreightTerms = model.FreightTerms,

                FreightPaymentAtId = model.FreightPaymentAtId,
                FreightPaymentAtText = model.FreightPaymentAtText,

                ShippedOnBoardDate = model.ShippedOnBoardDate,
                ReceivedForShipmentDate = model.ReceivedForShipmentDate,

                ClauseRemarks = model.ClauseRemarks,
                OnBoardNotation = model.OnBoardNotation,
                RemarksInBL = model.RemarksInBL,
                RemarksInManifest = model.RemarksInManifest,
                InternalNotes = model.InternalNotes,

                PODDetentionTariffId = model.POLDetentionTariffId,
                POLDetentionTariffId = model.POLDetentionTariffId,

                MRN = model.MRN,
                PursuitCode = model.PursuitCode,
                IncotermId = model.IncotermId,

                LCNo = model.LCNo,
                LCBankName = model.LCBankName,
                OrderRegistrationNo = model.OrderRegistrationNo,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now
            };
            AddOrUpdateParty(bl, ShipperRoleTypeId, model.ShipperPartyId, model.ShipperTextInBL, model, _currentUserService.UserId);
            AddOrUpdateParty(bl, ConsigneeRoleTypeId, model.ConsigneePartyId, model.ConsigneeTextInBL, model, _currentUserService.UserId);
            AddOrUpdateParty(bl, NotifyRoleTypeId, model.NotifyPartyId, model.NotifyPartyTextInBL, model, _currentUserService.UserId);
            AddOrUpdateParty(bl, AgentRoleTypeId, model.AgentPartyId, model.AgentTextInBL, model, _currentUserService.UserId);

            SyncModeSpecificDetails(bl, model, _currentUserService.UserId);

            _db.BLs.Add(bl);

            await _db.SaveChangesAsync();

            return bl.Id;
        }
        private async Task<Guid> UpdateAsync(Guid jobId, TransportDocumentUpsertVm model)
        {
            var bl = await _db.BLs
                .Include(x => x.BLParties)
                .Include(x => x.BLVoyages)
                .Include(x => x.BLFlights)
                .Include(x => x.BLTrips)
                .Include(x => x.BLRailJourneys)
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.JobId == jobId);
            if (bl == null)
                throw new InvalidOperationException("BL not found.");



            bl.BookingId = model.BookingId;
            bl.ShipmentId = model.ShipmentId;
            bl.ShipmentLegId = model.ShipmentLegId;

            if (model.CompanyId.HasValue)
                bl.CompanyId = model.CompanyId.Value;

            bl.TransportTypeId = model.TransportTypeId ?? 0;
            bl.BLTypeId = model.BLTypeId ?? 0;
            bl.BLLevelId = model.BLLevelId ?? 0;
            bl.TradeTypeId = model.TradeTypeId ?? 0;

            bl.BLNo = model.BLNo ?? string.Empty;
            bl.BLIssueDate = model.BLIssueDate;
            bl.PlaceOfIssue = model.PlaceOfIssue;
            bl.MasterBLNo = model.MasterBLNo;

            bl.CarrierId = model.CarrierId;
            bl.IssuerPartyId = model.IssuerPartyId;
            bl.IssuedByUs = model.IssuedByUs;

            bl.BLReleaseTypeId = model.BLReleaseTypeId;
            bl.ReleaseDate = model.ReleaseDate;

            bl.OriginalCount = model.OriginalCount ?? 0;
            bl.CopyCount = model.CopyCount ?? 0;

            bl.ExportReference = model.ExportReference;
            bl.ForwardingAgentReference = model.ForwardingAgentReference;
            bl.SerialNo = model.SerialNo;
            bl.IsSwitched = model.IsSwitched;


            bl.PORId = model.PORId;
            bl.PORText = model.PORText ?? string.Empty;

            bl.POLId = model.POLId;
            bl.POLText = model.POLText ?? string.Empty;

            bl.PODId = model.PODId;
            bl.PODText = model.PODText ?? string.Empty;

            bl.PODelId = model.PODelId;
            bl.PODelText = model.PODelText ?? string.Empty;

            bl.FinalDestinationText = model.FinalDestinationText ?? string.Empty;

            bl.TranshipmentPortId = model.TranshipmentPortId;

            bl.PointandCountryOfOriginId = model.PointandCountryOfOriginId;
            bl.PointandCountryOfOriginText = model.PointandCountryOfOriginText;


            bl.OnwardInlandRoutingId = model.OnwardInlandRoutingId;
            bl.OnwardInlandRoutingText = model.OnwardInlandRoutingText;

            bl.LoadTypeId = model.LoadTypeId;
            bl.MovementTypeId = model.MovementTypeId;

            bl.MarksAndNumbers = model.MarksAndNumbers;
            bl.GoodsDescription = model.GoodsDescription;

            bl.GrossWeightKg = model.GrossWeightKg;
            bl.NetWeightKg = model.NetWeightKg;
            bl.VolumeCBM = model.VolumeCBM;
            bl.ChargeableWeight = model.ChargeableWeight;

            bl.TotalPackages = model.TotalPackages ?? 0;
            bl.PackageTypeId = model.PackageTypeId;
            bl.PackageTypeText = model.PackageTypeText;

            bl.TotalContainers = model.TotalContainers ?? 0;

            bl.PaymentTypeId = model.PaymentTypeId;
            bl.FreightTerms = model.FreightTerms;

            bl.FreightPaymentAtId = model.FreightPaymentAtId;
            bl.FreightPaymentAtText = model.FreightPaymentAtText;

            bl.ShippedOnBoardDate = model.ShippedOnBoardDate;
            bl.ReceivedForShipmentDate = model.ReceivedForShipmentDate;

            bl.ClauseRemarks = model.ClauseRemarks;
            bl.OnBoardNotation = model.OnBoardNotation;
            bl.RemarksInBL = model.RemarksInBL;
            bl.RemarksInManifest = model.RemarksInManifest;
            bl.InternalNotes = model.InternalNotes;

            bl.MRN = model.MRN;
            bl.PursuitCode = model.PursuitCode;
            bl.IncotermId = model.IncotermId;

            bl.PODDetentionTariffId = model.PODDetentionTariffId;
            bl.POLDetentionTariffId = model.POLDetentionTariffId;

            bl.LCNo = model.LCNo;
            bl.LCBankName = model.LCBankName;
            bl.OrderRegistrationNo = model.OrderRegistrationNo;
            bl.EditUserId = _currentUserService.UserId;
            bl.UpdatedAt = DateTime.Now;

            AddOrUpdateParty(bl, ShipperRoleTypeId, model.ShipperPartyId, model.ShipperTextInBL, model, _currentUserService.UserId);
            AddOrUpdateParty(bl, ConsigneeRoleTypeId, model.ConsigneePartyId, model.ConsigneeTextInBL, model, _currentUserService.UserId);
            AddOrUpdateParty(bl, NotifyRoleTypeId, model.NotifyPartyId, model.NotifyPartyTextInBL, model, _currentUserService.UserId);
            AddOrUpdateParty(bl, AgentRoleTypeId, model.AgentPartyId, model.AgentTextInBL, model, _currentUserService.UserId);

            // 1. همگام‌سازی کانتینرها
            var incomingContainerIds = model.Containers?
                .Where(c => c.Id != Guid.Empty)
                .Select(c => c.Id)
                .ToHashSet() ?? new HashSet<Guid>();

            // الف) حذف کانتینرهایی که در لیست جدید نیستند
            var containersToDelete = bl.BLContainers
                .Where(c => !incomingContainerIds.Contains(c.Id))
                .ToList();

            foreach (var c in containersToDelete)
            {
                c.IsDeleted = true;
                c.DeletedAt = DateTime.Now;
                c.DeleteUserId = _currentUserService.UserId;
                _db.Entry(c).State = EntityState.Modified;
            }


            var oldAllocations = bl.BLContainers
                .SelectMany(x => x.CargoAllocations)
                .ToList();

            foreach (var item in oldAllocations)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.Now;
                item.DeleteUserId = _currentUserService.UserId;
            }


            //_db.BLContainerCargoAllocations.RemoveRange(oldAllocations);



            if (model.Containers?.Count > 0)
            {
                foreach (var containerDto in model.Containers)
                {
                    BLContainer blContainer;
                    if (containerDto.Id != Guid.Empty)
                    {
                        // کانتینر از قبل وجود دارد -> آپدیت
                        blContainer = bl.BLContainers.FirstOrDefault(c => c.Id == containerDto.Id);
                        if (blContainer != null)
                        {
                            blContainer.ContainerNumber = containerDto.ContainerNumber;
                            blContainer.Seq = containerDto.Seq;
                            blContainer.SealNumber = containerDto.SealNumber;
                            blContainer.SealNumber2 = containerDto.SealNumber2;
                            blContainer.DimensionUnitId = containerDto.DimensionUnitId;
                            blContainer.ContainerSizeTypeId = containerDto.ContainerSizeTypeId;
                            blContainer.TotalGrossWeight = containerDto.TotalGrossWeight;
                            blContainer.TotalNetWeight = containerDto.TotalNetWeight;
                            blContainer.TotalPackageCount = containerDto.TotalPackageCount;
                            blContainer.TotalVolumeCBM = containerDto.TotalVolumeCBM;
                            blContainer.CntOwnTypeId = containerDto.CntOwnTypeId.Value;
                            blContainer.FullMTId = containerDto.FullMTId;
                            blContainer.LoadTypeId = containerDto.LoadTypeId;
                            blContainer.TareWeight = containerDto.TareWeight;
                            blContainer.ExportPermissionNo = containerDto.ExportPermission;
                            blContainer.UpdatedAt = DateTime.Now;
                            blContainer.EditUserId = _currentUserService.UserId;
                            _db.Entry(blContainer).State = EntityState.Modified;

                            if (containerDto.CargoAllocations?.Count > 0)
                            {
                                foreach (var cargo in containerDto.CargoAllocations)
                                {
                                    if (cargo.CargoDetailId == Guid.Empty)
                                        continue;


                                    var cntAlloc = new BLContainerCargoAllocation
                                    {
                                        CargoDetailId = cargo.CargoDetailId,
                                        NetWeight = cargo.NetWeight,
                                        GrossWeight = cargo.GrossWeight,
                                        Volume = cargo.Volume,
                                        PackageCount = cargo.PackageCount,
                                        BLContainerId = blContainer.Id,
                                        Id = Guid.CreateVersion7(),
                                        CreatedAt = DateTime.Now,
                                        CreateUserId = _currentUserService.UserId
                                    };

                                    blContainer.CargoAllocations.Add(cntAlloc);
                                    _db.Entry(cntAlloc).State = EntityState.Added;
                                }
                            }
                        }
                    }
                    else
                    {
                        // کانتینر جدید است -> درج
                        blContainer = new BLContainer
                        {
                            Id = Guid.CreateVersion7(),
                            Seq = containerDto.Seq,
                            BLId = bl.Id,
                            ContainerNumber = containerDto.ContainerNumber,
                            SealNumber = containerDto.SealNumber,
                            SealNumber2 = containerDto.SealNumber2,
                            DimensionUnitId = containerDto.DimensionUnitId,
                            ContainerSizeTypeId = containerDto.ContainerSizeTypeId,
                            TotalGrossWeight = containerDto.TotalGrossWeight,
                            TotalNetWeight = containerDto.TotalNetWeight,
                            TotalPackageCount = containerDto.TotalPackageCount,
                            TotalVolumeCBM = containerDto.TotalVolumeCBM,
                            CntOwnTypeId = containerDto.CntOwnTypeId.Value,
                            FullMTId = containerDto.FullMTId,
                            LoadTypeId = containerDto.LoadTypeId,
                            TareWeight = containerDto.TareWeight,
                            ExportPermissionNo = containerDto.ExportPermission,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                            IsDeleted = false,
                        };

                        if (containerDto.CargoAllocations?.Count > 0)
                        {
                            foreach (var cargo in containerDto.CargoAllocations)
                            {
                                if (cargo.CargoDetailId == Guid.Empty)
                                    continue;

                                var cntAlloc = new BLContainerCargoAllocation
                                {
                                    CargoDetailId = cargo.CargoDetailId,
                                    NetWeight = cargo.NetWeight,
                                    GrossWeight = cargo.GrossWeight,
                                    Volume = cargo.Volume,
                                    PackageCount = cargo.PackageCount,
                                    BLContainerId = blContainer.Id,
                                    Id = Guid.CreateVersion7(),
                                    CreatedAt = DateTime.Now,
                                    CreateUserId = _currentUserService.UserId
                                };

                                blContainer.CargoAllocations.Add(cntAlloc);
                                _db.Entry(cntAlloc).State = EntityState.Added;

                            }
                        }

                        bl.BLContainers.Add(blContainer);
                        _db.Entry(blContainer).State = EntityState.Added;
                    }

                    if (blContainer == null) continue;

                    // همگام‌سازی تخصیص‌های کارگو (CargoAllocations) به ازای هر کانتینر
                    //SyncCargoAllocations(blContainer, containerDto.CargoAllocations, _currentUserService.UserId);
                }
            }
            SyncModeSpecificDetails(bl, model, _currentUserService.UserId);
            try
            {
                var changeStates = _db.ChangeTracker
                    .Entries()
                    .Where(x => x.Entity is BLParty)
                    .Select(x => new
                    {
                        Entity = x.Entity.GetType().Name,
                        State = x.State.ToString(),
                        //Id = ((BLContainer)x.Entity).Id,
                        //IsDeleted = ((BLContainer)x.Entity).IsDeleted,
                        //ContainerNo = ((BLContainer)x.Entity).ContainerNumber
                    })
                    .ToList();
                await _db.SaveChangesAsync();
                await ProcessContainerCyclesAsync(bl.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var details = ex.Entries.Select(e => new
                {
                    Entity = e.Entity.GetType().Name,
                    State = e.State.ToString(),
                    Values = e.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)
                }).ToList();

                throw new InvalidOperationException(
                    System.Text.Json.JsonSerializer.Serialize(details),
                    ex);
            }



            return bl.Id;
        }

        public async Task ProcessContainerCyclesAsync(Guid blId)
        {
            var bl = await _db.BLs.Where(x => x.Id == blId).FirstOrDefaultAsync();
            var containers = await _db.BLContainers.Where(x => x.BLId == blId).ToListAsync();
            if (containers is null || !containers.Any()) { return; }
            foreach (var container in containers)
            {
                if (container.ContainerUseId == null)
                {
                    var cnt = await _db.Containers.Where(x => x.ContainerNumber == container.ContainerNumber && !x.IsDeleted).FirstOrDefaultAsync();
                    if (cnt != null)
                    {
                        var existingContainerUse = await _db.ContainerUses.Where(x => x.ContainerId == cnt.Id && !x.IsDeleted && x.CycleClosed == false).FirstOrDefaultAsync();
                        if (existingContainerUse != null)
                            container.ContainerUseId = existingContainerUse.Id;
                        else
                        {
                            var containerUse = new ContainerUse
                            {
                                Id = Guid.CreateVersion7(),
                                ContainerId = cnt.Id,
                                CompanyId = bl.CompanyId,
                                ContainerNumber = cnt.ContainerNumber,
                                SealNumber = container.SealNumber?.Trim().ToUpperInvariant(),
                                ContainerSizeTypeId = container.ContainerSizeTypeId.Value,
                                TareWeight = (long)container.TareWeight,
                                GrossWeight = (long)container.TotalGrossWeight,
                                CycleClosed = false,
                                CntOwnTypeId = 1,
                                FullMTId = container.FullMTId.Value,
                                IsDeleted = false,
                                CreatedAt = DateTime.Now,
                                CreateUserId = _currentUserService.UserId,
                            };
                            _db.ContainerUses.Add(containerUse);
                            container.ContainerUseId = containerUse.Id;
                        }
                    }
                }
            }
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid jobId, Guid id)
        {
            var bl = await _db.BLs
                .FirstOrDefaultAsync(x => x.Id == id && x.JobId == jobId);

            if (bl == null)
                throw new InvalidOperationException("BL not found.");

            _db.BLs.Remove(bl);

            await _db.SaveChangesAsync();
        }

        public async Task IssueAsync(Guid jobId, Guid id)
        {
            var bl = await _db.BLs
                .FirstOrDefaultAsync(x => x.Id == id && x.JobId == jobId);

            if (bl == null)
                throw new InvalidOperationException("BL not found.");

            var issuedStatus = await GetStatusAsync(IssuedStatusCode);

            bl.BLStatusId = issuedStatus.Id;

            if (!bl.BLIssueDate.HasValue)
                bl.BLIssueDate = DateTime.Today;

            await _db.SaveChangesAsync();
        }

        public async Task VoidAsync(Guid jobId, Guid id)
        {
            var bl = await _db.BLs
                .FirstOrDefaultAsync(x => x.Id == id && x.JobId == jobId);

            if (bl == null)
                throw new InvalidOperationException("BL not found.");

            var voidStatus = await GetStatusAsync(VoidStatusCode);

            bl.BLStatusId = voidStatus.Id;

            await _db.SaveChangesAsync();
        }

        private async Task<dynamic> GetStatusAsync(string code)
        {
            var status = await _db.BLStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BLStatusName == code);

            if (status == null)
                throw new InvalidOperationException($"BL status '{code}' is not configured.");

            return status;
        }
        private void AddOrUpdateParty(
            BL bl,
            int roleTypeId,
            Guid? partyId,
            string? partyText,
            TransportDocumentUpsertVm model, Guid userId)
        {
            var hasValue = partyId.HasValue || !string.IsNullOrWhiteSpace(partyText);

            var party = bl.BLParties.FirstOrDefault(x => x.RoleTypeId == roleTypeId);

            if (!hasValue)
            {
                if (party != null)
                {
                    party.IsDeleted = true;
                    party.DeletedAt = DateTime.Now;
                    party.DeleteUserId = userId;
                    _db.Entry(party).State = EntityState.Modified;
                    //bl.BLParties.Remove(party);
                }
                return;
            }

            if (party == null)
            {
                party = new BLParty
                {
                    Id = Guid.CreateVersion7(),
                    BLId = bl.Id,
                    RoleTypeId = roleTypeId
                };

                party.PartyId = partyId;
                party.PartyTextInBL = partyText ?? string.Empty;

                if (roleTypeId == ConsigneeRoleTypeId)
                {
                    party.IsConsigneeToOrder = model.IsConsigneeToOrder;
                    party.IsConsigneeToOrderOf = model.IsConsigneeToOrderOf;
                    party.IsConsigneeToBearer = model.IsConsigneeToBearer;
                }

                if (roleTypeId == NotifyRoleTypeId)
                {
                    party.IsNotifySameAs = model.IsNotifySameAsConsignee;
                }

                party.CreatedAt = DateTime.Now;
                party.CreateUserId = userId;
                bl.BLParties.Add(party);
                _db.Entry(party).State = EntityState.Added;
            }
            else
            {
                party.PartyId = partyId;
                party.PartyTextInBL = partyText ?? string.Empty;

                if (roleTypeId == ConsigneeRoleTypeId)
                {
                    party.IsConsigneeToOrder = model.IsConsigneeToOrder;
                    party.IsConsigneeToOrderOf = model.IsConsigneeToOrderOf;
                    party.IsConsigneeToBearer = model.IsConsigneeToBearer;
                }

                if (roleTypeId == NotifyRoleTypeId)
                {
                    party.IsNotifySameAs = model.IsNotifySameAsConsignee;
                }
                _db.Entry(party).State = EntityState.Modified;
            }
        }



        private void SyncModeSpecificDetails(BL bl, TransportDocumentUpsertVm model, Guid userId)
        {
            // ۱. بررسی بر اساس مد حمل انتخاب‌شده
            switch (model.TransportTypeId)
            {
                case SeaTransportTypeId:
                    SyncSeaDetails(bl, model.Voyages ?? [], userId);
                    SoftDeleteRemaining(bl.BLTrips, [], userId);
                    SoftDeleteRemaining(bl.BLFlights, [], userId);
                    SoftDeleteRemaining(bl.BLRailJourneys, [], userId);
                    break;

                case AirTransportTypeId:
                    SyncAirDetails(bl, model.Flights ?? [], userId);
                    SoftDeleteRemaining(bl.BLVoyages, [], userId);
                    SoftDeleteRemaining(bl.BLTrips, [], userId);
                    SoftDeleteRemaining(bl.BLRailJourneys, [], userId);
                    break;

                case RoadTransportTypeId:
                    SyncRoadDetails(bl, model.Trips ?? [], userId);
                    SoftDeleteRemaining(bl.BLVoyages, [], userId);
                    SoftDeleteRemaining(bl.BLFlights, [], userId);
                    SoftDeleteRemaining(bl.BLRailJourneys, [], userId);
                    break;

                case RailTransportTypeId:
                    SyncRailDetails(bl, model.RailJourneys ?? [], userId);
                    SoftDeleteRemaining(bl.BLVoyages, [], userId);
                    SoftDeleteRemaining(bl.BLTrips, [], userId);
                    SoftDeleteRemaining(bl.BLFlights, [], userId);
                    break;

                case MultimodalTransportTypeId:
                    SyncSeaDetails(bl, model.Voyages ?? [], userId);
                    SyncRoadDetails(bl, model.Trips ?? [], userId);
                    SyncRailDetails(bl, model.RailJourneys ?? [], userId);
                    SyncAirDetails(bl, model.Flights ?? [], userId);
                    break;
            }
        }



        private void SyncRoadDetails(BL bl, IEnumerable<BLTripRowVm> rows, Guid userId)
        {
            var now = DateTime.Now;
            var submittedRows = rows.Where(row =>
                row.TripId.HasValue ||
                !string.IsNullOrWhiteSpace(row.TruckNo) ||
                !string.IsNullOrWhiteSpace(row.TrailerNo) ||
                !string.IsNullOrWhiteSpace(row.DriverName) ||
                !string.IsNullOrWhiteSpace(row.CMRNo) ||
                !string.IsNullOrWhiteSpace(row.TIRNo) ||
                row.LoadingDate.HasValue ||
                row.DeliveryDate.HasValue).ToList();

            var submittedIds = submittedRows
                .Where(r => r.Id.HasValue && r.Id.Value != Guid.Empty)
                .Select(r => r.Id.Value)
                .ToHashSet();

            // حذف رکوردهایی که کاربر از جدول حذف کرده است
            SoftDeleteRemaining(bl.BLTrips, submittedIds, userId);

            foreach (var row in submittedRows)
            {
                // ردیف موجود -> ویرایش
                if (row.Id.HasValue && row.Id.Value != Guid.Empty)
                {
                    var existing = bl.BLTrips.FirstOrDefault(x => x.Id == row.Id.Value && !x.IsDeleted);
                    if (existing != null)
                    {
                        existing.TripId = row.TripId;
                        existing.TruckNo = row.TruckNo?.Trim();
                        existing.TrailerNo = row.TrailerNo?.Trim();
                        existing.DriverName = row.DriverName?.Trim();
                        existing.DriverPassportNo = row.DriverPassportNo?.Trim();
                        existing.PackageCount = row.PackageCount;
                        existing.GrossWeight = row.GrossWeight;
                        existing.LoadingDate = row.LoadingDate;
                        existing.DeliveryDate = row.DeliveryDate;
                        existing.CMRNo = row.CMRNo?.Trim();
                        existing.TIRNo = row.TIRNo?.Trim();
                        existing.ManifestNo = row.ManifestNo?.Trim();
                        existing.EditUserId = userId;
                        existing.UpdatedAt = now;

                        _db.Entry(existing).State = EntityState.Modified;
                        continue;
                    }
                }

                // ردیف جدید -> درج
                var newTrip = new BLTrip
                {
                    Id = Guid.CreateVersion7(),
                    BLId = bl.Id,
                    TripId = row.TripId,
                    TruckNo = row.TruckNo?.Trim(),
                    TrailerNo = row.TrailerNo?.Trim(),
                    DriverName = row.DriverName?.Trim(),
                    DriverPassportNo = row.DriverPassportNo?.Trim(),
                    PackageCount = row.PackageCount,
                    GrossWeight = row.GrossWeight,
                    LoadingDate = row.LoadingDate,
                    DeliveryDate = row.DeliveryDate,
                    CMRNo = row.CMRNo?.Trim(),
                    TIRNo = row.TIRNo?.Trim(),
                    ManifestNo = row.ManifestNo?.Trim(),
                    CreateUserId = userId,
                    CreatedAt = now
                };

                bl.BLTrips.Add(newTrip);
                _db.Entry(newTrip).State = EntityState.Added;
            }
        }




        private void SyncSeaDetails(BL bl, IEnumerable<BLVoyageRowVm> rows, Guid userId)
        {
            var now = DateTime.Now;
            var submittedRows = rows.Where(row =>
                !string.IsNullOrWhiteSpace(row.VesselName) ||
                !string.IsNullOrWhiteSpace(row.VoyageNo) ||
                row.ShippedOnBoardDate.HasValue ||
                row.ReceivedForShipmentDate.HasValue).ToList();

            var submittedIds = submittedRows
                .Where(r => r.Id.HasValue && r.Id.Value != Guid.Empty)
                .Select(r => r.Id.Value)
                .ToHashSet();

            SoftDeleteRemaining(bl.BLVoyages, submittedIds, userId);

            foreach (var row in submittedRows)
            {
                if (row.Id.HasValue && row.Id.Value != Guid.Empty)
                {
                    var existing = bl.BLVoyages.FirstOrDefault(x => x.Id == row.Id.Value && !x.IsDeleted);
                    if (existing != null)
                    {
                        existing.VoyageTypeId = row.VoyageTypeId ?? 1;
                        existing.VesselName = row.VesselName?.Trim() ?? string.Empty;
                        existing.VoyageNo = row.VoyageNo?.Trim() ?? string.Empty;
                        existing.ShippedOnBoardDate = row.ShippedOnBoardDate;
                        existing.ReceivedForShipmentDate = row.ReceivedForShipmentDate;
                        existing.VoyageId = row.VoyageId;
                        existing.EditUserId = userId;
                        existing.UpdatedAt = now;
                        existing.IsFinal = row.IsFinal;

                        _db.Entry(existing).State = EntityState.Modified;
                        continue;
                    }
                }
                var newVoyage = new BLVoyage
                {
                    Id = Guid.CreateVersion7(),
                    BLId = bl.Id,
                    VoyageTypeId = row.VoyageTypeId ?? 1,
                    VesselName = row.VesselName?.Trim() ?? string.Empty,
                    VoyageNo = row.VoyageNo?.Trim() ?? string.Empty,
                    ShippedOnBoardDate = row.ShippedOnBoardDate,
                    ReceivedForShipmentDate = row.ReceivedForShipmentDate,
                    VoyageId = row.VoyageId,
                    CreateUserId = userId,
                    CreatedAt = now,
                    IsFinal = row.IsFinal
                };

                bl.BLVoyages.Add(newVoyage);
                _db.Entry(newVoyage).State = EntityState.Added;
            }
        }

        private void SyncRailDetails(BL bl, IEnumerable<BLRailJourneyRowVm> rows, Guid userId)
        {
            var now = DateTime.Now;
            var submittedRows = rows.Where(row =>
                !string.IsNullOrWhiteSpace(row.TrainNo) ||
                !string.IsNullOrWhiteSpace(row.WagonNo)).ToList();

            var submittedIds = submittedRows
                .Where(r => r.Id.HasValue && r.Id.Value != Guid.Empty)
                .Select(r => r.Id.Value)
                .ToHashSet();

            SoftDeleteRemaining(bl.BLRailJourneys, submittedIds, userId);

            foreach (var row in submittedRows)
            {
                if (row.Id.HasValue && row.Id.Value != Guid.Empty)
                {
                    var existing = bl.BLRailJourneys.FirstOrDefault(x => x.Id == row.Id.Value && !x.IsDeleted);
                    if (existing != null)
                    {
                        existing.TrainNo = row.TrainNo?.Trim() ?? string.Empty;
                        existing.WagonNo = row.WagonNo?.Trim() ?? string.Empty;
                        existing.EditUserId = userId;
                        existing.UpdatedAt = now;

                        _db.Entry(existing).State = EntityState.Modified;
                        continue;
                    }
                }

                var newRail = new BLRailJourney
                {
                    Id = Guid.CreateVersion7(),
                    BLId = bl.Id,
                    TrainNo = row.TrainNo?.Trim() ?? string.Empty,
                    WagonNo = row.WagonNo?.Trim() ?? string.Empty,
                    CreateUserId = userId,
                    CreatedAt = now
                };

                bl.BLRailJourneys.Add(newRail);
                _db.Entry(newRail).State = EntityState.Added;
            }
        }

        private void SyncAirDetails(BL bl, IEnumerable<BLFlightRowVm> rows, Guid userId)
        {
            var now = DateTime.Now;
            var submittedRows = rows.Where(row =>
                row.FlightId.HasValue ||
                row.FlightDateId.HasValue ||
                row.AirlineId.HasValue ||
                !string.IsNullOrWhiteSpace(row.FlightNo) ||
                row.FlightDateDate.HasValue ||
                !string.IsNullOrWhiteSpace(row.DeclaredValueForCarriage) ||
                !string.IsNullOrWhiteSpace(row.DeclaredValueForCustoms) ||
                !string.IsNullOrWhiteSpace(row.HandlingInformation)).ToList();

            var submittedIds = submittedRows
                .Where(r => r.Id.HasValue && r.Id.Value != Guid.Empty)
                .Select(r => r.Id.Value)
                .ToHashSet();

            SoftDeleteRemaining(bl.BLFlights, submittedIds, userId);

            foreach (var row in submittedRows)
            {
                if (row.Id.HasValue && row.Id.Value != Guid.Empty)
                {
                    var existing = bl.BLFlights.FirstOrDefault(x => x.Id == row.Id.Value && !x.IsDeleted);
                    if (existing != null)
                    {
                        existing.AirlineId = row.AirlineId;
                        existing.FlightId = row.FlightId;
                        existing.FlightDateId = row.FlightDateId;
                        existing.FlightNo = row.FlightNo?.Trim();
                        existing.FlightDateDate = row.FlightDateDate;
                        existing.FlightDateLocal = row.FlightDateLocal?.Trim();
                        existing.DeclaredValueForCarriage = row.DeclaredValueForCarriage?.Trim();
                        existing.DeclaredValueForCustoms = row.DeclaredValueForCustoms?.Trim();
                        existing.HandlingInformation = row.HandlingInformation?.Trim();
                        existing.EditUserId = userId;
                        existing.UpdatedAt = now;

                        _db.Entry(existing).State = EntityState.Modified;
                        continue;
                    }
                }

                var newFlight = new BLFlight
                {
                    Id = Guid.CreateVersion7(),
                    BLId = bl.Id,
                    AirlineId = row.AirlineId,
                    FlightId = row.FlightId,
                    FlightDateId = row.FlightDateId,
                    FlightNo = row.FlightNo?.Trim(),
                    FlightDateDate = row.FlightDateDate,
                    FlightDateLocal = row.FlightDateLocal?.Trim(),
                    DeclaredValueForCarriage = row.DeclaredValueForCarriage?.Trim(),
                    DeclaredValueForCustoms = row.DeclaredValueForCustoms?.Trim(),
                    HandlingInformation = row.HandlingInformation?.Trim(),
                    CreateUserId = userId,
                    CreatedAt = now
                };

                bl.BLFlights.Add(newFlight);
                _db.Entry(newFlight).State = EntityState.Added;
            }
        }



        private static void SoftDeleteRemaining<TEntity>(
    IEnumerable<TEntity> currentList,
    ICollection<Guid> keepIds,
    Guid userId) where TEntity : class
        {
            var now = DateTime.Now;
            foreach (var entity in currentList)
            {
                // در صورتی که اینتیتی‌های شما این اینترفیس یا پراپرتی‌ها را دارند:
                dynamic item = entity;
                if (item.IsDeleted) continue;

                if (!keepIds.Contains((Guid)item.Id))
                {
                    item.IsDeleted = true;
                    item.DeleteUserId = userId;
                    item.DeletedAt = now;
                }
            }
        }






        //private void AddModeSpecificDetails(BL bl, TransportDocumentUpsertVm model, Guid userId)
        //{
        //    switch (model.TransportTypeId)
        //    {
        //        case SeaTransportTypeId:
        //            AddSeaDetails(bl, model, userId);
        //            break;

        //        case AirTransportTypeId:
        //            AddAirDetails(bl, model, userId);
        //            break;

        //        case RoadTransportTypeId:
        //            AddRoadDetails(bl, model, userId);
        //            break;

        //        case RailTransportTypeId:
        //            AddRailDetails(bl, model, userId);
        //            break;
        //        case MultimodalTransportTypeId:
        //            AddSeaDetails(bl, model, userId);
        //            AddRoadDetails(bl, model, userId);
        //            AddRailDetails(bl, model, userId);
        //            break;
        //    }
        //}
        //private void AddSeaDetails(BL bl, TransportDocumentUpsertVm model, Guid userId)
        //{
        //    foreach (var row in model.Voyages)
        //    {
        //        var hasValue =
        //            !string.IsNullOrWhiteSpace(row.VesselName) ||
        //            !string.IsNullOrWhiteSpace(row.VoyageNo) ||
        //            row.ShippedOnBoardDate.HasValue ||
        //            row.ReceivedForShipmentDate.HasValue;

        //        if (!hasValue)
        //            continue;


        //        var blVoyage = new BLVoyage
        //        {
        //            //Id = row.Id.GetValueOrDefault(Guid.CreateVersion7()),
        //            Id = Guid.CreateVersion7(),
        //            BLId = bl.Id,
        //            VoyageTypeId = row.VoyageTypeId ?? 1,
        //            VesselName = row.VesselName ?? string.Empty,
        //            VoyageNo = row.VoyageNo ?? string.Empty,
        //            ShippedOnBoardDate = row.ShippedOnBoardDate,
        //            ReceivedForShipmentDate = row.ReceivedForShipmentDate,
        //            CreateUserId = userId,
        //            CreatedAt = DateTime.Now,
        //            VoyageId = row.VoyageId,
        //        };

        //        bl.BLVoyages.Add(blVoyage);
        //        _db.Entry(blVoyage).State = EntityState.Added;
        //    }
        //}

        //private void AddAirDetails(
        //    BL bl,
        //    TransportDocumentUpsertVm model,
        //    Guid userId)
        //{
        //    var now = DateTime.Now;

        //    foreach (var row in model.Flights ?? [])
        //    {
        //        var hasValue =
        //            row.FlightId.HasValue ||
        //            row.FlightDateId.HasValue ||
        //            row.AirlineId.HasValue ||
        //            !string.IsNullOrWhiteSpace(row.FlightNo) ||
        //            row.FlightDateDate.HasValue ||
        //            !string.IsNullOrWhiteSpace(row.DeclaredValueForCarriage) ||
        //            !string.IsNullOrWhiteSpace(row.DeclaredValueForCustoms) ||
        //            !string.IsNullOrWhiteSpace(row.HandlingInformation);

        //        if (!hasValue)
        //        {
        //            continue;
        //        }

        //        var blFlight = new BLFlight
        //        {
        //            Id = Guid.CreateVersion7(),
        //            BLId = bl.Id,

        //            AirlineId = row.AirlineId,
        //            FlightId = row.FlightId,
        //            FlightDateId = row.FlightDateId,

        //            FlightNo = row.FlightNo?.Trim(),
        //            FlightDateDate = row.FlightDateDate,
        //            FlightDateLocal = row.FlightDateLocal?.Trim(),

        //            DeclaredValueForCarriage =
        //                row.DeclaredValueForCarriage?.Trim(),

        //            DeclaredValueForCustoms =
        //                row.DeclaredValueForCustoms?.Trim(),

        //            HandlingInformation =
        //                row.HandlingInformation?.Trim(),

        //            CreateUserId = userId,
        //            CreatedAt = now
        //        };

        //        bl.BLFlights.Add(blFlight);

        //        // مطابق مشکل قبلی ChangeTracker پروژه:
        //        _db.Entry(blFlight).State = EntityState.Added;
        //    }
        //}

        //private void AddRoadDetails(
        //    BL bl,
        //    TransportDocumentUpsertVm model,
        //    Guid userId)
        //{
        //    var now = DateTime.Now;

        //    foreach (var row in model.Trips ?? [])
        //    {
        //        var hasValue =
        //            row.TripId.HasValue ||
        //            !string.IsNullOrWhiteSpace(row.TruckNo) ||
        //            !string.IsNullOrWhiteSpace(row.TrailerNo) ||
        //            !string.IsNullOrWhiteSpace(row.DriverName) ||
        //            !string.IsNullOrWhiteSpace(row.CMRNo) ||
        //            !string.IsNullOrWhiteSpace(row.TIRNo) ||
        //            row.LoadingDate.HasValue ||
        //            row.DeliveryDate.HasValue;

        //        if (!hasValue)
        //        {
        //            continue;
        //        }

        //        var blTrip = new BLTrip
        //        {
        //            Id = Guid.CreateVersion7(),
        //            BLId = bl.Id,

        //            TripId = row.TripId,

        //            TruckNo = row.TruckNo?.Trim(),
        //            TrailerNo = row.TrailerNo?.Trim(),
        //            DriverName = row.DriverName?.Trim(),
        //            DriverPassportNo = row.DriverPassportNo?.Trim(),

        //            PackageCount = row.PackageCount,
        //            GrossWeight = row.GrossWeight,

        //            LoadingDate = row.LoadingDate,
        //            DeliveryDate = row.DeliveryDate,

        //            CMRNo = row.CMRNo?.Trim(),
        //            TIRNo = row.TIRNo?.Trim(),
        //            ManifestNo = row.ManifestNo?.Trim(),

        //            CreateUserId = userId,
        //            CreatedAt = now
        //        };

        //        bl.BLTrips.Add(blTrip);

        //        // به‌دلیل مشکلی که قبلاً برای BLVoyage داشتید:
        //        _db.Entry(blTrip).State = EntityState.Added;
        //    }
        //}
        //private static void AddRailDetails(BL bl, TransportDocumentUpsertVm model, Guid userId)
        //{
        //    if (model.RailJourneys == null || !model.RailJourneys.Any())
        //        return;

        //    foreach (var row in model.RailJourneys)
        //    {
        //        var hasValue =
        //            !string.IsNullOrWhiteSpace(row.TrainNo) ||
        //            !string.IsNullOrWhiteSpace(row.WagonNo);

        //        if (!hasValue)
        //            continue;

        //        bl.BLRailJourneys.Add(new BLRailJourney
        //        {
        //            //Id = row.Id.GetValueOrDefault(Guid.CreateVersion7()),
        //            Id = Guid.CreateVersion7(),
        //            BLId = bl.Id,
        //            TrainNo = row.TrainNo ?? string.Empty,
        //            WagonNo = row.WagonNo ?? string.Empty,
        //            CreateUserId = userId,
        //            CreatedAt = DateTime.Now
        //        });
        //    }
        //}



        //private void ReplaceModeSpecificDetails(BL bl, TransportDocumentUpsertVm model)
        //{
        //    foreach (var item in bl.BLVoyages)
        //    {
        //        item.IsDeleted = true;
        //        item.DeleteUserId = _currentUserService.UserId;
        //        item.DeletedAt = DateTime.Now;
        //    }
        //    foreach (var item in bl.BLTrips)
        //    {
        //        item.IsDeleted = true;
        //        item.DeleteUserId = _currentUserService.UserId;
        //        item.DeletedAt = DateTime.Now;
        //    }
        //    foreach (var item in bl.BLFlights)
        //    {
        //        item.IsDeleted = true;
        //        item.DeleteUserId = _currentUserService.UserId;
        //        item.DeletedAt = DateTime.Now;
        //    }
        //    foreach (var item in bl.BLRailJourneys)
        //    {
        //        item.IsDeleted = true;
        //        item.DeleteUserId = _currentUserService.UserId;
        //        item.DeletedAt = DateTime.Now;
        //    }
        //    AddModeSpecificDetails(bl, model, _currentUserService.UserId);
        //}




        private async Task PrepareLookupsAsync(TransportDocumentUpsertVm model)
        {
            model.TradeTypes = await _db.TradeTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TradeTypeName
                })
                .ToListAsync();
            model.VoyageTypes = await _db.VoyageTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyageTypeName
                })
                .ToListAsync();
            model.DetentionTariffs = await _db.TariffBooks
                .AsNoTracking()
                .Where(X => X.TariffTypeId == 3)
                .OrderBy(x => x.Code)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Code
                })
                .ToListAsync();


            model.FullMTs = await _db.FullMT
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync();

            model.CntOwnTypes = await _db.CntOwnTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();

            //model.LoadTypes = await _db.LoadTypes
            //    .AsNoTracking()
            //    .Where(x => x.Id <= 4)   //For Containers
            //    .OrderBy(x => x.Id)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.LoadTypeCode
            //    })
            //    .ToListAsync();

            model.Incoterms = await _db.Incoterms
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.IncotermCode
                })
                .ToListAsync();

            model.BLStatuses = await _db.BLStatuses
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLStatusName
                })
                .ToListAsync();

            model.BLTypes = await _db.BLTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLTypeName
                })
                .ToListAsync();

            model.BLLevels = await _db.BLLevels
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLLevelName
                })
                .ToListAsync();

            model.TransportTypes = await _db.TransportTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TransportTypeName
                })
                .ToListAsync();

            model.BLReleaseTypes = await _db.BLReleaseTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLReleaseTypeName
                })
                .ToListAsync();

            model.LoadTypes = await _db.LoadTypes
                .AsNoTracking()
                .Where(x => x.Id <= 4)   //For Containers
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LoadTypeCode
                })
                .ToListAsync();

            model.MovementTypes = await _db.MovementTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MovementTypeCode
                })
                .ToListAsync();

            model.PaymentTypes = await _db.PaymentTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PaymentTypeName
                })
                .ToListAsync();

            //model.PackageTypes = await _db.PackageTypes
            //    .AsNoTracking()
            //    .OrderBy(x => x.PackageTypeName)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.PackageTypeName
            //    })
            //    .ToListAsync();

            model.Carriers = await _db.Carriers
                .Include(x => x.Person)
                .AsNoTracking()
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();

            //model.Locations = await _db.Locations
            //    .AsNoTracking()
            //    .OrderBy(x => x.LocationName)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.LocationName
            //    })
            //    .ToListAsync();

            model.Airlines = await _db.Carriers
                .Include(x => x.Person)
                .Where(x => x.CarrierTypeId == 3)
                .AsNoTracking()
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();

            model.ContainerSizeTypes = await _db.ContainerSizeTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShortName
                })
                .ToListAsync();




        }

        private static string BuildRouteSummary(string? pol, string? pod)
        {
            if (string.IsNullOrWhiteSpace(pol) && string.IsNullOrWhiteSpace(pod))
                return "-";

            if (string.IsNullOrWhiteSpace(pol))
                return pod ?? "-";

            if (string.IsNullOrWhiteSpace(pod))
                return pol;

            return $"{pol} → {pod}";
        }

        private static string BuildCargoSummary(
            int? totalPackages,
            string? packageTypeText,
            decimal? grossWeightKg,
            decimal? volumeCbm)
        {
            var parts = new List<string>();

            if (totalPackages.HasValue && totalPackages.Value > 0)
            {
                var packageText = string.IsNullOrWhiteSpace(packageTypeText)
                    ? "PKG"
                    : packageTypeText;

                parts.Add($"{totalPackages.Value:N0} {packageText}");
            }

            if (grossWeightKg.HasValue && grossWeightKg.Value > 0)
                parts.Add($"{grossWeightKg.Value:N2} KG");

            if (volumeCbm.HasValue && volumeCbm.Value > 0)
                parts.Add($"{volumeCbm.Value:N3} CBM");

            return parts.Count == 0 ? "-" : string.Join(" / ", parts);
        }


        private static void MapCargoDetailVmToEntity(
            CargoDetailVm vm,
            CargoDetail entity,
            Guid blId)
        {
            // Always force BL-level cargo relation from server side
            entity.BLId = blId;

            // Current phase: cargo is stored under BL, not container
            //entity.BLContainerId = null;

            entity.Sequence = vm.Sequence;

            // Package
            entity.PackageTypeId = vm.PackageTypeId;
            entity.PackageCount = vm.PackageCount;

            // Weight
            entity.GrossWeight = vm.GrossWeight;
            entity.NetWeight = vm.NetWeight;
            entity.ChargeableWeight = vm.ChargeableWeight;
            entity.WeightUnitId = vm.WeightUnitId;

            // Volume
            entity.Volume = vm.Volume;
            entity.VolumeUnitId = vm.VolumeUnitId;

            // Dimensions / Oversized
            entity.IsOverSized = vm.IsOverSized;
            entity.Length = vm.Length;
            entity.Width = vm.Width;
            entity.Height = vm.Height;
            entity.DimensionUnitId = vm.DimensionUnitId;

            // HS Code / Commodity
            entity.CountryHSCodeId = vm.CountryHSCodeId;
            entity.HSCodeToSave = vm.HSCodeToSave;
            entity.CargoCommodityId = vm.CargoCommodityId;

            // Dangerous Goods
            entity.IsDG = vm.IsDG;
            entity.DGClassId = vm.DGClassId;
            entity.UNNumber = vm.UNNumber;
            entity.DGFlashPoint = vm.DGFlashPoint;
            entity.DGPackingGroupId = vm.DGPackingGroupId;

            // Temperature Controlled
            entity.IsTemperatureControlled = vm.IsTemperatureControlled;
            entity.MinTemp = vm.MinTemp;
            entity.NormalTemp = vm.NormalTemp;
            entity.MaxTemp = vm.MaxTemp;
            entity.TempUnitId = vm.TempUnitId;

            // Cargo Characteristics
            entity.IsStackable = vm.IsStackable;
            entity.IsRotateable = vm.IsRotateable;
            entity.IsPerishable = vm.IsPerishable;
            entity.IsFragile = vm.IsFragile;

            // Texts
            entity.CargoDescription = vm.CargoDescription ?? string.Empty;
            entity.MarksAndNumbers = vm.MarksAndNumbers;
            entity.Remarks = vm.Remarks;
        }

        private static CargoDetailVm MapCargoDetailToVm(CargoDetail entity)
        {
            return new CargoDetailVm
            {
                Id = entity.Id,
                BLId = entity.BLId,

                Sequence = entity.Sequence,

                PackageTypeId = entity.PackageTypeId,
                PackageCount = entity.PackageCount,

                GrossWeight = entity.GrossWeight,
                NetWeight = entity.NetWeight,
                ChargeableWeight = entity.ChargeableWeight,
                WeightUnitId = entity.WeightUnitId,

                Volume = entity.Volume,
                VolumeUnitId = entity.VolumeUnitId,

                IsOverSized = entity.IsOverSized,
                Length = entity.Length,
                Width = entity.Width,
                Height = entity.Height,
                DimensionUnitId = entity.DimensionUnitId,

                CountryHSCodeId = entity.CountryHSCodeId,
                HSCodeToSave = entity.HSCodeToSave,
                CargoCommodityId = entity.CargoCommodityId,

                IsDG = entity.IsDG,
                DGClassId = entity.DGClassId,
                UNNumber = entity.UNNumber,
                DGFlashPoint = entity.DGFlashPoint,
                DGPackingGroupId = entity.DGPackingGroupId,

                IsTemperatureControlled = entity.IsTemperatureControlled,
                MinTemp = entity.MinTemp,
                NormalTemp = entity.NormalTemp,
                MaxTemp = entity.MaxTemp,
                TempUnitId = entity.TempUnitId,

                IsStackable = entity.IsStackable,
                IsRotateable = entity.IsRotateable,
                IsPerishable = entity.IsPerishable,
                IsFragile = entity.IsFragile,

                CargoDescription = entity.CargoDescription ?? string.Empty,
                MarksAndNumbers = entity.MarksAndNumbers,
                Remarks = entity.Remarks,

                PackageTypeName = entity.PackageType?.PackageTypeName,
                WeightUnitName = entity.WeightUnit?.MeasurementUnitName,
                VolumeUnitName = entity.VolumeUnit?.MeasurementUnitName,
                DimensionUnitName = entity.DimensionUnit?.MeasurementUnitName,
                TempUnitName = entity.TempUnit?.MeasurementUnitName,
                DGClassName = entity.DGClass?.DGClassName,
                DGPackingGroupName = entity.DGPackingGroup?.DGPackingGroupCode,
                CargoCommodityName = entity.CargoCommodity?.CargoCommodityName,
                CountryHSCodeText = entity.CountryHSCode?.CHSCode
            };
        }



        private static bool HasCargoDetailValue(CargoDetailVm x)
        {
            return
                x.PackageTypeId.HasValue ||
                x.PackageCount.HasValue ||

                x.GrossWeight.HasValue ||
                x.NetWeight.HasValue ||
                x.ChargeableWeight.HasValue ||
                x.WeightUnitId.HasValue ||

                x.Volume.HasValue ||
                x.VolumeUnitId.HasValue ||

                x.IsOverSized ||
                x.Length.HasValue ||
                x.Width.HasValue ||
                x.Height.HasValue ||
                x.DimensionUnitId.HasValue ||

                x.CountryHSCodeId.HasValue ||
                !string.IsNullOrWhiteSpace(x.HSCodeToSave) ||
                x.CargoCommodityId.HasValue ||

                x.IsDG ||
                x.DGClassId.HasValue ||
                !string.IsNullOrWhiteSpace(x.UNNumber) ||
                x.DGFlashPoint.HasValue ||
                x.DGPackingGroupId.HasValue ||

                x.IsTemperatureControlled ||
                x.MinTemp.HasValue ||
                x.NormalTemp.HasValue ||
                x.MaxTemp.HasValue ||
                x.TempUnitId.HasValue ||

                x.IsStackable ||
                x.IsRotateable ||
                x.IsPerishable ||
                x.IsFragile ||

                !string.IsNullOrWhiteSpace(x.CargoDescription) ||
                !string.IsNullOrWhiteSpace(x.MarksAndNumbers) ||
                !string.IsNullOrWhiteSpace(x.Remarks);
        }

        private void SyncCargoDetails(BL bl, TransportDocumentUpsertVm vm)
        {
            bl.CargoDetails ??= new List<CargoDetail>();

            var existingItems = bl.CargoDetails
                .Where(x => x.BLId == bl.Id)
                .ToList();

            var incomingItems = vm.CargoDetails?
                .Where(HasCargoDetailValue)
                .OrderBy(x => x.Sequence)
                .ToList()
                ?? new List<CargoDetailVm>();

            var incomingIds = incomingItems
                .Where(x => x.Id.HasValue && x.Id.Value != Guid.Empty)
                .Select(x => x.Id!.Value)
                .ToHashSet();

            var toDelete = existingItems
                .Where(x => !incomingIds.Contains(x.Id))
                .ToList();

            foreach (var entity in toDelete)
            {
                bl.CargoDetails.Remove(entity);
            }

            for (var i = 0; i < incomingItems.Count; i++)
            {
                var item = incomingItems[i];

                if (item.Sequence <= 0)
                    item.Sequence = i + 1;

                CargoDetail entity;

                if (item.Id.HasValue && item.Id.Value != Guid.Empty)
                {
                    entity = existingItems.FirstOrDefault(x => x.Id == item.Id.Value);

                    if (entity == null)
                    {
                        entity = new CargoDetail
                        {
                            Id = item.Id.Value
                        };

                        bl.CargoDetails.Add(entity);
                    }
                }
                else
                {
                    entity = new CargoDetail
                    {
                        Id = Guid.CreateVersion7()
                    };

                    bl.CargoDetails.Add(entity);
                }

                MapCargoDetailVmToEntity(item, entity, bl.Id);
            }
        }
        private IQueryable<BL> ApplyContextFilter(IQueryable<BL> query, BLContextDto context)
        {
            if (context.JobId.HasValue)
                return query.Where(x => x.JobId == context.JobId.Value);

            if (context.BookingId.HasValue)
                return query.Where(x => x.BookingId == context.BookingId.Value);

            if (context.ShipmentId.HasValue)
                return query.Where(x => x.ShipmentId == context.ShipmentId.Value);

            if (context.ShipmentLegId.HasValue)
                return query.Where(x => x.ShipmentLegId == context.ShipmentLegId.Value);

            return query.Where(x => false);
        }

        public async Task<string?> GetPartyTextInBLAsync(Guid partyId)
        {
            var party = await _db.Persons.FindAsync(partyId);
            if (party == null) return "";
            var result = "";
            result = party.PersonName;
            if (party.Address != null && party.Address.Length > 0) result += Environment.NewLine + party.Address;
            if (party.Phone != null && party.Phone.Length > 0) result += Environment.NewLine + "Tel:" + party.Phone;
            return result;
        }

        public async Task<AdvancedBLSplitVm?> GetAdvancedSplitWorkspaceAsync(Guid blId, Guid jobId)
        {
            var bl = await _db.BLs
                .AsNoTracking()
                .Include(x => x.Job)
                .Include(x => x.BLContainers)
                    .ThenInclude(c => c.CargoAllocations)
                        .ThenInclude(a => a.CargoDetail)
                .Include(x => x.CargoDetails)
                .FirstOrDefaultAsync(x => x.Id == blId && x.JobId == jobId);

            if (bl == null)
            {
                return null;
            }

            var sourceLines = new List<BLSplitSourceLineVm>();

            var hasContainerAllocations = bl.BLContainers != null
                && bl.BLContainers.Any(c => c.CargoAllocations != null && c.CargoAllocations.Any());

            if (hasContainerAllocations)
            {
                sourceLines.AddRange(
                    bl.BLContainers
                        .Where(c => c.CargoAllocations != null && c.CargoAllocations.Any())
                        .SelectMany(container => container.CargoAllocations.Select(allocation => new BLSplitSourceLineVm
                        {
                            SourceLineKey = $"ALLOC:{allocation.Id}",
                            SourceLineType = BLSplitSourceLineType.ContainerAllocation,

                            SourceAllocationId = allocation.Id,
                            SourceCargoDetailId = allocation.CargoDetailId,

                            BlContainerId = container.Id,
                            ContainerNo = container.ContainerNumber ?? string.Empty,

                            CargoNo = allocation.CargoDetail != null
                                ? allocation.CargoDetail.Sequence.ToString()
                                : string.Empty,
                            Description = allocation.CargoDetail != null
                                ? allocation.CargoDetail.CargoDescription ?? string.Empty
                                : string.Empty,

                            AvailablePackages = allocation.PackageCount ?? 0,
                            AvailableWeight = allocation.GrossWeight ?? 0,
                            AvailableVolume = allocation.Volume ?? 0
                        }))
                );
            }
            else
            {
                sourceLines.AddRange(
                    bl.CargoDetails.Select(cargo => new BLSplitSourceLineVm
                    {
                        SourceLineKey = $"CARGO:{cargo.Id}",
                        SourceLineType = BLSplitSourceLineType.DirectCargo,

                        SourceAllocationId = null,
                        SourceCargoDetailId = cargo.Id,

                        BlContainerId = null,
                        ContainerNo = string.Empty,

                        CargoNo = cargo.Sequence.ToString(),
                        Description = cargo.CargoDescription ?? string.Empty,

                        AvailablePackages = cargo.PackageCount ?? 0,
                        AvailableWeight = cargo.GrossWeight ?? 0,
                        AvailableVolume = cargo.Volume ?? 0
                    })
                );
            }

            sourceLines = sourceLines
                .OrderBy(x => x.ContainerNo)
                .ThenBy(x => x.CargoNo)
                .ToList();

            return new AdvancedBLSplitVm
            {
                BlId = bl.Id,
                BlNo = bl.BLNo ?? string.Empty,
                JobId = bl.JobId,
                JobNo = bl.Job?.JobNumber ?? string.Empty,
                IsContainerized = hasContainerAllocations,
                SourceLines = sourceLines,
                Targets = new List<BLSplitTargetVm>(),
                Totals = new BLSplitTotalsVm
                {
                    Packages = sourceLines.Sum(x => x.AvailablePackages),
                    Weight = sourceLines.Sum(x => x.AvailableWeight),
                    Volume = sourceLines.Sum(x => x.AvailableVolume)
                }
            };
        }





        public async Task<ServiceResult<Guid>> SubmitAdvancedSplitAsync(SubmitAdvancedBLSplitRequestVm request)
        {
            if (request == null)
            {
                return ServiceResult<Guid>.Failed("Request is null.");
            }

            if (request.SourceBlId == Guid.Empty)
            {
                return ServiceResult<Guid>.Failed("Source BL is required.");
            }

            if (request.JobId == Guid.Empty)
            {
                return ServiceResult<Guid>.Failed("JobId is required.");
            }

            if (request.Targets == null || !request.Targets.Any())
            {
                return ServiceResult<Guid>.Failed("At least one target is required.");
            }

            var sourceBl = await _db.BLs
                .Include(x => x.BLContainers)
                    .ThenInclude(c => c.CargoAllocations)
                .Include(x => x.CargoDetails)
                .FirstOrDefaultAsync(x => x.Id == request.SourceBlId && x.JobId == request.JobId);

            if (sourceBl == null)
            {
                return ServiceResult<Guid>.Failed("Source BL not found.");
            }

            var allAssignments = request.Targets
                .Where(t => t.Assignments != null)
                .SelectMany(t => t.Assignments.Select(a => new
                {
                    t.TargetKey,
                    t.TargetBlNo,
                    Assignment = a
                }))
                .ToList();

            if (!allAssignments.Any())
            {
                return ServiceResult<Guid>.Failed("No assignments submitted.");
            }

            var allocationIds = allAssignments
                .Where(x => x.Assignment.SourceLineType == BLSplitSourceLineType.ContainerAllocation)
                .Select(x => x.Assignment.SourceAllocationId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var cargoIds = allAssignments
                .Where(x => x.Assignment.SourceLineType == BLSplitSourceLineType.DirectCargo)
                .Select(x => x.Assignment.SourceCargoDetailId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var sourceAllocations = await _db.BLContainerCargoAllocations
                .Include(x => x.BLContainer)
                .Include(x => x.CargoDetail)
                .Where(x => allocationIds.Contains(x.Id))
                .ToListAsync();

            var sourceCargoDetails = await _db.CargoDetails
                .Where(x => cargoIds.Contains(x.Id))
                .ToListAsync();

            foreach (var allocation in sourceAllocations)
            {
                var assignedPackages = allAssignments
                    .Where(x => x.Assignment.SourceAllocationId == allocation.Id)
                    .Sum(x => x.Assignment.Packages);

                var assignedWeight = allAssignments
                    .Where(x => x.Assignment.SourceAllocationId == allocation.Id)
                    .Sum(x => x.Assignment.Weight);

                var assignedVolume = allAssignments
                    .Where(x => x.Assignment.SourceAllocationId == allocation.Id)
                    .Sum(x => x.Assignment.Volume);

                if (assignedPackages > (allocation.PackageCount ?? 0) ||
                    assignedWeight > (allocation.GrossWeight ?? 0) ||
                    assignedVolume > (allocation.Volume ?? 0))
                {
                    return ServiceResult<Guid>.Failed($"Assigned values exceed balance for allocation {allocation.Id}.");
                }
            }

            foreach (var cargo in sourceCargoDetails)
            {
                var assignedPackages = allAssignments
                    .Where(x => x.Assignment.SourceCargoDetailId == cargo.Id)
                    .Sum(x => x.Assignment.Packages);

                var assignedWeight = allAssignments
                    .Where(x => x.Assignment.SourceCargoDetailId == cargo.Id)
                    .Sum(x => x.Assignment.Weight);

                var assignedVolume = allAssignments
                    .Where(x => x.Assignment.SourceCargoDetailId == cargo.Id)
                    .Sum(x => x.Assignment.Volume);

                if (assignedPackages > (cargo.PackageCount ?? 0) ||
                    assignedWeight > (cargo.GrossWeight ?? 0) ||
                    assignedVolume > (cargo.Volume ?? 0))
                {
                    return ServiceResult<Guid>.Failed($"Assigned values exceed balance for cargo {cargo.Id}.");
                }
            }

            var targetBlMap = new Dictionary<string, BL>();

            foreach (var target in request.Targets)
            {
                BL targetBl;

                if (!string.IsNullOrWhiteSpace(target.TargetBlNo))
                {
                    targetBl = await _db.BLs
                        .Include(x => x.BLContainers)
                        .Include(x => x.CargoDetails)
                        .FirstOrDefaultAsync(x => x.BLNo == target.TargetBlNo && x.JobId == request.JobId);

                    if (targetBl == null)
                    {
                        return ServiceResult<Guid>.Failed($"Target BL '{target.TargetBlNo}' not found.");
                    }
                }
                else
                {
                    targetBl = new BL
                    {
                        Id = Guid.CreateVersion7(),
                        JobId = request.JobId,
                        BLNo = await GenerateNextBlNoAsync(request.JobId)
                    };

                    _db.BLs.Add(targetBl);
                }

                targetBlMap[target.TargetKey] = targetBl;
            }

            foreach (var target in request.Targets)
            {
                var targetBl = targetBlMap[target.TargetKey];

                foreach (var item in target.Assignments.Where(a => a.Packages > 0 || a.Weight > 0 || a.Volume > 0))
                {
                    if (item.SourceLineType == BLSplitSourceLineType.ContainerAllocation)
                    {
                        var sourceAllocation = sourceAllocations.First(x => x.Id == item.SourceAllocationId!.Value);
                        var sourceContainer = sourceAllocation.BLContainer;

                        var targetContainer = await EnsureTargetContainerAsync(targetBl, sourceContainer);

                        var newAllocation = new BLContainerCargoAllocation
                        {
                            Id = Guid.CreateVersion7(),
                            BLContainerId = targetContainer.Id,
                            CargoDetailId = sourceAllocation.CargoDetailId,
                            PackageCount = item.Packages,
                            GrossWeight = item.Weight,
                            Volume = item.Volume
                        };

                        _db.BLContainerCargoAllocations.Add(newAllocation);

                        sourceAllocation.PackageCount = (sourceAllocation.PackageCount ?? 0) - item.Packages;
                        sourceAllocation.GrossWeight = (sourceAllocation.GrossWeight ?? 0) - item.Weight;
                        sourceAllocation.Volume = (sourceAllocation.Volume ?? 0) - item.Volume;
                    }
                    else if (item.SourceLineType == BLSplitSourceLineType.DirectCargo)
                    {
                        var sourceCargo = sourceCargoDetails.First(x => x.Id == item.SourceCargoDetailId!.Value);

                        var newCargo = new CargoDetail
                        {
                            Id = Guid.CreateVersion7(),
                            BLId = targetBl.Id,
                            Sequence = sourceCargo.Sequence,
                            CargoDescription = sourceCargo.CargoDescription,
                            PackageCount = item.Packages,
                            GrossWeight = item.Weight,
                            Volume = item.Volume
                        };

                        _db.CargoDetails.Add(newCargo);

                        sourceCargo.PackageCount = (sourceCargo.PackageCount ?? 0) - item.Packages;
                        sourceCargo.GrossWeight = (sourceCargo.GrossWeight ?? 0) - item.Weight;
                        sourceCargo.Volume = (sourceCargo.Volume ?? 0) - item.Volume;
                    }
                }
            }

            var emptyAllocations = sourceAllocations
                .Where(x => (x.PackageCount ?? 0) == 0 && (x.GrossWeight ?? 0) == 0 && (x.Volume ?? 0) == 0)
                .ToList();

            if (emptyAllocations.Any())
            {
                _db.BLContainerCargoAllocations.RemoveRange(emptyAllocations);
            }

            var emptyCargoDetails = sourceCargoDetails
                .Where(x => (x.PackageCount ?? 0) == 0 && (x.GrossWeight ?? 0) == 0 && (x.Volume ?? 0) == 0)
                .ToList();

            if (emptyCargoDetails.Any())
            {
                _db.CargoDetails.RemoveRange(emptyCargoDetails);
            }

            await _db.SaveChangesAsync();

            return ServiceResult<Guid>.Success(sourceBl.Id);
        }






        private async Task<BLContainer> EnsureTargetContainerAsync(BL targetBl, BLContainer sourceContainer)
        {
            var containerNo = sourceContainer.ContainerNumber ?? string.Empty;

            var existingContainer = await _db.BLContainers
                .FirstOrDefaultAsync(x => x.BLId == targetBl.Id && x.ContainerNumber == containerNo);

            if (existingContainer != null)
            {
                return existingContainer;
            }

            var newContainer = new BLContainer
            {
                Id = Guid.CreateVersion7(),
                BLId = targetBl.Id,
                ContainerNumber = sourceContainer.ContainerNumber,
                ContainerSizeTypeId = sourceContainer.ContainerSizeTypeId,
                SealNumber = sourceContainer.SealNumber
            };

            _db.BLContainers.Add(newContainer);

            return newContainer;
        }





        private async Task<string> GenerateNextBlNoAsync(Guid jobId)
        {
            var count = await _db.BLs.CountAsync(x => x.JobId == jobId);
            return $"BL-{count + 1:000}";
        }

        public async Task<List<BLLookupItemVm>> SearchBLsAsync(string? term)
        {
            term = term?.Trim();
            var query = _db.BLs
                .AsNoTracking()
                .Where(x => !x.IsDeleted);


            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.BLNo.Contains(term) ||
                    x.MasterBLNo.Contains(term));
            }

            return await query
                .OrderBy(x => x.BLNo)
                .Take(20)
                .Select(x => new BLLookupItemVm
                {
                    Id = x.Id,
                    DisplayName = "H:" + x.BLNo + "-  M:" + x.MasterBLNo + ")",
                })
                .ToListAsync();
        }


        public async Task<ContainerAllocationModalVm> GetContainerAllocationModalAsync(
            Guid blId,
            Guid roadTripId,
            CancellationToken cancellationToken = default)
        {
            // ۱. واکشی اطلاعات سفر جاری
            var trip = await _db.BLTrips
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == roadTripId && t.BLId == blId, cancellationToken);

            if (trip == null)
            {
                throw new KeyNotFoundException($"Road trip with ID '{roadTripId}' was not found for BL '{blId}'.");
            }

            // ۲. واکشی لیست کانتینرهای مربوط به این بارنامه
            var cargoDetails = await _db.BLContainers
                .AsNoTracking()
                .Include(c => c.ContainerSizeType)
                .Where(c => c.BLId == blId && !string.IsNullOrEmpty(c.ContainerNumber))
                .ToListAsync(cancellationToken);

            var containerIds = cargoDetails.Select(c => c.Id).ToList();

            // ۳. واکشی تمام تخصیص‌های کانتینرهای این بارنامه به همه‌ی سفرها
            var existingAllocations = await _db.BLContainerTripAllocations
                .AsNoTracking()
                .Include(a => a.BLTrip)
                .Where(a => containerIds.Contains(a.BLContainerId))
                .ToListAsync(cancellationToken);

            // ساخت یک دیکشنری از کانتینرها به تخصیص‌های مربوطه‌شان جهت پیمایش سریع
            var allocationLookup = existingAllocations
                .GroupBy(a => a.BLContainerId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ۴. نگاشت به ViewModel
            var containerItems = cargoDetails.Select(c =>
            {
                // دریافت لیست تخصیص‌های این کانتینر (در صورت وجود)
                var allocations = allocationLookup.GetValueOrDefault(c.Id) ?? new List<BLContainerTripAllocation>();

                // بررسی اینکه آیا به همین سفر تخصیص داده شده است؟
                bool isAllocatedToThisTrip = allocations.Any(a => a.BLTripId == roadTripId);

                // یافتن رکوردی که به سفر دیگری تخصیص یافته است (جهت نمایش نام/شماره کامیون دیگر)
                var otherAllocation = allocations.FirstOrDefault(a => a.BLTripId != roadTripId);
                string? allocatedToOtherTruck = otherAllocation?.BLTrip?.TruckNo;

                return new ContainerAllocationItemVm
                {
                    ContainerId = c.Id,
                    ContainerNo = c.ContainerNumber ?? string.Empty,
                    ContainerTypeName = c.ContainerSizeType?.ShortName,
                    GrossWeight = c.TotalGrossWeight ?? 0,
                    PackageCount = c.TotalPackageCount,
                    IsAllocatedToThisTrip = isAllocatedToThisTrip,
                    AllocatedToOtherTruck = allocatedToOtherTruck
                };
            }).ToList();

            return new ContainerAllocationModalVm
            {
                BLId = blId,
                RoadTripId = roadTripId,
                TruckNo = trip.TruckNo,
                DriverName = trip.DriverName,
                AvailableContainers = containerItems
            };
        }

        public async Task<ServiceResult<bool>> SaveContainerAllocationAsync(
            SaveContainerAllocationDto dto,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId;

            // ۱. بررسی وجود کانتینر برای این بارنامه
            var containerIds = await _db.BLContainers
                .AsNoTracking()
                .Where(c => c.BLId == dto.BLId)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            if (!containerIds.Any())
            {
                return ServiceResult<bool>.Failed("No containers found for the specified BL.");
            }

            var selectedIdsSet = dto.SelectedContainerIds?.ToHashSet() ?? new HashSet<Guid>();

            // ۲. واکشی تخصیص‌های فعلی برای این سفر خاص (RoadTripId)
            var currentTripAllocations = await _db.BLContainerTripAllocations
                .Where(a => a.BLTripId == dto.RoadTripId && containerIds.Contains(a.BLContainerId))
                .ToListAsync(cancellationToken);

            var existingAllocatedContainerIds = currentTripAllocations.Select(a => a.BLContainerId).ToHashSet();

            // ۳. حذف کانتینرهایی که قبلاً تخصیص داشتند اما اکنون تیک آن‌ها برداشته شده است
            var allocationsToRemove = currentTripAllocations
                .Where(a => !selectedIdsSet.Contains(a.BLContainerId))
                .ToList();

            if (allocationsToRemove.Any())
            {
                _db.BLContainerTripAllocations.RemoveRange(allocationsToRemove);
            }

            // ۴. افزودن تخصیص‌های جدید
            var newContainerIdsToAllocate = selectedIdsSet
                .Where(id => containerIds.Contains(id) && !existingAllocatedContainerIds.Contains(id))
                .ToList();

            foreach (var containerId in newContainerIdsToAllocate)
            {
                var allocation = new BLContainerTripAllocation
                {
                    Id = Guid.CreateVersion7(),
                    BLContainerId = containerId,
                    BLTripId = dto.RoadTripId,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId
                };

                await _db.BLContainerTripAllocations.AddAsync(allocation, cancellationToken);
            }
            // ۵. ثبت تغییرات در دیتابیس
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<bool>.Success(true, "Container allocation updated successfully.");
        }
        public async Task<Guid?> GetPODTariffBookIdAsync(Guid blId)
        {
            return await _db.BLs
                .Where(x => x.Id == blId)
                .Select(x => x.PODDetentionTariffId)
                .FirstOrDefaultAsync();
        }
        public async Task<Guid?> GetInvoiceClientIdAsync(Guid blId)
        {
            var partyId = await _db.BLs
                 .Where(bl => bl.Id == blId)
                 .SelectMany(bl => bl.Job.Parties)
                 .Where(p => p.RoleTypeId == 2)
                 .Select(p => (Guid?)p.PartyId)
                 .FirstOrDefaultAsync();
            return partyId;
        }
        public async Task<bool> BLHasCntAsync(Guid blId)
        {
            if (!await _db.BLContainers.Where(x => x.BLId == blId && !x.IsDeleted).AnyAsync()) return false;
            return true;
        }

        public async Task<Guid> GetFinalPortIdForBLAsync(Guid blId)
        {
            var bl = await _db.BLs
                .Include(x => x.PODel)
                .Include(x => x.POD)
                .Where(x => x.Id == blId).FirstOrDefaultAsync();

            if (bl.PODel != null)
            {
                if (bl.PODel.CityTypeId == (int)EnumCityType.Port || bl.PODel.CityTypeId == (int)EnumCityType.CityAndPort)
                {
                    return bl.PODelId.Value;
                }
            }
            if (bl.POD != null)
            {
                if (bl.POD.CityTypeId == (int)EnumCityType.Port || bl.POD.CityTypeId == (int)EnumCityType.CityAndPort)
                {
                    return bl.PODId.Value;
                }
            }
            return Guid.Empty;
        }
        public async Task<FinalVoyageVm> GetFinalVoyageForBLAsync(Guid blId)
        {
            var vm = new FinalVoyageVm();
            vm.BLId = blId;
            vm.Message = "";
            var blVoyage = await _db.BLVoyages
                .Include(x => x.Voyage)
                    .ThenInclude(x => x.Vessel)
                .Where(x => x.BLId == blId && !x.IsDeleted && x.IsFinal == true).FirstOrDefaultAsync();
            if (blVoyage == null)
            {
                vm.Message = "No Final Voyage Defined for BL!!!!!";
                return vm;
            }
            vm.BLVoyageId = blVoyage.Id;
            vm.VoyageId = blVoyage.VoyageId;
            vm.VoyageNo = blVoyage.Voyage.VoyageNo;
            vm.VesselName = blVoyage.Voyage?.Vessel?.VesselName;
            var FinalPortId = await GetFinalPortIdForBLAsync(blId);
            if (FinalPortId == Guid.Empty)
            {
                vm.Message = "Final Port Not Defined in BL's PODel Or POD!!!!!";
                return vm;
            }
            vm.PortId = FinalPortId;
            var p = await _db.Locations.FindAsync(vm.PortId);
            vm.PortName = p.LocationName;

            var voyagePort = await _db.VoyagePorts.Where(x => x.InboundVoyageId == blVoyage.VoyageId && x.PortId == FinalPortId).FirstOrDefaultAsync();
            if (voyagePort == null)
            {
                vm.Message = $"Final Port is not defined in Calling Ports For The Voyage-{blVoyage.VoyageNo}";
                return vm;
            }
            var events = await _db.VoyagePortEvents.Where(x => x.VoyagePortId == voyagePort.Id && !x.IsDeleted).ToListAsync();
            if (events == null)
            {
                vm.Message = $"No Events Defined For Voyage {blVoyage.VoyageNo} in Port {voyagePort.Port.LocationName}";
                return vm;
            }

            var arriveEvent = events.Where(x => x.EventTypeId == 105).FirstOrDefault();
            if (arriveEvent == null)
            {
                vm.Message = $"No Arrival Event Defined for Voyage {blVoyage.VoyageNo} in Port {voyagePort.Port.LocationName}";
                return vm;
            }
            vm.ArrivalDate = arriveEvent.VoyagePortEventDateTime;

            var berthEvent = events.Where(x => x.EventTypeId == 110).FirstOrDefault();
            if (berthEvent == null)
            {
                vm.Message = $"No Berth Event Defined for Voyage {blVoyage.VoyageNo} in Port {voyagePort.Port.LocationName}";
                return vm;
            }
            vm.BerthDate = berthEvent.VoyagePortEventDateTime;

            var dischargeStartedEvent = events.Where(x => x.EventTypeId == 116).FirstOrDefault();
            if (dischargeStartedEvent == null)
            {
                vm.Message = $"No Discharge Started Event Defined for Voyage {blVoyage.VoyageNo} in Port {voyagePort.Port.LocationName}";
                return vm;
            }
            vm.DischargeStartDate = dischargeStartedEvent.VoyagePortEventDateTime;

            var dischargeCompletedEvent = events.Where(x => x.EventTypeId == 121).FirstOrDefault();
            if (dischargeCompletedEvent == null)
            {
                vm.Message = $"No Discharge Completed Event Defined for Voyage {blVoyage.VoyageNo} in Port {voyagePort.Port.LocationName}";
                return vm;
            }
            vm.DischargeCompleteDate = dischargeCompletedEvent.VoyagePortEventDateTime;
            return vm;
        }
    }
}
