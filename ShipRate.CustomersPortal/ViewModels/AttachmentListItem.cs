namespace ShipRate.WebAppCustomers.ViewModels
{
    public class AttachmentListItem
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = "";
        public string FileType { get; set; } = "";
        public string FileExtension { get; set; } = "";
        public decimal FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}