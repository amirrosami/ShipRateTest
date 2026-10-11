using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLOperation : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid BLId { get; set; }
        public int OperationTypeId { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Company Company { get; set; }
        public BL BL { get; set; }
        public OperationType OperationType { get; set; }
    }
}


/*
 * 
 * 
 * 
 * 

    DraftCreated
    DraftUpdated
    SubmittedForApproval
    Approved
    Issued
    Printed
    Reprinted
    Released
    TelexReleased
    SurrenderRequested
    SurrenderConfirmed
    Cancelled
    Amended
    Replaced
    Endorsed

*/