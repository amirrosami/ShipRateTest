using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.DocumentStore
{
    public class CorrespondenceTag : BusinessEntity
    {
        public Guid CorrespondenceId { get; set; }
        public Guid TagId { get; set; }
        //Relations
        public Correspondence Correspondence { get; set; }
        public Tag Tag { get; set; }
    }
}
/*برچسب مکاتبات
 * 
 * 
 * 
 * 
 */