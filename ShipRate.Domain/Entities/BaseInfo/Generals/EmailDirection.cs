using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class EmailDirection : SystemEntity
    {
        public string EmailDirectionName { get; set; } = string.Empty;
    }
}

//1=Send
//2=Receive