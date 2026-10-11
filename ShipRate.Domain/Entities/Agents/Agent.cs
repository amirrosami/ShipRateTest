using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.Agents
{
    public class Agent : BusinessEntity
    {
        public int? CompanyId { get; set; }
        public Guid? ParentId { get; set; }  //برای احتیاط بیشتر گرفته شده است
        public bool IsActive { get; set; } = true;
        //Relations
        public Person? Person { get; set; }
        public Person? Parent { get; set; }
        public Company? Company { get; set; }

        public List<AgentActScope> AgentActScopes { get; set; }

    }
}
/*
 * ParentId ====>(میتواند ایجنت باشد یا اجزای داخلی شرکت)
 * 
 * 
 * 
 */