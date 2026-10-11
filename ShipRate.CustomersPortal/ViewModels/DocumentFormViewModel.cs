namespace ShipRate.WebAppCustomers.ViewModels
{
    public class DocumentFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid ObjectId { get; set; }
        public int ObjectTypeId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; } = "";
        public string DocumentNo { get; set; } = "";
        public DateTime DocumentDate { get; set; } = DateTime.Now;
        public string? DocumentDescription { get; set; }
        public List<LookupItem> DocumentTypes { get; set; } = new();
    }

}