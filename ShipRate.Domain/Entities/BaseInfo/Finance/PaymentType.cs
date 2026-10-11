using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class PaymentType : SystemEntity
    {
        public string PaymentTypeName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        //public Guid DefaultCityIdForPaymentAt { get; set; }
    }
}
