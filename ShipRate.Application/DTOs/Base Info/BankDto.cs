using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class BankDto

    {
        public Guid Id { get; set; }
        public int CountryId { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankLocalName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string SWIFTCode { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    public class BankCreateEditDto

    {
        public Guid? Id { get; set; }
        
        public string BankName { get; set; } = string.Empty;
        public string BankLocalName { get; set; } = string.Empty;
        public string BankCode { get; set; }
        [Range(1,double.MaxValue ,ErrorMessage ="select a Counrty")]
        public int CountryId { get; set; }
        public string SWIFTCode { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}

