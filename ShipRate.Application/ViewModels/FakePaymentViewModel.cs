namespace ShipRate.Application.ViewModels
{
    public class FakePaymentViewModel
    {
        public string Token { get; set; }
        public decimal Amount { get; set; }
        public string ResNum { get; set; }
        public string State { get; set; } = "OK";
    }

}
