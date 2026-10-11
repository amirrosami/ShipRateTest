using ShipRate.Domain.Entities.Base;
namespace ShipRate.Domain.Entities.Operations
{
    public class PostalDispatchItem : BusinessEntity
    {
        public Guid PostalDispatchId { get; set; }
        public Guid? ObjectId { get; set; }
        public string ItemReference { get; set; } = string.Empty;
        public bool IsOriginal { get; set; }
        public string DocumentDescription { get; set; } = string.Empty;
        //Relations
        public PostalDispatch PostalDispatch { get; set; }
    }
}

/*ShipmentId اختیاری است
 * برای هر چی استفاده میشود
 * 
 * 
 * 
 * 
 */
