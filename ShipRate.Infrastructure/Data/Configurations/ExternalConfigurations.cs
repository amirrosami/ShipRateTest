using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Common.ExternalServices.Tax;
using ShipRate.Common.Notifications.Models;
using ShipRate.Common.Report;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", "sec");
            builder.HasKey(c => c.Id);
        }
    }
    public class NotificationAttachmentConfiguration : IEntityTypeConfiguration<NotificationAttachment>
    {
        public void Configure(EntityTypeBuilder<NotificationAttachment> builder)
        {
            builder.ToTable("NotificationAttachments", "sec");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Notification)
               .WithMany(x => x.Attachments)
               .HasForeignKey(x => x.NotificationId);
        }
    }
    public class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
    {
        public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
        {
            builder.ToTable("NotificationRecipients", "sec");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Notification)
            .WithMany(x => x.NotificationRecipients)
            .HasForeignKey(x => x.NotificationId);

        }
    }
    public class TaxInvoiceConfiguration : IEntityTypeConfiguration<TaxInvoice>
    {
        public void Configure(EntityTypeBuilder<TaxInvoice> builder)
        {
            builder.ToTable("TaxInvoices", "sec");
            builder.HasKey(c => c.Id);
        }
    }
    public class TaxInvoiceDtlConfiguration : IEntityTypeConfiguration<TaxInvoiceDtl>
    {
        public void Configure(EntityTypeBuilder<TaxInvoiceDtl> builder)
        {
            builder.ToTable("TaxInvoiceDtls", "sec");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Invoice)
            .WithMany(x => x.body)
            .HasForeignKey(x => x.InvoiceId);

        }
    }
    public class TaxInvoicePaymentConfiguration : IEntityTypeConfiguration<TaxInvoicePayment>
    {
        public void Configure(EntityTypeBuilder<TaxInvoicePayment> builder)
        {
            builder.ToTable("TaxInvoicePayments", "sec");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Invoice)
                .WithMany(x => x.payment)
                .HasForeignKey(x => x.InvoiceId);

        }
    }
    public class TaxInvoiceShippingGoodConfiguration : IEntityTypeConfiguration<TaxInvoiceShippingGood>
    {
        public void Configure(EntityTypeBuilder<TaxInvoiceShippingGood> builder)
        {
            builder.ToTable("TaxInvoiceShippingGoods", "sec");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Invoice).WithMany(x => x.sg).HasForeignKey(x => x.InvoiceId);

        }
    }
    public class ReportConfigConfiguration : IEntityTypeConfiguration<ReportConfig>
    {
        public void Configure(EntityTypeBuilder<ReportConfig> builder)
        {
            builder.ToTable("ReportConfigs", "sec");
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.ReportName).IsUnique();
        }
    }

}

