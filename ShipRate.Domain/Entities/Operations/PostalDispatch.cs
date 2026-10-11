using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Operations
{
    public class PostalDispatch : BusinessEntity
    {
        public DateTime DispatchDate { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid? CourierCompanyId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public int CourierMethodId { get; set; }  // Courier, HandCarry, Post
        public int StatusId { get; set; }


        //Relations
        public CourierCompany? CourierCompany { get; set; }
        public CourierMethod CourierMethod { get; set; }
        public Person Sender { get; set; }
        public Person Receiver { get; set; }
        public PostalDispatchStatus PostalDispatchStatus { get; set; }
        public List<PostalDispatchItem> Items { get; set; } = new();
    }
}

/*
 * CourierCompanyId        BIGINT  -- DHL, FedEx, Post, Hand Carry
 *  * Status                  NVARCHAR(30)  -- Sent, InTransit, Delivered, Lost
 * 
 * 
 * 
 * 
 * 
 * 
 */
