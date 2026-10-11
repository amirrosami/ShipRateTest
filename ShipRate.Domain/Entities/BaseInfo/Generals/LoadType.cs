using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class LoadType : SystemEntity
    {
        public string LoadTypeCode { get; set; }
        public string LoadTypeDescription { get; set; }

        //Relations
        public List<TransportTypeLoadType> TransportTypeLoadTypes { get; set; }


    }
}
