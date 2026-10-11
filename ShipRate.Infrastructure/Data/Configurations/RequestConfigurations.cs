using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.ServiceRequests;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.ToTable("ServiceRequests", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RequestNo).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Subject).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(5000);
            builder.Property(x => x.PartyDetailName).HasMaxLength(100);
            builder.Property(x => x.ExternalReferenceNo).HasMaxLength(100);
            builder.Property(x => x.Remarks).HasMaxLength(1000);

            builder.HasOne(x => x.RequestType).WithMany().HasForeignKey(x => x.RequestTypeId);
            builder.HasOne(x => x.RequestStatus).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.RequestPriority).WithMany().HasForeignKey(x => x.PriorityId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.CostCurrency).WithMany().HasForeignKey(x => x.CostCurrencyId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);

            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.EditUserId);
        }
    }
    public class ServiceRequestChargeConfiguration : IEntityTypeConfiguration<ServiceRequestCharge>
    {
        public void Configure(EntityTypeBuilder<ServiceRequestCharge> builder)
        {
            builder.ToTable("ServiceRequestCharges", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(1000);
            builder.Property(x => x.CostTitle).HasMaxLength(100);

            builder.HasOne(x => x.ServiceRequest).WithMany().HasForeignKey(x => x.ServiceRequestId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }

    public class ServiceRequestApprovalConfiguration : IEntityTypeConfiguration<ServiceRequestApproval>
    {
        public void Configure(EntityTypeBuilder<ServiceRequestApproval> builder)
        {
            builder.ToTable("ServiceRequestApprovals", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(1000);
            builder.Property(x => x.ApproverName).HasMaxLength(70);

            builder.HasOne(x => x.ServiceRequest).WithMany().HasForeignKey(x => x.ServiceRequestId);
            builder.HasOne(x => x.ApprovalType).WithMany().HasForeignKey(x => x.ApprovalTypeId);
            builder.HasOne(x => x.ApprovalStatus).WithMany().HasForeignKey(x => x.ApprovalStatusId);
            builder.HasOne(x => x.ApproverUser).WithMany().HasForeignKey(x => x.ApproverUserId);
        }
    }

    public class BLAmendmentRequestConfiguration : IEntityTypeConfiguration<BLAmendmentRequest>
    {
        public void Configure(EntityTypeBuilder<BLAmendmentRequest> builder)
        {
            builder.ToTable("BLAmendmentRequests", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Reason).HasMaxLength(1000);

            builder.HasOne(x => x.ServiceRequest).WithMany().HasForeignKey(x => x.ServiceRequestId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
        }
    }
    public class BLAmendmentRequestLineConfiguration : IEntityTypeConfiguration<BLAmendmentRequestLine>
    {
        public void Configure(EntityTypeBuilder<BLAmendmentRequestLine> builder)
        {
            builder.ToTable("BLAmendmentRequestLines", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RequestedValue).HasMaxLength(1000);
            builder.Property(x => x.CurrentValue).HasMaxLength(1000);
            builder.Property(x => x.Remarks).HasMaxLength(1000);

            builder.HasOne(x => x.BLAmendmentRequest).WithMany(x => x.Lines).HasForeignKey(x => x.BLAmendmentRequestId);
            builder.HasOne(x => x.FieldType).WithMany().HasForeignKey(x => x.FieldTypeId);
        }
    }

    public class ConsolidationRequestConfiguration : IEntityTypeConfiguration<ConsolidationRequest>
    {
        public void Configure(EntityTypeBuilder<ConsolidationRequest> builder)
        {
            builder.ToTable("ConsolidationRequests", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Reason).HasMaxLength(1000);

            builder.HasOne(x => x.ServiceRequest).WithMany().HasForeignKey(x => x.ServiceRequestId);
        }
    }

    public class ConsolidationRequestItemConfiguration : IEntityTypeConfiguration<ConsolidationRequestItem>
    {
        public void Configure(EntityTypeBuilder<ConsolidationRequestItem> builder)
        {
            builder.ToTable("ConsolidationRequestItems", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(1000);

            builder.HasOne(x => x.ConsolidationRequest).WithMany(x => x.Items).HasForeignKey(x => x.ConsolidationRequestId);
            builder.HasOne(x => x.SourceBL).WithMany().HasForeignKey(x => x.SourceBLId);
            builder.HasOne(x => x.SourceJob).WithMany().HasForeignKey(x => x.SourceJobId);
            builder.HasOne(x => x.SourceBLContainer).WithMany().HasForeignKey(x => x.SourceBLContainerId);
            builder.HasOne(x => x.SourceBooking).WithMany().HasForeignKey(x => x.SourceBookingId);

        }
    }

    public class SplitRequestConfiguration : IEntityTypeConfiguration<SplitRequest>
    {
        public void Configure(EntityTypeBuilder<SplitRequest> builder)
        {
            builder.ToTable("SplitRequests", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Reason).HasMaxLength(1000);
            builder.Property(x => x.Notes).HasMaxLength(4000);

            builder.HasOne(x => x.SourceBL).WithMany().HasForeignKey(x => x.SourceBLId);
            builder.HasOne(x => x.SourceJob).WithMany().HasForeignKey(x => x.SourceJobId);
            builder.HasOne(x => x.SourceBooking).WithMany().HasForeignKey(x => x.SourceBookingId);


            builder.HasOne(x => x.ServiceRequest).WithMany().HasForeignKey(x => x.ServiceRequestId);
        }
    }

    public class SplitRequestTargetConfiguration : IEntityTypeConfiguration<SplitRequestTarget>
    {
        public void Configure(EntityTypeBuilder<SplitRequestTarget> builder)
        {
            builder.ToTable("SplitRequestTargets", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(1000);
            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.TargetBLNo).HasMaxLength(60);
            builder.Property(x => x.TargetKey).HasMaxLength(50);

            builder.HasOne(x => x.SplitRequest).WithMany(x => x.Targets).HasForeignKey(x => x.SplitRequestId);
        }
    }


    public class SplitRequestAllocationConfiguration : IEntityTypeConfiguration<SplitRequestAllocation>
    {
        public void Configure(EntityTypeBuilder<SplitRequestAllocation> builder)
        {
            builder.ToTable("SplitRequestAllocations", "req");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(1000);
            builder.Property(x => x.SourceLineKey).HasMaxLength(100);
            builder.Property(x => x.ContainerNo).HasMaxLength(15);
            builder.Property(x => x.CargoNo).HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(2000);

            builder.HasOne(x => x.SplitRequestTarget).WithMany(x => x.Allocations).HasForeignKey(x => x.SplitRequestTargetId);
        }
    }




}
