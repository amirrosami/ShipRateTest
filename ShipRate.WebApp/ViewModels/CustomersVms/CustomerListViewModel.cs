namespace ShipRate.WebApp.ViewModels.CustomersVms
{
    public class CustomerListViewModel
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = "";
        public string? LocalName { get; set; } = "";
        public string? IndustryName { get; set; }
        public string? SalesUserName { get; set; }
        public Guid? SalesUserId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public List<string> TradeTypeNames { get; set; } = new();
    }

    public class CustomerSearchViewModel
    {
        public string? PersonName { get; set; }
        public string? Status { get; set; }        // "active" / "inactive" / null(all)
        public Guid? IndustryId { get; set; }
        public Guid? SalesUserId { get; set; }

        // ── Pagination ──
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CustomerEditViewModel
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = "";
        public Guid? IndustryId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        // ── Trade Types (multi-select) ──
        public List<int> SelectedTradeTypeIds { get; set; } = new();
        public List<TradeTypeDropdownItem> TradeTypes { get; set; } = new();

        // ── Dropdowns ──
        public List<IndustryDropdownItem> Industries { get; set; } = new();
    }

    public class IndustryDropdownItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class TradeTypeDropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class AssignSalesUserViewModel
    {
        public Guid CustomerId { get; set; }
        public string PersonName { get; set; } = "";
        public Guid? CurrentSalesUserId { get; set; }
        public string? CurrentSalesUserName { get; set; }
        public Guid? SelectedSalesUserId { get; set; }

        public List<SalesUserDropdownItem> SalesUsers { get; set; } = new();
    }

    public class SalesUserDropdownItem
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
    }


}
