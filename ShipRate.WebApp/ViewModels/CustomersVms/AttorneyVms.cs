using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.CustomersVms
{
    public class AttorneySearchViewModel
    {
        public string? ClientName { get; set; }
        public string? LawyerName { get; set; }
        public string? DocNo { get; set; }
        public int Page { get; set; } = 1;
    }


    public class AttorneyEditViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid ClientId { get; set; }

        [Required, MaxLength(30)]
        public string DocNo { get; set; } = "";

        [Required]
        public DateTime DocDate { get; set; } = DateTime.Now;

        public bool HasExpiryDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        // For display
        public string? ClientName { get; set; }
    }


    public class AttorneyIndexViewModel
    {
        public Guid Id { get; set; }
        public string DocNo { get; set; } = "";
        public DateTime DocDate { get; set; }
        public string ClientName { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public bool HasExpiryDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int LawyerCount { get; set; }
    }


    public class AttorneyLawyerViewModel
    {
        public Guid Id { get; set; }
        public Guid AttorneyId { get; set; }
        public Guid LawyerId { get; set; }
        public string LawyerName { get; set; } = "";
        public bool IsActive { get; set; }
    }

}
