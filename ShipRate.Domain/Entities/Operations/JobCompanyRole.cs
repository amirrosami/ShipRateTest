using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobCompanyRole : BusinessEntity
    {
        public Guid JobId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyRoleTypeId { get; set; }
        public int? ServiceTypeId { get; set; }


        public Guid? BranchId { get; set; }

        //Relations
        public Job Job { get; set; }
        public Company Company { get; set; }
        public CompanyRoleType CompanyRoleType { get; set; }
        public ServiceType? ServiceType { get; set; }
        public Branch? Branch { get; set; }
    }
}

