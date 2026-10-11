namespace ShipRate.WebApp.ViewModels.TransitViewModels
{
    // ViewModels/TripViewModels.cs
    public class TripListViewModel
    {
        public List<TripItemViewModel> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        // Search params (for pagination links)
        public string? PlateNumber { get; set; }
        public string? DriverName { get; set; }
        public int? StatusId { get; set; }
        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class TripItemViewModel
    {
        public Guid Id { get; set; }
        public string TripNumber { get; set; } = "";
        public string TruckPlateNumber { get; set; } = "";
        public string TruckTitle { get; set; } = "";
        public string DriverName { get; set; } = "";
        public string FromLocation { get; set; } = "";
        public string ToLocation { get; set; } = "";
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string StatusName { get; set; } = "";
        public int TripStatusId { get; set; }
        public string CarrierName { get; set; } = "";
        public string Route { get; set; }
        public bool IsActive { get; set; }
    }

    public class TripViewModel
    {
        public Guid? Id { get; set; }
        public int CompanyId { get; set; }
        public Guid TruckId { get; set; }
        public string TripNumber { get; set; } = "";
        public Guid DriverId { get; set; }

        public Guid FromLocationId { get; set; }
        public Guid ToLocationId { get; set; }

        public string? FromLocationName { get; set; }
        public string? ToLocationName { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public int TripStatusId { get; set; }
        public Guid CarrierId { get; set; }
        public string? Remarks { get; set; }
    }


    public class TripStopViewModel
    {
        public Guid? Id { get; set; }
        public Guid TripId { get; set; }
        public int StopNumber { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public DateTime? ScheduledArrival { get; set; }
        public DateTime? ScheduledDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public string Remarks { get; set; }
    }

    public class TripStopsListViewModel
    {
        public Guid TripId { get; set; }
        public string TripNumber { get; set; }
        public List<TripStopViewModel> Stops { get; set; } = new();
    }



}
