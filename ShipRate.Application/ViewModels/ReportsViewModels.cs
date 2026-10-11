namespace ShipRate.Application.ViewModels
{
    public class ReportItemViewModel
    {
        public int Id { get; set; }
        public string ReportKey { get; set; } = null!;
        public string ReportName { get; set; } = null!;
        public string ReportLocalName { get; set; } = null!;
        public string? Description { get; set; }

        public string ControllerName { get; set; } = null!;
        public string ActionName { get; set; } = null!;

        public string? FilterViewName { get; set; }
        public string? ResultViewName { get; set; }

        public string? IconClass { get; set; }

        public int CategoryId { get; set; }
        public int SortOrder { get; set; }
    }

    public class ReportCategoryCardViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryLocalName { get; set; } = null!;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ColorClass { get; set; }
        public int SortOrder { get; set; }

        public List<ReportItemViewModel> Reports { get; set; } = new();
    }

    public class ReportsIndexViewModel
    {
        public int? SelectedCompanyId { get; set; }
        public string? SelectedCompanyName { get; set; }

        public List<ReportCategoryCardViewModel> Categories { get; set; } = new();
    }
}
