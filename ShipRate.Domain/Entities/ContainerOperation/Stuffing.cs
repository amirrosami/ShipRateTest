using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class Stuffing : BusinessEntity
    {
        public Guid ShipmentOperationPlanId { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid JobCompanyRoleId { get; set; }

        public string? StuffingReferenceNo { get; set; }
        public Guid? StuffingLocationId { get; set; }

        public DateTime? StuffingStartedAt { get; set; }
        public DateTime? StuffingEndAt { get; set; }

        public string? SupervisorName { get; set; }
        public string? Remarks { get; set; }

        public DateTime? ConfirmedAt { get; set; }
        public Guid? ConfirmedByUser { get; set; }


        //Relations
        public ShipmentOperationPlan ShipmentOperationPlan { get; set; }
        public Shipment Shipment { get; set; }
        public JobCompanyRole JobCompanyRole { get; set; }
        public Location StuffingLocation { get; set; }

    }


    public class StuffingContainer : BusinessEntity
    {
        public Guid StuffingId { get; set; }
        public Guid? EmptyPickupContainerId { get; set; }
        public Guid? ContainerId { get; set; }
        public string? ContainerNo { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public string? SealNo { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal TareWeight { get; set; }
        public decimal NetWeigth { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public Stuffing Stuffing { get; set; }
        public Container Container { get; set; }
        public EmptyPickupContainer EmptyPickupContainer { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
    }

    public class StuffingContainerItem : BusinessEntity
    {
        public Guid StuffingContainerId { get; set; }
        public string? CommodityDescription { get; set; }
        public int PackageTypeId { get; set; }
        public int PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Volume { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public StuffingContainer StuffingContainer { get; set; }
        public PackageType PackageType { get; set; }
    }



}
