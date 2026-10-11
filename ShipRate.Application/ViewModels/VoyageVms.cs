namespace ShipRate.Application.ViewModels
{
    public class VoyageLookupItemVm
    {
        public Guid Id { get; set; }

        public string VesselName { get; set; } = string.Empty;

        public string VoyageNo { get; set; } = string.Empty;

        public string? DisplayText =>
            $"{VesselName} - {VoyageNo}";
    }



    public sealed class TripLookupItemVm
    {
        public Guid Id { get; set; }

        public string? TruckNo { get; set; }
        public string? TrailerNo { get; set; }
        public string? DriverName { get; set; }

        public string? CMRNo { get; set; }
        public string? TIRNo { get; set; }

        public string DisplayText =>
            string.Join(" - ",
                new[]
                {
                TruckNo,
                TrailerNo,
                DriverName
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public sealed class FlightLookupItemVm
    {
        public Guid Id { get; set; }

        public Guid AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;

        public string FlightNo { get; set; } = string.Empty;

        public string? OriginAirportName { get; set; }
        public string? DestinationAirportName { get; set; }

        public string DisplayText =>
            $"{AirlineName} - {FlightNo} ({OriginAirportName} → {DestinationAirportName})";
    }

    public sealed class FlightDateLookupItemVm
    {
        public Guid Id { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }

        public decimal DelayHours { get; set; }

        public string DisplayText =>
            $"ETD: {ETD:yyyy-MM-dd HH:mm} | ETA: {ETA:yyyy-MM-dd HH:mm}";
    }



}
