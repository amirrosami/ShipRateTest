using ShipRate.Application.Interfaces.BaseInfo;
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
using TaskStatus = ShipRate.Domain.Entities.BaseInfo.Generals.TaskStatus;

namespace ShipRate.Application.Services.BaseInfo
{
    public class BaseDataService : IBaseDataService
    {
        public IBaseDataRepository _repository { get; set; }
        public BaseDataService(IBaseDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PersonalityType>> GetPersonalityTypesAsync()
        {
            return await _repository.GetAllPersonalityTypesAsync();
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return await _repository.GetCountriesAsync();
        }

        public async Task<IEnumerable<Association>> GetAssociationAsync()
        {
            return await _repository.GetAssociationAsync();
        }
        public async Task<IEnumerable<Bank>> GetBanksAsync()
        {
            return await _repository.GetBanksAsync();
        }
        public async Task<IEnumerable<BlackListStatus>> GetBlackListStatusesAsync()
        {
            return await _repository.GetBlackListStatusesAsync();
        }
        public async Task<IEnumerable<BLChangeType>> GetBLChangeTypesAsync()
        {
            return await _repository.GetBLChangeTypesAsync();
        }

        public async Task<IEnumerable<BLReleaseType>> GetBLReleaseTypesAsync()
        {
            return await _repository.GetBLReleaseTypesAsync();
        }
        public async Task<IEnumerable<BLStatus>> GetBLStatusesAsync()
        {
            return await _repository.GetBLStatusesAsync();
        }
        public async Task<IEnumerable<BLType>> GetBLTypesAsync()
        {
            return await _repository.GetBLTypesAsync();
        }
        public async Task<IEnumerable<BookingChangeType>> GetBookingChangeTypesAsync()
        {
            return await _repository.GetBookingChangeTypesAsync();
        }
        public async Task<IEnumerable<BookingStatus>> GetBookingStatusesAsync()
        {
            return await _repository.GetBookingStatusesAsync();
        }
        public async Task<IEnumerable<BuySellType>> GetBuySellTypesAsync()
        {
            return await _repository.GetBuySellTypesAsync();
        }
        public async Task<IEnumerable<CalculationBy>> GetCalculationBysAsync()
        {
            return await _repository.GetCalculationBysAsync();
        }
        public async Task<IEnumerable<CalculationType>> GetCalculationTypesAsync()
        {
            return await _repository.GetCalculationTypesAsync();
        }
        public async Task<IEnumerable<CargoCommodity>> GetCargoCommoditiesAsync()
        {
            return await _repository.GetCargoCommoditiesAsync();
        }
        public async Task<IEnumerable<CargoHandlingType>> GetCargoHandlingTypesAsync()
        {
            return await _repository.GetCargoHandlingTypesAsync();
        }
        public async Task<IEnumerable<CargoNature>> GetCargoNaturesAsync()
        {
            return await _repository.GetCargoNaturesAsync();
        }
        public async Task<IEnumerable<CarrierType>> GetCarrierTypesAsync()
        {
            return await _repository.GetCarrierTypesAsync();
        }
        public async Task<IEnumerable<ChargeTypeCategory>> GetChargeTypeCategoriesAsync()
        {
            return await _repository.GetChargeTypeCategoriesAsync();
        }
        public async Task<IEnumerable<ChargeType>> GetChargeTypesAsync()
        {
            return await _repository.GetChargeTypesAsync();
        }
        public async Task<IEnumerable<ChequeType>> GetChequeTypesAsync()
        {
            return await _repository.GetChequeTypesAsync();
        }
        public async Task<IEnumerable<CityType>> GetCityTypesAsync()
        {
            return await _repository.GetCityTypesAsync();
        }
        public async Task<IEnumerable<ClaimStatus>> GetClaimStatusesAsync()
        {
            return await _repository.GetClaimStatusesAsync();
        }
        public async Task<IEnumerable<ClaimType>> GetClaimTypesAsync()
        {
            return await _repository.GetClaimTypesAsync();
        }
        public async Task<IEnumerable<ClearanceType>> GetClearanceTypesAsync()
        {
            return await _repository.GetClearanceTypesAsync();
        }
        public async Task<IEnumerable<CntActionType>> GetCntActionTypesAsync()
        {
            return await _repository.GetCntActionTypesAsync();
        }
        public async Task<IEnumerable<CntOwnType>> GetCntOwnTypesAsync()
        {
            return await _repository.GetCntOwnTypesAsync();
        }
        public async Task<IEnumerable<CommentType>> GetCommentTypesAsync()
        {
            return await _repository.GetCommentTypesAsync();
        }
        public async Task<IEnumerable<EventActionSource>> GetEventActionSourcesAsync()
        {
            return await _repository.GetEventActionSourcesAsync();
        }
        public async Task<IEnumerable<ContainerConditionStatus>> GetContainerConditionStatusesAsync()
        {
            return await _repository.GetContainerConditionStatusesAsync();
        }
        public async Task<IEnumerable<ContainerDamageArea>> GetContainerDamageAreasAsync()
        {
            return await _repository.GetContainerDamageAreasAsync();
        }
        public async Task<IEnumerable<ContainerDamageSeverity>> GetContainerDamageSeveritiesAsync()
        {
            return await _repository.GetContainerDamageSeveritiesAsync();
        }
        public async Task<IEnumerable<ContainerDamageType>> GetContainerDamageTypesAsync()
        {
            return await _repository.GetContainerDamageTypesAsync();
        }
        public async Task<IEnumerable<ContainerIOType>> GetContainerIOTypesAsync()
        {
            return await _repository.GetContainerIOTypesAsync();
        }
        public async Task<IEnumerable<ContainerSize>> GetContainerSizesAsync()
        {
            return await _repository.GetContainerSizesAsync();
        }
        public async Task<IEnumerable<ContainerType>> GetContainerTypesAsync()
        {
            return await _repository.GetContainerTypesAsync();
        }
        public async Task<IEnumerable<ContainerSizeType>> GetContainerSizeTypesAsync()
        {
            return await _repository.GetContainerSizeTypesAsync();
        }
        public async Task<IEnumerable<ContractType>> GetContractTypesAsync()
        {
            return await _repository.GetContractTypesAsync();
        }
        public async Task<IEnumerable<CourierMethod>> GetCourierMethodsAsync()
        {
            return await _repository.GetCourierMethodsAsync();
        }
        public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
        {
            return await _repository.GetCurrenciesAsync();
        }
        public async Task<IEnumerable<CustomField>> GetCustomFieldsAsync()
        {
            return await _repository.GetCustomFieldsAsync();
        }
        public async Task<IEnumerable<CustomsDeclarationType>> GetCustomsDeclarationTypesAsync()
        {
            return await _repository.GetCustomsDeclarationTypesAsync();
        }
        public async Task<IEnumerable<CustomsStatus>> GetCustomsStatusesAsync()
        {
            return await _repository.GetCustomsStatusesAsync();
        }
        public async Task<IEnumerable<DepositType>> GetDepositTypesAsync()
        {
            return await _repository.GetDepositTypesAsync();
        }
        public async Task<IEnumerable<DepotType>> GetDepotTypesAsync()
        {
            return await _repository.GetDepotTypesAsync();
        }
        public async Task<IEnumerable<DetentionDiscountType>> GetDetentionDiscountTypesAsync()
        {
            return await _repository.GetDetentionDiscountTypesAsync();
        }
        public async Task<IEnumerable<DetentionStatus>> GetDetentionStatusesAsync()
        {
            return await _repository.GetDetentionStatusesAsync();
        }
        public async Task<IEnumerable<DetentionUpdater>> GetDetentionUpdatersAsync()
        {
            return await _repository.GetDetentionUpdatersAsync();
        }
        public async Task<IEnumerable<DGPackingGroup>> GetDGPackingGroupsAsync()
        {
            return await _repository.GetDGPackingGroupsAsync();
        }
        public async Task<IEnumerable<DGRegulationSet>> GetDGRegulationSetsAsync()
        {
            return await _repository.GetDGRegulationSetsAsync();
        }
        public async Task<IEnumerable<DGClass>> GetDGDGClassesAsync()
        {
            return await _repository.GetDGDGClassesAsync();
        }
        public async Task<IEnumerable<DischargeLoad>> GetDischargeLoadsAsync()
        {
            return await _repository.GetDischargeLoadsAsync();
        }
        public async Task<IEnumerable<DocumentTypeCategory>> GetDocumentTypeCategoriesAsync()
        {
            return await _repository.GetDocumentTypeCategoriesAsync();
        }
        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync()
        {
            return await _repository.GetDocumentTypesAsync();
        }
        public async Task<IEnumerable<DOStatus>> GetDOStatusesAsync()
        {
            return await _repository.GetDOStatusesAsync();
        }
        public async Task<IEnumerable<EDIMessageType>> GetEDIMessageTypesAsync()
        {
            return await _repository.GetEDIMessageTypesAsync();
        }
        public async Task<IEnumerable<EmailDirection>> GetEmailDirectionsAsync()
        {
            return await _repository.GetEmailDirectionsAsync();
        }
        public async Task<IEnumerable<EndorsementType>> GetEndorsementTypesAsync()
        {
            return await _repository.GetEndorsementTypesAsync();
        }
        public async Task<IEnumerable<ENSDeclarationStatus>> GetENSDeclarationStatusesAsync()
        {
            return await _repository.GetENSDeclarationStatusesAsync();
        }
        public async Task<IEnumerable<EventTypeCategory>> GetEventTypeCategoriesAsync()
        {
            return await _repository.GetEventTypeCategoriesAsync();
        }
        public async Task<IEnumerable<Domain.Entities.BaseInfo.Generals.EventType>> GetEventTypesAsync()
        {
            return await _repository.GetEventTypesAsync();
        }
        public async Task<IEnumerable<ExchangeRateBase>> GetExchangeRateBasesAsync()
        {
            return await _repository.GetExchangeRateBasesAsync();
        }
        public async Task<IEnumerable<FinanceDirection>> GetFinanceDirectionsAsync()
        {
            return await _repository.GetFinanceDirectionsAsync();
        }
        public async Task<IEnumerable<FollowUpPriority>> GetFollowUpPrioritiesAsync()
        {
            return await _repository.GetFollowUpPrioritiesAsync();
        }
        public async Task<IEnumerable<FollowUpStatus>> GetFollowUpStatusesAsync()
        {
            return await _repository.GetFollowUpStatusesAsync();
        }
        public async Task<IEnumerable<FullMT>> GetFullMTsAsync()
        {
            return await _repository.GetFullMTsAsync();
        }
        public async Task<IEnumerable<HologramStatus>> GetHologramStatusesAsync()
        {
            return await _repository.GetHologramStatusesAsync();
        }
        public async Task<IEnumerable<HSLevel>> GetHSLevelsAsync()
        {
            return await _repository.GetHSLevelsAsync();
        }
        public async Task<IEnumerable<Incoterm>> GetIncotermsAsync()
        {
            return await _repository.GetIncotermsAsync();
        }
        public async Task<IEnumerable<Industry>> GetIndustriesAsync()
        {
            return await _repository.GetIndustriesAsync();
        }
        public async Task<IEnumerable<InfoTypeCategory>> GetInfoTypeCategoriesAsync()
        {
            return await _repository.GetInfoTypeCategoriesAsync();
        }
        public async Task<IEnumerable<InfoType>> GetInfoTypesAsync()
        {
            return await _repository.GetInfoTypesAsync();
        }
        public async Task<IEnumerable<InfoType>> GetInfoTypesAsync(int categoryId)
        {
            return await _repository.GetInfoTypesAsync(categoryId);
        }
        public async Task<IEnumerable<InOut>> GetInOutsAsync()
        {
            return await _repository.GetInOutsAsync();
        }
        public async Task<IEnumerable<InspectionMethod>> GetInspectionMethodsAsync()
        {
            return await _repository.GetInspectionMethodsAsync();
        }
        public async Task<IEnumerable<InspectionResult>> GetInspectionResultsAsync()
        {
            return await _repository.GetInspectionResultsAsync();
        }
        public async Task<IEnumerable<InspectionType>> GetInspectionTypesAsync()
        {
            return await _repository.GetInspectionTypesAsync();
        }
        public async Task<IEnumerable<InvoiceFlag>> GetInvoiceFlagsAsync()
        {
            return await _repository.GetInvoiceFlagsAsync();
        }
        public async Task<IEnumerable<InvoiceStatus>> GetInvoiceStatusesAsync()
        {
            return await _repository.GetInvoiceStatusesAsync();
        }
        public async Task<IEnumerable<ServiceType>> GetServiceTypesAsync()
        {
            return await _repository.GetServiceTypesAsync();
        }
        public async Task<IEnumerable<JobStage>> GetJobStagesAsync()
        {
            return await _repository.GetJobStagesAsync();
        }
        public async Task<IEnumerable<JobStatus>> GetJobStatusesAsync()
        {
            return await _repository.GetJobStatusesAsync();
        }
        public async Task<IEnumerable<JobType>> GetJobTypesAsync()
        {
            return await _repository.GetJobTypesAsync();
        }
        public async Task<IEnumerable<LeadSource>> GetLeadSourcesAsync()
        {
            return await _repository.GetLeadSourcesAsync();
        }
        public async Task<IEnumerable<LeadStatus>> GetLeadStatusesAsync()
        {
            return await _repository.GetLeadStatusesAsync();
        }
        public async Task<IEnumerable<LicenseStatus>> GetLicenseStatusesAsync()
        {
            return await _repository.GetLicenseStatusesAsync();
        }
        public async Task<IEnumerable<LineForwarderType>> GetLineForwarderTypesAsync()
        {
            return await _repository.GetLineForwarderTypesAsync();
        }
        public async Task<IEnumerable<LoadType>> GetLoadTypeAsync()
        {
            return await _repository.GetLoadTypeAsync();
        }
        public async Task<IEnumerable<Zone>> GetZonesAsync()
        {
            return await _repository.GetZonesAsync();
        }
        public async Task<IEnumerable<VoyageType>> GetVoyageTypesAsync()
        {
            return await _repository.GetVoyageTypesAsync();
        }
        public async Task<IEnumerable<VoyageOwnType>> GetVoyageOwnTypesAsync()
        {
            return await _repository.GetVoyageOwnTypesAsync();
        }
        public async Task<IEnumerable<VesselType>> GetVesselTypesAsync()
        {
            return await _repository.GetVesselTypesAsync();
        }
        public async Task<IEnumerable<VesselOwnType>> GetVesselOwnTypesAsync()
        {
            return await _repository.GetVesselOwnTypesAsync();
        }
        public async Task<IEnumerable<TruckType>> GetTruckTypesAsync()
        {
            return await _repository.GetTruckTypesAsync();
        }
        public async Task<IEnumerable<TransportType>> GetTransportTypesAsync()
        {
            return await _repository.GetTransportTypesAsync();
        }
        public async Task<IEnumerable<TransitType>> GetTransitTypesAsync()
        {
            return await _repository.GetTransitTypesAsync();
        }
        public async Task<IEnumerable<TransitMethod>> GetTransitMethodsAsync()
        {
            return await _repository.GetTransitMethodsAsync();
        }
        public async Task<IEnumerable<TradeType>> GetTradeTypesAsync()
        {
            return await _repository.GetTradeTypesAsync();
        }
        public async Task<IEnumerable<TIRCarnetStatus>> GetTIRCarnetStatusesAsync()
        {
            return await _repository.GetTIRCarnetStatusesAsync();
        }
        public async Task<IEnumerable<TIPStatus>> GetTIPStatusesAsync()
        {
            return await _repository.GetTIPStatusesAsync();
        }
        public async Task<IEnumerable<TerminalType>> GetTerminalTypesAsync()
        {
            return await _repository.GetTerminalTypesAsync();
        }
        public async Task<IEnumerable<TelexReleaseStatus>> GetTelexReleaseStatusesAsync()
        {
            return await _repository.GetTelexReleaseStatusesAsync();
        }
        public async Task<IEnumerable<TaskStatus>> GetTaskStatusesAsync()
        {
            return await _repository.GetTaskStatusesAsync();
        }
        public async Task<IEnumerable<TaskCategory>> GetTaskCategoriesAsync()
        {
            return await _repository.GetTaskCategoriesAsync();
        }
        public async Task<IEnumerable<TariffStatus>> GetTariffStatusesAsync()
        {
            return await _repository.GetTariffStatusesAsync();
        }
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await _repository.GetTagsAsync();
        }
        public async Task<IEnumerable<SystemObjectType>> GetSystemObjectTypesAsync()
        {
            return await _repository.GetSystemObjectTypesAsync();
        }
        public async Task<IEnumerable<ShipmentStatus>> GetShipmentStatusesAsync()
        {
            return await _repository.GetShipmentStatusesAsync();
        }
        public async Task<IEnumerable<SendToolType>> GetSendToolTypesAsync()
        {
            return await _repository.GetSendToolTypesAsync();
        }
        public async Task<IEnumerable<RouteMode>> GetRouteModesAsync()
        {
            return await _repository.GetRouteModesAsync();
        }
        public async Task<IEnumerable<RoleType>> GetRoleTypesAsync()
        {
            return await _repository.GetRoleTypesAsync();
        }
        public async Task<IEnumerable<RiskLevel>> GetRiskLevelsAsync()
        {
            return await _repository.GetRiskLevelsAsync();
        }
        public async Task<IEnumerable<ReturnCashStatus>> GetReturnCashStatusesAsync()
        {
            return await _repository.GetReturnCashStatusesAsync();
        }
        public async Task<IEnumerable<RateRequestStatus>> GetRateRequestStatusesAsync()
        {
            return await _repository.GetRateRequestStatusesAsync();
        }
        public async Task<IEnumerable<QuotationPriority>> GetQuotationPrioritiesAsync()
        {
            return await _repository.GetQuotationPrioritiesAsync();
        }
        public async Task<IEnumerable<ProductTransactionType>> GetProductTransactionTypesAsync()
        {
            return await _repository.GetProductTransactionTypesAsync();
        }
        public async Task<IEnumerable<ProductRequestStatus>> GetProductRequestStatusesAsync()
        {
            return await _repository.GetProductRequestStatusesAsync();
        }
        public async Task<IEnumerable<PortOperationType>> GetPortOperationTypesAsync()
        {
            return await _repository.GetPortOperationTypesAsync();
        }
        public async Task<IEnumerable<PettyCashExpenseStatus>> GetPettyCashExpenseStatusesAsync()
        {
            return await _repository.GetPettyCashExpenseStatusesAsync();
        }
        public async Task<IEnumerable<PaymentType>> GetPaymentTypesAsync()
        {
            return await _repository.GetPaymentTypesAsync();
        }
        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _repository.GetPaymentMethodsAsync();
        }
        public async Task<IEnumerable<PackageType>> GetPackageTypesAsync()
        {
            return await _repository.GetPackageTypesAsync();
        }
        public async Task<IEnumerable<OpportunityStage>> GetOpportunityStagesAsync()
        {
            return await _repository.GetOpportunityStagesAsync();
        }
        public async Task<IEnumerable<OperationType>> GetOperationTypesAsync()
        {
            return await _repository.GetOperationTypesAsync();
        }
        public async Task<IEnumerable<MovementType>> GetMovementTypesAsync()
        {
            return await _repository.GetMovementTypesAsync();
        }
        public async Task<IEnumerable<MovementNode>> GetMovementNodesAsync()
        {
            return await _repository.GetMovementNodesAsync();
        }
        public async Task<IEnumerable<MeasurementUnit>> GetMeasurementUnitsByCategoryIdAsync(int categoryId)
        {
            return await _repository.GetMeasurementUnitsByCategoryIdAsync(categoryId);
        }
        public async Task<IEnumerable<MeasurementUnitCategory>> GetMeasurementUnitCategoriesAsync()
        {
            return await _repository.GetMeasurementUnitCategoriesAsync();
        }
        public async Task<IEnumerable<ManifestType>> GetManifestTypesAsync()
        {
            return await _repository.GetManifestTypesAsync();
        }
        public async Task<IEnumerable<LostReasonCategory>> GetLostReasonCategoriesAsync()
        {
            return await _repository.GetLostReasonCategoriesAsync();
        }
        public async Task<IEnumerable<LostReason>> GetLostReasonsAsync()
        {
            return await _repository.GetLostReasonsAsync();
        }
        public async Task<IEnumerable<LostReason>> GetLostReasonsByCategoryIdAsync(int categoryId)
        {
            return await _repository.GetLostReasonsByCategoryIdAsync(categoryId);
        }
        public async Task<IEnumerable<LocationType>> GetLocationTypeAsync()
        {
            return await _repository.GetLocationTypeAsync();
        }
        public async Task<IEnumerable<Domain.Entities.BaseInfo.Locations.Location>> GetLocationsAsync()
        {
            return await _repository.GetLocationsAsync();
        }
        public async Task<IEnumerable<Domain.Entities.BaseInfo.Locations.Location>> GetLocationsByTypeIdAsync(int typeId)
        {
            return await _repository.GetLocationsByTypeIdAsync(typeId);
        }
        public async Task<IEnumerable<Carrier>> GetCarriers(int carrierTypeId)
        {
            return await _repository.GetCarriersAsync(carrierTypeId);
        }


        public async Task<IEnumerable<BankAccountPurpose>> GetBankAccountPurposesAsync()
        {
            return await _repository.GetBankAccountPurposes();
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            return await _repository.GetCompanies();
        }

        public async Task<IEnumerable<Agent>> GetAgents()
        {
            return await _repository.GetAgents();
        }

        public async Task<IEnumerable<BranchType>> GetBranchTypesAsync()
        {
            return await _repository.GetBranchTypesAsync();
        }


        public async Task<IEnumerable<TariffBook>> GetTariffBooksByTypeIdAsync(int tariffTypeId)
        {
            return await _repository.GetTariffBooksAsync(tariffTypeId);
        }
    }
}
