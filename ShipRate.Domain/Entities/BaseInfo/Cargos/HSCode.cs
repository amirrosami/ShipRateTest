using ShipRate.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class HSCode : BusinessEntity
    {
        [MinLength(2), MaxLength(6)]
        public string HSCodeCode { get; set; } = string.Empty;
        public string HSCodeName { get; set; } = string.Empty;
        public int HSLevelId { get; set; }
        public Guid? ParentId { get; set; }
        //Relations
        public HSLevel HSLevel { get; set; }
        public HSCode Parent { get; set; }
    }
}