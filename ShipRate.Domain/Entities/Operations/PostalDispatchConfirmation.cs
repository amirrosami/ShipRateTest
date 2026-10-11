using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Operations
{
    public class PostalDispatchConfirmation : BusinessEntity
    {
        public Guid PostalDispatchId { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string ConditionOnReceipt { get; set; }//Inract-Damaged
        public string Remarks { get; set; } = string.Empty;


        //Relations
        public PostalDispatch PostalDispatch { get; set; }
    }
}
