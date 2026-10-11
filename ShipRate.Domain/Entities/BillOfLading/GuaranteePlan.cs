using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.BillOfLading
{
    public class GuaranteePlan : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string LetterNo { get; set; }
        public DateTime LetterDate { get; set; }
        public int InOutId { get; set; }
        public Guid? ShippingLineId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid BLId { get; set; }
        public string BLNo { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Company Company { get; set; }
        public BL BL { get; set; }
        public InOut InOut { get; set; }
        public Carrier ShippingLine { get; set; }
        public Person Customer { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser UpdateUser { get; set; }
    }
}
