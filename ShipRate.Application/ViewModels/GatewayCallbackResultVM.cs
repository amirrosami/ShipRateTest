namespace ShipRate.Application.ViewModels
{
    public class GatewayCallbackResultVM
    {
        public bool IsSuccess { get; set; }

        public string? RefNumber { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public decimal Amount { get; set; }

    }

}
