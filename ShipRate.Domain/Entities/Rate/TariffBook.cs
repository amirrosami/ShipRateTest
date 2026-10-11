using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;

//14050231
//14050231
namespace ShipRate.Domain.Entities.Rate
{
    public class TariffType : SystemEntity
    {
        public string TariffTypeName { get; set; }
        public string TariffTypeLocalName { get; set; }
        public int? SortOrder { get; set; }
    }
    public class ScopeType : SystemEntity
    {
        public string ScopeTypeName { get; set; } = null!;
        public string ScopeTypeLocalName { get; set; } = null!;
        public string? Description { get; set; }
    }
    public class TariffBook : BusinessEntity
    {
        public int TariffTypeId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public Guid? PartyId { get; set; }
        public int? CurrencyId { get; set; }
        public int? StatusId { get; set; }

        //Relations
        public virtual TariffType? TariffType { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual Person? Party { get; set; }
        public virtual TariffStatus? Status { get; set; }
        public virtual ICollection<TariffRule> TariffRules { get; set; } = new HashSet<TariffRule>();
    }

    public class TariffRule : BusinessEntity
    {
        public Guid TariffBookId { get; set; }
        public int ChargeTypeId { get; set; }
        public int? ScopeTypeId { get; set; }
        public int? OriginCountryId { get; set; }
        public int? DestinationCountryId { get; set; }
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }

        public Guid? PartyId { get; set; }
        public int CalculationById { get; set; }
        public int CalculationTypeId { get; set; }

        public int? ContainerSizeId { get; set; }
        public int? ContainerTypeId { get; set; }
        public int? ContainerSizeTypeId { get; set; }

        public bool? IsOverSized { get; set; }
        public bool? IsOverWeight { get; set; }
        public bool? IsDG { get; set; }

        public int? TradeTypeId { get; set; }
        public int? FullMTId { get; set; }


        //public Guid? RuleGroupId { get; set; }

        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal Amount { get; set; }
        public int? CurrencyId { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public string? ExtraAttributes { get; set; }

        public virtual TariffBook TariffBook { get; set; } = null!;
        public virtual ChargeType ChargeType { get; set; } = null!;
        public virtual Person? Party { get; set; }
        public virtual Currency Currency { get; set; } = null!;
        public virtual CalculationBy CalculationBy { get; set; } = null!;
        public virtual CalculationType CalculationType { get; set; } = null!;
        public virtual ScopeType? ScopeType { get; set; }
        public virtual FullMT? FullMT { get; set; }

        public virtual Country? OriginCountry { get; set; }
        public virtual Country? DestinationCountry { get; set; }

        public virtual Location? Origin { get; set; }
        public virtual Location? Destination { get; set; }

        public virtual ContainerSize? ContainerSize { get; set; }
        public virtual ContainerType? ContainerType { get; set; }
        public virtual ContainerSizeType? ContainerSizeType { get; set; }
        public virtual TradeType? TradeType { get; set; }

        //public virtual ICollection<TariffRuleBaseCharge> BaseCharges { get; set; } = new HashSet<TariffRuleBaseCharge>();
        //public virtual TariffContext? Context { get; set; }
        //public virtual TariffScope? Scope { get; set; }
        //public virtual TariffRoute? Route { get; set; }
        //public virtual TariffRuleFormula? Formula { get; set; }
        //public virtual TariffRuleCoverage? Coverage { get; set; }

    }
    public class TariffRuleFormula : BusinessEntity
    {
        public Guid TariffRuleId { get; set; }
        public string FormulaExpression { get; set; } = null!;
        public string? FormulaDescription { get; set; }

        public virtual TariffRule TariffRule { get; set; } = null!;
    }

    public class TariffRuleCoverage : BusinessEntity
    {
        public Guid TariffRuleId { get; set; }
        public decimal CoverageAmount { get; set; }
        public int CurrencyId { get; set; }
        public string? CoverageDescription { get; set; }

        //Relations
        public virtual TariffRule TariffRule { get; set; } = null!;
        public Currency Currency { get; set; }
    }
}