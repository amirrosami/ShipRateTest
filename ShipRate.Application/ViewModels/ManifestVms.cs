namespace ShipRate.Application.ViewModels
{
    public sealed class BLManifestReportVm
    {
        public Guid BLId { get; init; }

        // Document
        public string BLNo { get; init; } = string.Empty;
        public string? MasterBLNo { get; init; }
        public string? BLType { get; init; }
        public string? BLLevel { get; init; }
        public DateTime? BLIssueDate { get; init; }
        public string? PlaceOfIssue { get; init; }

        public string? ManifestReference { get; init; }
        public DateTime PrintedAt { get; init; }

        // Carrier / Issuer
        public string? CarrierName { get; init; }
        public string? IssuerName { get; init; }
        public bool IssuedByUs { get; init; }

        // Commercial
        public string? FreightTerms { get; init; }
        public string? PaymentType { get; init; }
        public string? LoadType { get; init; }
        public string? MovementType { get; init; }
        public string? Incoterm { get; init; }
        public string? ExportReference { get; init; }
        public string? ForwardingAgentReference { get; init; }
        public string? MRN { get; init; }
        public string? PursuitCode { get; init; }

        // Routing
        public string? POR { get; init; }
        public string? POL { get; init; }
        public string? POD { get; init; }
        public string? PODel { get; init; }
        public string? FinalDestination { get; init; }
        public string? Origin { get; init; }
        public string? OnwardInlandRouting { get; init; }

        public DateTime? ReceivedForShipmentDate { get; init; }
        public DateTime? ShippedOnBoardDate { get; init; }

        // Parties
        public BLManifestPartyVm? Shipper { get; init; }
        public BLManifestPartyVm? Consignee { get; init; }
        public BLManifestPartyVm? NotifyParty { get; init; }

        // Transport
        public List<BLManifestVoyageVm> Voyages { get; init; } = [];

        // Cargo / Containers
        public List<BLManifestContainerVm> Containers { get; init; } = [];
        public List<BLManifestUnallocatedCargoVm> UnallocatedCargoes { get; init; } = [];
        public List<BLManifestCargoRowVm> CargoRows { get; init; } = [];

        // Declared totals from BL
        public int DeclaredTotalContainers { get; init; }
        public int DeclaredTotalPackages { get; init; }
        public decimal? DeclaredGrossWeightKg { get; init; }
        public decimal? DeclaredNetWeightKg { get; init; }
        public decimal? DeclaredVolumeCBM { get; init; }

        // Calculated totals from containers / allocations
        public int CalculatedTotalContainers { get; init; }
        public decimal? CalculatedGrossWeightKg { get; init; }
        public decimal? CalculatedNetWeightKg { get; init; }
        public decimal? CalculatedVolumeCBM { get; init; }

        public string? GoodsDescription { get; init; }
        public string? MarksAndNumbers { get; init; }
        public string? RemarksInManifest { get; init; }
        public string? ClauseRemarks { get; init; }
        public string? OnBoardNotation { get; init; }


        public int CalculatedTotalPackages { get; init; }






    }
    public sealed class BLManifestPartyVm
    {
        public string Title { get; init; } = string.Empty;
        public string DisplayText { get; init; } = string.Empty;

        public string? Telephone { get; init; }
        public string? Mobile { get; init; }
        public string? Fax { get; init; }
        public string? Email { get; init; }

        public bool IsToOrder { get; init; }
        public bool IsToOrderOf { get; init; }
        public bool IsToBearer { get; init; }
        public bool IsSameAsConsignee { get; init; }
    }
    public sealed class BLManifestVoyageVm
    {
        public string? VoyageType { get; init; }
        public string VesselName { get; init; } = string.Empty;
        public string VoyageNo { get; init; } = string.Empty;

        public DateTime? ShippedOnBoardDate { get; init; }
        public DateTime? ReceivedForShipmentDate { get; init; }

        public bool IsMainCarriage { get; init; }
    }
    public sealed class BLManifestContainerVm
    {
        public int RowNo { get; init; }

        public string ContainerNumber { get; init; } = string.Empty;
        public string? ContainerSizeType { get; init; }
        public string? LoadType { get; init; }
        public string? FullMT { get; init; }
        public string? OwnershipType { get; init; }

        public string? SealNumber { get; init; }
        public string? SealNumber2 { get; init; }

        public int TotalPackageCount { get; init; }
        public decimal? TotalGrossWeightKg { get; init; }
        public decimal? TotalNetWeightKg { get; init; }
        public decimal? TareWeightKg { get; init; }
        public decimal? TotalVolumeCBM { get; init; }

        public string? ExportPermissionNo { get; init; }

        public decimal? OverLength { get; init; }
        public decimal? OverWidth { get; init; }
        public decimal? OverHeight { get; init; }
        public string? DimensionUnit { get; init; }

        public List<BLManifestContainerCargoVm> Cargoes { get; init; } = [];
    }
    public sealed class BLManifestContainerCargoVm
    {
        public int RowNo { get; init; }

        public string CargoDescription { get; init; } = string.Empty;
        public string? MarksAndNumbers { get; init; }
        public string? HSCode { get; init; }

        public decimal? PackageCount { get; init; }
        public string? PackageType { get; init; }

        public decimal? GrossWeightKg { get; init; }
        public decimal? NetWeightKg { get; init; }
        public decimal? VolumeCBM { get; init; }

        public bool IsDG { get; init; }
        public string? DGClass { get; init; }
        public string? UNNumber { get; init; }
        public string? PackingGroup { get; init; }
        public string? ProperShippingName { get; init; }
        public string? MarinePollutant { get; init; }

        public bool IsTemperatureControlled { get; init; }
        public string? TemperatureRequirement { get; init; }

        public bool IsOverSized { get; init; }
        public string? Dimensions { get; init; }

        public bool IsPerishable { get; init; }
        public bool IsFragile { get; init; }

        public string? Remarks { get; init; }
    }
    public sealed class BLManifestUnallocatedCargoVm
    {
        public int Sequence { get; init; }

        public string CargoDescription { get; init; } = string.Empty;
        public string? MarksAndNumbers { get; init; }
        public string? HSCode { get; init; }

        public decimal? PackageCount { get; init; }
        public string? PackageType { get; init; }

        public decimal? GrossWeightKg { get; init; }
        public decimal? NetWeightKg { get; init; }
        public decimal? VolumeCBM { get; init; }

        public string? Remarks { get; init; }
    }
    public static class BLPartyRoleTypeIds
    {
        public const int Shipper = 51;
        public const int Consignee = 52;
        public const int NotifyParty = 53;
    }


    public sealed class BLDocumentReportVm
    {
        public Guid BLId { get; init; }
        public int CompanyId { get; init; }
        public int? TransportTypeId { get; init; }
        public string? TransportTypeName { get; init; }



        // Draft / Original behavior
        public bool IsDraft { get; init; }
        public bool IsOriginal { get; init; }
        public bool IsNegotiable { get; init; }

        public string DocumentCaption { get; init; } = "BILL OF LADING";

        // Issuer / Carrier
        public string? IssuerName { get; init; }
        public string? IssuerAddress { get; init; }
        public string? IssuerTelephone { get; init; }
        public string? IssuerEmail { get; init; }
        public string? IssuerWebsite { get; init; }

        public string? CarrierName { get; init; }

        // B/L document information
        public string BLNo { get; init; } = string.Empty;
        public string? MasterBLNo { get; init; }
        public string? BLType { get; init; }
        public string? BLLevel { get; init; }

        public DateTime? BLIssueDate { get; init; }
        public string? PlaceOfIssue { get; init; }

        public int? OriginalCount { get; init; }
        public int? CopyCount { get; init; }

        public string? ReleaseType { get; init; }
        public DateTime? ReleaseDate { get; init; }

        // Parties
        public BLManifestPartyVm? Shipper { get; init; }
        public BLManifestPartyVm? Consignee { get; init; }
        public BLManifestPartyVm? NotifyParty { get; init; }

        // Routing
        public string? PlaceOfReceipt { get; init; }
        public string? PortOfLoading { get; init; }
        public string? PortOfDischarge { get; init; }
        public string? PlaceOfDelivery { get; init; }
        public string? FinalDestination { get; init; }

        public string? PointAndCountryOfOrigin { get; init; }
        public string? OnwardInlandRouting { get; init; }

        // Sea voyage
        public string? PreCarriageBy { get; init; }
        public string? MainVesselName { get; init; }
        public string? MainVoyageNo { get; init; }
        public DateTime? ReceivedForShipmentDate { get; init; }
        public DateTime? ShippedOnBoardDate { get; init; }

        public List<BLManifestVoyageVm> Voyages { get; init; } = [];

        // Commercial
        public string? FreightTerms { get; init; }
        public string? PaymentType { get; init; }
        public string? FreightPaymentAt { get; init; }
        public string? Incoterm { get; init; }
        public string? ExportReference { get; init; }
        public string? ForwardingAgentReference { get; init; }
        public string? LCNo { get; init; }
        public string? LCBankName { get; init; }

        // Cargo
        public string? MarksAndNumbers { get; init; }
        public string? GoodsDescription { get; init; }

        public int TotalPackages { get; init; }
        public string? PackageType { get; init; }
        public decimal? GrossWeightKg { get; init; }
        public decimal? NetWeightKg { get; init; }
        public decimal? VolumeCBM { get; init; }

        public List<BLDocumentContainerVm> Containers { get; init; } = [];

        // Clauses / remarks
        public string? ClauseRemarks { get; init; }
        public string? OnBoardNotation { get; init; }
        public string? RemarksInBL { get; init; }

        // Signature
        public string? SignedByName { get; init; }
        public string? SignedByTitle { get; init; }

        public DateTime PrintedAt { get; init; }




        // Air transport / AWB
        public string? MainAirlineName { get; init; }
        public string? MainFlightNo { get; init; }
        public DateTime? MainFlightDate { get; init; }
        public string? MainFlightDateLocal { get; init; }

        public string? DeclaredValueForCarriage { get; init; }
        public string? DeclaredValueForCustoms { get; init; }
        public string? HandlingInformation { get; init; }

        public List<BLDocumentFlightVm> Flights { get; init; } = [];

        // Road transport / CMR
        public string? MainTruckNo { get; init; }
        public string? MainTrailerNo { get; init; }
        public string? MainDriverName { get; init; }

        public List<BLDocumentTripVm> Trips { get; init; } = [];





    }
    public sealed class BLDocumentTripVm
    {
        public int RowNo { get; init; }

        public string? TruckNo { get; init; }
        public string? TrailerNo { get; init; }

        public string? DriverName { get; init; }
        public string? DriverPassportNo { get; init; }

        public int? PackageCount { get; init; }
        public decimal? GrossWeight { get; init; }

        public DateTime? LoadingDate { get; init; }
        public DateTime? DeliveryDate { get; init; }

        public string? CMRNo { get; init; }
        public string? TIRNo { get; init; }
        public string? ManifestNo { get; init; }
    }
    public sealed class BLDocumentFlightVm
    {
        public int RowNo { get; init; }

        public string? AirlineName { get; init; }
        public string? FlightNo { get; init; }

        public DateTime? FlightDate { get; init; }
        public string? FlightDateLocal { get; init; }

        public DateTime? ETD { get; init; }
        public DateTime? ETA { get; init; }

        public DateTime? ActualDeparture { get; init; }
        public DateTime? ActualArrival { get; init; }

        public decimal DelayHours { get; init; }

        public string? DeclaredValueForCarriage { get; init; }
        public string? DeclaredValueForCustoms { get; init; }
        public string? HandlingInformation { get; init; }
    }
    public sealed class BLDocumentContainerVm
    {
        public int RowNo { get; init; }

        public string ContainerNumber { get; init; } = string.Empty;
        public string? ContainerSizeType { get; init; }

        public string? SealNumber { get; init; }
        public string? SealNumber2 { get; init; }

        public int TotalPackageCount { get; init; }
        public decimal? TotalGrossWeightKg { get; init; }
        public decimal? TotalVolumeCBM { get; init; }

        public string? OwnType { get; init; }
        public string? MovementType { get; init; }
        public decimal TareWeight { get; init; }
        public string? PackageType { get; init; }

    }





    public sealed class ContainerPackingListReportVm
    {
        public Guid BLId { get; init; }

        public string BLNo { get; init; } = string.Empty;
        public string? MasterBLNo { get; init; }

        public string? CompanyName { get; init; }
        public string? CarrierName { get; init; }

        public string? ShipperText { get; init; }
        public string? ConsigneeText { get; init; }

        public string? PlaceOfReceipt { get; init; }
        public string? PortOfLoading { get; init; }
        public string? PortOfDischarge { get; init; }
        public string? PlaceOfDelivery { get; init; }

        public string? VesselName { get; init; }
        public string? VoyageNo { get; init; }

        public bool IsCancelled { get; init; }
        public DateTime PrintedAt { get; init; }

        public int TotalContainers { get; init; }
        public int TotalPackages { get; init; }

        public decimal? TotalGrossWeightKg { get; init; }
        public decimal? TotalNetWeightKg { get; init; }
        public decimal? TotalVolumeCBM { get; init; }

        public List<ContainerPackingListContainerVm> Containers { get; init; } = [];
    }
    public sealed class ContainerPackingListContainerVm
    {
        public int RowNo { get; init; }

        public string ContainerNumber { get; init; } = string.Empty;
        public string? ContainerSizeType { get; init; }
        public string? ISOCode { get; init; }

        public string? SealNumber { get; init; }
        public string? SealNumber2 { get; init; }

        public decimal? TareWeightKg { get; init; }
        public decimal? MaxGrossWeightKg { get; init; }
        public decimal? VgmWeightKg { get; init; }

        public bool IsReefer { get; init; }
        public decimal? TemperatureCelsius { get; init; }
        public string? TemperatureUnit { get; init; }

        public bool HasDangerousGoods { get; init; }

        public int TotalPackages { get; init; }
        public decimal? TotalGrossWeightKg { get; init; }
        public decimal? TotalNetWeightKg { get; init; }
        public decimal? TotalVolumeCBM { get; init; }

        public string? ContainerRemarks { get; init; }

        public List<ContainerPackingListCargoVm> CargoItems { get; init; } = [];
    }

    public sealed class ContainerPackingListCargoVm
    {
        public int RowNo { get; init; }

        public string? CommodityName { get; init; }
        public string? CargoDescription { get; init; }
        public string? MarksAndNumbers { get; init; }

        public int PackageCount { get; init; }
        public string? PackageType { get; init; }

        public decimal? GrossWeightKg { get; init; }
        public decimal? NetWeightKg { get; init; }
        public decimal? VolumeCBM { get; init; }

        public bool IsDangerousGoods { get; init; }
        public string? UNNo { get; init; }
        public string? IMOClass { get; init; }
        public string? PackingGroup { get; init; }

        public string? Remarks { get; init; }
    }


    public sealed class CargoAllocationByContainerReportVm
    {
        public Guid BLId { get; init; }

        public string BLNo { get; init; } = string.Empty;
        public string? MasterBLNo { get; init; }

        public string? CompanyName { get; init; }
        public string? CarrierName { get; init; }

        public string? ShipperText { get; init; }
        public string? ConsigneeText { get; init; }

        public string? VesselName { get; init; }
        public string? VoyageNo { get; init; }

        public string? PortOfLoading { get; init; }
        public string? PortOfDischarge { get; init; }

        public bool IsCancelled { get; init; }

        public int TotalContainers { get; init; }
        public int TotalAllocationRows { get; init; }
        public int TotalPackages { get; init; }

        public decimal? TotalGrossWeightKg { get; init; }
        public decimal? TotalNetWeightKg { get; init; }
        public decimal? TotalVolumeCBM { get; init; }

        public DateTime PrintedAt { get; init; }

        public List<CargoAllocationByContainerRowVm> Rows { get; init; } = [];
    }


    public sealed class CargoAllocationByContainerRowVm
    {
        public int RowNo { get; init; }

        // Container
        public string ContainerNumber { get; init; } = string.Empty;
        public string? ContainerSizeType { get; init; }
        public string? ISOCode { get; init; }

        public string? SealNumber { get; init; }
        public string? SealNumber2 { get; init; }

        public bool IsReefer { get; init; }
        public decimal? TemperatureCelsius { get; init; }

        // Cargo allocation
        public string? CommodityName { get; init; }
        public string? CargoDescription { get; init; }
        public string? MarksAndNumbers { get; init; }

        public int PackageCount { get; init; }
        public string? PackageType { get; init; }

        public decimal? GrossWeightKg { get; init; }
        public decimal? NetWeightKg { get; init; }
        public decimal? VolumeCBM { get; init; }

        // Dangerous goods
        public bool IsDangerousGoods { get; init; }
        public string? UNNo { get; init; }
        public string? IMOClass { get; init; }
        public string? PackingGroup { get; init; }

        public string? Remarks { get; init; }
    }

    public sealed class BLManifestCargoRowVm
    {
        public int RowNo { get; init; }

        // Container context
        public int ContainerRowNo { get; init; }

        public string ContainerNumber { get; init; } = string.Empty;
        public string? ContainerSizeType { get; init; }
        public string? ContainerLoadType { get; init; }
        public string? FullMT { get; init; }
        public string? OwnershipType { get; init; }

        public string? SealNumber { get; init; }
        public string? SealNumber2 { get; init; }

        public decimal? ContainerTareWeightKg { get; init; }
        public string? ExportPermissionNo { get; init; }

        // Cargo
        public string CargoDescription { get; init; } = string.Empty;
        public string? MarksAndNumbers { get; init; }
        public string? HSCode { get; init; }

        public decimal? PackageCount { get; init; }
        public string? PackageType { get; init; }

        public decimal? GrossWeightKg { get; init; }
        public decimal? NetWeightKg { get; init; }
        public decimal? VolumeCBM { get; init; }

        // DG
        public bool IsDG { get; init; }
        public string? DGClass { get; init; }
        public string? UNNumber { get; init; }
        public string? PackingGroup { get; init; }
        public string? ProperShippingName { get; init; }
        public string? MarinePollutant { get; init; }

        // Special cargo
        public bool IsTemperatureControlled { get; init; }
        public string? TemperatureRequirement { get; init; }

        public bool IsOverSized { get; init; }
        public string? Dimensions { get; init; }

        public bool IsPerishable { get; init; }
        public bool IsFragile { get; init; }

        public string? Remarks { get; init; }

        // FastReport display-friendly fields
        public string SealDisplay { get; init; } = string.Empty;
        public string DGDisplay { get; init; } = string.Empty;
        public string SpecialHandlingDisplay { get; init; } = string.Empty;
    }

}
