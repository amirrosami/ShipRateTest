using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Customs
{
    public class CustomsClearance : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid JobId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? VGMId { get; set; }

        public Guid? ShipmentOperationPlanId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? JobCompanyRoleId { get; set; }

        public string? ClearanceNo { get; set; }
        public DateTime? ClearanceDate { get; set; }




        public Guid? CustomOfficeId { get; set; }
        public int? DeclarationTypeId { get; set; }
        public string DeclarationNumber { get; set; } = string.Empty;
        public int DeclarationVersion { get; set; } = 1;
        public DateTime? DeclarationDate { get; set; }

        public Guid ImporterExporterId { get; set; }
        public Guid CustomsBrokerId { get; set; }
        public string? DeclarantName { get; set; }
        public string? DeclarantLicense { get; set; }


        public string? ReferenceNumber { get; set; }




        //ارزش ها
        public decimal CIFValue { get; set; } = 0;
        public decimal? CifExchangeRate { get; set; }
        public int? CIFCurrencyId { get; set; }
        public decimal FOBValue { get; set; } = 0;
        public int? FOBCurrencyId { get; set; }
        public decimal FreightValue { get; set; } = 0;
        public decimal InsuranceValue { get; set; } = 0;
        public decimal CustomsValue { get; set; } = 0;
        public int? CustomsValueCurrencyId { get; set; }


        //حقوق و عوارض
        public decimal TotalDutyAmount { get; set; } = 0;
        public decimal TotalTaxAmount { get; set; } = 0;
        public decimal TotalOtherChargesAmount { get; set; } = 0;
        public decimal GrandTotalAmount { get; set; } = 0;
        public int? LocalCurrencyId { get; set; }

        public int? IncoTermId { get; set; }

        //-- کالا
        public string GoodsDescription { get; set; } = "";
        public int? PackageTypeId { get; set; }
        public int? TotalPackages { get; set; }
        public decimal? TotalGrossWeightKG { get; set; }
        public decimal? TotalNetWeightKG { get; set; }

        //--مسیر
        public int? OriginCountryId { get; set; }
        public int? DestinationCountryId { get; set; }
        public Guid? LoadPortId { get; set; }
        public Guid? DischargePortId { get; set; }


        public string? LCNumber { get; set; }
        public string? ProformaInvoiceNumber { get; set; }
        public string? CommercialInvoiceNumber { get; set; }


        public string? MRN { get; set; }
        public Guid? FreeZoneId { get; set; }
        public string? Remarks { get; set; }

        public int CustomsStatusId { get; set; }

        //Relations
        public Company? Company { get; set; }
        public Job? Job { get; set; }
        public BL? BL { get; set; }
        public Location? CustomOffice { get; set; }
        public Person? ImporterExporter { get; set; }
        public Person? CustomsBroker { get; set; }
        public Currency? CIFCurrency { get; set; }
        public Currency? FOBCurrency { get; set; }
        public Currency? CustomsValueCurrency { get; set; }
        public Incoterm? Incoterm { get; set; }
        public Person? Branch { get; set; }
        public Location? FreeZone { get; set; }
        public Location? LoadPort { get; set; }
        public CustomsStatus? CustomsStatus { get; set; }
        public CustomsDeclarationType? DeclarationType { get; set; }
        public ShipmentOperationPlan? ShipmentOperationPlan { get; set; }
        public Shipment? Shipment { get; set; }
        public JobCompanyRole? JobCompanyRole { get; set; }
        public VGM? VGM { get; set; }

        public ICollection<CustomsClearanceItem> Items { get; set; } = new List<CustomsClearanceItem>();
        public ICollection<CustomsClearanceContainer> Containers { get; set; } = new List<CustomsClearanceContainer>();
        public ICollection<CustomsClearanceCharge> Charges { get; set; } = new List<CustomsClearanceCharge>();
        public ICollection<CustomsClearanceLicense> Licenses { get; set; } = new List<CustomsClearanceLicense>();
        public ICollection<CustomsInspection> Inspections { get; set; } = new List<CustomsInspection>();
        public ICollection<CustomsHold> Holds { get; set; } = new List<CustomsHold>();

    }

    public class CustomsClearanceItem : BusinessEntity
    {
        public Guid CustomsClearanceId { get; set; }
        public int ItemSequence { get; set; }
        public Guid CountryHSCodeId { get; set; }
        public string HSCodeToSave { get; set; }
        public string? GoodsDescriptionEN { get; set; } = string.Empty;
        public string? GoodsDescriptionLocal { get; set; } = string.Empty;

        public decimal? Quantity { get; set; }
        public int? MeasurementUnitId { get; set; }
        public decimal? StatisticalQuantity { get; set; }
        public int? StatisticalMeasurementUnitId { get; set; }

        //ارزش
        public decimal? UnitPrice { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? CustomsValue { get; set; }
        public decimal? CustomsValueIRR { get; set; }    //ارزش ریالی

        //حقوق و عوارض هر قلم
        public decimal DutyRate { get; set; }
        public decimal DutyAmount { get; set; }
        public decimal CommercialBenefitRate { get; set; }
        public decimal CommercialBenefitAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal TotalDutiesItem { get; set; }


        //-- ترجیحات
        public string? PreferentialTariffCode { get; set; }
        public decimal? PreferentialDutyRate { get; set; }
        public string? CertificateOfOriginRef { get; set; }



        public string? Remarks { get; set; }

        //Relations
        public CustomsClearance CustomsClearance { get; set; }
        public CountryHSCode CountryHSCode { get; set; }
        public Currency Currency { get; set; }

    }


    public class CustomsClearanceContainer : BusinessEntity
    {
        public Guid CustomsClearanceId { get; set; }
        public Guid? ContainerId { get; set; }
        public string ContainerNo { get; set; }
        public int ContainerSizeTypeId { get; set; }

        public Guid? FullGateInContainerId { get; set; }
        public Guid? VGMContainerId { get; set; }

        public string? SealNo { get; set; }
        public int? PackageTypeId { get; set; }
        public decimal GrossWeightKg { get; set; }
        public decimal NetWeightKg { get; set; }
        public string? Remarks { get; set; }


        //Relations
        public CustomsClearance CustomsClearance { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
        public Container Container { get; set; }
        public FullGateInContainer FullGateInContainer { get; set; }
        public VGMContainer VGMContainer { get; set; }
        public PackageType PackageType { get; set; }
    }



    public class CustomsClearanceCharge : BusinessEntity
    {
        public Guid CustomsClearanceId { get; set; }
        /*
         * اگر هزینه مربوط به کل پرونده باشد، این فیلد Null می‌ماند.
         * اگر هزینه مربوط به یک قلم کالای خاص باشد، مقدار می‌گیرد.
         */
        public Guid? CustomsClearanceItemId { get; set; }
        public int ChargeTypeId { get; set; }
        public string ChargeDescription { get; set; } = null!;
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal LocalAmount { get; set; }
        public bool IsTax { get; set; }
        public bool IsSystemCalculated { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentReferenceNo { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public CustomsClearance CustomsClearance { get; set; } = null!;
        public CustomsClearanceItem? CustomsClearanceItem { get; set; }
        public ChargeType ChargeType { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
    }


    public class CustomsClearanceLicense : BusinessEntity
    {
        public Guid CustomsClearanceId { get; set; }
        public int LicenseTypeId { get; set; }
        public string? IssuingOrganization { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LicenseStatusId { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public CustomsClearance CustomsClearance { get; set; }
        public LicenseType LicenseType { get; set; }
        public LicenseStatus LicenseStatus { get; set; }
    }
    public class CustomsHold : BusinessEntity
    {
        public Guid CustomsClearanceId { get; set; }
        public int CustomsHoldReasonId { get; set; }
        public DateTime CustomsHoldStartDatetime { get; set; }
        public DateTime CustomsHoldReleaseDatetime { get; set; }
        public string Remarks { get; set; }

        //Relations
        public CustomsClearance CustomsClearance { get; set; }
        public CustomsHoldReason CustomsHoldReason { get; set; }
    }
    public class CustomsHoldReason : SystemEntity
    {
        public string HoldReasonName { get; set; } = string.Empty;
        public string HoldReasonLocalName { get; set; } = string.Empty;
    }

    public class CustomsInspection : BusinessEntity
    {
        public Guid CustomsClearanceId { get; set; }
        public int InspectionTypeId { get; set; }
        public int InspectionMethodId { get; set; }
        public int? InspectionResultId { get; set; }

        public DateTime InspectionDate { get; set; }
        public string? InspectorName { get; set; }
        public string? Location { get; set; }
        public string? Findings { get; set; }
        public bool DiscrepancyFound { get; set; }
        public string? DiscrepancyDescription { get; set; }
        public bool CorrectionsRequired { get; set; }
        public string? CorrectionsDescription { get; set; }
        public string? Remarks { get; set; }



        //Relations
        public CustomsClearance? CustomsClearance { get; set; }
        public InspectionType? InspectionType { get; set; }
        public InspectionMethod? InspectionMethod { get; set; }
        public InspectionResult? InspectionResult { get; set; }
    }


}























