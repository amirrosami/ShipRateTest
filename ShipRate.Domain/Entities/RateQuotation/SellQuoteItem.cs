using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class SellQuoteItem : BusinessEntity
    {
        public int ResponsibleCompanyId { get; set; }
        public Guid SellQuoteId { get; set; }
        public Guid? CarrierId { get; set; }
        public Guid? VendorId { get; set; }
        public int? ServiceTypeId { get; set; }

        public int? ContainerSizeTypeId { get; set; }

        public int LineNumber { get; set; }
        public int ChargeTypeId { get; set; }
        public int CalculationById { get; set; }
        public int CalculationTypeId { get; set; }
        public bool IsTaxable { get; set; }

        public string? Description { get; set; }


        public decimal SellQuantity { get; set; }
        public decimal SellUnitRate { get; set; }
        public decimal SellTaxPercent { get; set; }
        public decimal SellTaxAmount { get; set; }
        public decimal SellTotalAmount { get; set; }   //Final
        public int SellCurrencyId { get; set; }

        public decimal BuyQuantity { get; set; }
        public decimal BuyUnitRate { get; set; }
        public decimal BuyTaxPercent { get; set; }
        public decimal BuyTaxAmount { get; set; }
        public decimal BuyTotalAmount { get; set; }   //Final
        public int BuyCurrencyId { get; set; }



        public Guid? ExchangeRateId { get; set; }



        public string? RemarksToCustomer { get; set; }
        public string? InternalNotes { get; set; }

        //Relations
        public Company ResponsibleCompany { get; set; }
        public ServiceType ServiceType { get; set; }
        public SellQuote SellQuote { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
        public ChargeType ChargeType { get; set; }
        public Currency BuyCurrency { get; set; }
        public Currency SellCurrency { get; set; }
        public CalculationBy CalculationBy { get; set; }
        public CalculationType CalculationType { get; set; }
        public Person Vendor { get; set; }
        public Carrier Carrier { get; set; }
        public ExchangeRate? ExchangeRate { get; set; }
    }
}
