using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShipRate.Common.ExternalServices.Tax;
using ShipRate.Common.Notifications.Models;
using ShipRate.Common.Report;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.BookingSpec;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Marketting;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Contract;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Customs;
using ShipRate.Domain.Entities.DetentionCalculation;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Domain.Entities.NOA;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Operations.TasksManage;
using ShipRate.Domain.Entities.PettyCashControl;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Domain.Entities.Reports;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.ServiceRequests;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Domain.Entities.TransitOperation;
using ShipRate.Domaiun.Entities.Customers;
using System.Linq.Expressions;
using System.Reflection;
namespace ShipRate.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
        }
        #region BaseInfo Types and Statuses 
        public DbSet<BranchType> BranchTypes { get; set; }
        public DbSet<SystemObjectType> SystemObjectTypes { get; set; }
        public DbSet<ClearanceType> ClearanceTypes { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<TerminalType> TerminalTypes { get; set; }
        public DbSet<DepotType> DepotTypes { get; set; }
        public DbSet<CityType> CityTypes { get; set; }
        public DbSet<BlackListStatus> BlackListStatuses { get; set; }
        public DbSet<BLType> BLTypes { get; set; }
        public DbSet<BookingChangeType> BookingChangeTypes { get; set; }
        public DbSet<BLChangeType> BLChangeTypes { get; set; }
        public DbSet<BLStatus> BLStatuses { get; set; }
        public DbSet<BuySellType> BuySellTypes { get; set; }
        public DbSet<CarrierType> CarrierTypes { get; set; }
        public DbSet<CntOwnType> CntOwnTypes { get; set; }
        public DbSet<CntOwnLeaseType> CntOwnLeaseTypes { get; set; }
        public DbSet<EventActionSource> EventActionSources { get; set; }
        public DbSet<ContainerConditionStatus> ContainerConditionStatuses { get; set; }
        public DbSet<OpportunityStage> OpportunityStages { get; set; }

        public DbSet<ContainerDamageType> ContainerDamageTypes { get; set; }
        public DbSet<ContainerDamageArea> ContainerDamageAreas { get; set; }
        public DbSet<ContainerDamageSeverity> ContainerDamageSeverities { get; set; }

        public DbSet<DepositType> DepositTypes { get; set; }
        public DbSet<DetentionDiscountType> DetentionDiscountTypes { get; set; }
        public DbSet<DetentionStatus> DetentionStatuses { get; set; }
        public DbSet<DetentionUpdater> DetentionUpdaters { get; set; }
        public DbSet<DGClass> DGClasses { get; set; }
        public DbSet<DGPackingGroup> DGPackingGroups { get; set; }
        public DbSet<TransportType> TransportTypes { get; set; }
        public DbSet<DGRegulationSet> DGRegulationSets { get; set; }
        public DbSet<DischargeLoad> DischargeLoad { get; set; }
        public DbSet<FinanceDirection> FinanceDirections { get; set; }

        public DbSet<FollowUpPriority> FollowUpPriorities { get; set; }
        public DbSet<FollowUpStatus> FollowUpStatus { get; set; }
        public DbSet<FullMT> FullMT { get; set; }
        public DbSet<HologramStatus> HologramStatus { get; set; }
        public DbSet<HSLevel> HSLevels { get; set; }
        public DbSet<TradeType> TradeTypes { get; set; }
        public DbSet<LeadSource> LeadSources { get; set; }
        public DbSet<LeadStatus> LeadStatus { get; set; }
        public DbSet<LineForwarderType> LineForwarderTypes { get; set; }
        public DbSet<ManifestType> ManifestTypes { get; set; }
        public DbSet<MeasurementUnitCategory> MeasurementUnitCategories { get; set; }
        public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<PersonalityType> PersonalityTypes { get; set; }
        public DbSet<PortOperationType> PortOperationTypes { get; set; }
        public DbSet<ReturnCashStatus> ReturnCashStatus { get; set; }
        public DbSet<RiskLevel> RiskLevels { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }
        public DbSet<RouteMode> RouteModes { get; set; }
        public DbSet<SendToolType> SendToolTypes { get; set; }
        public DbSet<ShipmentStatus> ShipmentStatus { get; set; }
        public DbSet<ShipmentLegStatus> ShipmentLegStatus { get; set; }
        public DbSet<TariffStatus> TariffStatus { get; set; }
        public DbSet<TelexReleaseStatus> TelexReleaseStatus { get; set; }
        public DbSet<TIPStatus> TIPStatus { get; set; }
        public DbSet<TIRCarnetStatus> TIRCarnetStatus { get; set; }
        public DbSet<TransitMethod> TransitMethods { get; set; }
        public DbSet<TransitType> TransitTypes { get; set; }
        public DbSet<TruckType> TruckTypes { get; set; }
        public DbSet<VesselType> VesselTypes { get; set; }
        public DbSet<PettyCashExpenseStatus> PettyCashExpenseStatus { get; set; }
        public DbSet<ProductRequestStatus> ProductRequestStatus { get; set; }
        public DbSet<InOut> InOut { get; set; }

        public DbSet<AgentRoleType> AgentRoleTypes { get; set; }
        public DbSet<Incoterm> Incoterms { get; set; }
        public DbSet<LoadType> LoadTypes { get; set; }
        public DbSet<TransportTypeLoadType> TransportTypeLoadTypes { get; set; }
        public DbSet<MovementNode> MovementNodes { get; set; }
        public DbSet<MovementType> MovementTypes { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }

        public DbSet<CntActionType> CntActionTypes { get; set; }
        public DbSet<ContainerSize> ContainerSizes { get; set; }
        public DbSet<ContainerType> ContainerTypes { get; set; }
        public DbSet<ContainerSizeType> ContainerSizeTypes { get; set; }
        public DbSet<ContainerIOType> ContainerIOTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Zone> Zones { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Territory> Territories { get; set; }
        public DbSet<TerritoryCity> TerritoryCities { get; set; }

        public DbSet<CargoCommodity> CargoCommodities { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccountPurpose> BankAccountPurposes { get; set; }
        public DbSet<VesselOwnType> VesselOwnTypes { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }

        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonRole> PersonRoles { get; set; }
        public DbSet<PersonAddress> PersonAddresses { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<PersonContact> PersonContacts { get; set; }
        public DbSet<PersonExtraInfo> PersonExtraInfos { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<AgentActScope> AgentActScopes { get; set; }
        public DbSet<BlackListReason> BlackListReasons { get; set; }
        public DbSet<VoyageType> VoyageTypes { get; set; }
        public DbSet<ChargeTypeCategory> ChargeTypeCategories { get; set; }
        public DbSet<ChargeType> ChargeTypes { get; set; }
        public DbSet<ChequeType> ChequeTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<ExchangeRateBase> ExchangeRateBases { get; set; }
        public DbSet<InvoiceFlag> InvoiceFlags { get; set; }
        public DbSet<ProductTransactionType> ProductTransactionTypes { get; set; }
        public DbSet<Association> Associations { get; set; }
        public DbSet<AgentType> AgentTypes { get; set; }
        public DbSet<AgentRole> AgentRoles { get; set; }
        public DbSet<AgentTerritory> AgentTerritories { get; set; }
        public DbSet<DOStatus> DOStatuses { get; set; }
        public DbSet<HSCode> HSCodes { get; set; }
        public DbSet<CountryHSCode> CountryHSCodes { get; set; }
        public DbSet<HSCodeSnapshot> HSCodeSnapshots { get; set; }
        public DbSet<DG> DGs { get; set; }
        public DbSet<InfoTypeCategory> InfoTypeCategories { get; set; }
        public DbSet<InfoType> InfoTypes { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<QuotationPriority> QuotationPriorities { get; set; }
        public DbSet<PersonPic> PersonPics { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<LostReasonCategory> LostReasonCategories { get; set; }
        public DbSet<LostReason> LostReasons { get; set; }
        public DbSet<CourierMethod> CourierMethods { get; set; }
        public DbSet<EventTypeCategory> EventTypeCategories { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<DocumentTypeCategory> DocumentTypeCategories { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<DocumentTypeObjectType> DocumentTypeObjectTypes { get; set; }
        public DbSet<EmailDirection> EmailDirections { get; set; }
        public DbSet<VoyageOwnType> VoyageOwnTypes { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<ActivityResult> ActivityResults { get; set; }
        public DbSet<ActivityOutcome> ActivityOutcomes { get; set; }
        public DbSet<BLReleaseType> BLReleaseTypes { get; set; }
        public DbSet<ENSDeclarationStatus> ENSDeclarationStatus { get; set; }
        public DbSet<CargoHandlingType> CargoHandlingTypes { get; set; }
        public DbSet<CargoNature> CargoNatures { get; set; }
        public DbSet<CalculationBy> CalculationBy { get; set; }
        public DbSet<CalculationType> CalculationTypes { get; set; }
        public DbSet<RateRequestStatus> RateRequestStatus { get; set; }
        public DbSet<AgentScope> AgentScopes { get; set; }
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<ClaimType> ClaimTypes { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }
        public DbSet<CustomsDeclarationType> CustomsDeclarationTypes { get; set; }
        public DbSet<CustomsStatus> CustomsStatuses { get; set; }
        public DbSet<EDIMessageType> EDIMessageTypes { get; set; }
        public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<JobStage> JobStages { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<CommentType> CommentTypes { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<Domain.Entities.Operations.TasksManage.Task> Tasks { get; set; }
        public DbSet<TaskCheckListTemplate> TaskCheckListTemplates { get; set; }
        public DbSet<Domain.Entities.BaseInfo.Generals.TaskStatus> TaskStatus { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<EndorsementType> EndorsementTypes { get; set; }
        public DbSet<InspectionType> InspectionTypes { get; set; }
        public DbSet<InspectionMethod> InspectionMethods { get; set; }
        public DbSet<InspectionResult> InspectionResults { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<LicenseStatus> LicenseStatuses { get; set; }
        public DbSet<CorrespondenceSubject> CorrespondenceSubjects { get; set; }

        public DbSet<GPSDeviceStatus> GPSDeviceStatuses { get; set; }
        public DbSet<TripStatus> TripStatuses { get; set; }
        public DbSet<PostalDispatchStatus> PostalDispatchStatuses { get; set; }
        public DbSet<JobLinkType> JobLinkTypes { get; set; }
        public DbSet<LegType> LegTypes { get; set; }
        public DbSet<CompanyRoleType> CompanyRoleTypes { get; set; }
        public DbSet<RTSInstructionStatus> RTSInstructionStatus { get; set; }
        public DbSet<RateRequestSource> RateRequestSources { get; set; }
        public DbSet<QuotationStatus> QuotationStatuses { get; set; }
        public DbSet<InquiryStatus> InquiryStatuses { get; set; }
        public DbSet<PlanStatus> PlanStatuses { get; set; }
        public DbSet<VGMStatus> VGMStatuses { get; set; }
        public DbSet<StuffingType> StuffingTypes { get; set; }
        public DbSet<BLLevel> BLLevels { get; set; }
        public DbSet<BLLinkType> BLLinkTypes { get; set; }


        public DbSet<ContainerVoyageAllocationStatus> ContainerVoyageAllocationStatuses { get; set; }
        public DbSet<LoadListStatus> LoadListStatuses { get; set; }
        public DbSet<LoadListItemStatus> LoadListItemStatuses { get; set; }
        public DbSet<ChargeStatus> ChargeStatuses { get; set; }
        public DbSet<TreasuryDocumentType> TreasuryDocumentTypes { get; set; }
        public DbSet<TreasuryDocumentStatus> TreasuryDocumentStatuses { get; set; }
        public DbSet<ChequeStatus> ChequeStatuses { get; set; }
        public DbSet<SettlementItemType> SettlementItemTypes { get; set; }
        public DbSet<SettlementStatus> SettlementStatuses { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<CampaignType> CampaignTypes { get; set; }
        public DbSet<CampaignStatus> CampaignStatuses { get; set; }
        public DbSet<ScopeType> ScopeTypes { get; set; }
        public DbSet<TariffType> TariffTypes { get; set; }
        public DbSet<CustomsHoldReason> CustomsHoldReasons { get; set; }
        public DbSet<JobSourceType> JobSourceTypes { get; set; }
        public DbSet<ActivityCategory> ActivityCategories { get; set; }
        public DbSet<ReceiptDirection> ReceiptDirections { get; set; }
        public DbSet<DOOperationalMode> DOOperationalModes { get; set; }
        public DbSet<DOReceiveMethod> DOReceiveMethods { get; set; }

        #endregion


        #region CRM
        public DbSet<Customer> Customers { get; set; }
        public DbSet<BlackList> BlackLists { get; set; }
        public DbSet<CustomerGroupingBase> CustomerGroupingBases { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        public DbSet<CustomerBelongsToGroup> CustomerBelongsToGroups { get; set; }
        public DbSet<CustomerConfirmation> CustomerConfirmations { get; set; }
        public DbSet<CustomerFinanceSetting> CustomerFinanceSettings { get; set; }
        public DbSet<CustomerRating> CustomerRatings { get; set; }
        public DbSet<CustomerRoleType> CustomerRoleTypes { get; set; }
        public DbSet<CustomerTradeType> CustomerTradeTypes { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadTag> LeadTags { get; set; }
        public DbSet<CampaignMember> CampaignMembers { get; set; }
        public DbSet<CampaignActivity> CampaignActivities { get; set; }
        public DbSet<LeadActivity> LeadActivities { get; set; }
        public DbSet<FollowUpTask> FollowUpTasks { get; set; }
        public DbSet<Attorney> Attorneys { get; set; }
        public DbSet<AttorneyLawyer> AttorneyLawyers { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<SalesTarget> SalesTargets { get; set; }

        #endregion


        #region Containers
        public DbSet<Container> Containers { get; set; }
        public DbSet<ContainerNumberHistory> ContainerNumberHistory { get; set; }
        public DbSet<ContainerAction> ContainerActions { get; set; }
        public DbSet<ContainerActionAttribute> ContainerActionAttributes { get; set; }
        public DbSet<ContainerConditionReport> ContainerConditionReports { get; set; }
        public DbSet<ContainerDamage> ContainerDamages { get; set; }
        public DbSet<ContainerIO> ContainerIOs { get; set; }
        public DbSet<ContainerRepair> ContainerRepairs { get; set; }
        public DbSet<ContainerUse> ContainerUses { get; set; }

        public DbSet<Detention> Detentions { get; set; }
        public DbSet<DetentionItem> DetentionItems { get; set; }
        public DbSet<DetentionItemDetail> DetentionItemDetails { get; set; }
        public DbSet<DetentionStartBase> DetentionStartBases { get; set; }
        public DbSet<OnlineDetention> OnlineDetentions { get; set; }
        public DbSet<EmptyPickup> EmptyPickups { get; set; }
        public DbSet<EmptyPickupContainer> EmptyPickupContainers { get; set; }

        public DbSet<Stuffing> Stuffings { get; set; }
        public DbSet<StuffingContainer> StuffingContainers { get; set; }
        public DbSet<StuffingContainerItem> StuffingContainerItems { get; set; }

        public DbSet<VGM> VGMs { get; set; }
        public DbSet<VGMContainer> VGMContainers { get; set; }


        public DbSet<ContainerVoyageAllocation> ContainerVoyageAllocations { get; set; }
        public DbSet<LoadList> LoadLists { get; set; }
        public DbSet<LoadListItem> LoadListItems { get; set; }


        public DbSet<ContractType> ContractTypes { get; set; }

        #endregion


        #region MainOperations
        public DbSet<EntityReference> EntityReferences { get; set; }
        public DbSet<CourierCompany> CourierCompanies { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<CarrierCode> CarrierCodes { get; set; }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<ShippingLineService> ShippingLineServices { get; set; }
        public DbSet<Loop> Loops { get; set; }
        public DbSet<LoopPort> LoopPorts { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightDate> FlightDates { get; set; }
        public DbSet<PortDistance> PortDistances { get; set; }
        public DbSet<PostalDispatch> PostalDispatches { get; set; }
        public DbSet<PostalDispatchItem> PostalDispatchItems { get; set; }
        public DbSet<PostalDispatchConfirmation> PostalDispatchConfirmations { get; set; }
        public DbSet<TIP> TIPs { get; set; }
        public DbSet<Voyage> Voyages { get; set; }
        public DbSet<VoyagePort> VoyagePorts { get; set; }
        public DbSet<VoyagePortEvent> VoyagePortEvents { get; set; }
        public DbSet<VoyageRelation> VoyageRelations { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAttachment> DocumentAttachments { get; set; }
        public DbSet<DocumentEmail> DocumentEmails { get; set; }
        public DbSet<AgentChargeType> AgentChargeTypes { get; set; }
        public DbSet<AgentRoleCommissionPerSize> AgentRoleCommissionPerSizes { get; set; }
        public DbSet<AgencyContract> AgencyContracts { get; set; }

        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractChargeType> ContractChargeTypes { get; set; }
        public DbSet<ContractContainerSizeType> ContractContainerSizeTypes { get; set; }
        public DbSet<ContractCustomer> ContractCustomers { get; set; }
        public DbSet<ContractFreeTime> ContractFreeTimes { get; set; }
        public DbSet<ContractRoute> ContractRoutes { get; set; }
        public DbSet<ContractRate> ContractRates { get; set; }
        public DbSet<ContractDiscount> ContractDiscounts { get; set; }
        public DbSet<FACContract> FACContracts { get; set; }
        public DbSet<ContractSupplement> ContractSupplements { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeaDetail> BookingSeaDetails { get; set; }
        public DbSet<BookingSeaVoyage> BookingSeaVoyages { get; set; }
        public DbSet<BookingAirDetail> BookingAirDetails { get; set; }
        public DbSet<BookingAirFlight> BookingAirFlights { get; set; }
        public DbSet<BookingRoadDetail> BookingRoadDetails { get; set; }
        public DbSet<BookingRailDetail> BookingRailDetails { get; set; }

        public DbSet<BookingParty> BookingParties { get; set; }
        public DbSet<BookingLeg> BookingLegs { get; set; }
        public DbSet<BookingChange> BookingChanges { get; set; }
        public DbSet<BookingContainerType> BookingContainerTypes { get; set; }
        public DbSet<BookingContainerTypeCharge> BookingContainerTypeCharges { get; set; }
        public DbSet<BookingParty> BookingCustomers { get; set; }

        public DbSet<ExportDocsReceipt> ExportDocsReceipts { get; set; }
        public DbSet<ExportDocsReceiptItem> ExportDocsReceiptItems { get; set; }
        public DbSet<ExportDocsReceiptContainer> ExportDocsReceiptContainers { get; set; }

        public DbSet<RTSInstruction> RTSInstructions { get; set; }
        public DbSet<RTSInstructionContainerType> RTSInstructionContainerTypes { get; set; }
        public DbSet<RTSInstructionContainer> RTSInstructionContainers { get; set; }

        public DbSet<BL> BLs { get; set; }
        public DbSet<BLLink> BLLinks { get; set; }
        public DbSet<BLChange> BLChanges { get; set; }
        public DbSet<BLCharge> BLCharges { get; set; }
        public DbSet<BLContainer> BLContainers { get; set; }
        public DbSet<BLContainerTripAllocation> BLContainerTripAllocations { get; set; }
        public DbSet<BLContainerCargoAllocation> BLContainerCargoAllocations { get; set; }
        public DbSet<BLCargoAllocation> BLCargoAllocations { get; set; }
        public DbSet<BLParty> BLParties { get; set; }
        public DbSet<BLFlight> BLFlights { get; set; }
        public DbSet<BLOperation> BLOperations { get; set; }
        public DbSet<BLFollowUp> BLFollowUps { get; set; }
        public DbSet<CarrierBLType> CarrierBLTypes { get; set; }
        public DbSet<BLSerialSeries> BLSerialSeries { get; set; }
        public DbSet<BLSerialBatchAssign> BLSerialBatchAssigns { get; set; }
        public DbSet<BLSerial> BLSerials { get; set; }
        public DbSet<BLSurrender> BLSurrenders { get; set; }
        public DbSet<BLTrip> BLTrips { get; set; }
        public DbSet<BLRailJourney> BLRailJourneys { get; set; }
        public DbSet<BLVoyage> BLVoyages { get; set; }
        public DbSet<CargoDetail> CargoDetails { get; set; }
        public DbSet<CustomsReference> CustomsReferences { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public DbSet<DeliveryOrderContainer> DeliveryOrderContainers { get; set; }
        public DbSet<FreightLetter> FreightLetters { get; set; }
        public DbSet<ENSDeclaration> ENSDeclarations { get; set; }
        public DbSet<EPLGrantDO> EPLGrantDOs { get; set; }
        public DbSet<GuaranteePlan> GuaranteePlans { get; set; }
        public DbSet<HologramSeries> HologramSeries { get; set; }
        public DbSet<Hologram> Holograms { get; set; }
        public DbSet<TelexReleaseRequest> TelexReleaseRequests { get; set; }
        public DbSet<TelexMessage> TelexMessages { get; set; }
        public DbSet<TIPUsage> TIPUsages { get; set; }
        public DbSet<TIPUsageContainer> TIPUsageContainers { get; set; }

        public DbSet<Rate> Rates { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<ProvidedRate> ProvidedRates { get; set; }
        public DbSet<ProvidedRateRoute> ProvidedRateRoutes { get; set; }
        public DbSet<ProvidedRateRouteAir> ProvidedRateRouteAirs { get; set; }
        public DbSet<ProvidedRateRouteAirTier> ProvidedRateRouteAirTiers { get; set; }
        public DbSet<ProvidedRateRouteAirTierCharge> ProvidedRateRouteAirTierCharges { get; set; }
        public DbSet<ProvidedRateRouteContainer> ProvidedRateRouteContainers { get; set; }
        public DbSet<ProvidedRateRouteContainerCharge> ProvidedRateRouteContainerCharges { get; set; }
        public DbSet<ProvidedRateRouteFTL> ProvidedRateRouteFTLs { get; set; }
        public DbSet<ProvidedRateRouteFTLCharge> ProvidedRateRouteFTLCharges { get; set; }
        public DbSet<ProvidedRateRouteLTL> ProvidedRateRouteLTLs { get; set; }
        public DbSet<RateCountryPort> RateCountryPorts { get; set; }
        public DbSet<RateCountryPortCharge> RateCountryPortCharges { get; set; }
        public DbSet<RateCountryPortChargeTier> RateCountryPortChargeTiers { get; set; }


        public DbSet<RateRequest> RateRequests { get; set; }
        public DbSet<RateRequestService> RateRequestServices { get; set; }
        public DbSet<RateRequestPackage> RateRequestPackages { get; set; }
        public DbSet<RateRequestContainer> RateRequestContainers { get; set; }
        public DbSet<RateRequestInquiry> RateRequestInquiries { get; set; }
        public DbSet<RateRequestTransportType> RateRequestTransportTypes { get; set; }
        public DbSet<RateRequestValidity> RateRequestValidity { get; set; }
        public DbSet<BuyQuote> BuyQuotes { get; set; }
        public DbSet<BuyQuoteItem> BuyQuoteItems { get; set; }
        public DbSet<SellQuote> SellQuotes { get; set; }
        public DbSet<SellQuoteItem> SellQuoteItems { get; set; }
        public DbSet<EDIMessage> EDIMessages { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobRouting> JobRoutings { get; set; }
        public DbSet<JobInsurance> JobInsurances { get; set; }
        public DbSet<JobLC> JobLCs { get; set; }
        public DbSet<JobCompanyRole> JobCompanyRoles { get; set; }
        public DbSet<AppTask> AppTasks { get; set; }
        public DbSet<AppTaskComment> AppTaskComments { get; set; }
        public DbSet<AppTaskCheckList> AppTaskCheckLists { get; set; }
        public DbSet<AppTaskAssignment> AppTaskAssignments { get; set; }
        public DbSet<AppTaskReminder> AppTaskReminders { get; set; }
        public DbSet<AppTaskEscalation> AppTaskEscalations { get; set; }
        public DbSet<TaskSourceType> TaskSourceTypes { get; set; }

        public DbSet<JobCustomField> JobCustomFields { get; set; }
        public DbSet<JobDocument> JobDocuments { get; set; }
        public DbSet<JobLink> JobLinks { get; set; }
        public DbSet<JobParty> JobParties { get; set; }
        public DbSet<JobService> JobServices { get; set; }

        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentOperationPlan> ShipmentOperationPlans { get; set; }
        public DbSet<ShipmentBooking> ShipmentBookings { get; set; }
        public DbSet<ShipmentBL> ShipmentBLs { get; set; }
        public DbSet<ShipmentLeg> ShipmentLegs { get; set; }
        public DbSet<ShipmentCargo> ShipmentCargos { get; set; }
        public DbSet<ShipmentContainer> ShipmentContainers { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<BLEndorsement> BLEndorsements { get; set; }

        public DbSet<CustomsClearance> CustomsClearances { get; set; }
        public DbSet<CustomsClearanceContainer> CustomsClearanceContainers { get; set; }
        public DbSet<CustomsClearanceItem> CustomsClearanceItems { get; set; }
        public DbSet<CustomsClearanceCharge> CustomsClearanceCharges { get; set; }
        public DbSet<CustomsClearanceLicense> CustomsClearanceLicenses { get; set; }
        public DbSet<CustomsInspection> CustomsInspections { get; set; }

        public DbSet<Correspondence> Correspondences { get; set; }
        public DbSet<NOA> NOAs { get; set; }
        public DbSet<NOAPrice> NOAPrices { get; set; }
        public DbSet<NOASend> NOASends { get; set; }

        public DbSet<ShippingOrder> ShippingOrders { get; set; }
        public DbSet<ShippingOrderCargoLine> ShippingOrderCargoLines { get; set; }
        public DbSet<ShippingOrderContainer> ShippingOrderContainers { get; set; }
        public DbSet<ShippingOrderParty> ShippingOrderParties { get; set; }
        public DbSet<ShippingOrderContainerCargoAllocation> ShippingOrderContainerCargoAllocations { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        #endregion

        #region Finance
        public DbSet<AccountCode> AccountCodes { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ChequePurpose> ChequePurposes { get; set; }
        public DbSet<Cheque> Cheques { get; set; }
        public DbSet<ChequeReceipt> ChequeReceipts { get; set; }
        public DbSet<PaymentDocument> PaymentDocuments { get; set; }
        public DbSet<PaymentDocumentExtraInfo> PaymentDocumentExtraInfos { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }
        public DbSet<ReturnCashRequest> ReturnCashRequests { get; set; }
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<SettlementItem> SettlementItems { get; set; }
        public DbSet<SettlementItemPaymentAllocation> SettlementItemPaymentAllocations { get; set; }
        public DbSet<TreasuryDocument> TreasuryDocuments { get; set; }
        public DbSet<TreasuryDocumentItem> TreasuryDocumentItems { get; set; }

        public DbSet<Vault> Vaults { get; set; }
        public DbSet<VaultAsset> VaultAssets { get; set; }
        public DbSet<VaultMovement> VaultMovements { get; set; }
        public DbSet<CashPaymentRequest> CashPaymentRequests { get; set; }
        public DbSet<VaultItem> VaultItems { get; set; }
        public DbSet<VaultItemCustodyLog> VaultItemCustodyLogs { get; set; }

        public DbSet<VaultMovementSourceType> VaultMovementSourceTypes { get; set; }
        public DbSet<CashPaymentRequestStatus> CashPaymentRequestStatuses { get; set; }
        public DbSet<VaultItemCategory> VaultItemCategories { get; set; }
        public DbSet<VaultItemStatus> VaultItemStatus { get; set; }
        public DbSet<VaultCustodyActionType> VaultCustodyActionTypes { get; set; }

        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<PettyCash> PettyCashes { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<PettyCashExpense> PettyCashExpenses { get; set; }
        public DbSet<PettyCashRecharge> PettyCashRecharges { get; set; }
        public DbSet<PettyCashRechargeStatus> PettyCashRechargeStatuses { get; set; }

        public DbSet<InvoiceFNF> InvoiceFNFs { get; set; }
        public DbSet<InvoiceSubject> InvoiceSubjects { get; set; }
        public DbSet<InvoiceModel> InvoiceModels { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        public DbSet<OnlinePaymentStatus> OnlinePaymentStatuses { get; set; }
        public DbSet<OnlinePayment> OnlinePayments { get; set; }
        public DbSet<OnlinePaymentItem> OnlinePaymentItems { get; set; }
        public DbSet<OnlinePaymentBankAccount> OnlinePaymentBankAccounts { get; set; }



        public DbSet<CreditDebitAccType> CreditDebitAccTypes { get; set; }
        public DbSet<ChargeTypeCreditDebit> ChargeTypeCreditDebits { get; set; }

        public DbSet<SaoiCurrencyRate> SaoiCurrencyRates { get; set; }

        #endregion

        #region Inventory
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryProduct> InventoryProducts { get; set; }
        public DbSet<ProductRequest> ProductRequests { get; set; }
        public DbSet<ProductRequestItem> ProductRequestItems { get; set; }
        public DbSet<ProductStockTransaction> ProductStockTransactions { get; set; }
        #endregion

        #region Transit
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<GPSProvider> GPSProviders { get; set; }
        public DbSet<GPSDevice> GPSDevices { get; set; }
        public DbSet<GPSAssignment> GPSAssignments { get; set; }
        public DbSet<GPSRecharge> GPSRecharges { get; set; }
        public DbSet<GPSTrackingPoint> GPSTrackingPoints { get; set; }
        public DbSet<TIRCarnet> TIRCarnets { get; set; }
        public DbSet<TIRAssignment> TIRAssignments { get; set; }
        public DbSet<TIRRequest> TIRRequests { get; set; }
        public DbSet<TIRUsage> TIRUsages { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripStop> TripStops { get; set; }
        public DbSet<ShipmentLegBorder> ShipmentLegBorders { get; set; }
        public DbSet<RMTOIntegration> RMTOIntegrations { get; set; }
        #endregion

        #region Tariff
        public DbSet<TariffBook> TariffBooks { get; set; }
        public DbSet<TariffRule> TariffRules { get; set; }
        public DbSet<TariffRuleFormula> TariffRuleFormulas { get; set; }
        public DbSet<TariffRuleCoverage> TariffRuleCoverages { get; set; }
        #endregion

        #region Settings
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyRole> CompanyRoles { get; set; }
        public DbSet<NumberingSystem> NumberingSystems { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<IPGTerminal> IPGTerminals { get; set; }
        public DbSet<IPGTerminalBankAccount> IPGTerminalBankAccounts { get; set; }
        public DbSet<FormatType> FormatTypes { get; set; }
        public DbSet<CompanyFormat> CompanyFormats { get; set; }
        public DbSet<CompanyFormatParam> CompanyFormatParams { get; set; }
        #endregion

        #region "ServiceRequests"
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<ServiceRequestActionType> ServiceRequestActionTypes { get; set; }
        public DbSet<ServiceRequestApproval> ServiceRequestApprovals { get; set; }
        public DbSet<ServiceRequestApprovalStatus> ServiceRequestApprovalStatuses { get; set; }
        public DbSet<ServiceRequestApprovalType> ServiceRequestApprovalTypes { get; set; }
        public DbSet<ServiceRequestCharge> ServiceRequestCharges { get; set; }
        public DbSet<ServiceRequestPriority> ServiceRequestPriorities { get; set; }
        public DbSet<ServiceRequestStatus> ServiceRequestStatuses { get; set; }
        public DbSet<ServiceRequestType> ServiceRequestTypes { get; set; }
        public DbSet<SplitRequest> SplitRequests { get; set; }
        public DbSet<SplitRequestTarget> SplitRequestTargets { get; set; }
        public DbSet<SplitRequestAllocation> SplitRequestAllocations { get; set; }
        public DbSet<BLAmendmentFieldType> BLAmendmentFieldTypes { get; set; }
        public DbSet<BLAmendmentRequest> BLAmendmentRequests { get; set; }
        public DbSet<BLAmendmentRequestLine> BLAmendmentRequestLines { get; set; }
        public DbSet<ConsolidationRequest> ConsolidationRequests { get; set; }
        public DbSet<ConsolidationRequestItem> ConsolidationRequestItems { get; set; }
        #endregion


        #region Communications
        public DbSet<Domain.Entities.Notifs.Notification> AppNotifications { get; set; }
        public DbSet<Domain.Entities.Notifs.UserNotification> UserNotifications { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationAudience> NotificationAudiences { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationDelivery> NotificationDeliveries { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationSourceType> NotificationSourceTypes { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationAudienceType> NotificationAudienceTypes { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationDeliveryStatus> NotificationDeliveryStatuses { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationChannel> NotificationChannels { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationSeverity> NotificationSeverities { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationRule> NotificationRules { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationRuleUser> NotificationRuleUsers { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationRuleUserType> NotificationRuleUserTypes { get; set; }
        public DbSet<Domain.Entities.Notifs.NotificationRecipientMode> NotificationRecipientModes { get; set; }

        public DbSet<MessagingAttachmentVisibility> MessagingAttachmentVisibilities { get; set; }
        public DbSet<MessagingConversation> MessagingConversations { get; set; }
        public DbSet<MessagingConversationParticipant> MessagingConversationParticipants { get; set; }
        public DbSet<MessagingConversationScope> MessagingConversationScopes { get; set; }
        public DbSet<MessagingConversationStatus> MessagingConversationStatuses { get; set; }
        public DbSet<MessagingConversationType> MessagingConversationTypes { get; set; }
        public DbSet<MessagingMessage> MessagingMessages { get; set; }
        public DbSet<MessagingMessageAttachment> MessagingMessageAttachments { get; set; }
        public DbSet<MessagingMessageVisibility> MessagingMessageVisibilities { get; set; }
        public DbSet<MessagingParticipantRole> MessagingParticipantRoles { get; set; }
        public DbSet<MessagingParticipantKind> MessagingParticipantKinds { get; set; }

        public DbSet<SmsMessage> SmsMessages { get; set; }
        public DbSet<SmsRecipient> SmsRecipients { get; set; }
        public DbSet<SmsIncomingMessage> SmsIncomingMessages { get; set; }
        public DbSet<SmsMoCursor> SmsMoCursors { get; set; }

        public DbSet<MarketingCallLog> MarketingCallLogs { get; set; }
        #endregion




        #region Security
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
        public DbSet<UserPermissionGroup> UserPermissionGroups { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SecurityEvent> SecurityEvents => Set<SecurityEvent>();
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<TeamType> TeamTypes { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<UserTeamMember> UserTeamMembers { get; set; }
        public DbSet<TwoFactorMethod> TwoFactorMethods { get; set; }
        public DbSet<UserOtpCode> UserOtpCodes { get; set; }
        #endregion

        #region externalServices
        //Ghader
        public DbSet<ApiInquiryLog> ApiInquiryLogs { get; set; }
        //Amir
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAttachment> NotificationAttachments { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
        public DbSet<TaxInvoice> TaxInvoices { get; set; }
        public DbSet<TaxInvoiceDtl> TaxInvoiceDtls { get; set; }
        public DbSet<TaxInvoicePayment> TaxInvoicePayments { get; set; }
        public DbSet<TaxInvoiceShippingGood> TaxInvoiceShippingGoods { get; set; }
        public DbSet<ReportConfig> ReportConfigs { get; set; }
        #endregion

        #region Reports
        public DbSet<ReportCategory> ReportCategories { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportCompanyRoleType> ReportCompanyRoleTypes { get; set; }
        #endregion







        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

            var cascadeFks = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
            foreach (var fk in cascadeFks)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            //Identity (اجباری و صریح)
            //Identity (اجباری و صریح)

            modelBuilder.Entity<UserType>().ToTable("UserTypes", "sec");
            modelBuilder.Entity<UserType>().HasKey(c => c.Id);
            modelBuilder.Entity<UserType>().Property(c => c.UserTypeName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<UserType>().Property(c => c.UserTypeLocalName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<UserType>().Property(c => c.UserTypeDescription).HasMaxLength(200);

            modelBuilder.Entity<UserType>().HasData(
                new UserType { Id = 1, UserTypeName = "Internal(System)", UserTypeLocalName = "کاربر داخلی سیستم اصلی", UserTypeDescription = "کاربر متعلق به پرسنل شرکتهای داخلی" },
                new UserType { Id = 2, UserTypeName = "External(System)", UserTypeLocalName = "کاربر خارجی سیستم اصلی", UserTypeDescription = "کاربران خارجی اعم از Carrier و Agent" },
                new UserType { Id = 3, UserTypeName = "Customers Portal", UserTypeLocalName = "کاربر درگاه مشتریان", UserTypeDescription = "مشتریان " }
                );



            modelBuilder.Entity<UserCompany>().ToTable("UserCompanies", "sec");
            modelBuilder.Entity<UserCompany>().HasKey(c => c.Id);
            modelBuilder.Entity<UserCompany>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserCompany>().HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);


            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "sec");
            modelBuilder.Entity<ApplicationUser>().Property(x => x.FullName).HasMaxLength(60);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.FirstName).HasMaxLength(25);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.LastName).HasMaxLength(35);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Mobile).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.SalesFilePrefix).HasMaxLength(20);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.JobTitle).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().HasOne(x => x.UserType).WithMany().HasForeignKey(x => x.UserTypeId);
            modelBuilder.Entity<ApplicationUser>().HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
            modelBuilder.Entity<ApplicationUser>().HasOne(x => x.TwoFactorMethod).WithMany().HasForeignKey(x => x.TwoFactorMethodId);
            modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.SalesFilePrefix).IsUnique().HasFilter("[IsActive] = 1 AND [SalesFilePrefix] IS NOT NULL and [SalesFilePrefix]<>'' ");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles", "sec");

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole("Admin") { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), NormalizedName = "ADMIN", ConcurrencyStamp = "19d50e55-34d2-4548-8b43-8671e891d07e" },
                new ApplicationRole("Manager") { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), NormalizedName = "MANAGER", ConcurrencyStamp = "6f76ec9f-1afb-40bd-ad43-46a3c46e9930" },
                new ApplicationRole("Sales Manager") { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), NormalizedName = "SALES MANAGER", ConcurrencyStamp = "0c22590c-de9d-483e-8061-e5dad6506426" },
                new ApplicationRole("Sales User") { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), NormalizedName = "SALES USER", ConcurrencyStamp = "ffb67780-1961-4bec-a124-375558f3a102" },
                new ApplicationRole("External Party") { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), NormalizedName = "EXTERNAL PARTY", ConcurrencyStamp = "ffb67780-1961-4bec-a124-375558f3a102" },
                new ApplicationRole("Customer") { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), NormalizedName = "CUSTOMER", ConcurrencyStamp = "ffb67780-1961-4bec-a124-375558f3a102" }
            );

            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "sec");



            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "sec");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "sec");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "sec");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "sec");

            //modelBuilder.Entity<XmlSchemaCompilationSettings>().HasNoKey();

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var isDeletedProp = entityType.FindProperty("IsDeleted");
                if (isDeletedProp != null && isDeletedProp.ClrType == typeof(bool))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var falseConstant = Expression.Constant(false);
                    var condition = Expression.Equal(property, falseConstant);
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }







        }

        #region SaveChanges Methods
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BusinessEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Id = Guid.CreateVersion7();
                    entry.Entity.CreatedAt = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BusinessEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.Id == Guid.Empty)
                    {
                        entry.Entity.Id = Guid.CreateVersion7();
                    }
                    entry.Entity.CreatedAt = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BusinessEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.Id == Guid.Empty)
                    {
                        entry.Entity.Id = Guid.CreateVersion7();
                    }
                    entry.Entity.CreatedAt = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion






        public async Task<bool> CanSoftDelete<TEntity>(Guid entityId) where TEntity : BusinessEntity
        {
            var entityTypes = this.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    if (foreignKey.PrincipalEntityType.ClrType == typeof(TEntity))
                    {
                        var navigationProperty = foreignKey.Properties.FirstOrDefault();
                        if (navigationProperty == null) continue;

                        string propertyName = navigationProperty.Name;
                        string schemaName = entityType.GetSchema();
                        string tableName = entityType.GetTableName();
                        var sql = $"SELECT COUNT(1) As Value FROM {schemaName}.{tableName} WHERE {propertyName} = '{entityId}' AND IsDeleted = 0";
                        var count = await this.Database.SqlQueryRaw<int>(sql).FirstOrDefaultAsync();
                        if (count > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
