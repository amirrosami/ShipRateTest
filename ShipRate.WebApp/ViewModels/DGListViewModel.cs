namespace ShipRate.WebApp.ViewModels
{
    public class DGListViewModel
    {
        public List<DGListItem> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public int? DGClassId { get; set; }
        public List<DGClassOption> DGClasses { get; set; } = new();
    }

    public class DGListItem
    {
        public Guid Id { get; set; }
        public string DGClassNo { get; set; } = "";
        public string DGClassName { get; set; } = "";
        public string DGSubClassNo { get; set; } = "";
        public string DGUnNumber { get; set; } = "";
        public string DGName { get; set; } = "";
        public string DGLocalName { get; set; } = "";
        public string DGPackingGroupCode { get; set; } = "";
        public string DGRegulationSetName { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class DGClassOption
    {
        public int Id { get; set; }
        public string Display { get; set; } = "";
    }


    public class DGFormViewModel
    {
        public Guid? Id { get; set; }
        public int DGClassId { get; set; }
        public string DGSubClassNo { get; set; } = "";
        public string DGUnNumber { get; set; } = "";
        public string DGName { get; set; } = "";
        public string DGLocalName { get; set; } = "";
        public int DGPackingGroupId { get; set; }
        public int DGRegulationSetId { get; set; }
        public bool IsActive { get; set; } = true;

        public List<DGClassOption> DGClasses { get; set; } = new();
        public List<DGPackingGroupOption> PackingGroups { get; set; } = new();
        public List<DGRegulationSetOption> RegulationSets { get; set; } = new();
    }

    public class DGPackingGroupOption
    {
        public int Id { get; set; }
        public string Display { get; set; } = "";
    }

    public class DGRegulationSetOption
    {
        public int Id { get; set; }
        public string Display { get; set; } = "";
    }
}
