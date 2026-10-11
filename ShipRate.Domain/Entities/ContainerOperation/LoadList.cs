using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    /**********************************************************************************************************************************************************************************/
    /**********************************************************************************************************************************************************************************/
    public class ContainerVoyageAllocation : BusinessEntity
    {
        public Guid ContainerId { get; set; }
        public Guid VoyageId { get; set; }

        public Guid LoadVoyagePortId { get; set; }

        public int AllocationStatusId { get; set; }

        public bool IsEmpty { get; set; }

        public decimal? GrossWeight { get; set; }
        public decimal? VGMKg { get; set; }

        public string? SealNumber { get; set; }

        public int? LoadSequenceNo { get; set; }

        public string? PlannedBay { get; set; }
        public string? PlannedRow { get; set; }
        public string? PlannedTier { get; set; }
        public string? PlannedStowagePosition { get; set; }

        public string? ActualBay { get; set; }
        public string? ActualRow { get; set; }
        public string? ActualTier { get; set; }
        public string? ActualStowagePosition { get; set; }

        public DateTime? LoadedAt { get; set; }

        public bool IsHazardous { get; set; }
        public int? ImdgClassId { get; set; }
        public string? UnNumber { get; set; }

        public bool IsOOG { get; set; }
        public string? OogRemarks { get; set; }
        public string? OperationalRemarks { get; set; }

        //relations
        public ContainerVoyageAllocationStatus AllocationStatus { get; set; } = null!;
    }
    /**********************************************************************************************************************************************************************************/
    /**********************************************************************************************************************************************************************************/
    public class LoadList : BusinessEntity
    {
        public Guid VoyageId { get; set; }
        public Guid LoadVoyagePortId { get; set; }

        public string LoadListNo { get; set; } = null!;

        public int VersionNo { get; set; }

        public int StatusId { get; set; }

        public DateTime? PreparedAt { get; set; }
        public DateTime? SentToTerminalAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public string? Remarks { get; set; }


        //relations

        public LoadListStatus Status { get; set; } = null!;
        public ICollection<LoadListItem> Items { get; set; } = new List<LoadListItem>();
    }
    /**********************************************************************************************************************************************************************************/
    /**********************************************************************************************************************************************************************************/
    public class LoadListItem : BusinessEntity
    {
        public Guid LoadListId { get; set; }
        public Guid ContainerVoyageAllocationId { get; set; }

        public int ItemStatusId { get; set; }

        public int? LoadSequenceNo { get; set; }

        public string? PlannedBay { get; set; }
        public string? PlannedRow { get; set; }
        public string? PlannedTier { get; set; }
        public string? PlannedStowagePosition { get; set; }

        public string? ActualBay { get; set; }
        public string? ActualRow { get; set; }
        public string? ActualTier { get; set; }
        public string? ActualStowagePosition { get; set; }

        public DateTime? LoadedAt { get; set; }

        public string? ShortShippedReason { get; set; }
        public string? ShutOutReason { get; set; }
        public string? RolloverReason { get; set; }

        public string? Remarks { get; set; }


        //Relations
        public LoadListItemStatus ItemStatus { get; set; } = null!;
        public LoadList LoadList { get; set; } = null!;
        public ContainerVoyageAllocation ContainerVoyageAllocation { get; set; } = null!;
    }
    /**********************************************************************************************************************************************************************************/
    /**********************************************************************************************************************************************************************************/
}
