using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class CntActionType : SystemEntity
    {
        public string CntActionTypeCode { get; set; } = string.Empty;
        public string CntActionTypeName { get; set; } = string.Empty;
    }
}



/*
 * ALTER TABLE base.CntActionTypes
   ADD 
    RequiresContainerUse BIT NOT NULL DEFAULT 0,
    AllowsContainerUse BIT NOT NULL DEFAULT 1,
    IsOperational BIT NOT NULL DEFAULT 1;
 * 
 * بعدا این هم درست شود
 */
