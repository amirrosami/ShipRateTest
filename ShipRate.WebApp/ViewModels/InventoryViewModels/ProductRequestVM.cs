namespace ShipRate.WebApp.ViewModels.InventoryViewModels
{
    public class ProductRequestListVM
    {
        public Guid Id { get; set; }
        public int RequestNo { get; set; }
        public string RequestDate { get; set; } = "";
        public string RequestedBy { get; set; } = "";
        public string ApprovedBy { get; set; } = "";
        public string DeliveredBy { get; set; } = "";
        public string DeliveredAt { get; set; } = "";
        public int ItemCount { get; set; }
        public string Status { get; set; } = "";
        public int StatusId { get; set; }
        public string Remarks { get; set; } = "";
    }

    public class ProductRequestFormVM
    {
        public Guid Id { get; set; }
        public string Remarks { get; set; } = ""; public List<ProductRequestItemFormVM> Items { get; set; } = new();
    }

    public class ProductRequestItemFormVM
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
