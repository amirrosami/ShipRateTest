using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.BookingSpec;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Marketting;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Contract;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Customs;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Operations.TasksManage;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.ServiceRequests;
using ShipRate.Domain.Entities.TransitOperation;
using ShipRate.Domain.enums;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class ClearancTypeConfiguration : IEntityTypeConfiguration<ClearanceType>
    {
        public void Configure(EntityTypeBuilder<ClearanceType> builder)
        {
            builder.ToTable("ClearanceType", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ClearanceTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new ClearanceType { Id = 1, ClearanceTypeName = "TIP" },
                new ClearanceType { Id = 2, ClearanceTypeName = "Strip" },
                new ClearanceType { Id = 3, ClearanceTypeName = "Transit" }
            );

        }
    }
    public class SystemObjectTypeConfiguration : IEntityTypeConfiguration<SystemObjectType>
    {
        public void Configure(EntityTypeBuilder<SystemObjectType> builder)
        {
            builder.ToTable("SystemObjectTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.SystemObjectTypeName).IsRequired().HasMaxLength(70);
            builder.Property(c => c.SystemObjectTypeLocalName).IsRequired().HasMaxLength(70);

            //Seed Data
            builder.HasData(
                new SystemObjectType { Id = 1, SystemObjectTypeName = "Persons", SystemObjectTypeLocalName = "اشخاص و سازمانها" },
                new SystemObjectType { Id = 2, SystemObjectTypeName = "PersonContacts", SystemObjectTypeLocalName = "افراد مرتبط" },
                new SystemObjectType { Id = 3, SystemObjectTypeName = "Customers", SystemObjectTypeLocalName = "مشتریان" },
                new SystemObjectType { Id = 4, SystemObjectTypeName = "Shipments", SystemObjectTypeLocalName = "حمل" },
                new SystemObjectType { Id = 5, SystemObjectTypeName = "BLs", SystemObjectTypeLocalName = "بارنامه ها" },
                new SystemObjectType { Id = 6, SystemObjectTypeName = "Bookings", SystemObjectTypeLocalName = "بوکینگ ها" },
                new SystemObjectType { Id = 7, SystemObjectTypeName = "NOAs", SystemObjectTypeLocalName = "اعلامیه ورود" },
                new SystemObjectType { Id = 8, SystemObjectTypeName = "Users", SystemObjectTypeLocalName = "کاربران" },
                new SystemObjectType { Id = 9, SystemObjectTypeName = "TelexReleaseRequests", SystemObjectTypeLocalName = "درخواست تلکس ریلیز" },
                new SystemObjectType { Id = 10, SystemObjectTypeName = "TIPs", SystemObjectTypeLocalName = "پروانه ورود موقت" },
                new SystemObjectType { Id = 11, SystemObjectTypeName = "RateRequests", SystemObjectTypeLocalName = "درخواست نرخ حمل" },
                new SystemObjectType { Id = 12, SystemObjectTypeName = "ServiceRequests", SystemObjectTypeLocalName = "درخواست ها-کلی" },
                new SystemObjectType { Id = 15, SystemObjectTypeName = "DeliveryOrders", SystemObjectTypeLocalName = "(Delivery Order)دستور تحویل" },
                new SystemObjectType { Id = 16, SystemObjectTypeName = "GuaranteePlans", SystemObjectTypeLocalName = "طرح تضمین" },
                new SystemObjectType { Id = 20, SystemObjectTypeName = "ExportDocsReceipts", SystemObjectTypeLocalName = "رسید اسناد صادراتی" },
                new SystemObjectType { Id = 200, SystemObjectTypeName = "Invoices", SystemObjectTypeLocalName = "صورتحسابها-فاکتورها" },
                new SystemObjectType { Id = 250, SystemObjectTypeName = "PettyCashes", SystemObjectTypeLocalName = "تنحواه" },
                new SystemObjectType { Id = 251, SystemObjectTypeName = "PettyCashExpenses", SystemObjectTypeLocalName = "هزینه کرد تنخواه" },
                new SystemObjectType { Id = 252, SystemObjectTypeName = "PettyCashRecharges", SystemObjectTypeLocalName = "شارژ مجدد تنخواه" },
                new SystemObjectType { Id = 300, SystemObjectTypeName = "Jobs", SystemObjectTypeLocalName = "(Job)پرونده تجاری حمل" },
                new SystemObjectType { Id = 400, SystemObjectTypeName = "ActivityLogs", SystemObjectTypeLocalName = "رویدادها و لاگهای عملیاتی" },
                new SystemObjectType { Id = 450, SystemObjectTypeName = "Tariffs", SystemObjectTypeLocalName = "تعرفه ها" },
                new SystemObjectType { Id = 460, SystemObjectTypeName = "Correspondences", SystemObjectTypeLocalName = "مکاتبات" },
                new SystemObjectType { Id = 470, SystemObjectTypeName = "PostalDispatches", SystemObjectTypeLocalName = "مرسوله های پستی" },
                new SystemObjectType { Id = 480, SystemObjectTypeName = "Drivers", SystemObjectTypeLocalName = "رانندگان" },
                new SystemObjectType { Id = 490, SystemObjectTypeName = "CashPaymentRequests", SystemObjectTypeLocalName = "درخواست وجه نقد از گاوصندوق" },
                new SystemObjectType { Id = 491, SystemObjectTypeName = "VaultItems", SystemObjectTypeLocalName = "اقلام گاو صندوق" },
                new SystemObjectType { Id = 492, SystemObjectTypeName = "VaultMovements", SystemObjectTypeLocalName = "تراکنش گاوصندوق" },
                new SystemObjectType { Id = 493, SystemObjectTypeName = "VaultItemCustodyLogs", SystemObjectTypeLocalName = "ورود و خروج اقلام گاو صندوق" }
                );
        }
    }

    public class LocationTypeConfiguration : IEntityTypeConfiguration<LocationType>
    {
        public void Configure(EntityTypeBuilder<LocationType> builder)
        {
            builder.ToTable("LocationTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LocationTypeName).IsRequired().HasMaxLength(40);
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Seed Data
            builder.HasData(
                new LocationType { Id = 1, LocationTypeName = "Province" },
                new LocationType { Id = 2, LocationTypeName = "City/Port" },
                new LocationType { Id = 3, LocationTypeName = "Airport" },
                new LocationType { Id = 4, LocationTypeName = "Border" },
                new LocationType { Id = 5, LocationTypeName = "Customs Office" },
                new LocationType { Id = 6, LocationTypeName = "Terminal" },
                new LocationType { Id = 7, LocationTypeName = "Depot" },
                new LocationType { Id = 8, LocationTypeName = "Free Zone" },
                new LocationType { Id = 9, LocationTypeName = "Rail Ramp" }
            );
        }
    }
    public class TerminalTypeConfiguration : IEntityTypeConfiguration<TerminalType>
    {
        public void Configure(EntityTypeBuilder<TerminalType> builder)
        {
            builder.ToTable("TerminalType", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TerminalTypeName).IsRequired().HasMaxLength(40);
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Seed Data
            builder.HasData(
                new TerminalType { Id = 1, TerminalTypeName = "Container" },
                new TerminalType { Id = 2, TerminalTypeName = "Bulk" },
                new TerminalType { Id = 3, TerminalTypeName = "Roro" },
                new TerminalType { Id = 4, TerminalTypeName = "General Cargo" },
                new TerminalType { Id = 5, TerminalTypeName = "Passenger" },
                new TerminalType { Id = 6, TerminalTypeName = "Liquid Bulk" },
                new TerminalType { Id = 7, TerminalTypeName = "Dry Bulk" },
                new TerminalType { Id = 8, TerminalTypeName = "Air Cargo" }
            );



        }
    }
    public class DepotTypeConfiguration : IEntityTypeConfiguration<DepotType>
    {
        public void Configure(EntityTypeBuilder<DepotType> builder)
        {
            builder.ToTable("DepotType", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DepotTypeName).IsRequired().HasMaxLength(40);


            // Seed Data
            builder.HasData(
                new DepotType { Id = 1, DepotTypeName = "Container Yard" },
                new DepotType { Id = 2, DepotTypeName = "Warehouse" },
                new DepotType { Id = 3, DepotTypeName = "Cold Storage" },
                new DepotType { Id = 4, DepotTypeName = "Custom Bonded" }
            );
        }
    }
    public class CityTypeConfiguration : IEntityTypeConfiguration<CityType>
    {
        public void Configure(EntityTypeBuilder<CityType> builder)
        {
            builder.ToTable("CityTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CityTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new CityType { Id = 1, CityTypeName = "City" },
                new CityType { Id = 2, CityTypeName = "Port" },
                new CityType { Id = 3, CityTypeName = "City/Port" }
            );

        }
    }
    public class BlackListStatusConfiguration : IEntityTypeConfiguration<BlackListStatus>
    {
        public void Configure(EntityTypeBuilder<BlackListStatus> builder)
        {
            builder.ToTable("BlackListStatuses", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BlackListStatusName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new BlackListStatus { Id = 1, BlackListStatusName = "Is Into Black List" },
                new BlackListStatus { Id = 2, BlackListStatusName = "Is Out Of Black List" }
            );

        }
    }
    public class BLChangeTypeConfiguration : IEntityTypeConfiguration<BLChangeType>
    {
        public void Configure(EntityTypeBuilder<BLChangeType> builder)
        {
            builder.ToTable("BLChangeTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BLChangeTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new BLChangeType { Id = 1, BLChangeTypeName = "Club" },
                new BLChangeType { Id = 2, BLChangeTypeName = "Split" },
                new BLChangeType { Id = 3, BLChangeTypeName = "Switch" }
            );

        }
    }
    public class BLStatusConfiguration : IEntityTypeConfiguration<BLStatus>
    {
        public void Configure(EntityTypeBuilder<BLStatus> builder)
        {
            builder.ToTable("BLStatuses", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BLStatusName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new BLStatus { Id = -1, BLStatusName = "Draft" },
                new BLStatus { Id = 1, BLStatusName = "Issued" },
                new BLStatus { Id = 2, BLStatusName = "Surrendered" },
                new BLStatus { Id = 3, BLStatusName = "Void" },
                new BLStatus { Id = 4, BLStatusName = "Lost" },
                //Transit CMR, ...
                new BLStatus { Id = 11, BLStatusName = "Delivered" },
                new BLStatus { Id = 12, BLStatusName = "Closed" }
            );
        }
    }
    public class BLTypeConfiguration : IEntityTypeConfiguration<BLType>
    {
        public void Configure(EntityTypeBuilder<BLType> builder)
        {
            builder.ToTable("BLTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BLTypeCode).IsRequired().HasMaxLength(15);
            builder.Property(c => c.BLTypeName).IsRequired().HasMaxLength(40);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
            // Seed Data
            builder.HasData(
                new BLType { Id = 1, BLTypeCode = "MBL", BLTypeName = "Master BL", TransportTypeId = 1 },
                new BLType { Id = 2, BLTypeCode = "HBL", BLTypeName = "House BL", TransportTypeId = 1 },
                new BLType { Id = 3, BLTypeCode = "MAWBL", BLTypeName = "Master Air Waybill", TransportTypeId = 4 },
                new BLType { Id = 4, BLTypeCode = "HAWBL", BLTypeName = "House Air Waybill", TransportTypeId = 4 },
                new BLType { Id = 5, BLTypeCode = "CMR", BLTypeName = "CMR Road Consignment Note", TransportTypeId = 2 },
                new BLType { Id = 6, BLTypeCode = "RWB", BLTypeName = "Rail Waybill", TransportTypeId = 3 },
                new BLType { Id = 7, BLTypeCode = "MTD", BLTypeName = "Multimodal Transport Document" },
                new BLType { Id = 8, BLTypeCode = "FWB", BLTypeName = "Forwarder Waybill" },
                new BLType { Id = 9, BLTypeCode = "FIATA", BLTypeName = "FIAIA BL" },
                new BLType { Id = 10, BLTypeCode = "TRUCKWB", BLTypeName = "Truck Waybill", TransportTypeId = 2 },
                new BLType { Id = 11, BLTypeCode = "INLANDWB", BLTypeName = "Inland Waybill", TransportTypeId = 2 }
            );

            //new BLType { Id = 5, BLTypeCode = "MEMO_BL", BLTypeName = "Memo BL (Draft)" },
            //new BLType { Id = 6, BLTypeCode = "THROUGH_BL", BLTypeName = "Through BL" },
            //new BLType { Id = 7, BLTypeCode = "COMBINED_BL", BLTypeName = "Combined Transport BL" }
        }
    }
    public class BookingChangeTypeConfiguration : IEntityTypeConfiguration<BookingChangeType>
    {
        public void Configure(EntityTypeBuilder<BookingChangeType> builder)
        {
            builder.ToTable("BookingChangeTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BookingChangeTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new BookingChangeType { Id = 1, BookingChangeTypeName = "Merge" },
                new BookingChangeType { Id = 2, BookingChangeTypeName = "Divide" }
            );
        }
    }
    public class BuySellTypeConfiguration : IEntityTypeConfiguration<BuySellType>
    {
        public void Configure(EntityTypeBuilder<BuySellType> builder)
        {
            builder.ToTable("BuySellTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BuySellTypeName).IsRequired().HasMaxLength(40);
            // Seed Data
            builder.HasData(
                new BuySellType { Id = 1, BuySellTypeName = "Buy" },
                new BuySellType { Id = 2, BuySellTypeName = "Sell" }
            );
        }
    }
    public class CarrierTypeConfiguration : IEntityTypeConfiguration<CarrierType>
    {
        public void Configure(EntityTypeBuilder<CarrierType> builder)
        {
            builder.ToTable("CarrierTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CarrierTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new CarrierType { Id = 1, CarrierTypeName = "Shipping Line" },
                new CarrierType { Id = 2, CarrierTypeName = "NVOCC" },
                new CarrierType { Id = 3, CarrierTypeName = "Air Line" },
                new CarrierType { Id = 4, CarrierTypeName = "Road Transit" },
                new CarrierType { Id = 5, CarrierTypeName = "Rail Transit" },
                new CarrierType { Id = 6, CarrierTypeName = "Forwarder" }
            );
        }
    }
    public class CntOwnTypeConfiguration : IEntityTypeConfiguration<CntOwnType>
    {
        public void Configure(EntityTypeBuilder<CntOwnType> builder)
        {
            builder.ToTable("CntOwnTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CntOwnTypeName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.CntOwnTypeDesc).IsRequired().HasMaxLength(100);

            // Seed Data
            builder.HasData(
                new CntOwnType { Id = 1, CntOwnTypeName = "COC", CntOwnTypeDesc = "Carrier Owned Container" },
                new CntOwnType { Id = 2, CntOwnTypeName = "SOC", CntOwnTypeDesc = "Shipper Owned Container" },
                new CntOwnType { Id = 3, CntOwnTypeName = "Internal", CntOwnTypeDesc = "Owned By US" }
            );
        }
    }

    public class CntOwnLeaseTypeConfiguration : IEntityTypeConfiguration<CntOwnLeaseType>
    {
        public void Configure(EntityTypeBuilder<CntOwnLeaseType> builder)
        {
            builder.ToTable("CntOwnLeaseTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CntOwnLeaseTypeName).IsRequired().HasMaxLength(30);
            builder.Property(c => c.CntOwnLeaseTypeLocalName).IsRequired().HasMaxLength(30);

            // Seed Data
            builder.HasData(
                new CntOwnLeaseType { Id = 1, CntOwnLeaseTypeName = "Owned", CntOwnLeaseTypeLocalName = "ملکی" },
                new CntOwnLeaseType { Id = 2, CntOwnLeaseTypeName = "Leased", CntOwnLeaseTypeLocalName = "استیجاری" }
            );
        }
    }




    public class EventActionSourceConfiguration : IEntityTypeConfiguration<EventActionSource>
    {
        public void Configure(EntityTypeBuilder<EventActionSource> builder)
        {
            builder.ToTable("EventActionSources", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.EventActionSourceName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new EventActionSource { Id = 1, EventActionSourceName = "Manual" },
                new EventActionSource { Id = 2, EventActionSourceName = "EDI" },
                new EventActionSource { Id = 3, EventActionSourceName = "System" },
                new EventActionSource { Id = 4, EventActionSourceName = "API" },
                new EventActionSource { Id = 5, EventActionSourceName = "GPS" }
            );
        }
    }
    public class ContainerConditionStatusConfiguration : IEntityTypeConfiguration<ContainerConditionStatus>
    {
        public void Configure(EntityTypeBuilder<ContainerConditionStatus> builder)
        {
            builder.ToTable("ContainerConditionStatuses", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerConditionStatusName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new ContainerConditionStatus { Id = 1, ContainerConditionStatusName = "Sound" },
                new ContainerConditionStatus { Id = 2, ContainerConditionStatusName = "Damaged" },
                new ContainerConditionStatus { Id = 3, ContainerConditionStatusName = "Severe Damaged" },
                new ContainerConditionStatus { Id = 4, ContainerConditionStatusName = "Not Available" }
            );
        }
    }
    public class ContainerDamageSeverityConfiguration : IEntityTypeConfiguration<ContainerDamageSeverity>
    {
        public void Configure(EntityTypeBuilder<ContainerDamageSeverity> builder)
        {
            builder.ToTable("ContainerDamageSeverities", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerDamageSeverityName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new ContainerDamageSeverity { Id = 1, ContainerDamageSeverityName = "Minor" },
                new ContainerDamageSeverity { Id = 2, ContainerDamageSeverityName = "Major" },
                new ContainerDamageSeverity { Id = 3, ContainerDamageSeverityName = "Critical" }
            );
        }
    }


    public class ContainerDamageAreaConfiguration : IEntityTypeConfiguration<ContainerDamageArea>
    {
        public void Configure(EntityTypeBuilder<ContainerDamageArea> builder)
        {
            builder.ToTable("ContainerDamageAreas", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerDamageAreaName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new ContainerDamageArea { Id = 1, ContainerDamageAreaName = "Door" },
                new ContainerDamageArea { Id = 2, ContainerDamageAreaName = "Roof" },
                new ContainerDamageArea { Id = 3, ContainerDamageAreaName = "Side Panel" },
                new ContainerDamageArea { Id = 4, ContainerDamageAreaName = "Floor" },
                new ContainerDamageArea { Id = 5, ContainerDamageAreaName = "Corner Post" }
            );
        }
    }

    public class ContainerDamageTypeConfiguration : IEntityTypeConfiguration<ContainerDamageType>
    {
        public void Configure(EntityTypeBuilder<ContainerDamageType> builder)
        {
            builder.ToTable("ContainerDamageTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerDamageTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new ContainerDamageType { Id = 1, ContainerDamageTypeName = "Dent" }, //خوردکگی
                new ContainerDamageType { Id = 2, ContainerDamageTypeName = "Hole" },  //سوراخ
                new ContainerDamageType { Id = 3, ContainerDamageTypeName = "Rust" },  //زنگ
                new ContainerDamageType { Id = 4, ContainerDamageTypeName = "Bent" },  //خم شده
                new ContainerDamageType { Id = 5, ContainerDamageTypeName = "Crack" }  //ترک
            );
        }
    }








    public class DepositTypeConfiguration : IEntityTypeConfiguration<DepositType>
    {
        public void Configure(EntityTypeBuilder<DepositType> builder)
        {
            builder.ToTable("DepositTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DepositTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new DepositType { Id = 1, DepositTypeName = "Cash Deposit" },
                new DepositType { Id = 2, DepositTypeName = "NonCash Deposit" }
            );
        }
    }
    public class DetentionDiscountTypeConfiguration : IEntityTypeConfiguration<DetentionDiscountType>
    {
        public void Configure(EntityTypeBuilder<DetentionDiscountType> builder)
        {
            builder.ToTable("DetentionDiscountTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DetentionDiscountTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new DetentionDiscountType { Id = 1, DetentionDiscountTypeName = "Normal" },
                new DetentionDiscountType { Id = 2, DetentionDiscountTypeName = "Special" }
            );
        }
    }
    public class DetentionStatusConfiguration : IEntityTypeConfiguration<DetentionStatus>
    {
        public void Configure(EntityTypeBuilder<DetentionStatus> builder)
        {
            builder.ToTable("DetentionStatuses", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DetentionStatusName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new DetentionStatus { Id = -1, DetentionStatusName = "InActive" },
                new DetentionStatus { Id = 1, DetentionStatusName = "Active" },
                new DetentionStatus { Id = 2, DetentionStatusName = "Converted To Invoice" }
            );
        }
    }
    public class DetentionUpdaterConfiguration : IEntityTypeConfiguration<DetentionUpdater>
    {
        public void Configure(EntityTypeBuilder<DetentionUpdater> builder)
        {
            builder.ToTable("DetentionUpdaters", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DetentionUpdaterName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new DetentionUpdater { Id = 1, DetentionUpdaterName = "Automatic" },
                new DetentionUpdater { Id = 2, DetentionUpdaterName = "User" },
                new DetentionUpdater { Id = 3, DetentionUpdaterName = "Customer" }
            );
        }
    }
    public class DGClassConfiguration : IEntityTypeConfiguration<DGClass>
    {
        public void Configure(EntityTypeBuilder<DGClass> builder)
        {
            builder.ToTable("DGClasses", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DGClassName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.DGClassNo).IsRequired().HasMaxLength(10);
            builder.Property(c => c.DGClassDescription).IsRequired().HasMaxLength(200);

            // Seed Data
            builder.HasData(
                new DGClass { Id = 1, DGClassName = "Explosives", DGClassNo = "1", DGClassDescription = "" },
                new DGClass { Id = 2, DGClassName = "Gases", DGClassNo = "2", DGClassDescription = "" },
                new DGClass { Id = 3, DGClassName = "Flammable Liquids", DGClassNo = "3", DGClassDescription = "" },
                new DGClass { Id = 4, DGClassName = "Flammable Solids; Spontaneous Combustibles; Dangerous When Wet Materials", DGClassNo = "4", DGClassDescription = "" },
                new DGClass { Id = 5, DGClassName = "Oxidizers; Organic Peroxides", DGClassNo = "5", DGClassDescription = "" },
                new DGClass { Id = 6, DGClassName = "Toxic Substances; Infectious Substances", DGClassNo = "6", DGClassDescription = "" },
                new DGClass { Id = 7, DGClassName = "Radioactive", DGClassNo = "7", DGClassDescription = "" },
                new DGClass { Id = 8, DGClassName = "Corrosive", DGClassNo = "8", DGClassDescription = "" },
                new DGClass { Id = 9, DGClassName = "Miscellaneous", DGClassNo = "9", DGClassDescription = "" },
                new DGClass { Id = 21, DGClassName = "Flammable Gas", DGClassNo = "2.1", DGClassDescription = "" },
                new DGClass { Id = 22, DGClassName = "NonFlammable Gas", DGClassNo = "2.2", DGClassDescription = "" },
                new DGClass { Id = 23, DGClassName = "Toxic Gas", DGClassNo = "2.3", DGClassDescription = "" },
                new DGClass { Id = 41, DGClassName = "Flammable Solid", DGClassNo = "4.1", DGClassDescription = "" },
                new DGClass { Id = 42, DGClassName = "Spontaneously Combustible", DGClassNo = "4.2", DGClassDescription = "" },
                new DGClass { Id = 43, DGClassName = "Dangerous When Wet", DGClassNo = "4.3", DGClassDescription = "" },
                new DGClass { Id = 51, DGClassName = "Oxidizing Substances", DGClassNo = "5.1", DGClassDescription = "" },
                new DGClass { Id = 52, DGClassName = "Organic Peroxides", DGClassNo = "5.2", DGClassDescription = "" },
                new DGClass { Id = 61, DGClassName = "Toxic Substances", DGClassNo = "6.1", DGClassDescription = "" },
                new DGClass { Id = 62, DGClassName = "Infectious Substances", DGClassNo = "6.2", DGClassDescription = "" }
            );
        }
    }
    public class DGPackingGroupConfiguration : IEntityTypeConfiguration<DGPackingGroup>
    {
        public void Configure(EntityTypeBuilder<DGPackingGroup> builder)
        {
            builder.ToTable("DGPackingGroups", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DGPackingGroupCode).IsRequired().HasMaxLength(5);
            builder.Property(c => c.DGPackingGroupDescription).IsRequired().HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new DGPackingGroup { Id = 1, DGPackingGroupCode = "I", DGPackingGroupDescription = "High Danger" },
                new DGPackingGroup { Id = 2, DGPackingGroupCode = "II", DGPackingGroupDescription = "Medium Danger" },
                new DGPackingGroup { Id = 3, DGPackingGroupCode = "III", DGPackingGroupDescription = "Low Danger" }
            );
        }
    }
    public class TransportTypeConfiguration : IEntityTypeConfiguration<TransportType>
    {
        public void Configure(EntityTypeBuilder<TransportType> builder)
        {
            builder.ToTable("TransportTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TransportTypeName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new TransportType { Id = 1, TransportTypeName = "Sea" },
                new TransportType { Id = 2, TransportTypeName = "Road" },
                new TransportType { Id = 3, TransportTypeName = "Rail" },
                new TransportType { Id = 4, TransportTypeName = "Air" },
                new TransportType { Id = 5, TransportTypeName = "MultiModal" }
            );
        }
    }
    public class DGRegulationSetConfiguration : IEntityTypeConfiguration<DGRegulationSet>
    {
        public void Configure(EntityTypeBuilder<DGRegulationSet> builder)
        {
            builder.ToTable("DGRegulationSets", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DGRegulationSetName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new DGRegulationSet { Id = 1, TransportTypeId = EnumTransportType.Sea, DGRegulationSetName = "IMDG" },
                new DGRegulationSet { Id = 2, TransportTypeId = EnumTransportType.Road, DGRegulationSetName = "ADR" },
                new DGRegulationSet { Id = 3, TransportTypeId = EnumTransportType.Rail, DGRegulationSetName = "RID" },
                new DGRegulationSet { Id = 4, TransportTypeId = EnumTransportType.Air, DGRegulationSetName = "IATA" }
            );
        }
    }
    public class DischargeLoadConfiguration : IEntityTypeConfiguration<DischargeLoad>
    {
        public void Configure(EntityTypeBuilder<DischargeLoad> builder)
        {
            builder.ToTable("DischargeLoad", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DischargeLoadTitle).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new DischargeLoad { Id = 1, DischargeLoadTitle = "Discharge" },
                new DischargeLoad { Id = 2, DischargeLoadTitle = "Loading" }
            );
        }
    }
    public class FinanceDirectionConfiguration : IEntityTypeConfiguration<FinanceDirection>
    {
        public void Configure(EntityTypeBuilder<FinanceDirection> builder)
        {
            builder.ToTable("FinanceDirections", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FinanceDirectionName).IsRequired().HasMaxLength(60);
            builder.Property(c => c.FinanceDirectionLocalName).IsRequired().HasMaxLength(60);
            builder.Property(c => c.ChargeContextName).IsRequired().HasMaxLength(60);
            builder.Property(c => c.InvoiceContextName).IsRequired().HasMaxLength(60);
            builder.Property(c => c.PaymentDocumentContextName).IsRequired().HasMaxLength(60);

            // Seed Data
            builder.HasData(
                new FinanceDirection { Id = 1, FinanceDirectionName = "Receive(Revenue-Sell)", FinanceDirectionLocalName = "دریافت توسط شرکت(درآمد-فروش)", ChargeContextName = "Sell(درآمد)", InvoiceContextName = "Sell(فروش)", PaymentDocumentContextName = "Gather(دریافتی توسط شرکت)" },
                new FinanceDirection { Id = 2, FinanceDirectionName = "Payment(Cost-Buy)", FinanceDirectionLocalName = "پرداخت / واریز توسط شرکت(هزینه-خرید)", ChargeContextName = "Buy(هزینه)", InvoiceContextName = "Buy(خرید)", PaymentDocumentContextName = "Pay(پرداخت توسط شرکت)" }
            );
        }
    }

    public class FollowUpPriorityConfiguration : IEntityTypeConfiguration<FollowUpPriority>
    {
        public void Configure(EntityTypeBuilder<FollowUpPriority> builder)
        {
            builder.ToTable("FollowUpPriorities", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FollowUpPriorityName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new FollowUpPriority { Id = 1, FollowUpPriorityName = "Critical" },
                new FollowUpPriority { Id = 2, FollowUpPriorityName = "High" },
                new FollowUpPriority { Id = 3, FollowUpPriorityName = "Medium" },
                new FollowUpPriority { Id = 4, FollowUpPriorityName = "Low" }
            );
        }
    }
    public class FollowUpStatusConfiguration : IEntityTypeConfiguration<FollowUpStatus>
    {
        public void Configure(EntityTypeBuilder<FollowUpStatus> builder)
        {
            builder.ToTable("FollowUpStatuses", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FollowUpStatusName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new FollowUpStatus { Id = 1, FollowUpStatusName = "Pending" },
                new FollowUpStatus { Id = 2, FollowUpStatusName = "Completed" },
                new FollowUpStatus { Id = 3, FollowUpStatusName = "Overdue" },
                new FollowUpStatus { Id = 4, FollowUpStatusName = "Cancelled" }
            );
        }
    }
    public class FullMTConfiguration : IEntityTypeConfiguration<FullMT>
    {
        public void Configure(EntityTypeBuilder<FullMT> builder)
        {
            builder.ToTable("FullMT", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FullMTName).IsRequired().HasMaxLength(15);
            builder.Property(c => c.FullMTCode).IsRequired().HasMaxLength(5);

            // Seed Data
            builder.HasData(
                new FullMT { Id = 4, FullMTCode = "4", FullMTName = "Empty" },
                new FullMT { Id = 5, FullMTCode = "5", FullMTName = "Full" }
            );
        }
    }
    public class HologramStatusConfiguration : IEntityTypeConfiguration<HologramStatus>
    {
        public void Configure(EntityTypeBuilder<HologramStatus> builder)
        {
            builder.ToTable("HologramStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.HologramStatusName).IsRequired().HasMaxLength(30);

            // Seed Data
            builder.HasData(
                new HologramStatus { Id = 1, HologramStatusName = "Available" },
                new HologramStatus { Id = 2, HologramStatusName = "Assigned" },
                new HologramStatus { Id = 3, HologramStatusName = "Voided" },
                new HologramStatus { Id = 4, HologramStatusName = "Lost" }
            );
        }
    }
    public class HSLevelConfiguration : IEntityTypeConfiguration<HSLevel>
    {
        public void Configure(EntityTypeBuilder<HSLevel> builder)
        {
            builder.ToTable("HSLevels", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.HSLevelName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new HSLevel { Id = 2, HSLevelName = "Chapter" },
                new HSLevel { Id = 4, HSLevelName = "Heading" },
                new HSLevel { Id = 6, HSLevelName = "SubHeading" }
            );
        }
    }
    public class TradeTypeConfiguration : IEntityTypeConfiguration<TradeType>
    {
        public void Configure(EntityTypeBuilder<TradeType> builder)
        {
            builder.ToTable("TradeTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TradeTypeName).IsRequired().HasMaxLength(20);
            builder.Property(c => c.TradeTypeLocalName).HasMaxLength(30);

            // Seed Data
            builder.HasData(
                new TradeType { Id = 1, TradeTypeName = "Import", TradeTypeLocalName = "واردات" },
                new TradeType { Id = 2, TradeTypeName = "Export", TradeTypeLocalName = "صادرات" },
                new TradeType { Id = 3, TradeTypeName = "Internal Transit", TradeTypeLocalName = "ترانزیت داخلی" },
                new TradeType { Id = 4, TradeTypeName = "External Transit", TradeTypeLocalName = "ترانزیت خارجی" },
                new TradeType { Id = 5, TradeTypeName = "Tranship", TradeTypeLocalName = "ترانشیپ" },
                new TradeType { Id = 6, TradeTypeName = "Return", TradeTypeLocalName = "مرجوعی" },
                new TradeType { Id = 7, TradeTypeName = "Import Empty Container", TradeTypeLocalName = "واردات کانتینر خالی" },
                new TradeType { Id = 8, TradeTypeName = "Rajaee S.E.Z", TradeTypeLocalName = "منطقه ویژه اقتصادی رجائی" },

                new TradeType { Id = 20, TradeTypeName = "Cross Trade", TradeTypeLocalName = "کراس ترید" },
                new TradeType { Id = 21, TradeTypeName = "Domestic", TradeTypeLocalName = "حمل داخلی" },
                new TradeType { Id = 22, TradeTypeName = "Re-Export", TradeTypeLocalName = "صادرات مجدد" },
                new TradeType { Id = 23, TradeTypeName = "Temporary", TradeTypeLocalName = "ورود موقت" },
                new TradeType { Id = 24, TradeTypeName = "Cabotage", TradeTypeLocalName = "کابوتاژ" }
            );
        }
    }
    public class LeadSourceConfiguration : IEntityTypeConfiguration<LeadSource>
    {
        public void Configure(EntityTypeBuilder<LeadSource> builder)
        {
            builder.ToTable("LeadSources", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LeadSourceName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new LeadSource { Id = 1, LeadSourceName = "Web Site" },
                new LeadSource { Id = 2, LeadSourceName = "Google Ads" },
                new LeadSource { Id = 3, LeadSourceName = "LinkedIn" },
                new LeadSource { Id = 4, LeadSourceName = "Email Campaign" },
                new LeadSource { Id = 5, LeadSourceName = "Exhibition" },
                new LeadSource { Id = 6, LeadSourceName = "Referral" }
            );
        }
    }
    public class LeadStatusConfiguration : IEntityTypeConfiguration<LeadStatus>
    {
        public void Configure(EntityTypeBuilder<LeadStatus> builder)
        {
            builder.ToTable("LeadStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LeadStatusName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.LeadStatusLocalName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new LeadStatus { Id = 1, LeadStatusName = "New", LeadStatusLocalName = "جدید" },
                new LeadStatus { Id = 2, LeadStatusName = "Contacted", LeadStatusLocalName = "تماس گرفته شده" },
                new LeadStatus { Id = 3, LeadStatusName = "Qualified", LeadStatusLocalName = "واجد شرایط" },
                new LeadStatus { Id = 4, LeadStatusName = "Quoted", LeadStatusLocalName = "پیشنهاد ارسال شده" },
                new LeadStatus { Id = 5, LeadStatusName = "Negotiation", LeadStatusLocalName = "مذاکره" },
                new LeadStatus { Id = 6, LeadStatusName = "WON/Converted", LeadStatusLocalName = "برنده شده/تبدیل به مشتری" },
                new LeadStatus { Id = 7, LeadStatusName = "Lost", LeadStatusLocalName = "از دست رفته" },
                new LeadStatus { Id = 8, LeadStatusName = "DORMANT", LeadStatusLocalName = "غیرفعال" }
            );
        }
    }
    public class LineForwarderConfiguration : IEntityTypeConfiguration<LineForwarderType>
    {
        public void Configure(EntityTypeBuilder<LineForwarderType> builder)
        {
            builder.ToTable("LineForwarderTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LineForwarderTypeName).IsRequired().HasMaxLength(10);

            // Seed Data
            builder.HasData(
                new LineForwarderType { Id = 1, LineForwarderTypeName = "Line" },
                new LineForwarderType { Id = 2, LineForwarderTypeName = "Forwarder" }
            );
        }
    }
    public class ManifestTypeConfiguration : IEntityTypeConfiguration<ManifestType>
    {
        public void Configure(EntityTypeBuilder<ManifestType> builder)
        {
            builder.ToTable("ManifestTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ManifestTypeName).IsRequired().HasMaxLength(100);

            // Seed Data
            builder.HasData(
                new ManifestType { Id = 1, ManifestTypeName = "Cargo Manifest" },
                new ManifestType { Id = 2, ManifestTypeName = "Container Manifest" },
                new ManifestType { Id = 3, ManifestTypeName = "Hazardous Manifest" },
                new ManifestType { Id = 4, ManifestTypeName = "Reefer Manifest" },
                new ManifestType { Id = 5, ManifestTypeName = "Out Of Gauge Manifest" },
                new ManifestType { Id = 6, ManifestTypeName = "Vessel Manifest" },
                new ManifestType { Id = 7, ManifestTypeName = "Flight Manifest" },
                new ManifestType { Id = 8, ManifestTypeName = "Truck Manifest" },
                new ManifestType { Id = 9, ManifestTypeName = "Rail Manifest" },
                new ManifestType { Id = 10, ManifestTypeName = "Customs Manifest" },
                new ManifestType { Id = 11, ManifestTypeName = "Transshipment Manifest" }
            );
        }
    }
    public class MeasurementUnitCategoryConfiguration : IEntityTypeConfiguration<MeasurementUnitCategory>
    {
        public void Configure(EntityTypeBuilder<MeasurementUnitCategory> builder)
        {
            builder.ToTable("MeasurementUnitCategories", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.MeasurementUnitCategoryName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new MeasurementUnitCategory { Id = 1, MeasurementUnitCategoryName = "Weigth" },
                new MeasurementUnitCategory { Id = 2, MeasurementUnitCategoryName = "Volume" },
                new MeasurementUnitCategory { Id = 3, MeasurementUnitCategoryName = "Length" },
                new MeasurementUnitCategory { Id = 4, MeasurementUnitCategoryName = "Area" },
                new MeasurementUnitCategory { Id = 5, MeasurementUnitCategoryName = "Quantity" },
                new MeasurementUnitCategory { Id = 6, MeasurementUnitCategoryName = "Temperature" }
            );
        }
    }
    public class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
    {
        public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
        {
            builder.ToTable("MeasurementUnits", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.MeasurementUnitName).IsRequired().HasMaxLength(45);
            builder.Property(c => c.MeasurementUnitLocalName).IsRequired().HasMaxLength(45);
            builder.HasOne(x => x.MeasurementUnitCategory).WithMany().HasForeignKey(x => x.MeasurementUnitCategoryId);
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Seed Data
            builder.HasData(
                new MeasurementUnit { Id = 1, SystemDefined = true, MeasurementUnitCategoryId = 1, MeasurementUnitName = "KGS", MeasurementUnitLocalName = "کیلوگرم" },
                new MeasurementUnit { Id = 2, SystemDefined = true, MeasurementUnitCategoryId = 2, MeasurementUnitName = "CBM", MeasurementUnitLocalName = "متر مکعب" },
                new MeasurementUnit { Id = 3, SystemDefined = true, MeasurementUnitCategoryId = 3, MeasurementUnitName = "Centimeters", MeasurementUnitLocalName = "سانتی متر" },
                new MeasurementUnit { Id = 4, SystemDefined = true, MeasurementUnitCategoryId = 6, MeasurementUnitName = "Celesius", MeasurementUnitLocalName = "سلسیوس" }
            );
        }
    }

    public class PaymentTypeConfiguration : IEntityTypeConfiguration<PaymentType>
    {
        public void Configure(EntityTypeBuilder<PaymentType> builder)
        {
            builder.ToTable("PaymentTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PaymentTypeName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new PaymentType { Id = 1, PaymentTypeName = "Prepaid", SortOrder = 1 },
                new PaymentType { Id = 2, PaymentTypeName = "Collect", SortOrder = 2 },
                new PaymentType { Id = 3, PaymentTypeName = "Payable AT", SortOrder = 3 },
                new PaymentType { Id = 4, PaymentTypeName = "Third Party", SortOrder = 4 }
            );
        }
    }
    public class PersonalityTypeConfiguration : IEntityTypeConfiguration<PersonalityType>
    {
        public void Configure(EntityTypeBuilder<PersonalityType> builder)
        {
            builder.ToTable("PersonalityTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PersonalityTypeName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new PersonalityType { Id = 1, PersonalityTypeName = "Company" },
                new PersonalityType { Id = 2, PersonalityTypeName = "Individual" },
                new PersonalityType { Id = 3, PersonalityTypeName = "Government" },
                new PersonalityType { Id = 4, PersonalityTypeName = "Branch" }
            );
        }
    }
    public class PortOperationTypeConfiguration : IEntityTypeConfiguration<PortOperationType>
    {
        public void Configure(EntityTypeBuilder<PortOperationType> builder)
        {
            builder.ToTable("PortOperationTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PortOperationTypeName).IsRequired().HasMaxLength(30);

            // Seed Data
            builder.HasData(
                new PortOperationType { Id = 1, PortOperationTypeName = "Discharge" },
                new PortOperationType { Id = 2, PortOperationTypeName = "Loading" },
                new PortOperationType { Id = 3, PortOperationTypeName = "Both" }
            );
        }
    }
    public class ReturnCashStatusConfiguration : IEntityTypeConfiguration<ReturnCashStatus>
    {
        public void Configure(EntityTypeBuilder<ReturnCashStatus> builder)
        {
            builder.ToTable("ReturnCashStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ReturnCashStatusName).IsRequired().HasMaxLength(40);

            // Seed Data
            builder.HasData(
                new ReturnCashStatus { Id = 1, ReturnCashStatusName = "Registered" },
                new ReturnCashStatus { Id = 2, ReturnCashStatusName = "Accepted" },
                new ReturnCashStatus { Id = 3, ReturnCashStatusName = "Commercial Confirmed" },
                new ReturnCashStatus { Id = 4, ReturnCashStatusName = "Finance Confirmed" },
                new ReturnCashStatus { Id = 9, ReturnCashStatusName = "Rejected" }
            );
        }
    }
    public class RiskLevelConfiguration : IEntityTypeConfiguration<RiskLevel>
    {
        public void Configure(EntityTypeBuilder<RiskLevel> builder)
        {
            builder.ToTable("RiskLevels", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RiskLevelName).IsRequired().HasMaxLength(15);

            // Seed Data
            builder.HasData(
                new RiskLevel { Id = -1, RiskLevelName = "None" },
                new RiskLevel { Id = 1, RiskLevelName = "Low" },
                new RiskLevel { Id = 2, RiskLevelName = "Medium" },
                new RiskLevel { Id = 3, RiskLevelName = "High" }
            );
        }
    }
    public class RoleTypeConfiguration : IEntityTypeConfiguration<RoleType>
    {
        public void Configure(EntityTypeBuilder<RoleType> builder)
        {
            builder.ToTable("RoleTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RoleTypeName).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new RoleType { Id = 1, RoleTypeName = "Carrier" },
                new RoleType { Id = 2, RoleTypeName = "Customer" },
                new RoleType { Id = 3, RoleTypeName = "Agent" },
                new RoleType { Id = 4, RoleTypeName = "Courier Company" },
                new RoleType { Id = 5, RoleTypeName = "Driver" },
                new RoleType { Id = 6, RoleTypeName = "GPS Provider" },

                new RoleType { Id = 21, RoleTypeName = "Shipping Line" },
                new RoleType { Id = 22, RoleTypeName = "NVOCC" },
                new RoleType { Id = 23, RoleTypeName = "Air Line" },
                new RoleType { Id = 24, RoleTypeName = "Road Transit" },
                new RoleType { Id = 25, RoleTypeName = "Rail Transit" },
                new RoleType { Id = 26, RoleTypeName = "Forwarder" },
                new RoleType { Id = 27, RoleTypeName = "Warehouse" },

                new RoleType { Id = 31, RoleTypeName = "Booker Client" },

                new RoleType { Id = 51, RoleTypeName = "Shipper" },
                new RoleType { Id = 52, RoleTypeName = "Consignee" },
                new RoleType { Id = 53, RoleTypeName = "Notify Party" },
                new RoleType { Id = 54, RoleTypeName = "Lawyer" },
                new RoleType { Id = 55, RoleTypeName = "Supplier" },

                new RoleType { Id = 61, RoleTypeName = "Sender" },
                new RoleType { Id = 62, RoleTypeName = "Receiver" },
                new RoleType { Id = 63, RoleTypeName = "Deliver" },

                new RoleType { Id = 71, RoleTypeName = "Origin Agent" },
                new RoleType { Id = 72, RoleTypeName = "Destination Agent" }

            );
        }
    }
    public class RouteModeConfiguration : IEntityTypeConfiguration<RouteMode>
    {
        public void Configure(EntityTypeBuilder<RouteMode> builder)
        {
            builder.ToTable("RouteModes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RouteModeName).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new RouteMode { Id = 1, RouteModeName = "Sea" },
                new RouteMode { Id = 2, RouteModeName = "Road" },
                new RouteMode { Id = 3, RouteModeName = "Rail" },
                new RouteMode { Id = 4, RouteModeName = "Air" },
                new RouteMode { Id = 5, RouteModeName = "MultiModal" }
            );
        }
    }
    public class SendToolTypeConfiguration : IEntityTypeConfiguration<SendToolType>
    {
        public void Configure(EntityTypeBuilder<SendToolType> builder)
        {
            builder.ToTable("SendToolTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.SendToolTypeName).IsRequired().HasMaxLength(20);

            // Seed Data
            builder.HasData(
                new SendToolType { Id = 1, SendToolTypeName = "Email" },
                new SendToolType { Id = 2, SendToolTypeName = "SMS" },
                new SendToolType { Id = 3, SendToolTypeName = "Portal/App" }
            );
        }
    }
    public class ShipmentStatusConfiguration : IEntityTypeConfiguration<ShipmentStatus>
    {
        public void Configure(EntityTypeBuilder<ShipmentStatus> builder)
        {
            builder.ToTable("ShipmentStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ShipmentStatusName).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new ShipmentStatus { Id = 1, ShipmentStatusName = "Draft" },
                new ShipmentStatus { Id = 2, ShipmentStatusName = "Planned" },
                new ShipmentStatus { Id = 3, ShipmentStatusName = "Booked" },
                new ShipmentStatus { Id = 4, ShipmentStatusName = "In_Execution" },
                new ShipmentStatus { Id = 5, ShipmentStatusName = "Arrived" },
                new ShipmentStatus { Id = 6, ShipmentStatusName = "Delivered" },
                new ShipmentStatus { Id = 7, ShipmentStatusName = "Closed" },
                new ShipmentStatus { Id = 99, ShipmentStatusName = "Cancled" }
            );
        }
    }


    public class ShipmentLegStatusConfiguration : IEntityTypeConfiguration<ShipmentLegStatus>
    {
        public void Configure(EntityTypeBuilder<ShipmentLegStatus> builder)
        {
            builder.ToTable("ShipmentLegStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ShipmentLegStatusName).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new ShipmentLegStatus { Id = 1, ShipmentLegStatusName = "Planned" },
                new ShipmentLegStatus { Id = 2, ShipmentLegStatusName = "Ready" },
                new ShipmentLegStatus { Id = 3, ShipmentLegStatusName = "InProgress" },
                new ShipmentLegStatus { Id = 4, ShipmentLegStatusName = "Completed" },
                new ShipmentLegStatus { Id = 5, ShipmentLegStatusName = "Canceled" },
                new ShipmentLegStatus { Id = 6, ShipmentLegStatusName = "Skipped" }
            );
        }
    }

    public class TariffTypeConfiguration : IEntityTypeConfiguration<TariffType>
    {
        public void Configure(EntityTypeBuilder<TariffType> builder)
        {
            builder.ToTable("TariffTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TariffTypeName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.TariffTypeLocalName).IsRequired().HasMaxLength(100);

            // Seed Data
            builder.HasData(
                new TariffType { Id = 1, TariffTypeName = "Ocean Freight Tariff", TariffTypeLocalName = "تعرفه کرایه حمل دریایی", SortOrder = 1 },
                new TariffType { Id = 2, TariffTypeName = "Local Charges Tariff", TariffTypeLocalName = "تعرفه هزینه های محلی", SortOrder = 2 },
                new TariffType { Id = 3, TariffTypeName = "Detention Tariff", TariffTypeLocalName = "تعرفه دیرکرد کانتینر", SortOrder = 4 },
                new TariffType { Id = 4, TariffTypeName = "THC Tariff", TariffTypeLocalName = "تعرفه هزینه های تی اچ سی", SortOrder = 3 },
                new TariffType { Id = 5, TariffTypeName = "Forwarder Services Tariff", TariffTypeLocalName = "تعرفه خدمات فوروراردری", SortOrder = 5 }
            );
        }
    }



    public class TariffStatusConfiguration : IEntityTypeConfiguration<TariffStatus>
    {
        public void Configure(EntityTypeBuilder<TariffStatus> builder)
        {
            builder.ToTable("TariffStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TariffStatusName).IsRequired().HasMaxLength(25);

            // Seed Data
            builder.HasData(
                new TariffStatus { Id = -1, TariffStatusName = "Draft" },
                new TariffStatus { Id = 1, TariffStatusName = "Active" },
                new TariffStatus { Id = 2, TariffStatusName = "Expired" },
                new TariffStatus { Id = 3, TariffStatusName = "Archived" }
            );
        }
    }
    public class TelexReleaseStatusConfiguration : IEntityTypeConfiguration<TelexReleaseStatus>
    {
        public void Configure(EntityTypeBuilder<TelexReleaseStatus> builder)
        {
            builder.ToTable("TelexReleaseStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TelexReleaseStatusName).IsRequired().HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new TelexReleaseStatus { Id = 10, TelexReleaseStatusName = "Draft" },
                new TelexReleaseStatus { Id = 20, TelexReleaseStatusName = "Pending" },
                new TelexReleaseStatus { Id = 30, TelexReleaseStatusName = "Waiting For BL" },
                new TelexReleaseStatus { Id = 40, TelexReleaseStatusName = "Waiting For Line Approval" },
                new TelexReleaseStatus { Id = 50, TelexReleaseStatusName = "Approved" },
                new TelexReleaseStatus { Id = 60, TelexReleaseStatusName = "Rejected" },
                new TelexReleaseStatus { Id = 70, TelexReleaseStatusName = "Message Sent" },
                new TelexReleaseStatus { Id = 80, TelexReleaseStatusName = "Completed" },
                new TelexReleaseStatus { Id = 90, TelexReleaseStatusName = "Canceled" }
            );
        }
    }
    public class TIPStatusConfiguration : IEntityTypeConfiguration<TIPStatus>
    {
        public void Configure(EntityTypeBuilder<TIPStatus> builder)
        {
            builder.ToTable("TIPStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TIPStatusName).IsRequired().HasMaxLength(25);

            // Seed Data
            builder.HasData(
                new TIPStatus { Id = 1, TIPStatusName = "Active" },
                new TIPStatus { Id = 2, TIPStatusName = "Closed" },
                new TIPStatus { Id = 3, TIPStatusName = "Extended" },
                new TIPStatus { Id = 9, TIPStatusName = "Canceled" }
            );
        }
    }
    public class TIRCarnetStatusConfiguration : IEntityTypeConfiguration<TIRCarnetStatus>
    {
        public void Configure(EntityTypeBuilder<TIRCarnetStatus> builder)
        {
            builder.ToTable("TIRCarnetStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TIRCarnetStatusTitle).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new TIRCarnetStatus { Id = -1, TIRCarnetStatusTitle = "Draft" },
                new TIRCarnetStatus { Id = 1, TIRCarnetStatusTitle = "In Use" },
                new TIRCarnetStatus { Id = 2, TIRCarnetStatusTitle = "Closed" },
                new TIRCarnetStatus { Id = 3, TIRCarnetStatusTitle = "Returned" },
                new TIRCarnetStatus { Id = 4, TIRCarnetStatusTitle = "Settled" },
                new TIRCarnetStatus { Id = 5, TIRCarnetStatusTitle = "Expired" },
                new TIRCarnetStatus { Id = 9, TIRCarnetStatusTitle = "Canceled" }
            );
        }
    }
    public class TransitMethodConfiguration : IEntityTypeConfiguration<TransitMethod>
    {
        public void Configure(EntityTypeBuilder<TransitMethod> builder)
        {
            builder.ToTable("TransitMethods", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TransitMethodName).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new TransitMethod { Id = 1, TransitMethodName = "Transit By Line" },
                new TransitMethod { Id = 2, TransitMethodName = "Transit By Merchant" }
            );
        }
    }
    public class TransitTypeConfiguration : IEntityTypeConfiguration<TransitType>
    {
        public void Configure(EntityTypeBuilder<TransitType> builder)
        {
            builder.ToTable("TransitTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TransitTypeName).IsRequired().HasMaxLength(35);

            // Seed Data
            builder.HasData(
                new TransitType { Id = 1, TransitTypeName = "Internal Transit" },
                new TransitType { Id = 2, TransitTypeName = "External Transit" }
            );
        }
    }
    public class TruckTypeConfiguration : IEntityTypeConfiguration<TruckType>
    {
        public void Configure(EntityTypeBuilder<TruckType> builder)
        {
            builder.ToTable("TruckTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TruckTypeName).IsRequired().HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new TruckType { Id = 1, TruckTypeName = "Rigid Truck" },
                new TruckType { Id = 2, TruckTypeName = "Semi Trailer" },
                new TruckType { Id = 3, TruckTypeName = "Flat bed" },
                new TruckType { Id = 4, TruckTypeName = "Box Trailer" },
                new TruckType { Id = 5, TruckTypeName = "Reefer" },
                new TruckType { Id = 6, TruckTypeName = "Tanker" },
                new TruckType { Id = 7, TruckTypeName = "Low Bed" },
                new TruckType { Id = 8, TruckTypeName = "Container Chasis" },
                new TruckType { Id = 9, TruckTypeName = "Car Carrier" },
                new TruckType { Id = 10, TruckTypeName = "Tripper" },
                new TruckType { Id = 11, TruckTypeName = "Hazmat" },
                new TruckType { Id = 12, TruckTypeName = "Pickup" }
            );
        }
    }
    public class VesselTypeConfiguration : IEntityTypeConfiguration<VesselType>
    {
        public void Configure(EntityTypeBuilder<VesselType> builder)
        {
            builder.ToTable("VesselTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.VesselTypeName).IsRequired().HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new VesselType { Id = 1, VesselTypeName = "Container" },
                new VesselType { Id = 2, VesselTypeName = "Bulk" },
                new VesselType { Id = 3, VesselTypeName = "Tanker" },
                new VesselType { Id = 4, VesselTypeName = "RoRo" },
                new VesselType { Id = 5, VesselTypeName = "General Cargo" },
                new VesselType { Id = 6, VesselTypeName = "Barge" },
                new VesselType { Id = 7, VesselTypeName = "Multi-Purpose" },
                new VesselType { Id = 8, VesselTypeName = "LNG Carrier" },
                new VesselType { Id = 9, VesselTypeName = "LPG Carrier" }
            );
        }
    }
    public class PettyCashExpenseStatusConfiguration : IEntityTypeConfiguration<PettyCashExpenseStatus>
    {
        public void Configure(EntityTypeBuilder<PettyCashExpenseStatus> builder)
        {
            builder.ToTable("PettyCashExpenseStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.StatusLocalName).HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new PettyCashExpenseStatus { Id = 1, StatusName = "Draft" },
                new PettyCashExpenseStatus { Id = 2, StatusName = "Submitted" },
                new PettyCashExpenseStatus { Id = 3, StatusName = "Approved" },
                new PettyCashExpenseStatus { Id = 9, StatusName = "Rejected" }
            );
        }
    }

    public class PettyCashRechargeStatusConfiguration : IEntityTypeConfiguration<PettyCashRechargeStatus>
    {
        public void Configure(EntityTypeBuilder<PettyCashRechargeStatus> builder)
        {
            builder.ToTable("PettyCashRechargeStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).HasMaxLength(50);
            builder.Property(c => c.StatusLocalName).HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new PettyCashRechargeStatus { Id = 1, StatusName = "Draft" },
                new PettyCashRechargeStatus { Id = 2, StatusName = "Submitted" },
                new PettyCashRechargeStatus { Id = 3, StatusName = "Approved" },
                new PettyCashRechargeStatus { Id = 4, StatusName = "Charged" },
                new PettyCashRechargeStatus { Id = 9, StatusName = "Rejected" }
            );
        }
    }




    public class ProductRequestStatusConfiguration : IEntityTypeConfiguration<ProductRequestStatus>
    {
        public void Configure(EntityTypeBuilder<ProductRequestStatus> builder)
        {
            builder.ToTable("ProductRequestStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ProductRequestStatusName).IsRequired().HasMaxLength(45);

            // Seed Data
            builder.HasData(
                new ProductRequestStatus { Id = -1, ProductRequestStatusName = "Draft" },
                new ProductRequestStatus { Id = 1, ProductRequestStatusName = "Pending" },
                new ProductRequestStatus { Id = 2, ProductRequestStatusName = "Approved" },
                new ProductRequestStatus { Id = 3, ProductRequestStatusName = "Delivered" },
                new ProductRequestStatus { Id = 9, ProductRequestStatusName = "Rejected" }
            );
        }
    }
    public class InOutConfiguration : IEntityTypeConfiguration<InOut>
    {
        public void Configure(EntityTypeBuilder<InOut> builder)
        {
            builder.ToTable("InOut", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.InOutName).IsRequired().HasMaxLength(10);

            // Seed Data
            builder.HasData(
                new InOut { Id = 1, InOutName = "In" },
                new InOut { Id = 2, InOutName = "Out" }
            );
        }
    }

    public class AgentRoleTypeConfiguration : IEntityTypeConfiguration<AgentRoleType>
    {
        public void Configure(EntityTypeBuilder<AgentRoleType> builder)
        {
            builder.ToTable("AgentRoleTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.AgentRoleTypeName).IsRequired().HasMaxLength(100);

            // Seed Data
            builder.HasData(
                new AgentRoleType { Id = 1, AgentRoleTypeName = "Contracted Agent" },
                new AgentRoleType { Id = 2, AgentRoleTypeName = "Rate Provider" },
                new AgentRoleType { Id = 3, AgentRoleTypeName = "Tracking Partner" },
                new AgentRoleType { Id = 4, AgentRoleTypeName = "Customs Liation" }   //امور گمرکی
            );
        }
    }
    public class IncotermConfiguration : IEntityTypeConfiguration<Incoterm>
    {
        public void Configure(EntityTypeBuilder<Incoterm> builder)
        {
            builder.ToTable("Incoterms", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.IncotermCode).IsRequired().HasMaxLength(5);
            builder.Property(c => c.IncoTermTitle).IsRequired().HasMaxLength(50);
            builder.Property(c => c.IncoTermLocalTitle).IsRequired().HasMaxLength(50);
            builder.Property(c => c.IncoTermDescription).HasMaxLength(200);

            // Seed Data
            builder.HasData(
                new Incoterm { Id = 1, IncotermCode = "EXW", IncoTermTitle = "Ex Works", IncoTermDescription = "", IsDoorBased = true, IsMarineOnly = false, RequiresInsurance = false, IncoTermLocalTitle = "تحویل درب کارخانه" },
                new Incoterm { Id = 2, IncotermCode = "FCA", IncoTermTitle = "Free Carrier", IncoTermDescription = "", IsDoorBased = false, IsMarineOnly = false, RequiresInsurance = false, IncoTermLocalTitle = "تحویل به حامل" },
                new Incoterm { Id = 3, IncotermCode = "FOB", IncoTermTitle = "Free On Board", IncoTermDescription = "", IsDoorBased = false, IsMarineOnly = true, RequiresInsurance = false, IncoTermLocalTitle = "تحویل روی کشتی" },
                new Incoterm { Id = 4, IncotermCode = "CFR", IncoTermTitle = "Cost and Freight", IncoTermDescription = "", IsDoorBased = false, IsMarineOnly = true, RequiresInsurance = false, IncoTermLocalTitle = "هزینه و کرایه" },
                new Incoterm { Id = 5, IncotermCode = "CIF", IncoTermTitle = "Cost Insurance and Freight", IncoTermDescription = "", IsDoorBased = false, IsMarineOnly = true, RequiresInsurance = true, IncoTermLocalTitle = "هزینه، بیمه و کرایه" },
                new Incoterm { Id = 6, IncotermCode = "CPT", IncoTermTitle = "Carriage Paid To", IncoTermDescription = "", IsDoorBased = false, IsMarineOnly = false, RequiresInsurance = false, IncoTermLocalTitle = "کرایه پرداخت شده تا" },
                new Incoterm { Id = 7, IncotermCode = "CIP", IncoTermTitle = "Carriage and Insurance Paid To", IncoTermDescription = "", IsDoorBased = false, IsMarineOnly = false, RequiresInsurance = true, IncoTermLocalTitle = "کرایه و بیمه پرداخت شده تا" },
                new Incoterm { Id = 8, IncotermCode = "DAP", IncoTermTitle = "Delivered At Place", IncoTermDescription = "", IsDoorBased = true, IsMarineOnly = false, RequiresInsurance = false, IncoTermLocalTitle = "تحویل در مقصد" },
                new Incoterm { Id = 9, IncotermCode = "DPU", IncoTermTitle = "Delivered At Place Unloaded", IncoTermDescription = "", IsDoorBased = true, IsMarineOnly = false, RequiresInsurance = false, IncoTermLocalTitle = "تحویل در مقصد تخلیه شده" },
                new Incoterm { Id = 10, IncotermCode = "DDP", IncoTermTitle = "Delivered Duty Paid", IncoTermDescription = "", IsDoorBased = true, IsMarineOnly = false, RequiresInsurance = false, IncoTermLocalTitle = "تحویل با پرداخت عوارض" }
                //2	FAS	Free Alongside Ship
            );
        }
    }
    public class LoadTypeConfiguration : IEntityTypeConfiguration<LoadType>
    {
        public void Configure(EntityTypeBuilder<LoadType> builder)
        {
            builder.ToTable("LoadTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LoadTypeCode).IsRequired().HasMaxLength(10);
            builder.Property(c => c.LoadTypeDescription).IsRequired().HasMaxLength(50);



            // Seed Data
            builder.HasData(
                new LoadType { Id = 1, LoadTypeCode = "FCL", LoadTypeDescription = "" },
                new LoadType { Id = 2, LoadTypeCode = "FCL/LCL", LoadTypeDescription = "" },
                new LoadType { Id = 3, LoadTypeCode = "LCL/FCL", LoadTypeDescription = "" },
                new LoadType { Id = 4, LoadTypeCode = "LCL", LoadTypeDescription = "" },
                new LoadType { Id = 5, LoadTypeCode = "FTL", LoadTypeDescription = "" },
                new LoadType { Id = 6, LoadTypeCode = "LTL", LoadTypeDescription = "" },
                new LoadType { Id = 7, LoadTypeCode = "AIR", LoadTypeDescription = "" },
                new LoadType { Id = 8, LoadTypeCode = "RAIL", LoadTypeDescription = "" },
                new LoadType { Id = 99, LoadTypeCode = "OTHER", LoadTypeDescription = "" }
            );
        }
    }
    public class MovementNodeConfiguration : IEntityTypeConfiguration<MovementNode>
    {
        public void Configure(EntityTypeBuilder<MovementNode> builder)
        {
            builder.ToTable("MovementNodes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.MovementNodeName).IsRequired().HasMaxLength(15);

            // Seed Data
            builder.HasData(
                new MovementNode { Id = 1, MovementNodeName = "Port" },
                new MovementNode { Id = 2, MovementNodeName = "CFS" },
                new MovementNode { Id = 3, MovementNodeName = "Door" }
            );
        }
    }
    public class MovementTypeConfiguration : IEntityTypeConfiguration<MovementType>
    {
        public void Configure(EntityTypeBuilder<MovementType> builder)
        {
            builder.ToTable("MovementTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.MovementTypeCode).IsRequired().HasMaxLength(25);
            builder.Property(c => c.MovementTypeDescription).HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new MovementType { Id = 1, MovementTypeCode = "Port_Port", MovementTypeDescription = "", OriginNodeId = 1, DestinationNodeId = 1 },
                new MovementType { Id = 2, MovementTypeCode = "Port_Door", MovementTypeDescription = "", OriginNodeId = 1, DestinationNodeId = 3 },
                new MovementType { Id = 3, MovementTypeCode = "Door_Port", MovementTypeDescription = "", OriginNodeId = 3, DestinationNodeId = 1 },
                new MovementType { Id = 4, MovementTypeCode = "Door_Door", MovementTypeDescription = "", OriginNodeId = 3, DestinationNodeId = 3 },
                new MovementType { Id = 5, MovementTypeCode = "CFS_CFS", MovementTypeDescription = "", OriginNodeId = 2, DestinationNodeId = 2 },
                new MovementType { Id = 6, MovementTypeCode = "CFS_Port", MovementTypeDescription = "", OriginNodeId = 2, DestinationNodeId = 1 },
                new MovementType { Id = 7, MovementTypeCode = "Port_CFS", MovementTypeDescription = "", OriginNodeId = 1, DestinationNodeId = 2 },
                new MovementType { Id = 8, MovementTypeCode = "CFS_Door", MovementTypeDescription = "", OriginNodeId = 2, DestinationNodeId = 3 },
                new MovementType { Id = 9, MovementTypeCode = "Door_CFS", MovementTypeDescription = "", OriginNodeId = 3, DestinationNodeId = 2 }
            );
        }
    }
    public class PackageTypeConfiguration : IEntityTypeConfiguration<PackageType>
    {
        public void Configure(EntityTypeBuilder<PackageType> builder)
        {
            builder.ToTable("PackageTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PackageTypeCode).IsRequired().HasMaxLength(5);
            builder.Property(c => c.ENSCode).IsRequired().HasMaxLength(5);
            builder.Property(c => c.PackageTypeName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.PackageTypeDescription).IsRequired().HasMaxLength(250);
            builder.Property(c => c.NumericCode).IsRequired().HasMaxLength(100);
            builder.HasQueryFilter(x => !x.IsDeleted);


            builder.HasIndex(e => e.PackageTypeCode).HasDatabaseName("IX_PackageTypes_PackageTypeCode").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.PackageTypeName).HasDatabaseName("IX_PackageTypes_PackageTypeName").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.PackageTypeDescription).HasDatabaseName("IX_PackageTypes_PackageTypeDescription").IsUnique(false).IsClustered(false);





        }
    }

    public class CntActionTypeConfiguration : IEntityTypeConfiguration<CntActionType>
    {
        public void Configure(EntityTypeBuilder<CntActionType> builder)
        {
            builder.ToTable("CntActionTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CntActionTypeCode).IsRequired().HasMaxLength(10);
            builder.Property(c => c.CntActionTypeName).IsRequired().HasMaxLength(50);
            // Seed Data
            builder.HasData(
                new CntActionType { Id = 1, CntActionTypeCode = "RTS", CntActionTypeName = "Release To Shipper" },
                new CntActionType { Id = 2, CntActionTypeCode = "RBS", CntActionTypeName = "Return By Shipper" },
                new CntActionType { Id = 3, CntActionTypeCode = "LOB", CntActionTypeName = "Load On Board" },
                new CntActionType { Id = 4, CntActionTypeCode = "DIS", CntActionTypeName = "Discharge" },
                new CntActionType { Id = 5, CntActionTypeCode = "DTC", CntActionTypeName = "Deliver To Consignee" },
                new CntActionType { Id = 6, CntActionTypeCode = "RBC", CntActionTypeName = "Return By Consignee" }
            );
        }
    }
    public class ContainerSizeConfiguration : IEntityTypeConfiguration<ContainerSize>
    {
        public void Configure(EntityTypeBuilder<ContainerSize> builder)
        {
            builder.ToTable("ContainerSizes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerSizeName).IsRequired().HasMaxLength(2);
            builder.Property(c => c.TEUFactor).IsRequired();

            // Seed Data
            builder.HasData(
                new ContainerSize { Id = 20, ContainerSizeName = "20", TEUFactor = 1 },
                new ContainerSize { Id = 40, ContainerSizeName = "40", TEUFactor = 2 },
                new ContainerSize { Id = 45, ContainerSizeName = "45", TEUFactor = 2.25m }
            );

        }
    }
    public class ContainerTypeConfiguration : IEntityTypeConfiguration<ContainerType>
    {
        public void Configure(EntityTypeBuilder<ContainerType> builder)
        {
            builder.ToTable("ContainerTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerTypeAbb).IsRequired().HasMaxLength(5);
            builder.Property(c => c.ContainerTypeName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.isSpecial).IsRequired();
            builder.Property(c => c.isRefer).IsRequired();

            // Seed Data
            builder.HasData(
                new ContainerType { Id = 1, ContainerTypeAbb = "GP", ContainerTypeName = "Standard(GP)", isRefer = false, isSpecial = false },
                new ContainerType { Id = 2, ContainerTypeAbb = "HD", ContainerTypeName = "Hevey Duty", isRefer = false, isSpecial = false },
                new ContainerType { Id = 3, ContainerTypeAbb = "HC", ContainerTypeName = "High Cube", isRefer = false, isSpecial = false },
                new ContainerType { Id = 4, ContainerTypeAbb = "OT", ContainerTypeName = "Open Top", isRefer = false, isSpecial = true },
                new ContainerType { Id = 5, ContainerTypeAbb = "FR", ContainerTypeName = "Flat Rack", isRefer = false, isSpecial = true },
                new ContainerType { Id = 6, ContainerTypeAbb = "RF", ContainerTypeName = "Reefer", isRefer = true, isSpecial = false },
                new ContainerType { Id = 7, ContainerTypeAbb = "TK", ContainerTypeName = "Tank", isRefer = false, isSpecial = true },
                new ContainerType { Id = 8, ContainerTypeAbb = "PL", ContainerTypeName = "Platform", isRefer = false, isSpecial = true }
            );
        }
    }
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currency", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(c => c.CurrencyName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LocalName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Symbol).HasMaxLength(5);
            // Seed Data
            builder.HasData(
                new Currency { Id = 1, CurrencyCode = "USD", CurrencyName = "US Dollar", LocalName = "United States Dollar" },
                new Currency { Id = 2, CurrencyCode = "IRR", CurrencyName = "Iranian Rial" },
                new Currency { Id = 3, CurrencyCode = "AED", CurrencyName = "Emirates Dirham" },
                new Currency { Id = 4, CurrencyCode = "EUR", CurrencyName = "Euro" },
                new Currency { Id = 5, CurrencyCode = "AFN", CurrencyName = "Afghani" },
                new Currency { Id = 6, CurrencyCode = "ALL", CurrencyName = "Lek" },
                new Currency { Id = 7, CurrencyCode = "AMD", CurrencyName = "Dram" },
                new Currency { Id = 8, CurrencyCode = "ANG", CurrencyName = "Netherlands Antillian Guilder" },
                new Currency { Id = 9, CurrencyCode = "AOA", CurrencyName = "Kwanza" },
                new Currency { Id = 10, CurrencyCode = "ARS", CurrencyName = "Argentine Peso" },
                new Currency { Id = 11, CurrencyCode = "AUD", CurrencyName = "Australian Dollar" },
                new Currency { Id = 12, CurrencyCode = "AWG", CurrencyName = "Aruban Florin" },
                new Currency { Id = 13, CurrencyCode = "AZN", CurrencyName = "Azerbaijan Manat" },
                new Currency { Id = 14, CurrencyCode = "BAM", CurrencyName = "Convertible Mark" },
                new Currency { Id = 15, CurrencyCode = "BBD", CurrencyName = "Barbados Dollar" },
                new Currency { Id = 16, CurrencyCode = "BDT", CurrencyName = "Taka" },
                new Currency { Id = 17, CurrencyCode = "BGN", CurrencyName = "Bulgarian Lev" },
                new Currency { Id = 18, CurrencyCode = "BHD", CurrencyName = "Bahraini Dinar" },
                new Currency { Id = 19, CurrencyCode = "BIF", CurrencyName = "Burundi Franc" },
                new Currency { Id = 20, CurrencyCode = "BMD", CurrencyName = "Bermudian Dollar (customarily: Bermuda Dollar)" },
                new Currency { Id = 21, CurrencyCode = "BND", CurrencyName = "Brunei Dollar" },
                new Currency { Id = 22, CurrencyCode = "BOB", CurrencyName = "Boliviano" },
                new Currency { Id = 23, CurrencyCode = "BOV", CurrencyName = "Mvdol" },
                new Currency { Id = 24, CurrencyCode = "BRL", CurrencyName = "Brazilian Real" },
                new Currency { Id = 25, CurrencyCode = "BSD", CurrencyName = "Bahamian Dollar" },
                new Currency { Id = 26, CurrencyCode = "BTN", CurrencyName = "Ngultrum" },
                new Currency { Id = 27, CurrencyCode = "BWP", CurrencyName = "Pula" },
                new Currency { Id = 28, CurrencyCode = "BYN", CurrencyName = "Belarussian Ruble" },
                new Currency { Id = 29, CurrencyCode = "BZD", CurrencyName = "Belize Dollar" },
                new Currency { Id = 30, CurrencyCode = "CAD", CurrencyName = "Canadian Dollar" },
                new Currency { Id = 31, CurrencyCode = "CDF", CurrencyName = "Franc Congolais" },
                new Currency { Id = 32, CurrencyCode = "CHE", CurrencyName = "WIR Euro" },
                new Currency { Id = 33, CurrencyCode = "CHF", CurrencyName = "Swiss Franc" },
                new Currency { Id = 34, CurrencyCode = "CHW", CurrencyName = "WIR Franc" },
                new Currency { Id = 35, CurrencyCode = "CLF", CurrencyName = "Unidad de Fomento" },
                new Currency { Id = 36, CurrencyCode = "CLP", CurrencyName = "Chilean Peso" },
                new Currency { Id = 37, CurrencyCode = "CNY", CurrencyName = "Yuan Renminbi" },
                new Currency { Id = 38, CurrencyCode = "COP", CurrencyName = "Colombian Peso" },
                new Currency { Id = 39, CurrencyCode = "COU", CurrencyName = "Unidad de Valor Real" },
                new Currency { Id = 40, CurrencyCode = "CRC", CurrencyName = "Costa Rican Colon" },
                new Currency { Id = 41, CurrencyCode = "CUC", CurrencyName = "Peso Convertible" },
                new Currency { Id = 42, CurrencyCode = "CUP", CurrencyName = "Cuban Peso" },
                new Currency { Id = 43, CurrencyCode = "CVE", CurrencyName = "Cabo Verde Escudo" },
                new Currency { Id = 44, CurrencyCode = "CZK", CurrencyName = "Czech Koruna" },
                new Currency { Id = 45, CurrencyCode = "DJF", CurrencyName = "Djibouti Franc" },
                new Currency { Id = 46, CurrencyCode = "DKK", CurrencyName = "Danish Krone" },
                new Currency { Id = 47, CurrencyCode = "DOP", CurrencyName = "Dominican Peso" },
                new Currency { Id = 48, CurrencyCode = "DZD", CurrencyName = "Algerian Dinar" },
                new Currency { Id = 49, CurrencyCode = "EGP", CurrencyName = "Egyptian Pound" },
                new Currency { Id = 50, CurrencyCode = "ERN", CurrencyName = "Nakfa" },
                new Currency { Id = 51, CurrencyCode = "ETB", CurrencyName = "Ethopian Birr" },
                new Currency { Id = 52, CurrencyCode = "FJD", CurrencyName = "Fiji Dollar" },
                new Currency { Id = 53, CurrencyCode = "FKP", CurrencyName = "Falkland Islands Pound" },
                new Currency { Id = 54, CurrencyCode = "GBP", CurrencyName = "Pound Sterling" },
                new Currency { Id = 55, CurrencyCode = "GEL", CurrencyName = "Lari" },
                new Currency { Id = 56, CurrencyCode = "GHS", CurrencyName = "Ghana Cedi" },
                new Currency { Id = 57, CurrencyCode = "GIP", CurrencyName = "Gibraltar Pound" },
                new Currency { Id = 58, CurrencyCode = "GMD", CurrencyName = "Dalasi" },
                new Currency { Id = 59, CurrencyCode = "GNF", CurrencyName = "Guinean Franc" },
                new Currency { Id = 60, CurrencyCode = "GTQ", CurrencyName = "Quetzal" },
                new Currency { Id = 61, CurrencyCode = "GYD", CurrencyName = "Guyana Dollar" },
                new Currency { Id = 62, CurrencyCode = "HKD", CurrencyName = "Honk Kong Dollar" },
                new Currency { Id = 63, CurrencyCode = "HNL", CurrencyName = "Lempira" },
                new Currency { Id = 64, CurrencyCode = "HTG", CurrencyName = "Gourde" },
                new Currency { Id = 65, CurrencyCode = "HUF", CurrencyName = "Forint" },
                new Currency { Id = 66, CurrencyCode = "IDR", CurrencyName = "Rupiah" },
                new Currency { Id = 67, CurrencyCode = "ILS", CurrencyName = "New Israeli Sheqel" },
                new Currency { Id = 68, CurrencyCode = "INR", CurrencyName = "Indian Rupee" },
                new Currency { Id = 69, CurrencyCode = "IQD", CurrencyName = "Iraqi Dinar" },
                new Currency { Id = 70, CurrencyCode = "ISK", CurrencyName = "Iceland Krona" },
                new Currency { Id = 71, CurrencyCode = "JMD", CurrencyName = "Jamaican Dollar" },
                new Currency { Id = 72, CurrencyCode = "JOD", CurrencyName = "Jordanian Dinar" },
                new Currency { Id = 73, CurrencyCode = "JPY", CurrencyName = "Yen" },
                new Currency { Id = 74, CurrencyCode = "KES", CurrencyName = "Kenyan Shilling" },
                new Currency { Id = 75, CurrencyCode = "KGS", CurrencyName = "Som" },
                new Currency { Id = 76, CurrencyCode = "KHR", CurrencyName = "Riel" },
                new Currency { Id = 77, CurrencyCode = "KMF", CurrencyName = "Comorian Franc" },
                new Currency { Id = 78, CurrencyCode = "KPW", CurrencyName = "North Korean Won" },
                new Currency { Id = 79, CurrencyCode = "KRW", CurrencyName = "Won" },
                new Currency { Id = 80, CurrencyCode = "KWD", CurrencyName = "Kuwaiti Dinar" },
                new Currency { Id = 81, CurrencyCode = "KYD", CurrencyName = "Cayman Islands Dollar" },
                new Currency { Id = 82, CurrencyCode = "KZT", CurrencyName = "Tenge" },
                new Currency { Id = 83, CurrencyCode = "LAK", CurrencyName = "Lao Kip" },
                new Currency { Id = 84, CurrencyCode = "LBP", CurrencyName = "Lebanese Pound" },
                new Currency { Id = 85, CurrencyCode = "LKR", CurrencyName = "Sri Lanka Rupee" },
                new Currency { Id = 86, CurrencyCode = "LRD", CurrencyName = "Liberian Dollar" },
                new Currency { Id = 87, CurrencyCode = "LSL", CurrencyName = "Loti" },
                new Currency { Id = 88, CurrencyCode = "LYD", CurrencyName = "Libyan Dinar" },
                new Currency { Id = 89, CurrencyCode = "MAD", CurrencyName = "Morrocan Dirham" },
                new Currency { Id = 90, CurrencyCode = "MDL", CurrencyName = "Moldovan Leu" },
                new Currency { Id = 91, CurrencyCode = "MGA", CurrencyName = "Ariary" },
                new Currency { Id = 92, CurrencyCode = "MKD", CurrencyName = "Denar" },
                new Currency { Id = 93, CurrencyCode = "MMK", CurrencyName = "Kyat" },
                new Currency { Id = 94, CurrencyCode = "MNT", CurrencyName = "Tugrik" },
                new Currency { Id = 95, CurrencyCode = "MOP", CurrencyName = "Pataca" },
                new Currency { Id = 96, CurrencyCode = "MRU", CurrencyName = "Ouguiya" },
                new Currency { Id = 97, CurrencyCode = "MUR", CurrencyName = "Mauritius Rupee" },
                new Currency { Id = 98, CurrencyCode = "MVR", CurrencyName = "Rufiyaa" },
                new Currency { Id = 99, CurrencyCode = "MWK", CurrencyName = "Malawi Kwacha" },
                new Currency { Id = 100, CurrencyCode = "MXN", CurrencyName = "Mexican Peso" },
                new Currency { Id = 101, CurrencyCode = "MXV", CurrencyName = "Mexican Unidad de Inversion (UDI)" },
                new Currency { Id = 102, CurrencyCode = "MYR", CurrencyName = "Malaysian Ringgit" },
                new Currency { Id = 103, CurrencyCode = "MZN", CurrencyName = "Mozambique Metical" },
                new Currency { Id = 104, CurrencyCode = "NAD", CurrencyName = "Namibia Dollar" },
                new Currency { Id = 105, CurrencyCode = "NGN", CurrencyName = "Naira" },
                new Currency { Id = 106, CurrencyCode = "NIO", CurrencyName = "Cordoba Oro" },
                new Currency { Id = 107, CurrencyCode = "NOK", CurrencyName = "Norwegian Krone" },
                new Currency { Id = 108, CurrencyCode = "NPR", CurrencyName = "Nepalese Rupee" },
                new Currency { Id = 109, CurrencyCode = "NZD", CurrencyName = "New Zealand Dollar" },
                new Currency { Id = 110, CurrencyCode = "OMR", CurrencyName = "Rial Omani" },
                new Currency { Id = 111, CurrencyCode = "PAB", CurrencyName = "Balboa" },
                new Currency { Id = 112, CurrencyCode = "PEN", CurrencyName = "Sol" },
                new Currency { Id = 113, CurrencyCode = "PGK", CurrencyName = "Kina" },
                new Currency { Id = 114, CurrencyCode = "PHP", CurrencyName = "Philippine Piso" },
                new Currency { Id = 115, CurrencyCode = "PKR", CurrencyName = "Pakistan Rupee" },
                new Currency { Id = 116, CurrencyCode = "PLN", CurrencyName = "Zloty" },
                new Currency { Id = 117, CurrencyCode = "PYG", CurrencyName = "Guarani" },
                new Currency { Id = 118, CurrencyCode = "QAR", CurrencyName = "Qatari Rial" },
                new Currency { Id = 119, CurrencyCode = "RON", CurrencyName = "Romanian Leu" },
                new Currency { Id = 120, CurrencyCode = "RSD", CurrencyName = "Serbian Dinar" },
                new Currency { Id = 121, CurrencyCode = "RUB", CurrencyName = "Russian Ruble" },
                new Currency { Id = 122, CurrencyCode = "RWF", CurrencyName = "Rwanda Franc" },
                new Currency { Id = 123, CurrencyCode = "SAR", CurrencyName = "Saudi Riyal" },
                new Currency { Id = 124, CurrencyCode = "SBD", CurrencyName = "Solomon Islands Dollar" },
                new Currency { Id = 125, CurrencyCode = "SCR", CurrencyName = "Seychelles Rupee" },
                new Currency { Id = 126, CurrencyCode = "SDG", CurrencyName = "Sudanese Pound" },
                new Currency { Id = 127, CurrencyCode = "SEK", CurrencyName = "Swedish Krona" },
                new Currency { Id = 128, CurrencyCode = "SGD", CurrencyName = "Singapore Dollar" },
                new Currency { Id = 129, CurrencyCode = "SHP", CurrencyName = "St. Helena Pound" },
                new Currency { Id = 130, CurrencyCode = "SLE", CurrencyName = "Leone" },
                new Currency { Id = 131, CurrencyCode = "SOS", CurrencyName = "Somali Shilling" },
                new Currency { Id = 132, CurrencyCode = "SRD", CurrencyName = "Suriname Dollar" },
                new Currency { Id = 133, CurrencyCode = "SSP", CurrencyName = "South Sudanese Pound" },
                new Currency { Id = 134, CurrencyCode = "STN", CurrencyName = "Dobra" },
                new Currency { Id = 135, CurrencyCode = "SVC", CurrencyName = "El Salvador Colon" },
                new Currency { Id = 136, CurrencyCode = "SYP", CurrencyName = "Syrian Pound" },
                new Currency { Id = 137, CurrencyCode = "SZL", CurrencyName = "Lilangeni" },
                new Currency { Id = 138, CurrencyCode = "THB", CurrencyName = "Baht" },
                new Currency { Id = 139, CurrencyCode = "TJS", CurrencyName = "Somoni" },
                new Currency { Id = 140, CurrencyCode = "TMT", CurrencyName = "Turkmenistan New Manat" },
                new Currency { Id = 141, CurrencyCode = "TND", CurrencyName = "Tunisian Dinar" },
                new Currency { Id = 142, CurrencyCode = "TOP", CurrencyName = "Pa'anga" },
                new Currency { Id = 143, CurrencyCode = "TRY", CurrencyName = "Turkish Lira" },
                new Currency { Id = 144, CurrencyCode = "TTD", CurrencyName = "Trinidad and Tobago Dollar" },
                new Currency { Id = 145, CurrencyCode = "TWD", CurrencyName = "New Taiwan Dollar" },
                new Currency { Id = 146, CurrencyCode = "TZS", CurrencyName = "Tanzanian Shilling" },
                new Currency { Id = 147, CurrencyCode = "UAH", CurrencyName = "Hryvnia" },
                new Currency { Id = 148, CurrencyCode = "UGX", CurrencyName = "Uganda Shilling" },
                new Currency { Id = 149, CurrencyCode = "USN", CurrencyName = "US Dollar (Next day)" },
                new Currency { Id = 150, CurrencyCode = "UYI", CurrencyName = "Uruguayo Peso en Unidades" },
                new Currency { Id = 151, CurrencyCode = "UYU", CurrencyName = "Peso Uruguayo" },
                new Currency { Id = 152, CurrencyCode = "UYW", CurrencyName = "Unidad Previsional" },
                new Currency { Id = 153, CurrencyCode = "UZS", CurrencyName = "Uzbekistan Sum" },
                new Currency { Id = 154, CurrencyCode = "VED", CurrencyName = "Bol?var Soberano" },
                new Currency { Id = 155, CurrencyCode = "VES", CurrencyName = "Bol?var Soberano" },
                new Currency { Id = 156, CurrencyCode = "VND", CurrencyName = "Dong" },
                new Currency { Id = 157, CurrencyCode = "VUV", CurrencyName = "Vatu" },
                new Currency { Id = 158, CurrencyCode = "WST", CurrencyName = "Tala" },
                new Currency { Id = 159, CurrencyCode = "XAF", CurrencyName = "CFA Franc" },
                new Currency { Id = 160, CurrencyCode = "XAG", CurrencyName = "Silver" },
                new Currency { Id = 161, CurrencyCode = "XAU", CurrencyName = "Gold" },
                new Currency { Id = 162, CurrencyCode = "XBA", CurrencyName = "Bond Markets Units European Composite Unit (EURCO)" },
                new Currency { Id = 163, CurrencyCode = "XBB", CurrencyName = "European Monetary Unit (E.M.U.-6)" },
                new Currency { Id = 164, CurrencyCode = "XBC", CurrencyName = "European Unit of Account 9 (E.U.A.-9)" },
                new Currency { Id = 165, CurrencyCode = "XBD", CurrencyName = "European Unit of Account 17 (E.U.A.-17)" },
                new Currency { Id = 166, CurrencyCode = "XCD", CurrencyName = "East Carribean Dollar" },
                new Currency { Id = 167, CurrencyCode = "XDR", CurrencyName = "SDR" },
                new Currency { Id = 168, CurrencyCode = "XOF", CurrencyName = "CFA Franc" },
                new Currency { Id = 169, CurrencyCode = "XPD", CurrencyName = "Palladium" },
                new Currency { Id = 170, CurrencyCode = "XPF", CurrencyName = "CFP Franc" },
                new Currency { Id = 171, CurrencyCode = "XPT", CurrencyName = "Platinum" },
                new Currency { Id = 172, CurrencyCode = "XSU", CurrencyName = "Sucre" },
                new Currency { Id = 173, CurrencyCode = "XTS", CurrencyName = "Codes specifically reserved for testing purposes" },
                new Currency { Id = 174, CurrencyCode = "XUA", CurrencyName = "ADB Unit of Account" },
                new Currency { Id = 175, CurrencyCode = "XXX", CurrencyName = "The codes assigned for transactions where no currency is involved" },
                new Currency { Id = 176, CurrencyCode = "YER", CurrencyName = "Yemeni Rial" },
                new Currency { Id = 177, CurrencyCode = "ZAR", CurrencyName = "Rand" },
                new Currency { Id = 178, CurrencyCode = "ZMW", CurrencyName = "Zambian Kwacha" },
                new Currency { Id = 179, CurrencyCode = "ZWL", CurrencyName = "Zimbabwe Dollar" }

            );
        }
    }

    public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zones", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ZoneName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ZoneLocalName).IsRequired().HasMaxLength(50);
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CountryName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.CountryPhoneCode).HasMaxLength(5);
            builder.Property(c => c.CustomsCode).HasMaxLength(20);
            builder.Property(c => c.TimeZone).HasMaxLength(10);
            builder.Property(c => c.CountryLocalName).HasMaxLength(50);
            builder.Property(c => c.CountryIsoCode2).IsRequired().HasMaxLength(2);
            builder.Property(c => c.CountryIsoCode3).HasMaxLength(3);
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Seed Data
            builder.HasData(
                new Country { Id = 1, CountryIsoCode2 = "IR", CountryLocalName = "ایران", CountryName = "Islamic Republic Of Iran" },
                new Country { Id = 2, CountryIsoCode2 = "CN", CountryLocalName = "چین", CountryName = "China" },
                new Country { Id = 3, CountryIsoCode2 = "AE", CountryLocalName = "امارات", CountryName = "Emirates" },
                new Country { Id = 4, CountryIsoCode2 = "AD", CountryLocalName = "Andorra", CountryName = "Andorra" },
                new Country { Id = 5, CountryIsoCode2 = "AF", CountryLocalName = "Afghanistan", CountryName = "Afghanistan" },
                new Country { Id = 6, CountryIsoCode2 = "AG", CountryLocalName = "Antigua and Barbuda", CountryName = "Antigua and Barbuda" },
                new Country { Id = 7, CountryIsoCode2 = "AI", CountryLocalName = "Anguilla", CountryName = "Anguilla" },
                new Country { Id = 8, CountryIsoCode2 = "AL", CountryLocalName = "Albania", CountryName = "Albania" },
                new Country { Id = 9, CountryIsoCode2 = "AM", CountryLocalName = "Armenia", CountryName = "Armenia" },
                new Country { Id = 10, CountryIsoCode2 = "AO", CountryLocalName = "Angola", CountryName = "Angola" },
                new Country { Id = 11, CountryIsoCode2 = "AQ", CountryLocalName = "Antarctica", CountryName = "Antarctica" },
                new Country { Id = 12, CountryIsoCode2 = "AR", CountryLocalName = "Argentina", CountryName = "Argentina" },
                new Country { Id = 13, CountryIsoCode2 = "AS", CountryLocalName = "American Samoa", CountryName = "American Samoa" },
                new Country { Id = 14, CountryIsoCode2 = "AT", CountryLocalName = "Austria", CountryName = "Austria" },
                new Country { Id = 15, CountryIsoCode2 = "AU", CountryLocalName = "Australia", CountryName = "Australia" },
                new Country { Id = 16, CountryIsoCode2 = "AW", CountryLocalName = "Aruba", CountryName = "Aruba" },
                new Country { Id = 17, CountryIsoCode2 = "AX", CountryLocalName = "?land Islands", CountryName = "?land Islands" },
                new Country { Id = 18, CountryIsoCode2 = "AZ", CountryLocalName = "Azerbaijan", CountryName = "Azerbaijan" },
                new Country { Id = 19, CountryIsoCode2 = "BA", CountryLocalName = "Bosnia and Herzegovina", CountryName = "Bosnia and Herzegovina" },
                new Country { Id = 20, CountryIsoCode2 = "BB", CountryLocalName = "Barbados", CountryName = "Barbados" },
                new Country { Id = 21, CountryIsoCode2 = "BD", CountryLocalName = "Bangladesh", CountryName = "Bangladesh" },
                new Country { Id = 22, CountryIsoCode2 = "BE", CountryLocalName = "Belgium", CountryName = "Belgium" },
                new Country { Id = 23, CountryIsoCode2 = "BF", CountryLocalName = "Burkina Faso", CountryName = "Burkina Faso" },
                new Country { Id = 24, CountryIsoCode2 = "BG", CountryLocalName = "Bulgaria", CountryName = "Bulgaria" },
                new Country { Id = 25, CountryIsoCode2 = "BH", CountryLocalName = "Bahrain", CountryName = "Bahrain" },
                new Country { Id = 26, CountryIsoCode2 = "BI", CountryLocalName = "Burundi", CountryName = "Burundi" },
                new Country { Id = 27, CountryIsoCode2 = "BJ", CountryLocalName = "Benin", CountryName = "Benin" },
                new Country { Id = 28, CountryIsoCode2 = "BL", CountryLocalName = "Saint Barthélemy", CountryName = "Saint Barthélemy" },
                new Country { Id = 29, CountryIsoCode2 = "BM", CountryLocalName = "Bermuda", CountryName = "Bermuda" },
                new Country { Id = 30, CountryIsoCode2 = "BN", CountryLocalName = "Brunei Darussalam", CountryName = "Brunei Darussalam" },
                new Country { Id = 31, CountryIsoCode2 = "BO", CountryLocalName = "Bolivia", CountryName = "Bolivia" },
                new Country { Id = 32, CountryIsoCode2 = "BQ", CountryLocalName = "Bonaire, Sint Eustatius and Saba", CountryName = "Bonaire, Sint Eustatius and Saba" },
                new Country { Id = 33, CountryIsoCode2 = "BR", CountryLocalName = "Brazil", CountryName = "Brazil" },
                new Country { Id = 34, CountryIsoCode2 = "BS", CountryLocalName = "Bahamas", CountryName = "Bahamas" },
                new Country { Id = 35, CountryIsoCode2 = "BT", CountryLocalName = "Bhutan", CountryName = "Bhutan" },
                new Country { Id = 36, CountryIsoCode2 = "BW", CountryLocalName = "Botswana", CountryName = "Botswana" },
                new Country { Id = 37, CountryIsoCode2 = "BY", CountryLocalName = "Belarus", CountryName = "Belarus" },
                new Country { Id = 38, CountryIsoCode2 = "BZ", CountryLocalName = "Belize", CountryName = "Belize" },
                new Country { Id = 39, CountryIsoCode2 = "CA", CountryLocalName = "Canada", CountryName = "Canada" },
                new Country { Id = 40, CountryIsoCode2 = "CC", CountryLocalName = "Cocos (Keeling) Islands", CountryName = "Cocos (Keeling) Islands" },
                new Country { Id = 41, CountryIsoCode2 = "CD", CountryLocalName = "Congo, The Democratic Republic of the", CountryName = "Congo, The Democratic Republic of the" },
                new Country { Id = 42, CountryIsoCode2 = "CF", CountryLocalName = "Central African Republic", CountryName = "Central African Republic" },
                new Country { Id = 43, CountryIsoCode2 = "CG", CountryLocalName = "Congo", CountryName = "Congo" },
                new Country { Id = 44, CountryIsoCode2 = "CH", CountryLocalName = "Switzerland", CountryName = "Switzerland" },
                new Country { Id = 45, CountryIsoCode2 = "CI", CountryLocalName = "Côte d'Ivoire", CountryName = "Côte d'Ivoire" },
                new Country { Id = 46, CountryIsoCode2 = "CK", CountryLocalName = "Cook Islands", CountryName = "Cook Islands" },
                new Country { Id = 47, CountryIsoCode2 = "CL", CountryLocalName = "Chile", CountryName = "Chile" },
                new Country { Id = 48, CountryIsoCode2 = "CM", CountryLocalName = "Cameroon", CountryName = "Cameroon" },
                new Country { Id = 49, CountryIsoCode2 = "CO", CountryLocalName = "Colombia", CountryName = "Colombia" },
                new Country { Id = 50, CountryIsoCode2 = "CR", CountryLocalName = "Costa Rica", CountryName = "Costa Rica" },
                new Country { Id = 51, CountryIsoCode2 = "CU", CountryLocalName = "Cuba", CountryName = "Cuba" },
                new Country { Id = 52, CountryIsoCode2 = "CV", CountryLocalName = "Cape Verde", CountryName = "Cape Verde" },
                new Country { Id = 53, CountryIsoCode2 = "CW", CountryLocalName = "Curaçao", CountryName = "Curaçao" },
                new Country { Id = 54, CountryIsoCode2 = "CX", CountryLocalName = "Christmas Island", CountryName = "Christmas Island" },
                new Country { Id = 55, CountryIsoCode2 = "CY", CountryLocalName = "Cyprus", CountryName = "Cyprus" },
                new Country { Id = 56, CountryIsoCode2 = "CZ", CountryLocalName = "Czech Republic", CountryName = "Czech Republic" },
                new Country { Id = 57, CountryIsoCode2 = "DE", CountryLocalName = "Germany", CountryName = "Germany" },
                new Country { Id = 58, CountryIsoCode2 = "DJ", CountryLocalName = "Djibouti", CountryName = "Djibouti" },
                new Country { Id = 59, CountryIsoCode2 = "DK", CountryLocalName = "Denmark", CountryName = "Denmark" },
                new Country { Id = 60, CountryIsoCode2 = "DM", CountryLocalName = "Dominica", CountryName = "Dominica" },
                new Country { Id = 61, CountryIsoCode2 = "DO", CountryLocalName = "Dominican Republic", CountryName = "Dominican Republic" },
                new Country { Id = 62, CountryIsoCode2 = "DZ", CountryLocalName = "Algeria", CountryName = "Algeria" },
                new Country { Id = 63, CountryIsoCode2 = "EC", CountryLocalName = "Ecuador", CountryName = "Ecuador" },
                new Country { Id = 64, CountryIsoCode2 = "EE", CountryLocalName = "Estonia", CountryName = "Estonia" },
                new Country { Id = 65, CountryIsoCode2 = "EG", CountryLocalName = "Egypt", CountryName = "Egypt" },
                new Country { Id = 66, CountryIsoCode2 = "EH", CountryLocalName = "Western Sahara", CountryName = "Western Sahara" },
                new Country { Id = 67, CountryIsoCode2 = "ER", CountryLocalName = "Eritrea", CountryName = "Eritrea" },
                new Country { Id = 68, CountryIsoCode2 = "ES", CountryLocalName = "Spain", CountryName = "Spain" },
                new Country { Id = 69, CountryIsoCode2 = "ET", CountryLocalName = "Ethiopia", CountryName = "Ethiopia" },
                new Country { Id = 70, CountryIsoCode2 = "FI", CountryLocalName = "Finland", CountryName = "Finland" },
                new Country { Id = 71, CountryIsoCode2 = "FJ", CountryLocalName = "Fiji", CountryName = "Fiji" },
                new Country { Id = 72, CountryIsoCode2 = "FK", CountryLocalName = "Falkland Islands (Malvinas)", CountryName = "Falkland Islands (Malvinas)" },
                new Country { Id = 73, CountryIsoCode2 = "FM", CountryLocalName = "Micronesia, Federated States of", CountryName = "Micronesia, Federated States of" },
                new Country { Id = 74, CountryIsoCode2 = "FO", CountryLocalName = "Faroe Islands", CountryName = "Faroe Islands" },
                new Country { Id = 75, CountryIsoCode2 = "FR", CountryLocalName = "France", CountryName = "France" },
                new Country { Id = 76, CountryIsoCode2 = "GA", CountryLocalName = "Gabon", CountryName = "Gabon" },
                new Country { Id = 77, CountryIsoCode2 = "GB", CountryLocalName = "United Kingdom", CountryName = "United Kingdom" },
                new Country { Id = 78, CountryIsoCode2 = "GD", CountryLocalName = "Grenada", CountryName = "Grenada" },
                new Country { Id = 79, CountryIsoCode2 = "GE", CountryLocalName = "Georgia", CountryName = "Georgia" },
                new Country { Id = 80, CountryIsoCode2 = "GF", CountryLocalName = "French Guiana", CountryName = "French Guiana" },
                new Country { Id = 81, CountryIsoCode2 = "GG", CountryLocalName = "Guernsey", CountryName = "Guernsey" },
                new Country { Id = 82, CountryIsoCode2 = "GH", CountryLocalName = "Ghana", CountryName = "Ghana" },
                new Country { Id = 83, CountryIsoCode2 = "GI", CountryLocalName = "Gibraltar", CountryName = "Gibraltar" },
                new Country { Id = 84, CountryIsoCode2 = "GL", CountryLocalName = "Greenland", CountryName = "Greenland" },
                new Country { Id = 85, CountryIsoCode2 = "GM", CountryLocalName = "Gambia", CountryName = "Gambia" },
                new Country { Id = 86, CountryIsoCode2 = "GN", CountryLocalName = "Guinea", CountryName = "Guinea" },
                new Country { Id = 87, CountryIsoCode2 = "GP", CountryLocalName = "Guadeloupe", CountryName = "Guadeloupe" },
                new Country { Id = 88, CountryIsoCode2 = "GQ", CountryLocalName = "Equatorial Guinea", CountryName = "Equatorial Guinea" },
                new Country { Id = 89, CountryIsoCode2 = "GR", CountryLocalName = "Greece", CountryName = "Greece" },
                new Country { Id = 90, CountryIsoCode2 = "GS", CountryLocalName = "South Georgia and the South Sandwich Islands", CountryName = "South Georgia and the South Sandwich Islands" },
                new Country { Id = 91, CountryIsoCode2 = "GT", CountryLocalName = "Guatemala", CountryName = "Guatemala" },
                new Country { Id = 92, CountryIsoCode2 = "GU", CountryLocalName = "Guam", CountryName = "Guam" },
                new Country { Id = 93, CountryIsoCode2 = "GW", CountryLocalName = "Guinea-Bissau", CountryName = "Guinea-Bissau" },
                new Country { Id = 94, CountryIsoCode2 = "GY", CountryLocalName = "Guyana", CountryName = "Guyana" },
                new Country { Id = 95, CountryIsoCode2 = "HK", CountryLocalName = "Hong Kong", CountryName = "Hong Kong" },
                new Country { Id = 96, CountryIsoCode2 = "HM", CountryLocalName = "Heard Island and McDonald Islands", CountryName = "Heard Island and McDonald Islands" },
                new Country { Id = 97, CountryIsoCode2 = "HN", CountryLocalName = "Honduras", CountryName = "Honduras" },
                new Country { Id = 98, CountryIsoCode2 = "HR", CountryLocalName = "Croatia", CountryName = "Croatia" },
                new Country { Id = 99, CountryIsoCode2 = "HT", CountryLocalName = "Haiti", CountryName = "Haiti" },
                new Country { Id = 100, CountryIsoCode2 = "HU", CountryLocalName = "Hungary", CountryName = "Hungary" },
                new Country { Id = 101, CountryIsoCode2 = "ID", CountryLocalName = "Indonesia", CountryName = "Indonesia" },
                new Country { Id = 102, CountryIsoCode2 = "IE", CountryLocalName = "Ireland", CountryName = "Ireland" },
                new Country { Id = 103, CountryIsoCode2 = "IL", CountryLocalName = "Israel", CountryName = "Israel" },
                new Country { Id = 104, CountryIsoCode2 = "IM", CountryLocalName = "Isle of Man", CountryName = "Isle of Man" },
                new Country { Id = 105, CountryIsoCode2 = "IN", CountryLocalName = "India", CountryName = "India" },
                new Country { Id = 106, CountryIsoCode2 = "IO", CountryLocalName = "British Indian Ocean Territory", CountryName = "British Indian Ocean Territory" },
                new Country { Id = 107, CountryIsoCode2 = "IQ", CountryLocalName = "Iraq", CountryName = "Iraq" },
                new Country { Id = 108, CountryIsoCode2 = "IS", CountryLocalName = "Iceland", CountryName = "Iceland" },
                new Country { Id = 109, CountryIsoCode2 = "IT", CountryLocalName = "Italy", CountryName = "Italy" },
                new Country { Id = 110, CountryIsoCode2 = "JE", CountryLocalName = "Jersey", CountryName = "Jersey" },
                new Country { Id = 111, CountryIsoCode2 = "JM", CountryLocalName = "Jamaica", CountryName = "Jamaica" },
                new Country { Id = 112, CountryIsoCode2 = "JO", CountryLocalName = "Jordan", CountryName = "Jordan" },
                new Country { Id = 113, CountryIsoCode2 = "JP", CountryLocalName = "Japan", CountryName = "Japan" },
                new Country { Id = 114, CountryIsoCode2 = "KE", CountryLocalName = "Kenya", CountryName = "Kenya" },
                new Country { Id = 115, CountryIsoCode2 = "KG", CountryLocalName = "Kyrgyzstan", CountryName = "Kyrgyzstan" },
                new Country { Id = 116, CountryIsoCode2 = "KH", CountryLocalName = "Cambodia", CountryName = "Cambodia" },
                new Country { Id = 117, CountryIsoCode2 = "KI", CountryLocalName = "Kiribati", CountryName = "Kiribati" },
                new Country { Id = 118, CountryIsoCode2 = "KM", CountryLocalName = "Comoros", CountryName = "Comoros" },
                new Country { Id = 119, CountryIsoCode2 = "KN", CountryLocalName = "Saint Kitts and Nevis", CountryName = "Saint Kitts and Nevis" },
                new Country { Id = 120, CountryIsoCode2 = "KP", CountryLocalName = "Korea, Democratic People's Republic of", CountryName = "Korea, Democratic People's Republic of" },
                new Country { Id = 121, CountryIsoCode2 = "KR", CountryLocalName = "Korea, Republic of", CountryName = "Korea, Republic of" },
                new Country { Id = 122, CountryIsoCode2 = "KW", CountryLocalName = "Kuwait", CountryName = "Kuwait" },
                new Country { Id = 123, CountryIsoCode2 = "KY", CountryLocalName = "Cayman Islands", CountryName = "Cayman Islands" },
                new Country { Id = 124, CountryIsoCode2 = "KZ", CountryLocalName = "Kazakhstan", CountryName = "Kazakhstan" },
                new Country { Id = 125, CountryIsoCode2 = "LA", CountryLocalName = "Lao People's Democratic Republic", CountryName = "Lao People's Democratic Republic" },
                new Country { Id = 126, CountryIsoCode2 = "LB", CountryLocalName = "Lebanon", CountryName = "Lebanon" },
                new Country { Id = 127, CountryIsoCode2 = "LC", CountryLocalName = "Saint Lucia", CountryName = "Saint Lucia" },
                new Country { Id = 128, CountryIsoCode2 = "LI", CountryLocalName = "Liechtenstein", CountryName = "Liechtenstein" },
                new Country { Id = 129, CountryIsoCode2 = "LK", CountryLocalName = "Sri Lanka", CountryName = "Sri Lanka" },
                new Country { Id = 130, CountryIsoCode2 = "LR", CountryLocalName = "Liberia", CountryName = "Liberia" },
                new Country { Id = 131, CountryIsoCode2 = "LS", CountryLocalName = "Lesotho", CountryName = "Lesotho" },
                new Country { Id = 132, CountryIsoCode2 = "LT", CountryLocalName = "Lithuania", CountryName = "Lithuania" },
                new Country { Id = 133, CountryIsoCode2 = "LU", CountryLocalName = "Luxembourg", CountryName = "Luxembourg" },
                new Country { Id = 134, CountryIsoCode2 = "LV", CountryLocalName = "Latvia", CountryName = "Latvia" },
                new Country { Id = 135, CountryIsoCode2 = "LY", CountryLocalName = "Libya", CountryName = "Libya" },
                new Country { Id = 136, CountryIsoCode2 = "MA", CountryLocalName = "Morocco", CountryName = "Morocco" },
                new Country { Id = 137, CountryIsoCode2 = "MC", CountryLocalName = "Monaco", CountryName = "Monaco" },
                new Country { Id = 138, CountryIsoCode2 = "MD", CountryLocalName = "Moldova, Republic of", CountryName = "Moldova, Republic of" },
                new Country { Id = 139, CountryIsoCode2 = "ME", CountryLocalName = "Montenegro", CountryName = "Montenegro" },
                new Country { Id = 140, CountryIsoCode2 = "MF", CountryLocalName = "Saint Martin (French Part)", CountryName = "Saint Martin (French Part)" },
                new Country { Id = 141, CountryIsoCode2 = "MG", CountryLocalName = "Madagascar", CountryName = "Madagascar" },
                new Country { Id = 142, CountryIsoCode2 = "MH", CountryLocalName = "Marshall Islands", CountryName = "Marshall Islands" },
                new Country { Id = 143, CountryIsoCode2 = "MK", CountryLocalName = "North Macedonia", CountryName = "North Macedonia" },
                new Country { Id = 144, CountryIsoCode2 = "ML", CountryLocalName = "Mali", CountryName = "Mali" },
                new Country { Id = 145, CountryIsoCode2 = "MM", CountryLocalName = "Myanmar", CountryName = "Myanmar" },
                new Country { Id = 146, CountryIsoCode2 = "MN", CountryLocalName = "Mongolia", CountryName = "Mongolia" },
                new Country { Id = 147, CountryIsoCode2 = "MO", CountryLocalName = "Macao", CountryName = "Macao" },
                new Country { Id = 148, CountryIsoCode2 = "MP", CountryLocalName = "Northern Mariana Islands", CountryName = "Northern Mariana Islands" },
                new Country { Id = 149, CountryIsoCode2 = "MQ", CountryLocalName = "Martinique", CountryName = "Martinique" },
                new Country { Id = 150, CountryIsoCode2 = "MR", CountryLocalName = "Mauritania", CountryName = "Mauritania" },
                new Country { Id = 151, CountryIsoCode2 = "MS", CountryLocalName = "Montserrat", CountryName = "Montserrat" },
                new Country { Id = 152, CountryIsoCode2 = "MT", CountryLocalName = "Malta", CountryName = "Malta" },
                new Country { Id = 153, CountryIsoCode2 = "MU", CountryLocalName = "Mauritius", CountryName = "Mauritius" },
                new Country { Id = 154, CountryIsoCode2 = "MV", CountryLocalName = "Maldives", CountryName = "Maldives" },
                new Country { Id = 155, CountryIsoCode2 = "MW", CountryLocalName = "Malawi", CountryName = "Malawi" },
                new Country { Id = 156, CountryIsoCode2 = "MX", CountryLocalName = "Mexico", CountryName = "Mexico" },
                new Country { Id = 157, CountryIsoCode2 = "MY", CountryLocalName = "Malaysia", CountryName = "Malaysia" },
                new Country { Id = 158, CountryIsoCode2 = "MZ", CountryLocalName = "Mozambique", CountryName = "Mozambique" },
                new Country { Id = 159, CountryIsoCode2 = "NA", CountryLocalName = "Namibia", CountryName = "Namibia" },
                new Country { Id = 160, CountryIsoCode2 = "NC", CountryLocalName = "New Caledonia", CountryName = "New Caledonia" },
                new Country { Id = 161, CountryIsoCode2 = "NE", CountryLocalName = "Niger", CountryName = "Niger" },
                new Country { Id = 162, CountryIsoCode2 = "NF", CountryLocalName = "Norfolk Island", CountryName = "Norfolk Island" },
                new Country { Id = 163, CountryIsoCode2 = "NG", CountryLocalName = "Nigeria", CountryName = "Nigeria" },
                new Country { Id = 164, CountryIsoCode2 = "NI", CountryLocalName = "Nicaragua", CountryName = "Nicaragua" },
                new Country { Id = 165, CountryIsoCode2 = "NL", CountryLocalName = "Netherlands", CountryName = "Netherlands" },
                new Country { Id = 166, CountryIsoCode2 = "NO", CountryLocalName = "Norway", CountryName = "Norway" },
                new Country { Id = 167, CountryIsoCode2 = "NP", CountryLocalName = "Nepal", CountryName = "Nepal" },
                new Country { Id = 168, CountryIsoCode2 = "NR", CountryLocalName = "Nauru", CountryName = "Nauru" },
                new Country { Id = 169, CountryIsoCode2 = "NU", CountryLocalName = "Niue", CountryName = "Niue" },
                new Country { Id = 170, CountryIsoCode2 = "NZ", CountryLocalName = "New Zealand", CountryName = "New Zealand" },
                new Country { Id = 171, CountryIsoCode2 = "OM", CountryLocalName = "Oman", CountryName = "Oman" },
                new Country { Id = 172, CountryIsoCode2 = "PA", CountryLocalName = "Panama", CountryName = "Panama" },
                new Country { Id = 173, CountryIsoCode2 = "PE", CountryLocalName = "Peru", CountryName = "Peru" },
                new Country { Id = 174, CountryIsoCode2 = "PF", CountryLocalName = "French Polynesia", CountryName = "French Polynesia" },
                new Country { Id = 175, CountryIsoCode2 = "PG", CountryLocalName = "Papua New Guinea", CountryName = "Papua New Guinea" },
                new Country { Id = 176, CountryIsoCode2 = "PH", CountryLocalName = "Philippines", CountryName = "Philippines" },
                new Country { Id = 177, CountryIsoCode2 = "PK", CountryLocalName = "Pakistan", CountryName = "Pakistan" },
                new Country { Id = 178, CountryIsoCode2 = "PL", CountryLocalName = "Poland", CountryName = "Poland" },
                new Country { Id = 179, CountryIsoCode2 = "PM", CountryLocalName = "Saint Pierre and Miquelon", CountryName = "Saint Pierre and Miquelon" },
                new Country { Id = 180, CountryIsoCode2 = "PN", CountryLocalName = "Pitcairn", CountryName = "Pitcairn" },
                new Country { Id = 181, CountryIsoCode2 = "PR", CountryLocalName = "Puerto Rico", CountryName = "Puerto Rico" },
                new Country { Id = 182, CountryIsoCode2 = "PS", CountryLocalName = "Palestine, State of", CountryName = "Palestine, State of" },
                new Country { Id = 183, CountryIsoCode2 = "PT", CountryLocalName = "Portugal", CountryName = "Portugal" },
                new Country { Id = 184, CountryIsoCode2 = "PW", CountryLocalName = "Palau", CountryName = "Palau" },
                new Country { Id = 185, CountryIsoCode2 = "PY", CountryLocalName = "Paraguay", CountryName = "Paraguay" },
                new Country { Id = 186, CountryIsoCode2 = "QA", CountryLocalName = "Qatar", CountryName = "Qatar" },
                new Country { Id = 187, CountryIsoCode2 = "RE", CountryLocalName = "Reunion", CountryName = "Reunion" },
                new Country { Id = 188, CountryIsoCode2 = "RO", CountryLocalName = "Romania", CountryName = "Romania" },
                new Country { Id = 189, CountryIsoCode2 = "RS", CountryLocalName = "Serbia", CountryName = "Serbia" },
                new Country { Id = 190, CountryIsoCode2 = "RU", CountryLocalName = "Russian Federation", CountryName = "Russian Federation" },
                new Country { Id = 191, CountryIsoCode2 = "RW", CountryLocalName = "Rwanda", CountryName = "Rwanda" },
                new Country { Id = 192, CountryIsoCode2 = "SA", CountryLocalName = "Saudi Arabia", CountryName = "Saudi Arabia" },
                new Country { Id = 193, CountryIsoCode2 = "SB", CountryLocalName = "Solomon Islands", CountryName = "Solomon Islands" },
                new Country { Id = 194, CountryIsoCode2 = "SC", CountryLocalName = "Seychelles", CountryName = "Seychelles" },
                new Country { Id = 195, CountryIsoCode2 = "SD", CountryLocalName = "Sudan", CountryName = "Sudan" },
                new Country { Id = 196, CountryIsoCode2 = "SE", CountryLocalName = "Sweden", CountryName = "Sweden" },
                new Country { Id = 197, CountryIsoCode2 = "SG", CountryLocalName = "Singapore", CountryName = "Singapore" },
                new Country { Id = 198, CountryIsoCode2 = "SH", CountryLocalName = "Saint Helena, Ascension and Tristan Da Cunha", CountryName = "Saint Helena, Ascension and Tristan Da Cunha" },
                new Country { Id = 199, CountryIsoCode2 = "SI", CountryLocalName = "Slovenia", CountryName = "Slovenia" },
                new Country { Id = 200, CountryIsoCode2 = "SJ", CountryLocalName = "Svalbard and Jan Mayen", CountryName = "Svalbard and Jan Mayen" },
                new Country { Id = 201, CountryIsoCode2 = "SK", CountryLocalName = "Slovakia", CountryName = "Slovakia" },
                new Country { Id = 202, CountryIsoCode2 = "SL", CountryLocalName = "Sierra Leone", CountryName = "Sierra Leone" },
                new Country { Id = 203, CountryIsoCode2 = "SM", CountryLocalName = "San Marino", CountryName = "San Marino" },
                new Country { Id = 204, CountryIsoCode2 = "SN", CountryLocalName = "Senegal", CountryName = "Senegal" },
                new Country { Id = 205, CountryIsoCode2 = "SO", CountryLocalName = "Somalia", CountryName = "Somalia" },
                new Country { Id = 206, CountryIsoCode2 = "SR", CountryLocalName = "Suriname", CountryName = "Suriname" },
                new Country { Id = 207, CountryIsoCode2 = "SS", CountryLocalName = "South Sudan", CountryName = "South Sudan" },
                new Country { Id = 208, CountryIsoCode2 = "ST", CountryLocalName = "Sao Tome and Principe", CountryName = "Sao Tome and Principe" },
                new Country { Id = 209, CountryIsoCode2 = "SV", CountryLocalName = "El Salvador", CountryName = "El Salvador" },
                new Country { Id = 210, CountryIsoCode2 = "SX", CountryLocalName = "Sint Maarten (Dutch Part)", CountryName = "Sint Maarten (Dutch Part)" },
                new Country { Id = 211, CountryIsoCode2 = "SY", CountryLocalName = "Syrian Arab Republic", CountryName = "Syrian Arab Republic" },
                new Country { Id = 212, CountryIsoCode2 = "SZ", CountryLocalName = "Eswatini", CountryName = "Eswatini" },
                new Country { Id = 213, CountryIsoCode2 = "TC", CountryLocalName = "Turks and Caicos Islands", CountryName = "Turks and Caicos Islands" },
                new Country { Id = 214, CountryIsoCode2 = "TD", CountryLocalName = "Chad", CountryName = "Chad" },
                new Country { Id = 215, CountryIsoCode2 = "TF", CountryLocalName = "French Southern Territories", CountryName = "French Southern Territories" },
                new Country { Id = 216, CountryIsoCode2 = "TG", CountryLocalName = "Togo", CountryName = "Togo" },
                new Country { Id = 217, CountryIsoCode2 = "TH", CountryLocalName = "Thailand", CountryName = "Thailand" },
                new Country { Id = 218, CountryIsoCode2 = "TJ", CountryLocalName = "Tajikistan", CountryName = "Tajikistan" },
                new Country { Id = 219, CountryIsoCode2 = "TK", CountryLocalName = "Tokelau", CountryName = "Tokelau" },
                new Country { Id = 220, CountryIsoCode2 = "TL", CountryLocalName = "Timor-Leste", CountryName = "Timor-Leste" },
                new Country { Id = 221, CountryIsoCode2 = "TM", CountryLocalName = "Turkmenistan", CountryName = "Turkmenistan" },
                new Country { Id = 222, CountryIsoCode2 = "TN", CountryLocalName = "Tunisia", CountryName = "Tunisia" },
                new Country { Id = 223, CountryIsoCode2 = "TO", CountryLocalName = "Tonga", CountryName = "Tonga" },
                new Country { Id = 224, CountryIsoCode2 = "TR", CountryLocalName = "Türkiye", CountryName = "Türkiye" },
                new Country { Id = 225, CountryIsoCode2 = "TT", CountryLocalName = "Trinidad and Tobago", CountryName = "Trinidad and Tobago" },
                new Country { Id = 226, CountryIsoCode2 = "TV", CountryLocalName = "Tuvalu", CountryName = "Tuvalu" },
                new Country { Id = 227, CountryIsoCode2 = "TW", CountryLocalName = "Taiwan, Province of China", CountryName = "Taiwan, Province of China" },
                new Country { Id = 228, CountryIsoCode2 = "TZ", CountryLocalName = "Tanzania, United Republic of", CountryName = "Tanzania, United Republic of" },
                new Country { Id = 229, CountryIsoCode2 = "UA", CountryLocalName = "Ukraine", CountryName = "Ukraine" },
                new Country { Id = 230, CountryIsoCode2 = "UG", CountryLocalName = "Uganda", CountryName = "Uganda" },
                new Country { Id = 231, CountryIsoCode2 = "UM", CountryLocalName = "United States Minor Outlying Islands", CountryName = "United States Minor Outlying Islands" },
                new Country { Id = 232, CountryIsoCode2 = "US", CountryLocalName = "United States ", CountryName = "United States " },
                new Country { Id = 233, CountryIsoCode2 = "UY", CountryLocalName = "Uruguay", CountryName = "Uruguay" },
                new Country { Id = 234, CountryIsoCode2 = "UZ", CountryLocalName = "Uzbekistan", CountryName = "Uzbekistan" },
                new Country { Id = 235, CountryIsoCode2 = "VA", CountryLocalName = "Holy See (Vatican City State)", CountryName = "Holy See (Vatican City State)" },
                new Country { Id = 236, CountryIsoCode2 = "VC", CountryLocalName = "Saint Vincent and the Grenadines", CountryName = "Saint Vincent and the Grenadines" },
                new Country { Id = 237, CountryIsoCode2 = "VE", CountryLocalName = "Venezuela", CountryName = "Venezuela" },
                new Country { Id = 238, CountryIsoCode2 = "VG", CountryLocalName = "Virgin Islands, British", CountryName = "Virgin Islands, British" },
                new Country { Id = 239, CountryIsoCode2 = "VI", CountryLocalName = "Virgin Islands, U.S.", CountryName = "Virgin Islands, U.S." },
                new Country { Id = 240, CountryIsoCode2 = "VN", CountryLocalName = "Viet Nam", CountryName = "Viet Nam" },
                new Country { Id = 241, CountryIsoCode2 = "VU", CountryLocalName = "Vanuatu", CountryName = "Vanuatu" },
                new Country { Id = 242, CountryIsoCode2 = "WF", CountryLocalName = "Wallis and Futuna", CountryName = "Wallis and Futuna" },
                new Country { Id = 243, CountryIsoCode2 = "WS", CountryLocalName = "Samoa", CountryName = "Samoa" },
                new Country { Id = 244, CountryIsoCode2 = "XZ", CountryLocalName = "Installations in International Waters", CountryName = "Installations in International Waters" },
                new Country { Id = 245, CountryIsoCode2 = "YE", CountryLocalName = "Yemen", CountryName = "Yemen" },
                new Country { Id = 246, CountryIsoCode2 = "YT", CountryLocalName = "Mayotte", CountryName = "Mayotte" },
                new Country { Id = 247, CountryIsoCode2 = "ZA", CountryLocalName = "South Africa", CountryName = "South Africa" },
                new Country { Id = 248, CountryIsoCode2 = "ZM", CountryLocalName = "Zambia", CountryName = "Zambia" },
                new Country { Id = 249, CountryIsoCode2 = "ZW", CountryLocalName = "Zimbabwe", CountryName = "Zimbabwe" });
        }
    }
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)

        {
            builder.ToTable("Locations", "base");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.LocationCode).IsRequired().HasMaxLength(20);
            builder.Property(l => l.LocationName).IsRequired().HasMaxLength(200);
            builder.Property(l => l.LocationLocalName).IsRequired().HasMaxLength(200);
            builder.Property(l => l.IATACode).HasMaxLength(30);
            builder.Property(l => l.ICAOCode).HasMaxLength(30);
            builder.Property(l => l.Address).HasMaxLength(100);
            builder.Property(l => l.Phone).HasMaxLength(50);
            builder.Property(l => l.Email).HasMaxLength(35);

            builder.HasOne(x => x.LocationType).WithMany().HasForeignKey(x => x.LocationTypeId);
            builder.HasOne(x => x.ParentLocation).WithMany().HasForeignKey(x => x.ParentLocationId);
            builder.HasOne(x => x.DepotType).WithMany().HasForeignKey(x => x.DepotTypeId);
            builder.HasOne(x => x.TerminalType).WithMany().HasForeignKey(x => x.TerminalTypeId);
            builder.HasOne(x => x.CityType).WithMany().HasForeignKey(x => x.CityTypeId);
            builder.HasOne(x => x.CityForPort).WithMany().HasForeignKey(x => x.CityIdForPort);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
            builder.HasOne(x => x.Country2).WithMany().HasForeignKey(x => x.CountryId2);

            builder.HasIndex(e => e.LocationCode).HasDatabaseName("IX_Locations_LocationCode").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.LocationName).HasDatabaseName("IX_Locations_LocationName").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.LocationLocalName).HasDatabaseName("IX_Locations_LocationLocalName").IsUnique(false).IsClustered(false);


        }
    }

    public class CargoCommodityConfiguration : IEntityTypeConfiguration<CargoCommodity>
    {
        public void Configure(EntityTypeBuilder<CargoCommodity> builder)
        {
            builder.ToTable("CargoCommodities", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CargoCommodityName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.CargoCommodityLocalName).IsRequired().HasMaxLength(100);
        }
    }
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Banks", "base");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.BankCode).IsRequired().HasMaxLength(20);
            builder.Property(c => c.BankName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.BankLocalName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.SWIFTCode).HasMaxLength(25);
            builder.Property(c => c.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
        }
    }

    public class BankAccountPurposeConfiguration : IEntityTypeConfiguration<BankAccountPurpose>
    {
        public void Configure(EntityTypeBuilder<BankAccountPurpose> builder)
        {
            builder.ToTable("BankAccountPurposes", "base");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.BankAccountPurposeName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.BankAccountPurposeLocalName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new BankAccountPurpose { Id = 1, BankAccountPurposeName = "General", BankAccountPurposeLocalName = "عمومی" },
                new BankAccountPurpose { Id = 2, BankAccountPurposeName = "Local Charges", BankAccountPurposeLocalName = "هزینه های محلی" },
                new BankAccountPurpose { Id = 3, BankAccountPurposeName = "Cash Deposit", BankAccountPurposeLocalName = "سپرده نقدی" }
                );
        }
    }




    public class VesselOwnTypeConfiguration : IEntityTypeConfiguration<VesselOwnType>
    {
        public void Configure(EntityTypeBuilder<VesselOwnType> builder)
        {
            builder.ToTable("VesselOwnTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.VesselOwnTypeName).IsRequired().HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new VesselOwnType { Id = 1, VesselOwnTypeName = "Owned" },
                new VesselOwnType { Id = 2, VesselOwnTypeName = "Time Chartered" },
                new VesselOwnType { Id = 3, VesselOwnTypeName = "Exclusive Feeder" },
                new VesselOwnType { Id = 4, VesselOwnTypeName = "Common Feeder" }
            );


        }
    }


    public class IndustryConfiguration : IEntityTypeConfiguration<Industry>
    {
        public void Configure(EntityTypeBuilder<Industry> builder)
        {
            builder.ToTable("Industries", "base");
            builder.HasKey(c => c.Id);
            builder.Property("IndustryName").IsRequired().HasMaxLength(100);



        }
    }



    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.PersonName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PersonPersianName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.FirstName).HasMaxLength(35);
            builder.Property(x => x.LastName).HasMaxLength(50);
            builder.Property(x => x.EmailAddress).HasMaxLength(35);
            builder.Property(x => x.Phone).HasMaxLength(50);
            builder.Property(x => x.Address).HasMaxLength(200);
            builder.Property(x => x.PersianAddress).HasMaxLength(200);
            builder.Property(x => x.NationalID).HasMaxLength(20);
            builder.Property(x => x.Mobile).HasMaxLength(20);
            builder.Property(x => x.FaxNo).HasMaxLength(20);
            builder.Property(x => x.ZipCode).HasMaxLength(20);
            builder.Property(x => x.EcoCode).HasMaxLength(20);
            builder.Property(x => x.RegNo).HasMaxLength(20);

            builder.HasOne(x => x.PersonalityType).WithMany().HasForeignKey(x => x.PersonalityTypeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(e => e.PersonName).HasDatabaseName("IX_Persons_PersonName").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.PersonPersianName).HasDatabaseName("IX_Persons_PersonPersianName").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.NationalID).HasDatabaseName("IX_Persons_NationalID").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.Mobile).HasDatabaseName("IX_Persons_Mobile").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.EmailAddress).HasDatabaseName("IX_Persons_EmailAddress").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.Address).HasDatabaseName("IX_Persons_Address").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.PersianAddress).HasDatabaseName("IX_Persons_PersianAddress").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.Phone).HasDatabaseName("IX_Persons_Phone").IsUnique(false).IsClustered(false);

        }
    }
    public class PersonRoleConfiguration : IEntityTypeConfiguration<PersonRole>
    {
        public void Configure(EntityTypeBuilder<PersonRole> builder)
        {
            builder.ToTable("PersonRoles", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.RoleType).WithMany().HasForeignKey(x => x.RoleTypeId);
        }
    }

    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.BranchCode).HasMaxLength(30);
            builder.Property(x => x.BranchName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).HasMaxLength(200);
            builder.Property(x => x.ManagerName).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.Phone).HasMaxLength(50);
            builder.Property(x => x.Email).HasMaxLength(60);

            builder.HasOne(x => x.Parent).WithMany(x => x.Branches).HasForeignKey(x => x.ParentId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
            builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId);
            builder.HasOne(x => x.BranchType).WithMany().HasForeignKey(x => x.BranchTypeId);
        }
    }

    public class PersonAddressConfiguration : IEntityTypeConfiguration<PersonAddress>
    {
        public void Configure(EntityTypeBuilder<PersonAddress> builder)
        {
            builder.ToTable("PersonAddresses", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.AddressTitle).IsRequired().HasMaxLength(50);
            builder.Property(x => x.AddressLine1).IsRequired().HasMaxLength(300);
            builder.Property(x => x.AddressLine2).IsRequired().HasMaxLength(300);
            builder.Property(x => x.PostalCode).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Person).WithMany(x => x.Addresses).HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
            builder.HasOne(x => x.Province).WithMany().HasForeignKey(x => x.ProvinceId);
            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId);
            builder.HasOne(x => x.AddressType).WithMany().HasForeignKey(x => x.AddressTypeId);
        }
    }

    public class PersonPicConfiguration : IEntityTypeConfiguration<PersonPic>
    {
        public void Configure(EntityTypeBuilder<PersonPic> builder)
        {
            builder.ToTable("PersonPics", "base");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
        }
    }

    public class PersonExtraInfoConfiguration : IEntityTypeConfiguration<PersonExtraInfo>
    {
        public void Configure(EntityTypeBuilder<PersonExtraInfo> builder)
        {
            builder.ToTable("PersonExtraInfos", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.PersonExtraInfoValue).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Person).WithMany(x => x.PersonExtraInfos).HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.InfoType).WithMany().HasForeignKey(x => x.InfoTypeId);
        }
    }


    public class PersonContactConfiguration : IEntityTypeConfiguration<PersonContact>
    {
        public void Configure(EntityTypeBuilder<PersonContact> builder)
        {
            builder.ToTable("PersonContacts", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Role).HasMaxLength(100);
            builder.Property(c => c.FullName).HasMaxLength(50);
            builder.Property(c => c.Email).HasMaxLength(50);
            builder.Property(c => c.Phone).HasMaxLength(50);
            builder.Property(c => c.Mobile).HasMaxLength(50);

            builder.HasOne(x => x.Person).WithMany(x => x.PersonContacts).HasForeignKey(x => x.PersonId);
        }
    }

    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            builder.ToTable("Agents", "base");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.Id);
            builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class AgentActScopeConfiguration : IEntityTypeConfiguration<AgentActScope>
    {
        public void Configure(EntityTypeBuilder<AgentActScope> builder)
        {
            builder.ToTable("AgentActScopes", "base");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Agent).WithMany(x => x.AgentActScopes).HasForeignKey(x => x.AgentId);
            builder.HasOne(x => x.AgentScope).WithMany().HasForeignKey(x => x.AgentScopeId);

        }
    }




    public class BlackListReasonConfiguration : IEntityTypeConfiguration<BlackListReason>
    {
        public void Configure(EntityTypeBuilder<BlackListReason> builder)
        {
            builder.ToTable("BlackListReasons", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BlackListReasonName).IsRequired().HasMaxLength(70);

            builder.HasData(
                new BlackListReason { Id = 100, BlackListReasonName = "بدهی سیستمی", SystemDefined = true }
                );
        }
    }


    public class TerritoryConfiguration : IEntityTypeConfiguration<Territory>
    {
        public void Configure(EntityTypeBuilder<Territory> builder)
        {
            builder.ToTable("Territories", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TerritoryName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Remarks).IsRequired().HasMaxLength(200);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class TerritoryCityConfiguration : IEntityTypeConfiguration<TerritoryCity>
    {
        public void Configure(EntityTypeBuilder<TerritoryCity> builder)
        {
            builder.ToTable("TerritoryCities", "base");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId);
            builder.HasOne(x => x.Territory).WithMany(x => x.TerritoryCities).HasForeignKey(x => x.TerritoryId);
        }
    }

    public class ContainerIOTypeConfiguration : IEntityTypeConfiguration<ContainerIOType>
    {
        public void Configure(EntityTypeBuilder<ContainerIOType> builder)
        {
            builder.ToTable("ContainerIOTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContainerIOTypeName).IsRequired().HasMaxLength(100);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(c => c.InOutId);
        }
    }


    public class VoyageTypeConfiguration : IEntityTypeConfiguration<VoyageType>
    {
        public void Configure(EntityTypeBuilder<VoyageType> builder)
        {
            builder.ToTable("VoyageTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.VoyageTypeName).IsRequired().HasMaxLength(20);

            builder.HasData(
                new VoyageType { Id = 1, VoyageTypeName = "Pre Carriage Voyage" },
                new VoyageType { Id = 2, VoyageTypeName = "Main Voyage" },
                new VoyageType { Id = 3, VoyageTypeName = "On Carriage Voyage" }
                );
        }
    }

    public class VoyageOwnTypeConfiguration : IEntityTypeConfiguration<VoyageOwnType>
    {
        public void Configure(EntityTypeBuilder<VoyageOwnType> builder)
        {
            builder.ToTable("VoyageOwnTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.VoyageOwnTypeName).IsRequired().HasMaxLength(35);
        }
    }



    public class CourierMethodConfiguration : IEntityTypeConfiguration<CourierMethod>
    {
        public void Configure(EntityTypeBuilder<CourierMethod> builder)
        {
            builder.ToTable("CourierMethods", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CourierMethodName).IsRequired().HasMaxLength(50);

            // Seed Data
            builder.HasData(
                new CourierMethod { Id = 1, CourierMethodName = "Courier" },
                new CourierMethod { Id = 2, CourierMethodName = "Hand Carry" },
                new CourierMethod { Id = 3, CourierMethodName = "Post" }
            );


        }
    }

    public class EventTypeCategoryConfiguration : IEntityTypeConfiguration<EventTypeCategory>
    {
        public void Configure(EntityTypeBuilder<EventTypeCategory> builder)
        {
            builder.ToTable("EventTypeCategory", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.EventTypeCategoryName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EventTypeCategoryLocalName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new EventTypeCategory { Id = 1, EventTypeCategoryName = "Voyage Events", EventTypeCategoryLocalName = "سفرهای حمل و نقل دریایی", SystemDefinded = true },
                new EventTypeCategory { Id = 2, EventTypeCategoryName = "Shipment and Legs Events", EventTypeCategoryLocalName = "مسیرهای حمل و نقل", SystemDefinded = true },
                new EventTypeCategory { Id = 3, EventTypeCategoryName = "Booking Events", EventTypeCategoryLocalName = "بوکینگ ها", SystemDefinded = true },
                new EventTypeCategory { Id = 4, EventTypeCategoryName = "BL Events", EventTypeCategoryLocalName = "بارنامه ها", SystemDefinded = true },
                new EventTypeCategory { Id = 5, EventTypeCategoryName = "Invoice Events", EventTypeCategoryLocalName = "فاکتورها", SystemDefinded = true }
                );
        }
    }



    public class EventTypeConfiguration : IEntityTypeConfiguration<EventType>
    {
        public void Configure(EntityTypeBuilder<EventType> builder)
        {
            builder.ToTable("EventTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.EventTypeCode).IsRequired().HasMaxLength(30);
            builder.Property(x => x.EventTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EventTypeRemarks).IsRequired().HasMaxLength(200);



            builder.HasOne(x => x.ObjectType).WithMany().HasForeignKey(x => x.ObjectTypeId);
            builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);

            builder.HasData(
                new EventType { Id = 100, CategoryId = 1, EventTypeCode = "", EventTypeName = "Vessel ETA Updated", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 105, CategoryId = 1, EventTypeCode = "", EventTypeName = "Arrival To Port", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 110, CategoryId = 1, EventTypeCode = "", EventTypeName = "Vessel Berthed", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 115, CategoryId = 1, EventTypeCode = "", EventTypeName = "Operations Commenced", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 116, CategoryId = 1, EventTypeCode = "", EventTypeName = "Discharge Commenced", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 117, CategoryId = 1, EventTypeCode = "", EventTypeName = "Loading Commenced", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 120, CategoryId = 1, EventTypeCode = "", EventTypeName = "Operations Completed", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 121, CategoryId = 1, EventTypeCode = "", EventTypeName = "Discharge Completed", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 122, CategoryId = 1, EventTypeCode = "", EventTypeName = "Loading Completed", IsSystemEvent = false, SystemDefined = true },
                new EventType { Id = 125, CategoryId = 1, EventTypeCode = "", EventTypeName = "Voyage Completed", IsSystemEvent = false, SystemDefined = true },

                new EventType { Id = 200, CategoryId = 2, EventTypeCode = "", EventTypeName = "Shipment Report", IsSystemEvent = false, SystemDefined = true },

                new EventType { Id = 300, CategoryId = 3, EventTypeCode = "", EventTypeName = "Booking Report", IsSystemEvent = false, SystemDefined = true },

                new EventType { Id = 400, CategoryId = 4, EventTypeCode = "", EventTypeName = "BL Report", IsSystemEvent = false, SystemDefined = true },

                new EventType { Id = 500, CategoryId = 5, EventTypeCode = "", EventTypeName = "Invoice Report", IsSystemEvent = false, SystemDefined = true }
                );
        }
    }

    public class DocumentTypeCategoryConfiguration : IEntityTypeConfiguration<DocumentTypeCategory>
    {
        public void Configure(EntityTypeBuilder<DocumentTypeCategory> builder)
        {
            builder.ToTable("DocumentTypeCategories", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentTypeCategoryName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new DocumentTypeCategory { Id = 1, DocumentTypeCategoryName = "TRANSPORT" },
                new DocumentTypeCategory { Id = 2, DocumentTypeCategoryName = "COMMERCIAL" },
                new DocumentTypeCategory { Id = 3, DocumentTypeCategoryName = "REGULATORY" },
                new DocumentTypeCategory { Id = 4, DocumentTypeCategoryName = "INSURANCE" },
                new DocumentTypeCategory { Id = 5, DocumentTypeCategoryName = "DELIVERY" },
                new DocumentTypeCategory { Id = 6, DocumentTypeCategoryName = "INSTRUCTION" },
                new DocumentTypeCategory { Id = 99, DocumentTypeCategoryName = "OTHER" }
                );

        }
    }

    public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
    {
        public void Configure(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("DocumentTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DocumentTypeName).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.DocumentTypeCategory).WithMany().HasForeignKey(x => x.DocumentTypeCategoryId);
            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.UpdateUserId);
            builder.HasOne(x => x.DeleteUser).WithMany().HasForeignKey(x => x.DeleteUserId);

            builder.HasData(
                new DocumentType { Id = 100, DocumentTypeCategoryId = 1, DocumentTypeName = "Master BL", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 101, DocumentTypeCategoryId = 1, DocumentTypeName = "House BL", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 102, DocumentTypeCategoryId = 1, DocumentTypeName = "Master Air Waybill", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 103, DocumentTypeCategoryId = 1, DocumentTypeName = "House Air Waybill", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 104, DocumentTypeCategoryId = 1, DocumentTypeName = "CMR", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },

                new DocumentType { Id = 50, DocumentTypeCategoryId = 2, DocumentTypeName = "Rate Request Documents", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },

                new DocumentType { Id = 200, DocumentTypeCategoryId = 2, DocumentTypeName = "Performa Invoice", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 250, DocumentTypeCategoryId = 2, DocumentTypeName = "Petty Cash(تنخواه)", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 251, DocumentTypeCategoryId = 2, DocumentTypeName = "Petty Cash Expenses(هزینه کرد تنخواه)", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },
                new DocumentType { Id = 252, DocumentTypeCategoryId = 2, DocumentTypeName = "Petty Cash Recharge(شارژ مجدد تنخواه)", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 },

                new DocumentType { Id = 500, DocumentTypeCategoryId = 5, DocumentTypeName = "Temporary Import Permission(TIP)", HasExpiryDate = true, SystemDefined = true, AlarmDaysBeforeExpire = 30 },
                new DocumentType { Id = 505, DocumentTypeCategoryId = 5, DocumentTypeName = "Notice Of Arrival(NOA)", HasExpiryDate = false, SystemDefined = true, AlarmDaysBeforeExpire = 0 }

            );
        }
    }
    public class DocumentTypeObjectTypeConfiguration : IEntityTypeConfiguration<DocumentTypeObjectType>
    {
        public void Configure(EntityTypeBuilder<DocumentTypeObjectType> builder)
        {
            builder.ToTable("DocumentTypeObjectTypes", "base");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.DocumentType).WithMany().HasForeignKey(x => x.DocumentTypeId);
            builder.HasOne(x => x.ObjectType).WithMany().HasForeignKey(x => x.ObjectTypeId);

            builder.HasData(
                new DocumentTypeObjectType { Id = 1, ObjectTypeId = 5, DocumentTypeId = 100, SystemDefined = true },
                new DocumentTypeObjectType { Id = 2, ObjectTypeId = 5, DocumentTypeId = 101, SystemDefined = true },
                new DocumentTypeObjectType { Id = 3, ObjectTypeId = 5, DocumentTypeId = 102, SystemDefined = true },
                new DocumentTypeObjectType { Id = 4, ObjectTypeId = 5, DocumentTypeId = 103, SystemDefined = true },
                new DocumentTypeObjectType { Id = 5, ObjectTypeId = 5, DocumentTypeId = 104, SystemDefined = true },
                new DocumentTypeObjectType { Id = 6, ObjectTypeId = 11, DocumentTypeId = 50, SystemDefined = true }
            );


        }
    }
    public class EmailDirectionConfiguration : IEntityTypeConfiguration<EmailDirection>
    {
        public void Configure(EntityTypeBuilder<EmailDirection> builder)
        {
            builder.ToTable("EmailDirections", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.EmailDirectionName).IsRequired().HasMaxLength(10);
            builder.HasData(
                new EmailDirection { Id = 1, EmailDirectionName = "Send" },
                new EmailDirection { Id = 2, EmailDirectionName = "Receive" }
                );
        }
    }

    public class ChargeTypeCategoryConfiguration : IEntityTypeConfiguration<ChargeTypeCategory>
    {
        public void Configure(EntityTypeBuilder<ChargeTypeCategory> builder)
        {
            builder.ToTable("ChargeTypeCategory", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ChargeTypeCategoryName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ChargeTypeCategoryLocalName).IsRequired().HasMaxLength(100);

            ////Seed Data
            builder.HasData(
                new ChargeTypeCategory { Id = 1, DisplayOrder = 1, ChargeTypeCategoryName = "FREIGHT", ChargeTypeCategoryLocalName = "هزینه های حمل" },
                new ChargeTypeCategory { Id = 2, DisplayOrder = 2, ChargeTypeCategoryName = "LOCAL CHARGES", ChargeTypeCategoryLocalName = "هزینه های محلی" },
                new ChargeTypeCategory { Id = 3, DisplayOrder = 3, ChargeTypeCategoryName = "DEMMURAGE & DETENTION CHARGES", ChargeTypeCategoryLocalName = "هزینه های دیرکرد" },
                new ChargeTypeCategory { Id = 4, DisplayOrder = 4, ChargeTypeCategoryName = "THC CHARGES", ChargeTypeCategoryLocalName = "هزینه های تی اچ سی" },
                new ChargeTypeCategory { Id = 5, DisplayOrder = 5, ChargeTypeCategoryName = "CUSTOMS", ChargeTypeCategoryLocalName = "هزینه های گمرکی" },

                //new ChargeTypeCategory { Id = 6, DisplayOrder = 5, ChargeTypeCategoryName = "DOC", ChargeTypeCategoryLocalName = "هزینه های مستندات" },
                //new ChargeTypeCategory { Id = 7, DisplayOrder = 6, ChargeTypeCategoryName = "TRUCKING", ChargeTypeCategoryLocalName = "هزینه های تراک" },
                //new ChargeTypeCategory { Id = 8, DisplayOrder = 7, ChargeTypeCategoryName = "WAREHOUSE", ChargeTypeCategoryLocalName = "هزینه های انبارداری" },
                //new ChargeTypeCategory { Id = 10, DisplayOrder = 9, ChargeTypeCategoryName = "INSURANCE", ChargeTypeCategoryLocalName = "هزینه های بیمه" },

                new ChargeTypeCategory { Id = 99, DisplayOrder = 99, ChargeTypeCategoryName = "OTHER", ChargeTypeCategoryLocalName = "سایر" }
                );
        }
    }
    public class ChargeTypeConfiguration : IEntityTypeConfiguration<ChargeType>
    {
        public void Configure(EntityTypeBuilder<ChargeType> builder)
        {
            builder.ToTable("ChargeTypes", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.ChargeTypeCode).HasMaxLength(20);
            builder.Property(x => x.ChargeTypeCode2).HasMaxLength(20);
            builder.Property(x => x.ChargeTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ChargeTypeLocalName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.ChargeTypeCategory).WithMany(x => x.ChargeTypes).HasForeignKey(x => x.ChargeTypeCategoryId);
            builder.HasOne(x => x.DependsOnChargeType).WithMany().HasForeignKey(x => x.DependsOnChargeTypeId);
            builder.HasOne(x => x.CalculationBy).WithMany().HasForeignKey(x => x.CalculationById);
            builder.HasOne(x => x.CalculationType).WithMany().HasForeignKey(x => x.CalculationTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

            //Seed Data
            builder.HasData(
                    new ChargeType { Id = 1, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Ocean Freight", ChargeTypeLocalName = "کرایه حمل دریایی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 2, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Road Freight", ChargeTypeLocalName = "کرایه حمل زمینی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 3, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Air Freight", ChargeTypeLocalName = "کرایه حمل هوائی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 4, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Freight", ChargeTypeLocalName = "کرایه حمل", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 5, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Door to Destination Freight", ChargeTypeLocalName = "کرایه حمل درب کارخانه تا مقصد", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 6, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Telex Release", ChargeTypeLocalName = "تلکس ریلیز", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 7, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Warehouse Receipt", ChargeTypeLocalName = "قبض انبار", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 8, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Container Identity Certificate", ChargeTypeLocalName = "صدور شناسنامه کانتینر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 9, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Port Delivery Order", ChargeTypeLocalName = "صدور ترخیصیه در بندر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 10, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Advance Demurrage", ChargeTypeLocalName = "دموراژ علی الحساب", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 11, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Consumable Voucher", ChargeTypeLocalName = "بن مصرفی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 12, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Freight Forwarding Service", ChargeTypeLocalName = "خدمات فورواردری", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 13, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Lift On", ChargeTypeLocalName = "لیفت آن", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 14, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Customs Services", ChargeTypeLocalName = "خدمات گمرکی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 15, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Port Services", ChargeTypeLocalName = "خدمات بندری", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 16, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Transit Service", ChargeTypeLocalName = "ترانزیت", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 18, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Warehousing", ChargeTypeLocalName = "انبارداری", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 19, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Depot Storage", ChargeTypeLocalName = "دپو", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 20, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Commodity Identification Code", ChargeTypeLocalName = "کد شناسایی کالا", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 21, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Invoice and Packing List", ChargeTypeLocalName = "صورتحساب و پکینگ لیست", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 22, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Commercial Card", ChargeTypeLocalName = "کارت بازرگانی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 23, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Health Certificate", ChargeTypeLocalName = "گواهی بهداشت", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 24, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Local Charges", ChargeTypeLocalName = "هزینه های محلی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 25, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Insurance ", ChargeTypeLocalName = "بیمه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 26, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Container Yard Fire Insurance", ChargeTypeLocalName = "بیمه خسارت آتش سوزی محوطه کانتینری", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 27, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Environmental Fuel Surcharge(EFS)", ChargeTypeLocalName = "سوخت محیطی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 28, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "War Risk Surcharge", ChargeTypeLocalName = "ریسک مناطق جنگی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 29, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Low Sulphur Surcharge(LSS) ", ChargeTypeLocalName = " کاهش سولفور سوخت", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 30, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Factory Pickup Loading", ChargeTypeLocalName = "بارگیری از درب کارخانه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 31, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Document Courier", ChargeTypeLocalName = "پست اسناد", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 32, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Container Handling(LoLo)", ChargeTypeLocalName = "درآمد جابجایی کانتینر LoLo", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 33, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Certificate of Origin", ChargeTypeLocalName = "گواهی مبدا", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 34, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "EDI Service", ChargeTypeLocalName = "EDI", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 35, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Peak Season Surcharge", ChargeTypeLocalName = "اضافه فصلی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 36, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Air Courier", ChargeTypeLocalName = "پست هوایی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 37, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Miscellaneous Operating", ChargeTypeLocalName = "متفرقه عملیاتی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 38, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Temporary Import Declaration Printing", ChargeTypeLocalName = "چاپ اظهارنامه برای جواز ورود موقت", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 39, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Cargo Cancellation", ChargeTypeLocalName = "کنسلی بار", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 60, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Laboratory Service", ChargeTypeLocalName = "سرویسهای آزمایشگاهی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 61, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Fuel Charges", ChargeTypeLocalName = "سوخت مصرفی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 62, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Domestic Transit Commission", ChargeTypeLocalName = "کمیسیون ترانزیت داخلی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 63, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Trading", ChargeTypeLocalName = "تریدینگ", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 64, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Remittance Commission", ChargeTypeLocalName = "کارمزد حواله", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 65, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Agency Commission", ChargeTypeLocalName = "کارمزد نمایندگی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 66, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "FAC Commission", ChargeTypeLocalName = "FAC کمیسیون", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 67, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Carrier Commission(Credit)", ChargeTypeLocalName = "کمیسیون از خط کردیت", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 68, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Commission", ChargeTypeLocalName = "کمیسیون", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 69, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Inspection", ChargeTypeLocalName = "بازرسی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 70, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Late Payment Penalty", ChargeTypeLocalName = "جریمه دیرکرد", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 71, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Third Party", ChargeTypeLocalName = "نام شخص ثالث", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 72, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "THC Brokerage Service", ChargeTypeLocalName = "درآمد خدمات کارگزاری تی اچ سی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 73, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Goods Purchase", ChargeTypeLocalName = "خرید کالا", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 74, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Sales Return", ChargeTypeLocalName = "برگشت از فروش", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 75, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Cash Discount on Sales", ChargeTypeLocalName = "تخفیفات نقدی فروش", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 76, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Foreign Exchange Gain/Loss", ChargeTypeLocalName = " سود و زیان تسعیر ارز", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 77, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Other Non Operating Income and Expenses", ChargeTypeLocalName = "سایر درآمدها و هزینه های غیر عملیاتی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 78, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Other Income Insurance Share 30%", ChargeTypeLocalName = "سایر درآمدها بابت بیمه 30 درصد", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 79, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Transfer Fee", ChargeTypeLocalName = "Transfer Fee", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 80, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Bad Debt", ChargeTypeLocalName = "هزینه مطالبات سوخت شده", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 81, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Other Payroll Tax Related", ChargeTypeLocalName = "سایر درآمدها بابت مالیات حقوق", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 82, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "USD Exchange Gain", ChargeTypeLocalName = "سود دلاری اختلاف نرخ", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 83, ChargeTypeCategoryId = 1, ChargeTypeCode = "", ChargeTypeName = "Bank Deposit Interest", ChargeTypeLocalName = "سود سپرده بانکی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },

                    //Tariff darand 100-149  Mahalli ----150-159 : THC 160-170 :Detention
                    new ChargeType { Id = 100, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Delivery Order Issuance", ChargeTypeLocalName = "صدور ترخیصیه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 101, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Temporary Import Permit", ChargeTypeLocalName = "پروانه عبور موقت", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 102, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Empty Container Yard Unloading", ChargeTypeLocalName = "تخلیه کانتینر خالی در محوطه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 103, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Electronic Data Upload", ChargeTypeLocalName = "بارگذاری اطلاعات الکترونیک در سیستم یکپارچه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 104, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Extra HS Code", ChargeTypeLocalName = "اچ اس کد مازاد", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 105, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Container Damage Insurance", ChargeTypeLocalName = "بیمه خسارت کانتینر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 106, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Container Stripping and Shifting", ChargeTypeLocalName = "استریپ و جابجایی کانتینر مشترک با غیر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 107, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Label Removal", ChargeTypeLocalName = "برچسب زدایی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 108, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Container Seal", ChargeTypeLocalName = "پلمپ", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 109, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Tranship Statement", ChargeTypeLocalName = "حکم ترانشیپ", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 110, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Danger Upload", ChargeTypeLocalName = "بارگذاری و تبادل اسناد خطرناک", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 111, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Export Container Cancel", ChargeTypeLocalName = "کنسلی کانتینرهای صادراتی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 112, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Chabahar Container Shifting", ChargeTypeLocalName = "شیفتینگ کانتینر در چابهار", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },

                    new ChargeType { Id = 120, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Bill of Lading Issuance", ChargeTypeLocalName = "صدور بارنامه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 121, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Re‑Issuance of Bill of Lading", ChargeTypeLocalName = "صدور مجدد بارنامه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 122, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Bill of Lading Amendment", ChargeTypeLocalName = "اصلاحیه بارنامه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 123, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Switch Bill of Lading", ChargeTypeLocalName = "سوئیچ بارنامه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 125, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Re‑Issuance of Delivery Order", ChargeTypeLocalName = "صدور مجدد ترخیصیه", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 126, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "EPL", ChargeTypeLocalName = "EPL", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 127, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Handling Service", ChargeTypeLocalName = "هندلینگ", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 128, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Segregation", ChargeTypeLocalName = "تفکیک", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },

                    new ChargeType { Id = 140, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Non-Cash Deposit", ChargeTypeLocalName = "سپرده غیرنقدی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 141, ChargeTypeCategoryId = 2, ChargeTypeCode = "", ChargeTypeName = "Cash Deposit", ChargeTypeLocalName = "سپرده نقدی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },

                    new ChargeType { Id = 150, ChargeTypeCategoryId = 4, ChargeTypeCode = "", ChargeTypeName = "Import THC", ChargeTypeLocalName = "تی اچ سی وارداتی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 151, ChargeTypeCategoryId = 4, ChargeTypeCode = "", ChargeTypeName = "Export THC", ChargeTypeLocalName = "تی اچ سی صادراتی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 152, ChargeTypeCategoryId = 4, ChargeTypeCode = "", ChargeTypeName = "THC Wage", ChargeTypeLocalName = "کارگزاری تی اچ سی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 155, ChargeTypeCategoryId = 4, ChargeTypeCode = "", ChargeTypeName = "THC", ChargeTypeLocalName = "تی اچ سی", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },



                    new ChargeType { Id = 160, ChargeTypeCategoryId = 3, ChargeTypeCode = "", ChargeTypeName = "Container Detention", ChargeTypeLocalName = "دیرکرد کانتینر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 161, ChargeTypeCategoryId = 3, ChargeTypeCode = "", ChargeTypeName = "Advance Container Detention", ChargeTypeLocalName = "دیرکرد علی الحساب کانتینر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true },
                    new ChargeType { Id = 165, ChargeTypeCategoryId = 3, ChargeTypeCode = "", ChargeTypeName = "Container Damage ", ChargeTypeLocalName = "خسارت کانتینر", CalculationById = 1, IsTaxable = true, MultiplierRate = 1, SystemDefined = true }
                );
        }
    }
    public class ChequeTypeConfiguration : IEntityTypeConfiguration<ChequeType>
    {
        public void Configure(EntityTypeBuilder<ChequeType> builder)
        {
            builder.ToTable("ChequeTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ChequeTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ChequeTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new ChequeType { Id = 10, ChequeTypeName = "Normal", ChequeTypeLocalName = "عادی" },
                new ChequeType { Id = 20, ChequeTypeName = "PP Interbank", ChequeTypeLocalName = "رمز دار بین بانکی" }
                );
        }
    }
    public class ChequeStatusConfiguration : IEntityTypeConfiguration<ChequeStatus>
    {
        public void Configure(EntityTypeBuilder<ChequeStatus> builder)
        {
            builder.ToTable("ChequeStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ChequeStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ChequeStatusLocalName).IsRequired().HasMaxLength(70);

            ////Seed Data
            builder.HasData(
                new ChequeStatus { Id = 10, ChequeStatusName = "Draft", ChequeStatusLocalName = "پیش نویس" },
                new ChequeStatus { Id = 20, ChequeStatusName = "InVault", ChequeStatusLocalName = "نزد صندوق-دریافتی" },
                new ChequeStatus { Id = 30, ChequeStatusName = "Issued", ChequeStatusLocalName = "صادر شده-پرداختی" },
                new ChequeStatus { Id = 35, ChequeStatusName = "Deliverd To Party", ChequeStatusLocalName = "تحویل به ذینفع-پرداختی" },
                new ChequeStatus { Id = 40, ChequeStatusName = "InCollection", ChequeStatusLocalName = "در جریان وصول / واگذار به بانک" },
                new ChequeStatus { Id = 50, ChequeStatusName = "Cleared", ChequeStatusLocalName = "وصول شد" },
                new ChequeStatus { Id = 60, ChequeStatusName = "Bounced", ChequeStatusLocalName = "برگشت خورده/عدم پرداخت" },
                new ChequeStatus { Id = 70, ChequeStatusName = "Endorsed", ChequeStatusLocalName = "خرج شده و واگذار شده - مخصوص دریافتی" },

                new ChequeStatus { Id = 80, ChequeStatusName = "HeldAsGuarantee", ChequeStatusLocalName = "نزد طرف حساب به عنوان ضمانت" },
                new ChequeStatus { Id = 90, ChequeStatusName = "ReturnedToDrawer", ChequeStatusLocalName = "عودت شده به صادر کننده" },
                new ChequeStatus { Id = 99, ChequeStatusName = "Cancelled", ChequeStatusLocalName = "ابطال شد" }
                );
        }
    }





    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.PaymentMethodName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PaymentMethodLocalName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PaymentMethodRemarks).HasMaxLength(200);

            //Seed Data
            builder.HasData(
                new PaymentMethod { Id = 1, PaymentMethodName = "Cash", PaymentMethodLocalName = "پول نقد" },
                new PaymentMethod { Id = 2, PaymentMethodName = "Bank Transfer", PaymentMethodLocalName = "حواله بانکی" },
                new PaymentMethod { Id = 3, PaymentMethodName = "Cheque", PaymentMethodLocalName = "چک" },
                new PaymentMethod { Id = 4, PaymentMethodName = "DepositSlip", PaymentMethodLocalName = "واریز به حساب" },
                new PaymentMethod { Id = 5, PaymentMethodName = "CardToCard", PaymentMethodLocalName = "کارت به کارت" },
                new PaymentMethod { Id = 6, PaymentMethodName = "POS", PaymentMethodLocalName = "پوز" },
                new PaymentMethod { Id = 8, PaymentMethodName = "OnlinePayment", PaymentMethodLocalName = "پرداخت آنلاین از درگاه بانکی" },
                new PaymentMethod { Id = 100, PaymentMethodName = "Other", PaymentMethodLocalName = "سایر" }
                );
        }
    }


    public class SettlementItemTypeConfiguration : IEntityTypeConfiguration<SettlementItemType>
    {
        public void Configure(EntityTypeBuilder<SettlementItemType> builder)
        {
            builder.ToTable("SettlementItemTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.SettlementItemTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SettlementItemTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new SettlementItemType { Id = 1, SettlementItemTypeName = "Invoice", SettlementItemTypeLocalName = "Invoice" },
                new SettlementItemType { Id = 2, SettlementItemTypeName = "InvoiceItem", SettlementItemTypeLocalName = "InvoiceItem" },
                new SettlementItemType { Id = 3, SettlementItemTypeName = "Charge", SettlementItemTypeLocalName = "Charge" }
                );
        }
    }


    public class SettlementStatusConfiguration : IEntityTypeConfiguration<SettlementStatus>
    {
        public void Configure(EntityTypeBuilder<SettlementStatus> builder)
        {
            builder.ToTable("SettlementStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.SettlementStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SettlementStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new SettlementStatus { Id = 1, SettlementStatusName = "Draft", SettlementStatusLocalName = "پیش نویس" },
                new SettlementStatus { Id = 2, SettlementStatusName = "Approved", SettlementStatusLocalName = "تایید شده" },
                new SettlementStatus { Id = 10, SettlementStatusName = "Cancelled", SettlementStatusLocalName = "لغو شده" }
                );
        }
    }



    public class ExchangeRateBaseConfiguration : IEntityTypeConfiguration<ExchangeRateBase>
    {
        public void Configure(EntityTypeBuilder<ExchangeRateBase> builder)
        {
            builder.ToTable("ExchangeRateBases", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ExchangeRateBaseName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new ExchangeRateBase { Id = 10, ExchangeRateBaseName = "Formal-Ports" },
                new ExchangeRateBase { Id = 20, ExchangeRateBaseName = "Free Market" }
                );
        }
    }


    public class InvoiceFlagConfiguration : IEntityTypeConfiguration<InvoiceFlag>
    {
        public void Configure(EntityTypeBuilder<InvoiceFlag> builder)
        {
            builder.ToTable("InvoiceFlags", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InvoiceFlagDescription).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new InvoiceFlag { Id = -1, InvoiceFlagDescription = "Draft" },
                new InvoiceFlag { Id = 1, InvoiceFlagDescription = "In Progress" },
                new InvoiceFlag { Id = 2, InvoiceFlagDescription = "Paid" },
                new InvoiceFlag { Id = 3, InvoiceFlagDescription = "Canceled" },
                new InvoiceFlag { Id = 4, InvoiceFlagDescription = "Converted" }
                );
        }
    }
    public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
    {
        public void Configure(EntityTypeBuilder<InvoiceStatus> builder)
        {
            builder.ToTable("InvoiceStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InvoiceStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.InvoiceStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new InvoiceStatus { Id = -1, InvoiceStatusName = "Draft", InvoiceStatusLocalName = "پیش‌نویس" },
                new InvoiceStatus { Id = 1, InvoiceStatusName = "Pending Approval", InvoiceStatusLocalName = "منتظر تأیید" },
                new InvoiceStatus { Id = 2, InvoiceStatusName = "Approved", InvoiceStatusLocalName = "تأیید شده" },
                new InvoiceStatus { Id = 3, InvoiceStatusName = "Sent", InvoiceStatusLocalName = "ارسال شده" },
                new InvoiceStatus { Id = 4, InvoiceStatusName = "Partially Paid", InvoiceStatusLocalName = "بخشی پرداخت شده" },
                new InvoiceStatus { Id = 5, InvoiceStatusName = "Fully Paid", InvoiceStatusLocalName = "کامل پرداخت شده" },
                new InvoiceStatus { Id = 6, InvoiceStatusName = "Overdue", InvoiceStatusLocalName = "معوقه" },
                new InvoiceStatus { Id = 7, InvoiceStatusName = "Void", InvoiceStatusLocalName = "باطل شده" },
                new InvoiceStatus { Id = 8, InvoiceStatusName = "Cancelled", InvoiceStatusLocalName = "لغو شده" }
                );
        }
    }



    public class ProductTransactionTypeConfiguration : IEntityTypeConfiguration<ProductTransactionType>
    {
        public void Configure(EntityTypeBuilder<ProductTransactionType> builder)
        {
            builder.ToTable("ProductTransactionTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ProductTransactionTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ProductTransactionTypeName).IsRequired().HasMaxLength(100);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);

            builder.HasData(
                new ProductTransactionType { Id = 1, InOutId = 2, ProductTransactionTypeName = "Exit(Based On Request)", ProductTransactionTypeLocalName = "خروج بر اساس درخواست" },
                new ProductTransactionType { Id = 2, InOutId = 1, ProductTransactionTypeName = "Enter(Buy)", ProductTransactionTypeLocalName = "خرید" },
                new ProductTransactionType { Id = 3, InOutId = 1, ProductTransactionTypeName = "Adjust Stock (+)", ProductTransactionTypeLocalName = "تعدیل مثبت موجودی" },
                new ProductTransactionType { Id = 4, InOutId = 2, ProductTransactionTypeName = "Adjust Stock (-)", ProductTransactionTypeLocalName = "تعدیل منفی موجودی" }
                );


        }
    }

    public class AssociationConfiguration : IEntityTypeConfiguration<Association>
    {
        public void Configure(EntityTypeBuilder<Association> builder)
        {
            builder.ToTable("Associations", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AssociationName).IsRequired().HasMaxLength(100);


            builder.HasData(
                new Association { Id = 1, AssociationName = "Chamber Of Commerce(اتاق بازرگانی)" }
                );
        }
    }

    public class AgentTypeConfiguration : IEntityTypeConfiguration<AgentType>
    {
        public void Configure(EntityTypeBuilder<AgentType> builder)
        {
            builder.ToTable("AgentTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AgentTypeName).IsRequired().HasMaxLength(50);
        }

        //Seed data


        //0	GENERAL General Agent
        //1	PORT Port Agent
        //2	BOOKING Booking Agent
        //3	CLEARANCE Clearance Agent
        //4	HUSBANDRY Husbandry Agent
        //5	PROTECTING Protecting Agent
        //6	DELIVERY Delivery Agent



    }

    public class AgentTerritoryConfiguration : IEntityTypeConfiguration<AgentTerritory>
    {
        public void Configure(EntityTypeBuilder<AgentTerritory> builder)
        {
            builder.ToTable("AgentTerritories", "base");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
            builder.HasOne(x => x.Territory).WithMany().HasForeignKey(x => x.TerritoryId);
        }
    }

    public class AgentRoleConfiguration : IEntityTypeConfiguration<AgentRole>
    {
        public void Configure(EntityTypeBuilder<AgentRole> builder)
        {
            builder.ToTable("AgentRoles", "base");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId);
            builder.HasOne(x => x.AgentRoleType).WithMany().HasForeignKey(x => x.AgentRoleTypeId);
        }
    }

    public class DOStatusConfiguration : IEntityTypeConfiguration<DOStatus>
    {
        public void Configure(EntityTypeBuilder<DOStatus> builder)
        {
            builder.ToTable("DOStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DOStatusName).IsRequired().HasMaxLength(35);

            //Seed Data

            builder.HasData(
                new DOStatus { Id = 10, DOStatusName = "Draft" },
                new DOStatus { Id = 20, DOStatusName = "Pending Approval" },
                new DOStatus { Id = 30, DOStatusName = "Approved" },
                new DOStatus { Id = 40, DOStatusName = "Released" },
                new DOStatus { Id = 50, DOStatusName = "Collected" },
                new DOStatus { Id = 99, DOStatusName = "Cancelled" }
                );
        }
    }
    public class HSCodeConfiguration : IEntityTypeConfiguration<HSCode>
    {
        public void Configure(EntityTypeBuilder<HSCode> builder)
        {
            builder.ToTable("HSCodes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.HSCodeCode).IsRequired().HasMaxLength(12);
            builder.Property(x => x.HSCodeName).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.HSLevel).WithMany().HasForeignKey(x => x.HSLevelId);
            builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);

            builder.HasIndex(e => e.HSCodeCode).HasDatabaseName("IX_HSCodes_HSCodeCode").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.HSCodeName).HasDatabaseName("IX_HSCodes_HSCodeName").IsUnique(false).IsClustered(false);

        }
    }

    public class CountryHSCodeConfiguration : IEntityTypeConfiguration<CountryHSCode>
    {
        public void Configure(EntityTypeBuilder<CountryHSCode> builder)
        {
            builder.ToTable("CountryHSCodes", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CHSCode).HasMaxLength(12);
            builder.Property(x => x.CHSName).HasMaxLength(1000);
            builder.Property(x => x.LocalName).HasMaxLength(1000);


            builder.HasOne(x => x.HSCode).WithMany().HasForeignKey(x => x.HSCodeId);
            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);

            builder.HasIndex(e => e.CHSCode).HasDatabaseName("IX_CountryHSCodes_CHSCode").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.CHSName).HasDatabaseName("IX_CountryHSCodes_CHSName").IsUnique(false).IsClustered(false);
            builder.HasIndex(e => e.LocalName).HasDatabaseName("IX_CountryHSCodes_LocalName").IsUnique(false).IsClustered(false);





        }
    }

    public class HSCodeSnapShotConfiguration : IEntityTypeConfiguration<HSCodeSnapshot>
    {
        public void Configure(EntityTypeBuilder<HSCodeSnapshot> builder)
        {
            builder.ToTable("HSCodeSnapshots", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(12);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId);
            //Seed Data

        }
    }



    public class DGConfiguration : IEntityTypeConfiguration<DG>
    {
        public void Configure(EntityTypeBuilder<DG> builder)
        {
            builder.ToTable("DGs", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.DGSubClassNo).IsRequired().HasMaxLength(10);
            builder.Property(x => x.DGUnNumber).IsRequired().HasMaxLength(10);
            builder.Property(x => x.DGName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DGLocalName).IsRequired().HasMaxLength(100);


            builder.HasOne(x => x.DGClass).WithMany().HasForeignKey(x => x.DGClassId);
            builder.HasOne(x => x.DGPackingGroup).WithMany().HasForeignKey(x => x.DGPackingGroupId);
            builder.HasOne(x => x.DGRegulationSet).WithMany().HasForeignKey(x => x.DGRegulationSetId);

            //Seed Data

        }
    }

    public class InfoTypeCategoryConfiguration : IEntityTypeConfiguration<InfoTypeCategory>
    {
        public void Configure(EntityTypeBuilder<InfoTypeCategory> builder)
        {
            builder.ToTable("InfoTypeCategory", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InfoTypeCategoryName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new InfoTypeCategory { Id = 1, InfoTypeCategoryName = "Persons Information" }
                );
        }
    }
    public class InfoTypeConfiguration : IEntityTypeConfiguration<InfoType>
    {
        public void Configure(EntityTypeBuilder<InfoType> builder)
        {
            builder.ToTable("InfoTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InfoTypeName).IsRequired().HasMaxLength(45);
            builder.HasOne(x => x.InfoTypeCategory).WithMany(x => x.InfoTypes).HasForeignKey(x => x.InfoTypeCategoryId);

            //Seed Data
            builder.HasData(
                new InfoType { Id = 1, InfoTypeCategoryId = 1, InfoTypeName = "Mobile Number" },
                new InfoType { Id = 2, InfoTypeCategoryId = 1, InfoTypeName = "Phone Number" },
                new InfoType { Id = 3, InfoTypeCategoryId = 1, InfoTypeName = "Email Address" },
                new InfoType { Id = 4, InfoTypeCategoryId = 1, InfoTypeName = "Fax Number" }
                );

        }
    }
    public class OperationTypeConfiguration : IEntityTypeConfiguration<OperationType>
    {
        public void Configure(EntityTypeBuilder<OperationType> builder)
        {
            builder.ToTable("OperationTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.OperationTypeCode).IsRequired().HasMaxLength(50);
            builder.Property(x => x.OperationTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);

            //Seed Data


        }
    }

    public class QuotationPriorityConfiguration : IEntityTypeConfiguration<QuotationPriority>
    {
        public void Configure(EntityTypeBuilder<QuotationPriority> builder)
        {
            builder.ToTable("QuotationPriority", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.QuotationPriorityName).IsRequired().HasMaxLength(25);

            //Seed Data
            builder.HasData(
                new QuotationPriority { Id = -1, QuotationPriorityName = "Not Specified" },
                new QuotationPriority { Id = 1, QuotationPriorityName = "Best Price" },
                new QuotationPriority { Id = 2, QuotationPriorityName = "Best Time" }
                );
        }
    }

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TagTitle).IsRequired().HasMaxLength(50);
        }
    }
    public class LostReasonCategoryConfiguration : IEntityTypeConfiguration<LostReasonCategory>
    {
        public void Configure(EntityTypeBuilder<LostReasonCategory> builder)
        {
            builder.ToTable("LostReasonCategory", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LostReasonCategoryName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LostReasonCategoryLocalName).IsRequired().HasMaxLength(50);

            //Seed Data

            builder.HasData(
                new LostReasonCategory { Id = 1, LostReasonCategoryName = "Price & Commercial", LostReasonCategoryLocalName = "دلایل مرتبط با قیمت و شرایط مالی" },
                new LostReasonCategory { Id = 2, LostReasonCategoryName = "Competition", LostReasonCategoryLocalName = "از دست رفتن به دلیل انتخاب رقیب" },
                new LostReasonCategory { Id = 3, LostReasonCategoryName = "Service & Operation", LostReasonCategoryLocalName = "مشکلات مرتبط با سرویس یا عملیات" },
                new LostReasonCategory { Id = 4, LostReasonCategoryName = "Customer Decision", LostReasonCategoryLocalName = "دلایل مربوط به تصمیم مشتری" },
                new LostReasonCategory { Id = 5, LostReasonCategoryName = "Documentation & Compliance", LostReasonCategoryLocalName = "مشکلات مربوط به اسناد و قوانین" },
                new LostReasonCategory { Id = 6, LostReasonCategoryName = "Timing & Follow-up", LostReasonCategoryLocalName = "مشکلات در فرآیند فروش" },
                new LostReasonCategory { Id = 7, LostReasonCategoryName = "Capacity & Availability", LostReasonCategoryLocalName = "محدودیت منابع حمل" },
                new LostReasonCategory { Id = 8, LostReasonCategoryName = "Customer Relationship", LostReasonCategoryLocalName = "دلایل مرتبط با ارتباط با مشتری" },
                new LostReasonCategory { Id = 9, LostReasonCategoryName = "Scope/Service Mismatch", LostReasonCategoryLocalName = "عدم تطابق نیاز مشتری با خدمات" },
                new LostReasonCategory { Id = 10, LostReasonCategoryName = "Other/Unknown", LostReasonCategoryLocalName = "دلایل نامشخص" }
                );

        }
    }

    public class LostReasonConfiguration : IEntityTypeConfiguration<LostReason>
    {
        public void Configure(EntityTypeBuilder<LostReason> builder)
        {
            builder.ToTable("LostReasons", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LostReasonName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LostReasonLocalName).IsRequired().HasMaxLength(50);
            builder.HasOne(x => x.LostReasonCategory);
        }
    }

    public class ActivityTypeConfiguration : IEntityTypeConfiguration<ActivityType>
    {
        public void Configure(EntityTypeBuilder<ActivityType> builder)
        {
            builder.ToTable("ActivityTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ActivityTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ActivityTypeLocalName).IsRequired().HasMaxLength(100);

            //Seed Data  

            builder.HasData(
                new ActivityType { Id = 1, ActivityTypeName = "Phone Call", ActivityTypeLocalName = "تماس تلفنی" },
                new ActivityType { Id = 2, ActivityTypeName = "Email", ActivityTypeLocalName = "ایمیل" },
                new ActivityType { Id = 3, ActivityTypeName = "SMS", ActivityTypeLocalName = "پیامک" },
                new ActivityType { Id = 4, ActivityTypeName = "In-Person Meeting", ActivityTypeLocalName = "جلسه حضوری" },
                new ActivityType { Id = 5, ActivityTypeName = "Online Meeting", ActivityTypeLocalName = "جلسه آنلاین" },
                new ActivityType { Id = 6, ActivityTypeName = "Follow-Up", ActivityTypeLocalName = "پیگیری" },
                new ActivityType { Id = 7, ActivityTypeName = "Proposal Sent", ActivityTypeLocalName = "ارسال پیشنهاد" },
                new ActivityType { Id = 8, ActivityTypeName = "Product Demo", ActivityTypeLocalName = "دموی محصول" },
                new ActivityType { Id = 9, ActivityTypeName = "Consultation ", ActivityTypeLocalName = "مشاوره" }
                );
        }
    }
    public class ActivityResultConfiguration : IEntityTypeConfiguration<ActivityResult>
    {
        public void Configure(EntityTypeBuilder<ActivityResult> builder)
        {
            builder.ToTable("ActivityResults", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ActivityResultName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ActivityResultLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  

            builder.HasData(
                new ActivityResult { Id = 1, ActivityResultName = "Answered", ActivityResultLocalName = "پاسخ داده شد" },
                new ActivityResult { Id = 2, ActivityResultName = "No Answer", ActivityResultLocalName = "بی پاسخ" },
                new ActivityResult { Id = 3, ActivityResultName = "Busy", ActivityResultLocalName = "مشغول" },
                new ActivityResult { Id = 4, ActivityResultName = "Call Dropped", ActivityResultLocalName = "خط قطع شد" },
                new ActivityResult { Id = 5, ActivityResultName = "Email Opened", ActivityResultLocalName = "ایمیل باز شد" },
                new ActivityResult { Id = 6, ActivityResultName = "Email Clicked", ActivityResultLocalName = "ایمیل کلیک شد" },
                new ActivityResult { Id = 7, ActivityResultName = "Meeting Held", ActivityResultLocalName = "جلسه برگزار شد" },
                new ActivityResult { Id = 8, ActivityResultName = "Meeting Cancelled", ActivityResultLocalName = "جلسه لغو شد" },
                new ActivityResult { Id = 9, ActivityResultName = "Postponed ", ActivityResultLocalName = "به تعویق افتاد" }
                );
        }
    }
    public class ActivityOutcomeConfiguration : IEntityTypeConfiguration<ActivityOutcome>
    {
        public void Configure(EntityTypeBuilder<ActivityOutcome> builder)
        {
            builder.ToTable("ActivityOutcomes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ActivityOutcomeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ActivityOutcomeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new ActivityOutcome { Id = 1, ActivityOutcomeName = "Interested", ActivityOutcomeLocalName = "علاقه مند" },
                new ActivityOutcome { Id = 2, ActivityOutcomeName = "Needs Follow-Up", ActivityOutcomeLocalName = "نیاز به پیگیری" },
                new ActivityOutcome { Id = 3, ActivityOutcomeName = "Ready to Buy", ActivityOutcomeLocalName = "آماده خرید" },
                new ActivityOutcome { Id = 4, ActivityOutcomeName = "Not Interested", ActivityOutcomeLocalName = "عدم علاقه" },
                new ActivityOutcome { Id = 5, ActivityOutcomeName = "No Budget", ActivityOutcomeLocalName = "بودجه ندارد" },
                new ActivityOutcome { Id = 6, ActivityOutcomeName = "Bad Timing", ActivityOutcomeLocalName = "زمان بندی نامناسب" },
                new ActivityOutcome { Id = 7, ActivityOutcomeName = "Competitor Chosen", ActivityOutcomeLocalName = "رقیب انتخاب شد" },
                new ActivityOutcome { Id = 80, ActivityOutcomeName = "Converted to Opportunity", ActivityOutcomeLocalName = "تبدیل به فرصت فروش" },
                new ActivityOutcome { Id = 90, ActivityOutcomeName = "Lost", ActivityOutcomeLocalName = "از دست رفته" }
                );
        }

    }

    public class ContractTypeConfiguration : IEntityTypeConfiguration<ContractType>
    {
        public void Configure(EntityTypeBuilder<ContractType> builder)
        {
            builder.ToTable("ContractTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ContractTypeName).IsRequired().HasMaxLength(50);

            //Seed Data  
        }

    }

    public class BLReleaseTypeConfiguration : IEntityTypeConfiguration<BLReleaseType>
    {
        public void Configure(EntityTypeBuilder<BLReleaseType> builder)
        {
            builder.ToTable("BLReleaseTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.BLReleaseTypeCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.BLReleaseTypeName).IsRequired().HasMaxLength(40);
            builder.Property(x => x.BLReleaseTypeDescription).IsRequired().HasMaxLength(100);

            //Seed Data  

            builder.HasData(
                new BLReleaseType { Id = 1, BLReleaseTypeCode = "ORIGINAL", BLReleaseTypeName = "Original BL" },
                new BLReleaseType { Id = 2, BLReleaseTypeCode = "TELEX", BLReleaseTypeName = "Telex Release" },
                new BLReleaseType { Id = 3, BLReleaseTypeCode = "SURRENDER", BLReleaseTypeName = "Surrendered" },
                new BLReleaseType { Id = 4, BLReleaseTypeCode = "SEAWAYBILL", BLReleaseTypeName = "Sea Waybill" },
                new BLReleaseType { Id = 5, BLReleaseTypeCode = "EXPRESS", BLReleaseTypeName = "Express Release" },
                new BLReleaseType { Id = 6, BLReleaseTypeCode = "ELECTRONIC", BLReleaseTypeName = "Electronic" }
                );

        }

    }

    public class ENSDeclarationStatusConfiguration : IEntityTypeConfiguration<ENSDeclarationStatus>
    {
        public void Configure(EntityTypeBuilder<ENSDeclarationStatus> builder)
        {
            builder.ToTable("ENSDeclarationStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ENSDeclarationStatusName).IsRequired().HasMaxLength(15);
            //Seed Data  
            builder.HasData(
                new ENSDeclarationStatus { Id = -1, ENSDeclarationStatusName = "Draft" },
                new ENSDeclarationStatus { Id = 1, ENSDeclarationStatusName = "Submitted" },
                new ENSDeclarationStatus { Id = 2, ENSDeclarationStatusName = "Accepted" },
                new ENSDeclarationStatus { Id = 3, ENSDeclarationStatusName = "Rejected" }
                );
        }

    }

    public class CargoHandlingTypeConfiguration : IEntityTypeConfiguration<CargoHandlingType>
    {
        public void Configure(EntityTypeBuilder<CargoHandlingType> builder)
        {
            builder.ToTable("CargoHandlingTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CargoHandlingTypeName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new CargoHandlingType { Id = 1, CargoHandlingTypeName = "Containerized" },
                new CargoHandlingType { Id = 2, CargoHandlingTypeName = "Break Bulk" },
                new CargoHandlingType { Id = 3, CargoHandlingTypeName = "Bulk" },
                new CargoHandlingType { Id = 4, CargoHandlingTypeName = "Project" }
                );
        }
    }

    public class CargoNatureConfiguration : IEntityTypeConfiguration<CargoNature>
    {
        public void Configure(EntityTypeBuilder<CargoNature> builder)
        {
            builder.ToTable("CargoNatures", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CargoNatureName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new CargoNature { Id = 1, CargoNatureName = "General Cargo" },
                new CargoNature { Id = 2, CargoNatureName = "Vehicle" },
                new CargoNature { Id = 3, CargoNatureName = "Machinery" },
                new CargoNature { Id = 4, CargoNatureName = "Hazardous" },
                new CargoNature { Id = 5, CargoNatureName = "RoRo" },
                new CargoNature { Id = 6, CargoNatureName = "Liquid Bulk" },
                new CargoNature { Id = 7, CargoNatureName = "Reefer/Termeprature" },
                new CargoNature { Id = 8, CargoNatureName = "Out Of Gauge" },
                new CargoNature { Id = 9, CargoNatureName = "Out Of Gauge" },
                new CargoNature { Id = 10, CargoNatureName = "Live Animals" },
                new CargoNature { Id = 11, CargoNatureName = "Perishable" },
                new CargoNature { Id = 12, CargoNatureName = "Personal Effects" }
                );
        }
    }
    public class TransportTypeLoadTypeConfiguration : IEntityTypeConfiguration<TransportTypeLoadType>
    {
        public void Configure(EntityTypeBuilder<TransportTypeLoadType> builder)
        {
            builder.ToTable("TransportTypeLoadTypes", "base");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.LoadType).WithMany(x => x.TransportTypeLoadTypes).HasForeignKey(x => x.LoadTypeId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);

            // Seed Data
            builder.HasData(
                new TransportTypeLoadType { Id = 1, TransportTypeId = 1, LoadTypeId = 1 },
                new TransportTypeLoadType { Id = 2, TransportTypeId = 1, LoadTypeId = 2 },
                new TransportTypeLoadType { Id = 3, TransportTypeId = 1, LoadTypeId = 3 },
                new TransportTypeLoadType { Id = 4, TransportTypeId = 1, LoadTypeId = 4 },
                new TransportTypeLoadType { Id = 5, TransportTypeId = 2, LoadTypeId = 1 },
                new TransportTypeLoadType { Id = 6, TransportTypeId = 2, LoadTypeId = 5 },
                new TransportTypeLoadType { Id = 7, TransportTypeId = 2, LoadTypeId = 6 },
                new TransportTypeLoadType { Id = 8, TransportTypeId = 3, LoadTypeId = 1 },
                new TransportTypeLoadType { Id = 9, TransportTypeId = 4, LoadTypeId = 7 },
                new TransportTypeLoadType { Id = 10, TransportTypeId = 2, LoadTypeId = 4 }
            );
        }
    }

    public class CalculationByConfiguration : IEntityTypeConfiguration<CalculationBy>
    {
        public void Configure(EntityTypeBuilder<CalculationBy> builder)
        {
            builder.ToTable("CalculationBy", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CalculationByName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new CalculationBy { Id = 1, CalculationByName = "Per Container SizeType" },
                new CalculationBy { Id = 2, CalculationByName = "Per Container Size" },
                new CalculationBy { Id = 3, CalculationByName = "Per Container" },
                new CalculationBy { Id = 4, CalculationByName = "Per B/L" },
                new CalculationBy { Id = 5, CalculationByName = "Per Set" },
                new CalculationBy { Id = 6, CalculationByName = "Per Shipment" },
                new CalculationBy { Id = 7, CalculationByName = "Per KGS" },
                new CalculationBy { Id = 8, CalculationByName = "Per CBM" },
                new CalculationBy { Id = 9, CalculationByName = "Per TEU" },
                new CalculationBy { Id = 10, CalculationByName = "Per Day" },
                new CalculationBy { Id = 11, CalculationByName = "Per TON" },
                new CalculationBy { Id = 12, CalculationByName = "Percentage" },
                new CalculationBy { Id = 13, CalculationByName = "Flat/Lump Sum" }
                );
        }
    }

    public class CalculationTypeConfiguration : IEntityTypeConfiguration<CalculationType>
    {
        public void Configure(EntityTypeBuilder<CalculationType> builder)
        {
            builder.ToTable("CalculationTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CalculationTypeName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new CalculationType { Id = 1, CalculationTypeName = "Flat" },
                new CalculationType { Id = 2, CalculationTypeName = "Accumulative/Progressive" },
                new CalculationType { Id = 3, CalculationTypeName = "Single Tier/Best Match" }
                );
        }
    }

    public class RateRequestStatusConfiguration : IEntityTypeConfiguration<RateRequestStatus>
    {
        public void Configure(EntityTypeBuilder<RateRequestStatus> builder)
        {
            builder.ToTable("RateRequestStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.RateRequestStatusName).IsRequired().HasMaxLength(70);
            //Seed Data  
            builder.HasData(
                new RateRequestStatus { Id = 1, RateRequestStatusName = "Init By Customer" },
                new RateRequestStatus { Id = 2, RateRequestStatusName = "Sent By Customer" },
                new RateRequestStatus { Id = 3, RateRequestStatusName = "Sent To Sales Manager" },
                new RateRequestStatus { Id = 4, RateRequestStatusName = "Sent To Salesman" },
                new RateRequestStatus { Id = 5, RateRequestStatusName = "Sent To Agents" },
                new RateRequestStatus { Id = 6, RateRequestStatusName = "Rate Sent To Customer" },
                new RateRequestStatus { Id = 7, RateRequestStatusName = "Sales Manager Rejected" },
                new RateRequestStatus { Id = 8, RateRequestStatusName = "Salesman Rejected" },
                new RateRequestStatus { Id = 9, RateRequestStatusName = "Customer Rejected" },
                new RateRequestStatus { Id = 10, RateRequestStatusName = "Customer Accepted" },
                new RateRequestStatus { Id = 11, RateRequestStatusName = "Converted to Job" }
                );
        }

        //InitByCustomer = 1,
        //SentByCustomer = 2,
        //SentToSalesManager = 3,
        //SentToSalesman = 4,
        //SentToAgents = 5,
        //RateToCustomer = 6,
        //SalesManagerRejected = 7,
        //SalesmanRejected = 8,
        //CustomerRejected = 9,
        //CustomerAccepted = 10,
        //Booked = 11
    }



    public class AgentScopeConfiguration : IEntityTypeConfiguration<AgentScope>
    {
        public void Configure(EntityTypeBuilder<AgentScope> builder)
        {
            builder.ToTable("AgentScopes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AgentScopeCode).IsRequired().HasMaxLength(15);
            builder.Property(x => x.AgentScopeName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new AgentScope { Id = 1, AgentScopeCode = "ALL", AgentScopeName = "All Services" },
                new AgentScope { Id = 2, AgentScopeCode = "SEA", AgentScopeName = "Sea Freight" },
                new AgentScope { Id = 3, AgentScopeCode = "AIR", AgentScopeName = "Air Freight" },
                new AgentScope { Id = 4, AgentScopeCode = "LAND", AgentScopeName = "Land Transport" },
                new AgentScope { Id = 5, AgentScopeCode = "CUSTOMS", AgentScopeName = "Customs" },
                new AgentScope { Id = 6, AgentScopeCode = "ROUTE", AgentScopeName = "Specific Route" }
                );
        }
    }

    public class BookingStatusConfiguration : IEntityTypeConfiguration<BookingStatus>
    {
        public void Configure(EntityTypeBuilder<BookingStatus> builder)
        {
            builder.ToTable("BookingStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.BookingStatusCode).IsRequired().HasMaxLength(15);
            builder.Property(x => x.BookingStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.BookingStatusLocalName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new BookingStatus { Id = -1, BookingStatusCode = "DRAFT", BookingStatusName = "Draft", BookingStatusLocalName = "پیش نویس" },
                new BookingStatus { Id = 1, BookingStatusCode = "REQUESTED", BookingStatusName = "Requested", BookingStatusLocalName = "درخواست شده" },
                new BookingStatus { Id = 2, BookingStatusCode = "PENDING", BookingStatusName = "Pending Carrier", BookingStatusLocalName = "منتظر حامل" },
                new BookingStatus { Id = 3, BookingStatusCode = "CONFIRMED", BookingStatusName = "Confirmed", BookingStatusLocalName = "تأیید شده" },
                new BookingStatus { Id = 4, BookingStatusCode = "AMENDED", BookingStatusName = "Amended", BookingStatusLocalName = "اصلاح شده" },
                new BookingStatus { Id = 5, BookingStatusCode = "WAITLIST", BookingStatusName = "Waitlisted", BookingStatusLocalName = "لیست انتظار" },
                new BookingStatus { Id = 6, BookingStatusCode = "REJECTED", BookingStatusName = "Rejected", BookingStatusLocalName = "رد شده" },
                new BookingStatus { Id = 7, BookingStatusCode = "CANCELLED", BookingStatusName = "Cancelled", BookingStatusLocalName = "لغو شده" },
                new BookingStatus { Id = 8, BookingStatusCode = "COMPLETED", BookingStatusName = "Completed", BookingStatusLocalName = "تکمیل شده" },
                new BookingStatus { Id = 9, BookingStatusCode = "EXPIRED", BookingStatusName = "EXPIRED", BookingStatusLocalName = "منقضی شده" }
                );
        }
    }


    public class ClaimTypeConfiguration : IEntityTypeConfiguration<ClaimType>
    {
        public void Configure(EntityTypeBuilder<ClaimType> builder)
        {
            builder.ToTable("ClaimTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ClaimTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ClaimTypeLocalName).IsRequired().HasMaxLength(100);

            //Seed Data  
            builder.HasData(
                new ClaimType { Id = 1, ClaimTypeName = "Cargo Damage", ClaimTypeLocalName = "آسیب بار" },
                new ClaimType { Id = 2, ClaimTypeName = "Shortage", ClaimTypeLocalName = "کسری" },
                new ClaimType { Id = 3, ClaimTypeName = "Total Loss", ClaimTypeLocalName = "مفقودی کامل" },
                new ClaimType { Id = 4, ClaimTypeName = "Delay", ClaimTypeLocalName = "تأخیر" },
                new ClaimType { Id = 5, ClaimTypeName = "Overcharge", ClaimTypeLocalName = "هزینه اضافی" },
                new ClaimType { Id = 6, ClaimTypeName = "Misdelivery", ClaimTypeLocalName = "تحویل اشتباه" },
                new ClaimType { Id = 7, ClaimTypeName = "Contamination", ClaimTypeLocalName = "آلودگی" },
                new ClaimType { Id = 8, ClaimTypeName = "Theft / Pilferage", ClaimTypeLocalName = "سرقت" }
                );
        }
    }

    public class ClaimStatusConfiguration : IEntityTypeConfiguration<ClaimStatus>
    {
        public void Configure(EntityTypeBuilder<ClaimStatus> builder)
        {
            builder.ToTable("ClaimStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ClaimStatusName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ClaimStatusLocalName).IsRequired().HasMaxLength(100);

            //Seed Data  
            builder.HasData(
                new ClaimStatus { Id = 1, ClaimStatusName = "Draft", ClaimStatusLocalName = "پیش نویس" },
                new ClaimStatus { Id = 2, ClaimStatusName = "FILED", ClaimStatusLocalName = "ثبت شده" },
                new ClaimStatus { Id = 3, ClaimStatusName = "Under Investigation", ClaimStatusLocalName = "در حال بررسی" },
                new ClaimStatus { Id = 4, ClaimStatusName = "Approved", ClaimStatusLocalName = "تأیید شده" },
                new ClaimStatus { Id = 5, ClaimStatusName = "Partially Settled", ClaimStatusLocalName = "تسویه جزئی" },
                new ClaimStatus { Id = 6, ClaimStatusName = "Fully Settled", ClaimStatusLocalName = "تسویه کامل" },
                new ClaimStatus { Id = 7, ClaimStatusName = "Rejected", ClaimStatusLocalName = "رد شده" },
                new ClaimStatus { Id = 8, ClaimStatusName = "Closed", ClaimStatusLocalName = "بسته شده" }
                );
        }
    }

    public class CustomsDeclarationTypeConfiguration : IEntityTypeConfiguration<CustomsDeclarationType>
    {
        public void Configure(EntityTypeBuilder<CustomsDeclarationType> builder)
        {
            builder.ToTable("CustomsDeclarationTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CustomsDeclarationTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CustomsDeclarationTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new CustomsDeclarationType { Id = 1, CustomsDeclarationTypeName = "Import Declaration", CustomsDeclarationTypeLocalName = "اظهارنامه واردات" },
                new CustomsDeclarationType { Id = 2, CustomsDeclarationTypeName = "Export Declaration", CustomsDeclarationTypeLocalName = "اظهارنامه صادرات" },
                new CustomsDeclarationType { Id = 3, CustomsDeclarationTypeName = "Transit Declaration", CustomsDeclarationTypeLocalName = "اظهارنامه ترانزیت" },
                new CustomsDeclarationType { Id = 4, CustomsDeclarationTypeName = "Temporary Admission", CustomsDeclarationTypeLocalName = "ورود موقت" },
                new CustomsDeclarationType { Id = 5, CustomsDeclarationTypeName = "Re-Export", CustomsDeclarationTypeLocalName = "صادرات مجدد" },
                new CustomsDeclarationType { Id = 6, CustomsDeclarationTypeName = "Warehouse Entry", CustomsDeclarationTypeLocalName = "ورود به انبار" },
                new CustomsDeclarationType { Id = 7, CustomsDeclarationTypeName = "Free Zone Entry", CustomsDeclarationTypeLocalName = "ورود منطقه آزاد" },
                new CustomsDeclarationType { Id = 8, CustomsDeclarationTypeName = "Cabotage", CustomsDeclarationTypeLocalName = "کابوتاژ" }
                );
        }
    }


    public class CustomsHoldReasonConfiguration : IEntityTypeConfiguration<CustomsHoldReason>
    {
        public void Configure(EntityTypeBuilder<CustomsHoldReason> builder)
        {
            builder.ToTable("CustomsHoldReasons", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.HoldReasonName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.HoldReasonLocalName).IsRequired().HasMaxLength(100);

            //Seed Data  
            builder.HasData(
                new CustomsHoldReason { Id = 1, HoldReasonName = "License", HoldReasonLocalName = "نداشتن مجوز" }
                );
        }
    }





    public class CustomsStatusConfiguration : IEntityTypeConfiguration<CustomsStatus>
    {
        public void Configure(EntityTypeBuilder<CustomsStatus> builder)
        {
            builder.ToTable("CustomsStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CustomsStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CustomsStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new CustomsStatus { Id = -1, CustomsStatusName = "Draft", CustomsStatusLocalName = "پیش‌نویس" },
                new CustomsStatus { Id = 1, CustomsStatusName = "Submitted", CustomsStatusLocalName = "ارائه شده" },
                new CustomsStatus { Id = 2, CustomsStatusName = "Under Review", CustomsStatusLocalName = "در حال بررسی" },
                new CustomsStatus { Id = 3, CustomsStatusName = "Assessed", CustomsStatusLocalName = "ارزیابی شده" },
                new CustomsStatus { Id = 4, CustomsStatusName = "Duty Paid", CustomsStatusLocalName = "حقوق پرداخت شده" },
                new CustomsStatus { Id = 5, CustomsStatusName = "Under Examination", CustomsStatusLocalName = "در حال ارزیابی فیزیکی" },
                new CustomsStatus { Id = 6, CustomsStatusName = "Released", CustomsStatusLocalName = "ترخیص شده" },
                new CustomsStatus { Id = 7, CustomsStatusName = "Held / Suspended", CustomsStatusLocalName = "توقیف / معلق" },
                new CustomsStatus { Id = 8, CustomsStatusName = "Cancelled", CustomsStatusLocalName = "لغو شده" },
                new CustomsStatus { Id = 9, CustomsStatusName = "Rejected", CustomsStatusLocalName = "رد شده" },
                new CustomsStatus { Id = 10, CustomsStatusName = "Exit Confirmed", CustomsStatusLocalName = "تایید خروج" }
                );
        }
    }

    public class EDIMessageTypeConfiguration : IEntityTypeConfiguration<EDIMessageType>
    {
        public void Configure(EntityTypeBuilder<EDIMessageType> builder)
        {
            builder.ToTable("EDIMessageTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.EDIMessageTypeCode).IsRequired().HasMaxLength(15);
            builder.Property(x => x.EDIMessageTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EDIMessageTypeStandard).IsRequired().HasMaxLength(15);

            //Seed Data  
            builder.HasData(
                new EDIMessageType { Id = 1, EDIMessageTypeCode = "IFTMIN", EDIMessageTypeName = "Booking Instruction", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 2, EDIMessageTypeCode = "IFTMBF", EDIMessageTypeName = "Booking Confirmation", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 3, EDIMessageTypeCode = "IFTMCS", EDIMessageTypeName = "Shipment Status", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 4, EDIMessageTypeCode = "COPARN", EDIMessageTypeName = "Container Pre-Announcement", EDIMessageTypeStandard = "EDIFACT", IsActive = true },
                new EDIMessageType { Id = 5, EDIMessageTypeCode = "BAPLIE", EDIMessageTypeName = "Bay Plan", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 6, EDIMessageTypeCode = "CUSCAR", EDIMessageTypeName = "Customs Cargo Report", EDIMessageTypeStandard = "EDIFACT", IsActive = true },
                new EDIMessageType { Id = 7, EDIMessageTypeCode = "CUSDEC", EDIMessageTypeName = "Customs Declaration", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 8, EDIMessageTypeCode = "INVOIC", EDIMessageTypeName = "Invoice", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 9, EDIMessageTypeCode = "DESADV", EDIMessageTypeName = "Dispatch Advice", EDIMessageTypeStandard = "EDIFACT", IsActive = false },
                new EDIMessageType { Id = 10, EDIMessageTypeCode = "BOOKING", EDIMessageTypeName = "Booking Request(DCSA/ JSON)", EDIMessageTypeStandard = "DCSA", IsActive = false },
                new EDIMessageType { Id = 11, EDIMessageTypeCode = "TRACK", EDIMessageTypeName = "Track & Trace(DCSA / JSON)", EDIMessageTypeStandard = "DCSA", IsActive = false },
                new EDIMessageType { Id = 12, EDIMessageTypeCode = "SI", EDIMessageTypeName = "Shipping Instructions(DCSA)", EDIMessageTypeStandard = "DCSA", IsActive = false },
                new EDIMessageType { Id = 13, EDIMessageTypeCode = "TD", EDIMessageTypeName = "Transport Document(DCSA)", EDIMessageTypeStandard = "DCSA", IsActive = false },
                new EDIMessageType { Id = 14, EDIMessageTypeCode = "FWB", EDIMessageTypeName = "Air Waybill(Cargo-IMP)", EDIMessageTypeStandard = "IATA", IsActive = false },
                new EDIMessageType { Id = 15, EDIMessageTypeCode = "FHL", EDIMessageTypeName = "House Waybill(Cargo-IMP)", EDIMessageTypeStandard = "IATA", IsActive = false },
                new EDIMessageType { Id = 16, EDIMessageTypeCode = "FSU", EDIMessageTypeName = "Flight Status Update", EDIMessageTypeStandard = "IATA", IsActive = false },
                new EDIMessageType { Id = 17, EDIMessageTypeCode = "FFM", EDIMessageTypeName = "Flight Manifest ", EDIMessageTypeStandard = "IATA", IsActive = false },
                new EDIMessageType { Id = 18, EDIMessageTypeCode = "EPL", EDIMessageTypeName = "Electronic Packing List", EDIMessageTypeStandard = "EDIFACT", IsActive = true },
                new EDIMessageType { Id = 19, EDIMessageTypeCode = "MyTCTS", EDIMessageTypeName = "Export Custom File", EDIMessageTypeStandard = "EDIFACT", IsActive = true }
                );
        }
    }

    public class JobLinkTypeConfiguration : IEntityTypeConfiguration<JobLinkType>
    {
        public void Configure(EntityTypeBuilder<JobLinkType> builder)
        {
            builder.ToTable("JobLinkTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.JobLinkTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.JobLinkTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new JobLinkType { Id = 1, JobLinkTypeName = "Master/House", JobLinkTypeLocalName = "مستر/هاوس" },
                new JobLinkType { Id = 2, JobLinkTypeName = "Related", JobLinkTypeLocalName = "مرتبط" },
                new JobLinkType { Id = 3, JobLinkTypeName = "Split", JobLinkTypeLocalName = "تقسیم" },
                new JobLinkType { Id = 4, JobLinkTypeName = "Merged", JobLinkTypeLocalName = "تجمیع " }
                );
        }
    }
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("ServiceTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ServiceTypeCode).HasMaxLength(50);
            builder.Property(x => x.ServiceTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ServiceTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new ServiceType { Id = 1, ServiceTypeName = "Sea Freight(FCL)", ServiceTypeLocalName = "حمل‌ونقل دریاییFCL", SortOrder = 1 },
                new ServiceType { Id = 2, ServiceTypeName = "Sea Freight(LCL)", ServiceTypeLocalName = " حمل‌ونقل دریایی LCL", SortOrder = 2 },
                new ServiceType { Id = 3, ServiceTypeName = "Air Freight", ServiceTypeLocalName = "حمل‌ونقل هوایی", SortOrder = 3 },
                new ServiceType { Id = 4, ServiceTypeName = "Road Freight(FTL)", ServiceTypeLocalName = "حمل‌ونقل جاده‌ای FTL", SortOrder = 4 },
                new ServiceType { Id = 5, ServiceTypeName = "Road Freight(LTL)", ServiceTypeLocalName = "حمل‌ونقل جاده‌ای LTL", SortOrder = 5 },
                new ServiceType { Id = 6, ServiceTypeName = "Rail Freight", ServiceTypeLocalName = "حمل‌ونقل ریلی", SortOrder = 6 },
                new ServiceType { Id = 7, ServiceTypeName = "Multimodal", ServiceTypeLocalName = "حمل‌ونقل چندوجهی", SortOrder = 7 },
                new ServiceType { Id = 8, ServiceTypeName = "Customs Clearance", ServiceTypeLocalName = "ترخیص گمرکی", SortOrder = 8 },
                new ServiceType { Id = 9, ServiceTypeName = "Warehousing", ServiceTypeLocalName = "انبارداری", SortOrder = 9 },
                new ServiceType { Id = 10, ServiceTypeName = "Distribution", ServiceTypeLocalName = "توزیع", SortOrder = 10 },
                new ServiceType { Id = 11, ServiceTypeName = "Trucking", ServiceTypeLocalName = "خدمات کامیونی / حمل زمینی", SortOrder = 11 },
                new ServiceType { Id = 12, ServiceTypeName = "Insurance", ServiceTypeLocalName = "بیمه", SortOrder = 12 },
                new ServiceType { Id = 13, ServiceTypeName = "Documentation", ServiceTypeLocalName = "خدمات اسنادی", SortOrder = 13 },
                new ServiceType { Id = 14, ServiceTypeName = "Project Cargo", ServiceTypeLocalName = "محمولات پروژه‌ای", SortOrder = 14 },
                new ServiceType { Id = 15, ServiceTypeName = "Courier", ServiceTypeLocalName = "پیک / کوریر", SortOrder = 15 }
                );
        }
    }

    public class JobStageConfiguration : IEntityTypeConfiguration<JobStage>
    {
        public void Configure(EntityTypeBuilder<JobStage> builder)
        {
            builder.ToTable("JobStages", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.JobStageName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.JobStageLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new JobStage { Id = 1, JobStageName = "INQUIRY", JobStageLocalName = "استعلام", SortOrder = 1 },
                new JobStage { Id = 2, JobStageName = "QUOTATION", JobStageLocalName = "نرخ‌دهی", SortOrder = 2 },
                new JobStage { Id = 3, JobStageName = "BOOKING", JobStageLocalName = "رزرو", SortOrder = 3 },
                new JobStage { Id = 4, JobStageName = "DOCUMENTATION", JobStageLocalName = "آماده‌سازی اسناد", SortOrder = 4 },
                new JobStage { Id = 5, JobStageName = "PICKUP", JobStageLocalName = "دریافت بار", SortOrder = 5 },
                new JobStage { Id = 6, JobStageName = "ORIGIN_CUSTOMS", JobStageLocalName = "تشریفات/ترخیص گمرکی مبدا", SortOrder = 6 },
                new JobStage { Id = 7, JobStageName = "ORIGIN_PORT", JobStageLocalName = "عملیات بندری/ترمینالی مبدا", SortOrder = 7 },
                new JobStage { Id = 8, JobStageName = "DEPARTURE", JobStageLocalName = "خروج / حرکت", SortOrder = 8 },
                new JobStage { Id = 9, JobStageName = "TRANSIT", JobStageLocalName = "در مسیر / ترانزیت", SortOrder = 9 },
                new JobStage { Id = 10, JobStageName = "DESTINATION_PORT", JobStageLocalName = "عملیات بندری/ترمینالی مقصد", SortOrder = 10 },
                new JobStage { Id = 11, JobStageName = "DESTINATION_CUSTOMS", JobStageLocalName = "ترخیص گمرکی مقصد", SortOrder = 11 },
                new JobStage { Id = 12, JobStageName = "WAREHOUSING", JobStageLocalName = "انبارداری", SortOrder = 12 },
                new JobStage { Id = 13, JobStageName = "DELIVERY", JobStageLocalName = "تحویل", SortOrder = 13 },
                new JobStage { Id = 14, JobStageName = "INVOICING", JobStageLocalName = "صورتحساب", SortOrder = 14 },
                new JobStage { Id = 15, JobStageName = "SETTLEMENT", JobStageLocalName = "تسویه", SortOrder = 15 },
                new JobStage { Id = 16, JobStageName = "COMPLETION", JobStageLocalName = "اتمام عملیات", SortOrder = 16 }
                );
        }
    }





    public class JobStatusConfiguration : IEntityTypeConfiguration<JobStatus>
    {
        public void Configure(EntityTypeBuilder<JobStatus> builder)
        {
            builder.ToTable("JobStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.JobStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.JobStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            builder.HasData(
                new JobStatus { Id = 1, JobStatusName = "Draft", JobStatusLocalName = "درافت", SortOrder = 1 },
                new JobStatus { Id = 2, JobStatusName = "Confirmed", JobStatusLocalName = "تایید شده", SortOrder = 2 },
                new JobStatus { Id = 3, JobStatusName = "In Progress", JobStatusLocalName = "در جریان", SortOrder = 3 },
                new JobStatus { Id = 4, JobStatusName = "Completed", JobStatusLocalName = "تکمیل شده", SortOrder = 4 },
                new JobStatus { Id = 5, JobStatusName = "Closed", JobStatusLocalName = "بسته شده", SortOrder = 5 },
                new JobStatus { Id = 6, JobStatusName = "Canceled", JobStatusLocalName = "لغو شده", SortOrder = 6 },
                new JobStatus { Id = 7, JobStatusName = "On Hold", JobStatusLocalName = "متوقف/معلق", SortOrder = 7 }
                );
        }
    }

    public class JobTypeConfiguration : IEntityTypeConfiguration<JobType>
    {
        public void Configure(EntityTypeBuilder<JobType> builder)
        {
            builder.ToTable("JobTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.JobTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.JobTypeLocalName).IsRequired().HasMaxLength(50);
            //Seed Data  
            builder.HasData(
                new JobType { Id = 1, JobTypeName = "Import", JobTypeLocalName = "واردات", SortOrder = 1 },
                new JobType { Id = 2, JobTypeName = "Export", JobTypeLocalName = "صادرات", SortOrder = 2 },
                new JobType { Id = 3, JobTypeName = "Domestic", JobTypeLocalName = "داخلی", SortOrder = 3 },
                new JobType { Id = 4, JobTypeName = "Transit", JobTypeLocalName = "ترانزیت", SortOrder = 4 },
                new JobType { Id = 5, JobTypeName = "Cross Trade", JobTypeLocalName = "کراس‌ترید", SortOrder = 5 },
                new JobType { Id = 6, JobTypeName = "ImportReExport", JobTypeLocalName = "واردات و صادرات مجدد", SortOrder = 6 },
                new JobType { Id = 7, JobTypeName = "Return", JobTypeLocalName = "مرجوعی", SortOrder = 7 }
                );
        }
    }

    public class OpportunityStageConfiguration : IEntityTypeConfiguration<OpportunityStage>
    {
        public void Configure(EntityTypeBuilder<OpportunityStage> builder)
        {
            builder.ToTable("OpportunityStages", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.OpportunityStageName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.OpportunityStageLocalName).IsRequired().HasMaxLength(50);

            //Seed Data  
            //builder.HasData(
            //    );
        }
    }

    public class CommentTypeConfiguration : IEntityTypeConfiguration<CommentType>
    {
        public void Configure(EntityTypeBuilder<CommentType> builder)
        {
            builder.ToTable("CommentTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CommentTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CommentTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new CommentType { Id = 1, CommentTypeName = "Note" },
                new CommentType { Id = 2, CommentTypeName = "Question" },
                new CommentType { Id = 3, CommentTypeName = "Answer" },
                new CommentType { Id = 4, CommentTypeName = "Issue" },
                new CommentType { Id = 5, CommentTypeName = "Resolution" },
                new CommentType { Id = 6, CommentTypeName = "Internal" }
                );
        }
    }

    public class TaskCategoryConfiguration : IEntityTypeConfiguration<TaskCategory>
    {
        public void Configure(EntityTypeBuilder<TaskCategory> builder)
        {
            builder.ToTable("TaskCategory", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TaskCategoryName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TaskCategoryLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new TaskCategory { Id = 1, TaskCategoryName = "Documentation" },
                new TaskCategory { Id = 2, TaskCategoryName = "Operational" },
                new TaskCategory { Id = 3, TaskCategoryName = "Communication" },
                new TaskCategory { Id = 4, TaskCategoryName = "Financial" },
                new TaskCategory { Id = 5, TaskCategoryName = "Customs" },
                new TaskCategory { Id = 6, TaskCategoryName = "Administrative" }
                );
        }
    }

    public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Operations.TasksManage.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Operations.TasksManage.Task> builder)
        {
            builder.ToTable("Tasks", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TaskName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.TaskLocalName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.DefaultPriority).WithMany().HasForeignKey(x => x.DefaultPriorityId);
            builder.HasOne(x => x.TaskCategory).WithMany(x => x.Tasks).HasForeignKey(x => x.TaskCategoryId);
        }
    }

    public class TaskSourceTypeConfiguration : IEntityTypeConfiguration<TaskSourceType>
    {
        public void Configure(EntityTypeBuilder<TaskSourceType> builder)
        {
            builder.ToTable("TaskSourceTypes", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.TaskSourceName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TaskSourceLocalName).IsRequired().HasMaxLength(200);


            builder.HasData(
                new TaskSourceType { Id = 1, TaskSourceName = "Personal", TaskSourceLocalName = "شخصی", SortOrder = 1 },
                new TaskSourceType { Id = 2, TaskSourceName = "Assigned", TaskSourceLocalName = "ارجاعی", SortOrder = 2 },
                new TaskSourceType { Id = 3, TaskSourceName = "System", TaskSourceLocalName = "سیستمی", SortOrder = 3 }
                );

        }
    }


    public class TaskCheckListTemplateConfiguration : IEntityTypeConfiguration<TaskCheckListTemplate>
    {
        public void Configure(EntityTypeBuilder<TaskCheckListTemplate> builder)
        {
            builder.ToTable("TaskCheckListTemplates", "base");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CheckListItemTitle).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(100);
            builder.HasOne(x => x.Task).WithMany(x => x.TaskCheckListTemplates).HasForeignKey(x => x.TaskId);
        }
    }




    public class TaskCheckListConfiguration : IEntityTypeConfiguration<TaskCheckListTemplate>
    {
        public void Configure(EntityTypeBuilder<TaskCheckListTemplate> builder)
        {
            builder.ToTable("TaskCheckListTemplates", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CheckListItemTitle).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId);
        }
    }

    public class TaskStatusConfiguration : IEntityTypeConfiguration<Domain.Entities.BaseInfo.Generals.TaskStatus>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.BaseInfo.Generals.TaskStatus> builder)
        {
            builder.ToTable("TaskStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.TaskStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TaskStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new Domain.Entities.BaseInfo.Generals.TaskStatus { Id = 1, TaskStatusName = "Pending" },
                new Domain.Entities.BaseInfo.Generals.TaskStatus { Id = 2, TaskStatusName = "In Progress" },
                new Domain.Entities.BaseInfo.Generals.TaskStatus { Id = 3, TaskStatusName = "Completed" },
                new Domain.Entities.BaseInfo.Generals.TaskStatus { Id = 4, TaskStatusName = "Canceled" },
                new Domain.Entities.BaseInfo.Generals.TaskStatus { Id = 5, TaskStatusName = "On Hold" }
                );



        }
    }

    public class CustomFieldConfiguration : IEntityTypeConfiguration<CustomField>
    {
        public void Configure(EntityTypeBuilder<CustomField> builder)
        {
            builder.ToTable("CustomFields", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CustomFieldName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CustomFieldLocalName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CustomFieldDataType).IsRequired().HasMaxLength(20);
        }
    }
    public class EndorsementTypeConfiguration : IEntityTypeConfiguration<EndorsementType>
    {
        public void Configure(EntityTypeBuilder<EndorsementType> builder)
        {
            builder.ToTable("EndorsementTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.EndorsementTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EndorsementTypeLocalName).HasMaxLength(50);

            builder.HasData(
                new EndorsementType { Id = 1, EndorsementTypeName = "Straight", EndorsementTypeLocalName = "غیر قابل انتقال" },
                new EndorsementType { Id = 2, EndorsementTypeName = "To Order", EndorsementTypeLocalName = "به دستور" },
                new EndorsementType { Id = 3, EndorsementTypeName = "Bearer", EndorsementTypeLocalName = "حامل" },
                new EndorsementType { Id = 4, EndorsementTypeName = "Blank Endorsed", EndorsementTypeLocalName = "ظهرنویسی سفید" },
                new EndorsementType { Id = 5, EndorsementTypeName = "Special Endorsed", EndorsementTypeLocalName = "ظهرنویسی خاص" }
                );
        }
    }

    public class InspectionTypeConfiguration : IEntityTypeConfiguration<InspectionType>
    {
        public void Configure(EntityTypeBuilder<InspectionType> builder)
        {
            builder.ToTable("InspectionTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InspectionTypeName).IsRequired().HasMaxLength(15);
            builder.Property(x => x.InspectionTypeLocalName).HasMaxLength(15);


            //Seed Data
            builder.HasData(
                new InspectionType { Id = 1, InspectionTypeName = "GREEN", InspectionTypeLocalName = "سبز" },
                new InspectionType { Id = 2, InspectionTypeName = "YELLOW", InspectionTypeLocalName = "زرد" },
                new InspectionType { Id = 3, InspectionTypeName = "RED", InspectionTypeLocalName = "قرمز" }
                );
        }
    }

    public class InspectionMethodConfiguration : IEntityTypeConfiguration<InspectionMethod>
    {
        public void Configure(EntityTypeBuilder<InspectionMethod> builder)
        {
            builder.ToTable("InspectionMethods", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InspectionMethodName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.InspectionMethodLocalName).HasMaxLength(50);


            //Seed Data
            builder.HasData(
                new InspectionMethod { Id = 1, InspectionMethodName = "DOCUMENT CHECK", InspectionMethodLocalName = "" },
                new InspectionMethod { Id = 2, InspectionMethodName = "PHYSICAL_INSPECTION", InspectionMethodLocalName = "" },
                new InspectionMethod { Id = 3, InspectionMethodName = "SCANNING", InspectionMethodLocalName = "" },
                new InspectionMethod { Id = 4, InspectionMethodName = "SAMPLING", InspectionMethodLocalName = "" },
                new InspectionMethod { Id = 5, InspectionMethodName = "LABORATORY", InspectionMethodLocalName = "" }
                );
        }
    }

    public class InspectionResultConfiguration : IEntityTypeConfiguration<InspectionResult>
    {
        public void Configure(EntityTypeBuilder<InspectionResult> builder)
        {
            builder.ToTable("InspectionResults", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InspectionResultName).IsRequired().HasMaxLength(30);
            builder.Property(x => x.InspectionResultLocalName).HasMaxLength(30);


            //Seed Data
            builder.HasData(
                new InspectionResult { Id = 1, InspectionResultName = "PENDING", InspectionResultLocalName = "" },
                new InspectionResult { Id = 2, InspectionResultName = "PASS", InspectionResultLocalName = "" },
                new InspectionResult { Id = 3, InspectionResultName = "FAIL", InspectionResultLocalName = "" },
                new InspectionResult { Id = 4, InspectionResultName = "CONDITIONAL", InspectionResultLocalName = "" }
                );
        }
    }


    public class LicenseTypeConfiguration : IEntityTypeConfiguration<LicenseType>
    {
        public void Configure(EntityTypeBuilder<LicenseType> builder)
        {
            builder.ToTable("LicenseTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LicenseTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LicenseTypeLocalName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.IssuingOrgName).IsRequired().HasMaxLength(100);


            //Seed Data
            builder.HasData(
                new LicenseType { Id = 1, LicenseTypeName = "STANDARD_ORGANIZATION", LicenseTypeLocalName = "استاندارد", IssuingOrgName = "" },
                new LicenseType { Id = 2, LicenseTypeName = "HEALTH_MINISTRY", LicenseTypeLocalName = "بهداشت", IssuingOrgName = "" },
                new LicenseType { Id = 3, LicenseTypeName = "AGRICULTURE_MINISTRY", LicenseTypeLocalName = "جهاد کشاورزی", IssuingOrgName = "" },
                new LicenseType { Id = 4, LicenseTypeName = "ENVIRONMENT", LicenseTypeLocalName = "محیط زیست", IssuingOrgName = "" },
                new LicenseType { Id = 5, LicenseTypeName = "ATOMIC_ENERGY", LicenseTypeLocalName = "انرژی اتمی", IssuingOrgName = "" },
                new LicenseType { Id = 6, LicenseTypeName = "CULTURAL_HERITAGE", LicenseTypeLocalName = " میراث فرهنگی", IssuingOrgName = "" },
                new LicenseType { Id = 7, LicenseTypeName = "POLICE", LicenseTypeLocalName = " نیروی انتظامی", IssuingOrgName = "" },
                new LicenseType { Id = 8, LicenseTypeName = "TELECOMMUNICATIONS", LicenseTypeLocalName = "مخابرات", IssuingOrgName = "" },
                new LicenseType { Id = 9, LicenseTypeName = "FOOD_AND_DRUG", LicenseTypeLocalName = "غذا و دارو", IssuingOrgName = "" },
                new LicenseType { Id = 10, LicenseTypeName = "VETERINARY", LicenseTypeLocalName = "دامپزشکی", IssuingOrgName = "" },
                new LicenseType { Id = 11, LicenseTypeName = "INDUSTRIAL_MINISTRY", LicenseTypeLocalName = "صنعت و معدن", IssuingOrgName = "" },
                new LicenseType { Id = 12, LicenseTypeName = "COMMERCIAL_LICENSE", LicenseTypeLocalName = " مجوز بازرگانی", IssuingOrgName = "" },
                new LicenseType { Id = 13, LicenseTypeName = "QUARANTINE", LicenseTypeLocalName = "قرنطینه", IssuingOrgName = "" }
                );
        }
    }

    public class LicenseStatusConfiguration : IEntityTypeConfiguration<LicenseStatus>
    {
        public void Configure(EntityTypeBuilder<LicenseStatus> builder)
        {
            builder.ToTable("LicenseStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.LicenseStatusName).IsRequired().HasMaxLength(30);
            builder.Property(x => x.LicenseStatusLocalName).IsRequired().HasMaxLength(30);

            //Seed Data
            builder.HasData(
                new LicenseStatus { Id = 1, LicenseStatusName = "REQUIRED", LicenseStatusLocalName = "" },
                new LicenseStatus { Id = 2, LicenseStatusName = "APPLIED", LicenseStatusLocalName = "" },
                new LicenseStatus { Id = 3, LicenseStatusName = "RECEIVED", LicenseStatusLocalName = "" },
                new LicenseStatus { Id = 4, LicenseStatusName = "REJECTED", LicenseStatusLocalName = "" },
                new LicenseStatus { Id = 5, LicenseStatusName = "WAIVED", LicenseStatusLocalName = "" }
                );
        }
    }




    public class QuotationStatusConfiguration : IEntityTypeConfiguration<QuotationStatus>
    {
        public void Configure(EntityTypeBuilder<QuotationStatus> builder)
        {
            builder.ToTable("QuotationStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.QuotationStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.QuotationStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new QuotationStatus { Id = 1, QuotationStatusName = "Draft", QuotationStatusLocalName = "در جریان" },
                new QuotationStatus { Id = 2, QuotationStatusName = "Sent", QuotationStatusLocalName = "ارسال شد" },
                new QuotationStatus { Id = 3, QuotationStatusName = "Accepted", QuotationStatusLocalName = "قبول شد" },
                new QuotationStatus { Id = 4, QuotationStatusName = "Rejected", QuotationStatusLocalName = "رد شد" },
                new QuotationStatus { Id = 5, QuotationStatusName = "Expired", QuotationStatusLocalName = "منقضی شد" },
                new QuotationStatus { Id = 6, QuotationStatusName = "Cancelled", QuotationStatusLocalName = "لغو شد" },
                new QuotationStatus { Id = 7, QuotationStatusName = "Converted", QuotationStatusLocalName = "تبدیل شد" }
                );
        }
    }















    public class CorrespondenceSubjectConfiguration : IEntityTypeConfiguration<CorrespondenceSubject>
    {
        public void Configure(EntityTypeBuilder<CorrespondenceSubject> builder)
        {
            builder.ToTable("CorrespondenceSubjects", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.SubjectTitle).IsRequired().HasMaxLength(100);

            //Seed Data
            builder.HasData(
                new CorrespondenceSubject { Id = 1, SubjectTitle = "عمومی", SystemDefined = true }
                );
        }
    }


    public class GPSDeviceStatusConfiguration : IEntityTypeConfiguration<GPSDeviceStatus>
    {
        public void Configure(EntityTypeBuilder<GPSDeviceStatus> builder)
        {
            builder.ToTable("GPSDeviceStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.GPSDeviceStatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.GPSDeviceStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new GPSDeviceStatus { Id = 1, GPSDeviceStatusName = "Available", GPSDeviceStatusLocalName = "آماده نصب" },
                new GPSDeviceStatus { Id = 2, GPSDeviceStatusName = "Installed", GPSDeviceStatusLocalName = "نصب روی کامیون " },
                new GPSDeviceStatus { Id = 3, GPSDeviceStatusName = "Faulty", GPSDeviceStatusLocalName = "خراب و نیازمند تعمیر" },
                new GPSDeviceStatus { Id = 4, GPSDeviceStatusName = "InRepair", GPSDeviceStatusLocalName = "در حال تعمیر" },
                new GPSDeviceStatus { Id = 5, GPSDeviceStatusName = "Uninstalled", GPSDeviceStatusLocalName = "جدا شده از کامیون" },
                new GPSDeviceStatus { Id = 6, GPSDeviceStatusName = "Retired", GPSDeviceStatusLocalName = "قدیمی و غیرقابل استفاده" }
                );
        }
    }
    public class TripStatusConfiguration : IEntityTypeConfiguration<TripStatus>
    {
        public void Configure(EntityTypeBuilder<TripStatus> builder)
        {
            builder.ToTable("TripStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TripStatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.TripStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new TripStatus { Id = 1, TripStatusName = "Registered", TripStatusLocalName = "ثبت شده" },
                new TripStatus { Id = 2, TripStatusName = "Approved", TripStatusLocalName = "تایید شده" },
                new TripStatus { Id = 3, TripStatusName = "Loading", TripStatusLocalName = "در حال بارگیری" },
                new TripStatus { Id = 4, TripStatusName = "Departed", TripStatusLocalName = "حرکت از مبدا" },
                new TripStatus { Id = 5, TripStatusName = "AtBorder", TripStatusLocalName = "در مرز" },
                new TripStatus { Id = 6, TripStatusName = "BorderCleared", TripStatusLocalName = "ترخیص از مرز" },
                new TripStatus { Id = 7, TripStatusName = "InTransit", TripStatusLocalName = "در مسیر مقصد" },
                new TripStatus { Id = 8, TripStatusName = "Arrived", TripStatusLocalName = "رسیده به مقصد" },
                new TripStatus { Id = 9, TripStatusName = "Unloading", TripStatusLocalName = "در حال تخلیه" },
                new TripStatus { Id = 10, TripStatusName = "Delivered", TripStatusLocalName = "تحویل داده شده" },
                new TripStatus { Id = 11, TripStatusName = "Completed", TripStatusLocalName = "اتمام و بستن سفر" },
                new TripStatus { Id = 99, TripStatusName = "Canceled", TripStatusLocalName = "لغو شده" }
                );
        }
    }

    public class PostalDispatchStatusConfiguration : IEntityTypeConfiguration<PostalDispatchStatus>
    {
        public void Configure(EntityTypeBuilder<PostalDispatchStatus> builder)
        {
            builder.ToTable("PostalDispatchStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.StatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new PostalDispatchStatus { Id = 1, StatusName = "Registered", StatusLocalName = "ثبت شده" },
                new PostalDispatchStatus { Id = 2, StatusName = "Pending", StatusLocalName = "در انتظار ارسال" },
                new PostalDispatchStatus { Id = 3, StatusName = "Dispatched ", StatusLocalName = "ارسال شده" },
                new PostalDispatchStatus { Id = 4, StatusName = "In Transit", StatusLocalName = "در حال حمل" },
                new PostalDispatchStatus { Id = 5, StatusName = "Delivered", StatusLocalName = "تحویل داده شده" },
                new PostalDispatchStatus { Id = 6, StatusName = "Returned", StatusLocalName = "برگشت خورده" },
                new PostalDispatchStatus { Id = 7, StatusName = "Cancelled", StatusLocalName = "لغو شده" }
                );
        }
    }


    public class LegTypeConfiguration : IEntityTypeConfiguration<LegType>
    {
        public void Configure(EntityTypeBuilder<LegType> builder)
        {
            builder.ToTable("LegTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LegTypeName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new LegType { Id = 1, LegTypeName = "Main Leg" },
                new LegType { Id = 2, LegTypeName = "Pre-Carriage" },
                new LegType { Id = 3, LegTypeName = "On-Carriage" },
                new LegType { Id = 4, LegTypeName = "Transshipment" },
                new LegType { Id = 5, LegTypeName = "Pickup" },
                new LegType { Id = 6, LegTypeName = "Delivery" }
                );
        }
    }

    public class CompanyRoleTypeConfiguration : IEntityTypeConfiguration<CompanyRoleType>
    {
        public void Configure(EntityTypeBuilder<CompanyRoleType> builder)
        {
            builder.ToTable("CompanyRoleTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CompanyRoleTypeName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.CompanyRoleTypeLocalName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new CompanyRoleType { Id = 1, CompanyRoleTypeName = "Forwarder", CompanyRoleTypeLocalName = "فورواردر" },
                new CompanyRoleType { Id = 2, CompanyRoleTypeName = "Shipping Line", CompanyRoleTypeLocalName = "خط کشتیرانی" },
                new CompanyRoleType { Id = 3, CompanyRoleTypeName = "NVOCC", CompanyRoleTypeLocalName = "NVOCC" },
                new CompanyRoleType { Id = 4, CompanyRoleTypeName = "Transit Carrier", CompanyRoleTypeLocalName = "کریر ترانزیت" },
                new CompanyRoleType { Id = 5, CompanyRoleTypeName = "Customs Broker", CompanyRoleTypeLocalName = "کارگزار گمرک" },
                new CompanyRoleType { Id = 6, CompanyRoleTypeName = "Agent", CompanyRoleTypeLocalName = "نماینده" }
                );
        }
    }


    public class RTSInstructionStatusConfiguration : IEntityTypeConfiguration<RTSInstructionStatus>
    {
        public void Configure(EntityTypeBuilder<RTSInstructionStatus> builder)
        {
            builder.ToTable("RTSInstructionStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.StatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new RTSInstructionStatus { Id = 1, StatusName = "Draft", StatusLocalName = "در حال آماده سازی" },
                new RTSInstructionStatus { Id = 2, StatusName = "Issued", StatusLocalName = "صادر شده" },
                new RTSInstructionStatus { Id = 3, StatusName = "Partialy Fulfilled", StatusLocalName = "انجام ناقص" },
                new RTSInstructionStatus { Id = 4, StatusName = "Fulfilled", StatusLocalName = "انجام کامل" },
                new RTSInstructionStatus { Id = 5, StatusName = "Canceled", StatusLocalName = "لغو شده" }
                );
        }
    }


    public class RateRequestSourceConfiguration : IEntityTypeConfiguration<RateRequestSource>
    {
        public void Configure(EntityTypeBuilder<RateRequestSource> builder)
        {
            builder.ToTable("RateRequestSources", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RateRequestSourceName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.RateRequestSourceLocalName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new RateRequestSource { Id = 1, RateRequestSourceName = "Email", RateRequestSourceLocalName = "ایمیل" },
                new RateRequestSource { Id = 2, RateRequestSourceName = "Phone", RateRequestSourceLocalName = "تلفنی" },
                new RateRequestSource { Id = 3, RateRequestSourceName = "Website", RateRequestSourceLocalName = "وب سایت" },
                new RateRequestSource { Id = 4, RateRequestSourceName = "Face To Face", RateRequestSourceLocalName = "حضوری" },
                new RateRequestSource { Id = 5, RateRequestSourceName = "WhatsApp", RateRequestSourceLocalName = "واتس اپ" }
                );
        }
    }

    public class AddressTypeConfiguration : IEntityTypeConfiguration<AddressType>
    {
        public void Configure(EntityTypeBuilder<AddressType> builder)
        {
            builder.ToTable("AddressTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.AddressTypeName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.AddressTypeLocalName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new AddressType { Id = 1, AddressTypeName = "Billing", AddressTypeLocalName = "صورتحساب" },
                new AddressType { Id = 2, AddressTypeName = "Shipping", AddressTypeLocalName = "ارسال و دریافت محموله" },
                new AddressType { Id = 3, AddressTypeName = "Pickup", AddressTypeLocalName = "برداشتن کالا" },
                new AddressType { Id = 4, AddressTypeName = "Delivery", AddressTypeLocalName = "تحویل" },
                new AddressType { Id = 5, AddressTypeName = "Warehouse", AddressTypeLocalName = "انبار" },
                new AddressType { Id = 6, AddressTypeName = "Registered", AddressTypeLocalName = "ثبتی" },
                new AddressType { Id = 10, AddressTypeName = "Other", AddressTypeLocalName = "سایر" }
                );
        }
    }


    public class InquiryStatusConfiguration : IEntityTypeConfiguration<InquiryStatus>
    {
        public void Configure(EntityTypeBuilder<InquiryStatus> builder)
        {
            builder.ToTable("InquiryStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.StatusLocalName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new InquiryStatus { Id = 1, StatusName = "Draft", StatusLocalName = "درافت" },
                new InquiryStatus { Id = 2, StatusName = "Sent", StatusLocalName = "ارسال شده" },
                new InquiryStatus { Id = 3, StatusName = "Seen", StatusLocalName = "مشاهده شده" },
                new InquiryStatus { Id = 4, StatusName = "Replied", StatusLocalName = "پاسخ داده شده" },
                new InquiryStatus { Id = 5, StatusName = "Rejected", StatusLocalName = "رد شده" },
                new InquiryStatus { Id = 6, StatusName = "Expired", StatusLocalName = "منقضی شده" },
                new InquiryStatus { Id = 7, StatusName = "Cancelled", StatusLocalName = "لغو شده" }
                );
        }
    }


    public class PlanStatusConfiguration : IEntityTypeConfiguration<PlanStatus>
    {
        public void Configure(EntityTypeBuilder<PlanStatus> builder)
        {
            builder.ToTable("PlanStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.StatusLocalName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new PlanStatus { Id = 1, StatusName = "Planned", StatusLocalName = "برنامه ریزی شده" },
                new PlanStatus { Id = 2, StatusName = "Pending", StatusLocalName = "در انتظار" },
                new PlanStatus { Id = 3, StatusName = "InProgress", StatusLocalName = "در جریان" },
                new PlanStatus { Id = 4, StatusName = "Done", StatusLocalName = "انجام شده" },
                new PlanStatus { Id = 5, StatusName = "Skipped", StatusLocalName = "پرش و عدم انجام" },
                new PlanStatus { Id = 6, StatusName = "Cancelled", StatusLocalName = "لغو شده" },
                new PlanStatus { Id = 7, StatusName = "NotApplicable", StatusLocalName = "غیر قابل اعمال" }
                );
        }
    }

    public class VGMStatusConfiguration : IEntityTypeConfiguration<VGMStatus>
    {
        public void Configure(EntityTypeBuilder<VGMStatus> builder)
        {
            builder.ToTable("VGMStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StatusName).IsRequired().HasMaxLength(40);
            builder.Property(c => c.StatusLocalName).IsRequired().HasMaxLength(40);

            //Seed Data
            builder.HasData(
                new VGMStatus { Id = 1, StatusName = "Draft", StatusLocalName = "درافت" },
                new VGMStatus { Id = 2, StatusName = "Submitted", StatusLocalName = "ارسال شده" },
                new VGMStatus { Id = 3, StatusName = "Confirmed", StatusLocalName = "تایید شده" },
                new VGMStatus { Id = 4, StatusName = "Rejected", StatusLocalName = "رد شده" },
                new VGMStatus { Id = 5, StatusName = "Canceled", StatusLocalName = "لغو شده" }
                );
        }
    }

    public class StuffingTypeConfiguration : IEntityTypeConfiguration<StuffingType>
    {
        public void Configure(EntityTypeBuilder<StuffingType> builder)
        {
            builder.ToTable("StuffingTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StuffingTypeName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.StuffingTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new StuffingType { Id = 1, StuffingTypeName = "Factory Stuffed", StuffingTypeLocalName = "کارخانه" },
                new StuffingType { Id = 2, StuffingTypeName = "CFS Stuffed", StuffingTypeLocalName = "CFS" },
                new StuffingType { Id = 3, StuffingTypeName = "Terminal Stuffed", StuffingTypeLocalName = "ترمینال" },
                new StuffingType { Id = 4, StuffingTypeName = "Warehouse Stuffed", StuffingTypeLocalName = "انبار" },
                new StuffingType { Id = 5, StuffingTypeName = "Shipper Stuffed", StuffingTypeLocalName = "فروشنده" },
                new StuffingType { Id = 6, StuffingTypeName = "Carrier Stuffed", StuffingTypeLocalName = "حمل کننده" }
                );
        }
    }


    public class BLLevelConfiguration : IEntityTypeConfiguration<BLLevel>
    {
        public void Configure(EntityTypeBuilder<BLLevel> builder)
        {
            builder.ToTable("BLLevels", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BLLevelName).IsRequired().HasMaxLength(30);

            //Seed Data
            builder.HasData(
                new BLLevel { Id = 1, BLLevelName = "Master" },
                new BLLevel { Id = 2, BLLevelName = "House" },
                new BLLevel { Id = 3, BLLevelName = "Direct" }
                );
        }
    }

    public class BLLinkTypeConfiguration : IEntityTypeConfiguration<BLLinkType>
    {
        public void Configure(EntityTypeBuilder<BLLinkType> builder)
        {
            builder.ToTable("BLLinkTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BLLinkTypeName).IsRequired().HasMaxLength(100);

            //Seed Data
            builder.HasData(
                new BLLinkType { Id = 1, BLLinkTypeName = "House → Master" },
                new BLLinkType { Id = 2, BLLinkTypeName = "Amendment of" },
                new BLLinkType { Id = 3, BLLinkTypeName = "Replacement of" },
                new BLLinkType { Id = 4, BLLinkTypeName = "Switch BL" },
                new BLLinkType { Id = 5, BLLinkTypeName = "Split from" },
                new BLLinkType { Id = 6, BLLinkTypeName = "Consolidated into" },
                new BLLinkType { Id = 7, BLLinkTypeName = "Derived from" },
                new BLLinkType { Id = 8, BLLinkTypeName = "Cancelled and replaced by" }
                );
        }
    }


    public class ContainerVoyageAllocationStatusConfiguration : IEntityTypeConfiguration<ContainerVoyageAllocationStatus>
    {
        public void Configure(EntityTypeBuilder<ContainerVoyageAllocationStatus> builder)
        {
            builder.ToTable("ContainerVoyageAllocationStatuses", "base");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(300);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasData(
                new ContainerVoyageAllocationStatus
                {
                    Id = 1,
                    Code = "PLANNED",
                    Name = "Planned",
                    Description = "کانتینر برای این سفر برنامه‌ریزی شده است.",
                    SortOrder = 1,
                    IsActive = true
                },
                new ContainerVoyageAllocationStatus
                {
                    Id = 2,
                    Code = "CONFIRMED",
                    Name = "Confirmed",
                    Description = "تخصیص کانتینر به سفر تأیید شده است.",
                    SortOrder = 2,
                    IsActive = true
                },
                new ContainerVoyageAllocationStatus
                {
                    Id = 3,
                    Code = "ROLLED_OVER",
                    Name = "Rolled Over",
                    Description = "کانتینر از این سفر به سفر بعد منتقل شده است.",
                    SortOrder = 3,
                    IsActive = true
                },
                new ContainerVoyageAllocationStatus
                {
                    Id = 4,
                    Code = "CANCELLED",
                    Name = "Cancelled",
                    Description = "تخصیص کانتینر به این سفر لغو شده است.",
                    SortOrder = 4,
                    IsActive = true
                }
            );
        }
    }


    public class LoadListStatusConfiguration : IEntityTypeConfiguration<LoadListStatus>
    {
        public void Configure(EntityTypeBuilder<LoadListStatus> builder)
        {
            builder.ToTable("LoadListStatuses", "base");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(300);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasData(
                new LoadListStatus
                {
                    Id = 1,
                    Code = "DRAFT",
                    Name = "Draft",
                    Description = "لیست بارگیری در حال تهیه است.",
                    SortOrder = 1,
                    IsActive = true
                },
                new LoadListStatus
                {
                    Id = 2,
                    Code = "PREPARED",
                    Name = "Prepared",
                    Description = "لیست بارگیری آماده شده است.",
                    SortOrder = 2,
                    IsActive = true
                },
                new LoadListStatus
                {
                    Id = 3,
                    Code = "SENT_TO_TERMINAL",
                    Name = "Sent To Terminal",
                    Description = "لیست بارگیری به ترمینال ارسال شده است.",
                    SortOrder = 3,
                    IsActive = true
                },
                new LoadListStatus
                {
                    Id = 4,
                    Code = "CONFIRMED",
                    Name = "Confirmed",
                    Description = "لیست بارگیری تأیید شده است.",
                    SortOrder = 4,
                    IsActive = true
                },
                new LoadListStatus
                {
                    Id = 5,
                    Code = "CLOSED",
                    Name = "Closed",
                    Description = "لیست بارگیری نهایی و بسته شده است.",
                    SortOrder = 5,
                    IsActive = true
                },
                new LoadListStatus
                {
                    Id = 6,
                    Code = "CANCELLED",
                    Name = "Cancelled",
                    Description = "لیست بارگیری لغو شده است.",
                    SortOrder = 6,
                    IsActive = true
                }
            );
        }
    }


    public class LoadListItemStatusConfiguration : IEntityTypeConfiguration<LoadListItemStatus>
    {
        public void Configure(EntityTypeBuilder<LoadListItemStatus> builder)
        {
            builder.ToTable("LoadListItemStatuses", "base");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(300);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasData(
                new LoadListItemStatus
                {
                    Id = 1,
                    Code = "PLANNED",
                    Name = "Planned",
                    Description = "کانتینر در لیست بارگیری قرار گرفته است.",
                    SortOrder = 1,
                    IsActive = true
                },
                new LoadListItemStatus
                {
                    Id = 2,
                    Code = "READY_TO_LOAD",
                    Name = "Ready To Load",
                    Description = "کانتینر آماده بارگیری است.",
                    SortOrder = 2,
                    IsActive = true
                },
                new LoadListItemStatus
                {
                    Id = 3,
                    Code = "LOADED",
                    Name = "Loaded",
                    Description = "کانتینر با موفقیت بارگیری شده است.",
                    SortOrder = 3,
                    IsActive = true
                },
                new LoadListItemStatus
                {
                    Id = 4,
                    Code = "SHORT_SHIPPED",
                    Name = "Short Shipped",
                    Description = "کانتینر در لیست بوده ولی بارگیری نشده است.",
                    SortOrder = 4,
                    IsActive = true
                },
                new LoadListItemStatus
                {
                    Id = 5,
                    Code = "SHUT_OUT",
                    Name = "Shut Out",
                    Description = "کانتینر به دلیل محدودیت یا مشکل عملیاتی جا مانده است.",
                    SortOrder = 5,
                    IsActive = true
                },
                new LoadListItemStatus
                {
                    Id = 6,
                    Code = "ROLLED_OVER",
                    Name = "Rolled Over",
                    Description = "کانتینر به سفر بعد منتقل شده است.",
                    SortOrder = 6,
                    IsActive = true
                },
                new LoadListItemStatus
                {
                    Id = 7,
                    Code = "CANCELLED",
                    Name = "Cancelled",
                    Description = "کانتینر از لیست بارگیری حذف یا لغو شده است.",
                    SortOrder = 7,
                    IsActive = true
                }
            );
        }
    }



    public class ChargeStatusConfiguration : IEntityTypeConfiguration<ChargeStatus>
    {
        public void Configure(EntityTypeBuilder<ChargeStatus> builder)
        {
            builder.ToTable("ChargeStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ChargeStatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ChargeStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new ChargeStatus { Id = 1, ChargeStatusName = "Draft", ChargeStatusLocalName = "پیش نویس" },
                new ChargeStatus { Id = 2, ChargeStatusName = "Approved", ChargeStatusLocalName = "تایید شده" },
                new ChargeStatus { Id = 3, ChargeStatusName = "Canceled", ChargeStatusLocalName = "لغو شده" }
                );
        }
    }
    public class TreasuryDocumentTypeConfiguration : IEntityTypeConfiguration<TreasuryDocumentType>
    {
        public void Configure(EntityTypeBuilder<TreasuryDocumentType> builder)
        {
            builder.ToTable("TreasuryDocumentTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TreasuryDocumentTypeName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.TreasuryDocumentTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new TreasuryDocumentType { Id = 1, TreasuryDocumentTypeName = "Receipt", TreasuryDocumentTypeLocalName = "دریافت" },
                new TreasuryDocumentType { Id = 2, TreasuryDocumentTypeName = "Payment", TreasuryDocumentTypeLocalName = "پرداخت" }
                );
        }
    }

    public class TreasuryDocumentStatusConfiguration : IEntityTypeConfiguration<TreasuryDocumentStatus>
    {
        public void Configure(EntityTypeBuilder<TreasuryDocumentStatus> builder)
        {
            builder.ToTable("TreasuryDocumentStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TreasuryDocumentStatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.TreasuryDocumentStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new TreasuryDocumentStatus { Id = 1, TreasuryDocumentStatusName = "Draft", TreasuryDocumentStatusLocalName = "پیش نویس" },
                new TreasuryDocumentStatus { Id = 2, TreasuryDocumentStatusName = "Confirmed", TreasuryDocumentStatusLocalName = "تایید شده" },
                new TreasuryDocumentStatus { Id = 3, TreasuryDocumentStatusName = "Posted", TreasuryDocumentStatusLocalName = "Posted" },
                new TreasuryDocumentStatus { Id = 4, TreasuryDocumentStatusName = "Canceled", TreasuryDocumentStatusLocalName = "لغو شده" }
                );
        }
    }

    public class BranchTypeConfiguration : IEntityTypeConfiguration<BranchType>
    {
        public void Configure(EntityTypeBuilder<BranchType> builder)
        {
            builder.ToTable("BranchTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BranchTypeName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.BranchTypeLocalName).IsRequired().HasMaxLength(100);

            //Seed Data
            builder.HasData(
                new BranchType { Id = 1, BranchTypeName = "Head Office", BranchTypeLocalName = "دفتر مرکزی" },
                new BranchType { Id = 2, BranchTypeName = "Operational", BranchTypeLocalName = "عملیات" },
                new BranchType { Id = 3, BranchTypeName = "Sales", BranchTypeLocalName = "فروش" },
                new BranchType { Id = 4, BranchTypeName = "Finance", BranchTypeLocalName = "مالی" },
                new BranchType { Id = 5, BranchTypeName = "Warehouse", BranchTypeLocalName = "انبار" },
                new BranchType { Id = 6, BranchTypeName = "PortOffice", BranchTypeLocalName = "دفتر بندر" }
                );
        }
    }


    public class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
    {
        public void Configure(EntityTypeBuilder<EntityType> builder)
        {
            builder.ToTable("EntityTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.EntityTypeName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new EntityType { Id = 1, EntityTypeName = "Job" },
                new EntityType { Id = 2, EntityTypeName = "Booking" },
                new EntityType { Id = 3, EntityTypeName = "Shipment" },
                new EntityType { Id = 4, EntityTypeName = "ShipmentLeg" },
                new EntityType { Id = 5, EntityTypeName = "BL" },
                new EntityType { Id = 6, EntityTypeName = "BLContainer" },
                new EntityType { Id = 7, EntityTypeName = "Container" },
                new EntityType { Id = 8, EntityTypeName = "RateRequest" },
                new EntityType { Id = 9, EntityTypeName = "BuyQuote" },
                new EntityType { Id = 10, EntityTypeName = "SellQuotae" },
                new EntityType { Id = 11, EntityTypeName = "Invoice" },
                new EntityType { Id = 12, EntityTypeName = "Charge" },
                new EntityType { Id = 13, EntityTypeName = "Person" },
                new EntityType { Id = 14, EntityTypeName = "CustomsClearance" },
                new EntityType { Id = 15, EntityTypeName = "Truck" },
                new EntityType { Id = 16, EntityTypeName = "TIRCarnet" },
                new EntityType { Id = 17, EntityTypeName = "BlackList" },
                new EntityType { Id = 18, EntityTypeName = "Cheque" },
                new EntityType { Id = 19, EntityTypeName = "PaymentDocument" },
                new EntityType { Id = 20, EntityTypeName = "Detention" },
                new EntityType { Id = 21, EntityTypeName = "DeliveryOrder" },
                new EntityType { Id = 22, EntityTypeName = "DeliveryOrder" },
                new EntityType { Id = 23, EntityTypeName = "GPSDevice" },
                new EntityType { Id = 24, EntityTypeName = "Hologram" },
                new EntityType { Id = 25, EntityTypeName = "Lead" },
                new EntityType { Id = 26, EntityTypeName = "License" },
                new EntityType { Id = 27, EntityTypeName = "LoadList" },
                new EntityType { Id = 28, EntityTypeName = "LoadListItem" },
                new EntityType { Id = 29, EntityTypeName = "Member" },
                new EntityType { Id = 30, EntityTypeName = "Plan" },
                new EntityType { Id = 31, EntityTypeName = "PostalDispatch" },
                new EntityType { Id = 32, EntityTypeName = "ProductRequest" },
                new EntityType { Id = 33, EntityTypeName = "NOA" },
                new EntityType { Id = 34, EntityTypeName = "ReturnCash" },
                new EntityType { Id = 35, EntityTypeName = "ReturnCash" },
                new EntityType { Id = 36, EntityTypeName = "RTSInstruction" },
                new EntityType { Id = 37, EntityTypeName = "RTSInstruction" },
                new EntityType { Id = 38, EntityTypeName = "Settlement" },
                new EntityType { Id = 39, EntityTypeName = "Settlement" },
                new EntityType { Id = 40, EntityTypeName = "Tariff" },
                new EntityType { Id = 41, EntityTypeName = "Task" },
                new EntityType { Id = 42, EntityTypeName = "Task" },
                new EntityType { Id = 43, EntityTypeName = "TelexRelease" },
                new EntityType { Id = 44, EntityTypeName = "TIP" },
                new EntityType { Id = 45, EntityTypeName = "TreasuryDocument" },
                new EntityType { Id = 46, EntityTypeName = "Trip" },
                new EntityType { Id = 47, EntityTypeName = "VGM" },
                new EntityType { Id = 48, EntityTypeName = "Customer" }
                );
        }
    }
    public class CampaignTypeConfiguration : IEntityTypeConfiguration<CampaignType>
    {
        public void Configure(EntityTypeBuilder<CampaignType> builder)
        {
            builder.ToTable("CampaignTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CampaignTypeName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.CampaignTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new CampaignType { Id = 1, CampaignTypeName = "Email Marketing", CampaignTypeLocalName = "بازاریابی ایمیلی" },
                new CampaignType { Id = 2, CampaignTypeName = "LinkedIn Campaign", CampaignTypeLocalName = "کمپین لینکدین" },
                new CampaignType { Id = 3, CampaignTypeName = "Google Ads", CampaignTypeLocalName = "گوگل ادز" },
                new CampaignType { Id = 4, CampaignTypeName = "Exhibition", CampaignTypeLocalName = "نمایشگاه" },
                new CampaignType { Id = 5, CampaignTypeName = "Cold Calling", CampaignTypeLocalName = "تماس سرد" },
                new CampaignType { Id = 6, CampaignTypeName = "Partner Referral", CampaignTypeLocalName = "معرفی همکار" },
                new CampaignType { Id = 7, CampaignTypeName = "Webinar", CampaignTypeLocalName = "وبینار" }
                );
        }
    }

    public class CampaignStatusConfiguration : IEntityTypeConfiguration<CampaignStatus>
    {
        public void Configure(EntityTypeBuilder<CampaignStatus> builder)
        {
            builder.ToTable("CampaignStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CampaignStatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.CampaignStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new CampaignStatus { Id = 1, CampaignStatusName = "Planned", CampaignStatusLocalName = "برنامه ریزی شده" },
                new CampaignStatus { Id = 2, CampaignStatusName = "Active", CampaignStatusLocalName = "فعال" },
                new CampaignStatus { Id = 3, CampaignStatusName = "Paused", CampaignStatusLocalName = "متوقف شده" },
                new CampaignStatus { Id = 4, CampaignStatusName = "Completed", CampaignStatusLocalName = "تمام شده" },
                new CampaignStatus { Id = 5, CampaignStatusName = "Cancelled", CampaignStatusLocalName = "لغو شده" }
                );
        }
    }

    public class MemberStatusConfiguration : IEntityTypeConfiguration<MemberStatus>
    {
        public void Configure(EntityTypeBuilder<MemberStatus> builder)
        {
            builder.ToTable("MemberStatus", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.MemberStatusName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.MemberStatusLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new MemberStatus { Id = 1, MemberStatusName = "Targeted", MemberStatusLocalName = "هدف گذاری" },
                new MemberStatus { Id = 2, MemberStatusName = "Sent", MemberStatusLocalName = "ارسال شد" },
                new MemberStatus { Id = 3, MemberStatusName = "Opened", MemberStatusLocalName = "باز کرد" },
                new MemberStatus { Id = 4, MemberStatusName = "Clicked", MemberStatusLocalName = "کلیک کرد" },
                new MemberStatus { Id = 5, MemberStatusName = "Responded", MemberStatusLocalName = "پاسخ داد" },
                new MemberStatus { Id = 6, MemberStatusName = "Interested", MemberStatusLocalName = "علاقه مند" },
                new MemberStatus { Id = 7, MemberStatusName = "Not Interested", MemberStatusLocalName = "عدم علاقه" },
                new MemberStatus { Id = 8, MemberStatusName = "Converted", MemberStatusLocalName = "تبدیل شد" },
                new MemberStatus { Id = 9, MemberStatusName = "Unsubscribed", MemberStatusLocalName = "لغو عضویت" }
                );
        }
    }
    public class ScopeTypeConfiguration : IEntityTypeConfiguration<ScopeType>
    {
        public void Configure(EntityTypeBuilder<ScopeType> builder)
        {
            builder.ToTable("ScopeTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ScopeTypeName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ScopeTypeLocalName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Description).HasMaxLength(200);
            //Seed Data
            builder.HasData(
                new ScopeType { Id = 1, ScopeTypeName = "Global", ScopeTypeLocalName = "بدون محدودیت مکانی یا طرف حساب" },
                new ScopeType { Id = 2, ScopeTypeName = "Country", ScopeTypeLocalName = "محدوده بر اساس کشور" },
                new ScopeType { Id = 3, ScopeTypeName = "Port", ScopeTypeLocalName = "محدوده بر اساس بندر" },
                new ScopeType { Id = 4, ScopeTypeName = "AirPort", ScopeTypeLocalName = "محدوده بر اساس فرودگاه" },
                new ScopeType { Id = 5, ScopeTypeName = "Terminal", ScopeTypeLocalName = "محدوده بر اساس ترمینال" },
                new ScopeType { Id = 6, ScopeTypeName = "City", ScopeTypeLocalName = "محدوده بر اساس شهر" },
                new ScopeType { Id = 7, ScopeTypeName = "Carrier", ScopeTypeLocalName = "مخصوص یک حمل کننده" },
                new ScopeType { Id = 8, ScopeTypeName = "Agent", ScopeTypeLocalName = "مخصوص یک نماینده" },
                new ScopeType { Id = 9, ScopeTypeName = "Customer", ScopeTypeLocalName = "مربوط به یک مشتری خاص" },
                new ScopeType { Id = 10, ScopeTypeName = "Contract", ScopeTypeLocalName = "بر اساس قرارداد خاص" },
                new ScopeType { Id = 11, ScopeTypeName = "Route", ScopeTypeLocalName = "بر اساس مسیر مشخص" }
                );
        }
    }


    public class JobSourceTypeConfiguration : IEntityTypeConfiguration<JobSourceType>
    {
        public void Configure(EntityTypeBuilder<JobSourceType> builder)
        {
            builder.ToTable("JobSourceTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.JobSourceTypeName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.JobSourceTypeLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new JobSourceType { Id = 1, JobSourceTypeName = "Manual", JobSourceTypeLocalName = "دستی" },
                new JobSourceType { Id = 2, JobSourceTypeName = "Rate Quotation", JobSourceTypeLocalName = "استعلام و تایید نرخ" }
                );
        }
    }

    public class ActivityCategoryConfiguration : IEntityTypeConfiguration<ActivityCategory>
    {
        public void Configure(EntityTypeBuilder<ActivityCategory> builder)
        {
            builder.ToTable("ActivityCategory", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ActivityCategoryName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ActivityCategoryLocalName).IsRequired().HasMaxLength(50);

            //Seed Data
            builder.HasData(
                new ActivityCategory { Id = 1, ActivityCategoryName = "Event", ActivityCategoryLocalName = "رویداد" },
                new ActivityCategory { Id = 2, ActivityCategoryName = "History", ActivityCategoryLocalName = "تاریخچه" },
                new ActivityCategory { Id = 3, ActivityCategoryName = "Status", ActivityCategoryLocalName = "وضعیت" },
                new ActivityCategory { Id = 4, ActivityCategoryName = "Audit", ActivityCategoryLocalName = "امنیتی/کنترلی" },
                new ActivityCategory { Id = 5, ActivityCategoryName = "System", ActivityCategoryLocalName = "عملیات اتوماتیک سیستمی" }
                );
        }
    }

    public class ServiceRequestTypeConfiguration : IEntityTypeConfiguration<ServiceRequestType>
    {
        public void Configure(EntityTypeBuilder<ServiceRequestType> builder)
        {
            builder.ToTable("ServiceRequestTypes", "base");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Property(c => c.LocalName).HasMaxLength(50);

            //Seed Data
            builder.HasData(
                    new ServiceRequestType { Id = 1, Name = "BL Amendment", LocalName = "اصلاح بارنامه" },
                    new ServiceRequestType { Id = 2, Name = "Consolidation", LocalName = "تجمیع/ترکیب" },
                    new ServiceRequestType { Id = 3, Name = "Split", LocalName = "تقسیم" },
                    //new ServiceRequestType { Id = 4, Name = "Change Consignee", LocalName = "اصلاح صاحب کالا" },
                    //new ServiceRequestType { Id = 5, Name = "Change Notify Party", LocalName = "اصلاح نوتیفای" },
                    //new ServiceRequestType { Id = 6, Name = "Change Delivery Order", LocalName = "تغییر ترخیصیه" },
                    //new ServiceRequestType { Id = 7, Name = "Change Container Allocation", LocalName = "تغییر تخصیص کانتینر" },
                    new ServiceRequestType { Id = 4, Name = "Document Correction", LocalName = "اصلاح مستندات" },
                    //new ServiceRequestType { Id = 9, Name = "Cargo Information Correction", LocalName = "اصلاح اطلاعات محموله" },
                    //new ServiceRequestType { Id = 10, Name = "Freight Revision", LocalName = "اصلاح کرایه حمل" },
                    new ServiceRequestType { Id = 5, Name = "Customs Related Request", LocalName = "درخواست گمرکی" },
                    //new ServiceRequestType { Id = 12, Name = "Carrier / Agent Follow-up", LocalName = "پیگیری کریر و ایجنت" },
                    //new ServiceRequestType { Id = 13, Name = "Internal Operation Request", LocalName = "درخواست عملیات داخلی" },
                    new ServiceRequestType { Id = 99, Name = "Other", LocalName = "سایر" }
                );
        }



        public class ServiceRequestStatusConfiguration : IEntityTypeConfiguration<ServiceRequestStatus>
        {
            public void Configure(EntityTypeBuilder<ServiceRequestStatus> builder)
            {
                builder.ToTable("ServiceRequestStatus", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
                builder.Property(c => c.LocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new ServiceRequestStatus { Id = 1, Name = "Draft" },
                        new ServiceRequestStatus { Id = 2, Name = "Registered" },
                        new ServiceRequestStatus { Id = 3, Name = "Under Review" },
                        new ServiceRequestStatus { Id = 4, Name = "Waiting For Customer" },
                        new ServiceRequestStatus { Id = 5, Name = "Waiting For Carrier" },
                        new ServiceRequestStatus { Id = 6, Name = "Waiting For Agent" },
                        new ServiceRequestStatus { Id = 7, Name = "Waiting For Internal Approval" },
                        new ServiceRequestStatus { Id = 8, Name = "Approved" },
                        new ServiceRequestStatus { Id = 9, Name = "Rejected" },
                        new ServiceRequestStatus { Id = 10, Name = "In Progress" },
                        new ServiceRequestStatus { Id = 11, Name = "Completed" },
                        new ServiceRequestStatus { Id = 12, Name = "Closed" },
                        new ServiceRequestStatus { Id = 13, Name = "Cancelled" }
                    );
            }
        }



        public class ServiceRequestPriorityConfiguration : IEntityTypeConfiguration<ServiceRequestPriority>
        {
            public void Configure(EntityTypeBuilder<ServiceRequestPriority> builder)
            {
                builder.ToTable("ServiceRequestPriority", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
                builder.Property(c => c.LocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
            new ServiceRequestPriority { Id = 1, Name = "Low" },
            new ServiceRequestPriority { Id = 2, Name = "Normal" },
            new ServiceRequestPriority { Id = 3, Name = "High" },
            new ServiceRequestPriority { Id = 4, Name = "Urgent" },
            new ServiceRequestPriority { Id = 5, Name = "Critical" }
                    );
            }
        }

        public class ReceiptDirectionConfiguration : IEntityTypeConfiguration<ReceiptDirection>
        {
            public void Configure(EntityTypeBuilder<ReceiptDirection> builder)
            {
                builder.ToTable("ReceiptDirections", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.ReceiptDirectionName).IsRequired().HasMaxLength(50);
                builder.Property(c => c.ReceiptDirectionLocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new ReceiptDirection { Id = 1, ReceiptDirectionName = "Issued By Us", ReceiptDirectionLocalName = "صادره توسط ما" },
                        new ReceiptDirection { Id = 2, ReceiptDirectionName = "Received By Us", ReceiptDirectionLocalName = "دریافتی توسط ما" }
                    );
            }
        }

        public class ServiceRequestApprovalTypeConfiguration : IEntityTypeConfiguration<ServiceRequestApprovalType>
        {
            public void Configure(EntityTypeBuilder<ServiceRequestApprovalType> builder)
            {
                builder.ToTable("ServiceRequestApprovalTypes", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
                builder.Property(c => c.LocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new ServiceRequestApprovalType { Id = 1, Name = "Customer Approval" },
                        new ServiceRequestApprovalType { Id = 2, Name = "Carrier Approval" },
                        new ServiceRequestApprovalType { Id = 3, Name = "Agent Approval" },
                        new ServiceRequestApprovalType { Id = 4, Name = "Finance Approval" },
                        new ServiceRequestApprovalType { Id = 5, Name = "Operation Approval" },
                        new ServiceRequestApprovalType { Id = 6, Name = "Management Approval" },
                        new ServiceRequestApprovalType { Id = 99, Name = "Other" }
                    );
            }
        }


        public class ServiceRequestApprovalStatusConfiguration : IEntityTypeConfiguration<ServiceRequestApprovalStatus>
        {
            public void Configure(EntityTypeBuilder<ServiceRequestApprovalStatus> builder)
            {
                builder.ToTable("ServiceRequestApprovalStatus", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
                builder.Property(c => c.LocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new ServiceRequestApprovalStatus { Id = 1, Name = "Pending" },
                        new ServiceRequestApprovalStatus { Id = 2, Name = "Approved" },
                        new ServiceRequestApprovalStatus { Id = 3, Name = "Rejected" },
                        new ServiceRequestApprovalStatus { Id = 4, Name = "Cancelled" },
                        new ServiceRequestApprovalStatus { Id = 5, Name = "Not Required" }
                    );
            }
        }
        public class BLAmendmentFieldTypeConfiguration : IEntityTypeConfiguration<BLAmendmentFieldType>
        {
            public void Configure(EntityTypeBuilder<BLAmendmentFieldType> builder)
            {
                builder.ToTable("BLAmendmentFieldTypes", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
                builder.Property(c => c.LocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new BLAmendmentFieldType { Id = 1, Name = "Shipper" },
                        new BLAmendmentFieldType { Id = 2, Name = "Consignee" },
                        new BLAmendmentFieldType { Id = 3, Name = "Notify Party" },
                        new BLAmendmentFieldType { Id = 4, Name = "Description Of Goods" },
                        new BLAmendmentFieldType { Id = 5, Name = "Gross Weight" },
                        new BLAmendmentFieldType { Id = 6, Name = "Net Weight" },
                        new BLAmendmentFieldType { Id = 7, Name = "Measurement" },
                        new BLAmendmentFieldType { Id = 8, Name = "Marks And Numbers" },
                        new BLAmendmentFieldType { Id = 9, Name = "Package Count" },
                        new BLAmendmentFieldType { Id = 10, Name = "Package Type" },
                        new BLAmendmentFieldType { Id = 11, Name = "Container No" },
                        new BLAmendmentFieldType { Id = 12, Name = "Seal No" },
                        new BLAmendmentFieldType { Id = 13, Name = "Voyage No" },
                        new BLAmendmentFieldType { Id = 14, Name = "Vessel Name" },
                        new BLAmendmentFieldType { Id = 15, Name = "Flight No" },
                        new BLAmendmentFieldType { Id = 16, Name = "Airline Name" },
                        new BLAmendmentFieldType { Id = 17, Name = "Truck No" },
                        new BLAmendmentFieldType { Id = 18, Name = "Driver Name" },
                        new BLAmendmentFieldType { Id = 19, Name = "POL" },
                        new BLAmendmentFieldType { Id = 20, Name = "POD" },
                        new BLAmendmentFieldType { Id = 21, Name = "Final Destination" },
                        new BLAmendmentFieldType { Id = 22, Name = "Place Of Receipt" },
                        new BLAmendmentFieldType { Id = 23, Name = "Place Of Delivery" },
                        new BLAmendmentFieldType { Id = 24, Name = "Freight Term" },
                        new BLAmendmentFieldType { Id = 25, Name = "Freight Prepaid / Collect" },
                        new BLAmendmentFieldType { Id = 26, Name = "Issue Date" },
                        new BLAmendmentFieldType { Id = 27, Name = "Declared Value" },
                        new BLAmendmentFieldType { Id = 28, Name = "Handling Information" },
                        new BLAmendmentFieldType { Id = 29, Name = "Document No" },
                        new BLAmendmentFieldType { Id = 30, Name = "Reference No" },
                        new BLAmendmentFieldType { Id = 99, Name = "Other" }
                    );
            }
        }


        public class DOOperationalModeConfiguration : IEntityTypeConfiguration<DOOperationalMode>
        {
            public void Configure(EntityTypeBuilder<DOOperationalMode> builder)
            {
                builder.ToTable("DOOperationalModes", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.DOOperationalModeName).IsRequired().HasMaxLength(50);
                builder.Property(c => c.DOOperationalModeLocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new DOOperationalMode { Id = 1, DOOperationalModeName = "Own", DOOperationalModeLocalName = "ترخیصیه توسط خود ما صادر میشود" },
                        new DOOperationalMode { Id = 2, DOOperationalModeName = "Third Party", DOOperationalModeLocalName = "صادره توسط طرف ثالث و پیگیری توسط ما" },
                        new DOOperationalMode { Id = 3, DOOperationalModeName = "Agent", DOOperationalModeLocalName = "صدور توسط ما به نمایندگی از اشخاص ثالث" }
                    );
            }
        }

        public class DOReceiveMethodConfiguration : IEntityTypeConfiguration<DOReceiveMethod>
        {
            public void Configure(EntityTypeBuilder<DOReceiveMethod> builder)
            {
                builder.ToTable("DOReceiveMethods", "base");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.DOReceiveMethodName).IsRequired().HasMaxLength(50);
                builder.Property(c => c.DOReceiveMethodLocalName).HasMaxLength(50);

                //Seed Data
                builder.HasData(
                        new DOReceiveMethod { Id = 1, DOReceiveMethodName = "IN Person", DOReceiveMethodLocalName = "حضوری" },
                        new DOReceiveMethod { Id = 2, DOReceiveMethodName = "EMAIL", DOReceiveMethodLocalName = "ایمیل" },
                        new DOReceiveMethod { Id = 3, DOReceiveMethodName = "WEB_SERVICE", DOReceiveMethodLocalName = "حضوری" },
                        new DOReceiveMethod { Id = 4, DOReceiveMethodName = "PORTAL", DOReceiveMethodLocalName = "پورتال" },
                        new DOReceiveMethod { Id = 5, DOReceiveMethodName = "WHATSAPP", DOReceiveMethodLocalName = "واتساپ" },
                        new DOReceiveMethod { Id = 6, DOReceiveMethodName = "TELEGRAM", DOReceiveMethodLocalName = "نلگرام" },
                        new DOReceiveMethod { Id = 7, DOReceiveMethodName = "FAX", DOReceiveMethodLocalName = "فکس" },
                        new DOReceiveMethod { Id = 8, DOReceiveMethodName = "COURIER", DOReceiveMethodLocalName = "پیک صادر کننده" },
                        new DOReceiveMethod { Id = 99, DOReceiveMethodName = "OTHER", DOReceiveMethodLocalName = "سایر" }
                    );
            }
        }
    }
}