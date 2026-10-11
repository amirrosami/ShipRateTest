using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class ChargeType : SystemEntity
    {
        public string? ChargeTypeCode { get; set; } = string.Empty;
        public string? ChargeTypeCode2 { get; set; } = string.Empty;
        public string ChargeTypeName { get; set; } = string.Empty;
        public string ChargeTypeLocalName { get; set; } = string.Empty;
        public int ChargeTypeCategoryId { get; set; }


        // Financial Behavior
        public bool? IsTaxable { get; set; }
        public bool? AllowManualAmount { get; set; }
        public bool? AllowRateBased { get; set; }


        // Currency & Unit
        public int? CurrencyId { get; set; }
        public int? CalculationById { get; set; }
        public int? CalculationTypeId { get; set; }

        // Accounting / Reporting
        public bool? AffectProfit { get; set; }

        // وابستگی به Charge دیگر (پایه)
        public int? DependsOnChargeTypeId { get; set; }
        // در صورت DependencyType == Multiplier، این فیلد مقدار ضریب را نگه می‌دارد (مثلاً 0.15 برای ۱۵٪)
        public decimal? MultiplierRate { get; set; }

        public bool SystemDefined { get; set; }

        public DateTime? CreatedAt { get; set; }
        public Guid? CreateUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? EditUserId { get; set; }

        public string? Remarks { get; set; } = string.Empty;

        //Relations
        public ChargeTypeCategory? ChargeTypeCategory { get; set; }
        public CalculationBy CalculationBy { get; set; }
        public CalculationType CalculationType { get; set; }
        public ChargeType DependsOnChargeType { get; set; }
        public Currency? Currency { get; set; }
    }
}
