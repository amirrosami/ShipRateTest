/*کار دارد */

using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerIO : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ContainerId { get; set; }
        public int ContainerIOTypeId { get; set; }
        public DateTime ContainerIODate { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Container Container { get; set; }
        public ContainerIOType ContainerIOType { get; set; }
        public Company Company { get; set; }
    }
}

/*بیشتر بررسی شود
 * 
 * 
 * 
 * 
 */