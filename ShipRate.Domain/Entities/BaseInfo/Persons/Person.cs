using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Finance;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class Person : BusinessEntity
    {
        public int PersonalityTypeId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string PersonPersianName { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }

        public int CountryId { get; set; }   //1=Iran
        public Guid? CityId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? PersianAddress { get; set; } = "-";

        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string FaxNo { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? WebSite { get; set; } = string.Empty;

        public string NationalID { get; set; } = string.Empty;
        public string EcoCode { get; set; } = string.Empty;
        public string RegNo { get; set; } = string.Empty;
        public DateTime? PersonRegDate { get; set; }

        //Relations 
        public Country? Country { get; set; }
        public PersonalityType? PersonalityType { get; set; }
        public Location City { get; set; }

        public List<PersonContact> PersonContacts { get; set; } = new();
        public List<PersonExtraInfo> PersonExtraInfos { get; set; } = new();
        public List<BankAccount> BankAccounts { get; set; } = new();
        public List<PersonAddress> Addresses { get; set; } = new();
        public List<Branch> Branches { get; set; } = new();

    }
}
