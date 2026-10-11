using ShipRate.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class CargoCommodity : BusinessEntity
    {
        [MaxLength(250)]
        public string CargoCommodityName { get; set; }
        [MaxLength(250)]
        public string CargoCommodityLocalName { get; set; }
        public bool IsActive { get; set; } = true;

    }
}