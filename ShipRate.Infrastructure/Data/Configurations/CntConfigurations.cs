using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.DetentionCalculation;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class ContainerConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.ToTable("Containers", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ContainerNumber).IsRequired().HasMaxLength(12);
            builder.Property(c => c.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.CntOwnTypeId);
            builder.HasOne(x => x.CntOwnLeaseType).WithMany().HasForeignKey(x => x.OwnLeaseTypeId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId);
            builder.HasOne(x => x.ResponsibleCompany).WithMany().HasForeignKey(x => x.ResponsibleCompanyId);

            builder.HasIndex(e => e.ContainerNumber).HasDatabaseName("IX_Containers_ContainerNumber").IsUnique(true).IsClustered(false);

        }
    }

    public class ContainerNumberHistoryConfiguration : IEntityTypeConfiguration<ContainerNumberHistory>
    {
        public void Configure(EntityTypeBuilder<ContainerNumberHistory> builder)
        {
            builder.ToTable("ContainerNumberHistory", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.OldContainerNumber).IsRequired().HasMaxLength(12);  //ex:IRXU123456-7
            builder.Property(c => c.NewContainerNumber).IsRequired().HasMaxLength(12);  //ex:IRXU123456-7
            builder.Property(c => c.Remarks).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
        }
    }

    public class ContainerActionConfiguration : IEntityTypeConfiguration<ContainerAction>
    {
        public void Configure(EntityTypeBuilder<ContainerAction> builder)
        {
            builder.ToTable("ContainerActions", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerNumber).IsRequired().HasMaxLength(12);  //ex:IRXU123456-7
            builder.Property(c => c.ContainerActionSourceReference).HasMaxLength(30);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.CntActionType).WithMany().HasForeignKey(x => x.CntActionTypeId);
            builder.HasOne(x => x.EventActionSource).WithMany().HasForeignKey(x => x.EventActionSourceId);
            builder.HasOne(x => x.ContainerUse).WithMany().HasForeignKey(x => x.ContainerUseId);
            builder.HasOne(x => x.FullMT).WithMany().HasForeignKey(x => x.FullMTId);

            builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId);
            builder.HasOne(x => x.SubLocation).WithMany().HasForeignKey(x => x.SubLocationId);
            builder.HasOne(x => x.Voyage).WithMany().HasForeignKey(x => x.VoyageId);
            builder.HasOne(x => x.VoyageType).WithMany().HasForeignKey(x => x.VoyageTypeId);
            builder.HasOne(x => x.LoadList).WithMany().HasForeignKey(x => x.LoadListId);
            builder.HasOne(x => x.ContainerVoyageAllocation).WithMany().HasForeignKey(x => x.ContainerVoyageAllocationId);
            builder.HasOne(x => x.ContainerCondition).WithMany().HasForeignKey(x => x.ContainerConditionStatusId);


            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.RTSInstruction).WithMany().HasForeignKey(x => x.RTSInstructionId);

            builder.HasIndex(e => e.ContainerNumber).HasDatabaseName("IX_ContainerActions_ContainerNumber").IsUnique(false).IsClustered(false);
        }
    }

    public class ContainerActionAttributeConfiguration : IEntityTypeConfiguration<ContainerActionAttribute>
    {
        public void Configure(EntityTypeBuilder<ContainerActionAttribute> builder)
        {
            builder.ToTable("ContainerActionAttributes", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.AttributeKey).IsRequired().HasMaxLength(50);
            builder.Property(c => c.AttributeValue).IsRequired().HasMaxLength(500);
            builder.Property(c => c.AttributeValueType).IsRequired().HasMaxLength(30);

            builder.HasOne(x => x.ContainerAction).WithMany(x => x.Attributes).HasForeignKey(x => x.ContainerActionId);
        }
    }


    public class ContainerSizeTypeConfiguration : IEntityTypeConfiguration<ContainerSizeType>
    {
        public void Configure(EntityTypeBuilder<ContainerSizeType> builder)
        {
            builder.ToTable("ContainerSizeTypes", "base");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.ContainerSize).WithMany().HasForeignKey(x => x.ContainerSizeId);
            builder.HasOne(x => x.ContainerType).WithMany().HasForeignKey(x => x.ContainerTypeId);
            builder.Property(c => c.FullName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ShortName).IsRequired().HasMaxLength(15);
            builder.Property(c => c.IsoCode).IsRequired().HasMaxLength(5);


            builder.HasData(
                new ContainerSizeType { Id = 10, ContainerSizeId = 20, ContainerTypeId = 1, FullName = "20ft,Standard Dry", ShortName = "20GP", DefaultTareWeight = 2200, MaxPayLoadKg = 20000, IsoCode = "2200" },
                new ContainerSizeType { Id = 20, ContainerSizeId = 20, ContainerTypeId = 2, FullName = "20ft,Heavy Duty", ShortName = "20HD", DefaultTareWeight = 2200, MaxPayLoadKg = 20000, IsoCode = "2209" },
                new ContainerSizeType { Id = 30, ContainerSizeId = 20, ContainerTypeId = 4, FullName = "20ft,Open Top", ShortName = "20OT", DefaultTareWeight = 2200, MaxPayLoadKg = 20000, IsoCode = "2250" },
                new ContainerSizeType { Id = 40, ContainerSizeId = 20, ContainerTypeId = 5, FullName = "20ft,Flat Rack", ShortName = "20FR", DefaultTareWeight = 2200, MaxPayLoadKg = 20000, IsoCode = "2262" },
                new ContainerSizeType { Id = 50, ContainerSizeId = 20, ContainerTypeId = 6, FullName = "20ft,Reefer", ShortName = "20RF", DefaultTareWeight = 2200, MaxPayLoadKg = 20000, IsoCode = "2230" },
                new ContainerSizeType { Id = 60, ContainerSizeId = 20, ContainerTypeId = 7, FullName = "20ft,Tank", ShortName = "20TK", DefaultTareWeight = 2200, MaxPayLoadKg = 20000, IsoCode = "2270" },

                new ContainerSizeType { Id = 70, ContainerSizeId = 40, ContainerTypeId = 1, FullName = "40ft,Standard Dry", ShortName = "40GP", DefaultTareWeight = 4200, MaxPayLoadKg = 40000, IsoCode = "4200" },
                new ContainerSizeType { Id = 80, ContainerSizeId = 40, ContainerTypeId = 3, FullName = "40ft,High Cube", ShortName = "40HC", DefaultTareWeight = 4200, MaxPayLoadKg = 40000, IsoCode = "4500" },
                new ContainerSizeType { Id = 90, ContainerSizeId = 40, ContainerTypeId = 4, FullName = "40ft,Open Top", ShortName = "40OT", DefaultTareWeight = 4200, MaxPayLoadKg = 40000, IsoCode = "4250" },
                new ContainerSizeType { Id = 100, ContainerSizeId = 40, ContainerTypeId = 5, FullName = "40ft,Flat Rack", ShortName = "40FR", DefaultTareWeight = 4200, MaxPayLoadKg = 40000, IsoCode = "4262" },
                new ContainerSizeType { Id = 110, ContainerSizeId = 40, ContainerTypeId = 6, FullName = "40ft,Reefer", ShortName = "40RF", DefaultTareWeight = 4200, MaxPayLoadKg = 40000, IsoCode = "4230" },
                new ContainerSizeType { Id = 120, ContainerSizeId = 40, ContainerTypeId = 7, FullName = "40ft,Tank", ShortName = "40TK", DefaultTareWeight = 4200, MaxPayLoadKg = 40000, IsoCode = "4270" }

                );


        }
    }

    public class ContainerConditionReportConfiguration : IEntityTypeConfiguration<ContainerConditionReport>
    {
        public void Configure(EntityTypeBuilder<ContainerConditionReport> builder)
        {
            builder.ToTable("ContainerConditionReports", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.InspectorName).HasMaxLength(50);
            builder.HasOne(x => x.ContainerAction).WithMany().HasForeignKey(x => x.ContainerActionId);
            builder.HasOne(x => x.ContainerConditionStatus).WithMany().HasForeignKey(x => x.ContainerConditionStatusId);
        }
    }

    public class ContainerDamageConfiguration : IEntityTypeConfiguration<ContainerDamage>
    {
        public void Configure(EntityTypeBuilder<ContainerDamage> builder)
        {
            builder.ToTable("ContainerDamages", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.ContainerConditionReport).WithMany().HasForeignKey(x => x.ContainerConditionReportId);
            builder.HasOne(x => x.DamageType).WithMany().HasForeignKey(x => x.ContainerDamageTypeId);
            builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.ContainerDamageAreaId);
            builder.HasOne(x => x.Severity).WithMany().HasForeignKey(x => x.ContainerDamageSeverityId);
        }
    }

    public class ContainerIOConfiguration : IEntityTypeConfiguration<ContainerIO>
    {
        public void Configure(EntityTypeBuilder<ContainerIO> builder)
        {
            builder.ToTable("ContainerIO", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerIOType).WithMany().HasForeignKey(x => x.ContainerIOTypeId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }
    public class ContainerRepairConfiguration : IEntityTypeConfiguration<ContainerRepair>
    {
        public void Configure(EntityTypeBuilder<ContainerRepair> builder)
        {
            builder.ToTable("ContainerRepairs", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.RepairBy).WithMany().HasForeignKey(x => x.RepairById);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }
    public class ContainerUseConfiguration : IEntityTypeConfiguration<ContainerUse>
    {
        public void Configure(EntityTypeBuilder<ContainerUse> builder)
        {
            builder.ToTable("ContainerUses", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ContainerNumber).IsRequired().HasMaxLength(12);
            builder.Property(x => x.SealNumber).IsRequired().HasMaxLength(10);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.FullMT).WithMany().HasForeignKey(x => x.FullMTId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.CntOwnTypeId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.RTSInstruction).WithMany().HasForeignKey(x => x.RTSInstructionId);

            builder.HasIndex(x => new { x.ContainerId, x.CycleClosed });
            builder.HasIndex(x => new { x.ContainerNumber, x.CycleClosed });

        }
    }
    public class DetentionConfiguration : IEntityTypeConfiguration<Detention>
    {
        public void Configure(EntityTypeBuilder<Detention> builder)
        {
            builder.ToTable("Detentions", "cnt");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.PrevDetention).WithMany().HasForeignKey(x => x.PrevDetentionId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.FinanceDirection).WithMany().HasForeignKey(x => x.FinanceDirectionId);
            builder.HasOne(x => x.MainCurrency).WithMany().HasForeignKey(x => x.MainCurrencyId);
            builder.HasOne(x => x.PaymentCurrency).WithMany().HasForeignKey(x => x.FinalCurrencyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.ExchangeRate).WithMany().HasForeignKey(x => x.ExchangeRateId);
            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);
            builder.HasOne(x => x.DetentionStatus).WithMany().HasForeignKey(x => x.DetentionStatusId);
            builder.HasOne(x => x.DetentionUpdater).WithMany().HasForeignKey(x => x.DetentionUpdateById);
            builder.HasOne(x => x.TariffBook).WithMany().HasForeignKey(x => x.TariffBookId);

            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.EditUser).WithMany().HasForeignKey(x => x.EditUserId);

            builder.Property(x => x.DetentionNo).HasDefaultValueSql("NEXT VALUE FOR dbo.Seq_DetentionCalculationNo");
            builder.HasIndex(d => d.DetentionNo).IsUnique();
        }
    }

    public class DetentionItemConfiguration : IEntityTypeConfiguration<DetentionItem>
    {
        public void Configure(EntityTypeBuilder<DetentionItem> builder)
        {
            builder.ToTable("DetentionItems", "cnt");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Detention).WithMany(x => x.Items).HasForeignKey(x => x.DetentionId);
            builder.HasOne(x => x.ContainerUse).WithMany().HasForeignKey(x => x.ContainerUseId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
        }
    }
    public class DetentionItemDetailConfiguration : IEntityTypeConfiguration<DetentionItemDetail>
    {
        public void Configure(EntityTypeBuilder<DetentionItemDetail> builder)
        {
            builder.ToTable("DetentionItemDetails", "cnt");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.DetentionItem).WithMany(x => x.Details).HasForeignKey(x => x.DetentionItemId);
        }
    }
    public class DetentionStartBaseConfiguration : IEntityTypeConfiguration<DetentionStartBase>
    {
        public void Configure(EntityTypeBuilder<DetentionStartBase> builder)
        {
            builder.ToTable("DetentionStartBases", "cnt");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
        }
    }

    public class OnlineDetentionConfiguration : IEntityTypeConfiguration<OnlineDetention>
    {
        public void Configure(EntityTypeBuilder<OnlineDetention> builder)
        {
            builder.ToTable("OnlineDetentions", "cnt");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.IssuerParty).WithMany().HasForeignKey(x => x.IssuerPartyId);
            builder.HasOne(x => x.IssuedToParty).WithMany().HasForeignKey(x => x.IssuedToPartyId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
        }
    }

    public class EmptyPickupConfiguration : IEntityTypeConfiguration<EmptyPickup>
    {
        public void Configure(EntityTypeBuilder<EmptyPickup> builder)
        {
            builder.ToTable("EmptyPickups", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.TrailerPlateNo).HasMaxLength(50);
            builder.Property(x => x.TruckPlateNo).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.DriverName).HasMaxLength(100);
            builder.Property(x => x.PickupReferenceNo).HasMaxLength(100);

            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ShipmentOperationPlan).WithMany().HasForeignKey(x => x.ShipmentOperationPlanId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.PickupLocation).WithMany().HasForeignKey(x => x.PickupLocationId);
            builder.HasOne(x => x.Driver).WithMany().HasForeignKey(x => x.DriverId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
        }
    }

    public class EmptyPickupContainerConfiguration : IEntityTypeConfiguration<EmptyPickupContainer>
    {
        public void Configure(EntityTypeBuilder<EmptyPickupContainer> builder)
        {
            builder.ToTable("EmptyPickupContainers", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ContainerNo).HasMaxLength(12);
            builder.Property(x => x.ConditionRemarks).HasMaxLength(200);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.SealNo).HasMaxLength(50);


            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.EmptyPickup).WithMany().HasForeignKey(x => x.EmptyPickupId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.ConditionStatus).WithMany().HasForeignKey(x => x.ConditionStatusId);
            builder.HasOne(x => x.RTSInstructionContainer).WithMany().HasForeignKey(x => x.RTSInstructionContainerId);
            builder.HasOne(x => x.RTSInstructionContainerType).WithMany().HasForeignKey(x => x.RTSInstructionContainerTypeId);
        }
    }


    public class StuffingConfiguration : IEntityTypeConfiguration<Stuffing>
    {
        public void Configure(EntityTypeBuilder<Stuffing> builder)
        {
            builder.ToTable("Stuffings", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.StuffingReferenceNo).HasMaxLength(50);
            builder.Property(x => x.SupervisorName).HasMaxLength(60);

            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ShipmentOperationPlan).WithMany().HasForeignKey(x => x.ShipmentOperationPlanId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.StuffingLocation).WithMany().HasForeignKey(x => x.StuffingLocationId);
        }
    }

    public class StuffingContainerConfiguration : IEntityTypeConfiguration<StuffingContainer>
    {
        public void Configure(EntityTypeBuilder<StuffingContainer> builder)
        {
            builder.ToTable("StuffingContainers", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.ContainerNo).HasMaxLength(12);
            builder.Property(x => x.SealNo).HasMaxLength(35);

            builder.HasOne(x => x.Stuffing).WithMany().HasForeignKey(x => x.StuffingId);
            builder.HasOne(x => x.EmptyPickupContainer).WithMany().HasForeignKey(x => x.EmptyPickupContainerId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
        }
    }

    public class StuffingContainerItemConfiguration : IEntityTypeConfiguration<StuffingContainerItem>
    {
        public void Configure(EntityTypeBuilder<StuffingContainerItem> builder)
        {
            builder.ToTable("StuffingContainerItems", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.StuffingContainer).WithMany().HasForeignKey(x => x.StuffingContainerId);
            builder.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId);
        }
    }

    public class FullGateInConfiguration : IEntityTypeConfiguration<FullGateIn>
    {
        public void Configure(EntityTypeBuilder<FullGateIn> builder)
        {
            builder.ToTable("FullGateIns", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.GateInReferenceNo).HasMaxLength(50);
            builder.Property(x => x.DriverName).HasMaxLength(60);
            builder.Property(x => x.TrailerPlateNo).HasMaxLength(40);
            builder.Property(x => x.TruckPlateNo).HasMaxLength(40);

            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ShipmentOperationPlan).WithMany().HasForeignKey(x => x.ShipmentOperationPlanId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.Terminal).WithMany().HasForeignKey(x => x.TerminalId);
            builder.HasOne(x => x.Stuffing).WithMany().HasForeignKey(x => x.StuffingId);
            builder.HasOne(x => x.Driver).WithMany().HasForeignKey(x => x.DriverId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
        }
    }


    public class FullGateInContainerConfiguration : IEntityTypeConfiguration<FullGateInContainer>
    {
        public void Configure(EntityTypeBuilder<FullGateInContainer> builder)
        {
            builder.ToTable("FullGateInContainers", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.ContainerNo).HasMaxLength(12);
            builder.Property(x => x.SealNo).HasMaxLength(35);

            builder.HasOne(x => x.FullGateIn).WithMany().HasForeignKey(x => x.FullGateInId);
            builder.HasOne(x => x.StuffingContainer).WithMany().HasForeignKey(x => x.StuffingContainerId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerConditionStatus).WithMany().HasForeignKey(x => x.ConditionStatusId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
        }
    }


    public class VGMConfiguration : IEntityTypeConfiguration<VGM>
    {
        public void Configure(EntityTypeBuilder<VGM> builder)
        {
            builder.ToTable("VGMs", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.ReferenceNo).HasMaxLength(50);

            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ShipmentOperationPlan).WithMany().HasForeignKey(x => x.ShipmentOperationPlanId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.FullGateIn).WithMany().HasForeignKey(x => x.FullGateInId);
        }
    }

    public class VGMContainerConfiguration : IEntityTypeConfiguration<VGMContainer>
    {
        public void Configure(EntityTypeBuilder<VGMContainer> builder)
        {
            builder.ToTable("VGMContainers", "cnt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.ContainerNo).HasMaxLength(12);
            builder.Property(x => x.WeighbridgeName).HasMaxLength(50);
            builder.Property(x => x.WeighbridgeTicketNo).HasMaxLength(50);

            builder.HasOne(x => x.VGM).WithMany().HasForeignKey(x => x.VGMId);
            builder.HasOne(x => x.FullGateInContainer).WithMany().HasForeignKey(x => x.FullGateInContainerId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.WeightUnit).WithMany().HasForeignKey(x => x.WeightUnitId);
        }
    }


    public class ContainerVoyageAllocationConfiguration : IEntityTypeConfiguration<ContainerVoyageAllocation>
    {
        public void Configure(EntityTypeBuilder<ContainerVoyageAllocation> builder)
        {
            builder.ToTable("ContainerVoyageAllocations", "ops");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.ContainerId)
                .IsRequired();

            builder.Property(x => x.VoyageId)
                .IsRequired();

            builder.Property(x => x.LoadVoyagePortId)
                .IsRequired();

            builder.Property(x => x.AllocationStatusId)
                .IsRequired();

            builder.Property(x => x.IsEmpty)
                .IsRequired();

            builder.Property(x => x.GrossWeight)
                .HasPrecision(18, 3);

            builder.Property(x => x.VGMKg)
                .HasPrecision(18, 3);

            builder.Property(x => x.SealNumber)
                .HasMaxLength(50);

            builder.Property(x => x.LoadSequenceNo);

            builder.Property(x => x.PlannedBay)
                .HasMaxLength(10);

            builder.Property(x => x.PlannedRow)
                .HasMaxLength(10);

            builder.Property(x => x.PlannedTier)
                .HasMaxLength(10);

            builder.Property(x => x.PlannedStowagePosition)
                .HasMaxLength(30);

            builder.Property(x => x.ActualBay)
                .HasMaxLength(10);

            builder.Property(x => x.ActualRow)
                .HasMaxLength(10);

            builder.Property(x => x.ActualTier)
                .HasMaxLength(10);

            builder.Property(x => x.ActualStowagePosition)
                .HasMaxLength(30);

            builder.Property(x => x.LoadedAt);


            builder.Property(x => x.IsHazardous)
                .IsRequired();

            builder.Property(x => x.UnNumber)
                .HasMaxLength(20);

            builder.Property(x => x.IsOOG)
                .IsRequired();

            builder.Property(x => x.OogRemarks)
                .HasMaxLength(500);

            builder.Property(x => x.OperationalRemarks)
                .HasMaxLength(1000);


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.CreateUserId)
                .IsRequired();

            builder.Property(x => x.EditUserId);

            builder.HasIndex(x => x.ContainerId);
            builder.HasIndex(x => x.VoyageId);
            builder.HasIndex(x => x.LoadVoyagePortId);
            builder.HasIndex(x => x.AllocationStatusId);

            builder.HasIndex(x => new { x.VoyageId, x.LoadVoyagePortId });

            builder.HasIndex(x => new
            {
                x.ContainerId,
                x.VoyageId,
                x.LoadVoyagePortId,
                x.IsDeleted
            })
            .IsUnique()
            .HasDatabaseName("UX_ContainerVoyageAllocations_Container_Voyage_Load_Discharge_IsDeleted");
            builder.HasOne(x => x.AllocationStatus).WithMany().HasForeignKey(x => x.AllocationStatusId);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }



    public class LoadListConfiguration : IEntityTypeConfiguration<LoadList>
    {
        public void Configure(EntityTypeBuilder<LoadList> builder)
        {
            builder.ToTable("LoadLists", "ops");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.VoyageId)
                .IsRequired();

            builder.Property(x => x.LoadVoyagePortId)
                .IsRequired();

            builder.Property(x => x.LoadListNo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.VersionNo)
                .IsRequired();

            builder.Property(x => x.StatusId)
                .IsRequired();

            builder.Property(x => x.PreparedAt);

            builder.Property(x => x.SentToTerminalAt);

            builder.Property(x => x.ConfirmedAt);

            builder.Property(x => x.ClosedAt);

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.CreateUserId)
                .IsRequired();

            builder.Property(x => x.EditUserId);

            builder.HasIndex(x => x.VoyageId);
            builder.HasIndex(x => x.LoadVoyagePortId);
            builder.HasIndex(x => x.StatusId);
            builder.HasIndex(x => x.LoadListNo);

            builder.HasIndex(x => new
            {
                x.VoyageId,
                x.LoadVoyagePortId,
                x.VersionNo,
                x.IsDeleted
            })
            .IsUnique()
            .HasDatabaseName("UX_LoadLists_Voyage_LoadPort_Version_IsDeleted");

            builder.HasIndex(x => new
            {
                x.LoadListNo,
                x.IsDeleted
            })
            .IsUnique()
            .HasDatabaseName("UX_LoadLists_LoadListNo_IsDeleted");
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasMany(x => x.Items).WithOne(x => x.LoadList).HasForeignKey(x => x.LoadListId);
        }
    }


    public class LoadListItemConfiguration : IEntityTypeConfiguration<LoadListItem>
    {
        public void Configure(EntityTypeBuilder<LoadListItem> builder)
        {
            builder.ToTable("LoadListItems", "ops");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.LoadListId)
                .IsRequired();

            builder.Property(x => x.ContainerVoyageAllocationId)
                .IsRequired();

            builder.Property(x => x.ItemStatusId)
                .IsRequired();

            builder.Property(x => x.LoadSequenceNo);

            builder.Property(x => x.PlannedBay)
                .HasMaxLength(10);

            builder.Property(x => x.PlannedRow)
                .HasMaxLength(10);

            builder.Property(x => x.PlannedTier)
                .HasMaxLength(10);

            builder.Property(x => x.PlannedStowagePosition)
                .HasMaxLength(30);

            builder.Property(x => x.ActualBay)
                .HasMaxLength(10);

            builder.Property(x => x.ActualRow)
                .HasMaxLength(10);

            builder.Property(x => x.ActualTier)
                .HasMaxLength(10);

            builder.Property(x => x.ActualStowagePosition)
                .HasMaxLength(30);

            builder.Property(x => x.LoadedAt);

            builder.Property(x => x.ShortShippedReason)
                .HasMaxLength(500);

            builder.Property(x => x.ShutOutReason)
                .HasMaxLength(500);

            builder.Property(x => x.RolloverReason)
                .HasMaxLength(500);

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.CreateUserId)
                .IsRequired();

            builder.Property(x => x.EditUserId);

            builder.HasIndex(x => x.LoadListId);
            builder.HasIndex(x => x.ContainerVoyageAllocationId);
            builder.HasIndex(x => x.ItemStatusId);

            builder.HasIndex(x => new
            {
                x.LoadListId,
                x.ContainerVoyageAllocationId,
                x.IsDeleted
            })
            .IsUnique()
            .HasDatabaseName("UX_LoadListItems_LoadList_Allocation_IsDeleted");

            builder.HasOne(x => x.LoadList)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.LoadListId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ItemStatus)
                .WithMany()
                .HasForeignKey(x => x.ItemStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}
