using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class ExportDocsReceipt : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int ReceiptDirectionId { get; set; }

        public Guid PartyId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public Guid BookingId { get; set; }
        public string? PersonPersonName { get; set; } = string.Empty;  //نماینده مشتری
        public bool IsConfirmed { get; set; }
        public Guid? ConfirmUserId { get; set; }
        public DateTime ConfirmDate { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        //Relation
        public Company Company { get; set; }
        public Person Party { get; set; }
        public Booking Booking { get; set; }
        public ApplicationUser ConfirmUser { get; set; }
        public ReceiptDirection ReceiptDirection { get; set; }

        public ICollection<ExportDocsReceiptItem> Items { get; set; } = new List<ExportDocsReceiptItem>();
        public ICollection<ExportDocsReceiptContainer> Containers { get; set; } = new List<ExportDocsReceiptContainer>();

    }

    public class ExportDocsReceiptItem : BusinessEntity
    {
        public Guid ExportDocsReceiptId { get; set; }
        public Guid DocumentId { get; set; }

        //Relations
        public ExportDocsReceipt Receipt { get; set; }
        public Document Document { get; set; }
    }
    public class ExportDocsReceiptContainer : BusinessEntity
    {
        public Guid ExportDocsReceiptId { get; set; }
        public Guid? ContainerId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public DateTime? RBSDate { get; set; }
        //Relations
        public ExportDocsReceipt? Receipt { get; set; }
        public Container? Container { get; set; }
        public ContainerUse? ContainerUse { get; set; }
    }
    public class ReceiptDirection : SystemEntity
    {
        public string ReceiptDirectionName { get; set; } = string.Empty;
        public string ReceiptDirectionLocalName { get; set; } = string.Empty;
    }
}
