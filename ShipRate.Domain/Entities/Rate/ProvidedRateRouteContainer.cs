using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteContainer : BusinessEntity
    {
        public Guid ProvidedRateRouteId { get; set; }
        public int? ContainerSizeTypeId { get; set; }
        //Relations
        public ProvidedRateRoute ProvidedRateRoute { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
    }
}