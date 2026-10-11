//تکمیل شود
using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLFollowUp : BusinessEntity
    {
        public Guid BLId { get; set; }
        public string FollowUpQuestion { get; set; }
        public string? FollowUpAnswer { get; set; }

        //Relation
        public BL BL { get; set; }
    }
}
