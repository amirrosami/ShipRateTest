using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class Job : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int JobSourceTypeId { get; set; }
        public string JobNumber { get; set; }
        public string JobFileNumber { get; set; }
        public DateTime? JobDate { get; set; }
        public int JobStatusId { get; set; }

        public Guid? SellQuoteId { get; set; }
        public int TransportTypeId { get; set; }
        public int TradeTypeId { get; set; }
        public int LoadTypeId { get; set; }
        public int? IncotermId { get; set; }
        public string IncotermsLocationName { get; set; } = string.Empty;
        public int FreightPaymentTypeId { get; set; }


        public int CargoHandlingTypeId { get; set; }
        public int CargoNatureId { get; set; }
        public DateTime? CargoReadyDate { get; set; }

        public int? TotalPackages { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }
        public decimal? ChargeableWeightKg { get; set; }
        public decimal? ContainerCount { get; set; }





        public Guid? SalesUserId { get; set; }
        //     public Guid? MasterJobId { get; set; }   //for consolidated / master-house
        //    public bool IsConsolMaster { get; set; }
        //    ConsolType VARCHAR(20),                          -- DIRECT_CONSOL, BUYER_CONSOL, CO_LOAD



        public DateTime? ClosedDate { get; set; }
        public string? CancellationReason { get; set; }

        public string? InternalNotes { get; set; }
        public string? OperationalNotes { get; set; }
        public string? CustomerNotes { get; set; }


        public Guid? POLDetentionTariffId { get; set; }
        public Guid? PODDetentionTariffId { get; set; }

        //Relations
        public Company Company { get; set; }
        public SellQuote SellQuote { get; set; }
        public JobStatus JobStatus { get; set; }
        public TransportType TransportType { get; set; }
        public CargoHandlingType CargoHandlingType { get; set; }
        public CargoNature CargoNature { get; set; }
        public LoadType LoadType { get; set; }
        public TradeType TradeType { get; set; }
        public Incoterm Incoterm { get; set; }
        public PaymentType FreightPaymentType { get; set; }
        public TariffBook? POLDetentionTariff { get; set; }
        public TariffBook? PODDetentionTariff { get; set; }
        public ApplicationUser SalesPerson { get; set; }
        public JobSourceType JobSourceType { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        public ApplicationUser? UpdatedByUser { get; set; }

        public List<JobRouting> Routing = new();
        public List<JobInsurance> Insurance = new();
        public List<JobLC> LC = new();
        public List<JobCompanyRole> JobCompanyRoles = new();

        public List<RateRequestContainer> Containers = new();
        public List<RateRequestPackage> Packages = new();
        public List<JobParty> Parties = new();
        public List<CargoDetail> CargoDetails = new();
    }
}
