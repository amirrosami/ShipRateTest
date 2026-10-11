namespace ShipRate.WebApp.ViewModels
{
    public class CustomerViewModel
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }

    public class LocatonViewModel
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }

}
