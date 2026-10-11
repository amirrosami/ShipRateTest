namespace ShipRate.WebApp.ViewModels
{
    public class ServiceFormViewModel
    {

        public Guid? Id { get; set; }
        public Guid? ShippingLineId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

}
