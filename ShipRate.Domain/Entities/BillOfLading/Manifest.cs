//GapGPT
//هنوز کار دارد . برای کشتی و کامیون  و ....هواپیما
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Enums;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class Manifest : BusinessEntity
    {
        public int? VoyageId { get; set; }
        public Guid Id { get; set; }
        public string ManifestNumber { get; set; } // شماره یکتای مانیفست
        public EnumManifestType ManifestTypeTypeId { get; set; } // نوع مانیفست (مقدار شمارشی)
        public DateTime IssueDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public Guid OriginPortId { get; private set; }
        public Guid DestinationPortId { get; private set; }
        //public ManifestStatus Status { get; private set; }

        //// لیست کانتینرها/بارهایی که در این مانیفست ثبت شده‌اند
        //private readonly List<ManifestItem> _items = new();
        //public IReadOnlyCollection<ManifestItem> Items => _items.AsReadOnly();

        //// لیست اسناد مرتبط (مثلاً بارنامه‌های انفرادی)
        //private readonly List<ManifestDocument> _documents = new();
        //public IReadOnlyCollection<ManifestDocument> Documents => _documents.AsReadOnly();

    }
}
