using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Finance
{
    public class Bank : BusinessEntity
    {
        public string BankCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankLocalName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string SWIFTCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        //Relations
        public Country? Country { get; set; }
    }
}
