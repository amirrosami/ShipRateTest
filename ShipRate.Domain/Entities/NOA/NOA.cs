//بررسی بیشتر
using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.NOA
{
    // Notice Of Arrival
    public class NOA : BusinessEntity
    {
        public int InOutId { get; set; }
        public int SequenceNumber { get; set; }
        public string NOANumber { get; set; } = null!;
        public int CompanyId { get; set; }
        public bool HasMaster { get; set; }
        public string? MasterNOANo { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? MasterNOAReceiveDate { get; set; }
        public Guid? CarrierId { get; set; }
        public Guid? AgentId { get; set; }
        public Guid BLId { get; set; }
        public Guid? VoyageId { get; set; }
        public string? VesselName { get; set; }
        public string? VoyageNo { get; set; }
        public Guid? ArrivalPortId { get; set; }
        public Guid? DischargeTerminalId { get; set; }
        public DateTime? AvailableForPickupDate { get; set; }
        public Guid? ConsigneeId { get; set; }
        public Guid? NotifyPartyId { get; set; }
        public Guid? NOAFormatId { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? Remarks { get; set; }
        public DateTime? IssuedDate { get; set; }
        public long? CalcRate { get; set; }

        // Relations
        public Company Company { get; set; } = null!;
        public BL BL { get; set; } = null!;
        public Person? Consignee { get; set; }
        public Person? NotifyParty { get; set; }

        public Carrier? Carrier { get; set; }
        public Agent? Agent { get; set; }

        public Location? ArrivalPort { get; set; }
        public Location? DischargeTerminal { get; set; }





        public Voyage? Voyage { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        public ICollection<NOAPrice> Prices { get; set; } = new List<NOAPrice>();
        public ICollection<NOASend> Sends { get; set; } = new List<NOASend>();
    }

    public class NOAPrice : BusinessEntity
    {
        public Guid NOAId { get; set; }
        public int SequenceNo { get; set; }
        public int ChargeTypeId { get; set; }

        public decimal? SysCalcInitialValue { get; set; }
        public decimal? SysCalcVATAmount { get; set; }
        public decimal? SysCalcTotalValue { get; set; }

        public decimal InitialValue { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TotalValue { get; set; }
        public int CurrencyId { get; set; }
        public Guid? BankAccountId { get; set; }
        public int? ClearanceTypeId { get; set; }

        // Relations
        public NOA NOA { get; set; } = null!;
        public BankAccount? BankAccount { get; set; }
        public Currency Currency { get; set; } = null!;
        public ChargeType ChargeType { get; set; } = null!;
        public ClearanceType ClearanceType { get; set; } = null!;
    }

    public class NOASend : BusinessEntity
    {
        public Guid NOAId { get; set; }
        public int SendToolTypeId { get; set; }

        public string? NOASender { get; set; }
        public string? NOAReceiver { get; set; }

        public DateTime? SendDateTime { get; set; }
        public DateTime? ReceiveDateTime { get; set; }

        public Guid? SmsMessageId { get; set; }
        // Relations
        public NOA NOA { get; set; } = null!;
        public SendToolType SendToolType { get; set; } = null!;
        public SmsMessage? SmsMessage { get; set; }
    }
}