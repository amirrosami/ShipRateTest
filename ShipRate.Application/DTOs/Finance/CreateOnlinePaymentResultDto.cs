namespace ShipRate.Application.DTOs.Finance
{
    public class CreateOnlinePaymentResultDto
    {
        public string Token { get; set; }
        public string RedirectUrl { get; set; }
        public string ResNum { get; set; }
    }

   

}
