//GapGPT
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.enums;
using ShipRate.Domain.Enums;

namespace ShipRate.Domain.Entities.Rate
{
    public class TariffRate : BusinessEntity
    {
        public Guid TariffId { get; set; }

        // --- اسکوپ جغرافیایی و عملیاتی ---
        public Guid? OriginPortId { get; set; }
        public Guid? OriginTerminalId { get; set; }
        public Guid? OriginDepotId { get; set; }

        public Guid? DestinationPortId { get; set; }
        public Guid? DestinationTerminalId { get; set; }
        public Guid? DestinationDepotId { get; set; }

        // --- اسکوپ نوع بار و عملیات ---
        public Guid? ContainerTypeId { get; set; } // از یک جدول مرجع ContainerType

        //public CargoType CargoType { get; set; } // General, Dangerous, Reefer, ...
        public EnumTradeType TradeType { get; set; } // Import, Export, Transit

        // --- جزئیات نرخ ---
        public decimal Amount { get; set; }
        public EnumChargeUnit ChargeUnitId { get; set; } // Flat, PerContainer, PerKg, PerCbm, PerDay, ...
        public EnumBuySell Direction { get; set; } // Buy, Sell (مهم برای تفکیک هزینه و درآمد)

        //// حداقل/حداکثر برای نرخ‌های پلکانی (Tiered)
        //public decimal? MinimumCharge { get; set; }
        //public decimal? MaximumCharge { get; set; }

        //// برای نرخ‌های وابسته به وزن/حجم
        //public decimal? WeightBreakFrom { get; set; }
        //public decimal? WeightBreakTo { get; set; }
        //public decimal? VolumeBreakFrom { get; set; }
        //public decimal? VolumeBreakTo { get; set; }

        // شرایط خاص (مثلا فقط برای بارهای FCL معتبر است)
        public string Conditions { get; set; }


        //Relations
        public Tariff Tariff { get; set; }
        public Location OriginPort { get; set; }
        public Location OriginTerminal { get; set; }
        public Location OriginDepot { get; set; }
        public Location DestinationPort { get; set; }
        public Location DestinationTerminal { get; set; }
        public Location DestinationDepot { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
        public CalculationBy ChargeUnit { get; set; }
        public BuySellType BuySellType { get; set; }



    }
}