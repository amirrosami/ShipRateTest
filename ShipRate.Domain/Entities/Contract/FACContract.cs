using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Contract
{
    public class FACContract : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid CarrierId { get; set; }
        public Guid ForwarderId { get; set; }   //is customer
        public string FacContractTitle { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal? FACPercent { get; set; }
        public decimal? FACPerTEU { get; set; }
        public decimal? FACPerCBM { get; set; }
        public int? CurrencyId { get; set; }

        public int? POLFreeDays { get; set; }
        public int? PODFreeDays { get; set; }
        public string? FacContractRemarks { get; set; } = string.Empty;

        //Relations
        public Company? Company { get; set; }
        public Carrier? Carrier { get; set; }
        public Customer? Customer { get; set; }
        public Currency? Currency { get; set; }

    }
}