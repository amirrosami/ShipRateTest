using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class TIRUsage : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid TIRAssignmentId { get; set; }
        public Guid? BorderId { get; set; }
        public Guid? CustomsOfficeId { get; set; }
        public int InOutId { get; set; }
        public string VoletNo { get; set; } //شماره ووله مصرفی
        public DateTime TIRUsageUsedAt { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public TIRAssignment TIRAssignment { get; set; }
        public Location Border { get; set; }
        public Location CustomsOffice { get; set; }
        public InOut InOut { get; set; }
        public Company Company { get; set; }
    }
}

