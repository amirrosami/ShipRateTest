using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Contract;
using ShipRate.Domain.Entities.Customs;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.NOA;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Operations.TasksManage;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.RateQuotation;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
    {
        public void Configure(EntityTypeBuilder<Carrier> builder)
        {
            builder.ToTable("Carriers", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CarrierCode).HasMaxLength(20);
            builder.Property(x => x.BookingEmail).HasMaxLength(35);
            builder.Property(x => x.RegistrationNumber).HasMaxLength(35);
            builder.Property(x => x.OperatingRegion).HasMaxLength(50);
            builder.Property(x => x.TrackingUrl).HasMaxLength(100);
            builder.Property(x => x.AWBPrefix).HasMaxLength(20);

            builder.HasOne(x => x.CarrierType).WithMany().HasForeignKey(x => (int)x.CarrierTypeId);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.Id);

        }
    }

    public class CarrierCodeConfiguration : IEntityTypeConfiguration<CarrierCode>
    {
        public void Configure(EntityTypeBuilder<CarrierCode> builder)
        {
            builder.ToTable("CarrierCodes", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CodeType).IsRequired().HasMaxLength(20);
            builder.Property(x => x.CodeValue).IsRequired().HasMaxLength(25);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
        }
    }


    public class CourierCompanyConfiguration : IEntityTypeConfiguration<CourierCompany>
    {
        public void Configure(EntityTypeBuilder<CourierCompany> builder)
        {
            builder.ToTable("CourierCompanies", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.Id);
        }
    }

    public class VesselConfiguration : IEntityTypeConfiguration<Vessel>
    {
        public void Configure(EntityTypeBuilder<Vessel> builder)
        {
            builder.ToTable("Vessels", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VesselCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.VesselName).IsRequired().HasMaxLength(70);
            builder.Property(x => x.CallSign).HasMaxLength(50);
            builder.Property(x => x.IMONumber).HasMaxLength(50);
            builder.Property(x => x.MMSI).HasMaxLength(10);






            builder.HasOne(x => x.VesselType)
                    .WithMany()
                    .HasForeignKey(x => x.VesselTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FlagCountry)
                    .WithMany()
                    .HasForeignKey(x => x.FlagCountryId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.VesselName).HasDatabaseName("IX_Vessels_VesselName").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.IMONumber).HasDatabaseName("IX_Vessels_IMONumber").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.MMSI).HasDatabaseName("IX_Vessels_MMSI").IsUnique(false).IsClustered(false);
        }
    }
    public class SuippingLineServiceConfiguration : IEntityTypeConfiguration<ShippingLineService>
    {
        public void Configure(EntityTypeBuilder<ShippingLineService> builder)
        {
            builder.ToTable("ShippingLineServices", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ShippingLineServiceCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ShippingLineServiceName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ShippingLineServiceRemarks).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.ShippingLine).WithMany(x => x.ShippingLineServices).HasForeignKey(x => x.ShippingLineId);
        }
    }


    public class LoopConfiguration : IEntityTypeConfiguration<Loop>
    {
        public void Configure(EntityTypeBuilder<Loop> builder)
        {
            builder.ToTable("Loops", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.LoopName).HasMaxLength(50);
            builder.Property(x => x.LoopCode).HasMaxLength(20);
            builder.Property(x => x.LoopRemarks).HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.ShippingLineService).WithMany(x => x.Loops).HasForeignKey(x => x.ShippingLineServiceId);
        }
    }

    public class LoopPortConfiguration : IEntityTypeConfiguration<LoopPort>
    {
        public void Configure(EntityTypeBuilder<LoopPort> builder)
        {
            builder.ToTable("LoopPorts", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LoopPortRemarks).HasMaxLength(200);
            builder.HasOne(x => x.Loop).WithMany(x => x.LoopPorts).HasForeignKey(x => x.LoopId);
            builder.HasOne(x => x.Port).WithMany().HasForeignKey(x => x.PortId);

        }
    }

    //public class TransportMovementConfiguration : IEntityTypeConfiguration<TransportMovement>
    //{
    //    public void Configure(EntityTypeBuilder<TransportMovement> builder)
    //    {
    //        builder.ToTable("TransportMovements", "ops");
    //        builder.HasKey(c => c.Id);
    //        builder.Property(x => x.MovementNumber).IsRequired().HasMaxLength(25);
    //        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
    //        builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
    //        builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
    //    }
    //}

    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.ToTable("Flights", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.FlightNumber).IsRequired().HasMaxLength(20);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.AirLine).WithMany().HasForeignKey(x => x.AirLineId);
            builder.HasOne(x => x.OriginAirPort).WithMany().HasForeignKey(x => x.OriginAirportId);
            builder.HasOne(x => x.DestinationAirPort).WithMany().HasForeignKey(x => x.DestinationAirportId);

        }
    }

    public class FlightDateConfiguration : IEntityTypeConfiguration<FlightDate>
    {
        public void Configure(EntityTypeBuilder<FlightDate> builder)
        {
            builder.ToTable("FlightDates", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).HasMaxLength(200);
            builder.HasOne(x => x.Flight).WithMany().HasForeignKey(x => x.FlightId);
        }
    }

    public class PortDistanceConfiguration : IEntityTypeConfiguration<PortDistance>
    {
        public void Configure(EntityTypeBuilder<PortDistance> builder)
        {
            builder.ToTable("PortDistances", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Source).HasMaxLength(100);

            builder.HasOne(x => x.FromPort).WithMany().HasForeignKey(x => x.FromPortId);
            builder.HasOne(x => x.ToPort).WithMany().HasForeignKey(x => x.ToPortId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
        }
    }
    public class PostalDispatchConfiguration : IEntityTypeConfiguration<PostalDispatch>
    {
        public void Configure(EntityTypeBuilder<PostalDispatch> builder)
        {
            builder.ToTable("PostalDispatches", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.TrackingNumber).HasMaxLength(50);


            builder.HasOne(x => x.CourierCompany).WithMany().HasForeignKey(x => x.CourierCompanyId);
            builder.HasOne(x => x.CourierMethod).WithMany().HasForeignKey(x => x.CourierMethodId);
            builder.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId);
            builder.HasOne(x => x.Receiver).WithMany().HasForeignKey(x => x.ReceiverId);
            builder.HasOne(x => x.PostalDispatchStatus).WithMany().HasForeignKey(x => x.StatusId);

        }
    }
    public class PostalDispatchItemConfiguration : IEntityTypeConfiguration<PostalDispatchItem>
    {
        public void Configure(EntityTypeBuilder<PostalDispatchItem> builder)
        {
            builder.ToTable("PostalDispatchItems", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentDescription).HasMaxLength(500);
            builder.Property(x => x.ItemReference).HasMaxLength(100);
            builder.HasOne(x => x.PostalDispatch).WithMany(x => x.Items).HasForeignKey(x => x.PostalDispatchId);
        }
    }
    public class PostalDispatchConfirmationConfiguration : IEntityTypeConfiguration<PostalDispatchConfirmation>
    {
        public void Configure(EntityTypeBuilder<PostalDispatchConfirmation> builder)
        {
            builder.ToTable("PostalDispatchConfirmations", "ops");
            builder.Property(x => x.ConditionOnReceipt).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.PostalDispatch).WithMany().HasForeignKey(x => x.PostalDispatchId);
        }
    }


    public class TIPConfiguration : IEntityTypeConfiguration<TIP>
    {
        public void Configure(EntityTypeBuilder<TIP> builder)
        {
            builder.ToTable("TIPs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TIPNo).IsRequired().HasMaxLength(25);
            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.CustomsOffice).WithMany().HasForeignKey(x => x.CustomsOfficeId);
            builder.HasOne(x => x.TIPStatus).WithMany().HasForeignKey(x => x.TIPStatusId);
            builder.HasOne(x => x.VoyagePort).WithMany().HasForeignKey(x => x.VoyagePortId);
        }
    }

    public class VoyageConfiguration : IEntityTypeConfiguration<Voyage>
    {
        public void Configure(EntityTypeBuilder<Voyage> builder)
        {
            builder.ToTable("Voyages", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VoyageNo).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
            builder.Property(x => x.DirectionCode).HasMaxLength(10);

            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Vessel).WithMany().HasForeignKey(x => x.VesselId);
            builder.HasOne(x => x.VoyageOwnType).WithMany().HasForeignKey(x => x.VoyageOwnTypeId);
            builder.HasOne(x => x.ShippingLineService).WithMany().HasForeignKey(x => x.ShippingLineServiceId);
            builder.HasOne(x => x.Loop).WithMany().HasForeignKey(x => x.LoopId);

            builder.HasIndex(e => e.VoyageNo).HasDatabaseName("IX_Voyages_VoyageNo").IsUnique(false).IsClustered(false);
        }
    }

    public class VoyagePortConfiguration : IEntityTypeConfiguration<VoyagePort>
    {
        public void Configure(EntityTypeBuilder<VoyagePort> builder)
        {
            builder.ToTable("VoyagePorts", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.InboundVoyage).WithMany().HasForeignKey(x => x.InboundVoyageId);
            builder.HasOne(x => x.OutboundVoyage).WithMany().HasForeignKey(x => x.OutboundVoyageId);
            builder.HasOne(x => x.Port).WithMany().HasForeignKey(x => x.PortId);
        }
    }


    public class VoyagePortEventConfiguration : IEntityTypeConfiguration<VoyagePortEvent>
    {
        public void Configure(EntityTypeBuilder<VoyagePortEvent> builder)
        {
            builder.ToTable("VoyagePortEvents", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VoyagePortEventRemarks).IsRequired().HasMaxLength(200);


            builder.HasOne(x => x.VoyagePort).WithMany(x => x.VoyagePortEvents).HasForeignKey(x => x.VoyagePortId);
            builder.HasOne(x => x.VoyageEventType).WithMany().HasForeignKey(x => x.EventTypeId);
            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.EditUserId);
        }
    }

    public class VoyageRelationConfiguration : IEntityTypeConfiguration<VoyageRelation>
    {
        public void Configure(EntityTypeBuilder<VoyageRelation> builder)
        {
            builder.ToTable("VoyageRelations", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Port).WithMany().HasForeignKey(x => x.PortId);
            builder.HasOne(x => x.Terminal).WithMany().HasForeignKey(x => x.TerminalId);
            builder.HasOne(x => x.InboundVoyage).WithMany().HasForeignKey(x => x.InboundVoyageId);
            builder.HasOne(x => x.OutboundVoyage).WithMany().HasForeignKey(x => x.OutboundVoyageId);
            builder.HasOne(x => x.PortOperationType).WithMany().HasForeignKey(x => x.PortOperationTypeId);
        }
    }

    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentDescription).IsRequired().HasMaxLength(200);
            builder.Property(x => x.DocumentTitle).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DocumentNo).IsRequired().HasMaxLength(35);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.DocumentType).WithMany().HasForeignKey(x => x.DocumentTypeId);
            builder.HasOne(x => x.ObjectType).WithMany().HasForeignKey(x => x.ObjectTypeId);

            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.EditUser).WithMany().HasForeignKey(x => x.EditUserId);


            builder.HasIndex(e => e.DocumentTitle).HasDatabaseName("IX_Documents_DocumentTitle").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.DocumentNo).HasDatabaseName("IX_Documents_DocumentNo").IsUnique(false).IsClustered(false);

        }
    }

    public class DocumentAttachmentConfiguration : IEntityTypeConfiguration<DocumentAttachment>
    {
        public void Configure(EntityTypeBuilder<DocumentAttachment> builder)
        {
            builder.ToTable("DocumentAttachments", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentAttachmentFileExtension).IsRequired().HasMaxLength(10);
            builder.Property(x => x.DocumentAttachmentFileName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.DocumentAttachmentFilePath).IsRequired().HasMaxLength(150);
            builder.Property(x => x.DocumentAttachmentFileType).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Document).WithMany(x => x.Attachments).HasForeignKey(x => x.DocumentId);
        }
    }

    public class DocumentEmailConfiguration : IEntityTypeConfiguration<DocumentEmail>
    {
        public void Configure(EntityTypeBuilder<DocumentEmail> builder)
        {
            builder.ToTable("DocumentEmails", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentEmailFrom).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DocumentEmailTo).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DocumentEmailSubject).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.DocumentEmailCC).HasMaxLength(200);
            builder.Property(x => x.DocumentEmailBCC).HasMaxLength(200);

            builder.HasOne(x => x.Document).WithMany(x => x.Emails).HasForeignKey(x => x.DocumentId);
        }
    }

    public class AgentChargeTypeConfiguration : IEntityTypeConfiguration<AgentChargeType>
    {
        public void Configure(EntityTypeBuilder<AgentChargeType> builder)
        {
            builder.ToTable("AgentChargeTypes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
        }
    }

    public class AgentRoleCommissionPerSizeTypeConfiguration : IEntityTypeConfiguration<AgentRoleCommissionPerSize>
    {
        public void Configure(EntityTypeBuilder<AgentRoleCommissionPerSize> builder)
        {
            builder.ToTable("AgentRoleCommissionPerSizes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.AgentRole).WithMany().HasForeignKey(x => x.AgentRoleId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.ContainerSize).WithMany().HasForeignKey(x => x.ContainerSizeId);
        }
    }

    public class AgencyContractConfiguration : IEntityTypeConfiguration<AgencyContract>
    {
        public void Configure(EntityTypeBuilder<AgencyContract> builder)
        {
            builder.ToTable("AgencyContracts", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ContractNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.AgreementType).HasMaxLength(50);
            builder.Property(x => x.Terms).HasMaxLength(200);






            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
        }
    }


    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.FPNo).HasMaxLength(50);
            builder.Property(x => x.ContractNumber).HasMaxLength(50);
            builder.Property(x => x.ContractTitle).HasMaxLength(100);
            builder.Property(x => x.ContractRemarks).HasMaxLength(200);


            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.ContractType).WithMany().HasForeignKey(x => x.ContractTypeId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.ShippingLineService).WithMany().HasForeignKey(x => x.ShippingLineServiceId);
        }
    }

    public class ContractChargeTypeConfiguration : IEntityTypeConfiguration<ContractChargeType>
    {
        public void Configure(EntityTypeBuilder<ContractChargeType> builder)
        {
            builder.ToTable("ContractChargeTypes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
        }
    }
    public class ContractContainerSizeTypeConfiguration : IEntityTypeConfiguration<ContractContainerSizeType>
    {
        public void Configure(EntityTypeBuilder<ContractContainerSizeType> builder)
        {
            builder.ToTable("ContractContainerSizeTypes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
        }
    }

    public class ContractCustomerConfiguration : IEntityTypeConfiguration<ContractCustomer>
    {
        public void Configure(EntityTypeBuilder<ContractCustomer> builder)
        {
            builder.ToTable("ContractCustomers", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.CustomerRoleType).WithMany().HasForeignKey(x => x.CustomerRoleTypeId);
        }
    }
    public class ContractFreeTimeConfiguration : IEntityTypeConfiguration<ContractFreeTime>
    {
        public void Configure(EntityTypeBuilder<ContractFreeTime> builder)
        {
            builder.ToTable("ContractFreeTimes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.DischargeLoad).WithMany().HasForeignKey(x => x.DischargeLoadId);
        }
    }

    public class ContractRouteConfiguration : IEntityTypeConfiguration<ContractRoute>
    {
        public void Configure(EntityTypeBuilder<ContractRoute> builder)
        {
            builder.ToTable("ContractRoutes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.Route).WithMany().HasForeignKey(x => x.RouteId);
            builder.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId);
            builder.HasOne(x => x.Destination).WithMany().HasForeignKey(x => x.DestinationId);
        }
    }

    public class ContractRateConfiguration : IEntityTypeConfiguration<ContractRate>
    {
        public void Configure(EntityTypeBuilder<ContractRate> builder)
        {
            builder.ToTable("ContractRates", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.ContractRoute).WithMany().HasForeignKey(x => x.ContractRouteId);
            builder.HasOne(x => x.ContractContainerSizeType).WithMany().HasForeignKey(x => x.ContractContainerSizeTypeId);
            builder.HasOne(x => x.ContractChargeType).WithMany().HasForeignKey(x => x.ContractChargeTypeId);
            builder.HasOne(x => x.DG).WithMany().HasForeignKey(x => x.DGId);
            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.CntOwnTypeId);
        }
    }

    public class ContractDiscountConfiguration : IEntityTypeConfiguration<ContractDiscount>
    {
        public void Configure(EntityTypeBuilder<ContractDiscount> builder)
        {
            builder.ToTable("ContractDiscounts", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RequestRemarks).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ConfirmRejectRemarks).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.ContractRate).WithMany().HasForeignKey(x => x.ContractRateId);
            builder.HasOne(x => x.RequestedBy).WithMany().HasForeignKey(x => x.RequestedById);
            builder.HasOne(x => x.ConfirmRejectedBy).WithMany().HasForeignKey(x => x.ConfirmRejectedById);
        }
    }
    public class FACContractConfiguration : IEntityTypeConfiguration<FACContract>
    {
        public void Configure(EntityTypeBuilder<FACContract> builder)
        {
            builder.ToTable("FACContracts", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.FacContractTitle).IsRequired().HasMaxLength(200);
            builder.Property(x => x.FacContractRemarks).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.ForwarderId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }

    public class ContractSupplementConfiguration : IEntityTypeConfiguration<ContractSupplement>
    {
        public void Configure(EntityTypeBuilder<ContractSupplement> builder)
        {
            builder.ToTable("ContractSupplements", "ops");
            builder.HasKey(c => c.Id);


            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.FACContract).WithMany().HasForeignKey(x => x.FACContractId);
        }
    }

    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LineBookingNo).HasMaxLength(40);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.BookingNo).IsRequired().HasMaxLength(40);

            builder.HasOne(x => x.Contract).WithMany().HasForeignKey(x => x.ContractId);
            builder.HasOne(x => x.RateRequest).WithMany().HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.BuyQuote).WithMany().HasForeignKey(x => x.BuyQuoteId);
            builder.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId);
            builder.HasOne(x => x.POL).WithMany().HasForeignKey(x => x.POLId);
            builder.HasOne(x => x.POD).WithMany().HasForeignKey(x => x.PODId);
            builder.HasOne(x => x.Dest).WithMany().HasForeignKey(x => x.DestId);
            builder.HasOne(x => x.TranshipPort).WithMany().HasForeignKey(x => x.TranshipPortId);
            builder.HasOne(x => x.ParentBooking).WithMany().HasForeignKey(x => x.ParentBookingId);

            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierPartyId);
            builder.HasOne(x => x.VendorParty).WithMany().HasForeignKey(x => x.VendorPartyId);
            builder.HasOne(x => x.BookingAgentParty).WithMany().HasForeignKey(x => x.BookingAgentPartyId);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CompanyRoleType).WithMany().HasForeignKey(x => x.CompanyRoleTypeId);
            builder.HasOne(x => x.BookingStatus).WithMany().HasForeignKey(x => x.BookingStatusId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.LoadType).WithMany().HasForeignKey(x => x.LoadTypeId);
            builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId);



            builder.HasIndex(e => e.BookingNo).HasDatabaseName("IX_Bookings_BookingNo").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.LineBookingNo).HasDatabaseName("IX_Bookings_LineBookingNo").IsUnique(false).IsClustered(false);

        }
    }


    public class BookingLegConfiguration : IEntityTypeConfiguration<BookingLeg>
    {
        public void Configure(EntityTypeBuilder<BookingLeg> builder)
        {
            builder.ToTable("BookingLegs", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Booking).WithMany(x => x.BookingLegs).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.ShipmentLeg).WithMany().HasForeignKey(x => x.ShipmentLegId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
            builder.HasOne(x => x.FromLocation).WithMany().HasForeignKey(x => x.FromLocationId);
            builder.HasOne(x => x.ToLocation).WithMany().HasForeignKey(x => x.ToLocationId);
        }
    }

    public class BookingSeaDetailConfiguration : IEntityTypeConfiguration<BookingSeaDetail>
    {
        public void Configure(EntityTypeBuilder<BookingSeaDetail> builder)
        {
            builder.ToTable("BookingSeaDetails", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Booking).WithOne(x => x.SeaDetail).HasForeignKey<BookingSeaDetail>(x => x.BookingId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => x.BookingId).IsUnique();


            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.EmptyPickupDepot).WithMany().HasForeignKey(x => x.EmptyPickupDepotId);
            builder.HasOne(x => x.EmptyReturnDepot).WithMany().HasForeignKey(x => x.EmptyReturnDepotId);
            builder.HasOne(x => x.ReceivingTerminal).WithMany().HasForeignKey(x => x.ReceivingTerminalId);
        }
    }

    public class BookingSeaVoyageConfiguration : IEntityTypeConfiguration<BookingSeaVoyage>
    {
        public void Configure(EntityTypeBuilder<BookingSeaVoyage> builder)
        {
            builder.ToTable("BookingSeaVoyages", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.BookingSeaDetail).WithMany(x => x.Voyages).HasForeignKey(x => x.BookingSeaDetailId);
            builder.HasOne(x => x.Voyage).WithMany().HasForeignKey(x => x.VoyageId);
            builder.HasOne(x => x.FromPort).WithMany().HasForeignKey(x => x.FromPortId);
            builder.HasOne(x => x.ToPort).WithMany().HasForeignKey(x => x.ToPortId);
            builder.HasOne(x => x.Vessel).WithMany().HasForeignKey(x => x.VesselId);
        }
    }

    public class BookingRoadDetailConfiguration : IEntityTypeConfiguration<BookingRoadDetail>
    {
        public void Configure(EntityTypeBuilder<BookingRoadDetail> builder)
        {
            builder.ToTable("BookingRoadDetails", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Booking).WithMany(x => x.RoadDetails).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.BookingLeg).WithMany().HasForeignKey(x => x.BookingLegId);
            builder.HasOne(x => x.PickupLocation).WithMany().HasForeignKey(x => x.PickupLocationId);
            builder.HasOne(x => x.DeliveryLocation).WithMany().HasForeignKey(x => x.DeliveryLocationId);
            builder.HasOne(x => x.TruckType).WithMany().HasForeignKey(x => x.TruckTypeId);
            builder.HasOne(x => x.ContainerPickupDepot).WithMany().HasForeignKey(x => x.ContainerPickupDepotId);
            builder.HasOne(x => x.ContainerReturnDepot).WithMany().HasForeignKey(x => x.ContainerReturnDepotId);
        }
    }
    public class BookingRailDetailConfiguration : IEntityTypeConfiguration<BookingRailDetail>
    {
        public void Configure(EntityTypeBuilder<BookingRailDetail> builder)
        {
            builder.ToTable("BookingRailDetails", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Booking).WithMany(x => x.RailDetails).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.BookingLeg).WithMany().HasForeignKey(x => x.BookingLegId);
            builder.HasOne(x => x.RailOperator).WithMany().HasForeignKey(x => x.RailOperatorId);
            builder.HasOne(x => x.OriginStation).WithMany().HasForeignKey(x => x.OriginStationId);
            builder.HasOne(x => x.DestinationStation).WithMany().HasForeignKey(x => x.DestinationStationId);
        }
    }

    public class BookingAirDetailConfiguration : IEntityTypeConfiguration<BookingAirDetail>
    {
        public void Configure(EntityTypeBuilder<BookingAirDetail> builder)
        {
            builder.ToTable("BookingAirDetails", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Booking).WithOne(x => x.AirDetail).HasForeignKey<BookingAirDetail>(x => x.BookingId);
            builder.HasOne(x => x.DepartureAirport).WithMany().HasForeignKey(x => x.DepartureAirportId);
            builder.HasOne(x => x.ArrivalAirport).WithMany().HasForeignKey(x => x.ArrivalAirportId);
            builder.HasOne(x => x.Airline).WithMany().HasForeignKey(x => x.AirlineId);
            builder.HasOne(x => x.CargoTerminal).WithMany().HasForeignKey(x => x.CargoTerminalId);
        }
    }

    public class BookingAirFlightConfiguration : IEntityTypeConfiguration<BookingAirFlight>
    {
        public void Configure(EntityTypeBuilder<BookingAirFlight> builder)
        {
            builder.ToTable("BookingAirFlights", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.BookingAirDetail).WithMany(x => x.Flights).HasForeignKey(x => x.BookingAirDetailId);
            builder.HasOne(x => x.FromAirport).WithMany().HasForeignKey(x => x.FromAirportId);
            builder.HasOne(x => x.ToAirport).WithMany().HasForeignKey(x => x.ToAirportId);
            builder.HasOne(x => x.Airline).WithMany().HasForeignKey(x => x.AirlineId);
        }
    }





    public class BookingBorderConfiguration : IEntityTypeConfiguration<BookingBorder>
    {
        public void Configure(EntityTypeBuilder<BookingBorder> builder)
        {
            builder.ToTable("BookingBorders", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.Border).WithMany().HasForeignKey(x => x.BorderId);
        }
    }

    public class BookingChangeConfiguration : IEntityTypeConfiguration<BookingChange>
    {
        public void Configure(EntityTypeBuilder<BookingChange> builder)
        {
            builder.ToTable("BookingChanges", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.BookingChangeType).WithMany().HasForeignKey(x => x.BookingChangeTypeId);
            builder.HasOne(x => x.NewBooking).WithMany().HasForeignKey(x => x.NewBookingId);
            builder.HasOne(x => x.OldBooking).WithMany().HasForeignKey(x => x.OldBookingId);
        }
    }

    public class BookingContainerTypeConfiguration : IEntityTypeConfiguration<BookingContainerType>
    {
        public void Configure(EntityTypeBuilder<BookingContainerType> builder)
        {
            builder.ToTable("BookingContainerTypes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Booking).WithMany(x => x.BookingContainerTypes).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.CntOwnTypeId);
        }
    }

    public class BookingContainerTypeChargeConfiguration : IEntityTypeConfiguration<BookingContainerTypeCharge>
    {
        public void Configure(EntityTypeBuilder<BookingContainerTypeCharge> builder)
        {
            builder.ToTable("BookingContainerTypeCharges", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.BookingContainerType).WithMany().HasForeignKey(x => x.BookingContainerTypeId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.PaymentAtCountry).WithMany().HasForeignKey(x => x.PaymentCountryId);
            builder.HasOne(x => x.PayableToParty).WithMany().HasForeignKey(x => x.PayableToPartyId);
        }
    }
    public class BookingPartyConfiguration : IEntityTypeConfiguration<BookingParty>
    {
        public void Configure(EntityTypeBuilder<BookingParty> builder)
        {
            builder.ToTable("BookingParty", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Booking).WithMany(x => x.BookingParties).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.PartyRoleType).WithMany().HasForeignKey(x => x.PartyRoleTypeId);
        }
    }
    public class ExportDocsReceiptConfiguration : IEntityTypeConfiguration<ExportDocsReceipt>
    {
        public void Configure(EntityTypeBuilder<ExportDocsReceipt> builder)
        {
            builder.ToTable("ExportDocsReceipts", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.ReceiptNo).HasMaxLength(30);
            builder.Property(x => x.PersonPersonName).HasMaxLength(100);//شخص تحویل دهنده

            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.ConfirmUser).WithMany().HasForeignKey(x => x.ConfirmUserId);

        }
    }

    public class ExportDocsReceiptItemConfiguration : IEntityTypeConfiguration<ExportDocsReceiptItem>
    {
        public void Configure(EntityTypeBuilder<ExportDocsReceiptItem> builder)
        {
            builder.ToTable("ExportDocsReceiptItems", "ops");

            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Receipt).WithMany(x => x.Items).HasForeignKey(x => x.ExportDocsReceiptId);
            builder.HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId);
        }
    }
    public class ExportDocsReceiptContainerConfiguration : IEntityTypeConfiguration<ExportDocsReceiptContainer>
    {
        public void Configure(EntityTypeBuilder<ExportDocsReceiptContainer> builder)
        {
            builder.ToTable("ExportDocsReceiptContainers", "ops");

            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Receipt).WithMany(x => x.Containers).HasForeignKey(x => x.ExportDocsReceiptId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.ContainerUse).WithMany().HasForeignKey(x => x.ContainerUseId);
        }
    }

    public class RTSInstructionConfiguration : IEntityTypeConfiguration<RTSInstruction>
    {
        public void Configure(EntityTypeBuilder<RTSInstruction> builder)
        {
            builder.ToTable("RTSInstructions", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.RTSInstructionNo).HasMaxLength(40);
            builder.Property(x => x.CancelationReason).HasMaxLength(200);

            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.Depot).WithMany().HasForeignKey(x => x.DepotId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.Shipper).WithMany().HasForeignKey(x => x.ShipperId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);

            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.ShipmentOperationPlan).WithMany().HasForeignKey(x => x.ShipmentOperationPlanId);

            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.EditUserId);

        }
    }
    public class RTSInstructionContainerTypeConfiguration : IEntityTypeConfiguration<RTSInstructionContainerType>
    {
        public void Configure(EntityTypeBuilder<RTSInstructionContainerType> builder)
        {
            builder.ToTable("RTSInstructionContainerTypes", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.RTSInstruction).WithMany(x => x.RTSInstructionContainerTypes).HasForeignKey(x => x.RTSInstructionId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
        }
    }

    public class RTSInstructionContainerConfiguration : IEntityTypeConfiguration<RTSInstructionContainer>
    {
        public void Configure(EntityTypeBuilder<RTSInstructionContainer> builder)
        {
            builder.ToTable("RTSInstructionContainers", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ContainerNumber).HasMaxLength(12);

            builder.HasOne(x => x.RTSInstruction).WithMany().HasForeignKey(x => x.RTSInstructionId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
        }
    }

    public class BLConfiguration : IEntityTypeConfiguration<BL>
    {
        public void Configure(EntityTypeBuilder<BL> builder)
        {
            builder.ToTable("BLs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.BLNo).HasMaxLength(50);
            builder.Property(x => x.MasterBLNo).HasMaxLength(50);
            builder.Property(x => x.PORText).HasMaxLength(50);
            builder.Property(x => x.POLText).HasMaxLength(50);
            builder.Property(x => x.PODText).HasMaxLength(50);
            builder.Property(x => x.PODelText).HasMaxLength(50);
            builder.Property(x => x.PODelText).HasMaxLength(50);
            builder.Property(x => x.FreightTerms).HasMaxLength(50);
            builder.Property(x => x.LCBankName).HasMaxLength(200);
            builder.Property(x => x.ExportReference).HasMaxLength(35);
            builder.Property(x => x.GoodsDescription).HasMaxLength(2000);
            builder.Property(x => x.PackageTypeText).HasMaxLength(50);

            builder.Property(x => x.VolumeCBM).HasPrecision(18, 6);

            builder.Property(x => x.PlaceOfIssue).HasMaxLength(100);
            builder.Property(x => x.ForwardingAgentReference).HasMaxLength(100);
            builder.Property(x => x.SerialNo).HasMaxLength(20);
            builder.Property(x => x.FinalDestinationText).HasMaxLength(70);
            builder.Property(x => x.FreightPaymentAtText).HasMaxLength(70);
            builder.Property(x => x.PointandCountryOfOriginText).HasMaxLength(70);
            builder.Property(x => x.OnwardInlandRoutingText).HasMaxLength(70);
            builder.Property(x => x.MarksAndNumbers).HasMaxLength(200);
            builder.Property(x => x.RemarksInBL).HasMaxLength(200);
            builder.Property(x => x.RemarksInManifest).HasMaxLength(200);
            builder.Property(x => x.MRN).HasMaxLength(40);
            builder.Property(x => x.PursuitCode).HasMaxLength(50);
            builder.Property(x => x.LCNo).HasMaxLength(50);
            builder.Property(x => x.OrderRegistrationNo).HasMaxLength(50);
            builder.Property(x => x.ClauseRemarks).HasMaxLength(100);
            builder.Property(x => x.OnBoardNotation).HasMaxLength(100);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.POR).WithMany().HasForeignKey(x => x.PORId);
            builder.HasOne(x => x.POL).WithMany().HasForeignKey(x => x.POLId);
            builder.HasOne(x => x.POD).WithMany().HasForeignKey(x => x.PODId);
            builder.HasOne(x => x.PODel).WithMany().HasForeignKey(x => x.PODelId);
            builder.HasOne(x => x.TranshipmentPort).WithMany().HasForeignKey(x => x.TranshipmentPortId);
            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);

            builder.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId);
            builder.HasOne(x => x.MovementType).WithMany().HasForeignKey(x => x.MovementTypeId);
            builder.HasOne(x => x.LoadType).WithMany().HasForeignKey(x => x.LoadTypeId);
            builder.HasOne(x => x.BLReleaseType).WithMany().HasForeignKey(x => x.BLReleaseTypeId);
            builder.HasOne(x => x.BLStatus).WithMany().HasForeignKey(x => x.BLStatusId);
            builder.HasOne(x => x.SalesPerson).WithMany().HasForeignKey(x => x.SalesPersonId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
            builder.HasOne(x => x.BLLevel).WithMany().HasForeignKey(x => x.BLLevelId);
            builder.HasOne(x => x.IssuerParty).WithMany().HasForeignKey(x => x.IssuerPartyId);
            builder.HasOne(x => x.BLType).WithMany().HasForeignKey(x => x.BLTypeId);
            builder.HasOne(x => x.FreightPaymentAt).WithMany().HasForeignKey(x => x.FreightPaymentAtId);
            builder.HasOne(x => x.Incoterm).WithMany().HasForeignKey(x => x.IncotermId);

            builder.HasOne(x => x.POLDetentionTariff).WithMany().HasForeignKey(x => x.POLDetentionTariffId);
            builder.HasOne(x => x.PODDetentionTariff).WithMany().HasForeignKey(x => x.PODDetentionTariffId);

            builder.HasOne(x => x.MasterPOLDetentionTariff).WithMany().HasForeignKey(x => x.MasterPOLDetentionTariffId);
            builder.HasOne(x => x.MasterPODDetentionTariff).WithMany().HasForeignKey(x => x.MasterPODDetentionTariffId);

            builder.HasMany(x => x.BLContainers)
                       .WithOne(x => x.BL)
                       .HasForeignKey(x => x.BLId)
                       .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(e => e.BLNo).HasDatabaseName("IX_BLs_BLNo").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.MasterBLNo).HasDatabaseName("IX_BLs_MasterBLNo").IsUnique(false).IsClustered(false);




        }
    }

    public class ShipmentBLConfiguration : IEntityTypeConfiguration<ShipmentBL>
    {
        public void Configure(EntityTypeBuilder<ShipmentBL> builder)
        {
            builder.ToTable("ShipmentBLs", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
        }
    }





    public class BLLinkConfiguration : IEntityTypeConfiguration<BLLink>
    {
        public void Configure(EntityTypeBuilder<BLLink> builder)
        {
            builder.ToTable("BLLinks", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.FromBL).WithMany().HasForeignKey(x => x.FromBLId);
            builder.HasOne(x => x.ToBL).WithMany().HasForeignKey(x => x.ToBLId);
            builder.HasOne(x => x.BLLinkType).WithMany().HasForeignKey(x => x.BLLinkTypeId);
        }
    }




    public class BLCargoAllocationConfiguration : IEntityTypeConfiguration<BLCargoAllocation>
    {
        public void Configure(EntityTypeBuilder<BLCargoAllocation> builder)
        {
            builder.ToTable("BLCargoAllocations", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.BL).WithMany(x => x.CargoAllocations).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.CargoDetail).WithMany().HasForeignKey(x => x.CargoDetailId);
            builder.HasOne(x => x.ShipmentCargo).WithMany().HasForeignKey(x => x.ShipmentCargoId);
        }
    }

    public class BLChangeConfiguration : IEntityTypeConfiguration<BLChange>
    {
        public void Configure(EntityTypeBuilder<BLChange> builder)
        {
            builder.ToTable("BLChanges", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.OldBL).WithMany().HasForeignKey(x => x.OldBLId);
            builder.HasOne(x => x.NewBL).WithMany().HasForeignKey(x => x.NewBLId);
            builder.HasOne(x => x.BLChangeType).WithMany().HasForeignKey(x => x.BLChangeTypeId);

        }
    }

    public class BLEndorsementConfiguration : IEntityTypeConfiguration<BLEndorsement>
    {
        public void Configure(EntityTypeBuilder<BLEndorsement> builder)
        {
            builder.ToTable("BLEndorsements", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.EndorserName).HasMaxLength(200);
            builder.Property(x => x.EndorseeName).HasMaxLength(200);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.EndorsementType).WithMany().HasForeignKey(x => x.EndorsementTypeId);
            builder.HasOne(x => x.Endorser).WithMany().HasForeignKey(x => x.EndorserId);
            builder.HasOne(x => x.Endorsee).WithMany().HasForeignKey(x => x.EndorseeId);

        }
    }





    public class BLChargeConfiguration : IEntityTypeConfiguration<BLCharge>
    {
        public void Configure(EntityTypeBuilder<BLCharge> builder)
        {
            builder.ToTable("BLCharges", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.BL).WithMany(x => x.BLCharges).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.PaymentType).WithMany().HasForeignKey(x => x.PaymentTypeId);
            builder.HasOne(x => x.PaymentCountry).WithMany().HasForeignKey(x => x.PaymentCountryId);
            builder.HasOne(x => x.PaymentAgent).WithMany().HasForeignKey(x => x.PaymentAgentId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.BLOperation).WithMany().HasForeignKey(x => x.BLOperationId);
            builder.HasOne(x => x.CalculationBy).WithMany().HasForeignKey(x => x.CalculationById);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);


        }
    }

    public class BLContainerConfiguration : IEntityTypeConfiguration<BLContainer>
    {
        public void Configure(EntityTypeBuilder<BLContainer> builder)
        {
            builder.ToTable("BLContainers", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ContainerNumber).HasMaxLength(12);
            builder.Property(x => x.SealNumber).HasMaxLength(20);
            builder.Property(x => x.SealNumber2).HasMaxLength(20);
            builder.Property(x => x.ExportPermissionNo).HasMaxLength(50);

            builder.Property(x => x.TotalVolumeCBM).HasPrecision(18, 6);



            builder.HasOne(x => x.BL).WithMany(x => x.BLContainers).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.ContainerUse).WithMany().HasForeignKey(x => x.ContainerUseId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.DimensionUnit).WithMany().HasForeignKey(x => x.DimensionUnitId);
            builder.HasOne(x => x.FullMT).WithMany().HasForeignKey(x => x.FullMTId);
            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.CntOwnTypeId);
            builder.HasOne(x => x.LoadType).WithMany().HasForeignKey(x => x.LoadTypeId);

            builder.HasIndex(e => e.ContainerNumber).HasDatabaseName("IX_BLContainers_ContainerNumber").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.SealNumber).HasDatabaseName("IX_BLContainers_SealNumber").IsUnique(false).IsClustered(false);
        }
    }
    public class BLContainerCargoAllocationConfiguration : IEntityTypeConfiguration<BLContainerCargoAllocation>
    {
        public void Configure(EntityTypeBuilder<BLContainerCargoAllocation> builder)
        {
            builder.ToTable("BLContainerCargoAllocations", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.BLContainer).WithMany(x => x.CargoAllocations).HasForeignKey(x => x.BLContainerId);
            builder.HasOne(x => x.CargoDetail).WithMany().HasForeignKey(x => x.CargoDetailId);
        }
    }
    public class BLPartyConfiguration : IEntityTypeConfiguration<BLParty>
    {
        public void Configure(EntityTypeBuilder<BLParty> builder)
        {
            builder.ToTable("BLParty", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.PartyTextInBL).HasMaxLength(500);
            builder.Property(x => x.PartyNationalNo).HasMaxLength(25);
            builder.Property(x => x.PartyTelephone).HasMaxLength(35);
            builder.Property(x => x.PartyMobile).HasMaxLength(15);
            builder.Property(x => x.PartyEmail).HasMaxLength(45);
            builder.Property(x => x.PartyFax).HasMaxLength(25);



            builder.HasOne(x => x.BL).WithMany(x => x.BLParties).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.PartyRoleType).WithMany().HasForeignKey(x => x.RoleTypeId);

        }
    }
    public class BLFlightConfiguration : IEntityTypeConfiguration<BLFlight>
    {
        public void Configure(EntityTypeBuilder<BLFlight> builder)
        {
            builder.ToTable("BLFlights", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.FlightNo).HasMaxLength(60);
            builder.Property(x => x.DeclaredValueForCarriage).HasMaxLength(100);
            builder.Property(x => x.DeclaredValueForCustoms).HasMaxLength(100);
            builder.Property(x => x.HandlingInformation).HasMaxLength(4000);

            builder.HasOne(x => x.BL).WithMany(x => x.BLFlights).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Flight).WithMany().HasForeignKey(x => x.FlightId);
            builder.HasOne(x => x.FlightDate).WithMany().HasForeignKey(x => x.FlightDateId);

        }
    }
    public class BLOperationConfiguration : IEntityTypeConfiguration<BLOperation>
    {
        public void Configure(EntityTypeBuilder<BLOperation> builder)
        {
            builder.ToTable("BLOperations", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.OperationType).WithMany().HasForeignKey(x => x.OperationTypeId);

        }
    }
    public class BLFollowUpConfiguration : IEntityTypeConfiguration<BLFollowUp>
    {
        public void Configure(EntityTypeBuilder<BLFollowUp> builder)
        {
            builder.ToTable("BLFollowUps", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.FollowUpQuestion).HasMaxLength(200);
            builder.Property(x => x.FollowUpAnswer).HasMaxLength(200);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
        }
    }
    public class CarrierBLTypeConfiguration : IEntityTypeConfiguration<CarrierBLType>
    {
        public void Configure(EntityTypeBuilder<CarrierBLType> builder)
        {
            builder.ToTable("CarrierBLTypes", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CarrierBLTypeName).HasMaxLength(50);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
        }
    }

    public class BLSerialSeriesConfiguration : IEntityTypeConfiguration<BLSerialSeries>
    {
        public void Configure(EntityTypeBuilder<BLSerialSeries> builder)
        {
            builder.ToTable("BLSerialSeries", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.SeriesCode).IsRequired().HasMaxLength(35);
            builder.Property(x => x.SeriesName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Prefix).HasMaxLength(20);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.CarrierBLType).WithMany().HasForeignKey(x => x.CarrierBLTypeId);
        }
    }
    public class BLSerialBatchAssignConfiguration : IEntityTypeConfiguration<BLSerialBatchAssign>
    {
        public void Configure(EntityTypeBuilder<BLSerialBatchAssign> builder)
        {
            builder.ToTable("BLSerialBatchAssigns", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(20);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.BLSerialSeries).WithMany().HasForeignKey(x => x.BLSerialSeriesId);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
        }
    }
    public class BLSerialConfiguration : IEntityTypeConfiguration<BLSerial>
    {
        public void Configure(EntityTypeBuilder<BLSerial> builder)
        {
            builder.ToTable("BLSerials", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.BLSerialSeries).WithMany().HasForeignKey(x => x.BLSerialSeriesId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
        }
    }
    public class BLSurrenderConfiguration : IEntityTypeConfiguration<BLSurrender>
    {
        public void Configure(EntityTypeBuilder<BLSurrender> builder)
        {
            builder.ToTable("BLSurrenders", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.BLType).WithMany().HasForeignKey(x => x.BLTypeId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BLType).WithMany().HasForeignKey(x => x.BLTypeId);
            builder.HasOne(x => x.ConfirmByUser).WithMany().HasForeignKey(x => x.ConfirmByUserId);
        }
    }

    public class BLTripConfiguration : IEntityTypeConfiguration<BLTrip>
    {
        public void Configure(EntityTypeBuilder<BLTrip> builder)
        {
            builder.ToTable("BLTrips", "ops");
            builder.Property(x => x.TrailerNo).HasMaxLength(50);
            builder.Property(x => x.TruckNo).HasMaxLength(50);
            builder.Property(x => x.DriverName).HasMaxLength(50);
            builder.Property(x => x.DriverPassportNo).HasMaxLength(50);
            builder.Property(x => x.CMRNo).HasMaxLength(50);
            builder.Property(x => x.TIRNo).HasMaxLength(50);
            builder.Property(x => x.ManifestNo).HasMaxLength(50);
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.BL).WithMany(x => x.BLTrips).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Trip).WithMany().HasForeignKey(x => x.TripId);
        }
    }

    public class BLContainerTripAllocationConfiguration : IEntityTypeConfiguration<BLContainerTripAllocation>
    {
        public void Configure(EntityTypeBuilder<BLContainerTripAllocation> builder)
        {
            builder.ToTable("BLContainerTripAllocations", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.BLTrip).WithMany(x => x.AllocatedContainers).HasForeignKey(x => x.BLTripId);
            builder.HasOne(x => x.BLContainer).WithMany().HasForeignKey(x => x.BLContainerId);
        }
    }




    public class BLRailJourneyConfiguration : IEntityTypeConfiguration<BLRailJourney>
    {
        public void Configure(EntityTypeBuilder<BLRailJourney> builder)
        {
            builder.ToTable("BLRailJourneys", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.WagonNo).HasMaxLength(50);
            builder.Property(x => x.TrainNo).HasMaxLength(50);

            builder.HasOne(x => x.BL).WithMany(x => x.BLRailJourneys).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.DepartureStation).WithMany().HasForeignKey(x => x.DepartureStationId);
            builder.HasOne(x => x.ArrivalStation).WithMany().HasForeignKey(x => x.ArrivalStationId);
        }
    }




    public class BLVoyageConfiguration : IEntityTypeConfiguration<BLVoyage>
    {
        public void Configure(EntityTypeBuilder<BLVoyage> builder)
        {
            builder.ToTable("BLVoyages", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.VoyageNo).HasMaxLength(25);
            builder.Property(x => x.VesselName).HasMaxLength(30);

            builder.HasOne(x => x.BL).WithMany(x => x.BLVoyages).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Voyage).WithMany().HasForeignKey(x => x.VoyageId);
            builder.HasOne(x => x.VoyageType).WithMany().HasForeignKey(x => x.VoyageTypeId);
        }
    }

    public class CargoDetailConfiguration : IEntityTypeConfiguration<CargoDetail>
    {
        public void Configure(EntityTypeBuilder<CargoDetail> builder)
        {
            builder.ToTable("CargoDetails", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.HSCodeToSave).HasMaxLength(12);
            builder.Property(x => x.MarksAndNumbers).HasMaxLength(500);
            builder.Property(x => x.CargoDescription).HasMaxLength(1000);
            builder.Property(x => x.UNNumber).HasMaxLength(30);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            //New
            builder.Property(x => x.PSN).HasMaxLength(100);
            builder.Property(x => x.MarinePolutant).HasMaxLength(10);
            builder.Property(x => x.MSDSPageNumber).HasMaxLength(10);

            builder.Property(x => x.Volume).HasPrecision(18, 6);
            builder.HasOne(x => x.BL).WithMany(x => x.CargoDetails).HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.CountryHSCode).WithMany().HasForeignKey(x => x.CountryHSCodeId);
            builder.HasOne(x => x.RateRequest).WithMany(x => x.CargoDetails).HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.Job).WithMany(x => x.CargoDetails).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Booking).WithMany(x => x.CargoDetails).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.BookingLeg).WithMany(x => x.CargoDetails).HasForeignKey(x => x.BookingLegId);
            builder.HasOne(x => x.BookingContainerType).WithMany().HasForeignKey(x => x.BookingContainerTypeId);
            builder.HasOne(x => x.WeightUnit).WithMany().HasForeignKey(x => x.WeightUnitId);
            builder.HasOne(x => x.VolumeUnit).WithMany().HasForeignKey(x => x.VolumeUnitId);
            builder.HasOne(x => x.DimensionUnit).WithMany().HasForeignKey(x => x.DimensionUnitId);
            builder.HasOne(x => x.DGPackingGroup).WithMany().HasForeignKey(x => x.DGPackingGroupId);
            builder.HasOne(x => x.DGClass).WithMany().HasForeignKey(x => x.DGClassId);
            builder.HasOne(x => x.TempUnit).WithMany().HasForeignKey(x => x.TempUnitId);
            builder.HasOne(x => x.CargoCommodity).WithMany().HasForeignKey(x => x.CargoCommodityId);
            builder.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId);
            builder.HasOne(x => x.InnerPackageType).WithMany().HasForeignKey(x => x.InnerPackageTypeId);
        }
    }

    public class CustomsReferenceConfiguration : IEntityTypeConfiguration<CustomsReference>
    {
        public void Configure(EntityTypeBuilder<CustomsReference> builder)
        {
            builder.ToTable("CustomsReferences", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ProcedureType).HasMaxLength(12);
            builder.Property(x => x.MRN).HasMaxLength(45);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
            builder.HasOne(x => x.CustomsClearance).WithMany().HasForeignKey(x => x.CustomsClearanceId);

        }
    }

    public class DeliveryOrderConfiguration : IEntityTypeConfiguration<DeliveryOrder>
    {
        public void Configure(EntityTypeBuilder<DeliveryOrder> builder)
        {
            builder.ToTable("DeliveryOrders", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DONo).HasMaxLength(25);
            builder.Property(x => x.DeliveryLocation).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.CollectedByPerson).HasMaxLength(200);
            builder.Property(x => x.ReleaseComments).HasMaxLength(1000);
            builder.Property(x => x.ExternalIssuerText).HasMaxLength(100);
            builder.Property(x => x.CancellationReason).HasMaxLength(1000);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.ClearanceType).WithMany().HasForeignKey(x => x.ClearanceTypeId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.CarrierAgent).WithMany().HasForeignKey(x => x.CarrierAgentId);
            builder.HasOne(x => x.BLReleaseType).WithMany().HasForeignKey(x => x.BLReleaseTypeId);
            builder.HasOne(x => x.TelexMessage).WithMany().HasForeignKey(x => x.TelexMessageId);
            builder.HasOne(x => x.CustomsOffice).WithMany().HasForeignKey(x => x.CustomsOfficeId);
            builder.HasOne(x => x.DischargePort).WithMany().HasForeignKey(x => x.DischargePortId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            //builder.HasOne(x => x.IssuedToPerson).WithMany().HasForeignKey(x => x.IssuedToPersonId);
            builder.HasOne(x => x.ReleasedByUser).WithMany().HasForeignKey(x => x.ReleasedByUserId);
            builder.HasOne(x => x.DOOperationalMode).WithMany().HasForeignKey(x => x.DOOperationalModeId);
            builder.HasOne(x => x.ExternalIssuer).WithMany().HasForeignKey(x => x.ExternalIssuerId);
            builder.HasOne(x => x.ReceivedByUser).WithMany().HasForeignKey(x => x.ReceivedByUserId);
            builder.HasOne(x => x.VerifiedByUser).WithMany().HasForeignKey(x => x.VerifiedByUserId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.EditedByUser).WithMany().HasForeignKey(x => x.EditUserId);
            builder.HasOne(x => x.DeliveryMethod).WithMany().HasForeignKey(x => x.DeliveryMethodId);
            builder.HasOne(x => x.ReceiveMethod).WithMany().HasForeignKey(x => x.ReceiveMethodId);
            builder.HasOne(x => x.CanceledByUser).WithMany().HasForeignKey(x => x.CancelUserId);

            builder.HasIndex(x => new { x.CompanyId, x.Serial, x.DOOperationalModeId })
                       .IsUnique()
                       .HasDatabaseName("IX_DeliveryOrders_CompanyId_Serial_DOOperationalModeId");
        }
    }

    public class DeliveryOrderContainerConfiguration : IEntityTypeConfiguration<DeliveryOrderContainer>
    {
        public void Configure(EntityTypeBuilder<DeliveryOrderContainer> builder)
        {
            builder.ToTable("DeliveryOrderContainers", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ContainerNumber).IsRequired().HasMaxLength(12);
            builder.Property(x => x.SealNumber).IsRequired().HasMaxLength(30);

            builder.HasOne(x => x.DeliveryOrder).WithMany(x => x.Containers).HasForeignKey(x => x.DeliveryOrderId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.BLContainer).WithMany().HasForeignKey(x => x.BLContainerId);
            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.cntOwnTypeId);
        }
    }
    public class DeliveryOrderContainerPackageConfiguration : IEntityTypeConfiguration<DeliveryOrderContainerPackage>
    {
        public void Configure(EntityTypeBuilder<DeliveryOrderContainerPackage> builder)
        {
            builder.ToTable("DeliveryOrderContainerPackages", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.HSCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.MarksAndNumbers).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DescriptionOfGoods).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.VolumeCBM).HasPrecision(18, 6);
            builder.HasOne(x => x.DeliveryOrderContainer).WithMany(x => x.Packages).HasForeignKey(x => x.DeliveryOrderContainerId);
            builder.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId);
        }
    }
    public class ENSDeclarationConfiguration : IEntityTypeConfiguration<ENSDeclaration>
    {
        public void Configure(EntityTypeBuilder<ENSDeclaration> builder)
        {
            builder.ToTable("ENSDeclarations", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.MRN).IsRequired().HasMaxLength(30);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.ENSDeclarationStatus).WithMany().HasForeignKey(x => x.ENSDeclarationStatusId);
            builder.HasOne(x => x.FirstEntryPort).WithMany().HasForeignKey(x => x.FirstEntryPortId);
            builder.HasOne(x => x.FirstEntryBorder).WithMany().HasForeignKey(x => x.FirstEntryBorderId);

        }
    }
    public class EPLGrantDOConfiguration : IEntityTypeConfiguration<EPLGrantDO>
    {
        public void Configure(EntityTypeBuilder<EPLGrantDO> builder)
        {
            builder.ToTable("EPLGrantDOs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.BarFarabaranCode).HasMaxLength(30);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        }
    }

    public class GuaranteePlanConfiguration : IEntityTypeConfiguration<GuaranteePlan>
    {
        public void Configure(EntityTypeBuilder<GuaranteePlan> builder)
        {
            builder.ToTable("GuaranteePlans", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.LetterNo).HasMaxLength(30);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.BLNo).HasMaxLength(40);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);
            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.EditUserId);
        }
    }
    public class HologramSeriesConfiguration : IEntityTypeConfiguration<HologramSeries>
    {
        public void Configure(EntityTypeBuilder<HologramSeries> builder)
        {
            builder.ToTable("HologramSeries", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.BLSerialSeries).WithMany().HasForeignKey(x => x.BLSerialSeriesId);
        }
    }
    public class HologramConfiguration : IEntityTypeConfiguration<Hologram>
    {
        public void Configure(EntityTypeBuilder<Hologram> builder)
        {
            builder.ToTable("Holograms", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.HologramNumber).HasMaxLength(15);
            builder.Property(x => x.EncryptedSerial).HasMaxLength(50);
            builder.Property(x => x.SerialHash).HasMaxLength(50);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BLSerial).WithMany().HasForeignKey(x => x.BLSerialId);
            builder.HasOne(x => x.HologramSeries).WithMany().HasForeignKey(x => x.HologramSeriesId);
            builder.HasOne(x => x.HologramStatus).WithMany().HasForeignKey(x => x.HologramStatusId);
        }
    }

    public class TelexReleaseRequestConfiguration : IEntityTypeConfiguration<TelexReleaseRequest>
    {
        public void Configure(EntityTypeBuilder<TelexReleaseRequest> builder)
        {
            builder.ToTable("TelexReleaseRequests", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.RequestNo).HasMaxLength(30);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.TelexReleaseStatus).WithMany().HasForeignKey(x => x.TelexReleaseStatusId);
            builder.HasOne(x => x.BLSurrender).WithMany().HasForeignKey(x => x.BLSurrenderId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.ShippingLine).WithMany().HasForeignKey(x => x.ShippingLineId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.LineForwarderType).WithMany().HasForeignKey(x => x.LineForwarderTypeId);
        }
    }

    public class TelexMessageConfiguration : IEntityTypeConfiguration<TelexMessage>
    {
        public void Configure(EntityTypeBuilder<TelexMessage> builder)
        {
            builder.ToTable("TelexMessages", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.Recipient).HasMaxLength(200);
            builder.Property(x => x.Subject).HasMaxLength(200);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.TelexReleaseRequest).WithMany().HasForeignKey(x => x.TelexReleaseRequestId);
            builder.HasOne(x => x.SendToolType).WithMany().HasForeignKey(x => x.SendToolTypeId);
        }
    }


    public class TIPUsageConfiguration : IEntityTypeConfiguration<TIPUsage>
    {
        public void Configure(EntityTypeBuilder<TIPUsage> builder)
        {
            builder.ToTable("TIPUsages", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.LetterNo).IsRequired().HasMaxLength(35);
            builder.Property(x => x.BLNo).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.TIP).WithMany().HasForeignKey(x => x.TIPId);
        }
    }

    public class TIPUsageContainerConfiguration : IEntityTypeConfiguration<TIPUsageContainer>
    {
        public void Configure(EntityTypeBuilder<TIPUsageContainer> builder)
        {
            builder.ToTable("TIPUsageContainers", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ContainerNumber).IsRequired().HasMaxLength(12);

            builder.HasOne(x => x.TIPUsage).WithMany().HasForeignKey(x => x.TIPUsageId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.SizeType).WithMany().HasForeignKey(x => x.SizeTypeId);
        }
    }





    public class RateConfiguration : IEntityTypeConfiguration<Rate>
    {
        public void Configure(EntityTypeBuilder<Rate> builder)
        {
            builder.ToTable("Rates", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RateTitle).HasMaxLength(100);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
            builder.HasOne(x => x.LoadType).WithMany().HasForeignKey(x => x.LoadTypeId);
        }
    }
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.ToTable("Routes", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RouteName).HasMaxLength(70);

            builder.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId);
            builder.HasOne(x => x.Destination).WithMany().HasForeignKey(x => x.DestinationId);
            builder.HasOne(x => x.RouteMode).WithMany().HasForeignKey(x => x.RouteModeId);
        }
    }

    public class ProvidedRateConfiguration : IEntityTypeConfiguration<ProvidedRate>
    {
        public void Configure(EntityTypeBuilder<ProvidedRate> builder)
        {
            builder.ToTable("ProvidedRates", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
        }
    }

    public class ProvidedRateRouteConfiguration : IEntityTypeConfiguration<ProvidedRateRoute>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRoute> builder)
        {
            builder.ToTable("ProvidedRateRoutes", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.ProvidedRate).WithMany().HasForeignKey(x => x.ProvidedRateId);
            builder.HasOne(x => x.Route).WithMany().HasForeignKey(x => x.RouteId);
        }
    }
    public class ProvidedRateRouteContainerConfiguration : IEntityTypeConfiguration<ProvidedRateRouteContainer>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteContainer> builder)
        {
            builder.ToTable("ProvidedRateRouteContainers", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.ProvidedRateRoute).WithMany().HasForeignKey(x => x.ProvidedRateRouteId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
        }
    }
    public class ProvidedRateRouteContainerChargeConfiguration : IEntityTypeConfiguration<ProvidedRateRouteContainerCharge>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteContainerCharge> builder)
        {
            builder.ToTable("ProvidedRateRouteContainerCharges", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.ProvidedRateRouteContainer).WithMany().HasForeignKey(x => x.ProvidedRateRouteContainerId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

        }
    }


    public class ProvidedRateRouteAirConfiguration : IEntityTypeConfiguration<ProvidedRateRouteAir>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteAir> builder)
        {
            builder.ToTable("ProvidedRateRouteAirs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);


            builder.HasOne(x => x.ProvidedRateRoute).WithMany().HasForeignKey(x => x.ProvidedRateRouteId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }

    public class ProvidedRateRouteAirTierConfiguration : IEntityTypeConfiguration<ProvidedRateRouteAirTier>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteAirTier> builder)
        {
            builder.ToTable("ProvidedRateRouteAirTiers", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.ProvidedRateRouteAir).WithMany().HasForeignKey(x => x.ProvidedRateRouteAirId);
        }
    }
    public class ProvidedRateRouteAirTierChargeConfiguration : IEntityTypeConfiguration<ProvidedRateRouteAirTierCharge>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteAirTierCharge> builder)
        {
            builder.ToTable("ProvidedRateRouteAirTierCharges", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.ProvidedRateRouteAirTier).WithMany().HasForeignKey(x => x.ProvidedRateRouteAirTierId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
        }
    }

    public class ProvidedRateRouteFTLConfiguration : IEntityTypeConfiguration<ProvidedRateRouteFTL>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteFTL> builder)
        {
            builder.ToTable("ProvidedRateRouteFTLs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);


            builder.HasOne(x => x.ProvidedRateRoute).WithMany().HasForeignKey(x => x.ProvidedRateRouteId);
            //builder.HasOne(x => x.Truckinmg).WithMany().HasForeignKey(x => x.ChargeTypeId);
        }
    }
    public class ProvidedRateRouteFTLChargeConfiguration : IEntityTypeConfiguration<ProvidedRateRouteFTLCharge>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteFTLCharge> builder)
        {
            builder.ToTable("ProvidedRateRouteFTLCharges", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.ProvidedRateRouteFTL).WithMany().HasForeignKey(x => x.ProvidedRateRouteFTLId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);

        }
    }
    public class ProvidedRateRouteLTLConfiguration : IEntityTypeConfiguration<ProvidedRateRouteLTL>
    {
        public void Configure(EntityTypeBuilder<ProvidedRateRouteLTL> builder)
        {
            builder.ToTable("ProvidedRateRouteLTLs", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.ProvidedRateRoute).WithMany().HasForeignKey(x => x.ProvidedRateRouteId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.MeasurementUnit).WithMany().HasForeignKey(x => x.MeasurementUnitId);

        }
    }

    public class RateCountryPortConfiguration : IEntityTypeConfiguration<RateCountryPort>
    {
        public void Configure(EntityTypeBuilder<RateCountryPort> builder)
        {
            builder.ToTable("RateCountryPorts", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);


            builder.HasOne(x => x.Rate).WithMany().HasForeignKey(x => x.RateId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
            builder.HasOne(x => x.Port).WithMany().HasForeignKey(x => x.PortId);
            builder.HasOne(x => x.DischargeLoad).WithMany().HasForeignKey(x => x.DischargeLoadId);

        }
    }
    public class RateCountryPortChargeConfiguration : IEntityTypeConfiguration<RateCountryPortCharge>
    {
        public void Configure(EntityTypeBuilder<RateCountryPortCharge> builder)
        {
            builder.ToTable("RateCountryPortCharges", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.RateCountryPort).WithMany().HasForeignKey(x => x.RateCountryPortId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.CalculationType).WithMany().HasForeignKey(x => x.CalculationTypeId);
            builder.HasOne(x => x.CalculationBy).WithMany().HasForeignKey(x => x.CalculationById);

        }
    }
    public class RateCountryPortChargeTierConfiguration : IEntityTypeConfiguration<RateCountryPortChargeTier>
    {
        public void Configure(EntityTypeBuilder<RateCountryPortChargeTier> builder)
        {
            builder.ToTable("RateCountryPortChargeTiers", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.RateCountryPortCharge).WithMany().HasForeignKey(x => x.RateCountryPortChargeId);
        }
    }

    public class RateRequestConfiguration : IEntityTypeConfiguration<RateRequest>
    {
        public void Configure(EntityTypeBuilder<RateRequest> builder)
        {
            builder.ToTable("RateRequests", "ops");
            builder.HasKey(c => c.Id);


            builder.Property(x => x.RequestNumber).IsRequired().HasMaxLength(35);
            builder.Property(x => x.CustomerName).HasMaxLength(70);
            builder.Property(x => x.ContactName).HasMaxLength(50);
            builder.Property(x => x.MobileNo).HasMaxLength(15);
            builder.Property(x => x.EmailAddress).HasMaxLength(50);
            builder.Property(x => x.OriginHSCode).HasMaxLength(12);
            builder.Property(x => x.DestinationHSCode).HasMaxLength(12);
            builder.Property(x => x.OriginZipCode).HasMaxLength(25);
            builder.Property(x => x.DestinationZipCode).HasMaxLength(25);

            builder.Property(x => x.PickupAddress).HasMaxLength(200);
            builder.Property(x => x.DeliveryAddress).HasMaxLength(200);
            builder.Property(x => x.DGDescription).HasMaxLength(200);



            builder.Property(x => x.CargoCommodityDescription).HasMaxLength(200);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId);
            builder.HasOne(x => x.Destination).WithMany().HasForeignKey(x => x.DestinationId);

            builder.HasOne(x => x.CargoCommodity).WithMany().HasForeignKey(x => x.CargoCommodityId);
            builder.HasOne(x => x.QuotationPriority).WithMany().HasForeignKey(x => x.QuotationPriorityId);
            builder.HasOne(x => x.CargoHandlingType).WithMany().HasForeignKey(x => x.CargoHandlingTypeId);
            builder.HasOne(x => x.CargoNature).WithMany().HasForeignKey(x => x.CargoNatureId);
            builder.HasOne(x => x.RateRequestStatus).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.LoadType).WithMany().HasForeignKey(x => x.LoadTypeId);
            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.CustomerContact).WithMany().HasForeignKey(x => x.CustomerContactId);
            builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.HasOne(x => x.SellToCountry).WithMany().HasForeignKey(x => x.SellToCountryId);
            builder.HasOne(x => x.MovementType).WithMany().HasForeignKey(x => x.MovementTypeId);
            builder.HasOne(x => x.RateRequestSource).WithMany().HasForeignKey(x => x.RateRequestSourceId);
            builder.HasOne(x => x.AssignedSalesUser).WithMany().HasForeignKey(x => x.AssignedSalesUserId);
            builder.HasOne(x => x.SalesManager).WithMany().HasForeignKey(x => x.SalesManagerId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.LastEditedByUser).WithMany().HasForeignKey(x => x.EditUserId);
            builder.HasOne(x => x.PaymentType).WithMany().HasForeignKey(x => x.PaymentTypeId);
        }
    }

    public class RateRequestPackageConfiguration : IEntityTypeConfiguration<RateRequestPackage>
    {
        public void Configure(EntityTypeBuilder<RateRequestPackage> builder)
        {
            builder.ToTable("RateRequestPackages", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId);
            builder.HasOne(x => x.RateRequest).WithMany(x => x.Packages).HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.WeightMeasurementUnit).WithMany().HasForeignKey(x => x.WeightMeasurementUnitId);
            builder.HasOne(x => x.LengthMeasurementUnit).WithMany().HasForeignKey(x => x.LengthMeasurementUnitId);
            builder.HasOne(x => x.VolumeMeasurementUnit).WithMany().HasForeignKey(x => x.VolumeMeasurementUnitId);
            builder.HasOne(x => x.Job).WithMany(x => x.Packages).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
        }
    }



    public class RateRequestServiceConfiguration : IEntityTypeConfiguration<RateRequestService>
    {
        public void Configure(EntityTypeBuilder<RateRequestService> builder)
        {
            builder.ToTable("RateRequestServices", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.RateRequest).WithMany(x => x.Services).HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId);
            builder.HasOne(x => x.RateRequestInquiry).WithMany().HasForeignKey(x => x.RateRequestInquiryId);
        }
    }



    //public class RateRequestDeliveryOptionConfiguration : IEntityTypeConfiguration<RateRequestDeliveryOption>
    //{
    //    public void Configure(EntityTypeBuilder<RateRequestDeliveryOption> builder)
    //    {
    //        builder.ToTable("RateRequestDeliveryOptions", "ops");
    //        builder.HasKey(c => c.Id);

    //        builder.Property(x => x.DeliveryAddress).HasMaxLength(300);
    //        builder.Property(x => x.PickupAddress).HasMaxLength(300);
    //        builder.Property(x => x.ZipCodeOrigin).HasMaxLength(40);
    //        builder.Property(x => x.ZipCodeDestination).HasMaxLength(40);

    //        builder.HasOne(x => x.RateRequest).WithMany().HasForeignKey(x => x.RateRequestId);
    //    }
    //}





    public class RateRequestContainerConfiguration : IEntityTypeConfiguration<RateRequestContainer>
    {
        public void Configure(EntityTypeBuilder<RateRequestContainer> builder)
        {
            builder.ToTable("RateRequestContainers", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.RateRequest).WithMany(x => x.Containers).HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.Job).WithMany(x => x.Containers).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.CntOwnType).WithMany().HasForeignKey(x => x.CntOwnTypeId);
        }
    }
    //public class RateRequestFTLConfiguration : IEntityTypeConfiguration<RateRequestFTL>
    //{
    //    public void Configure(EntityTypeBuilder<RateRequestFTL> builder)
    //    {
    //        builder.ToTable("RateRequestFTLs", "ops");
    //        builder.HasKey(c => c.Id);
    //        builder.Property(x => x.Remarks).HasMaxLength(200);

    //        builder.HasOne(x => x.RateRequest).WithMany().HasForeignKey(x => x.RateRequestId);
    //        builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
    //        builder.HasOne(x => x.TotalLengthMeasurement).WithMany().HasForeignKey(x => x.TotalLengthMeasurementId);
    //        builder.HasOne(x => x.TotalWeightMeasurement).WithMany().HasForeignKey(x => x.TotalWeightMeasurementId);
    //    }
    //}

    //public class RateRequestLCLLTLConfiguration : IEntityTypeConfiguration<RateRequestLCLLTL>
    //{
    //    public void Configure(EntityTypeBuilder<RateRequestLCLLTL> builder)
    //    {
    //        builder.ToTable("RateRequestLCLLTLs", "ops");
    //        builder.HasKey(c => c.Id);
    //        builder.Property(x => x.Remarks).HasMaxLength(200);

    //        builder.HasOne(x => x.RateRequest).WithMany().HasForeignKey(x => x.RateRequestId);
    //        builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
    //        builder.HasOne(x => x.UnitLengthMeasurement).WithMany().HasForeignKey(x => x.UnitLengthMeasurementId);
    //        builder.HasOne(x => x.UnitWeightMeasurement).WithMany().HasForeignKey(x => x.UnitWeightMeasurementId);
    //        builder.HasOne(x => x.TotalWeightMeasurement).WithMany().HasForeignKey(x => x.TotalWeightMeasurementId);
    //        builder.HasOne(x => x.TotalVolumeMeasurement).WithMany().HasForeignKey(x => x.TotalVolumeMeasurementId);

    //    }
    //}



    public class RateRequestTransportTypeConfiguration : IEntityTypeConfiguration<RateRequestTransportType>
    {
        public void Configure(EntityTypeBuilder<RateRequestTransportType> builder)
        {
            builder.ToTable("RateRequestTransportTypes", "ops");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.RateRequest).WithMany(x => x.TransportTypes).HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);

        }
    }

    public class RateRequestValidityConfiguration : IEntityTypeConfiguration<RateRequestValidity>
    {
        public void Configure(EntityTypeBuilder<RateRequestValidity> builder)
        {
            builder.ToTable("RateRequestValidity", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.RateRequest).WithMany().HasForeignKey(x => x.RateRequestId);
        }
    }

    public class RateRequestInquiryConfiguration : IEntityTypeConfiguration<RateRequestInquiry>
    {
        public void Configure(EntityTypeBuilder<RateRequestInquiry> builder)
        {
            builder.ToTable("RateRequestInquiry", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InquiryNo).HasMaxLength(30);
            builder.Property(x => x.RejectReason).HasMaxLength(500);
            builder.Property(x => x.RemarksInInquiry).HasMaxLength(500);

            builder.HasOne(x => x.RateRequest).WithMany(x => x.Inquiries).HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.Vendor).WithMany().HasForeignKey(x => x.VendorId);
            builder.HasOne(x => x.RejectedByUser).WithMany().HasForeignKey(x => x.RejectedByUserId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId);
            builder.HasOne(x => x.Destination).WithMany().HasForeignKey(x => x.DestinationId);
            builder.HasOne(x => x.SentByUser).WithMany().HasForeignKey(x => x.SentByUserId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
        }
    }
    public class BuyQuoteConfiguration : IEntityTypeConfiguration<BuyQuote>
    {
        public void Configure(EntityTypeBuilder<BuyQuote> builder)
        {
            builder.ToTable("BuyQuotes", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.OptionsServiceName).HasMaxLength(100);
            builder.Property(x => x.VehicleName).HasMaxLength(50);
            builder.Property(x => x.CarrierName).HasMaxLength(200);
            builder.Property(x => x.VendorName).HasMaxLength(200);
            builder.Property(x => x.TransportMovementNo).HasMaxLength(30);
            builder.Property(x => x.Remarks).HasMaxLength(4000);
            builder.Property(x => x.QuotationNo).IsRequired().HasMaxLength(40);

            builder.HasOne(x => x.RateRequestInquiry).WithMany().HasForeignKey(x => x.RateRequestInquiryId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Vendor).WithMany().HasForeignKey(x => x.VendorId);

            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.EditedByUser).WithMany().HasForeignKey(x => x.EditUserId);

            builder.HasIndex(x => new { x.RateRequestInquiryId, x.CarrierId, x.QuotationNo }).IsUnique();



        }
    }

    public class BuyQuoteItemConfiguration : IEntityTypeConfiguration<BuyQuoteItem>
    {
        public void Configure(EntityTypeBuilder<BuyQuoteItem> builder)
        {
            builder.ToTable("BuyQuoteItems", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.BuyQuote).WithMany(x => x.BuyQuoteItems).HasForeignKey(x => x.BuyQuoteId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

        }
    }


    public class SellQuoteConfiguration : IEntityTypeConfiguration<SellQuote>
    {
        public void Configure(EntityTypeBuilder<SellQuote> builder)
        {
            builder.ToTable("SellQuotes", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ServiceName).HasMaxLength(70);
            builder.Property(x => x.VehicleNames).HasMaxLength(100);
            builder.Property(x => x.MovementNumbers).HasMaxLength(100);
            builder.Property(x => x.Remarks).HasMaxLength(4000);
            builder.Property(x => x.QuotationNo).IsRequired().HasMaxLength(40);

            builder.HasOne(x => x.RateRequest).WithMany().HasForeignKey(x => x.RateRequestId);
            builder.HasOne(x => x.SelectedBuyQuote).WithMany().HasForeignKey(x => x.SelectedBuyQuoteId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.LastEditUser).WithMany().HasForeignKey(x => x.EditUserId);
            builder.HasOne(x => x.SentByUser).WithMany().HasForeignKey(x => x.SentByUserId);
        }
    }

    public class SellQuoteItemConfiguration : IEntityTypeConfiguration<SellQuoteItem>
    {
        public void Configure(EntityTypeBuilder<SellQuoteItem> builder)
        {
            builder.ToTable("SellQuoteItems", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.RemarksToCustomer).HasMaxLength(350);
            builder.Property(x => x.InternalNotes).HasMaxLength(1000);

            builder.HasOne(x => x.SellQuote).WithMany(x => x.Items).HasForeignKey(x => x.SellQuoteId);
            builder.HasOne(x => x.ResponsibleCompany).WithMany().HasForeignKey(x => x.ResponsibleCompanyId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.BuyCurrency).WithMany().HasForeignKey(x => x.BuyCurrencyId);
            builder.HasOne(x => x.SellCurrency).WithMany().HasForeignKey(x => x.SellCurrencyId);
            builder.HasOne(x => x.CalculationBy).WithMany().HasForeignKey(x => x.CalculationById);
            builder.HasOne(x => x.CalculationType).WithMany().HasForeignKey(x => x.CalculationTypeId);
            builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId);
            builder.HasOne(x => x.ExchangeRate).WithMany().HasForeignKey(x => x.ExchangeRateId);
        }
    }

    public class EDIMessageConfiguration : IEntityTypeConfiguration<EDIMessage>
    {
        public void Configure(EntityTypeBuilder<EDIMessage> builder)
        {
            builder.ToTable("EDIMessages", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ReferenceNumber).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.MessageType).WithMany().HasForeignKey(x => x.MessageTypeId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);

        }
    }

    public class AppTaskConfiguration : IEntityTypeConfiguration<AppTask>
    {
        public void Configure(EntityTypeBuilder<AppTask> builder)
        {
            builder.ToTable("AppTasks", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.TaskDesciption).HasMaxLength(500);
            builder.Property(x => x.ResultNotes).HasMaxLength(500);

            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);
            builder.HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId);
            builder.HasOne(x => x.TaskStatus).WithMany().HasForeignKey(x => x.TaskStatusId);
            builder.HasOne(x => x.AssignedByUser).WithMany().HasForeignKey(x => x.AssignedByUserId);
            builder.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId);
            builder.HasOne(x => x.Priority).WithMany().HasForeignKey(x => x.PriorityId);

        }
    }

    public class AppTaskCommentConfiguration : IEntityTypeConfiguration<AppTaskComment>
    {
        public void Configure(EntityTypeBuilder<AppTaskComment> builder)
        {
            builder.ToTable("AppTaskComments", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CommentText).HasMaxLength(1000);

            builder.HasOne(x => x.AppTask).WithMany(X => X.Comments).HasForeignKey(x => x.AppTaskId);
            builder.HasOne(x => x.CommentType).WithMany().HasForeignKey(x => x.CommentTypeId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
        }
    }

    public class AppTaskCheckListConfiguration : IEntityTypeConfiguration<AppTaskCheckList>
    {
        public void Configure(EntityTypeBuilder<AppTaskCheckList> builder)
        {
            builder.ToTable("AppTaskCheckLists", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CheckListItemTitle).HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(200);

            builder.HasOne(x => x.AppTask).WithMany(x => x.CheckLists).HasForeignKey(x => x.AppTaskId);
            builder.HasOne(x => x.CheckedByUser).WithMany().HasForeignKey(x => x.CheckedByUserId);
        }
    }
    public class AppTaskAssignmentConfiguration : IEntityTypeConfiguration<AppTaskAssignment>
    {
        public void Configure(EntityTypeBuilder<AppTaskAssignment> builder)
        {
            builder.ToTable("AppTaskAssignments", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Note).HasMaxLength(500);

            builder.HasOne(x => x.AppTask).WithMany(x => x.Assignments).HasForeignKey(x => x.AppTaskId);
            builder.HasOne(x => x.AssignedToUser).WithMany().HasForeignKey(x => x.AssignedToUserId);
            builder.HasOne(x => x.AssignedByUser).WithMany().HasForeignKey(x => x.AssignedByUserId);
        }
    }

    public class AppTaskReminderConfiguration : IEntityTypeConfiguration<AppTaskReminder>
    {
        public void Configure(EntityTypeBuilder<AppTaskReminder> builder)
        {
            builder.ToTable("AppTaskReminders", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Channel).HasMaxLength(20);
            builder.Property(x => x.Note).HasMaxLength(500);

            builder.HasOne(x => x.AppTask).WithMany(x => x.Reminders).HasForeignKey(x => x.AppTaskId);
        }
    }


    public class AppTaskEscalationConfiguration : IEntityTypeConfiguration<AppTaskEscalation>
    {
        public void Configure(EntityTypeBuilder<AppTaskEscalation> builder)
        {
            builder.ToTable("AppTaskEscalations", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Note).HasMaxLength(500);

            builder.HasOne(x => x.AppTask).WithMany(x => x.Escalations).HasForeignKey(x => x.AppTaskId);
            builder.HasOne(x => x.EscalatedToUser).WithMany().HasForeignKey(x => x.EscalatedToUserId);
            builder.HasOne(x => x.EscalatedByUser).WithMany().HasForeignKey(x => x.EscalatedByUserId);
        }
    }






    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.JobNumber).HasMaxLength(35);
            builder.Property(x => x.JobFileNumber).HasMaxLength(35);
            builder.Property(x => x.IncotermsLocationName).HasMaxLength(35);
            builder.Property(x => x.CancellationReason).HasMaxLength(100);
            builder.Property(x => x.CustomerNotes).HasMaxLength(1000);
            builder.Property(x => x.InternalNotes).HasMaxLength(1000);
            builder.Property(x => x.OperationalNotes).HasMaxLength(1000);

            builder.Property(x => x.VolumeCBM).HasPrecision(18, 6);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.SellQuote).WithMany().HasForeignKey(x => x.SellQuoteId);
            builder.HasOne(x => x.JobStatus).WithMany().HasForeignKey(x => x.JobStatusId);
            builder.HasOne(x => x.JobSourceType).WithMany().HasForeignKey(x => x.JobSourceTypeId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
            builder.HasOne(x => x.CargoHandlingType).WithMany().HasForeignKey(x => x.CargoHandlingTypeId);
            builder.HasOne(x => x.CargoNature).WithMany().HasForeignKey(x => x.CargoNatureId);
            builder.HasOne(x => x.LoadType).WithMany().HasForeignKey(x => x.LoadTypeId);
            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);
            builder.HasOne(x => x.Incoterm).WithMany().HasForeignKey(x => x.IncotermId);
            builder.HasOne(x => x.FreightPaymentType).WithMany().HasForeignKey(x => x.FreightPaymentTypeId);
            builder.HasOne(x => x.SalesPerson).WithMany().HasForeignKey(x => x.SalesUserId);

            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdatedByUser).WithMany().HasForeignKey(x => x.EditUserId);


            builder.HasIndex(e => e.JobFileNumber).HasDatabaseName("IX_Jobs_JobFileNumber").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.JobNumber).HasDatabaseName("IX_Jobs_JobNumber").IsUnique(false).IsClustered(false);

        }
    }
    public class JobRoutingConfiguration : IEntityTypeConfiguration<JobRouting>
    {
        public void Configure(EntityTypeBuilder<JobRouting> builder)
        {
            builder.ToTable("JobRoutings", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.PlaceOfReceiptName).HasMaxLength(70);
            builder.Property(x => x.PlaceOfDeliveryName).HasMaxLength(70);

            builder.HasOne(x => x.Job).WithMany(x => x.Routing).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.MovementType).WithMany().HasForeignKey(x => x.MovementTypeId);
            builder.HasOne(x => x.OriginLocation).WithMany().HasForeignKey(x => x.OriginLocationId);
            builder.HasOne(x => x.DestinationLocation).WithMany().HasForeignKey(x => x.DestinationLocationId);
        }
    }
    public class JobInsuranceConfiguration : IEntityTypeConfiguration<JobInsurance>
    {
        public void Configure(EntityTypeBuilder<JobInsurance> builder)
        {
            builder.ToTable("JobInsurances", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Job).WithMany(x => x.Insurance).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.InsuranceCompany).WithMany().HasForeignKey(x => x.InsuranceCompanyId);
            builder.HasOne(x => x.InsuranceValueCurrency).WithMany().HasForeignKey(x => x.InsuranceValueCurrencyId);
        }
    }
    public class JobLCConfiguration : IEntityTypeConfiguration<JobLC>
    {
        public void Configure(EntityTypeBuilder<JobLC> builder)
        {
            builder.ToTable("JobLCs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LCNumber).HasMaxLength(30);
            builder.Property(x => x.LCIssuingBankName).HasMaxLength(50);

            builder.HasOne(x => x.Job).WithMany(x => x.LC).HasForeignKey(x => x.JobId);
        }
    }



    public class JobCompanyRoleConfiguration : IEntityTypeConfiguration<JobCompanyRole>
    {
        public void Configure(EntityTypeBuilder<JobCompanyRole> builder)
        {
            builder.ToTable("JobCompanyRoles", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Job).WithMany(x => x.JobCompanyRoles).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CompanyRoleType).WithMany().HasForeignKey(x => x.CompanyRoleTypeId);
            builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId);
            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);
        }
    }


    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ShipmentTitle).HasMaxLength(50);
            builder.HasOne(x => x.ShipmentStatus).WithMany().HasForeignKey(x => x.ShipmentStatusId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
        }
    }

    public class ShipmentBookingConfiguration : IEntityTypeConfiguration<ShipmentBooking>
    {
        public void Configure(EntityTypeBuilder<ShipmentBooking> builder)
        {
            builder.ToTable("ShipmentBookings", "ops");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
        }
    }





    public class ShipmentLegConfiguration : IEntityTypeConfiguration<ShipmentLeg>
    {
        public void Configure(EntityTypeBuilder<ShipmentLeg> builder)
        {
            builder.ToTable("ShipmentLegs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            //builder.Property(x => x.VesselName).HasMaxLength(35);
            //builder.Property(x => x.VoyageNo).HasMaxLength(35);
            //builder.Property(x => x.FlightNumberPlusDate).HasMaxLength(60);
            //builder.Property(x => x.TruckPlateNumber).HasMaxLength(40);
            builder.Property(x => x.ReceiptPlaceText).HasMaxLength(60);
            builder.Property(x => x.DeliveryPlaceText).HasMaxLength(60);

            builder.Property(x => x.VolumeCBM).HasPrecision(18, 6);



            builder.HasOne(x => x.Shipment).WithMany(x => x.ShipmentLegs).HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.FromLocation).WithMany().HasForeignKey(x => x.FromLocationId);
            builder.HasOne(x => x.ToLocation).WithMany().HasForeignKey(x => x.ToLocationId);
            builder.HasOne(x => x.LegType).WithMany().HasForeignKey(x => x.LegTypeId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Voyage).WithMany().HasForeignKey(x => x.VoyageId);
            builder.HasOne(x => x.Vessel).WithMany().HasForeignKey(x => x.VesselId);
            builder.HasOne(x => x.Flight).WithMany().HasForeignKey(x => x.FlightId);
            builder.HasOne(x => x.FlightDate).WithMany().HasForeignKey(x => x.FlightDateId);
            builder.HasOne(x => x.Trip).WithMany().HasForeignKey(x => x.TripId);
            builder.HasOne(x => x.Truck).WithMany().HasForeignKey(x => x.TruckId);
            builder.HasOne(x => x.Driver).WithMany().HasForeignKey(x => x.DriverId);
            builder.HasOne(x => x.LegStatus).WithMany().HasForeignKey(x => x.LegStatusId);

        }
    }

    public class ShipmentCargoConfiguration : IEntityTypeConfiguration<ShipmentCargo>
    {
        public void Configure(EntityTypeBuilder<ShipmentCargo> builder)
        {
            builder.ToTable("ShipmentCargos", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);


            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.CargoDetail).WithMany().HasForeignKey(x => x.CargoDetailId);
            builder.HasOne(x => x.ShipmentContainer).WithMany().HasForeignKey(x => x.ShipmentContainerId);
        }
    }

    public class ShipmentContainerConfiguration : IEntityTypeConfiguration<ShipmentContainer>
    {
        public void Configure(EntityTypeBuilder<ShipmentContainer> builder)
        {
            builder.ToTable("ShipmentContainers", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.SealNumber).HasMaxLength(30);


            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ContainerUse).WithMany().HasForeignKey(x => x.ContainerUseId);
        }
    }
    public class JobCustomFieldConfiguration : IEntityTypeConfiguration<JobCustomField>
    {
        public void Configure(EntityTypeBuilder<JobCustomField> builder)
        {
            builder.ToTable("JobCustomFields", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.FieldValue).HasMaxLength(100);

            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.CustomField).WithMany().HasForeignKey(x => x.CustomFieldId);
        }
    }
    public class JobDocumentConfiguration : IEntityTypeConfiguration<JobDocument>
    {
        public void Configure(EntityTypeBuilder<JobDocument> builder)
        {
            builder.ToTable("JobDocuments", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentTitle).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(100);
            builder.Property(x => x.DocumentNumber).HasMaxLength(35);
            builder.Property(x => x.IssuedBy).HasMaxLength(35);

            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId);
        }
    }

    public class JobLinkConfiguration : IEntityTypeConfiguration<JobLink>
    {
        public void Configure(EntityTypeBuilder<JobLink> builder)
        {
            builder.ToTable("JobLinks", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(100);

            builder.HasOne(x => x.SourceJob).WithMany().HasForeignKey(x => x.SourceJobId);
            builder.HasOne(x => x.TargetJob).WithMany().HasForeignKey(x => x.TargetJobId);
            builder.HasOne(x => x.JobLinkType).WithMany().HasForeignKey(x => x.JobLinkTypeId);
        }
    }

    public class JobPartyConfiguration : IEntityTypeConfiguration<JobParty>
    {
        public void Configure(EntityTypeBuilder<JobParty> builder)
        {
            builder.ToTable("JobParty", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ContactEmail).HasMaxLength(35);
            builder.Property(x => x.ContactName).HasMaxLength(50);
            builder.Property(x => x.ContactPhone).HasMaxLength(35);

            builder.HasOne(x => x.Job).WithMany(x => x.Parties).HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.RoleType).WithMany().HasForeignKey(x => x.RoleTypeId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);
        }
    }

    public class JobServiceConfiguration : IEntityTypeConfiguration<JobService>
    {
        public void Configure(EntityTypeBuilder<JobService> builder)
        {
            builder.ToTable("JobServices", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId);
            builder.HasOne(x => x.Vendor).WithMany().HasForeignKey(x => x.VendorId);
        }
    }
    public class CustomsClearanceConfiguration : IEntityTypeConfiguration<CustomsClearance>
    {
        public void Configure(EntityTypeBuilder<CustomsClearance> builder)
        {
            builder.ToTable("CustomsClearances", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);
            builder.Property(x => x.LCNumber).HasMaxLength(200);
            builder.Property(x => x.CommercialInvoiceNumber).HasMaxLength(30);
            builder.Property(x => x.ProformaInvoiceNumber).HasMaxLength(30);
            builder.Property(x => x.DeclarantName).HasMaxLength(100);
            builder.Property(x => x.DeclarantLicense).HasMaxLength(50);
            builder.Property(x => x.DeclarationNumber).HasMaxLength(30);

            builder.Property(x => x.MRN).HasMaxLength(35);



            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.ShipmentOperationPlan).WithMany().HasForeignKey(x => x.ShipmentOperationPlanId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.CustomOffice).WithMany().HasForeignKey(x => x.CustomOfficeId);
            builder.HasOne(x => x.ImporterExporter).WithMany().HasForeignKey(x => x.ImporterExporterId);
            builder.HasOne(x => x.CustomsBroker).WithMany().HasForeignKey(x => x.CustomsBrokerId);
            builder.HasOne(x => x.CIFCurrency).WithMany().HasForeignKey(x => x.CIFCurrencyId);
            builder.HasOne(x => x.FOBCurrency).WithMany().HasForeignKey(x => x.FOBCurrencyId);
            builder.HasOne(x => x.CustomsValueCurrency).WithMany().HasForeignKey(x => x.CustomsValueCurrencyId);
            builder.HasOne(x => x.Incoterm).WithMany().HasForeignKey(x => x.IncoTermId);
            builder.HasOne(x => x.LoadPort).WithMany().HasForeignKey(x => x.LoadPortId);
            builder.HasOne(x => x.FreeZone).WithMany().HasForeignKey(x => x.FreeZoneId);
            builder.HasOne(x => x.CustomsStatus).WithMany().HasForeignKey(x => x.CustomsStatusId);
            builder.HasOne(x => x.DeclarationType).WithMany().HasForeignKey(x => x.DeclarationTypeId);
            builder.HasOne(x => x.VGM).WithMany().HasForeignKey(x => x.VGMId);
        }
    }

    public class CustomsClearanceItemConfiguration : IEntityTypeConfiguration<CustomsClearanceItem>
    {
        public void Configure(EntityTypeBuilder<CustomsClearanceItem> builder)
        {
            builder.ToTable("CustomsClearanceItems", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);
            builder.Property(x => x.HSCodeToSave).HasMaxLength(12);
            builder.Property(x => x.GoodsDescriptionEN).HasMaxLength(300);
            builder.Property(x => x.GoodsDescriptionLocal).HasMaxLength(300);
            builder.Property(x => x.PreferentialTariffCode).HasMaxLength(50);
            builder.Property(x => x.CertificateOfOriginRef).HasMaxLength(50);

            //Relations
            builder.HasOne(x => x.CustomsClearance).WithMany(x => x.Items).HasForeignKey(x => x.CustomsClearanceId);
            builder.HasOne(x => x.CountryHSCode).WithMany().HasForeignKey(x => x.CountryHSCodeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }


    public class CustomsClearanceContainerConfiguration : IEntityTypeConfiguration<CustomsClearanceContainer>
    {
        public void Configure(EntityTypeBuilder<CustomsClearanceContainer> builder)
        {
            builder.ToTable("CustomsClearanceContainers", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);
            builder.Property(x => x.ContainerNo).IsRequired().HasMaxLength(12);
            builder.Property(x => x.SealNo).HasMaxLength(35);


            //Relations
            builder.HasOne(x => x.CustomsClearance).WithMany(x => x.Containers).HasForeignKey(x => x.CustomsClearanceId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);
            builder.HasOne(x => x.Container).WithMany().HasForeignKey(x => x.ContainerId);
            builder.HasOne(x => x.FullGateInContainer).WithMany().HasForeignKey(x => x.FullGateInContainerId);
            builder.HasOne(x => x.VGMContainer).WithMany().HasForeignKey(x => x.VGMContainerId);
            builder.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId);
        }
    }


    public class CustomsClearanceChargeConfiguration : IEntityTypeConfiguration<CustomsClearanceCharge>
    {
        public void Configure(EntityTypeBuilder<CustomsClearanceCharge> builder)
        {
            builder.ToTable("CustomsClearanceCharges", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);

            //Relations
            builder.HasOne(x => x.CustomsClearance).WithMany(x => x.Charges).HasForeignKey(x => x.CustomsClearanceId);
            builder.HasOne(x => x.CustomsClearanceItem).WithMany().HasForeignKey(x => x.CustomsClearanceItemId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }

    public class CustomsHoldConfiguration : IEntityTypeConfiguration<CustomsHold>
    {
        public void Configure(EntityTypeBuilder<CustomsHold> builder)
        {
            builder.ToTable("CustomsHolds", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);

            //Relations
            builder.HasOne(x => x.CustomsClearance).WithMany(x => x.Holds).HasForeignKey(x => x.CustomsClearanceId);
        }
    }

    public class CustomsClearanceLicenseConfiguration : IEntityTypeConfiguration<CustomsClearanceLicense>
    {
        public void Configure(EntityTypeBuilder<CustomsClearanceLicense> builder)
        {
            builder.ToTable("CustomsClearanceLicenses", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);
            builder.Property(x => x.IssuingOrganization).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LicenseNumber).IsRequired().HasMaxLength(300);
            //Relations
            builder.HasOne(x => x.CustomsClearance).WithMany(x => x.Licenses).HasForeignKey(x => x.CustomsClearanceId);
            builder.HasOne(x => x.LicenseType).WithMany().HasForeignKey(x => x.LicenseTypeId);
            builder.HasOne(x => x.LicenseStatus).WithMany().HasForeignKey(x => x.LicenseStatusId);
        }
    }
    public class CustomsInspectionConfiguration : IEntityTypeConfiguration<CustomsInspection>
    {
        public void Configure(EntityTypeBuilder<CustomsInspection> builder)
        {
            builder.ToTable("CustomsInspections", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(400);
            builder.Property(x => x.InspectorName).HasMaxLength(100);
            builder.Property(x => x.Location).HasMaxLength(100);
            builder.Property(x => x.Findings).HasMaxLength(500);
            builder.Property(x => x.DiscrepancyDescription).HasMaxLength(500);
            builder.Property(x => x.CorrectionsDescription).HasMaxLength(500);

            //Relations
            builder.HasOne(x => x.CustomsClearance).WithMany(x => x.Inspections).HasForeignKey(x => x.CustomsClearanceId);
            builder.HasOne(x => x.InspectionType).WithMany().HasForeignKey(x => x.InspectionTypeId);
            builder.HasOne(x => x.InspectionMethod).WithMany().HasForeignKey(x => x.InspectionMethodId);
            builder.HasOne(x => x.InspectionResult).WithMany().HasForeignKey(x => x.InspectionResultId);
        }
    }

    public class CorrespondenceConfiguration : IEntityTypeConfiguration<Correspondence>
    {
        public void Configure(EntityTypeBuilder<Correspondence> builder)
        {
            builder.ToTable("Correspondences", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.SenderReceiverTitle).HasMaxLength(150);
            builder.Property(x => x.CorrespondenceNo).IsRequired().HasMaxLength(35);
            builder.Property(x => x.SubjectFreeText).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Keywords).IsRequired().HasMaxLength(200);

            //Relations
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);
            builder.HasOne(x => x.SenderReceiver).WithMany().HasForeignKey(x => x.SenderReceiverId);
            builder.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId);
            builder.HasOne(x => x.SystemObjectType).WithMany().HasForeignKey(x => x.SystemObjectTypeId);


            builder.HasIndex(e => e.CorrespondenceNo).HasDatabaseName("IX_Correspondences_CorrespondenceNo").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.CorrespondenceNo).HasDatabaseName("IX_Correspondences_CorrespondenceNo").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.Keywords).HasDatabaseName("IX_Correspondences_Keywords").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.SenderReceiverTitle).HasDatabaseName("IX_Correspondences_SenderReceiverTitle").IsUnique(false).IsClustered(false);

        }
    }
    public class NOAConfiguration : IEntityTypeConfiguration<NOA>
    {
        public void Configure(EntityTypeBuilder<NOA> builder)
        {
            builder.ToTable("NOA", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.NOANumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.VesselName).HasMaxLength(50);
            builder.Property(x => x.VoyageNo).HasMaxLength(50);
            builder.Property(x => x.SpecialInstructions).HasMaxLength(2000);
            builder.Property(x => x.Remarks).HasMaxLength(500);

            //Relations
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Carrier).WithMany().HasForeignKey(x => x.CarrierId);
            builder.HasOne(x => x.Consignee).WithMany().HasForeignKey(x => x.ConsigneeId);
            builder.HasOne(x => x.NotifyParty).WithMany().HasForeignKey(x => x.NotifyPartyId);
            builder.HasOne(x => x.ArrivalPort).WithMany().HasForeignKey(x => x.ArrivalPortId);
            builder.HasOne(x => x.DischargeTerminal).WithMany().HasForeignKey(x => x.DischargeTerminalId);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
            builder.HasOne(x => x.Voyage).WithMany().HasForeignKey(x => x.VoyageId);
            builder.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreateUserId);
        }
    }

    public class NOAPriceConfiguration : IEntityTypeConfiguration<NOAPrice>
    {
        public void Configure(EntityTypeBuilder<NOAPrice> builder)
        {
            builder.ToTable("NOAPrices", "ops");
            builder.HasKey(c => c.Id);

            //Relations
            builder.HasOne(x => x.NOA).WithMany(x => x.Prices).HasForeignKey(x => x.NOAId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.ClearanceType).WithMany().HasForeignKey(x => x.ClearanceTypeId);
        }
    }


    public class NOASendConfiguration : IEntityTypeConfiguration<NOASend>
    {
        public void Configure(EntityTypeBuilder<NOASend> builder)
        {
            builder.ToTable("NOASend", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.NOASender).HasMaxLength(100);
            builder.Property(x => x.NOAReceiver).HasMaxLength(100);

            //Relations
            builder.HasOne(x => x.NOA).WithMany(x => x.Sends).HasForeignKey(x => x.NOAId);
            builder.HasOne(x => x.SendToolType).WithMany().HasForeignKey(x => x.SendToolTypeId);
            builder.HasOne(x => x.SmsMessage).WithMany().HasForeignKey(x => x.SmsMessageId);
        }
    }


    public class ShipmentOperationPlanConfiguration : IEntityTypeConfiguration<ShipmentOperationPlan>
    {
        public void Configure(EntityTypeBuilder<ShipmentOperationPlan> builder)
        {
            builder.ToTable("ShipmentOperationPlans", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);

            //Relations
            builder.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId);
            builder.HasOne(x => x.OperationType).WithMany().HasForeignKey(x => x.OperationTypeId);
            builder.HasOne(x => x.PlannedLocation).WithMany().HasForeignKey(x => x.PlannedLocationId);
            builder.HasOne(x => x.PlanStatus).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.JobCompanyRole).WithMany().HasForeignKey(x => x.JobCompanyRoleId);
            builder.HasOne(x => x.ResponsibilityParty).WithMany().HasForeignKey(x => x.ResponsibilityPartyId);
        }
    }


    public class EntityReferenceConfiguration : IEntityTypeConfiguration<EntityReference>
    {
        public void Configure(EntityTypeBuilder<EntityReference> builder)
        {
            builder.ToTable("EntityReferences", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ReferenceType).HasMaxLength(30);
            builder.Property(x => x.ReferenceValue).HasMaxLength(100);

            //Relations
            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);

        }
    }

    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLog", "ops");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(100);
            builder.Property(x => x.ReferenceNo).HasMaxLength(100);
            builder.Property(x => x.FromStatusTable).HasMaxLength(50);
            builder.Property(x => x.ToStatusTable).HasMaxLength(50);
            builder.Property(x => x.FieldName).HasMaxLength(50);
            builder.Property(x => x.OldValue).HasMaxLength(200);
            builder.Property(x => x.NewValue).HasMaxLength(200);
            builder.Property(x => x.IpAddress).HasMaxLength(20);
            builder.Property(x => x.UserAgent).HasMaxLength(200);
            builder.Property(x => x.Severity).HasMaxLength(20);
            builder.Property(x => x.IconCssClass).HasMaxLength(50);
            builder.Property(x => x.SourceSystem).HasMaxLength(30);
            builder.Property(x => x.ExternalReferenceNo).HasMaxLength(50);

            //Relations
            builder.HasOne(x => x.EventType).WithMany().HasForeignKey(x => x.EventTypeId);
            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);
            builder.HasOne(x => x.ActivityCategory).WithMany().HasForeignKey(x => x.ActivityCategoryId);
            builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId);
            builder.HasOne(x => x.PerformedByUser).WithMany().HasForeignKey(x => x.PerformedByUserId);

            builder.HasIndex(e => e.ReferenceNo).HasDatabaseName("IX_ActivityLogs_ReferenceNo").IsUnique(false).IsClustered(false);

        }
    }


    public class ApiInquiryLogConfiguration : IEntityTypeConfiguration<ApiInquiryLog>
    {
        public void Configure(EntityTypeBuilder<ApiInquiryLog> builder)
        {
            builder.ToTable("ApiInquiryLogs", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Provider).HasMaxLength(50);
            builder.Property(x => x.ServiceName).HasMaxLength(50);
            builder.Property(x => x.Endpoint).HasMaxLength(70);
        }
    }

    public class FreightLetterConfiguration : IEntityTypeConfiguration<FreightLetter>
    {
        public void Configure(EntityTypeBuilder<FreightLetter> builder)
        {
            builder.ToTable("FreightLetters", "ops");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.MBLNo).HasMaxLength(50);
            builder.Property(x => x.HBLNo).HasMaxLength(50);
            builder.Property(x => x.FromLocationName).HasMaxLength(50);
            builder.Property(x => x.ToLocationName).HasMaxLength(50);
            builder.Property(x => x.CustomerName).HasMaxLength(200);

            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.DeliveryOrder).WithMany().HasForeignKey(x => x.DOId);
            builder.HasOne(x => x.CustomsOffice).WithMany().HasForeignKey(x => x.CustomsOfficeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

        }
    }



}
