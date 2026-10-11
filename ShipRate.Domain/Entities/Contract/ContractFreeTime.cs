//این کلاس کار دارد
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
namespace ShipRate.Domain.Entities.Contract
{
    public class ContractFreeTime : BusinessEntity
    {
        public Guid ContractId { get; set; }
        public int DischargeLoadId { get; set; }
        public int ContractFreeTimeDays { get; set; }

        //Relations 
        public Contract? Contract { get; set; }
        public DischargeLoad? DischargeLoad { get; set; }
    }
}