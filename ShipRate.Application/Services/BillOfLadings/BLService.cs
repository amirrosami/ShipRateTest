using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Interfaces.BillOfLadings;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.BillOfLadings
{
    public class BLService : IBLService
    {
        private readonly ApplicationDbContext _db;

        // Replace these ids with your real transport type ids
        private const int SeaTransportTypeId = 1;
        private const int RoadTransportTypeId = 2;
        private const int RailTransportTypeId = 3;
        private const int AirTransportTypeId = 4;

        public BLService(ApplicationDbContext db)
        {
            _db = db;
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

        public async Task<BLUpsertDto> GetCreateModelAsync(BLContextDto context)
        {
            var model = new BLUpsertDto
            {
                JobId = context.JobId,
                BookingId = context.BookingId,
                ShipmentId = context.ShipmentId,
                ShipmentLegId = context.ShipmentLegId,
                BLIssueDate = DateTime.Today,
                IssuedByUs = true,
                IsContainerized = false,
                Parties = new List<BLPartyDto>
                {
                    new BLPartyDto {RoleTypeId=51 },
                    new BLPartyDto {RoleTypeId=52 },
                    new BLPartyDto {RoleTypeId=53 }
                },
                Containers = new List<BLContainerDto>
                {
                    new BLContainerDto
                    {
                        CargoDetails = new List<CargoDetailDto>
                        {
                            new CargoDetailDto { Sequence = 1 },
                        }
                    }
                },
                CargoDetails = new List<CargoDetailDto>
                {
                    new CargoDetailDto { Sequence = 1 },
                },
                Voyages = new List<BLVoyageDto> { new BLVoyageDto() },
                Flights = new List<BLFlightDto> { new BLFlightDto() },
                Trips = new List<BLTripDto> { new BLTripDto() },
                RailJourneys = new List<BLRailJourneyDto> { new BLRailJourneyDto() }
            };
            return await Task.FromResult(model);
        }

        public async Task<BLUpsertDto> GetEditModelAsync(Guid id)
        {
            var bl = await _db.BLs
                .AsNoTracking()
                .Include(x => x.BLParties.Where(c => !c.IsDeleted))
                .Include(x => x.BLContainers.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.CargoAllocations.Where(cd => !cd.IsDeleted))
                .Include(x => x.CargoDetails.Where(c => !c.IsDeleted))
                .Include(x => x.BLVoyages.Where(c => !c.IsDeleted))
                .Include(x => x.BLFlights.Where(c => !c.IsDeleted))
                .Include(x => x.BLTrips.Where(c => !c.IsDeleted))
                .Include(x => x.BLRailJourneys.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (bl == null)
                throw new Exception("BL not found.");

            var dto = new BLUpsertDto
            {
                Id = bl.Id,

                JobId = bl.JobId,
                BookingId = bl.BookingId,
                ShipmentId = bl.ShipmentId,
                ShipmentLegId = bl.ShipmentLegId,

                CompanyId = bl.CompanyId,
                BLLevelId = bl.BLLevelId,
                TransportTypeId = bl.TransportTypeId,

                BLNo = bl.BLNo,
                BLIssueDate = bl.BLIssueDate.Value,

                CarrierId = bl.CarrierId,
                BLTypeId = bl.BLTypeId,

                OriginalCount = bl.OriginalCount.Value,
                CopyCount = bl.CopyCount.Value,

                LoadTypeId = bl.LoadTypeId.Value,
                MovementTypeId = bl.MovementTypeId.Value,
                BLStatusId = bl.BLStatusId,
                PaymentTypeId = bl.PaymentTypeId.Value,

                IssuedByUs = bl.IssuedByUs,
                IsSwitched = bl.IsSwitched,
                //IsContainerized = bl.IsContainerized,

                IssuerPartyId = bl.IssuerPartyId,

                PORId = bl.PORId.Value,
                PORText = bl.PORText,
                POLId = bl.POLId.Value,
                POLText = bl.POLText,
                PODId = bl.PODId.Value,
                PODText = bl.PODText,
                PODelId = bl.PODelId.Value,
                PODelText = bl.PODelText,

                FinalDestinationText = bl.FinalDestinationText,
                FreightPrepaidATId = bl.FreightPaymentAtId,
                FreightPrepaidATText = bl.FreightPaymentAtText,
                PointandCountryOfOriginId = bl.PointandCountryOfOriginId,
                PointandCountryOfOriginText = bl.PointandCountryOfOriginText,
                OnwardInlandRoutingId = bl.OnwardInlandRoutingId,
                OnwardInlandRoutingText = bl.OnwardInlandRoutingText,

                GoodsDescription = bl.GoodsDescription,
                MarksAndNumbers = bl.MarksAndNumbers,
                RemarksInBL = bl.RemarksInBL,
                GrossWeightKg = bl.GrossWeightKg,
                NetWeightKg = bl.NetWeightKg,
                VolumeCBM = bl.VolumeCBM,
                ChargeableWeight = bl.ChargeableWeight,
                TotalContainers = bl.TotalContainers,
                TotalPackages = bl.TotalPackages,
                PackageTypeId = bl.PackageTypeId,
                PackageTypeText = bl.PackageTypeText,

                ExportReference = bl.ExportReference,
                ForwardingAgentReference = bl.ForwardingAgentReference,
                SerialNo = bl.SerialNo,
                MRN = bl.MRN,
                PursuitCode = bl.PursuitCode,
                FreightTerms = bl.FreightTerms,
                LCNo = bl.LCNo,
                LCBankName = bl.LCBankName,
                OrderRegistrationNo = bl.OrderRegistrationNo,

                ShippedOnBoardDate = bl.ShippedOnBoardDate,
                ReceivedForShipmentDate = bl.ReceivedForShipmentDate,
                ReleaseDate = bl.ReleaseDate,

                BLReleaseTypeId = bl.BLReleaseTypeId,
                SalesPersonId = bl.SalesPersonId,
                TranshipmentPortId = bl.TranshipmentPortId,

                ClauseRemarks = bl.ClauseRemarks,
                OnBoardNotation = bl.OnBoardNotation,
                RemarksInManifest = bl.RemarksInManifest,
                InternalNotes = bl.InternalNotes,

                Parties = bl.BLParties
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new BLPartyDto
                    {
                        Id = x.Id,
                        RoleTypeId = x.RoleTypeId,
                        PartyId = x.PartyId,
                        PartyTextInBL = x.PartyTextInBL,
                        PartyNationalNo = x.PartyNationalNo,
                        PartyTelephone = x.PartyTelephone,
                        PartyMobile = x.PartyMobile,
                        PartyFax = x.PartyFax,
                        PartyEmail = x.PartyEmail,
                        IsConsigneeToOrder = x.IsConsigneeToOrder,
                        IsConsigneeToOrderOf = x.IsConsigneeToOrderOf,
                        IsConsigneeToBearer = x.IsConsigneeToBearer,
                        IsNotifySameAs = x.IsNotifySameAs
                    })
                    .ToList(),

                Containers = bl.BLContainers
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new BLContainerDto
                    {
                        Id = x.Id,
                        ContainerUseId = x.ContainerUseId.Value,
                        ContainerNumber = x.ContainerNumber,
                        ContainerSizeTypeId = x.ContainerSizeTypeId.Value,
                        TotalPackageCount = x.TotalPackageCount,
                        SealNumber = x.SealNumber,
                        SealNumber2 = x.SealNumber2,
                        TotalVolumeCBM = x.TotalVolumeCBM,
                        BLContainerPercent = x.BLContainerPercent,
                        TotalGrossWeight = x.TotalGrossWeight,
                        TotalNetWeight = x.TotalNetWeight,
                        DimensionUnitId = x.DimensionUnitId,
                        OverHeight = x.OverHeight,
                        OverLength = x.OverLength,
                        OverWidth = x.OverWidth
                        //,
                        //CargoDetails = x.CargoDetails
                        //    .OrderBy(cd => cd.Sequence)
                        //    .ThenBy(cd => cd.CreatedAt)
                        //    .Select(cd => new CargoDetailDto
                        //    {
                        //        Id = cd.Id,
                        //        Sequence = cd.Sequence,
                        //        PackageTypeId = cd.PackageTypeId,
                        //        PackageCount = (int)cd.PackageCount.Value,
                        //        GrossWeight = cd.GrossWeight,
                        //        NetWeight = cd.NetWeight,
                        //        Volume = cd.Volume,
                        //        CargoDescription = cd.CargoDescription,
                        //        MarksAndNumbers = cd.MarksAndNumbers,
                        //        HSCodeToSave = cd.HSCodeToSave,
                        //        Remarks = cd.Remarks,
                        //        IsDG = cd.IsDG,
                        //        IsOverSized = cd.IsOverSized,
                        //        IsTemperatureControlled = cd.IsTemperatureControlled,
                        //        IsFragile = cd.IsFragile,
                        //        IsPerishable = cd.IsPerishable,
                        //        IsStackable = cd.IsStackable
                        //    })
                        //    .ToList()
                    })
                    .ToList(),

                CargoDetails = bl.CargoDetails
                    .OrderBy(x => x.Sequence)
                    .ThenBy(x => x.CreatedAt)
                    .Select(x => new CargoDetailDto
                    {
                        Id = x.Id,
                        Sequence = x.Sequence,
                        PackageTypeId = x.PackageTypeId,
                        PackageCount = (int)x.PackageCount.Value,
                        GrossWeight = x.GrossWeight,
                        NetWeight = x.NetWeight,
                        Volume = x.Volume,
                        CargoDescription = x.CargoDescription,
                        MarksAndNumbers = x.MarksAndNumbers,
                        HSCodeToSave = x.HSCodeToSave,
                        Remarks = x.Remarks,
                        IsDG = x.IsDG,
                        IsOverSized = x.IsOverSized,
                        IsTemperatureControlled = x.IsTemperatureControlled,
                        IsFragile = x.IsFragile,
                        IsPerishable = x.IsPerishable,
                        IsStackable = x.IsStackable
                    })
                    .ToList(),

                Voyages = bl.BLVoyages
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new BLVoyageDto
                    {
                        Id = x.Id,
                        VoyageTypeId = x.VoyageTypeId,
                        VoyageId = x.VoyageId,
                        VoyageNo = x.VoyageNo,
                        VesselName = x.VesselName,
                        ReceivedForShipmentDate = x.ReceivedForShipmentDate,
                        ShippedOnBoardDate = x.ShippedOnBoardDate
                    })
                    .ToList(),

                Flights = bl.BLFlights
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new BLFlightDto
                    {
                        Id = x.Id,
                        FlightId = x.FlightId,
                        FlightDateId = x.FlightDateId,
                        FlightNo = x.FlightNo,
                        AirlineId = x.AirlineId,
                        DeclaredValueForCarriage = x.DeclaredValueForCarriage,
                        DeclaredValueForCustoms = x.DeclaredValueForCustoms,
                        FlightDateDate = x.FlightDateDate,
                        HandlingInformation = x.HandlingInformation
                    })
                    .ToList(),

                Trips = bl.BLTrips
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new BLTripDto
                    {
                        Id = x.Id,
                        TripId = x.TripId,
                        LoadingDate = x.LoadingDate,
                        DeliveryDate = x.DeliveryDate,
                        DriverName = x.DriverName,
                        DriverPassportNo = x.DriverPassportNo,
                        TruckNo = x.TruckNo,
                        TrailerNo = x.TrailerNo
                    })
                    .ToList(),

                RailJourneys = bl.BLRailJourneys
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new BLRailJourneyDto
                    {
                        Id = x.Id,
                        TrainNo = x.TrainNo,
                        WagonNo = x.WagonNo,
                        DepartureDate = x.DepartureDate,
                        ArrivalDate = x.ArrivalDate
                    })
                    .ToList()
            };

            EnsureMinimumRowsForEdit(dto);

            return dto;
        }

        public async Task<BLLookupDto> GetLookupsAsync()
        {
            return new BLLookupDto
            {
                Companies = await _db.Companies
                    .Include(x => x.Person)
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Person.PersonName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.Person.PersonName
                    })
                    .ToListAsync(),

                BLTypes = await _db.BLTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.BLTypeName
                    })
                    .ToListAsync(),

                BLLevels = await _db.BLLevels
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.BLLevelName
                    })
                    .ToListAsync(),

                TransportTypes = await _db.TransportTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.TransportTypeName
                    })
                    .ToListAsync(),

                LoadTypes = await _db.LoadTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.LoadTypeCode
                    })
                    .ToListAsync(),

                MovementTypes = await _db.MovementTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.MovementTypeCode
                    })
                    .ToListAsync(),

                BLStatuses = await _db.BLStatuses
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.BLStatusName
                    })
                    .ToListAsync(),

                PaymentTypes = await _db.PaymentTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.PaymentTypeName
                    })
                    .ToListAsync(),

                Locations = await _db.Locations
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.LocationName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.LocationName
                    })
                    .ToListAsync(),

                Persons = await _db.Persons
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.PersonName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.PersonName
                    })
                    .ToListAsync(),


                Carriers = await _db.Carriers
                    .Include(x => x.Person)
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Person.PersonName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.Person.PersonName
                    })
                    .ToListAsync(),

                IssueParties = await _db.Persons
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.PersonName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.PersonName
                    })
                    .ToListAsync(),



                BLPartyTypes = await _db.RoleTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.RoleTypeName
                    })
                    .ToListAsync(),

                Commodities = await _db.CargoCommodities
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.CargoCommodityName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.CargoCommodityName
                    })
                    .ToListAsync(),

                PackageTypes = await _db.PackageTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.PackageTypeName)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.PackageTypeName
                    })
                    .ToListAsync(),

                ContainerUses = await _db.ContainerUses
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.Id.ToString()
                    })
                    .ToListAsync(),

                ContainerSizeTypes = await _db.ContainerSizeTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.ShortName.ToString()
                    })
                    .ToListAsync(),

                VoyageTypes = await _db.VoyageTypes
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.VoyageTypeName
                    })
                    .ToListAsync(),

                Voyages = await _db.Voyages
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.VoyageNo)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.VoyageNo
                    })
                    .ToListAsync(),

                Trips = await _db.Trips
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.TripNumber)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id.ToString(),
                        Name = x.TripNumber
                    })
                    .ToListAsync()
            };
        }

        public async Task<(bool Success, string Message, Guid? Id)> SaveAsync(BLUpsertDto dto)
        {
            NormalizeDto(dto);
            NormalizeTransportSpecificSections(dto);

            var resolvedJobId = await ResolveJobIdAsync(dto);
            if (!resolvedJobId.HasValue || resolvedJobId.Value == Guid.Empty)
                return (false, "JobId could not be resolved.", null);

            dto.JobId = resolvedJobId.Value;
            dto.BLStatusId = -1;
            var validation = ValidateDto(dto);
            if (!validation.Success)
                return (false, validation.Message, null);

            if (!dto.Id.HasValue || dto.Id.Value == Guid.Empty)
                return await CreateAsync(dto);

            return await UpdateAsync(dto);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var bl = await _db.BLs
                .Include(x => x.BLParties)
                .Include(x => x.BLContainers)
                    .ThenInclude(c => c.CargoAllocations)
                .Include(x => x.CargoDetails)
                .Include(x => x.BLVoyages)
                .Include(x => x.BLFlights)
                .Include(x => x.BLTrips)
                .Include(x => x.BLRailJourneys)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (bl == null)
                return false;

            bl.IsDeleted = true;
            bl.BLDeletedAt = DateTime.Now;

            foreach (var item in bl.BLParties.Where(x => !x.IsDeleted))
                item.IsDeleted = true;

            foreach (var container in bl.BLContainers.Where(x => !x.IsDeleted))
            {
                container.IsDeleted = true;

                foreach (var cargo in container.CargoAllocations.Where(x => !x.IsDeleted))
                    cargo.IsDeleted = true;
            }

            foreach (var cargo in bl.CargoDetails.Where(x => !x.IsDeleted))
                cargo.IsDeleted = true;

            foreach (var item in bl.BLVoyages.Where(x => !x.IsDeleted))
                item.IsDeleted = true;

            foreach (var item in bl.BLFlights.Where(x => !x.IsDeleted))
                item.IsDeleted = true;

            foreach (var item in bl.BLTrips.Where(x => !x.IsDeleted))
                item.IsDeleted = true;

            foreach (var item in bl.BLRailJourneys.Where(x => !x.IsDeleted))
                item.IsDeleted = true;

            await _db.SaveChangesAsync();
            return true;
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

        private void NormalizeDto(BLUpsertDto dto)
        {
            dto.Parties = dto.Parties?
                .Where(x =>
                    x.RoleTypeId > 0 ||
                    x.PartyId.HasValue ||
                    !string.IsNullOrWhiteSpace(x.PartyTextInBL))
                .ToList() ?? new List<BLPartyDto>();

            dto.CargoDetails = dto.CargoDetails?
                .Where(x => !string.IsNullOrWhiteSpace(x.CargoDescription))
                .ToList() ?? new List<CargoDetailDto>();

            dto.Voyages = dto.Voyages?
                .Where(x =>
                    x.VoyageTypeId > 0 ||
                    x.VoyageId.HasValue ||
                    !string.IsNullOrWhiteSpace(x.VoyageNo) ||
                    !string.IsNullOrWhiteSpace(x.VesselName))
                .ToList() ?? new List<BLVoyageDto>();

            dto.Flights = dto.Flights?
                .Where(x =>
                    x.FlightId.HasValue ||
                    x.FlightDateId.HasValue ||
                    !string.IsNullOrWhiteSpace(x.FlightNo) ||
                    x.AirlineId.HasValue)
                .ToList() ?? new List<BLFlightDto>();

            dto.Trips = dto.Trips?
                .Where(x =>
                    x.TripId.HasValue ||
                    !string.IsNullOrWhiteSpace(x.DriverName) ||
                    !string.IsNullOrWhiteSpace(x.TruckNo) ||
                    !string.IsNullOrWhiteSpace(x.TrailerNo))
                .ToList() ?? new List<BLTripDto>();

            dto.RailJourneys = dto.RailJourneys?
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.TrainNo) ||
                    !string.IsNullOrWhiteSpace(x.WagonNo) ||
                    x.DepartureDate.HasValue ||
                    x.ArrivalDate.HasValue)
                .ToList() ?? new List<BLRailJourneyDto>();

            dto.Containers = dto.Containers?
                .Where(x =>
                    x.ContainerUseId != Guid.Empty ||
                    !string.IsNullOrWhiteSpace(x.ContainerNumber) ||
                    x.ContainerSizeTypeId > 0)
                .ToList() ?? new List<BLContainerDto>();

            foreach (var container in dto.Containers)
            {
                container.CargoDetails = container.CargoDetails?
                    .Where(x => !string.IsNullOrWhiteSpace(x.CargoDescription))
                    .ToList() ?? new List<CargoDetailDto>();
            }
        }

        private void NormalizeTransportSpecificSections(BLUpsertDto dto)
        {
            if (dto.TransportTypeId != SeaTransportTypeId)
                dto.Voyages = new List<BLVoyageDto>();

            if (dto.TransportTypeId != AirTransportTypeId)
                dto.Flights = new List<BLFlightDto>();

            if (dto.TransportTypeId != RoadTransportTypeId)
                dto.Trips = new List<BLTripDto>();

            if (dto.TransportTypeId != RailTransportTypeId)
                dto.RailJourneys = new List<BLRailJourneyDto>();
        }

        private (bool Success, string Message) ValidateDto(BLUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.BLNo))
                return (false, "BL No is required.");

            if (dto.CompanyId <= 0)
                return (false, "CompanyId is required.");

            if (dto.BLLevelId <= 0)
                return (false, "BLLevelId is required.");

            if (dto.TransportTypeId <= 0)
                return (false, "TransportTypeId is required.");

            if (dto.LoadTypeId <= 0)
                return (false, "LoadTypeId is required.");

            if (dto.MovementTypeId <= 0)
                return (false, "MovementTypeId is required.");

            if (dto.BLStatusId <= 0 && dto.BLStatusId != -1)
                return (false, "BLStatusId is required.");

            if (dto.PaymentTypeId <= 0)
                return (false, "PaymentTypeId is required.");

            if (dto.PORId == Guid.Empty || dto.POLId == Guid.Empty || dto.PODId == Guid.Empty || dto.PODelId == Guid.Empty)
                return (false, "POR, POL, POD and PODel are required.");

            if (dto.IsContainerized)
            {
                if (!dto.Containers.Any())
                    return (false, "For containerized BL, at least one container is required.");

                if (dto.CargoDetails.Any())
                    return (false, "For containerized BL, loose cargo details are not allowed.");

                foreach (var container in dto.Containers)
                {
                    if (string.IsNullOrWhiteSpace(container.ContainerNumber))
                        return (false, "Container number is required.");

                    //if (container.ContainerUseId == Guid.Empty)
                    //    return (false, "ContainerUseId is required.");

                    if (container.ContainerSizeTypeId <= 0)
                        return (false, "ContainerSizeTypeId is required.");

                    if (container.CargoDetails == null || !container.CargoDetails.Any())
                        return (false, $"Container {container.ContainerNumber} must contain cargo details.");
                }
            }
            else
            {
                if (dto.Containers.Any())
                    return (false, "Non-containerized BL cannot have containers.");

                if (dto.CargoDetails == null || !dto.CargoDetails.Any())
                    return (false, "At least one cargo detail is required.");
            }

            return (true, string.Empty);
        }

        private async Task<Guid?> ResolveJobIdAsync(BLUpsertDto dto)
        {
            if (dto.JobId.HasValue && dto.JobId.Value != Guid.Empty)
                return dto.JobId.Value;

            // Adjust these dbsets/entities based on your real schema

            if (dto.BookingId.HasValue)
            {
                var booking = await _db.Bookings
                    .AsNoTracking()
                    .Where(x => x.Id == dto.BookingId.Value)
                    .Select(x => new { x.JobId })
                    .FirstOrDefaultAsync();

                if (booking != null)
                    return booking.JobId;
            }

            if (dto.ShipmentId.HasValue)
            {
                var shipment = await _db.Shipments
                    .AsNoTracking()
                    .Where(x => x.Id == dto.ShipmentId.Value)
                    .Select(x => new { x.JobId })
                    .FirstOrDefaultAsync();

                if (shipment != null)
                    return shipment.JobId;
            }

            if (dto.ShipmentLegId.HasValue)
            {
                var leg = await _db.ShipmentLegs
                    .Include(x => x.Shipment)
                    .AsNoTracking()
                    .Where(x => x.Id == dto.ShipmentLegId.Value)
                    .Select(x => new { x.Shipment.JobId })
                    .FirstOrDefaultAsync();

                if (leg != null)
                    return leg.JobId;
            }

            return null;
        }

        private async Task<(bool Success, string Message, Guid? Id)> CreateAsync(BLUpsertDto dto)
        {
            var entity = new BL
            {
                Id = Guid.CreateVersion7(),

                JobId = dto.JobId!.Value,
                BookingId = dto.BookingId,
                ShipmentId = dto.ShipmentId,
                ShipmentLegId = dto.ShipmentLegId,

                CompanyId = dto.CompanyId,
                BLLevelId = dto.BLLevelId,
                TransportTypeId = dto.TransportTypeId,

                BLNo = dto.BLNo.Trim(),
                BLIssueDate = dto.BLIssueDate,
                PlaceOfIssue = dto.IssuePlace,

                CarrierId = dto.CarrierId,
                BLTypeId = dto.BLTypeId.Value,

                OriginalCount = dto.OriginalCount,
                CopyCount = dto.CopyCount,

                LoadTypeId = dto.LoadTypeId,
                MovementTypeId = dto.MovementTypeId,
                BLStatusId = -1,
                PaymentTypeId = dto.PaymentTypeId,

                IssuedByUs = dto.IssuedByUs,
                IsSwitched = dto.IsSwitched,
                //IsContainerized = dto.IsContainerized,

                IssuerPartyId = dto.IssuerPartyId,

                PORId = dto.PORId,
                PORText = dto.PORText ?? "-",
                POLId = dto.POLId,
                POLText = dto.POLText ?? "-",
                PODId = dto.PODId,
                PODText = dto.PODText ?? "-",
                PODelId = dto.PODelId,
                PODelText = dto.PODelText ?? "-",

                FinalDestinationText = dto.FinalDestinationText ?? "-",
                FreightPaymentAtId = dto.FreightPrepaidATId,
                FreightPaymentAtText = dto.FreightPrepaidATText,
                PointandCountryOfOriginId = dto.PointandCountryOfOriginId,
                PointandCountryOfOriginText = dto.PointandCountryOfOriginText,
                OnwardInlandRoutingId = dto.OnwardInlandRoutingId,
                OnwardInlandRoutingText = dto.OnwardInlandRoutingText,

                GoodsDescription = dto.GoodsDescription,
                MarksAndNumbers = dto.MarksAndNumbers,
                RemarksInBL = dto.RemarksInBL,
                GrossWeightKg = dto.GrossWeightKg,
                NetWeightKg = dto.NetWeightKg,
                VolumeCBM = dto.VolumeCBM,
                ChargeableWeight = dto.ChargeableWeight,
                TotalContainers = dto.TotalContainers.Value,
                TotalPackages = dto.TotalPackages.Value,
                PackageTypeId = dto.PackageTypeId,
                PackageTypeText = dto.PackageTypeText,

                ExportReference = dto.ExportReference,
                ForwardingAgentReference = dto.ForwardingAgentReference,
                SerialNo = dto.SerialNo,
                MRN = dto.MRN,
                PursuitCode = dto.PursuitCode,
                FreightTerms = dto.FreightTerms,
                LCNo = dto.LCNo,
                LCBankName = dto.LCBankName,
                OrderRegistrationNo = dto.OrderRegistrationNo,

                ShippedOnBoardDate = dto.ShippedOnBoardDate,
                ReceivedForShipmentDate = dto.ReceivedForShipmentDate,
                ReleaseDate = dto.ReleaseDate,

                BLReleaseTypeId = dto.BLReleaseTypeId,
                SalesPersonId = dto.SalesPersonId,
                TranshipmentPortId = dto.TranshipmentPortId,

                ClauseRemarks = dto.ClauseRemarks,
                OnBoardNotation = dto.OnBoardNotation,
                RemarksInManifest = dto.RemarksInManifest,
                InternalNotes = dto.InternalNotes,

                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.BLs.Add(entity);

            await _db.SaveChangesAsync();

            await SyncChildCollectionsAsync(entity.Id, dto);

            return (true, "Saved successfully.", entity.Id);
        }

        private async Task<(bool Success, string Message, Guid? Id)> UpdateAsync(BLUpsertDto dto)
        {
            var entity = await _db.BLs
                .FirstOrDefaultAsync(x => x.Id == dto.Id!.Value && !x.IsDeleted);

            if (entity == null)
                return (false, "BL not found.", null);

            entity.JobId = dto.JobId!.Value;
            entity.BookingId = dto.BookingId;
            entity.ShipmentId = dto.ShipmentId;
            entity.ShipmentLegId = dto.ShipmentLegId;

            entity.CompanyId = dto.CompanyId;
            entity.BLLevelId = dto.BLLevelId;
            entity.TransportTypeId = dto.TransportTypeId;

            entity.BLNo = dto.BLNo.Trim();
            entity.BLIssueDate = dto.BLIssueDate;

            entity.CarrierId = dto.CarrierId;
            entity.BLTypeId = dto.BLTypeId.Value;

            entity.OriginalCount = dto.OriginalCount;
            entity.CopyCount = dto.CopyCount;

            entity.LoadTypeId = dto.LoadTypeId;
            entity.MovementTypeId = dto.MovementTypeId;
            entity.BLStatusId = -1;
            entity.PaymentTypeId = dto.PaymentTypeId;

            entity.IssuedByUs = dto.IssuedByUs;
            entity.IsSwitched = dto.IsSwitched;
            //entity.IsContainerized = dto.IsContainerized;

            entity.IssuerPartyId = dto.IssuerPartyId;

            entity.PORId = dto.PORId;
            entity.PORText = dto.PORText ?? "-";
            entity.POLId = dto.POLId;
            entity.POLText = dto.POLText ?? "-";
            entity.PODId = dto.PODId;
            entity.PODText = dto.PODText ?? "-";
            entity.PODelId = dto.PODelId;
            entity.PODelText = dto.PODelText ?? "-";

            entity.FinalDestinationText = dto.FinalDestinationText ?? "-";
            entity.FreightPaymentAtId = dto.FreightPrepaidATId;
            entity.FreightPaymentAtText = dto.FreightPrepaidATText;
            entity.PointandCountryOfOriginId = dto.PointandCountryOfOriginId;
            entity.PointandCountryOfOriginText = dto.PointandCountryOfOriginText;
            entity.OnwardInlandRoutingId = dto.OnwardInlandRoutingId;
            entity.OnwardInlandRoutingText = dto.OnwardInlandRoutingText;

            entity.GoodsDescription = dto.GoodsDescription;
            entity.MarksAndNumbers = dto.MarksAndNumbers;
            entity.RemarksInBL = dto.RemarksInBL;
            entity.GrossWeightKg = dto.GrossWeightKg;
            entity.NetWeightKg = dto.NetWeightKg;
            entity.VolumeCBM = dto.VolumeCBM;
            entity.ChargeableWeight = dto.ChargeableWeight;
            entity.TotalContainers = dto.TotalContainers.Value;
            entity.TotalPackages = dto.TotalPackages.Value;
            entity.PackageTypeId = dto.PackageTypeId;
            entity.PackageTypeText = dto.PackageTypeText;

            entity.ExportReference = dto.ExportReference;
            entity.ForwardingAgentReference = dto.ForwardingAgentReference;
            entity.SerialNo = dto.SerialNo;
            entity.MRN = dto.MRN;
            entity.PursuitCode = dto.PursuitCode;
            entity.FreightTerms = dto.FreightTerms;
            entity.LCNo = dto.LCNo;
            entity.LCBankName = dto.LCBankName;
            entity.OrderRegistrationNo = dto.OrderRegistrationNo;

            entity.ShippedOnBoardDate = dto.ShippedOnBoardDate;
            entity.ReceivedForShipmentDate = dto.ReceivedForShipmentDate;
            entity.ReleaseDate = dto.ReleaseDate;

            entity.BLReleaseTypeId = dto.BLReleaseTypeId;
            entity.SalesPersonId = dto.SalesPersonId;
            entity.TranshipmentPortId = dto.TranshipmentPortId;

            entity.ClauseRemarks = dto.ClauseRemarks;
            entity.OnBoardNotation = dto.OnBoardNotation;
            entity.RemarksInManifest = dto.RemarksInManifest;
            entity.InternalNotes = dto.InternalNotes;

            await _db.SaveChangesAsync();

            await SyncChildCollectionsAsync(entity.Id, dto);

            return (true, "Updated successfully.", entity.Id);
        }

        private async Task SyncChildCollectionsAsync(Guid blId, BLUpsertDto dto)
        {
            await SyncPartiesAsync(blId, dto.Parties);

            if (dto.IsContainerized)
            {
                await SyncContainersAsync(blId, dto.Containers);
                //await SoftDeleteLooseCargoAsync(blId);
            }
            else
            {
                await SoftDeleteContainersAsync(blId);
                await SyncLooseCargoAsync(blId, dto.CargoDetails);
            }

            await SyncVoyagesAsync(blId, dto.Voyages);
            await SyncFlightsAsync(blId, dto.Flights);
            await SyncTripsAsync(blId, dto.Trips);
            await SyncRailJourneysAsync(blId, dto.RailJourneys);

            await _db.SaveChangesAsync();
        }

        private async Task SyncPartiesAsync(Guid blId, List<BLPartyDto> items)
        {
            var existing = await _db.BLParties
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
                row.IsDeleted = true;

            foreach (var item in items)
            {
                BLParty entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new BLParty
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.BLParties.Add(entity);
                }

                entity.RoleTypeId = item.RoleTypeId;
                entity.PartyId = item.PartyId;
                entity.PartyTextInBL = item.PartyTextInBL;
                entity.PartyNationalNo = item.PartyNationalNo;
                entity.PartyTelephone = item.PartyTelephone;
                entity.PartyMobile = item.PartyMobile;
                entity.PartyFax = item.PartyFax;
                entity.PartyEmail = item.PartyEmail;
                entity.IsConsigneeToOrder = item.IsConsigneeToOrder;
                entity.IsConsigneeToOrderOf = item.IsConsigneeToOrderOf;
                entity.IsConsigneeToBearer = item.IsConsigneeToBearer;
                entity.IsNotifySameAs = item.IsNotifySameAs;
            }
        }

        private async Task SyncContainersAsync(Guid blId, List<BLContainerDto> items)
        {
            var existing = await _db.BLContainers
                .Include(x => x.CargoAllocations)
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
            {
                row.IsDeleted = true;
                foreach (var cargo in row.CargoAllocations.Where(c => !c.IsDeleted))
                    cargo.IsDeleted = true;
            }

            foreach (var item in items)
            {
                BLContainer entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new BLContainer
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.BLContainers.Add(entity);
                }

                entity.ContainerUseId = item.ContainerUseId;
                entity.ContainerNumber = item.ContainerNumber;
                entity.ContainerSizeTypeId = item.ContainerSizeTypeId;
                entity.TotalPackageCount = item.TotalPackageCount.Value;
                entity.SealNumber = item.SealNumber;
                entity.SealNumber2 = item.SealNumber2;
                entity.TotalVolumeCBM = item.TotalVolumeCBM;
                entity.BLContainerPercent = item.BLContainerPercent.Value;
                entity.TotalGrossWeight = item.TotalGrossWeight.Value;
                entity.TotalNetWeight = item.TotalNetWeight.Value;
                entity.DimensionUnitId = item.DimensionUnitId.Value;
                entity.OverHeight = item.OverHeight.Value;
                entity.OverLength = item.OverLength.Value;
                entity.OverWidth = item.OverWidth.Value;

                await _db.SaveChangesAsync();
            }
        }


        //private async Task SoftDeleteLooseCargoAsync(Guid blId)
        //{
        //    var looseCargo = await _db.CargoDetails
        //        .Where(x => x.BLId == blId && x.BLContainerId == null && !x.IsDeleted)
        //        .ToListAsync();

        //    foreach (var item in looseCargo)
        //        item.IsDeleted = true;
        //}

        private async Task SoftDeleteContainersAsync(Guid blId)
        {
            var containers = await _db.BLContainers
                .Include(x => x.CargoAllocations)
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            foreach (var container in containers)
            {
                container.IsDeleted = true;

                foreach (var cargo in container.CargoAllocations.Where(x => !x.IsDeleted))
                    cargo.IsDeleted = true;
            }
        }

        private async Task SyncLooseCargoAsync(Guid blId, List<CargoDetailDto> items)
        {
            var existing = await _db.CargoDetails
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
                row.IsDeleted = true;

            foreach (var item in items)
            {
                CargoDetail entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new CargoDetail
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.CargoDetails.Add(entity);
                }

                entity.BLId = blId;
                entity.Sequence = item.Sequence.Value;
                entity.PackageTypeId = item.PackageTypeId;
                entity.PackageCount = item.PackageCount;
                entity.GrossWeight = item.GrossWeight;
                entity.NetWeight = item.NetWeight;
                entity.Volume = item.Volume;
                entity.CargoDescription = item.CargoDescription;
                entity.MarksAndNumbers = item.MarksAndNumbers;
                entity.HSCodeToSave = item.HSCodeToSave;
                entity.Remarks = item.Remarks;
                entity.IsDG = item.IsDG;
                entity.IsOverSized = item.IsOverSized;
                entity.IsTemperatureControlled = item.IsTemperatureControlled;
                entity.IsFragile = item.IsFragile;
                entity.IsPerishable = item.IsPerishable;
                entity.IsStackable = item.IsStackable;
            }
        }

        private async Task SyncVoyagesAsync(Guid blId, List<BLVoyageDto> items)
        {
            var existing = await _db.BLVoyages
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
                row.IsDeleted = true;

            foreach (var item in items)
            {
                BLVoyage entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new BLVoyage
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.BLVoyages.Add(entity);
                }

                entity.VoyageTypeId = item.VoyageTypeId;
                entity.VoyageId = item.VoyageId;
                entity.VoyageNo = item.VoyageNo;
                entity.VesselName = item.VesselName;
                entity.ReceivedForShipmentDate = item.ReceivedForShipmentDate;
                entity.ShippedOnBoardDate = item.ShippedOnBoardDate;
            }
        }

        private async Task SyncFlightsAsync(Guid blId, List<BLFlightDto> items)
        {
            var existing = await _db.BLFlights
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
                row.IsDeleted = true;

            foreach (var item in items)
            {
                BLFlight entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new BLFlight
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.BLFlights.Add(entity);
                }

                entity.FlightId = item.FlightId;
                entity.FlightDateId = item.FlightDateId;
                entity.FlightNo = item.FlightNo;
                entity.AirlineId = item.AirlineId;
                entity.DeclaredValueForCarriage = item.DeclaredValueForCarriage;
                entity.DeclaredValueForCustoms = item.DeclaredValueForCustoms;
                entity.FlightDateDate = item.FlightDateDate;
                entity.HandlingInformation = item.HandlingInformation;
            }
        }

        private async Task SyncTripsAsync(Guid blId, List<BLTripDto> items)
        {
            var existing = await _db.BLTrips
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
                row.IsDeleted = true;

            foreach (var item in items)
            {
                BLTrip entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new BLTrip
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.BLTrips.Add(entity);
                }

                entity.TripId = item.TripId;
                entity.LoadingDate = item.LoadingDate;
                entity.DeliveryDate = item.DeliveryDate;
                entity.DriverName = item.DriverName;
                entity.DriverPassportNo = item.DriverPassportNo;
                entity.TruckNo = item.TruckNo;
                entity.TrailerNo = item.TrailerNo;
            }
        }

        private async Task SyncRailJourneysAsync(Guid blId, List<BLRailJourneyDto> items)
        {
            var existing = await _db.BLRailJourneys
                .Where(x => x.BLId == blId && !x.IsDeleted)
                .ToListAsync();

            var incomingIds = items.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

            foreach (var row in existing.Where(x => !incomingIds.Contains(x.Id)))
                row.IsDeleted = true;

            foreach (var item in items)
            {
                BLRailJourney entity;

                if (item.Id.HasValue)
                {
                    entity = existing.First(x => x.Id == item.Id.Value);
                }
                else
                {
                    entity = new BLRailJourney
                    {
                        Id = Guid.CreateVersion7(),
                        BLId = blId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.BLRailJourneys.Add(entity);
                }

                entity.TrainNo = item.TrainNo;
                entity.WagonNo = item.WagonNo;
                entity.DepartureDate = item.DepartureDate;
                entity.ArrivalDate = item.ArrivalDate;
            }
        }

        private void EnsureMinimumRowsForEdit(BLUpsertDto dto)
        {
            if (!dto.Parties.Any())
            {
                dto.Parties.Add(new BLPartyDto());
                dto.Parties.Add(new BLPartyDto());
                dto.Parties.Add(new BLPartyDto());
            }

            if (!dto.Containers.Any())
            {
                dto.Containers.Add(new BLContainerDto
                {
                    CargoDetails = new List<CargoDetailDto>
                    {
                        new CargoDetailDto { Sequence = 1 }
                    }
                });
            }

            foreach (var container in dto.Containers)
            {
                if (!container.CargoDetails.Any())
                    container.CargoDetails.Add(new CargoDetailDto { Sequence = 1 });
            }

            if (!dto.CargoDetails.Any())
            {
                dto.CargoDetails.Add(new CargoDetailDto { Sequence = 1 });
            }

            if (!dto.Voyages.Any())
                dto.Voyages.Add(new BLVoyageDto());

            if (!dto.Flights.Any())
                dto.Flights.Add(new BLFlightDto());

            if (!dto.Trips.Any())
                dto.Trips.Add(new BLTripDto());

            if (!dto.RailJourneys.Any())
                dto.RailJourneys.Add(new BLRailJourneyDto());
        }
    }
}

