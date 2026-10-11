using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Agents;
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
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Contract;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Operations.TasksManage;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Domain.Entities.TransitOperation;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;
using TaskStatus = ShipRate.Domain.Entities.BaseInfo.Generals.TaskStatus;

namespace ShipRate.Infrastructure.Repositories
{
    public class BaseDataRepository : IBaseDataRepository
    {
        protected readonly ApplicationDbContext _context;

        public BaseDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PersonalityType>> GetAllPersonalityTypesAsync()
        {
            return await _context.PersonalityTypes.ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return await _context.Countries.ToListAsync();
        }
        public async Task<IEnumerable<Association>> GetAssociationAsync()
        {
            return await _context.Associations.ToListAsync();
        }
        public async Task<IEnumerable<Bank>> GetBanksAsync()
        {
            return await _context.Banks.ToListAsync();
        }
        public async Task<IEnumerable<BlackListStatus>> GetBlackListStatusesAsync()
        {
            return await _context.BlackListStatuses.ToListAsync();
        }
        public async Task<IEnumerable<BLChangeType>> GetBLChangeTypesAsync()
        {
            return await _context.BLChangeTypes.ToListAsync();
        }
        public async Task<IEnumerable<BLReleaseType>> GetBLReleaseTypesAsync()
        {
            return await _context.BLReleaseTypes.ToListAsync();
        }
        public async Task<IEnumerable<BLStatus>> GetBLStatusesAsync()
        {
            return await _context.BLStatuses.ToListAsync();
        }
        public async Task<IEnumerable<BLType>> GetBLTypesAsync()
        {
            return await _context.BLTypes.ToListAsync();
        }
        public async Task<IEnumerable<BookingChangeType>> GetBookingChangeTypesAsync()
        {
            return await _context.BookingChangeTypes.ToListAsync();
        }
        public async Task<IEnumerable<BookingStatus>> GetBookingStatusesAsync()
        {
            return await _context.BookingStatuses.ToListAsync();
        }
        public async Task<IEnumerable<BuySellType>> GetBuySellTypesAsync()
        {
            return await _context.BuySellTypes.ToListAsync();
        }
        public async Task<IEnumerable<CalculationBy>> GetCalculationBysAsync()
        {
            return await _context.CalculationBy.ToListAsync();
        }
        public async Task<IEnumerable<CalculationType>> GetCalculationTypesAsync()
        {
            return await _context.CalculationTypes.ToListAsync();
        }
        public async Task<IEnumerable<CargoCommodity>> GetCargoCommoditiesAsync()
        {
            return await _context.CargoCommodities.OrderBy(x => x.CargoCommodityName).ToListAsync();
        }
        public async Task<IEnumerable<CargoHandlingType>> GetCargoHandlingTypesAsync()
        {
            return await _context.CargoHandlingTypes.ToListAsync();
        }
        public async Task<IEnumerable<CargoNature>> GetCargoNaturesAsync()
        {
            return await _context.CargoNatures.ToListAsync();
        }
        public async Task<IEnumerable<CarrierType>> GetCarrierTypesAsync()
        {
            return await _context.CarrierTypes.ToListAsync();
        }
        public async Task<IEnumerable<ChargeTypeCategory>> GetChargeTypeCategoriesAsync()
        {
            return await _context.ChargeTypeCategories.ToListAsync();
        }
        public async Task<IEnumerable<ChargeType>> GetChargeTypesAsync()
        {
            return await _context.ChargeTypes.ToListAsync();
        }
        public async Task<IEnumerable<ChequeType>> GetChequeTypesAsync()
        {
            return await _context.ChequeTypes.ToListAsync();
        }
        public async Task<IEnumerable<CityType>> GetCityTypesAsync()
        {
            return await _context.CityTypes.ToListAsync();
        }
        public async Task<IEnumerable<ClaimStatus>> GetClaimStatusesAsync()
        {
            return await _context.ClaimStatuses.ToListAsync();
        }
        public async Task<IEnumerable<ClaimType>> GetClaimTypesAsync()
        {
            return await _context.ClaimTypes.ToListAsync();
        }
        public async Task<IEnumerable<ClearanceType>> GetClearanceTypesAsync()
        {
            return await _context.ClearanceTypes.ToListAsync();
        }
        public async Task<IEnumerable<CntActionType>> GetCntActionTypesAsync()
        {
            return await _context.CntActionTypes.ToListAsync();
        }
        public async Task<IEnumerable<CntOwnType>> GetCntOwnTypesAsync()
        {
            return await _context.CntOwnTypes.ToListAsync();
        }
        public async Task<IEnumerable<CommentType>> GetCommentTypesAsync()
        {
            return await _context.CommentTypes.ToListAsync();
        }
        public async Task<IEnumerable<EventActionSource>> GetEventActionSourcesAsync()
        {
            return await _context.EventActionSources.ToListAsync();
        }
        public async Task<IEnumerable<ContainerConditionStatus>> GetContainerConditionStatusesAsync()
        {
            return await _context.ContainerConditionStatuses.ToListAsync();
        }
        public async Task<IEnumerable<ContainerDamageArea>> GetContainerDamageAreasAsync()
        {
            return await _context.ContainerDamageAreas.ToListAsync();
        }
        public async Task<IEnumerable<ContainerDamageSeverity>> GetContainerDamageSeveritiesAsync()
        {
            return await _context.ContainerDamageSeverities.ToListAsync();
        }
        public async Task<IEnumerable<ContainerDamageType>> GetContainerDamageTypesAsync()
        {
            return await _context.ContainerDamageTypes.ToListAsync();
        }
        public async Task<IEnumerable<ContainerIOType>> GetContainerIOTypesAsync()
        {
            return await _context.ContainerIOTypes.ToListAsync();
        }
        public async Task<IEnumerable<ContainerSize>> GetContainerSizesAsync()
        {
            return await _context.ContainerSizes.ToListAsync();
        }
        public async Task<IEnumerable<ContainerType>> GetContainerTypesAsync()
        {
            return await _context.ContainerTypes.ToListAsync();
        }
        public async Task<IEnumerable<ContainerSizeType>> GetContainerSizeTypesAsync()
        {
            return await _context.ContainerSizeTypes
                        .Include(x => x.ContainerType)
                        .Include(x => x.ContainerSize)
                        .ToListAsync();
        }
        public async Task<IEnumerable<ContractType>> GetContractTypesAsync()
        {
            return await _context.ContractTypes.ToListAsync();
        }
        public async Task<IEnumerable<CourierMethod>> GetCourierMethodsAsync()
        {
            return await _context.CourierMethods.ToListAsync();
        }
        public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
        {
            return await _context.Currencies.ToListAsync();
        }
        public async Task<IEnumerable<CustomField>> GetCustomFieldsAsync()
        {
            return await _context.CustomFields.ToListAsync();
        }
        public async Task<IEnumerable<CustomsDeclarationType>> GetCustomsDeclarationTypesAsync()
        {
            return await _context.CustomsDeclarationTypes.ToListAsync();
        }
        public async Task<IEnumerable<CustomsStatus>> GetCustomsStatusesAsync()
        {
            return await _context.CustomsStatuses.ToListAsync();
        }
        public async Task<IEnumerable<DepositType>> GetDepositTypesAsync()
        {
            return await _context.DepositTypes.ToListAsync();
        }
        public async Task<IEnumerable<DepotType>> GetDepotTypesAsync()
        {
            return await _context.DepotTypes.ToListAsync();
        }
        public async Task<IEnumerable<DetentionDiscountType>> GetDetentionDiscountTypesAsync()
        {
            return await _context.DetentionDiscountTypes.ToListAsync();
        }
        public async Task<IEnumerable<DetentionStatus>> GetDetentionStatusesAsync()
        {
            return await _context.DetentionStatuses.ToListAsync();
        }
        public async Task<IEnumerable<DetentionUpdater>> GetDetentionUpdatersAsync()
        {
            return await _context.DetentionUpdaters.ToListAsync();
        }
        public async Task<IEnumerable<DGPackingGroup>> GetDGPackingGroupsAsync()
        {
            return await _context.DGPackingGroups.ToListAsync();
        }
        public async Task<IEnumerable<DGRegulationSet>> GetDGRegulationSetsAsync()
        {
            return await _context.DGRegulationSets.ToListAsync();
        }
        public async Task<IEnumerable<DGClass>> GetDGDGClassesAsync()
        {
            return await _context.DGClasses.ToListAsync();
        }
        public async Task<IEnumerable<DischargeLoad>> GetDischargeLoadsAsync()
        {
            return await _context.DischargeLoad.ToListAsync();
        }
        public async Task<IEnumerable<DocumentTypeCategory>> GetDocumentTypeCategoriesAsync()
        {
            return await _context.DocumentTypeCategories.ToListAsync();
        }
        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync()
        {
            return await _context.DocumentTypes.ToListAsync();
        }
        public async Task<IEnumerable<DOStatus>> GetDOStatusesAsync()
        {
            return await _context.DOStatuses.ToListAsync();
        }
        public async Task<IEnumerable<EDIMessageType>> GetEDIMessageTypesAsync()
        {
            return await _context.EDIMessageTypes.ToListAsync();
        }
        public async Task<IEnumerable<EmailDirection>> GetEmailDirectionsAsync()
        {
            return await _context.EmailDirections.ToListAsync();
        }
        public async Task<IEnumerable<EndorsementType>> GetEndorsementTypesAsync()
        {
            return await _context.EndorsementTypes.ToListAsync();
        }
        public async Task<IEnumerable<ENSDeclarationStatus>> GetENSDeclarationStatusesAsync()
        {
            return await _context.ENSDeclarationStatus.ToListAsync();
        }
        public async Task<IEnumerable<EventTypeCategory>> GetEventTypeCategoriesAsync()
        {
            return await _context.EventTypeCategories.ToListAsync();
        }
        public async Task<IEnumerable<Domain.Entities.BaseInfo.Generals.EventType>> GetEventTypesAsync()
        {
            return await _context.EventTypes.ToListAsync();
        }
        public async Task<IEnumerable<ExchangeRateBase>> GetExchangeRateBasesAsync()
        {
            return await _context.ExchangeRateBases.ToListAsync();
        }
        public async Task<IEnumerable<FinanceDirection>> GetFinanceDirectionsAsync()
        {
            return await _context.FinanceDirections.ToListAsync();
        }
        public async Task<IEnumerable<FollowUpPriority>> GetFollowUpPrioritiesAsync()
        {
            return await _context.FollowUpPriorities.ToListAsync();
        }
        public async Task<IEnumerable<FollowUpStatus>> GetFollowUpStatusesAsync()
        {
            return await _context.FollowUpStatus.ToListAsync();
        }
        public async Task<IEnumerable<FullMT>> GetFullMTsAsync()
        {
            return await _context.FullMT.ToListAsync();
        }
        public async Task<IEnumerable<HologramStatus>> GetHologramStatusesAsync()
        {
            return await _context.HologramStatus.ToListAsync();
        }
        public async Task<IEnumerable<HSLevel>> GetHSLevelsAsync()
        {
            return await _context.HSLevels.ToListAsync();
        }
        public async Task<IEnumerable<Incoterm>> GetIncotermsAsync()
        {
            return await _context.Incoterms.ToListAsync();
        }
        public async Task<IEnumerable<Industry>> GetIndustriesAsync()
        {
            return await _context.Industries.ToListAsync();
        }
        public async Task<IEnumerable<InfoTypeCategory>> GetInfoTypeCategoriesAsync()
        {
            return await _context.InfoTypeCategories.ToListAsync();
        }
        public async Task<IEnumerable<InfoType>> GetInfoTypesAsync()
        {
            return await _context.InfoTypes.ToListAsync();
        }
        public async Task<IEnumerable<InfoType>> GetInfoTypesAsync(int categoryId)
        {
            return await _context.InfoTypes.Where(x => x.InfoTypeCategoryId == categoryId).ToListAsync();
        }
        public async Task<IEnumerable<InOut>> GetInOutsAsync()
        {
            return await _context.InOut.ToListAsync();
        }
        public async Task<IEnumerable<InspectionMethod>> GetInspectionMethodsAsync()
        {
            return await _context.InspectionMethods.ToListAsync();
        }
        public async Task<IEnumerable<InspectionResult>> GetInspectionResultsAsync()
        {
            return await _context.InspectionResults.ToListAsync();
        }
        public async Task<IEnumerable<InspectionType>> GetInspectionTypesAsync()
        {
            return await _context.InspectionTypes.ToListAsync();
        }
        public async Task<IEnumerable<InvoiceFlag>> GetInvoiceFlagsAsync()
        {
            return await _context.InvoiceFlags.ToListAsync();
        }
        public async Task<IEnumerable<InvoiceStatus>> GetInvoiceStatusesAsync()
        {
            return await _context.InvoiceStatuses.ToListAsync();
        }
        public async Task<IEnumerable<ServiceType>> GetServiceTypesAsync()
        {
            return await _context.ServiceTypes.ToListAsync();
        }
        public async Task<IEnumerable<JobStage>> GetJobStagesAsync()
        {
            return await _context.JobStages.ToListAsync();
        }
        public async Task<IEnumerable<JobStatus>> GetJobStatusesAsync()
        {
            return await _context.JobStatuses.ToListAsync();
        }
        public async Task<IEnumerable<JobType>> GetJobTypesAsync()
        {
            return await _context.JobTypes.ToListAsync();
        }
        public async Task<IEnumerable<LeadSource>> GetLeadSourcesAsync()
        {
            return await _context.LeadSources.ToListAsync();
        }
        public async Task<IEnumerable<LeadStatus>> GetLeadStatusesAsync()
        {
            return await _context.LeadStatus.ToListAsync();
        }
        public async Task<IEnumerable<LicenseStatus>> GetLicenseStatusesAsync()
        {
            return await _context.LicenseStatuses.ToListAsync();
        }
        public async Task<IEnumerable<LineForwarderType>> GetLineForwarderTypesAsync()
        {
            return await _context.LineForwarderTypes.ToListAsync();
        }
        public async Task<IEnumerable<LoadType>> GetLoadTypeAsync()
        {
            return await _context.LoadTypes.ToListAsync();
        }
        public async Task<IEnumerable<Zone>> GetZonesAsync()
        {
            return await _context.Zones.ToListAsync();
        }
        public async Task<IEnumerable<VoyageType>> GetVoyageTypesAsync()
        {
            return await _context.VoyageTypes.ToListAsync();
        }
        public async Task<IEnumerable<VoyageOwnType>> GetVoyageOwnTypesAsync()
        {
            return await _context.VoyageOwnTypes.ToListAsync();
        }
        public async Task<IEnumerable<VesselType>> GetVesselTypesAsync()
        {
            return await _context.VesselTypes.ToListAsync();
        }
        public async Task<IEnumerable<VesselOwnType>> GetVesselOwnTypesAsync()
        {
            return await _context.VesselOwnTypes.ToListAsync();
        }
        public async Task<IEnumerable<TruckType>> GetTruckTypesAsync()
        {
            return await _context.TruckTypes.ToListAsync();
        }
        public async Task<IEnumerable<TransportType>> GetTransportTypesAsync()
        {
            return await _context.TransportTypes.ToListAsync();
        }
        public async Task<IEnumerable<TransitType>> GetTransitTypesAsync()
        {
            return await _context.TransitTypes.ToListAsync();
        }
        public async Task<IEnumerable<TransitMethod>> GetTransitMethodsAsync()
        {
            return await _context.TransitMethods.ToListAsync();
        }
        public async Task<IEnumerable<TradeType>> GetTradeTypesAsync()
        {
            return await _context.TradeTypes.ToListAsync();
        }
        public async Task<IEnumerable<TIRCarnetStatus>> GetTIRCarnetStatusesAsync()
        {
            return await _context.TIRCarnetStatus.ToListAsync();
        }
        public async Task<IEnumerable<TIPStatus>> GetTIPStatusesAsync()
        {
            return await _context.TIPStatus.ToListAsync();
        }
        public async Task<IEnumerable<TerminalType>> GetTerminalTypesAsync()
        {
            return await _context.TerminalTypes.ToListAsync();
        }
        public async Task<IEnumerable<TelexReleaseStatus>> GetTelexReleaseStatusesAsync()
        {
            return await _context.TelexReleaseStatus.ToListAsync();
        }
        public async Task<IEnumerable<TaskStatus>> GetTaskStatusesAsync()
        {
            return await _context.TaskStatus.ToListAsync();
        }
        public async Task<IEnumerable<TaskCategory>> GetTaskCategoriesAsync()
        {
            return await _context.TaskCategories.ToListAsync();
        }
        public async Task<IEnumerable<TariffStatus>> GetTariffStatusesAsync()
        {
            return await _context.TariffStatus.ToListAsync();
        }
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }
        public async Task<IEnumerable<SystemObjectType>> GetSystemObjectTypesAsync()
        {
            return await _context.SystemObjectTypes.ToListAsync();
        }
        public async Task<IEnumerable<ShipmentStatus>> GetShipmentStatusesAsync()
        {
            return await _context.ShipmentStatus.ToListAsync();
        }
        public async Task<IEnumerable<SendToolType>> GetSendToolTypesAsync()
        {
            return await _context.SendToolTypes.ToListAsync();
        }
        public async Task<IEnumerable<RouteMode>> GetRouteModesAsync()
        {
            return await _context.RouteModes.ToListAsync();
        }
        public async Task<IEnumerable<RoleType>> GetRoleTypesAsync()
        {
            return await _context.RoleTypes.ToListAsync();
        }
        public async Task<IEnumerable<RiskLevel>> GetRiskLevelsAsync()
        {
            return await _context.RiskLevels.ToListAsync();
        }
        public async Task<IEnumerable<ReturnCashStatus>> GetReturnCashStatusesAsync()
        {
            return await _context.ReturnCashStatus.ToListAsync();
        }
        public async Task<IEnumerable<RateRequestStatus>> GetRateRequestStatusesAsync()
        {
            return await _context.RateRequestStatus.ToListAsync();
        }
        public async Task<IEnumerable<QuotationPriority>> GetQuotationPrioritiesAsync()
        {
            return await _context.QuotationPriorities.ToListAsync();
        }
        public async Task<IEnumerable<ProductTransactionType>> GetProductTransactionTypesAsync()
        {
            return await _context.ProductTransactionTypes.ToListAsync();
        }
        public async Task<IEnumerable<ProductRequestStatus>> GetProductRequestStatusesAsync()
        {
            return await _context.ProductRequestStatus.ToListAsync();
        }
        public async Task<IEnumerable<PortOperationType>> GetPortOperationTypesAsync()
        {
            return await _context.PortOperationTypes.ToListAsync();
        }
        public async Task<IEnumerable<PettyCashExpenseStatus>> GetPettyCashExpenseStatusesAsync()
        {
            return await _context.PettyCashExpenseStatus.ToListAsync();
        }
        public async Task<IEnumerable<PaymentType>> GetPaymentTypesAsync()
        {
            return await _context.PaymentTypes.ToListAsync();
        }
        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.ToListAsync();
        }
        public async Task<IEnumerable<PackageType>> GetPackageTypesAsync()
        {
            return await _context.PackageTypes.ToListAsync();
        }
        public async Task<IEnumerable<OpportunityStage>> GetOpportunityStagesAsync()
        {
            return await _context.OpportunityStages.ToListAsync();
        }
        public async Task<IEnumerable<OperationType>> GetOperationTypesAsync()
        {
            return await _context.OperationTypes.ToListAsync();
        }
        public async Task<IEnumerable<MovementType>> GetMovementTypesAsync()
        {
            return await _context.MovementTypes.ToListAsync();
        }
        public async Task<IEnumerable<MovementNode>> GetMovementNodesAsync()
        {
            return await _context.MovementNodes.ToListAsync();
        }
        public async Task<IEnumerable<MeasurementUnit>> GetMeasurementUnitsByCategoryIdAsync(int categoryId)
        {
            return await _context.MeasurementUnits.Where(x => x.MeasurementUnitCategoryId == categoryId).ToListAsync();
        }
        public async Task<IEnumerable<MeasurementUnitCategory>> GetMeasurementUnitCategoriesAsync()
        {
            return await _context.MeasurementUnitCategories.ToListAsync();
        }
        public async Task<IEnumerable<ManifestType>> GetManifestTypesAsync()
        {
            return await _context.ManifestTypes.ToListAsync();
        }
        public async Task<IEnumerable<LostReasonCategory>> GetLostReasonCategoriesAsync()
        {
            return await _context.LostReasonCategories.ToListAsync();
        }
        public async Task<IEnumerable<LostReason>> GetLostReasonsAsync()
        {
            return await _context.LostReasons.ToListAsync();
        }
        public async Task<IEnumerable<LostReason>> GetLostReasonsByCategoryIdAsync(int categoryId)
        {
            return await _context.LostReasons.Where(x => x.LostReasonCategoryId == categoryId).ToListAsync();
        }
        public async Task<IEnumerable<LocationType>> GetLocationTypeAsync()
        {
            return await _context.LocationTypes.ToListAsync();
        }
        public async Task<IEnumerable<Location>> GetLocationsAsync()
        {
            return await _context.Locations.ToListAsync();
        }
        public async Task<IEnumerable<Location>> GetLocationsByTypeIdAsync(int typeId)
        {
            return await _context.Locations.Where(x => x.LocationTypeId == typeId).ToListAsync();
        }
        public async Task<IEnumerable<Carrier>> GetCarriersAsync(int typeId)
        {
            return await _context.Carriers
                .Include(x => x.Person)
                .Where(x => x.CarrierTypeId == typeId).ToListAsync();
        }

        public async Task<IEnumerable<BankAccountPurpose>> GetBankAccountPurposes()
        {
            return await _context.BankAccountPurposes.ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            return await _context.Companies
                .Include(x => x.Person)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agent>> GetAgents()
        {
            return await _context.Agents
                .Include(x => x.Person)
                .ToListAsync();
        }
        public async Task<IEnumerable<BranchType>> GetBranchTypesAsync()
        {
            return await _context.BranchTypes.ToListAsync();
        }
        public async Task<IEnumerable<TariffBook>> GetTariffBooksAsync(int tariffTypeId)
        {
            return await _context.TariffBooks
                .Where(x => x.TariffTypeId == tariffTypeId).ToListAsync();
        }
    }
}
