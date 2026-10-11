using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Domain.enums;
using ShipRate.Domain.Enums;

namespace ShipRate.Domain.Entities.Rate
{
    public class Tariff : BusinessEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // اسکوپ اصلی: این نرخ‌ها متعلق به کدام شرکت در هلدینگ است؟
        public int CompanyId { get; set; } // شرکت مالک/استفاده‌کننده نرخ (مثلا شرکت فورواردر)
        public Company Company { get; set; }

        // نرخ‌ها مربوط به کدام طرف قرارداد خارجی است؟ (اختیاری)
        public Guid? CounterpartyCompanyId { get; set; } // مثلا خط کشتیرانی X
        public Person CounterpartyCompany { get; set; }

        public int TariffTypeId { get; set; }
        public TariffType TariffType { get; set; } // Freight, THC, Documentation, Insurance, ...

        public int TransportTypeId { get; set; }
        public EnumTransportType TransportType { get; set; } // Sea, Air, Road, Rail, Multimodal


        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public int TariffStatusId { get; set; }
        public EnumTariffStatus Status { get; set; } // Draft, Active, Expired, Archived

        // ارز پایه برای تمام نرخ‌های این تعرفه
        public int CurrencyId { get; set; } // USD, EUR, ...

        // مجموعه خطوط نرخ این تعرفه
        public ICollection<TariffRate> Rates { get; set; } = new List<TariffRate>();
    }
}
