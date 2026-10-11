using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Marketing
{
    public class SalesTarget : BusinessEntity
    {
        public int STYear { get; set; }
        public int? STMonth { get; set; }
        public int? STQuarter { get; set; }


        //[Id][bigint] IDENTITY(1,1) NOT NULL,
        //[Year] [int] NOT NULL,
        //[Month] [int] NULL,
        //[Quarter][int] NULL,
        //[SalesPersonId][bigint] NULL,
        //[BranchId][bigint] NULL,
        //[TransportModeId][tinyint] NULL,
        //[TargetRevenue][decimal](18, 4) NULL,
        //[TargetProfit][decimal](18, 4) NULL,
        //[TargetShipments][int] NULL,
        //[TargetTEU][int] NULL,
        //[CurrencyId][smallint] NULL,
        //[ActualRevenue][decimal](18, 4) NULL,
        //[ActualProfit][decimal](18, 4) NULL,
        //[ActualShipments][int] NULL,
        //[ActualTEU][int] NULL,



    }
}


/*بعدا تکمیل شود 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */