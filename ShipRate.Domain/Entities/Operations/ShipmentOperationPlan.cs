using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentOperationPlan : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public int OperationTypeId { get; set; }
        public Guid JobCompanyRoleId { get; set; }
        public int SequenceNo { get; set; }

        public bool IsApplicable { get; set; }
        public bool IsRequired { get; set; }
        public bool IsInOurScope { get; set; }
        public bool IsExternallyPerformed { get; set; }
        public bool IsTrackingRequired { get; set; }


        //public int? TrackingModeId { get; set; }   
        public Guid? ResponsibilityPartyId { get; set; }   //همه را بشه ثبت کرد نه فقط شرکتهای خودمون
        public Guid? PlannedLocationId { get; set; }


        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public int StatusId { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public Shipment Shipment { get; set; }
        public OperationType OperationType { get; set; }
        public JobCompanyRole JobCompanyRole { get; set; }
        public Person ResponsibilityParty { get; set; }
        public Location PlannedLocation { get; set; }
        public PlanStatus PlanStatus { get; set; }
    }
}


/*
 * 
 * 
 * 
 * 
 * Tracking Mode:
 * 1 = FullExecution
   2 = MilestoneOnly
   3 = ExternalEventOnly
   4 = NotTracked
 * 
 * 
 * 
 * 
 * 
 * 
 * 
معنی دقیق فیلدهای کلیدی
IsApplicable

مهم‌تر از چیزی است که به نظر می‌رسد.

یعنی این عملیات اساساً برای این Shipment مطرح هست یا نه.

مثلاً:

    در Air Shipment، STUFFING ممکن است IsApplicable = 0
    در Sea FCL، RTS = 1

IsRequired

اگر عملیات applicable است، آیا انجامش برای تکمیل فرایند لازم است؟

مثلاً:

    CARGO_RECEIVING در FCL Factory Stuffing ممکن است:
        IsApplicable = 0
    STUFFING:
        IsApplicable = 1
        IsRequired = 1
        ولی شاید internal نباشد

IsInOurScope

آیا این عملیات جزو حوزه مسئولیت شرکت ماست؟

مثلاً:

    Stuffing در کارخانه shipper:
        IsApplicable = 1
        IsRequired = 1
        IsInOurScope = 0

IsExternallyPerformed

برای وقتی که عملیات وجود دارد ولی اجراکننده طرف ثالث است.

مثلاً:

    Cargo receiving توسط CFS partner
    Stuffing توسط shipper
    Gate-in توسط trucking/terminal

IsTrackingRequired

حتی اگر عملیات در scope شما نباشد، ممکن است لازم باشد tracking شود.

مثلاً:

    Load on vessel
    Gate in full
    Stuffing at shipper site

TrackingModeId

پیشنهاد enum:

                                                                    text
1 = FullExecution
2 = MilestoneOnly
3 = ExternalEventOnly
4 = NotTracked

مثال:

    Cargo receiving در انبار خودتان → FullExecution
    Stuffing در کارخانه shipper → MilestoneOnly
    Load on vessel از line event feed → ExternalEventOnly
    عملیاتی که اصلاً برای شما مهم نیست → NotTracked * 
 * 
 */

