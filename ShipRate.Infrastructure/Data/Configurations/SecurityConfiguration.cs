using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions", "sec");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.Name)
                  .IsUnique(); // نام دسترسی باید یکتا باشد
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.DisplayName).IsRequired().HasMaxLength(200);

            builder.HasData(
            new Permission { Id = 10, Name = "BaseInfo", DisplayName = "Base Info Section", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101001, Name = "BaseInfo.Persons.View", DisplayName = "View Persons and Organizations", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101002, Name = "BaseInfo.Persons.Edit", DisplayName = "Edit Persons and Organizations", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101005, Name = "BaseInfo.Persons.ApiGetPersonInfo", DisplayName = "Get Person Info Through API", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101010, Name = "BaseInfo.Persons.ApiGetReturnChequesInfo", DisplayName = "Get Returned Cheques Info", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101101, Name = "BaseInfo.Locations.View", DisplayName = "View Locations", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101102, Name = "BaseInfo.Locations.Edit", DisplayName = "Edit Locations", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 101201, Name = "BaseInfo.Territories.View", DisplayName = "View Territories", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101202, Name = "BaseInfo.Territories.Edit", DisplayName = "Edit Territories", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101301, Name = "BaseInfo.Banks.View", DisplayName = "View Banks", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101302, Name = "BaseInfo.Banks.Edit", DisplayName = "Edit Banks", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101401, Name = "BaseInfo.Vessels.View", DisplayName = "View Vessels", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101402, Name = "BaseInfo.Vessels.Edit", DisplayName = "Edit Vessels", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101501, Name = "BaseInfo.PackageTypes.View", DisplayName = "View Package Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101502, Name = "BaseInfo.PackageTypes.Edit", DisplayName = "Edit Package Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101601, Name = "BaseInfo.CargoCommodities.View", DisplayName = "View Cargo Commodities", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101602, Name = "BaseInfo.CargoCommodities.Edit", DisplayName = "Edit Cargo Commodities", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101701, Name = "BaseInfo.HSCodes.View", DisplayName = "View HS Codes", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101702, Name = "BaseInfo.HSCodes.Edit", DisplayName = "Edit HS Codes", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101801, Name = "BaseInfo.EventTypes.View", DisplayName = "View Event Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101802, Name = "BaseInfo.EventTypes.Edit", DisplayName = "Edit Event Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 101901, Name = "BaseInfo.MeasurementUnits.View", DisplayName = "View Measurement Units", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 101902, Name = "BaseInfo.MeasurementUnits.Edit", DisplayName = "Edit Measurement Units", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 102001, Name = "BaseInfo.DocumentTypes.View", DisplayName = "View Document Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 102002, Name = "BaseInfo.DocumentTypes.Edit", DisplayName = "Edit Document Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 102101, Name = "BaseInfo.TaskTypes.View", DisplayName = "View Task Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 102102, Name = "BaseInfo.TaskTypes.Edit", DisplayName = "Edit Task Types", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 102201, Name = "BaseInfo.BlackListReasons.View", DisplayName = "View Black List Reasons", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 102202, Name = "BaseInfo.BlackListReasons.Edit", DisplayName = "Edit Black List Reasons", Category = "Base Info", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 15, Name = "BaseOperations", DisplayName = "Base Operations Section", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151001, Name = "BaseOperations.ServiceAndLoops.View", DisplayName = "View Services and Loops", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151002, Name = "BaseOperations.ServiceAndLoops.Edit", DisplayName = "Edit Services and Loops", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151101, Name = "BaseOperations.Voyages.View", DisplayName = "View Voyages", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151102, Name = "BaseOperations.Voyages.Edit", DisplayName = "Edit Voyages", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151105, Name = "BaseOperations.Voyages.EditPortEvents", DisplayName = "Edit Voyage Port Events", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 151201, Name = "BaseOperations.Flights.View", DisplayName = "View Flights", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151202, Name = "BaseOperations.Flights.Edit", DisplayName = "Edit Flights", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151301, Name = "BaseOperations.Routes.View", DisplayName = "View Routes", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151302, Name = "BaseOperations.Routes.Edit", DisplayName = "Edit Routes", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151401, Name = "BaseOperations.Distances.View", DisplayName = "View Distances", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151402, Name = "BaseOperations.Distances.Edit", DisplayName = "Edit Distances", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151501, Name = "BaseOperations.AgentsManagement.View", DisplayName = "View Agents Management", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151502, Name = "BaseOperations.AgentsManagement.Edit", DisplayName = "Edit Agents Management", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151505, Name = "BaseOperations.AgentsManagement.Activation", DisplayName = "Activate/Deactivate Agents", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151601, Name = "BaseOperations.SerialsAndHolograms.View", DisplayName = "View Serials And Holograms", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151602, Name = "BaseOperations.SerialsAndHolograms.Edit", DisplayName = "Edit Serials And Holograms", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151701, Name = "BaseOperations.Tariffs.View", DisplayName = "View Tariffs", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151702, Name = "BaseOperations.Tariffs.Edit", DisplayName = "Edit Tariffs", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 151801, Name = "BaseOperations.FACContracts.View", DisplayName = "View FAC Contracts", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 151802, Name = "BaseOperations.FACContracts.Edit", DisplayName = "Edit FAC Contracts", Category = "Base Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 20, Name = "Operations", DisplayName = "Operations Section", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201001, Name = "Operations.RateRequests.View", DisplayName = "View Rate Requests", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201002, Name = "Operations.RateRequests.Edit", DisplayName = "Handle(Edit) Rate Requests", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201005, Name = "Operations.RateRequests.GenerateJob", DisplayName = "Generate Job From Accepted Sell Quotation", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201101, Name = "Operations.Jobs.View", DisplayName = "View Jobs", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201102, Name = "Operations.Jobs.Edit", DisplayName = "Edit Job File", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201201, Name = "Operations.Jobs.Bookings.View", DisplayName = "View Bookings", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201202, Name = "Operations.Jobs.Bookings.Edit", DisplayName = "Edit Bookings", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201301, Name = "Operations.Jobs.BLs.View", DisplayName = "View BLs(Transport Documents)", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201302, Name = "Operations.Jobs.BLs.Edit", DisplayName = "Edit BLs(Transport Documents)", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201401, Name = "Operations.Jobs.Charges.View", DisplayName = "View BLs(Transport Documents)", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201402, Name = "Operations.Jobs.Charges.Edit", DisplayName = "Edit BLs(Transport Documents)", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201501, Name = "Operations.Jobs.Invoices.View", DisplayName = "View Job Invoices", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201502, Name = "Operations.Jobs.Invoices.Edit", DisplayName = "Edit Job Invoices", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201601, Name = "Operations.Jobs.Settlements.View", DisplayName = "View Job Invoices Settlements ", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201602, Name = "Operations.Jobs.Settlements.Edit", DisplayName = "Edit Job Invoices Settlements ", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201701, Name = "Operations.Jobs.Customs.View", DisplayName = "View Job Customs Clearances", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201702, Name = "Operations.Jobs.Customs.Edit", DisplayName = "Edit Job Customs Clearances ", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201801, Name = "Operations.ServiceRequests.View", DisplayName = "View Requests", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201802, Name = "Operations.ServiceRequests.Edit", DisplayName = "Edit Requests", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 201805, Name = "Operations.ServiceRequests.Approve", DisplayName = "Approve Requests", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 201901, Name = "Operations.BLManagement.View", DisplayName = "View BLs", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 202001, Name = "Operations.ExportDocsReceipts.View", DisplayName = "View Export Docs Receipts", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 202002, Name = "Operations.ExportDocsReceipts.Edit", DisplayName = "Edit Export Docs Receipts", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 202101, Name = "Operations.VoyagesManagement.View", DisplayName = "View Voyages Management", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 202102, Name = "Operations.VoyagesManagement.Handle", DisplayName = "Handle Voyages Operations and Management", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 202201, Name = "Operations.TelexReleases.View", DisplayName = "View Telex Releases", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 202202, Name = "Operations.TelexReleases.Edit", DisplayName = "Edit TelexReleases", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 202205, Name = "Operations.TelexReleases.Approve", DisplayName = "Approve TelexReleases", Category = "Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 25, Name = "ImportSpecific", DisplayName = "Import Specific Operations", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 251001, Name = "ImportSpecific.NOAs.View", DisplayName = "View Notice Of Arrivals", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251002, Name = "ImportSpecific.NOAs.Edit", DisplayName = "Edit Notice Of Arrivals", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251005, Name = "ImportSpecific.NOAs.Send", DisplayName = "Send Notice Of Arrivals", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 251101, Name = "ImportSpecific.TIPs.View", DisplayName = "View TIPs(Temporary Import Permissions)", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251102, Name = "ImportSpecific.TIPs.Edit", DisplayName = "Edit TIPs(Temporary Import Permissions)", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 251201, Name = "ImportSpecific.DeliveryOrders.View", DisplayName = "View Delivery Orders", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251202, Name = "ImportSpecific.DeliveryOrders.Edit", DisplayName = "Edit Delivery Orders", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251210, Name = "ImportSpecific.DeliveryOrders.Approve", DisplayName = "Approve Delivery Orders", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251215, Name = "ImportSpecific.DeliveryOrders.Release", DisplayName = "Release Delivery Orders", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 251301, Name = "ImportSpecific.GuaranteePlans.View", DisplayName = "View Guarantee Plans", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 251302, Name = "ImportSpecific.GuaranteePlans.Edit", DisplayName = "Edit Guarantee Plans", Category = "Import Specific Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 28, Name = "Containers", DisplayName = "Container Operations", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 281001, Name = "Containers.Containers.View", DisplayName = "View Containers", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281002, Name = "Containers.Containers.Edit", DisplayName = "Edit Containers", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 281101, Name = "Containers.ContainerActions.View", DisplayName = "View Containers Actions", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281102, Name = "Containers.ContainerActions.Edit", DisplayName = "Edit Containers Actions", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281105, Name = "Containers.ContainerActions.ReleaseToShipper", DisplayName = "Register Container Release To Shipper(RTS)", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281110, Name = "Containers.ContainerActions.Discharge", DisplayName = "Register Container Discharge(DIS)", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281115, Name = "Containers.ContainerActions.LoadOnBoard", DisplayName = "Register Container Load On Board(LOB)", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281120, Name = "Containers.ContainerActions.ReturnByConsignee", DisplayName = "Register Container Return By Consignee(RBC)", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 281201, Name = "Containers.Detentions.View", DisplayName = "View Containers Detentions", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281202, Name = "Containers.Detentions.Edit", DisplayName = "Edit Containers Detentions", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 281301, Name = "Containers.ContainerTraces.View", DisplayName = "View Containers Traces", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 281302, Name = "Containers.ContainerTraces.Edit", DisplayName = "Edit Containers Traces", Category = "Container Operations", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 30, Name = "Finance", DisplayName = "Finance Section", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301001, Name = "Finance.ChargeTypes.View", DisplayName = "View Charge Types", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301002, Name = "Finance.ChargeTypes.Edit", DisplayName = "Edit Charge Types", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301101, Name = "Finance.AccountCodes.View", DisplayName = "View Account Codes", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301102, Name = "Finance.AccountCodes.Edit", DisplayName = "Edit Account Codes", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301201, Name = "Finance.ExchangeRates.View", DisplayName = "View Exchange Rates", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301202, Name = "Finance.ExchangeRates.Edit", DisplayName = "Edit Exchange Rates", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301301, Name = "Finance.Invoices.View", DisplayName = "View Invoices", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301302, Name = "Finance.Invoices.Edit", DisplayName = "Edit Invoices", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301401, Name = "Finance.PaymentDocuments.View", DisplayName = "View Payment Documents", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301402, Name = "Finance.PaymentDocuments.Edit", DisplayName = "Edit Payment Documents", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301501, Name = "Finance.TreasuryDocuments.View", DisplayName = "View Treasury Documents", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301502, Name = "Finance.TreasuryDocuments.Edit", DisplayName = "Edit Treasury Documents", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301505, Name = "Finance.TreasuryDocuments.Confirm", DisplayName = "Confirm Treasury Documents", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301601, Name = "Finance.Settlements.View", DisplayName = "View Settlements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301602, Name = "Finance.Settlements.Edit", DisplayName = "Edit Settlements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301701, Name = "Finance.ReturnCash.View", DisplayName = "View Return Cash", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301702, Name = "Finance.ReturnCash.Edit", DisplayName = "Edit Return Cash", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301801, Name = "Finance.AssetTypes.View", DisplayName = "View Asset Types", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301802, Name = "Finance.AssetTypes.Edit", DisplayName = "Edit Asset Types", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 301901, Name = "Finance.VaultMovements.View", DisplayName = "View Vault Movements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301902, Name = "Finance.VaultMovements.Edit", DisplayName = "Edit Vault Movements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301905, Name = "Finance.VaultMovements.FirstConfirm", DisplayName = "First Confirm Vault Movements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301910, Name = "Finance.VaultMovements.Authorize", DisplayName = "Authorize Vault Movements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301915, Name = "Finance.VaultMovements.Reject", DisplayName = "Reject Vault Movements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 301920, Name = "Finance.VaultMovements.Execute", DisplayName = "Execute Vault Movements", Category = "Finance", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 40, Name = "Transit", DisplayName = "Transit Section", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 401001, Name = "Transit.Trucks.View", DisplayName = "View Trucks", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 401002, Name = "Transit.Trucks.Edit", DisplayName = "Edit Trucks", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 401101, Name = "Transit.Drivers.View", DisplayName = "View Drivers", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 401102, Name = "Transit.Drivers.Edit", DisplayName = "Edit Drivers", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 401201, Name = "Transit.Trips.View", DisplayName = "View Trips", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 401202, Name = "Transit.Trips.Edit", DisplayName = "Edit Trips", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 401301, Name = "Transit.GPSDevices.View", DisplayName = "View GPS Devices", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 401302, Name = "Transit.GPSDevices.Edit", DisplayName = "Edit GPS Devices", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 401401, Name = "Transit.TIRCarnets.View", DisplayName = "View TIR Carnets", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 401402, Name = "Transit.TIRCarnets.Edit", DisplayName = "Edit TIR Carnets", Category = "Transit", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 50, Name = "CRM", DisplayName = "CRM Section", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 501001, Name = "CRM.CustomerConfirmations.View", DisplayName = "View Customer Confirmations", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501002, Name = "CRM.CustomerConfirmations.Edit", DisplayName = "Edit Customer Confirmations", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501003, Name = "CRM.CustomerConfirmations.ConfirmOrReject", DisplayName = "Confirm/Reject Customers", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 501101, Name = "CRM.Customers.View", DisplayName = "View Customers", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501102, Name = "CRM.Customers.Edit", DisplayName = "Edit Customers", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501103, Name = "CRM.Customers.AssignSalesUser", DisplayName = "Assign Sales User", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 501201, Name = "CRM.Industries.View", DisplayName = "View Industries", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501202, Name = "CRM.Industries.Edit", DisplayName = "Edit industries", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 501301, Name = "CRM.Attornies.View", DisplayName = "View Attornies", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501302, Name = "CRM.Attornies.Edit", DisplayName = "Edit Attornies", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 501401, Name = "CRM.BlackLists.View", DisplayName = "View Black Lists", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501402, Name = "CRM.BlackLists.Edit", DisplayName = "Edit Black Lists", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501403, Name = "CRM.BlackLists.Exit", DisplayName = "Exit From Black Lists", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 501501, Name = "CRM.CustomerGroupings.View", DisplayName = "View Customers Grouping", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 501502, Name = "CRM.CustomerGroupings.Edit", DisplayName = "Edit Customers Grouping", Category = "CRM", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 60, Name = "Marketing", DisplayName = "Marketing Section", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 601001, Name = "Marketing.LostReasons.View", DisplayName = "View Lost Reasons", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 601002, Name = "Marketing.LostReasons.Edit", DisplayName = "Edit Lost Reasons", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 601101, Name = "Marketing.Leads.View", DisplayName = "View Leads", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 601102, Name = "Marketing.Leads.Edit", DisplayName = "Edit Leads", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 601105, Name = "Marketing.Leads.Assign", DisplayName = "Assign Sales User", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 601201, Name = "Marketing.Campaigns.View", DisplayName = "View Campaigns", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 601202, Name = "Marketing.Campaigns.Edit", DisplayName = "Edit Campaigns", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 601301, Name = "Marketing.Opportunities.View", DisplayName = "View Opportunities", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 601302, Name = "Marketing.Opportunities.Edit", DisplayName = "Edit Opportunities", Category = "Marketing", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 70, Name = "Agents Sections", DisplayName = "Agents Section", Category = "Agents Section", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 80, Name = "PettyCash", DisplayName = "Petty Cash Section", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 801001, Name = "PettyCash.CostCenters.View", DisplayName = "View Cost Centers", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 801002, Name = "PettyCash.CostCenters.Edit", DisplayName = "Edit Cost Centers", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 801101, Name = "PettyCash.ExpenseCategories.View", DisplayName = "View Expense Categories", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 801102, Name = "PettyCash.ExpenseCategories.Edit", DisplayName = "Edit Expense Categories", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 801201, Name = "PettyCash.PettyCashes.View", DisplayName = "View Petty Cashes", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 801202, Name = "PettyCash.PettyCashes.Edit", DisplayName = "Edit Petty Cashes", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 801205, Name = "PettyCash.PettyCashes.ApproveRejectExpense", DisplayName = "Approve/Reject Expense", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 801210, Name = "PettyCash.PettyCashes.ApproveRejectRecharge", DisplayName = "Approve/Reject Recharge", Category = "Petty Cash", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 82, Name = "Inventory", DisplayName = "Inventory Sections", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 821001, Name = "Inventory.Inventories.View", DisplayName = "View Inventories", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821002, Name = "Inventory.Inventories.Edit", DisplayName = "Edit Inventories", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 821101, Name = "Inventory.ProductCategories.View", DisplayName = "View Product Categories", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821102, Name = "Inventory.ProductCategories.Edit", DisplayName = "Edit Product Categories", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 821201, Name = "Inventory.Products.View", DisplayName = "View Products", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821202, Name = "Inventory.Products.Edit", DisplayName = "Edit Products", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 821301, Name = "Inventory.ProductRequests.View", DisplayName = "View Product Requests", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821302, Name = "Inventory.ProductRequests.Edit", DisplayName = "Edit Product Requests", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821305, Name = "Inventory.ProductRequests.ApproveReject", DisplayName = "Approve/Reject Product Requests", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821310, Name = "Inventory.ProductRequests.Deliver", DisplayName = "Deliver Product Requests", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 821401, Name = "Inventory.ProductStockTransactions.View", DisplayName = "View Product Stock Transactions", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821402, Name = "Inventory.ProductStockTransactions.Edit", DisplayName = "Edit Product Stock Transactions", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 821405, Name = "Inventory.ProductStockTransactions.Deliver", DisplayName = "Deliver Stock Transactions", Category = "Inventory", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 85, Name = "Utilities", DisplayName = "Utilities Sections", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 851001, Name = "Utilities.Documents.View", DisplayName = "View All Documents(Attachments)", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 851101, Name = "Utilities.Correspondences.View", DisplayName = "View Correspondences", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 851102, Name = "Utilities.Correspondences.Edit", DisplayName = "Edit Correspondences", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 851201, Name = "Utilities.PostalDispatch.View", DisplayName = "View Postal Dispatch", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 851202, Name = "Utilities.PostalDispatch.Edit", DisplayName = "Edit Postal Dispatch", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 851301, Name = "Utilities.EDIMessages.View", DisplayName = "View EDI Messages", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },
            new Permission { Id = 851302, Name = "Utilities.EDIMessages.Edit", DisplayName = "Create EDI Messages", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },

            new Permission { Id = 851401, Name = "Utilities.Maps.View", DisplayName = "View Maps", Category = "Utilities", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" },


            new Permission { Id = 99, Name = "Reports", DisplayName = "Reports Section", Category = "Report", IsActive = true, CreatedAt = DateTime.Parse("2026-06-21"), Description = "" }


            );
        }
    }

    public class TeamTypeConfiguration : IEntityTypeConfiguration<TeamType>
    {
        public void Configure(EntityTypeBuilder<TeamType> builder)
        {
            builder.ToTable("TeamTypes", "sec");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TeamTypeName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.TeamTypeLocalName).IsRequired().HasMaxLength(100);
            //Seed Data
            builder.HasData(
                new TeamType { Id = 1, TeamTypeName = "Sales", TeamTypeLocalName = "تیم فروش" },
                new TeamType { Id = 2, TeamTypeName = "Operations", TeamTypeLocalName = "تیم عملیات" },
                new TeamType { Id = 3, TeamTypeName = "Documentation", TeamTypeLocalName = "تیم اسناد و مدارک" },
                new TeamType { Id = 4, TeamTypeName = "Accounting", TeamTypeLocalName = "تیم حسابداری" },
                new TeamType { Id = 5, TeamTypeName = "Management", TeamTypeLocalName = "تیم مدیریت" },
                new TeamType { Id = 6, TeamTypeName = "Clearance", TeamTypeLocalName = "تیم ترخیصیه" },
                new TeamType { Id = 99, TeamTypeName = "General", TeamTypeLocalName = "تیم عمومی / سایر" }
                );
        }
    }




    //public class UserTypeConfiguration : IEntityTypeConfiguration<UserType>
    //{
    //    public void Configure(EntityTypeBuilder<UserType> builder)
    //    {
    //        builder.ToTable("UserTypes", "sec");
    //        builder.HasKey(c => c.Id);
    //        builder.Property(c => c.UserTypeName).IsRequired().HasMaxLength(50);
    //        builder.Property(c => c.UserTypeLocalName).IsRequired().HasMaxLength(50);
    //        builder.Property(c => c.UserTypeDescription).HasMaxLength(200);

    //        builder.HasData(
    //            new UserType { Id = 1, UserTypeName = "Internal(System)", UserTypeLocalName = "کاربر داخلی سیستم اصلی", UserTypeDescription = "کاربر متعلق به پرسنل شرکتهای داخلی" },
    //            new UserType { Id = 2, UserTypeName = "External(System)", UserTypeLocalName = "کاربر خارجی سیستم اصلی", UserTypeDescription = "کاربران خارجی اعم از Carrier و Agent" },
    //            new UserType { Id = 3, UserTypeName = "Customers Portal", UserTypeLocalName = "کاربر درگاه مشتریان", UserTypeDescription = "مشتریان " }
    //            );
    //    }
    //}

    //public class UserCompanyConfiguration : IEntityTypeConfiguration<UserCompany>
    //{
    //    public void Configure(EntityTypeBuilder<UserCompany> builder)
    //    {
    //        builder.ToTable("UserCompanies", "sec");
    //        builder.HasKey(c => c.Id);

    //        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    //        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);

    //    }
    //}






    public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.ToTable("UserPermissions", "sec");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PermissionId).IsRequired();
            builder.Property(c => c.UserId).IsRequired();

            builder.HasOne(up => up.User)
                  .WithMany(u => u.UserPermissions)
                  .HasForeignKey(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // ارتباط با Permission
            builder.HasOne(up => up.Permission)
                  .WithMany(p => p.UserPermissions)
                  .HasForeignKey(up => up.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index برای کوئری سریع‌تر
            builder.HasIndex(up => up.UserId);
            builder.HasIndex(up => up.PermissionId);
            builder.HasIndex(up => new { up.PermissionId, up.UserId }).IsUnique();

        }
    }


    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions", "sec");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PermissionId).IsRequired();
            builder.Property(c => c.RoleId).IsRequired();

            builder.HasOne(up => up.Role).WithMany(u => u.RolePermissions).HasForeignKey(up => up.RoleId).OnDelete(DeleteBehavior.Cascade);

            // ارتباط با Permission
            builder.HasOne(up => up.Permission).WithMany().HasForeignKey(up => up.PermissionId).OnDelete(DeleteBehavior.Cascade);

            // Index برای کوئری سریع‌تر
            builder.HasIndex(up => up.RoleId);
            builder.HasIndex(up => up.PermissionId);

            builder.HasIndex(up => new { up.PermissionId, up.RoleId }).IsUnique();
        }
    }


    public class PermissionGroupConfiguration : IEntityTypeConfiguration<PermissionGroup>
    {
        public void Configure(EntityTypeBuilder<PermissionGroup> builder)
        {
            builder.ToTable("PermissionGroups", "sec");

            builder.HasKey(pg => pg.Id);

            builder.Property(pg => pg.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(pg => pg.Name).IsUnique();

            builder.Property(pg => pg.Description)
                .HasMaxLength(500);
        }
    }

    public class PermissionGroupPermissionConfiguration
        : IEntityTypeConfiguration<PermissionGroupPermission>
    {
        public void Configure(EntityTypeBuilder<PermissionGroupPermission> builder)
        {
            builder.ToTable("PermissionGroupPermissions", "sec");

            builder.HasKey(pgp => pgp.Id);

            builder.HasOne(pgp => pgp.PermissionGroup)
                .WithMany(pg => pg.PermissionGroupPermissions)
                .HasForeignKey(pgp => pgp.PermissionGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pgp => pgp.Permission)
                .WithMany()
                .HasForeignKey(pgp => pgp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique: یک Permission فقط یک بار در یک Group
            builder.HasIndex(pgp => new { pgp.PermissionGroupId, pgp.PermissionId })
                .IsUnique();
        }
    }

    public class UserPermissionGroupConfiguration
        : IEntityTypeConfiguration<UserPermissionGroup>
    {
        public void Configure(EntityTypeBuilder<UserPermissionGroup> builder)
        {
            builder.ToTable("UserPermissionGroups", "sec");

            builder.HasKey(upg => upg.Id);

            builder.HasOne(upg => upg.User)
                .WithMany()
                .HasForeignKey(upg => upg.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(upg => upg.PermissionGroup).WithMany(pg => pg.UserPermissionGroups).HasForeignKey(upg => upg.PermissionGroupId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.AssignedByUser).WithMany().HasForeignKey(a => a.AssignedByUserId).OnDelete(DeleteBehavior.Cascade);

            // Unique: یک User فقط یک بار یک Group را می‌گیرد
            builder.HasIndex(upg => new { upg.UserId, upg.PermissionGroupId })
                .IsUnique()
                .HasFilter("[IsDeleted] = CAST(0 AS bit)");
        }
    }
    public class AuditLogConfiguration
        : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs", "sec");
            builder.HasKey(upg => upg.Id);
        }
    }
    public class SecurityEventConfiguration
        : IEntityTypeConfiguration<SecurityEvent>
    {
        public void Configure(EntityTypeBuilder<SecurityEvent> builder)
        {
            builder.ToTable("SecurityEvents", "sec");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.EventType).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Email).HasMaxLength(256);
            builder.Property(e => e.IpAddress).HasMaxLength(45);
            builder.Property(e => e.UserAgent).HasMaxLength(500);
            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.Email);
            builder.HasIndex(e => e.EventType);
        }
    }



    public class UserTeamConfiguration : IEntityTypeConfiguration<UserTeam>
    {
        public void Configure(EntityTypeBuilder<UserTeam> builder)
        {
            builder.ToTable("UserTeams", "sec");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.TeamName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.TeamLocalName).IsRequired().HasMaxLength(100);

            //Relations
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.TeamType).WithMany().HasForeignKey(x => x.TeamTypeId);
            builder.HasOne(x => x.Manager).WithMany().HasForeignKey(x => x.ManagerId);
        }
    }

    public class UserTeamMemberConfiguration : IEntityTypeConfiguration<UserTeamMember>
    {
        public void Configure(EntityTypeBuilder<UserTeamMember> builder)
        {
            builder.ToTable("UserTeamMembers", "sec");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.MemberRole).IsRequired().HasMaxLength(100);

            //Relations
            builder.HasOne(x => x.Team).WithMany().HasForeignKey(x => x.UserTeamId);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        }
    }

    public class TwoFactorMethodConfiguration : IEntityTypeConfiguration<TwoFactorMethod>
    {
        public void Configure(EntityTypeBuilder<TwoFactorMethod> builder)
        {
            builder.ToTable("TwoFactorMethods", "sec");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.TwoFactorMethodName).IsRequired().HasMaxLength(35);

            builder.HasData(
                new TwoFactorMethod { Id = 1, TwoFactorMethodName = "None" },
                new TwoFactorMethod { Id = 2, TwoFactorMethodName = "Sms" },
                new TwoFactorMethod { Id = 3, TwoFactorMethodName = "GoogleAuthenticator" },
                new TwoFactorMethod { Id = 4, TwoFactorMethodName = "Email" }
                );
        }
    }


    public class UserOtpCodeConfiguration : IEntityTypeConfiguration<UserOtpCode>
    {
        public void Configure(EntityTypeBuilder<UserOtpCode> builder)
        {
            builder.ToTable("UserOtpCodes", "sec");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CodeHash).HasMaxLength(128);
            builder.Property(x => x.Method)
                       .HasConversion<int>()
                       .HasDefaultValue(TwoFactorMethodEnum.None);

            builder.HasOne(x => x.User).WithMany(x => x.OtpCodes).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => new
            {
                x.UserId,
                x.Method,
                x.IsUsed,
                x.ExpiresAt
            });


        }
    }

}





