using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Marketing
{
    public class LostReason : SystemEntity
    {
        public int LostReasonCategoryId { get; set; }
        public string LostReasonName { get; set; } = string.Empty;
        public string LostReasonLocalName { get; set; } = string.Empty;

        public Guid CreateUserId { get; set; }
        public DateTime CreateDatetime { get; set; }
        public Guid? EditUserId { get; set; }
        public DateTime UpdateDatetime { get; set; }

        //Relations
        public LostReasonCategory LostReasonCategory { get; set; }
    }
}


//| LostReasonId | Title | Category |
//| ------------ | ------------------- | ----------- |
//| 1 | Rate too high       | Pricing     |
//| 2            | Transit time long   | Service     |
//| 3            | Competitor chosen   | Competition |
//| 4            | No response         | Customer    |
//| 5            | Documentation issue | Operation   |
