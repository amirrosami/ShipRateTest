namespace ShipRate.WebAppCustomers.ViewModels
{
    public class DocumentListItem
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = "";
        public string DocumentTitle { get; set; } = "";
        public string DocumentNo { get; set; } = "";
        public DateTime DocumentDate { get; set; }
        public string? Description { get; set; }
        public int AttachmentCount { get; set; }
        public int EmailCount { get; set; }
        public string? CreateUserName { get; set; }
        public string? LastEditUserName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? LastEditDateTime { get; set; }
    }

}