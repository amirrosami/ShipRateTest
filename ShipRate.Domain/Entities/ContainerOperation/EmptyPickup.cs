using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class EmptyPickup : BusinessEntity
    {
        public Guid ShipmentOperationPlanId { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid JobCompanyRoleId { get; set; }

        public Guid? RTSInstructionId { get; set; }

        public string? PickupReferenceNo { get; set; }
        public Guid? PickupLocationId { get; set; }
        public DateTime PickupdateTime { get; set; }

        public Guid? DriverId { get; set; }
        public string? DriverName { get; set; }

        public Guid? TruckId { get; set; }
        public string? TruckPlateNo { get; set; }
        public string? TrailerPlateNo { get; set; }

        public string? Remarks { get; set; }

        public DateTime? ConfirmedAt { get; set; }
        public Guid? ConfirmedBy { get; set; }


        //Relations
        public ShipmentOperationPlan ShipmentOperationPlan { get; set; }
        public Shipment Shipment { get; set; }
        public JobCompanyRole JobCompanyRole { get; set; }
        public Location PickupLocation { get; set; }
        public Driver Driver { get; set; }
        public Truck Truck { get; set; }
    }
}
