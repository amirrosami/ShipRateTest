using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.CompaniesViewModels
{
    public class CompanySelectorViewModel
    {
        public int? CurrentCompanyId { get; set; }
        public List<CompanyItemViewModel> Companies { get; set; } = new();
    }

    public class CompanyItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }







    public class CompanyNumberingListViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<NumberingSystemItemViewModel> Items { get; set; } = new();
    }

    public class NumberingSystemItemViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string EntityType { get; set; } // غیرقابل ویرایش (فقط نمایشی)

        [Required, MaxLength(20)]
        public string Prefix { get; set; }

        [Required, MaxLength(20)]
        public string Suffix { get; set; }

        [Required, Range(1, 20)]
        public int PadLength { get; set; }

        public bool ResetYearly { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int CurrentSeq { get; set; }

        [Required]
        public int CurrentYear { get; set; }

        public bool IsActive { get; set; }
    }






}
