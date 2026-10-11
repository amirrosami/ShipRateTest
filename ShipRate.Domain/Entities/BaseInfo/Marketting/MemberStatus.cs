using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class MemberStatus : SystemEntity
    {
        public string MemberStatusName { get; set; }
        public string MemberStatusLocalName { get; set; }
    }
}
