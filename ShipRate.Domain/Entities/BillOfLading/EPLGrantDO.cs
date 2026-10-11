using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class EPLGrantDO : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid BLId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime GrantDate { get; set; }
        public string BarFarabaranCode { get; set; } //کد رهگیری

        //Relations
        public Company Company { get; set; }
        public BL BL { get; set; }
        public Customer Customer { get; set; }
    }
}


/*Vagozaree DO Dar Samaneh EPL Gomrok 
 * BarFarabaran Ham Hast 
 * 
 * 
 * 
 */
