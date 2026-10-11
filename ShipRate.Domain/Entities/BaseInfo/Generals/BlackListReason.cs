using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class BlackListReason : SystemEntity
    {
        public string BlackListReasonName { get; set; } = string.Empty;
        public bool SystemDefined { get; set; }
        //Relations
        //public List<BlackList>? BlackLists { get; set; }
    }
}
