using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.ContainerOperation;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentContainer : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public string? SealNumber { get; set; }
        public int TotalPackages { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? VGMWeightKg { get; set; }
        public decimal? TareWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }

        //Relations
        public Shipment Shipment { get; set; }
        public ContainerUse ContainerUse { get; set; }
    }
}

