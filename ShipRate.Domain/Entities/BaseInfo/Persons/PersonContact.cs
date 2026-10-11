using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class PersonContact : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        //Relation
        public Person Person { get; set; }
    }
}


/*  نکته : فقط یک نفر میتواند اصلی باشد
 * و مکاتبات ایمیلی به آدرس ایشان خواهد رفت 
 * 
 * 
 * 
 */