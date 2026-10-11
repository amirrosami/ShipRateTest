using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class CountryHSCode : BusinessEntity
    {
        public int CountryId { get; set; }
        public Guid HSCodeId { get; set; }
        [MinLength(8), MaxLength(12)]
        public string CHSCode { get; set; } = string.Empty;
        public string CHSName { get; set; } = string.Empty;
        public string LocalName { get; set; } = string.Empty;
        public DateTime? CountryHSCodeValidFrom { get; set; }
        public DateTime CountryHSCodeValidTo { get; set; }
        public decimal? DutyRate { get; set; }
        public decimal? CommercialBenefitRate { get; set; }
        public bool IsActive { get; set; } = true;



        //Relations
        public HSCode HSCode { get; set; }
        public Country Country { get; set; }
    }
}
