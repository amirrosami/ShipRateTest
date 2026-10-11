using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Rate;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class TariffBookConfiguration : IEntityTypeConfiguration<TariffBook>
    {
        public void Configure(EntityTypeBuilder<TariffBook> builder)
        {
            builder.ToTable("TariffBooks", "tariff");
            builder.HasKey(c => c.Id);


            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(500);

            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.TariffType).WithMany().HasForeignKey(x => x.TariffTypeId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);

        }
    }

    public class TariffRuleConfiguration : IEntityTypeConfiguration<TariffRule>
    {
        public void Configure(EntityTypeBuilder<TariffRule> builder)
        {
            builder.ToTable("TariffRules", "tariff");
            builder.HasKey(c => c.Id);



            builder.Property(x => x.ExtraAttributes).HasMaxLength(2000);

            builder.HasOne(x => x.TariffBook).WithMany(x => x.TariffRules).HasForeignKey(x => x.TariffBookId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.OriginCountry).WithMany().HasForeignKey(x => x.OriginCountryId);
            builder.HasOne(x => x.DestinationCountry).WithMany().HasForeignKey(x => x.DestinationCountryId);

            builder.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId);
            builder.HasOne(x => x.Destination).WithMany().HasForeignKey(x => x.DestinationId);
            builder.HasOne(x => x.CalculationBy).WithMany().HasForeignKey(x => x.CalculationById);
            builder.HasOne(x => x.CalculationType).WithMany().HasForeignKey(x => x.CalculationTypeId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.ScopeType).WithMany().HasForeignKey(x => x.ScopeTypeId);
            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);

            builder.HasOne(x => x.ContainerSize).WithMany().HasForeignKey(x => x.ContainerSizeId);
            builder.HasOne(x => x.ContainerType).WithMany().HasForeignKey(x => x.ContainerTypeId);
            builder.HasOne(x => x.ContainerSizeType).WithMany().HasForeignKey(x => x.ContainerSizeTypeId);

            builder.HasOne(x => x.FullMT).WithMany().HasForeignKey(x => x.FullMTId);
        }
    }

    public class TariffRuleFormulaConfiguration : IEntityTypeConfiguration<TariffRuleFormula>
    {
        public void Configure(EntityTypeBuilder<TariffRuleFormula> builder)
        {
            builder.ToTable("TariffRuleFormulas", "tariff");
            builder.HasKey(c => c.Id);


            builder.Property(x => x.FormulaExpression).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.FormulaDescription).HasMaxLength(2000);

            builder.HasOne(x => x.TariffRule).WithMany().HasForeignKey(x => x.TariffRuleId);
        }
    }
    public class TariffRuleCoverageConfiguration : IEntityTypeConfiguration<TariffRuleCoverage>
    {
        public void Configure(EntityTypeBuilder<TariffRuleCoverage> builder)
        {
            builder.ToTable("TariffRuleCoverages", "tariff");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.CoverageDescription).HasMaxLength(200);
            builder.HasOne(x => x.TariffRule).WithMany().HasForeignKey(x => x.TariffRuleId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }
}
