using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.TransitOperation;

namespace ShipRate.Infrastructure.Data.Configurations
{

    public class TruckConfiguration : IEntityTypeConfiguration<Truck>
    {
        public void Configure(EntityTypeBuilder<Truck> builder)
        {
            builder.ToTable("Trucks", "transit");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.TruckReferenceTitle).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TruckPlateNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.TrailerPlateNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.FleetSmartCardNo).IsRequired().HasMaxLength(30);
            builder.Property(x => x.TPInsuranceNo).IsRequired().HasMaxLength(30);

            builder.Property(x => x.MaxVolumeCBM).HasPrecision(18, 6);



            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.LastDriver).WithMany().HasForeignKey(x => x.LastDriverId);
            builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.TruckType).WithMany().HasForeignKey(x => x.TruckTypeId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);

            builder.HasIndex(e => e.TruckPlateNumber).HasDatabaseName("IX_Trucks_TruckPlateNumber").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.TrailerPlateNumber).HasDatabaseName("IX_Trucks_TrailerPlateNumber").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.FleetSmartCardNo).HasDatabaseName("IX_Trucks_FleetSmartCardNo").IsUnique(false).IsClustered(false);

        }
    }

    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers", "transit");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.PassportNo).IsRequired().HasMaxLength(30);
            builder.Property(x => x.LicenseNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.SmartCardNo).IsRequired().HasMaxLength(30);


            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.Id);
        }
    }
    public class GPSProviderConfiguration : IEntityTypeConfiguration<GPSProvider>
    {
        public void Configure(EntityTypeBuilder<GPSProvider> builder)
        {
            builder.ToTable("GPSProviders", "transit");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.SiteAddress).HasMaxLength(45);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.Id);
        }
    }

    public class GPSDeviceConfiguration : IEntityTypeConfiguration<GPSDevice>
    {
        public void Configure(EntityTypeBuilder<GPSDevice> builder)
        {
            builder.ToTable("GPSDevices", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.GPSDeviceSerialNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.GPSProvider).WithMany().HasForeignKey(x => x.GPSProviderId);
            builder.HasOne(x => x.GPSDeviceStatus).WithMany().HasForeignKey(x => x.GPSDeviceStatusId);
        }
    }
    public class GPSAssignmentConfiguration : IEntityTypeConfiguration<GPSAssignment>
    {
        public void Configure(EntityTypeBuilder<GPSAssignment> builder)
        {
            builder.ToTable("GPSAssignments", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(100);
            builder.HasOne(x => x.GPSDevice).WithMany().HasForeignKey(x => x.GPSDeviceId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
        }
    }
    public class GPSRechargeConfiguration : IEntityTypeConfiguration<GPSRecharge>
    {
        public void Configure(EntityTypeBuilder<GPSRecharge> builder)
        {
            builder.ToTable("GPSRecharges", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(100);
            builder.HasOne(x => x.GPSDevice).WithMany().HasForeignKey(x => x.GPSDeviceId);
            builder.HasOne(x => x.GPSProvider).WithMany().HasForeignKey(x => x.GPSProviderId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class GPSTrackingPointConfiguration : IEntityTypeConfiguration<GPSTrackingPoint>
    {
        public void Configure(EntityTypeBuilder<GPSTrackingPoint> builder)
        {
            builder.ToTable("GPSTrackingPoints", "transit");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.GPSDevice).WithMany().HasForeignKey(x => x.GPSDeviceId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
        }
    }

    public class TIRCarnetConfiguration : IEntityTypeConfiguration<TIRCarnet>
    {
        public void Configure(EntityTypeBuilder<TIRCarnet> builder)
        {
            builder.ToTable("TIRCarnets", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TIRCarnetNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.IssuingCountryId);
            builder.HasOne(x => x.TIRCarnetStatus).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.IssuingAssociation).WithMany().HasForeignKey(x => x.IssuingAssociationId);
        }
    }

    public class TIRRequestConfiguration : IEntityTypeConfiguration<TIRRequest>
    {
        public void Configure(EntityTypeBuilder<TIRRequest> builder)
        {
            builder.ToTable("TIRRequests", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TIRRequestNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
        }
    }

    public class TIRUsageConfiguration : IEntityTypeConfiguration<TIRUsage>
    {
        public void Configure(EntityTypeBuilder<TIRUsage> builder)
        {
            builder.ToTable("TIRUsages", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.VoletNo).IsRequired().HasMaxLength(40);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.TIRAssignment).WithMany().HasForeignKey(x => x.TIRAssignmentId);
            builder.HasOne(x => x.Border).WithMany().HasForeignKey(x => x.BorderId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);
            builder.HasOne(x => x.CustomsOffice).WithMany().HasForeignKey(x => x.CustomsOfficeId);
        }
    }
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trips", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TripNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
            builder.HasOne(x => x.FromLocation).WithMany().HasForeignKey(x => x.FromLocationId);
            builder.HasOne(x => x.ToLocation).WithMany().HasForeignKey(x => x.ToLocationId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.TripStatus).WithMany().HasForeignKey(x => x.TripStatusId);

        }
    }
    public class TripStopConfiguration : IEntityTypeConfiguration<TripStop>
    {
        public void Configure(EntityTypeBuilder<TripStop> builder)
        {
            builder.ToTable("TripStops", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Trip).WithMany().HasForeignKey(x => x.TripId);
            builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId);
        }
    }
    public class ShipmentLegBorderBorderPointConfiguration : IEntityTypeConfiguration<ShipmentLegBorder>
    {
        public void Configure(EntityTypeBuilder<ShipmentLegBorder> builder)
        {
            builder.ToTable("ShipmentLegBorders", "transit");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.ShipmentLeg).WithMany().HasForeignKey(x => x.ShipmentLegId);
            builder.HasOne(x => x.Border).WithMany().HasForeignKey(x => x.BorderId);

        }
    }

    public class TransitPermitBorderPointConfiguration : IEntityTypeConfiguration<TransitPermit>
    {
        public void Configure(EntityTypeBuilder<TransitPermit> builder)
        {
            builder.ToTable("TransitPermits", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.PermitType).IsRequired().HasMaxLength(40);
            builder.Property(x => x.PermitNumber).IsRequired().HasMaxLength(40);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
        }
    }

    public class TIRAssignmentConfiguration : IEntityTypeConfiguration<TIRAssignment>
    {
        public void Configure(EntityTypeBuilder<TIRAssignment> builder)
        {
            builder.ToTable("TIRAssignments", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
            builder.HasOne(x => x.TIRCarnet).WithMany().HasForeignKey(x => x.TIRCarnetId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
        }
    }

    public class RMTOIntegrationConfiguration : IEntityTypeConfiguration<RMTOIntegration>
    {
        public void Configure(EntityTypeBuilder<RMTOIntegration> builder)
        {
            builder.ToTable("RMTOIntegrations", "transit");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RMTOBillNo).IsRequired().HasMaxLength(40);
            builder.Property(x => x.TrackingCode).IsRequired().HasMaxLength(40);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PayLoad).IsRequired().HasMaxLength(2000);


            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ShipmentLeg).WithMany().HasForeignKey(x => x.ShipmentLegId);
        }
    }
}
