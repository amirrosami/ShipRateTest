using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Rate
{
    public class Rate : BusinessEntity
    {
        public int CompanyId { get; set; }   //مالک دیتا
        public int TransportTypeId { get; set; }
        public int LoadTypeId { get; set; }
        public string RateTitle { get; set; }

        //Relations 
        public Company Company { get; set; }
        public TransportType TransportType { get; set; }
        public LoadType LoadType { get; set; }

        //public List<RateRoute>? RateRoutes { get; set; }
        //public List<RatePort>? RatePorts { get; set; }
    }
}