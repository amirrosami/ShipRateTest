using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.BillOfLading
{
    public class DeliveryOrder : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int Serial { get; set; }
        public int DOOperationalModeId { get; set; }

        public Guid? CarrierId { get; set; }
        public Guid? CarrierAgentId { get; set; }
        public Guid? ExternalIssuerId { get; set; }
        public string? ExternalIssuerText { get; set; }
        public DateTime? ReceivedAtDateTime { get; set; }
        public Guid? ReceivedByUserId { get; set; }
        public int? ReceiveMethodId { get; set; }


        public Guid BLId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? TelexMessageId { get; set; }

        public Guid? CustomsOfficeId { get; set; }
        public Guid? ConsigneeId { get; set; }
        public string? ConsigneeText { get; set; } //صرفا جهت ذخیره در ترخیصیه
        public Guid? NotifyPartyId { get; set; }
        public string? NotifyPartyText { get; set; }  //صرفا جهت ذخیره در ترخیصیه

        public Guid? DeliveryToPersonId { get; set; }//شرکتی که ترخیصیه به نام آن صادر شده است
        public string? DeliveryToPersonText { get; set; }  //شرکتی که ترخیصیه به نام آن صادر شده استText
        public string? DeliveryToPersonName { get; set; }  //شخص تحویل گیرنده ترخیصیه
        public string? DeliveryToPersonNationalId { get; set; }  //شماره ملی شخص
        public string? DeliveryToPersonMobile { get; set; }  //شماره موبایل شخص
        public int? DeliveryMethodId { get; set; }

        public bool? IsVerified { get; set; }
        public Guid? VerifiedByUserId { get; set; }
        public DateTime? VerifiedAtDateTime { get; set; }
        public string? VerificationRemarks { get; set; }

        public Guid? VoyageId { get; set; }
        public string? VesselName { get; set; } = string.Empty;
        public string? VoyageNumber { get; set; } = string.Empty;

        public string? DONo { get; set; } = string.Empty;
        public DateTime? DoDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public string? EPLTrackCode { get; set; }
        public string? WarehouseReceiptTrackCode { get; set; }

        public Guid? DischargePortId { get; set; }
        public string? DeliveryLocation { get; set; } = string.Empty;


        public DateTime? CollectedDate { get; set; }
        public string? CollectedByPerson { get; set; }

        public Guid? TIPId { get; set; }

        public int? ClearanceTypeId { get; set; }
        public int? OriginalBLCount { get; set; }
        public int? BLReleaseTypeId { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public int? StatusId { get; set; }



        //مهم و حساس 
        public Guid? ReleasedByUserId { get; set; }
        public DateTime? ReleasedAtDateTime { get; set; }
        public string? ReleaseComments { get; set; } = string.Empty;

        public DateTime? CancelledAt { get; set; }
        public Guid? CancelUserId { get; set; }
        public string? CancellationReason { get; set; }


        //Relations
        public Job Job { get; set; }
        public BL BL { get; set; }
        public Company Company { get; set; }
        public Carrier Carrier { get; set; }
        public Agent CarrierAgent { get; set; }
        public BLReleaseType BLReleaseType { get; set; }
        public TelexMessage TelexMessage { get; set; }
        public Location CustomsOffice { get; set; }
        public ClearanceType ClearanceType { get; set; }
        public Person Consignee { get; set; }
        public Person NotifyParty { get; set; }

        public Person DeliveryToPerson { get; set; }
        public DOReceiveMethod DeliveryMethod { get; set; }

        public Person ExternalIssuer { get; set; }

        public Location DischargePort { get; set; }
        public DOStatus Status { get; set; }

        public ApplicationUser ReleasedByUser { get; set; }

        public ApplicationUser CreatedByUser { get; set; }
        public ApplicationUser EditedByUser { get; set; }
        public ApplicationUser CanceledByUser { get; set; }


        public ApplicationUser ReceivedByUser { get; set; }
        public DOReceiveMethod ReceiveMethod { get; set; }

        public ApplicationUser VerifiedByUser { get; set; }

        public DOOperationalMode DOOperationalMode { get; set; }


        public TIP TIP { get; set; }


        public ICollection<DeliveryOrderContainer> Containers { get; set; } = new HashSet<DeliveryOrderContainer>();



    }
    public class DOOperationalMode : SystemEntity
    {
        public string DOOperationalModeName { get; set; } = string.Empty;
        public string DOOperationalModeLocalName { get; set; } = string.Empty;
    }
    public class DOReceiveMethod : SystemEntity
    {
        public string DOReceiveMethodName { get; set; } = string.Empty;
        public string DOReceiveMethodLocalName { get; set; } = string.Empty;
    }

    public class DeliveryOrderContainer : BusinessEntity
    {
        public Guid DeliveryOrderId { get; set; }
        public Guid BLContainerId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public int ContainerSizeTypeId { get; set; }
        public string? SealNumber { get; set; } = string.Empty;
        public decimal GrossWeightKG { get; set; }
        public int cntOwnTypeId { get; set; }
        //Relations
        public DeliveryOrder? DeliveryOrder { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
        public BLContainer? BLContainer { get; set; }
        public CntOwnType? CntOwnType { get; set; }
        public ICollection<DeliveryOrderContainerPackage> Packages { get; set; } = new HashSet<DeliveryOrderContainerPackage>();
    }

    public class DeliveryOrderContainerPackage : BusinessEntity
    {
        public Guid DeliveryOrderContainerId { get; set; }
        public int PackageCount { get; set; }
        public int? PackageTypeId { get; set; }

        public decimal? GrossWeightKG { get; set; }
        public decimal? NetWeightKG { get; set; }
        public decimal? VolumeCBM { get; set; }

        public string? HSCode { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? DescriptionOfGoods { get; set; }

        //Relations
        public PackageType? PackageType { get; set; }
        public DeliveryOrderContainer? DeliveryOrderContainer { get; set; }
    }

    public class FreightLetter : BusinessEntity
    {
        public Guid? DOId { get; set; }
        public Guid? BLId { get; set; }
        public string? MBLNo { get; set; }
        public string? HBLNo { get; set; }
        public Guid CustomsOfficeId { get; set; }
        public decimal FreightValue { get; set; }
        public int CurrencyId { get; set; }
        public string FromLocationName { get; set; }
        public string ToLocationName { get; set; }
        public string CustomerName { get; set; }
        public int LetterNo { get; set; }
        public DateTime LetterDate { get; set; }

        //Relations
        public DeliveryOrder DeliveryOrder { get; set; }
        public BL BL { get; set; }
        public Location CustomsOffice { get; set; }
        public Currency Currency { get; set; }
    }
}
