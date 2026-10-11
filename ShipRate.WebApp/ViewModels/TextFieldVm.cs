using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.WebApp.ViewModels
{
    public class TextFieldVm
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool Required { get; set; }
    }


    public class SelectFieldVm
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string Selected { get; set; }

        public IEnumerable<SelectListItem> Options { get; set; }

        public bool Required { get; set; }
    }



    public class GridFilterViewModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }

        public string ListUrl { get; set; }

        public string? CustomFilterPartial { get; set; }

        public Dictionary<string, object> CustomFilterValuesDict { get; set; } = new Dictionary<string, object>();
        public object CustomFilterValues { get; set; }

    }

}
