namespace ShipRate.WebAppCustomers.ViewModels
{
    public class DocumentManagerViewModel
    {
        public Guid ObjectId { get; set; }
        public int ObjectTypeId { get; set; }
        public List<DocumentListItem> Documents { get; set; } = new();
    }

}