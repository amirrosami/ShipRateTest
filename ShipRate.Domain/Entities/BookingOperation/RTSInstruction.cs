using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BookingSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class RTSInstruction : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid BookingId { get; set; }
        public Guid JobId { get; set; }

        public Guid? ShipmentOperationPlanId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? JobCompanyRoleId { get; set; }




        public Guid ShipperId { get; set; }
        public Guid CarrierId { get; set; }
        public string RTSInstructionNo { get; set; } = string.Empty;
        public DateTime? RTSInstructionDate { get; set; }
        public Guid DepotId { get; set; }
        public int StatusId { get; set; }

        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }

        public Guid? IssuedByUserId { get; set; }

        public DateTime? ApprovedAT { get; set; }   //اگر Approve داری
        public Guid? ApprovedByUserId { get; set; }

        public DateTime? CanceledAT { get; set; }
        public Guid? CanceledByUserId { get; set; }
        public string CancelationReason { get; set; } = string.Empty;



        public string? Remarks { get; set; }
        //Relations
        public Company? Company { get; set; }
        public Booking? Booking { get; set; }
        public Location? Depot { get; set; }
        public Person? Shipper { get; set; }
        public Carrier? Carrier { get; set; }
        public RTSInstructionStatus? Status { get; set; }
        public Job Job { get; set; }

        public ApplicationUser? CreateUser { get; set; }
        public ApplicationUser? UpdateUser { get; set; }

        public JobCompanyRole JobCompanyRole { get; set; }
        public Shipment Shipment { get; set; }
        public ShipmentOperationPlan ShipmentOperationPlan { get; set; }
        public List<RTSInstructionContainerType> RTSInstructionContainerTypes { get; set; }
    }
}