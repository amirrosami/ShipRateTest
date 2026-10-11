using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.CustomersVms
{
    public class CustomerConfirmationListViewModel
    {
        public List<CustomerConfirmationListItem> Items { get; set; } = new();
        public string? SearchName { get; set; }
        public string? SearchStatus { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class CustomerConfirmationListItem
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string ConfirmRemarks { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ConfirmUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ConfirmDate { get; set; }
    }

    // --- Create ---
    public class CustomerConfirmationCreateViewModel
    {
        [Required(ErrorMessage = "Person is required.")]
        public Guid? PersonId { get; set; }

        // For display in search input
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Remarks is required.")]
        [MaxLength(200, ErrorMessage = "Maximum 200 characters.")]
        public string Remarks { get; set; } = string.Empty;
    }

    // --- Edit ---
    public class CustomerConfirmationEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Person is required.")]
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Remarks is required.")]
        [MaxLength(200)]
        public string Remarks { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }

    // --- Confirm / Reject ---
    public class CustomerConfirmationActionViewModel
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Maximum 200 characters.")]
        public string ConfirmRemarks { get; set; } = string.Empty;

        /// <summary>
        /// "Confirmed" or "Rejected"
        /// </summary>
        [Required]
        public string Action { get; set; } = string.Empty;
    }

    // --- Person Search Result ---
    public class PersonSearchResult
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
