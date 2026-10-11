using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class ShippingInstruction : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid JobId { get; set; }
        public Guid BookingId { get; set; }
        public string SINumber { get; set; }
        public Guid CustomerId { get; set; }
        public string ShipperText { get; set; }
        public string ConsigneeText { get; set; }
        public string NotifypartyText { get; set; }
        public string GoodsDescription { get; set; }
        public string MarksAndNumbers { get; set; }
        public string SpecialInstructions { get; set; }
        public int FreightPaymentTypeId { get; set; }

        public DateTime? ReceivedDate { get; set; }
        public DateTime? ProcessedDate { get; set; }

        //Relations
        public Company Company { get; set; }
        public Guid Job { get; set; }
        public Guid Booking { get; set; }
        public Customer Customer { get; set; }
    }
}


/*به نظرم خیلی ناقص است . باید درست شود 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */