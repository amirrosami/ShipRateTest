using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequestPackage : BusinessEntity
    {
        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }

        public int PackageTypeId { get; set; }

        public int UnitCount { get; set; }

        public decimal? UnitLength { get; set; }
        public decimal? UnitWidth { get; set; }
        public decimal? UnitHeight { get; set; }
        public int? LengthMeasurementUnitId { get; set; }

        public decimal? UnitVolume { get; set; }
        public int? VolumeMeasurementUnitId { get; set; }


        public decimal? UnitGrossWeight { get; set; }
        public decimal? UnitNetWeight { get; set; }
        public int? WeightMeasurementUnitId { get; set; }

        public decimal? TotalLength { get; set; }
        public decimal? TotalWidth { get; set; }
        public decimal? TotalHeight { get; set; }
        public decimal? TotalVolume { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }

        public string? Remarks { get; set; }

        //Relations
        public RateRequest? RateRequest { get; set; }
        public Job? Job { get; set; }
        public Booking? Booking { get; set; }
        public PackageType PackageType { get; set; }
        public MeasurementUnit? LengthMeasurementUnit { get; set; }
        public MeasurementUnit? VolumeMeasurementUnit { get; set; }
        public MeasurementUnit? WeightMeasurementUnit { get; set; }
    }
}
