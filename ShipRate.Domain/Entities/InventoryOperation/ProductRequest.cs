using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class ProductRequest : BusinessEntity
    {
        public int RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public int CompanyId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public Guid? ApprovedByManagerId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? DeliveredByStoreKeeperId { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public int ProductRequestStatusId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public ProductRequestStatus ProductRequestStatus { get; set; }
        public ICollection<ProductRequestItem> Items { get; set; }
        public Company Company { get; set; }
        public ApplicationUser RequestedByUser { get; set; }
        public ApplicationUser? ApprovedByManager { get; set; }
        public ApplicationUser? DeliveredByStoreKeeper { get; set; }




    }
}
