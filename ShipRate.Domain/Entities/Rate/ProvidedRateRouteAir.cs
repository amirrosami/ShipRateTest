using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteAir : BusinessEntity
    {
        public Guid ProvidedRateRouteId { get; set; }
        public int CurrencyId { get; set; }
        public int VolumeCoefficient { get; set; }
        public string Remarks { get; set; }

        //Relations 
        public ProvidedRateRoute? ProvidedRateRoute { get; set; }
        public Currency? Currency { get; set; }
    }
}
