namespace ShipRate.WebApp.ViewModels
{
    public class RateRequestContainerViewModelOld
    {
        public int ContainerSizeTypeId { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal Counts { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
