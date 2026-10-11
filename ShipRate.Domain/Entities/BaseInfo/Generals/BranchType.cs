using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class BranchType : SystemEntity
    {
        public string BranchTypeName { get; set; }
        public string BranchTypeLocalName { get; set; }
    }
}
