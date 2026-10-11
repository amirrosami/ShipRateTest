using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class FullGateIn : BusinessEntity
    {
        public Guid ShipmentOperationPlanId { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid JobCompanyRoleId { get; set; }

        public Guid? StuffingId { get; set; }
        public string? GateInReferenceNo { get; set; }
        public Guid? TerminalId { get; set; }

        public Guid? DriverId { get; set; }
        public string? DriverName { get; set; }
        public Guid? TruckId { get; set; }
        public string? TruckPlateNo { get; set; }
        public string? TrailerPlateNo { get; set; }



        public string? Remarks { get; set; }



        //Relations
        public ShipmentOperationPlan ShipmentOperationPlan { get; set; }
        public Shipment Shipment { get; set; }
        public JobCompanyRole JobCompanyRole { get; set; }
        public Location Terminal { get; set; }
        public Stuffing Stuffing { get; set; }
        public Driver Driver { get; set; }
        public Truck Truck { get; set; }
    }




    public class FullGateInContainer : BusinessEntity
    {
        public Guid FullGateInId { get; set; }
        public Guid? StuffingContainerId { get; set; }

        public Guid? ContainerId { get; set; }
        public string ContainerNo { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public string? SealNo { get; set; }

        public DateTime GateInAt { get; set; }
        public string? EIRNo { get; set; }
        public string? GatePassNo { get; set; }

        public decimal? GrossWeight { get; set; }
        public decimal? VGMWeight { get; set; }




        public int? ConditionStatusId { get; set; }
        public string? DamageRemarks { get; set; }


        public string? Remarks { get; set; }
        //Relations
        public FullGateIn FullGateIn { get; set; }
        public StuffingContainer StuffingContainer { get; set; }
        public Container Container { get; set; }
        public ContainerConditionStatus ContainerConditionStatus { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
    }

}

//EIR=Equipment Interchange Receipt