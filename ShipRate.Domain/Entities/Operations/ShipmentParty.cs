using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Enums;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentParty : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public EnumRoleType RoleTypeId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? TradePartyId { get; set; }

        //Relations
        public Shipment Shipment { get; set; }
        public RoleType RoleType { get; set; }
        public Customer Customer { get; set; }
    }
}
/*خیلی جای بحث دارد 
 * اگر به Trade وصل شود نیازی نیست 
 * یا اینکه 
 * TradePartyId بیشتر بررسی شود 
 * Agent 
 * 
 */