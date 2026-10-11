using Microsoft.AspNetCore.Identity;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.Security
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public string? Mobile { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SalesFilePrefix { get; set; }
        public string? JobTitle { get; set; }
        public byte[]? Pic { get; set; }
        public int? UserTypeId { get; set; }
        public Guid? PersonId { get; set; }
        public int? TwoFactorMethodId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? EditUserId { get; set; }

        public Guid? CreateUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
        public bool IsSystem { get; set; } = false;
        public Guid? DeleteUserId { get; set; }
        public DateTime? DeletedAt { get; set; }


        //Relations
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
        public UserType UserType { get; set; }
        public Person Person { get; set; }
        public TwoFactorMethod TwoFactorMethod { get; set; }
        public virtual ICollection<UserOtpCode> OtpCodes { get; set; }
              = new List<UserOtpCode>();
    }
    public class UserType : SystemEntity
    {
        public string UserTypeName { get; set; } = string.Empty;
        public string UserTypeLocalName { get; set; } = string.Empty;
        public string UserTypeDescription { get; set; } = string.Empty;
    }
    public class UserCompany : BusinessEntity
    {
        public Guid UserId { get; set; }
        public int CompanyId { get; set; }
        //Relations
        public ApplicationUser User { get; set; }
        public Company Company { get; set; }
    }
    public class TwoFactorMethod
    {
        public int Id { get; set; }
        public string TwoFactorMethodName { get; set; } = string.Empty;
    }
    public enum TwoFactorMethodEnum
    {
        None = 1,
        Sms = 2,
        GoogleAuthenticator = 3,
        Email = 4
    }

    public class UserOtpCode : BusinessEntity
    {
        public Guid UserId { get; set; }
        public string CodeHash { get; set; } = null!;
        public TwoFactorMethodEnum Method { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public bool IsUsed { get; set; }
        public int AttemptCount { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }





}


