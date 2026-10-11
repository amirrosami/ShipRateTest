namespace ShipRate.Application.DTOs.Persons
{
    public class PersonDto
    {
        public Guid Id { get; set; }
        public int PersonalityTypeId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string PersonPersianName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }

        public int CountryId { get; set; }   //1=Iran


        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string NationalID { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string FaxNo { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string EcoCode { get; set; } = string.Empty;
        public string RegNo { get; set; } = string.Empty;
        public DateTime? PersonRegDate { get; set; }

        public PersonDto() { }
        public PersonDto(Guid id, int personalityTypeId, string personName, string personPersianName, string firstName, string lastName, DateTime? birthDate, int countryId, string emailAddress, string phone, string address, string nationalID, string mobile, string faxNo, string zipCode, string ecoCode, string regNo, DateTime? personRegDate)
        {
            Id = id;
            PersonalityTypeId = personalityTypeId;
            PersonName = personName;
            PersonPersianName = personPersianName;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            CountryId = countryId;
            EmailAddress = emailAddress;
            Phone = phone;
            Address = address;
            NationalID = nationalID;
            Mobile = mobile;
            FaxNo = faxNo;
            ZipCode = zipCode;
            EcoCode = ecoCode;
            RegNo = regNo;
            PersonRegDate = personRegDate;
        }
    }
}
