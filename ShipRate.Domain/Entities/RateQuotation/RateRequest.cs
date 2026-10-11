using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequest : BusinessEntity
    {
        public int CompanyId { get; set; }   //مالک دیتا
        public int RateRequestSourceId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerContactId { get; set; }

        public string RequestNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string? Introducer { get; set; }


        public int LoadTypeId { get; set; }
        public int CargoHandlingTypeId { get; set; }
        public int CargoNatureId { get; set; }
        public int? MovementTypeId { get; set; }



        public Guid? CargoCommodityId { get; set; }
        public string? CargoCommodityDescription { get; set; } = string.Empty;
        public string? OriginHSCode { get; set; } = string.Empty;
        public string? DestinationHSCode { get; set; } = string.Empty;
        public DateTime? CargoReadinessDate { get; set; }

        public bool IsHazardous { get; set; }
        public Guid? DGId { get; set; }
        public string? DGDescription { get; set; }


        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }

        public int? TradeTypeId { get; set; }

        public long? CargoValue { get; set; }
        public int? CurrencyId { get; set; }

        public int? QuotationPriorityId { get; set; }

        //Transit Specific

        public bool PickupRequired { get; set; }
        public string? PickupAddress { get; set; }
        public bool DeliveryRequired { get; set; }
        public string? DeliveryAddress { get; set; }

        public bool CustomsClearanceRequired { get; set; }
        public bool InsuranceRequired { get; set; }
        public bool WarehousingRequired { get; set; }
        public bool StuffingRequired { get; set; }
        public bool? ExternalTransitRequired { get; set; }
        public bool? InternalTransitRequired { get; set; }

        public int? PaymentTypeId { get; set; }

        public bool HasEx1 { get; set; }
        public string? OriginZipCode { get; set; }  //برای ترانزیت خیلی مهم است
        public string? DestinationZipCode { get; set; }
        public bool? TransitHasTranshipment { get; set; } //حمل مستقیم یا حمل ترانشیپ برای ترانزیت وارداتی
        public int? SellToCountryId { get; set; }//فروش کالا به کدام کشور است 

        public int StatusId { get; set; }
        public Guid? SalesManagerId { get; set; }

        public Guid? AssignedSalesUserId { get; set; }
        public DateTime? AssignedSalesUserDate { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        public Guid? RejectUserId { get; set; }
        public DateTime? RejectDate { get; set; }
        public string? RejectComments { get; set; }

        //Relations
        public Company Company { get; set; }
        public RateRequestSource RateRequestSource { get; set; }
        public Person? Customer { get; set; }
        public Lead? Lead { get; set; }
        public Location Origin { get; set; }
        public Location Destination { get; set; }
        public CargoCommodity? CargoCommodity { get; set; }
        public QuotationPriority? QuotationPriority { get; set; }
        public CargoHandlingType CargoHandlingType { get; set; }
        public CargoNature CargoNature { get; set; }
        public RateRequestStatus RateRequestStatus { get; set; }
        public LoadType LoadType { get; set; }
        public TradeType TradeType { get; set; }
        public Currency? Currency { get; set; }
        public PersonContact? CustomerContact { get; set; }
        public Country? SellToCountry { get; set; }
        public MovementType? MovementType { get; set; }
        public PaymentType PaymentType { get; set; }
        public ApplicationUser? AssignedSalesUser { get; set; }
        public ApplicationUser? SalesManager { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        public ApplicationUser? LastEditedByUser { get; set; }


        // Navigation Properties
        public virtual ICollection<RateRequestService> Services { get; set; }
        public virtual ICollection<RateRequestTransportType> TransportTypes { get; set; }
        public virtual ICollection<RateRequestPackage> Packages { get; set; }
        public virtual ICollection<RateRequestContainer> Containers { get; set; }
        public virtual ICollection<RateRequestInquiry> Inquiries { get; set; }
        public virtual ICollection<CargoDetail> CargoDetails { get; set; }

    }
}

