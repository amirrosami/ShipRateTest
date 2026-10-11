namespace ShipRate.WebApp.ViewModels.FinanceVms
{
    public class ExpenseCategoryVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal MaxAllowedAmount { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class ExpenseCategoryFormVM
    {
        public Guid? Id { get; set; }
        public int CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal MaxAllowedAmount { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
