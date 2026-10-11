using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Marketing
{
    public class LostReasonCategory : SystemEntity
    {
        public string LostReasonCategoryName { get; set; } = string.Empty;
        public string LostReasonCategoryLocalName { get; set; } = string.Empty;
        public Guid? CreateUserId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public Guid? EditUserId { get; set; }
        public DateTime? UpdateDatetime { get; set; }
    }
}



//| LostReasonId | Title | Category |
//| ------------ | ------------------- | ----------- |
//| 1 | Rate too high       | Pricing     |
//| 2            | Transit time long   | Service     |
//| 3            | Competitor chosen   | Competition |
//| 4            | No response         | Customer    |
//| 5            | Documentation issue | Operation   |
