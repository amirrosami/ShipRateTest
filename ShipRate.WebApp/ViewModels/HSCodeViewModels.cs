namespace ShipRate.WebApp.ViewModels
{
    public class HSCodeIndexViewModel
    {
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        public List<HSCodeListItem> HSCodes { get; set; }
    }

    public class HSCodeListItem
    {
        public Guid Id { get; set; }
        public string HSCodeCode { get; set; }
        public string HSCodeName { get; set; }
        public string HSLevelName { get; set; }
        public string ParentCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class HSCodeFormViewModel
    {
        public Guid? Id { get; set; }
        public string HSCodeCode { get; set; }
        public string HSCodeName { get; set; }
        public int HSLevelId { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }

        public List<HSLevelOption> HSLevels { get; set; } = new List<HSLevelOption>();
        public List<HSCodeOption> ParentHSCodes { get; set; } = new List<HSCodeOption>();
    }

    public class HSLevelOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HSCodeOption
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class CountryHSCodeListItem
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; }
        public string CHSCode { get; set; }
        public string CHSName { get; set; }
        public string LocalName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? DutyRate { get; set; }
        public decimal? CommercialBenefitRate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CountryHSCodeFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid HSCodeId { get; set; }
        public int CountryId { get; set; }
        public string CHSCode { get; set; }
        public string CHSName { get; set; }
        public string LocalName { get; set; }
        public DateTime? CountryHSCodeValidFrom { get; set; }
        public DateTime CountryHSCodeValidTo { get; set; }
        public decimal? DutyRate { get; set; }
        public decimal? CommercialBenefitRate { get; set; }
        public bool IsActive { get; set; }
        public string? HSCodeName { get; set; }

        public List<CountryOption> Countries { get; set; } = new List<CountryOption>();
    }

    public class CountryOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
