using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Settings
{
    public class Company : SystemEntity
    {
        public Guid PersonId { get; set; }
        public int? ParentId { get; set; }
        public string AccountingAddress { get; set; }
        public string CompanyAbb { get; set; }
        //Relations
        public Person Person { get; set; }
    }
}
/*کلیه تنظمیات کمپانی در جا سیو میشود
 * 
 * 
 * 
 */
