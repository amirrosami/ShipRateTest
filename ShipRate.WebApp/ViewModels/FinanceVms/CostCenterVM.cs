namespace ShipRate.WebApp.ViewModels.FinanceVms
{
    public class CostCenterVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class CostCenterFormVM
    {
        public Guid? Id { get; set; }
        public int CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
