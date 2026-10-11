namespace ShipRate.WebApp.ViewModels.AgentVms
{
    public class AgentListViewModel
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ScopeNames { get; set; } = new();

    }

    public class AgentSearchViewModel
    {
        public string? PersonName { get; set; }
        public string? Status { get; set; }        // "active" / "inactive" / null(all)

        // ── Pagination ──
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AgentEditViewModel
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = "";
        public bool IsActive { get; set; }

        // ── Trade Types (multi-select) ──
        public List<int> SelectedScopeIds { get; set; } = new();

        // ── Dropdowns ──
        public List<ScopeDropdownItem> Scopes { get; set; } = new();
    }

    public class ScopeDropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

}
