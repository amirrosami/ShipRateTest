using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentChargeType : BusinessEntity
    {
        public int ChargeTypeId { get; set; }
        public Guid? AgentId { get; set; }

        //Relations
        public Agent? Agent { get; set; }
        public ChargeType? ChargeType { get; set; }

    }
}

/*این جدول همه هزینه هایی که میتواند توسط ایجنت در دبیت نوت وارد شود را فیلتر میکند
 *شاید برخی از هزینه ها را صرفا برخی از ایجنت ها بتوانند بزنند 
 * 
 * 
 * 
 * 
 * 
 * 
 */
