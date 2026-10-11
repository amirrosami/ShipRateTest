using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class VGM : BusinessEntity
    {
        public Guid ShipmentOperationPlanId { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid JobCompanyRoleId { get; set; }

        public Guid? FullGateInId { get; set; }

        public string? ReferenceNo { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string? Remarks { get; set; }


        //Relations
        public ShipmentOperationPlan ShipmentOperationPlan { get; set; }
        public Shipment Shipment { get; set; }
        public JobCompanyRole JobCompanyRole { get; set; }
        public FullGateIn FullGateIn { get; set; }

    }

    public class VGMContainer : BusinessEntity
    {
        public Guid VGMId { get; set; }
        public Guid? FullGateInContainerId { get; set; }
        public Guid? ContainerId { get; set; }
        public string? ContainerNo { get; set; }
        public int ContainerSizeTypeId { get; set; }


        public DateTime WeighedAt { get; set; }

        public int WeighingMethod { get; set; }        // 1 Or 2
        public decimal VGMWeight { get; set; }
        public int WeightUnitId { get; set; }


        public decimal? CargoWeight { get; set; }
        public decimal? PackingWeight { get; set; }
        public decimal? TareWeight { get; set; }


        public string? WeighbridgeName { get; set; }   //نام باسکول
        public string? WeighbridgeTicketNo { get; set; }  //شماره قبض باسکول

        public string Remarks { get; set; }






        //Relations
        public VGM VGM { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
        public FullGateInContainer FullGateInContainer { get; set; }
        public Container Container { get; set; }
        public MeasurementUnit WeightUnit { get; set; }

    }




}
