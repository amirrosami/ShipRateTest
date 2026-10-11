using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Operations
{
    public class FlightDate : BusinessEntity
    {
        public Guid FlightId { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        //public Guid TransportMovementId { get; set; }
        public decimal DelayHours { get; set; }
        public string? Description { get; set; } = string.Empty;
        //Relations
        public Flight Flight { get; set; }
        //public TransportMovement TransportMovement { get; set; }
    }
}
