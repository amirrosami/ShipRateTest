using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Customers
{
    public class Attorney : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ClientId { get; set; }   //Movakkel
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public bool HasExpiryDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        //Relations
        public Customer Client { get; set; }
        public Company Company { get; set; }
        public List<AttorneyLawyer> AttorneyLawyers { get; set; }
    }
}
