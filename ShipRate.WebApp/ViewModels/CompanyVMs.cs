using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels
{
    public class CompanyListViewModel
    {
        public int Id { get; set; }
        public string PersonName { get; set; } // از جدول Persons
        public string AccountingAddress { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public string CompanyAbb { get; set; }
    }


    public class CompanySettingsViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<SettingItemViewModel> Settings { get; set; } = new();
    }

    public class SettingItemViewModel
    {
        public int? Id { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
    }


    public class NumberingSystemViewModel
    {
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        [Required, MaxLength(100)]
        public string EntityType { get; set; }

        //[Required, MaxLength(20)]
        public string Prefix { get; set; }

        //[Required, MaxLength(20)]
        public string? Suffix { get; set; }

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
