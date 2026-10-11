namespace ShipRate.WebAppCustomers.ViewModels
{
    public class EmailListItem
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public DateTime SendDate { get; set; }
        public string Direction { get; set; } = "";
    }

}