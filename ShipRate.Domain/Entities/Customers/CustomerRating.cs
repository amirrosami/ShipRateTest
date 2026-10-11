using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerRating : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime RatingDate { get; set; }
        public decimal RatingScore { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Customer Customer { get; set; }
        public Company Company { get; set; }
    }
}
