using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Reports
{
    public class ReportCategory : SystemEntity
    {
        public int SortOrder { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryLocalName { get; set; } = null!;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ColorClass { get; set; }

        //Relations
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
    public class Report : SystemEntity
    {
        public int SortOrder { get; set; }
        public string ReportKey { get; set; } = null!;
        public string ReportName { get; set; } = null!;
        public string? ReportLocalName { get; set; } = null!;
        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public string ControllerName { get; set; } = null!;
        public string ActionName { get; set; } = null!;
        public string? FilterViewName { get; set; }
        public string? ResultViewName { get; set; }
        public string? IconClass { get; set; }

        //Relations
        public ReportCategory Category { get; set; } = null!;
        public ICollection<ReportCompanyRoleType> ReportCompanyRoleTypes { get; set; } = new List<ReportCompanyRoleType>();
    }

    public class ReportCompanyRoleType : SystemEntity
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int CompanyRoleTypeId { get; set; }

        //Relations
        public Report Report { get; set; } = null!;
        public CompanyRoleType CompanyRoleType { get; set; } = null!;
    }
}
