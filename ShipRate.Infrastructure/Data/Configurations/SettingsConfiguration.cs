using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Infrastructure.Data.Configurations
{

    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies", "cmp");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AccountingAddress).HasMaxLength(200);
            builder.Property(x => x.CompanyAbb).HasMaxLength(15);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
        }
    }

    public class CompanyRoleConfiguration : IEntityTypeConfiguration<CompanyRole>
    {
        public void Configure(EntityTypeBuilder<CompanyRole> builder)
        {
            builder.ToTable("CompanyRoles", "cmp");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CompanyRoleType).WithMany().HasForeignKey(x => x.CompanyRoleTypeId);
        }
    }

    public class IPGTerminalConfiguration : IEntityTypeConfiguration<IPGTerminal>
    {
        public void Configure(EntityTypeBuilder<IPGTerminal> builder)
        {
            builder.ToTable("IPGTerminals", "cmp");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.IPGTerminalName).HasMaxLength(100);
            builder.Property(x => x.IPGTerminalId).HasMaxLength(50);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class IPGTerminalBankAccountConfiguration : IEntityTypeConfiguration<IPGTerminalBankAccount>
    {
        public void Configure(EntityTypeBuilder<IPGTerminalBankAccount> builder)
        {
            builder.ToTable("IPGTerminalBankAccounts", "cmp");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
            builder.HasOne(x => x.Terminal).WithMany(x => x.IPGAccounts).HasForeignKey(x => x.IPGTerminalId);
        }
    }











    public class NumberingSystemConfiguration : IEntityTypeConfiguration<NumberingSystem>
    {
        public void Configure(EntityTypeBuilder<NumberingSystem> builder)
        {
            builder.ToTable("NumberingSystems", "cmp");
            builder.HasKey(c => c.Id);

            builder.Property(b => b.EntityType).HasMaxLength(100);
            builder.Property(b => b.Prefix).HasMaxLength(20);
            builder.Property(b => b.Suffix).HasMaxLength(20);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.ToTable("SystemSettings", "cmp");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);

            builder.Property(x => x.SettingKey).HasMaxLength(100);
            builder.Property(x => x.SettingValue).HasColumnType("nvarchar(max)").IsRequired(true);
            builder.Property(x => x.Description).HasMaxLength(200);

        }
    }
    public class FormatTypeConfiguration : IEntityTypeConfiguration<FormatType>
    {
        public void Configure(EntityTypeBuilder<FormatType> builder)
        {
            builder.ToTable("FormatTypes", "cmp");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.FormatTypeName).HasMaxLength(50);
            builder.Property(x => x.FormatTypeLocalName).HasMaxLength(50);
        }
    }

    public class CompanyFormatConfiguration : IEntityTypeConfiguration<CompanyFormat>
    {
        public void Configure(EntityTypeBuilder<CompanyFormat> builder)
        {
            builder.ToTable("CompanyFormats", "cmp");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CompanyFormatName).HasMaxLength(100);
            builder.Property(x => x.CompanyFormatLocalName).HasMaxLength(100);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.FormatType).WithMany().HasForeignKey(x => x.FormatTypeId);
            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);
        }
    }


    public class CompanyFormatParamConfiguration : IEntityTypeConfiguration<CompanyFormatParam>
    {
        public void Configure(EntityTypeBuilder<CompanyFormatParam> builder)
        {
            builder.ToTable("CompanyFormatParams", "cmp");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ParamName).HasMaxLength(100);
            builder.Property(x => x.ParamLocalName).HasMaxLength(100);

            builder.Property(x => x.ParamFixedValue)
                .HasColumnType("nvarchar(max)");
            builder.Property(x => x.ParamFormula).HasMaxLength(4000);
            builder.HasOne(x => x.CompanyFormat).WithMany(x => x.Params).HasForeignKey(x => x.CompanyFormatId);
        }
    }



}
