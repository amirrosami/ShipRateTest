using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobRouting : BusinessEntity
    {
        public Guid JobId { get; set; }

        public Guid OriginLocationId { get; set; }
        public Guid DestinationLocationId { get; set; }
        public string? PlaceOfReceiptName { get; set; } = string.Empty;
        public string? PlaceOfDeliveryName { get; set; } = string.Empty;
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public int MovementTypeId { get; set; }

        //Relations 
        public Job Job { get; set; }
        public Location OriginLocation { get; set; }
        public Location DestinationLocation { get; set; }
        public MovementType MovementType { get; set; }

    }
}
