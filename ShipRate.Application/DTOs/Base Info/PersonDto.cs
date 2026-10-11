using ShipRate.Domain.Entities.BaseInfo.Persons;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class BranchDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }

        public int CountryId { get; set; }
        public Guid? LocationId { get; set; }
        public int BranchTypeId { get; set; }

        public string? PersonName { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchTypeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }





    public class PersonContactDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string PersonName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class PersonDto
    {
        public Guid Id { get; set; }
        public int PersonalityTypeId { get; set; }
        public string PersonalityTypeName { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public string PersonPersianName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }

        public int CountryId { get; set; }   //1=Iran
        public Guid? CityId { get; set; }

        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PersianAddress { get; set; } = string.Empty;
        public string NationalID { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string FaxNo { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string EcoCode { get; set; } = string.Empty;
        public string RegNo { get; set; } = string.Empty;
        public DateTime? PersonRegDate { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public bool HasLatinLogo { get; set; } = false;
        public bool HasLocalLogo { get; set; } = false;

        public bool IsCarrier { get; set; } = false;
        public bool IsCustomer { get; set; } = false;
        public bool IsDriver { get; set; } = false;
        public bool IsUser { get; set; } = false;
        public bool IsAgent { get; set; } = false;
        public bool IsCourierCompany { get; set; } = false;
        public bool IsGPSProvider { get; set; } = false;
        public Guid? lastPictureId { get; set; }
        public List<RoleType> RoleTypes { get; set; }
        public List<PersonContactDto>? ContactsList { get; set; }
        public List<PersonExtraInfoDto>? PersonExtraInfoList { get; set; }
        public List<BankAccountDto>? BankAccountsList { get; set; }
        public AgentDto Agent { get; set; }
        public GPSProviderDto GPSProviderDto { get; set; }
        public DriverDto Driver { get; set; }
        public CourierCompanyDto CourierCompany { get; set; }
        public CarrierDto Carrier { get; set; }
    }


    public class PersonCreateUpdateDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Personality Type is required.")]
        public int PersonalityTypeId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string PersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Person Local Name is required")]
        public string PersonPersianName { get; set; } = string.Empty;

        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }   //1=Iran
        public Guid? CityId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]

        public string EmailAddress { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone is required.")]

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;

        public string PersianAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nationa ID is required.")]
        public string NationalID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile is required.")]
        public string Mobile { get; set; } = string.Empty;

        public string? FaxNo { get; set; } = string.Empty;
        public string? ZipCode { get; set; } = string.Empty;
        public string? EcoCode { get; set; } = string.Empty;

        public string RegNo { get; set; } = string.Empty;
        public DateTime? PersonRegDate { get; set; } = DateTime.Now;
        public bool IsAgent { get; set; } = false;
        public bool IsCarrier { get; set; } = false;
        public bool IsCourierCompany { get; set; } = false;
        public bool IsDriver { get; set; } = false;
        public bool IsGPSProvider { get; set; }
        public AgentDto? Agent { get; set; } = new();
        public CarrierDto? Carrier { get; set; } = new();
        public DriverDto? Driver { get; set; } = new();
        public GPSProviderDto GPSProviderDto { get; set; } = new();
        public CourierCompanyDto CourierCompany { get; set; } = new();
        public List<int> SelectedRoleTypes { get; set; } = new();
        public List<RoleType> RoleTypes { get; set; }
    }


    public class PersonContactCreateUpdateDto
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
    public class CarrierDto
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }
        public int? CarrierTypeId { get; set; }
        public string? CarrierCode { get; set; } = string.Empty;
        public string? AWBPrefix { get; set; } = string.Empty;
        public string? BookingEmail { get; set; } = string.Empty;
        public string? OperatingRegion { get; set; } = string.Empty;
        public string? TrackingUrl { get; set; } = string.Empty;
        public bool IsVesselOwner { get; set; }
    }

    public class AgentDto
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }
        public int CompanyId { get; set; }
        public Guid? ParentId { get; set; }
    }

    public class CustomerDto
    {
        public Guid? Id { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string PersonLocalName { get; set; } = string.Empty;
        public Guid PersonId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SalePersonId { get; set; }
        public Guid? SaleUserId { get; set; }
    }

    public class GPSProviderDto
    {
        public Guid Id { get; set; }
        public string SiteAddress { get; set; }
        public string Remarks { get; set; }
        public Guid PersonId { get; set; }

    }

    public class CourierCompanyDto
    {
        public string Remarks { get; set; }
        public Guid PersonId { get; set; }
        public Guid Id { get; set; }
    }

    public class DriverDto
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }
        public Guid? CarrierId { get; set; }
        public string? PassportNo { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; } = string.Empty;
        public string? SmartCardNo { get; set; } = string.Empty;
    }
    public class PersonExtraInfoDto
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }
        public int? ExtraInfoTypeId { get; set; }
        public string ExtraInfoTypeName { get; set; } = string.Empty;
        public string PersonExtraInfoValue { get; set; } = string.Empty;
    }

    public class BankAccountDto
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public string BankAccountTitle { get; set; } = string.Empty;
        public string BankAccountTitlePersian { get; set; } = string.Empty;

        public Guid BankId { get; set; }
        public string BankName { get; set; } = string.Empty;

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BranchNo { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        public int BankAccountPurposeId { get; set; }
        public string BankAccountPurposeName { get; set; } = string.Empty;
    }
}
