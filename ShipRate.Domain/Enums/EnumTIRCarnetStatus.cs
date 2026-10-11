namespace ShipRate.Domain.Enums
{
    public enum EnumTIRCarnetStatus
    {
        Draft = -1,
        InUse = 1,
        Closed = 2,
        Returned = 3,
        Settled = 4,
        Expired = 5,
        Canceled = 9
    }
}
/*InUse =After Assign To Shipment
 * Closed=After POD
 * 
 * 
 * 
 */

/*Only Event-Based
 * 
 * 
 */