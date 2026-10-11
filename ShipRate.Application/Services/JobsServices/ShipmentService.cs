using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.JobsServices
{
    public partial class ShipmentService : IShipmentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IActivityLogService _activityLogService;
        private readonly IJobService _jobService;

        public ShipmentService(ApplicationDbContext db, IActivityLogService activityLogService, IJobService jobService)
        {
            _db = db;
            _activityLogService = activityLogService;
            _jobService = jobService;
        }

        public async Task<PagedResultDto<ShipmentLegListItemVm>> GetShipmentLegListAsync(ShipmentLegFilterVm filter)
        {
            var query = _db.ShipmentLegs
                .AsNoTracking()
                .Include(x => x.LegType)
                .Include(x => x.TransportType)
                .Include(x => x.FromLocation)
                .Include(x => x.ToLocation)
                .Include(x => x.Carrier).ThenInclude(x => x.Person)
                .Include(x => x.LegStatus)
                .Where(x => !x.IsDeleted && x.ShipmentId == filter.ShipmentId)
                .OrderBy(x => x.SequenceNo)
                .ThenBy(x => x.CreatedAt);

            var items = await query
                .Select(x => new ShipmentLegListItemVm
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    SequenceNo = x.SequenceNo,
                    LegTypeId = x.LegTypeId,
                    LegTypeName = x.LegType != null ? x.LegType.LegTypeName : null,
                    TransportTypeId = x.TransportTypeId,
                    TransportTypeName = x.TransportType != null ? x.TransportType.TransportTypeName : null,
                    FromLocationId = x.FromLocationId,
                    FromLocationName = x.FromLocation.LocationName,
                    ToLocationId = x.ToLocationId,
                    ToLocationName = x.ToLocation.LocationName,
                    CarrierId = x.CarrierId,
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : null,
                    PlannedDepartureDate = x.PlannedDepartureDate,
                    PlannedArrivalDate = x.PlannedArrivalDate,
                    ActualDepartureDate = x.ActualDepartureDate,
                    ActualArrivalDate = x.ActualArrivalDate,
                    LegStatusId = x.LegStatusId,
                    LegStatusName = x.LegStatus != null ? x.LegStatus.ShipmentLegStatusName : null,
                    TotalPackages = x.TotalPackages,
                    GrossWeightKg = x.GrossWeightKg,
                    NetWeightKg = x.NetWeightKg,
                    ChargeableWeightKg = x.ChargeableWeightKg,
                    VolumeCBM = x.VolumeCBM,
                    //IsActive = x.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<ShipmentLegListItemVm>
            {
                Items = items,
                TotalCount = items.Count
            };
        }

        public async Task<ShipmentLegFormVm> BuildShipmentLegCreateVmAsync(Guid shipmentId)
        {
            var vm = new ShipmentLegFormVm
            {
                ShipmentId = shipmentId,
                SequenceNo = await GetNextShipmentLegSequenceNoAsync(shipmentId),
                IsActive = true
            };

            await FillShipmentLegLookupsAsync(vm);
            return vm;
        }

        public async Task<ShipmentLegFormVm?> BuildShipmentLegEditVmAsync(Guid id)
        {
            var entity = await _db.ShipmentLegs
                .Include(x => x.FromLocation)
                .Include(x => x.ToLocation)
                .Include(x => x.Voyage)
                    .ThenInclude(x => x.Vessel)
                .Include(x => x.Trip)
                    .ThenInclude(x => x.Truck)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            var vm = new ShipmentLegFormVm
            {
                Id = entity.Id,
                ShipmentId = entity.ShipmentId,
                SequenceNo = entity.SequenceNo,
                LegTypeId = entity.LegTypeId,
                TransportTypeId = entity.TransportTypeId,
                FromLocationId = entity.FromLocationId,
                ToLocationId = entity.ToLocationId,
                FromLocationName = entity.FromLocation.LocationName,
                ToLocationName = entity.ToLocation.LocationName,
                CarrierId = entity.CarrierId,
                PlannedDepartureDate = entity.PlannedDepartureDate,
                PlannedArrivalDate = entity.PlannedArrivalDate,
                ActualDepartureDate = entity.ActualDepartureDate,
                ActualArrivalDate = entity.ActualArrivalDate,
                ChargeableWeightKg = entity.ChargeableWeightKg,
                GrossWeightKg = entity.GrossWeightKg,
                NetWeightKg = entity.NetWeightKg,
                VolumeCBM = entity.VolumeCBM,
                TotalPackages = entity.TotalPackages,
                PackageTypeId = entity.PackageTypeId,
                ReceiptPlaceText = entity.ReceiptPlaceText,
                DeliveryPlaceText = entity.DeliveryPlaceText,
                FlightId = entity.FlightId,
                FlightDateId = entity.FlightDateId,
                VoyageId = entity.VoyageId,
                VesselId = entity.VesselId,
                SelectedVoyageText = entity.VoyageId is null ? null : entity.Voyage.Vessel.VesselName + "-" + entity.Voyage.VoyageNo,
                SelectedTripText = entity.TripId is null ? null : entity.Trip.TripNumber + "---" + entity.Trip.Truck.TruckPlateNumber + "/" + entity.Trip.Truck.TrailerPlateNumber,
                TripId = entity.TripId,
                TruckId = entity.TruckId,
                DriverId = entity.DriverId,
                LegStatusId = entity.LegStatusId,
                Remarks = entity.Remarks,
                //IsActive = entity.IsActive
            };

            if (entity.TransportTypeId.HasValue)
            {
                vm.SelectedTransportTypeName = await _db.TransportTypes
                    .Where(x => x.Id == entity.TransportTypeId.Value)
                    .Select(x => x.TransportTypeName)
                    .FirstOrDefaultAsync();
            }

            await FillShipmentLegLookupsAsync(vm);
            return vm;
        }

        public async Task<ShipmentLegDetailsVm?> GetShipmentLegDetailsAsync(Guid id)
        {
            return await _db.ShipmentLegs
                .AsNoTracking()
                .Include(x => x.LegType)
                .Include(x => x.TransportType)
                .Include(x => x.FromLocation)
                .Include(x => x.ToLocation)
                .Include(x => x.Carrier).ThenInclude(x => x.Person)
                .Include(x => x.Flight)
                .Include(x => x.FlightDate)
                .Include(x => x.Voyage)
                .Include(x => x.Vessel)
                .Include(x => x.Trip)
                .Include(x => x.Truck)
                .Include(x => x.Driver).ThenInclude(x => x.Person)
                .Include(x => x.LegStatus)
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new ShipmentLegDetailsVm
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    SequenceNo = x.SequenceNo,
                    LegTypeName = x.LegType != null ? x.LegType.LegTypeName : null,
                    TransportTypeName = x.TransportType != null ? x.TransportType.TransportTypeName : null,
                    FromLocationName = x.FromLocation.LocationName,
                    ToLocationName = x.ToLocation.LocationName,
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : null,
                    FlightName = x.Flight != null ? x.Flight.FlightNumber : null,
                    FlightDateName = x.FlightDate != null
                        ? ((x.FlightDate.ETD.HasValue ? x.FlightDate.ETD.Value.ToString("yyyy-MM-dd HH:mm") : "-")
                           + " / " +
                           (x.FlightDate.ETA.HasValue ? x.FlightDate.ETA.Value.ToString("yyyy-MM-dd HH:mm") : "-"))
                        : null,
                    VoyageName = x.Voyage != null ? x.Voyage.VoyageNo : null,
                    VesselName = x.Vessel != null ? x.Vessel.VesselName : null,
                    TripName = x.Trip != null ? x.Trip.TripNumber : null,
                    TruckName = x.Truck != null ? x.Truck.TruckReferenceTitle : null,
                    DriverName = x.Driver != null ? x.Driver.Person.PersonName : null,
                    LegStatusName = x.LegStatus != null ? x.LegStatus.ShipmentLegStatusName : null,
                    //PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : null,
                    PlannedDepartureDate = x.PlannedDepartureDate,
                    PlannedArrivalDate = x.PlannedArrivalDate,
                    ActualDepartureDate = x.ActualDepartureDate,
                    ActualArrivalDate = x.ActualArrivalDate,
                    ChargeableWeightKg = x.ChargeableWeightKg,
                    GrossWeightKg = x.GrossWeightKg,
                    NetWeightKg = x.NetWeightKg,
                    VolumeCBM = x.VolumeCBM,
                    TotalPackages = x.TotalPackages,
                    ReceiptPlaceText = x.ReceiptPlaceText,
                    DeliveryPlaceText = x.DeliveryPlaceText,
                    Remarks = x.Remarks,
                    //IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ShipmentLegDeleteVm?> GetShipmentLegDeleteVmAsync(Guid id)
        {
            return await _db.ShipmentLegs
                .AsNoTracking()
                .Include(x => x.FromLocation)
                .Include(x => x.ToLocation)
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new ShipmentLegDeleteVm
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    SequenceNo = x.SequenceNo,
                    FromLocationName = x.FromLocation.LocationName,
                    ToLocationName = x.ToLocation.LocationName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string? Error, Guid Id)> CreateShipmentLegAsync(ShipmentLegFormVm vm, Guid currentUserId)
        {
            var shipmentExists = await _db.Shipments.AnyAsync(x => x.Id == vm.ShipmentId && !x.IsDeleted);
            if (!shipmentExists)
                return (false, "Invalid Shipment.", Guid.Empty);

            if (!vm.FromLocationId.HasValue || !await _db.Locations.AnyAsync(x => x.Id == vm.FromLocationId.Value && !x.IsDeleted))
                return (false, "Invalid From Location.", Guid.Empty);

            if (!vm.ToLocationId.HasValue || !await _db.Locations.AnyAsync(x => x.Id == vm.ToLocationId.Value && !x.IsDeleted))
                return (false, "Invalid To Location.", Guid.Empty);

            if (vm.FromLocationId == vm.ToLocationId)
                return (false, "From Location and To Location cannot be the same.", Guid.Empty);


            await ResolveTransportTypeNameAsync(vm);
            NormalizeShipmentLegConditionalFields(vm);





            var validation = await ValidateShipmentLegRelationsAsync(vm);
            if (!validation.Success)
                return (false, validation.Error, Guid.Empty);

            if (await ExistsDuplicateShipmentLegSequenceAsync(vm.ShipmentId, vm.SequenceNo, null))
                return (false, "Sequence No already exists for this shipment.", Guid.Empty);

            var entity = new ShipmentLeg
            {
                Id = Guid.CreateVersion7(),
                ShipmentId = vm.ShipmentId,
                SequenceNo = vm.SequenceNo,
                LegTypeId = vm.LegTypeId,
                TransportTypeId = vm.TransportTypeId,
                FromLocationId = vm.FromLocationId.Value,
                ToLocationId = vm.ToLocationId.Value,
                CarrierId = vm.CarrierId,
                PlannedDepartureDate = vm.PlannedDepartureDate,
                PlannedArrivalDate = vm.PlannedArrivalDate,
                ActualDepartureDate = vm.ActualDepartureDate,
                ActualArrivalDate = vm.ActualArrivalDate,
                ChargeableWeightKg = vm.ChargeableWeightKg,
                GrossWeightKg = vm.GrossWeightKg,
                NetWeightKg = vm.NetWeightKg,
                VolumeCBM = vm.VolumeCBM,
                TotalPackages = vm.TotalPackages,
                PackageTypeId = vm.PackageTypeId,
                ReceiptPlaceText = vm.ReceiptPlaceText?.Trim(),
                DeliveryPlaceText = vm.DeliveryPlaceText?.Trim(),
                FlightId = vm.FlightId,
                FlightDateId = vm.FlightDateId,
                VoyageId = vm.VoyageId,
                VesselId = vm.VesselId,
                TripId = vm.TripId,
                TruckId = vm.TruckId,
                DriverId = vm.DriverId,
                LegStatusId = 1,
                Remarks = vm.Remarks?.Trim(),
                //IsActive = vm.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            _db.ShipmentLegs.Add(entity);
            await _db.SaveChangesAsync();

            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Shipment Leg Created",
                    Description = "Sequence No " + vm.SequenceNo.ToString(),
                    EntityTypeId = 1,
                    EntityId = await GetShipmentJobId(vm.ShipmentId),
                    PerformedByUserId = currentUserId,
                    IsSystemGenerated = true,
                }
                );
            return (true, null, entity.Id);
        }

        public async Task<(bool Success, string? Error)> UpdateShipmentLegAsync(ShipmentLegFormVm vm, Guid currentUserId)
        {
            if (!vm.Id.HasValue)
                return (false, "Invalid Id.");

            var entity = await _db.ShipmentLegs
                .FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted);

            if (entity == null)
                return (false, "Record not found.");

            if (!vm.FromLocationId.HasValue || !await _db.Locations.AnyAsync(x => x.Id == vm.FromLocationId.Value && !x.IsDeleted))
                return (false, "Invalid From Location.");

            if (!vm.ToLocationId.HasValue || !await _db.Locations.AnyAsync(x => x.Id == vm.ToLocationId.Value && !x.IsDeleted))
                return (false, "Invalid To Location.");

            if (vm.FromLocationId == vm.ToLocationId)
                return (false, "From Location and To Location cannot be the same.");

            await ResolveTransportTypeNameAsync(vm);
            NormalizeShipmentLegConditionalFields(vm);



            var validation = await ValidateShipmentLegRelationsAsync(vm);
            if (!validation.Success)
                return (false, validation.Error);

            if (await ExistsDuplicateShipmentLegSequenceAsync(entity.ShipmentId, vm.SequenceNo, entity.Id))
                return (false, "Sequence No already exists for this shipment.");

            entity.SequenceNo = vm.SequenceNo;
            entity.LegTypeId = vm.LegTypeId;
            entity.TransportTypeId = vm.TransportTypeId;
            entity.FromLocationId = vm.FromLocationId.Value;
            entity.ToLocationId = vm.ToLocationId.Value;
            entity.CarrierId = vm.CarrierId;
            entity.PlannedDepartureDate = vm.PlannedDepartureDate;
            entity.PlannedArrivalDate = vm.PlannedArrivalDate;
            entity.ActualDepartureDate = vm.ActualDepartureDate;
            entity.ActualArrivalDate = vm.ActualArrivalDate;
            entity.ChargeableWeightKg = vm.ChargeableWeightKg;
            entity.GrossWeightKg = vm.GrossWeightKg;
            entity.NetWeightKg = vm.NetWeightKg;
            entity.VolumeCBM = vm.VolumeCBM;
            entity.TotalPackages = vm.TotalPackages;
            entity.PackageTypeId = vm.PackageTypeId;
            entity.ReceiptPlaceText = vm.ReceiptPlaceText?.Trim();
            entity.DeliveryPlaceText = vm.DeliveryPlaceText?.Trim();
            entity.FlightId = vm.FlightId;
            entity.FlightDateId = vm.FlightDateId;
            entity.VoyageId = vm.VoyageId;
            entity.VesselId = vm.VesselId;
            entity.TripId = vm.TripId;
            entity.TruckId = vm.TruckId;
            entity.DriverId = vm.DriverId;
            entity.Remarks = vm.Remarks?.Trim();
            //entity.IsActive = vm.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteShipmentLegAsync(Guid id, Guid currentUserId)
        {
            var entity = await _db.ShipmentLegs
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return (false, "Record not found.");

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task FillShipmentLegLookupsAsync(ShipmentLegFormVm vm)
        {
            vm.LegTypes = await _db.LegTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.LegTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LegTypeName
                })
                .ToListAsync();

            vm.TransportTypes = await _db.TransportTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TransportTypeName
                })
                .ToListAsync();

            //vm.Locations = await _db.Locations
            //    .AsNoTracking()
            //    .Where(x => !x.IsDeleted && x.IsActive)
            //    .OrderBy(x => x.LocationName)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.LocationName
            //    })
            //    .ToListAsync();

            vm.Carriers = await _db.Carriers
                .AsNoTracking()
                .Include(x => x.Person)
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();

            vm.Flights = await _db.Flights
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.FlightNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FlightNumber
                })
                .ToListAsync();

            vm.FlightDates = await _db.FlightDates
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.ETD)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = (x.ETD.HasValue ? x.ETD.Value.ToString("yyyy-MM-dd HH:mm") : "-")
                           + " / " +
                           (x.ETA.HasValue ? x.ETA.Value.ToString("yyyy-MM-dd HH:mm") : "-")
                })
                .ToListAsync();

            vm.Voyages = await _db.Voyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.VoyageNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyageNo
                })
                .ToListAsync();

            vm.Vessels = await _db.Vessels
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.VesselName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VesselName
                })
                .ToListAsync();

            vm.Trips = await _db.Trips
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TripNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TripNumber
                })
                .ToListAsync();

            vm.Trucks = await _db.Trucks
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TruckReferenceTitle)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TruckReferenceTitle
                })
                .ToListAsync();

            vm.Drivers = await _db.Drivers
                .AsNoTracking()
                .Include(x => x.Person)
                //.Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();

            vm.LegStatuses = await _db.ShipmentLegStatus
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShipmentLegStatusName
                })
                .ToListAsync();

            vm.PackageTypes = await _db.PackageTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.PackageTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PackageTypeName
                })
                .ToListAsync();
        }

        private async Task<int> GetNextShipmentLegSequenceNoAsync(Guid shipmentId)
        {
            var max = await _db.ShipmentLegs
                .Where(x => x.ShipmentId == shipmentId && !x.IsDeleted)
                .Select(x => (int?)x.SequenceNo)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        private async Task<bool> ExistsDuplicateShipmentLegSequenceAsync(Guid shipmentId, int sequenceNo, Guid? excludedId)
        {
            return await _db.ShipmentLegs.AnyAsync(x =>
                !x.IsDeleted &&
                x.ShipmentId == shipmentId &&
                x.SequenceNo == sequenceNo &&
                (!excludedId.HasValue || x.Id != excludedId.Value));
        }

        private async Task<(bool Success, string? Error)> ValidateShipmentLegRelationsAsync(ShipmentLegFormVm vm)
        {
            if (vm.LegTypeId.HasValue &&
                !await _db.LegTypes.AnyAsync(x => x.Id == vm.LegTypeId.Value && !x.IsDeleted && x.IsActive))
                return (false, "Invalid Leg Type.");

            if (vm.TransportTypeId.HasValue &&
                !await _db.TransportTypes.AnyAsync(x => x.Id == vm.TransportTypeId.Value && !x.IsDeleted && x.IsActive))
                return (false, "Invalid Transport Type.");

            if (vm.CarrierId.HasValue &&
                !await _db.Carriers.AnyAsync(x => x.Id == vm.CarrierId.Value && !x.IsDeleted))
                return (false, "Invalid Carrier.");

            if (vm.FlightId.HasValue &&
                !await _db.Flights.AnyAsync(x => x.Id == vm.FlightId.Value && !x.IsDeleted))
                return (false, "Invalid Flight.");

            if (vm.FlightDateId.HasValue &&
                !await _db.FlightDates.AnyAsync(x => x.Id == vm.FlightDateId.Value && !x.IsDeleted))
                return (false, "Invalid Flight Date.");

            if (vm.VoyageId.HasValue &&
                !await _db.Voyages.AnyAsync(x => x.Id == vm.VoyageId.Value && !x.IsDeleted))
                return (false, "Invalid Voyage.");

            if (vm.VesselId.HasValue &&
                !await _db.Vessels.AnyAsync(x => x.Id == vm.VesselId.Value && !x.IsDeleted))
                return (false, "Invalid Vessel.");

            if (vm.TripId.HasValue &&
                !await _db.Trips.AnyAsync(x => x.Id == vm.TripId.Value && !x.IsDeleted))
                return (false, "Invalid Trip.");

            if (vm.TruckId.HasValue &&
                !await _db.Trucks.AnyAsync(x => x.Id == vm.TruckId.Value && !x.IsDeleted))
                return (false, "Invalid Truck.");

            if (vm.DriverId.HasValue &&
                !await _db.Drivers.AnyAsync(x => x.Id == vm.DriverId.Value && !x.IsDeleted))
                return (false, "Invalid Driver.");

            if (vm.LegStatusId.HasValue &&
                !await _db.ShipmentLegStatus.AnyAsync(x => x.Id == vm.LegStatusId.Value && !x.IsDeleted && x.IsActive))
                return (false, "Invalid Leg Status.");

            if (vm.PackageTypeId.HasValue &&
                !await _db.PackageTypes.AnyAsync(x => x.Id == vm.PackageTypeId.Value && !x.IsDeleted && x.IsActive))
                return (false, "Invalid Package Type.");

            if (vm.PlannedDepartureDate.HasValue && vm.PlannedArrivalDate.HasValue &&
                vm.PlannedDepartureDate > vm.PlannedArrivalDate)
                return (false, "Planned Departure cannot be greater than Planned Arrival.");

            if (vm.ActualDepartureDate.HasValue && vm.ActualArrivalDate.HasValue &&
                vm.ActualDepartureDate > vm.ActualArrivalDate)
                return (false, "Actual Departure cannot be greater than Actual Arrival.");


            if (vm.IsAir)
            {
                if (!vm.FlightId.HasValue)
                    return (false, "Flight is required for Air transport.");

                if (!vm.FlightDateId.HasValue)
                    return (false, "Flight Date is required for Air transport.");
            }

            if (vm.IsSea)
            {
                if (!vm.VoyageId.HasValue)
                    return (false, "Voyage is required for Sea transport.");

                //if (!vm.VesselId.HasValue)
                //    return (false, "Vessel is required for Sea transport.");
            }

            if (vm.IsRoad)
            {
                if (!vm.TripId.HasValue)
                    return (false, "Trip is required for Road transport.");
                //if (!vm.TruckId.HasValue)
                //    return (false, "Truck is required for Road transport.");

                //if (!vm.DriverId.HasValue)
                //    return (false, "Driver is required for Road transport.");
            }
            return (true, null);
        }

        private async Task<Guid> GetShipmentJobId(Guid shipmentId)
        {
            var shipment = await _db.Shipments.FindAsync(shipmentId);
            return shipment.JobId;
        }

        private async Task ResolveTransportTypeNameAsync(ShipmentLegFormVm vm)
        {
            if (vm.TransportTypeId.HasValue)
            {
                vm.SelectedTransportTypeName = await _db.TransportTypes
                    .AsNoTracking()
                    .Where(x => x.Id == vm.TransportTypeId.Value)
                    .Select(x => x.TransportTypeName)
                    .FirstOrDefaultAsync();
            }
            else
            {
                vm.SelectedTransportTypeName = null;
            }
        }

        private void NormalizeShipmentLegConditionalFields(ShipmentLegFormVm vm)
        {
            if (vm.IsAir)
            {
                vm.VoyageId = null;
                vm.VesselId = null;
                vm.TripId = null;
                vm.TruckId = null;
                vm.DriverId = null;
            }
            else if (vm.IsSea)
            {
                vm.FlightId = null;
                vm.FlightDateId = null;
                vm.TripId = null;
                vm.TruckId = null;
                vm.DriverId = null;
            }
            else if (vm.IsRoad)
            {
                vm.FlightId = null;
                vm.FlightDateId = null;
                vm.VoyageId = null;
                vm.VesselId = null;
            }
            else
            {
                vm.FlightId = null;
                vm.FlightDateId = null;
                vm.VoyageId = null;
                vm.VesselId = null;
                vm.TripId = null;
                vm.TruckId = null;
                vm.DriverId = null;
            }
        }
    }
}
