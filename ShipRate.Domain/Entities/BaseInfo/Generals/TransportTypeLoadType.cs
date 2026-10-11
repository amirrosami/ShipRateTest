using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class TransportTypeLoadType : SystemEntity
    {
        public int TransportTypeId { get; set; }
        public int LoadTypeId { get; set; }

        //Relations
        public TransportType TransportType { get; set; }
        public LoadType LoadType { get; set; }
    }
}
