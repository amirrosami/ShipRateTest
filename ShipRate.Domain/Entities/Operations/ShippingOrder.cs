using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Customs;

namespace ShipRate.Domain.Entities.Operations
{
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class ShippingOrder : BusinessEntity
    {
        public Guid JobId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentOperationPlanId { get; set; }
        public Guid? Booking { get; set; }
        public Guid? BL { get; set; }

        public string ShippingOrderNo { get; set; }
        public Guid? SubmittedByJobCompanyRoleId { get; set; }
        public Guid? SubmittedByPartyId { get; set; }
        public Guid? SubmittedToJobCompanyRoleId { get; set; }
        public Guid? SubmittedToPartyId { get; set; }

        public int RevisionNo { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? CanceledDate { get; set; }

        public int? PaymentTypeId { get; set; }
        public int? IncotermId { get; set; }
        public Guid? VesselId { get; set; }
        public string? VoyageNo { get; set; }

        public string? CargoDescription { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? InternalRemarks { get; set; }
        public string? ExternalRemarks { get; set; }


        //Relations
        public Job Job { get; set; }
        public Shipment Shipment { get; set; }
        public ShipmentOperationPlan ShipmentOperationPlan { get; set; }
        public JobCompanyRole SubmittedByJobCompanyRole { get; set; }
        public JobCompanyRole SubmittedToJobCompanyRole { get; set; }
        public Person SubmittedByParty { get; set; }
        public Person SubmittedToParty { get; set; }
        public PaymentType PaymentType { get; set; }
        public Incoterm Incoterm { get; set; }
        public Vessel Vessel { get; set; }
    }
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class ShippingOrderParty : BusinessEntity
    {
        public Guid ShippingOrderId { get; set; }
        public int RoleTypeId { get; set; }
        public string PartyFullText { get; set; }
        //Relations
        public ShippingOrder ShippingOrder { get; set; }
        public RoleType RoleType { get; set; }
    }
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class ShippingOrderCargoLine : BusinessEntity
    {
        public Guid ShippingOrderId { get; set; }
        public int SequenceNo { get; set; }

        public Guid? CommodityId { get; set; }
        public string? CommodityText { get; set; }
        public Guid? CountryHSCodeId { get; set; }
        public string HSCode { get; set; }

        public int PackageTypeId { get; set; }
        public string GoodsDescription { get; set; }
        public string? MarksAndNumbers { get; set; }


        public decimal? PackageQty { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }

        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? GeightCm { get; set; }

        public int? DGClassId { get; set; }
        public string? UnNumber { get; set; }
        public decimal? FlashPoint { get; set; }
        public string? TemperatureSetting { get; set; }

        public int? OriginCountryId { get; set; }
        public string? Remarks { get; set; }




        //Relations
        public ShippingOrder ShippingOrder { get; set; }
        public CargoCommodity CargoCommodity { get; set; }
        public CountryHSCode CountryHSCode { get; set; }
        public PackageType PackageType { get; set; }
        public Country OriginCountry { get; set; }
        public DGClass DGClass { get; set; }

    }
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class ShippingOrderContainer : BusinessEntity
    {
        public Guid ShippingOrderId { get; set; }
        public int SequenceNo { get; set; }

        public Guid? ShipmentContainerId { get; set; }
        public Guid? ContainerId { get; set; }
        public string ContainerNo { get; set; }
        public int ContainerSizeTypeId { get; set; }



        public int? StuffingTypeId { get; set; }

        public Guid? FullGateInContainerId { get; set; }
        public Guid? VGMContainerId { get; set; }
        public Guid? CustomsClearanceContainerId { get; set; }

        public string? SealNo { get; set; }
        public decimal? PackageQty { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? TareWeightKg { get; set; }
        public decimal? VGMKg { get; set; }
        public decimal? VolumeCBM { get; set; }


        public string? TemperatureSetting { get; set; }
        public string? VentilationSetting { get; set; }
        public string? CommoditySummary { get; set; }
        public string? Remarks { get; set; }


        //Relations
        public ShippingOrder ShippingOrder { get; set; }
        public ShipmentContainer ShipmentContainer { get; set; }
        public Container Container { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
        public StuffingType StuffingType { get; set; }

        public FullGateInContainer FullGateInContainer { get; set; }
        public VGMContainer VGMContainer { get; set; }
        public CustomsClearanceContainer CustomsClearanceContainer { get; set; }
    }

    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class ShippingOrderContainerCargoAllocation : BusinessEntity
    {
        public Guid ShippingOrderContainerId { get; set; }
        public Guid ShippingOrderCargoLineId { get; set; }

        public int PackageTypeId { get; set; }
        public decimal? PackageQty { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }

        public string? MarksAndNumbers { get; set; }


        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? GeightCm { get; set; }


        public bool IsPartial { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public ShippingOrderContainer ShippingOrderContainer { get; set; }
        public ShippingOrderCargoLine ShippingOrderCargoLine { get; set; }
        public PackageType PackageType { get; set; }
    }
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************

}


///Ports should be added
///