using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobInsurance : BusinessEntity
    {
        public Guid JobId { get; set; }
        public bool IsInsuranceRequired { get; set; }
        public string? InsurancePolicyNumber { get; set; }  //شماره بیمه نامه
        public Guid? InsuranceCompanyId { get; set; }
        public decimal? InsuredValueAmount { get; set; }
        public int? InsuranceValueCurrencyId { get; set; }

        //Relations
        public Job Job { get; set; }
        public Person InsuranceCompany { get; set; }
        public Currency InsuranceValueCurrency { get; set; }
    }
}
