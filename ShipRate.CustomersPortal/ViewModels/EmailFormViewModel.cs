namespace ShipRate.WebAppCustomers.ViewModels
{
    public class EmailFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid DocumentId { get; set; }
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public string To { get; set; } = "";
        public string CC { get; set; } = "";
        public string BCC { get; set; } = "";
        public int EmailDirectionId { get; set; }
        public List<LookupItem> EmailDirections { get; set; } = new();
    }

}