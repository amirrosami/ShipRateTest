namespace ShipRate.Domain.Enums
{
    public enum EnumShipmentStatus
    {
        Draft = 1,
        Opened = 2,
        BOOKED = 3,
        IN_TRANSIT = 4,
        ARRIVED = 5,
        DELIVERED = 6,
        CLOSED = 7,
        CANCELLED = 99
    }
}


/*
DRAFT
  ↓ confirm
OPEN
  ↓ booking
BOOKED
  ↓ depart
IN_TRANSIT
  ↓ arrive
ARRIVED
  ↓ deliver
DELIVERED
  ↓ close
CLOSED

OPEN / BOOKED
  ↓ cancel
CANCELLED
 * 
 * 
 * 
 * 
 * 
 */