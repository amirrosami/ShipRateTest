using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.PettyCashControl;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.ToTable("BankAccounts", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.BankAccountTitle).IsRequired().HasMaxLength(100);
            builder.Property(x => x.BankAccountTitlePersian).IsRequired().HasMaxLength(100);
            builder.Property(x => x.AccountNumber).HasMaxLength(40);
            builder.Property(x => x.IBAN).HasMaxLength(40);
            builder.Property(x => x.BranchNo).HasMaxLength(10);
            builder.Property(x => x.BranchName).HasMaxLength(100);
            builder.Property(x => x.BranchAddress).HasMaxLength(200);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Bank)
                    .WithMany()
                    .HasForeignKey(x => x.BankId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BankAccountPurpose)
                    .WithMany()
                    .HasForeignKey(x => x.BankAccountPurposeId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Person).WithMany(x => x.BankAccounts).HasForeignKey(x => x.PersonId);
        }
    }
    public class ChequePurposeConfiguration : IEntityTypeConfiguration<ChequePurpose>
    {
        public void Configure(EntityTypeBuilder<ChequePurpose> builder)
        {
            builder.ToTable("ChequePurposes", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ChequePurposeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ChequePurposeLocalName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new ChequePurpose { Id = 1, ChequePurposeName = "Commercial", ChequePurposeLocalName = "عادی/جهت تسویه بدهی و فاکتور" },
                new ChequePurpose { Id = 2, ChequePurposeName = "Guarantee", ChequePurposeLocalName = "ضمانت / امانی" }
                );
        }
    }
    public class ChequeConfiguration : IEntityTypeConfiguration<Cheque>
    {
        public void Configure(EntityTypeBuilder<Cheque> builder)
        {
            builder.ToTable("Cheques", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ChequeNo).IsRequired().HasMaxLength(20);
            builder.Property(x => x.BankBranchCode).HasMaxLength(10);
            builder.Property(x => x.BankBranchName).HasMaxLength(40);
            builder.Property(x => x.BankAccountNo).HasMaxLength(35);
            builder.Property(x => x.IBAN).HasMaxLength(30);
            builder.Property(x => x.SayadNo).HasMaxLength(30);
            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Bank).WithMany().HasForeignKey(x => x.BankId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.ChequeType).WithMany().HasForeignKey(x => x.ChequeTypeId);
            builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
            builder.HasOne(x => x.ChequePurpose).WithMany().HasForeignKey(x => x.ChequePurposeId);
        }
    }

    public class ChequeReceiptConfiguration : IEntityTypeConfiguration<ChequeReceipt>
    {
        public void Configure(EntityTypeBuilder<ChequeReceipt> builder)
        {
            builder.ToTable("ChequeReceipts", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Cheque).WithMany().HasForeignKey(x => x.ChequeId);
        }
    }

    public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
    {
        public void Configure(EntityTypeBuilder<Charge> builder)
        {
            builder.ToTable("Charges", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ChargeDescription).HasMaxLength(300);
            builder.Property(x => x.Remarks).HasMaxLength(100);

            builder.Property(x => x.UnitRate).HasPrecision(18, 6);
            builder.Property(x => x.Amount).HasPrecision(18, 6);
            builder.Property(x => x.BaseAmount).HasPrecision(18, 6);
            builder.Property(x => x.GrossAmount).HasPrecision(18, 6);
            builder.Property(x => x.TaxAmount).HasPrecision(18, 6);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Booking).WithMany(x => x.Charges).HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.BookingLeg).WithMany(x => x.Charges).HasForeignKey(x => x.BookingLegId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.CalculationBy).WithMany().HasForeignKey(x => x.CalculationById);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.BaseCurrency).WithMany().HasForeignKey(x => x.BaseCurrencyId);
            builder.HasOne(x => x.ApprovedBy).WithMany().HasForeignKey(x => x.ApprovedById);
            builder.HasOne(x => x.FinanceDirection).WithMany().HasForeignKey(x => x.DirectionId);
        }
    }



    public class PaymentDocumentConfiguration : IEntityTypeConfiguration<PaymentDocument>
    {
        public void Configure(EntityTypeBuilder<PaymentDocument> builder)
        {
            builder.ToTable("PaymentDocuments", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ReferenceNo).HasMaxLength(70);
            builder.Property(x => x.Description).HasMaxLength(300);
            //builder.Property(e => e.SystemId).ValueGeneratedOnAdd().UseIdentityColumn(seed: 100000, increment: 1);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);

            builder.HasOne(x => x.Bank).WithMany().HasForeignKey(x => x.BankId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.FinanceDirection).WithMany().HasForeignKey(x => x.FinanceDirectionId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
            builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId);
            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.EditUser).WithMany().HasForeignKey(x => x.EditUserId);

            builder.Property(x => x.SystemId).HasDefaultValueSql("NEXT VALUE FOR dbo.Seq_PaymentSystemId");
            builder.HasIndex(d => d.SystemId).IsUnique();

        }
    }

    public class PaymentDocumentExtraInfoConfiguration : IEntityTypeConfiguration<PaymentDocumentExtraInfo>
    {
        public void Configure(EntityTypeBuilder<PaymentDocumentExtraInfo> builder)
        {
            builder.ToTable("PaymentDocumentExtraInfos", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ExtraInfoKey).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ExtraInfoValue).IsRequired().HasMaxLength(200);
        }
    }

    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.ToTable("ExchangeRates", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ExchangeRateSource).HasMaxLength(50);
            builder.HasOne(x => x.FromCurrency).WithMany().HasForeignKey(x => x.FromCurrencyId);
            builder.HasOne(x => x.ToCurrency).WithMany().HasForeignKey(x => x.ToCurrencyId);
            builder.HasOne(x => x.ExchangeRateBase).WithMany().HasForeignKey(x => x.ExchangeRateBaseId);
        }
    }

    public class InvoiceTypeConfiguration : IEntityTypeConfiguration<InvoiceType>
    {
        public void Configure(EntityTypeBuilder<InvoiceType> builder)
        {
            builder.ToTable("InvoiceTypes", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InvoiceTypeName).HasMaxLength(50);
            builder.Property(x => x.InvoiceTypeCode).HasMaxLength(20);


            //Seed Data
            builder.HasData(
                new InvoiceType { Id = 1, InvoiceTypeCode = "SALES", InvoiceTypeName = "Sales Invoice" },
                new InvoiceType { Id = 2, InvoiceTypeCode = "PURCHASE", InvoiceTypeName = "Purchase Invoice" },
                new InvoiceType { Id = 3, InvoiceTypeCode = "CREDIT_NOTE", InvoiceTypeName = "Credit Note" },
                new InvoiceType { Id = 4, InvoiceTypeCode = "DEBIT_NOTE", InvoiceTypeName = "Debit Note" },
                new InvoiceType { Id = 5, InvoiceTypeCode = "PROFORMA", InvoiceTypeName = "Proforma Invoice" }
                );


        }
    }

    public class ReturnCashRequestConfiguration : IEntityTypeConfiguration<ReturnCashRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnCashRequest> builder)
        {
            builder.ToTable("ReturnCashRequests", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ReqNo).IsRequired().HasMaxLength(20);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);
            builder.HasOne(x => x.PaymentDocument).WithMany().HasForeignKey(x => x.PaymentDocumentId);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.ReturnCashStatus).WithMany().HasForeignKey(x => x.ReturnCashStatusId);

        }
    }
    public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
    {
        public void Configure(EntityTypeBuilder<Settlement> builder)
        {
            builder.ToTable("Settlements", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).HasMaxLength(500);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);

            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.EditUser).WithMany().HasForeignKey(x => x.EditUserId);
            builder.Property(x => x.SettlementNo).HasDefaultValueSql("NEXT VALUE FOR dbo.Seq_SettlementNo");

        }
    }

    public class SettlementItemConfiguration : IEntityTypeConfiguration<SettlementItem>
    {
        public void Configure(EntityTypeBuilder<SettlementItem> builder)
        {
            builder.ToTable("SettlementItems", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(100);
            builder.HasOne(x => x.Settlement).WithMany(x => x.Items).HasForeignKey(x => x.SettlementId);
            builder.HasOne(x => x.Invoice).WithMany(x => x.SettlementItems).HasForeignKey(x => x.InvoiceId);
            builder.HasOne(x => x.InvoiceItem).WithMany(x => x.SettlementItems).HasForeignKey(x => x.InvoiceItemId);
            builder.HasOne(x => x.Charge).WithMany(x => x.SettlementItems).HasForeignKey(x => x.ChargeId);
            builder.HasOne(x => x.Detention).WithMany(x => x.SettlementItems).HasForeignKey(x => x.DetentionId);
        }
    }
    public class SettlementItemPaymentAllocationConfiguration : IEntityTypeConfiguration<SettlementItemPaymentAllocation>
    {
        public void Configure(EntityTypeBuilder<SettlementItemPaymentAllocation> builder)
        {
            builder.ToTable("SettlementItemPaymentAllocations", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Settlement).WithMany(x => x.PaymentAllocations).HasForeignKey(x => x.SettlementId);
            builder.HasOne(x => x.SettlementItem).WithMany(x => x.PaymentAllocations).HasForeignKey(x => x.SettlementItemId);
            builder.HasOne(x => x.PaymentDocument).WithMany(x => x.SettlementItemAllocations).HasForeignKey(x => x.PaymentDocumentId);
            builder.HasOne(x => x.ExchangeRateBase).WithMany().HasForeignKey(x => x.ExchangeRateBaseId);
            builder.HasOne(x => x.ExchangeRateEntity).WithMany().HasForeignKey(x => x.ExchangeRateId);
        }
    }

    public class VaultConfiguration : IEntityTypeConfiguration<Vault>
    {
        public void Configure(EntityTypeBuilder<Vault> builder)
        {
            builder.ToTable("Vaults", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.VaultName).HasMaxLength(150).IsRequired(); builder.Property(x => x.Remarks).HasMaxLength(500);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.KeeperUser).WithMany().HasForeignKey(x => x.KeeperUserId).OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class VaultAssetConfiguration : IEntityTypeConfiguration<VaultAsset>
    {
        public void Configure(EntityTypeBuilder<VaultAsset> builder)
        {
            builder.ToTable("VaultAssets", "fin");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Vault).WithMany(x => x.Assets).HasForeignKey(x => x.VaultId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

            builder.HasIndex(x => new { x.VaultId, x.CurrencyId }).IsUnique();

        }
    }

    public class VaultMovementConfiguration : IEntityTypeConfiguration<VaultMovement>
    {
        public void Configure(EntityTypeBuilder<VaultMovement> builder)
        {
            builder.ToTable("VaultMovements", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ReferenceNo).HasMaxLength(60);
            builder.Property(x => x.DepositorOrReceiver).HasMaxLength(150);
            builder.Property(x => x.Remarks).HasMaxLength(500);

            builder.HasOne(x => x.Vault).WithMany(x => x.Movements).HasForeignKey(x => x.VaultId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.ExecutedByUser).WithMany().HasForeignKey(x => x.ExecutedByUserId);
            builder.HasOne(x => x.SourceType).WithMany().HasForeignKey(x => x.VaultMovementSourceTypeId);
            builder.HasOne(x => x.InOut).WithMany().HasForeignKey(x => x.InOutId);
            builder.HasIndex(x => x.MovementDate);
            builder.HasIndex(x => new { x.VaultId, x.CurrencyId });
        }
    }
    public class CashPaymentRequestConfiguration : IEntityTypeConfiguration<CashPaymentRequest>
    {
        public void Configure(EntityTypeBuilder<CashPaymentRequest> builder)
        {
            builder.ToTable("CashPaymentRequests", "fin");
            builder.HasKey(c => c.Id);

            builder.HasIndex(x => x.RequestNo).IsUnique();
            builder.Property(x => x.RequestNo).HasMaxLength(30).IsRequired();
            builder.Property(x => x.PayeeName).HasMaxLength(150);
            builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RejectReason).HasMaxLength(500);
            builder.Property(x => x.Amount).HasPrecision(18, 4);

            builder.HasOne(x => x.Vault).WithMany().HasForeignKey(x => x.VaultId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.InitialConfirmUser).WithMany().HasForeignKey(x => x.InitialConfirmedByUserId);
            builder.HasOne(x => x.FinalConfirmUser).WithMany().HasForeignKey(x => x.FinalConfirmedByUserId);
            builder.HasOne(x => x.PaidByUser).WithMany().HasForeignKey(x => x.PaidByUserId);

            builder.HasOne(x => x.CashPaymentRequestStatus).WithMany().HasForeignKey(x => x.StatusId);
        }
    }


    public class VaultItemConfiguration : IEntityTypeConfiguration<VaultItem>
    {
        public void Configure(EntityTypeBuilder<VaultItem> builder)
        {

            builder.ToTable("VaultItems", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VaultItemCategoryId).HasConversion<int>();


            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.IdentifierNo).HasMaxLength(100);
            builder.Property(x => x.OwnerOrBeneficiary).HasMaxLength(200);
            builder.Property(x => x.NominalValue).HasPrecision(18, 4);
            builder.Property(x => x.ShelfLocation).HasMaxLength(100);
            builder.Property(x => x.CurrentHolderExternalName).HasMaxLength(200);
            builder.Property(x => x.AttachmentPath).HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.HasIndex(x => x.ExpiryDate);
            builder.HasIndex(x => x.VaultItemStatusId);

            builder.HasOne(x => x.Vault).WithMany(x => x.Items).HasForeignKey(x => x.VaultId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.VaultItemCategory).WithMany().HasForeignKey(x => x.VaultItemCategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.VaultItemStatus).WithMany().HasForeignKey(x => x.VaultItemStatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CurrentHolderUser).WithMany().HasForeignKey(x => x.CurrentHolderUserId).OnDelete(DeleteBehavior.SetNull);
        }
    }


    public class VaultItemCustodyLogConfiguration : IEntityTypeConfiguration<VaultItemCustodyLog>
    {
        public void Configure(EntityTypeBuilder<VaultItemCustodyLog> builder)
        {

            builder.ToTable("VaultItemCustodyLogs", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VaultCustodyActionTypeId).HasConversion<int>();

            builder.Property(x => x.HandedToExternalName).HasMaxLength(200);
            builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            builder.Property(x => x.HandoverReceiptScanPath).HasMaxLength(500);
            builder.HasIndex(x => x.ActionDate);

            builder.HasOne(x => x.VaultItem).WithMany(x => x.CustodyLogs)
                .HasForeignKey(x => x.VaultItemId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.ExecutedByUser).WithMany().HasForeignKey(x => x.ExecutedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.HandedToUser).WithMany().HasForeignKey(x => x.HandedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.VaultCustodyActionType).WithMany()
                .HasForeignKey(x => x.VaultCustodyActionTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }



    public class PettyCashConfiguration : IEntityTypeConfiguration<PettyCash>
    {
        public void Configure(EntityTypeBuilder<PettyCash> builder)
        {
            builder.ToTable("PettyCashes", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CustodianUser).WithMany().HasForeignKey(x => x.CustodianUserId);
        }
    }

    public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
    {
        public void Configure(EntityTypeBuilder<CostCenter> builder)
        {
            builder.ToTable("CostCenters", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }
    public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
    {
        public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
        {
            builder.ToTable("ExpenseCategory", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AccountCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class PettyCashExpenseConfiguration : IEntityTypeConfiguration<PettyCashExpense>
    {
        public void Configure(EntityTypeBuilder<PettyCashExpense> builder)
        {
            builder.ToTable("PettyCashExpenses", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.PettyCash).WithMany(x => x.Expenses).HasForeignKey(x => x.PettyCashId);
            builder.HasOne(x => x.ExpenseCategory).WithMany().HasForeignKey(x => x.ExpenseCategoryId);
            builder.HasOne(x => x.CostCenter).WithMany().HasForeignKey(x => x.CostCenterId);
            builder.HasOne(x => x.ApprovedByUser).WithMany().HasForeignKey(x => x.ApprovedByUserId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
        }
    }

    public class PettyCashRechargeConfiguration : IEntityTypeConfiguration<PettyCashRecharge>
    {
        public void Configure(EntityTypeBuilder<PettyCashRecharge> builder)
        {
            builder.ToTable("PettyCashRecharges", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
            //builder.Property(x => x.DocumentNumber).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.PettyCash).WithMany(x => x.Recharges).HasForeignKey(x => x.PettyCashId);

            builder.HasOne(x => x.ApprovedByUser).WithMany().HasForeignKey(x => x.ApprovedByUserId);
            builder.HasOne(x => x.RejectedByUser).WithMany().HasForeignKey(x => x.RejectedByUserId);
            builder.HasOne(x => x.ChargedByUser).WithMany().HasForeignKey(x => x.ChargedByUserId);


        }
    }
    public class InvoiceFNFConfiguration : IEntityTypeConfiguration<InvoiceFNF>
    {
        public void Configure(EntityTypeBuilder<InvoiceFNF> builder)
        {
            builder.ToTable("InvoiceFNF", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.FNFName).HasMaxLength(50);

            //Seed Data 
            builder.HasData(
                new InvoiceFNF { Id = 1, FNFName = "F" },
                new InvoiceFNF { Id = 2, FNFName = "NF" }
            );
        }
    }
    public class InvoiceSubjectConfiguration : IEntityTypeConfiguration<InvoiceSubject>
    {
        public void Configure(EntityTypeBuilder<InvoiceSubject> builder)
        {
            builder.ToTable("InvoiceSubjects", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.InvoiceSubjectName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.InvoiceSubjectLocalName).IsRequired().HasMaxLength(100);

            //Seed Data 
            builder.HasData(
                new InvoiceSubject { Id = 1, InvoiceSubjectName = "Sales Invoice", InvoiceSubjectLocalName = "فروش کالا و خدمات" },
                new InvoiceSubject { Id = 2, InvoiceSubjectName = "Freight Invoice", InvoiceSubjectLocalName = "صورتحساب کرایه حمل" },
                new InvoiceSubject { Id = 3, InvoiceSubjectName = "Detention Invoice", InvoiceSubjectLocalName = "صورتحساب دیرکرد کانتینر" },
                new InvoiceSubject { Id = 4, InvoiceSubjectName = "DO Invoice", InvoiceSubjectLocalName = "صورتحساب هزینه های محلی" },
                new InvoiceSubject { Id = 10, InvoiceSubjectName = "Warranty Invoice", InvoiceSubjectLocalName = "صورتحساب سپرده ها" }
            );
        }
    }

    public class InvoiceModeConfiguration : IEntityTypeConfiguration<InvoiceModel>
    {
        public void Configure(EntityTypeBuilder<InvoiceModel> builder)
        {
            builder.ToTable("InvoiceModels", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.InvoiceModelName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.InvoiceModelLocalName).IsRequired().HasMaxLength(50);

            //Seed Data 
            builder.HasData(
                new InvoiceModel { Id = 1, InvoiceModelName = "Invoice", InvoiceModelLocalName = "فاکتور" },
                new InvoiceModel { Id = 2, InvoiceModelName = "PreInvoice", InvoiceModelLocalName = "پیش فاکتور" }
            );
        }
    }



    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InvoiceNo).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.Property(x => x.InvoiceSubjectTitle).HasMaxLength(100);
            builder.Property(x => x.SerialPrefix).HasMaxLength(5);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);

            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
            builder.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
            builder.HasOne(x => x.BL).WithMany().HasForeignKey(x => x.BLId);

            builder.HasOne(x => x.Detention).WithMany().HasForeignKey(x => x.DetentionId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
            builder.HasOne(x => x.InvoiceStatus).WithMany().HasForeignKey(x => x.InvoiceStatusId);
            builder.HasOne(x => x.InvoiceType).WithMany().HasForeignKey(x => x.InvoiceTypeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);

            builder.HasOne(x => x.ApprovedBy).WithMany().HasForeignKey(x => x.ApprovedById);
            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.EditUserId);

            builder.HasOne(x => x.FinanceDirection).WithMany().HasForeignKey(x => x.FinanceDirectionId);
            builder.HasOne(x => x.InvoiceFNF).WithMany().HasForeignKey(x => x.InvoiceFNFId);
            builder.HasOne(x => x.InvoiceSubject).WithMany().HasForeignKey(x => x.InvoiceSubjectId);
            builder.HasOne(x => x.InvoiceModel).WithMany().HasForeignKey(x => x.InvoiceModelId);

            builder.HasIndex(x => new { x.SerialPrefix, x.Serial, x.CompanyId })
                       .HasFilter("[IsDeleted] = 0")
                       .IsUnique()
                       .HasDatabaseName("IX_Invoices_SerialPrefix_Serial");
        }
    }

    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable("InvoiceItems", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ChargeTypeSupplementRemarks).HasMaxLength(100);

            builder.Property(x => x.QTY).HasPrecision(18, 4);
            builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
            builder.Property(x => x.TotalAmount).HasPrecision(18, 4);
            builder.Property(x => x.TaxAmount).HasPrecision(18, 4);

            builder.HasOne(x => x.Invoice).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.InvoiceId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.Charge).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.ChargeId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }

    public class CustomsPaymentConfiguration : IEntityTypeConfiguration<CustomsPayment>
    {
        public void Configure(EntityTypeBuilder<CustomsPayment> builder)
        {
            builder.ToTable("CustomsPayments", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PaymentType).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.CustomsClearance).WithMany().HasForeignKey(x => x.CustomsClearanceId);
            builder.HasOne(x => x.Settlement).WithMany().HasForeignKey(x => x.SettlementId);
            builder.HasOne(x => x.PaymentDocument).WithMany().HasForeignKey(x => x.PaymentDocumentId);
            builder.HasOne(x => x.PettyCash).WithMany().HasForeignKey(x => x.PettyCashId);
        }
    }

    public class TreasuryDocumentConfiguration : IEntityTypeConfiguration<TreasuryDocument>
    {
        public void Configure(EntityTypeBuilder<TreasuryDocument> builder)
        {
            builder.ToTable("TreasuryDocuments", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).HasMaxLength(400);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.TreasuryDocumentType).WithMany().HasForeignKey(x => x.TreasuryDocumentTypeId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.TreasuryDocumentStatusId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Party).WithMany().HasForeignKey(x => x.PartyId);
        }
    }

    public class TreasuryDocumentItemConfiguration : IEntityTypeConfiguration<TreasuryDocumentItem>
    {
        public void Configure(EntityTypeBuilder<TreasuryDocumentItem> builder)
        {
            builder.ToTable("TreasuryDocumentItems", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Description).HasMaxLength(400);

            builder.HasOne(x => x.TreasuryDocument).WithMany(x => x.TreasuryDocumentItems).HasForeignKey(x => x.TreasuryDocumentId);
            builder.HasOne(x => x.PaymentDocument).WithMany().HasForeignKey(x => x.PaymentDocumentId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }

    public class CreditDebitAccTypeConfiguration : IEntityTypeConfiguration<CreditDebitAccType>
    {
        public void Configure(EntityTypeBuilder<CreditDebitAccType> builder)
        {
            builder.ToTable("CreditDebitAccTypes", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AccTypeName).IsRequired().HasMaxLength(100);

            //Seed Data
            builder.HasData(
                new CreditDebitAccType { Id = 1, AccTypeName = "Fixed Account Code" },
                new CreditDebitAccType { Id = 2, AccTypeName = "Customer Account Code" }
                );

        }
    }


    public class ChargeTypeCreditDebitConfiguration : IEntityTypeConfiguration<ChargeTypeCreditDebit>
    {
        public void Configure(EntityTypeBuilder<ChargeTypeCreditDebit> builder)
        {
            builder.ToTable("ChargeTypeCreditDebits", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Buy_Credit_AccCode).HasMaxLength(20);
            builder.Property(x => x.Buy_Debit_AccCode).HasMaxLength(20);
            builder.Property(x => x.Sell_Credit_AccCode).HasMaxLength(20);
            builder.Property(x => x.Sell_Debit_AccCode).HasMaxLength(20);

            builder.HasOne(x => x.Buy_Debit_AccType).WithMany().HasForeignKey(x => x.Buy_Debit_AccTypeId);
            builder.HasOne(x => x.Buy_Credit_AccType).WithMany().HasForeignKey(x => x.Buy_Credit_AccTypeId);

            builder.HasOne(x => x.Sell_Debit_AccType).WithMany().HasForeignKey(x => x.Sell_Debit_AccTypeId);
            builder.HasOne(x => x.Sell_Credit_AccType).WithMany().HasForeignKey(x => x.Sell_Credit_AccTypeId);

        }
    }


    public class OnlinePaymentStatusConfiguration : IEntityTypeConfiguration<OnlinePaymentStatus>
    {
        public void Configure(EntityTypeBuilder<OnlinePaymentStatus> builder)
        {
            builder.ToTable("OnlinePaymentStatus", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.OnlinePaymentStatusName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.OnlinePaymentStatusLocalName).HasMaxLength(100);

            //Seed Data
            builder.HasData(
                new OnlinePaymentStatus { Id = 10, OnlinePaymentStatusName = "Draft" },
                new OnlinePaymentStatus { Id = 20, OnlinePaymentStatusName = "Requested" },
                new OnlinePaymentStatus { Id = 30, OnlinePaymentStatusName = "Verified" },
                new OnlinePaymentStatus { Id = 40, OnlinePaymentStatusName = "Finally Confirmed" },
                new OnlinePaymentStatus { Id = 50, OnlinePaymentStatusName = "Settled" },
                new OnlinePaymentStatus { Id = 80, OnlinePaymentStatusName = "Rejected" },
                new OnlinePaymentStatus { Id = 90, OnlinePaymentStatusName = "Canceled" }
                );
        }
    }




    public class OnlinePaymentConfiguration : IEntityTypeConfiguration<OnlinePayment>
    {
        public void Configure(EntityTypeBuilder<OnlinePayment> builder)
        {
            builder.ToTable("OnlinePayments", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.BankReferenceNo).HasMaxLength(60);
            builder.Property(x => x.TraceNo).HasMaxLength(60);

            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Bank).WithMany().HasForeignKey(x => x.BankId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.Terminal).WithMany().HasForeignKey(x => x.IPGTerminalId);

        }
    }

    public class OnlinePaymentItemConfiguration : IEntityTypeConfiguration<OnlinePaymentItem>
    {
        public void Configure(EntityTypeBuilder<OnlinePaymentItem> builder)
        {
            builder.ToTable("OnlinePaymentItems", "fin");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.OnlinePayment).WithMany(x => x.Items).HasForeignKey(x => x.OnlinePaymentId);
            builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId);
            builder.HasOne(x => x.Charge).WithMany().HasForeignKey(x => x.ChargeId);

        }
    }

    public class OnlinePaymentBankAccountConfiguration : IEntityTypeConfiguration<OnlinePaymentBankAccount>
    {
        public void Configure(EntityTypeBuilder<OnlinePaymentBankAccount> builder)
        {
            builder.ToTable("OnlinePaymentBankAccounts", "fin");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.OnlinePayment).WithMany(x => x.Accounts).HasForeignKey(x => x.OnlinePaymentId);
            builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
        }
    }
    public class AccountCodeConfiguration : IEntityTypeConfiguration<AccountCode>
    {
        public void Configure(EntityTypeBuilder<AccountCode> builder)
        {
            builder.ToTable("AccountCodes", "fin");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.AccCode).HasMaxLength(20);
            builder.Property(x => x.BuyAccCode).HasMaxLength(20);
            builder.Property(x => x.SellAccCode).HasMaxLength(20);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.ChargeType).WithMany().HasForeignKey(x => x.ChargeTypeId);
            builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.UpdateUser).WithMany().HasForeignKey(x => x.EditUserId);

        }
    }
    public class VaultMovementSourceTypeConfiguration : IEntityTypeConfiguration<VaultMovementSourceType>
    {
        public void Configure(EntityTypeBuilder<VaultMovementSourceType> builder)
        {
            builder.ToTable("VaultMovementSourceTypes", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VaultMovementSourceTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.VaultMovementSourceTypeLocalName).IsRequired().HasMaxLength(100);

            builder.HasData(
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.CustomerReceipt, VaultMovementSourceTypeName = "CustomerReceipt", VaultMovementSourceTypeLocalName = "دریافت نقدی از مشتری" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.BankToVault, VaultMovementSourceTypeName = "BankToVault", VaultMovementSourceTypeLocalName = "شارژ از بانک" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.PettyCashSettlement, VaultMovementSourceTypeName = "PettyCashSettlement", VaultMovementSourceTypeLocalName = "عودت تنخواه" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.InterVaultTransferIn, VaultMovementSourceTypeName = "InterVaultTransferIn", VaultMovementSourceTypeLocalName = "انتقال از گاوصندوق دیگر" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.DepositRefundIn, VaultMovementSourceTypeName = "DepositRefundIn", VaultMovementSourceTypeLocalName = "استرداد سپرده" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.PhysicalSurplus, VaultMovementSourceTypeName = "PhysicalSurplus", VaultMovementSourceTypeLocalName = "مازاد شمارش فیزیکی" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.CapitalInjection, VaultMovementSourceTypeName = "CapitalInjection", VaultMovementSourceTypeLocalName = "تزریق سرمایه" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.CashPaymentRequest, VaultMovementSourceTypeName = "CashPaymentRequest", VaultMovementSourceTypeLocalName = "درخواست پرداخت نقدی" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.VaultToBank, VaultMovementSourceTypeName = "VaultToBank", VaultMovementSourceTypeLocalName = "واریز به بانک" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.PettyCashAdvance, VaultMovementSourceTypeName = "PettyCashAdvance", VaultMovementSourceTypeLocalName = "پرداخت علی‌الحساب تنخواه" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.InterVaultTransferOut, VaultMovementSourceTypeName = "InterVaultTransferOut", VaultMovementSourceTypeLocalName = "انتقال به گاوصندوق دیگر" },
                            new VaultMovementSourceType { Id = (int)EnumVaultMovementSourceType.PhysicalShortage, VaultMovementSourceTypeName = "PhysicalShortage", VaultMovementSourceTypeLocalName = "کسری شمارش فیزیکی" }
                );
        }
    }
    public class CashPaymentRequestStatusConfiguration : IEntityTypeConfiguration<CashPaymentRequestStatus>
    {
        public void Configure(EntityTypeBuilder<CashPaymentRequestStatus> builder)
        {
            builder.ToTable("CashPaymentRequestStatus", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CashPaymentRequestStatusName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CashPaymentRequestStatusLocalName).IsRequired().HasMaxLength(100);

            builder.HasData(
                            new CashPaymentRequestStatus { Id = (int)EnumCashPaymentRequestStatus.Pending, CashPaymentRequestStatusName = "Pending", CashPaymentRequestStatusLocalName = "در انتظار پرداخت" },
                            new CashPaymentRequestStatus { Id = (int)EnumCashPaymentRequestStatus.Paid, CashPaymentRequestStatusName = "Paid", CashPaymentRequestStatusLocalName = "پرداخت شده" },
                            new CashPaymentRequestStatus { Id = (int)EnumCashPaymentRequestStatus.Rejected, CashPaymentRequestStatusName = "Rejected", CashPaymentRequestStatusLocalName = "رد شده" },
                            new CashPaymentRequestStatus { Id = (int)EnumCashPaymentRequestStatus.Cancelled, CashPaymentRequestStatusName = "Cancelled", CashPaymentRequestStatusLocalName = "لغو شده" },
                            new CashPaymentRequestStatus { Id = (int)EnumCashPaymentRequestStatus.InitialConfirmed, CashPaymentRequestStatusName = "Initial Confirmed", CashPaymentRequestStatusLocalName = "تایید اولیه شده" },
                            new CashPaymentRequestStatus { Id = (int)EnumCashPaymentRequestStatus.FinalConfirmed, CashPaymentRequestStatusName = "Final Confirmed", CashPaymentRequestStatusLocalName = "تایید نهایی شده" }
        );
        }
    }
    public class VaultItemCategoryConfiguration : IEntityTypeConfiguration<VaultItemCategory>
    {
        public void Configure(EntityTypeBuilder<VaultItemCategory> builder)
        {
            builder.ToTable("VaultItemCategory", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VaultItemCategoryName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.VaultItemCategoryLocalName).IsRequired().HasMaxLength(100);

            builder.HasData(
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.TitleDeed, VaultItemCategoryName = "TitleDeed", VaultItemCategoryLocalName = "سند مالکیت ملکی" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.VesselDocument, VaultItemCategoryName = "VesselDocument", VaultItemCategoryLocalName = "اسناد شناور" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.OriginalBL, VaultItemCategoryName = "OriginalBL", VaultItemCategoryLocalName = "بارنامه کاغذی اصل" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.BankGuarantee, VaultItemCategoryName = "BankGuarantee", VaultItemCategoryLocalName = "ضمانت‌نامه بانکی" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.PromissoryNote, VaultItemCategoryName = "PromissoryNote", VaultItemCategoryLocalName = "سفته ضمانت" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.GuaranteeCheque, VaultItemCategoryName = "GuaranteeCheque", VaultItemCategoryLocalName = "چک ضمانتی" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.CorporateCertificate, VaultItemCategoryName = "CorporateCertificate", VaultItemCategoryLocalName = "اسناد شرکتی" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.HardwareDevice, VaultItemCategoryName = "HardwareDevice", VaultItemCategoryLocalName = "توکن و تجهیزات" },
                            new VaultItemCategory { Id = (int)EnumVaultItemCategory.Other, VaultItemCategoryName = "Other", VaultItemCategoryLocalName = "سایر" }
        );
        }
    }
    public class VaultItemStatusConfiguration : IEntityTypeConfiguration<VaultItemStatus>
    {
        public void Configure(EntityTypeBuilder<VaultItemStatus> builder)
        {
            builder.ToTable("VaultItemStatus", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VaultItemStatusName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.VaultItemStatusLocalName).IsRequired().HasMaxLength(100);

            builder.HasData(
                            new VaultItemStatus { Id = (int)EnumVaultItemStatus.InVault, VaultItemStatusName = "InVault", VaultItemStatusLocalName = "موجود در گاوصندوق" },
                            new VaultItemStatus { Id = (int)EnumVaultItemStatus.CheckedOut, VaultItemStatusName = "CheckedOut", VaultItemStatusLocalName = "به امانت رفته" },
                            new VaultItemStatus { Id = (int)EnumVaultItemStatus.PermanentOut, VaultItemStatusName = "PermanentOut", VaultItemStatusLocalName = "خروج قطعی" },
                            new VaultItemStatus { Id = (int)EnumVaultItemStatus.Archived, VaultItemStatusName = "Archived", VaultItemStatusLocalName = "بایگانی راکد" }
        );
        }
    }

    public class VaultCustodyActionTypeConfiguration : IEntityTypeConfiguration<VaultCustodyActionType>
    {
        public void Configure(EntityTypeBuilder<VaultCustodyActionType> builder)
        {
            builder.ToTable("VaultCustodyActionTypes", "fin");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.VaultCustodyActionTypeName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.VaultCustodyActionTypeLocalName).IsRequired().HasMaxLength(100);

            builder.HasData(
            new VaultCustodyActionType { Id = (int)EnumVaultCustodyActionType.InitialDeposit, VaultCustodyActionTypeName = "InitialDeposit", VaultCustodyActionTypeLocalName = "ورود اولیه به صندوق" },
            new VaultCustodyActionType { Id = (int)EnumVaultCustodyActionType.BorrowCheckOut, VaultCustodyActionTypeName = "BorrowCheckOut", VaultCustodyActionTypeLocalName = "تحویل امانی" },
            new VaultCustodyActionType { Id = (int)EnumVaultCustodyActionType.ReturnCheckIn, VaultCustodyActionTypeName = "ReturnCheckIn", VaultCustodyActionTypeLocalName = "عودت به صندوق" },
            new VaultCustodyActionType { Id = (int)EnumVaultCustodyActionType.PermanentOut, VaultCustodyActionTypeName = "PermanentOut", VaultCustodyActionTypeLocalName = "خروج دائمی" }
        );
        }
    }

}
