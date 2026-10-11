using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domaiun.Entities.Customers;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers", "crm");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Remarks).HasMaxLength(200);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.Id);
            builder.HasOne(x => x.SalesUser).WithMany().HasForeignKey(x => x.SalesUserId);
            builder.HasOne(x => x.Industry).WithMany().HasForeignKey(x => x.IndustryId);
        }
    }
    public class BlackListConfiguration : IEntityTypeConfiguration<BlackList>
    {
        public void Configure(EntityTypeBuilder<BlackList> builder)
        {
            builder.ToTable("BlackLists", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.BlackListEnterRemarks).HasMaxLength(200);
            builder.Property(b => b.BlackListExitRemarks).HasMaxLength(200);

            builder.HasOne(x => x.BlackListReason)
                    .WithMany()
                    .HasForeignKey(x => x.BlackListReasonId);
        }
    }

    public class CustomerGroupingBaseConfiguration : IEntityTypeConfiguration<CustomerGroupingBase>
    {
        public void Configure(EntityTypeBuilder<CustomerGroupingBase> builder)
        {
            builder.ToTable("CustomerGroupingBases", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.CustomerGroupingBaseName).HasMaxLength(50);
        }
    }

    public class CustomerGroupConfiguration : IEntityTypeConfiguration<CustomerGroup>
    {
        public void Configure(EntityTypeBuilder<CustomerGroup> builder)
        {
            builder.ToTable("CustomerGroups", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.CustomerGroupName).HasMaxLength(100);

            builder.HasOne(x => x.CustomerGroupingBase)
                    .WithMany(x => x.CustomerGroups)
                    .HasForeignKey(x => x.CustomerGroupingBaseId);
        }
    }

    public class BelongConfiguration : IEntityTypeConfiguration<CustomerBelongsToGroup>
    {
        public void Configure(EntityTypeBuilder<CustomerBelongsToGroup> builder)
        {
            builder.ToTable("CustomerBelongsToGroups", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.EnterRemarks).HasMaxLength(200);
            builder.Property(b => b.ExitRemarks).HasMaxLength(200);

            builder.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.CustomerGroup)
                    .WithMany(x => x.CustomerBelongsToGroups)
                    .HasForeignKey(x => x.CustomerGroupId);
        }
    }

    public class CustomerConfirmationConfiguration : IEntityTypeConfiguration<CustomerConfirmation>
    {
        public void Configure(EntityTypeBuilder<CustomerConfirmation> builder)
        {
            builder.ToTable("CustomerConfirmations", "crm");
            builder.HasKey(c => c.Id);

            builder.Property(b => b.Remarks).HasMaxLength(200);
            builder.Property(b => b.COnfirmRemarks).HasMaxLength(200);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.ConfirmUser).WithMany().HasForeignKey(x => x.ConfirmUserId);

        }
    }

    public class FinanceSettingConfiguration : IEntityTypeConfiguration<CustomerFinanceSetting>
    {
        public void Configure(EntityTypeBuilder<CustomerFinanceSetting> builder)
        {
            builder.ToTable("CustomerFinanceSettings", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.FinanceRemarks).HasMaxLength(200);
            builder.Property(b => b.TaxNumber).HasMaxLength(25);
            builder.Property(b => b.AccountCode).HasMaxLength(20);

            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.RiskLevel).WithMany().HasForeignKey(x => x.RiskLevelId);
        }
    }

    public class RatingConfiguration : IEntityTypeConfiguration<CustomerRating>
    {
        public void Configure(EntityTypeBuilder<CustomerRating> builder)
        {
            builder.ToTable("CustomerRatings", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }
    public class CustomerRoleTypeConfiguration : IEntityTypeConfiguration<CustomerRoleType>
    {
        public void Configure(EntityTypeBuilder<CustomerRoleType> builder)
        {
            builder.ToTable("CustomerRoleTypes", "crm");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.RoleType).WithMany().HasForeignKey(x => x.RoleTypeId);
        }
    }
    public class CustomerTradeTypeConfiguration : IEntityTypeConfiguration<CustomerTradeType>
    {
        public void Configure(EntityTypeBuilder<CustomerTradeType> builder)
        {
            builder.ToTable("CustomerTradeTypes", "crm");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Customer).WithMany(x => x.CustomerTradeTypes).HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);
        }
    }

    public class SalesPersonHistoryConfiguration : IEntityTypeConfiguration<SalesPersonHistory>
    {
        public void Configure(EntityTypeBuilder<SalesPersonHistory> builder)
        {
            builder.ToTable("SalesPersonHistory", "crm");
            builder.HasKey(c => c.Id);

            builder.Property(b => b.Remarks).HasMaxLength(200);


            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.SalesPerson).WithMany().HasForeignKey(x => x.SalesPersonId);

        }
    }
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.CampaignName).HasMaxLength(100);
            builder.Property(b => b.Remarks).HasMaxLength(400);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            builder.HasOne(x => x.CampaignType).WithMany().HasForeignKey(x => x.CampaignTypeId);
            builder.HasOne(x => x.CampaignStatus).WithMany().HasForeignKey(x => x.CampaignStatusId);
        }
    }
    public class LeadConfiguration : IEntityTypeConfiguration<Lead>
    {
        public void Configure(EntityTypeBuilder<Lead> builder)
        {
            builder.ToTable("Leads", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.CargoType).HasMaxLength(100);
            builder.Property(b => b.LostReasonDescription).HasMaxLength(200);
            builder.Property(b => b.CompanyName).HasMaxLength(100);
            builder.Property(b => b.ContactName).HasMaxLength(100);
            builder.Property(b => b.EmailAddress).HasMaxLength(50);
            builder.Property(b => b.Phone).HasMaxLength(50);
            builder.Property(b => b.IndustryName).HasMaxLength(100);

            builder.HasOne(x => x.TradeType).WithMany().HasForeignKey(x => x.TradeTypeId);
            builder.HasOne(x => x.LeadSource).WithMany().HasForeignKey(x => x.LeadSourceId);
            builder.HasOne(x => x.LeadStatus).WithMany().HasForeignKey(x => x.LeadStatusId);
            builder.HasOne(x => x.LostReason).WithMany().HasForeignKey(x => x.LostReasonId);
            builder.HasOne(x => x.Industry).WithMany().HasForeignKey(x => x.IndustryId);
            builder.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId);
            builder.HasOne(x => x.AssignedToUser).WithMany().HasForeignKey(x => x.AssignedToUserId);
        }
    }
    public class LeadTagConfiguration : IEntityTypeConfiguration<LeadTag>
    {
        public void Configure(EntityTypeBuilder<LeadTag> builder)
        {
            builder.ToTable("LeadTags", "crm");
            builder.HasKey(c => c.Id);



            builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId);
        }
    }



    public class CampaignMemberConfiguration : IEntityTypeConfiguration<CampaignMember>
    {
        public void Configure(EntityTypeBuilder<CampaignMember> builder)
        {
            builder.ToTable("CampaignMembers", "crm");
            builder.HasKey(c => c.Id);

            builder.Property(b => b.Remarks).HasMaxLength(400);

            builder.HasOne(x => x.Campaign).WithMany(x => x.Members).HasForeignKey(x => x.CampaignId);
            builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);
        }
    }


    public class CampaignActivityConfiguration : IEntityTypeConfiguration<CampaignActivity>
    {
        public void Configure(EntityTypeBuilder<CampaignActivity> builder)
        {
            builder.ToTable("CampaignActivity", "crm");
            builder.HasKey(c => c.Id);

            builder.Property(b => b.Subject).HasMaxLength(250);
            builder.Property(b => b.Remarks).HasMaxLength(400);

            builder.HasOne(x => x.Campaign).WithMany(x => x.Activities).HasForeignKey(x => x.CampaignId);
            builder.HasOne(x => x.ActivityType).WithMany().HasForeignKey(x => x.ActivityTypeId);
        }
    }




    public class LeadActivityConfiguration : IEntityTypeConfiguration<LeadActivity>
    {
        public void Configure(EntityTypeBuilder<LeadActivity> builder)
        {
            builder.ToTable("LeadActivity", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.Description).HasMaxLength(200);
            builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.HasOne(x => x.ActivityType).WithMany().HasForeignKey(x => x.ActivityTypeId);
            builder.HasOne(x => x.ActivityResult).WithMany().HasForeignKey(x => x.ActivityResultId);
            builder.HasOne(x => x.ActivityOutcome).WithMany().HasForeignKey(x => x.ActivityOutcomeId);
        }
    }

    public class FollowupTaskConfiguration : IEntityTypeConfiguration<FollowUpTask>
    {
        public void Configure(EntityTypeBuilder<FollowUpTask> builder)
        {
            builder.ToTable("FollowUpTasks", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.FollowUpTaskDescription).HasMaxLength(200);
            builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.HasOne(x => x.LeadActivity).WithMany().HasForeignKey(x => x.LeadActivityId);
            builder.HasOne(x => x.ReminderChannel).WithMany().HasForeignKey(x => x.ReminderChannelId);
            builder.HasOne(x => x.FollowUpStatus).WithMany().HasForeignKey(x => x.FollowUpStatusId);
            builder.HasOne(x => x.FollowUpPriority).WithMany().HasForeignKey(x => x.FollowUpPriorityId);
        }
    }

    public class AttorneyConfiguration : IEntityTypeConfiguration<Attorney>
    {
        public void Configure(EntityTypeBuilder<Attorney> builder)
        {
            builder.ToTable("Attorney", "crm");
            builder.HasKey(c => c.Id);
            builder.Property(b => b.Remarks).HasMaxLength(200);
            builder.Property(b => b.DocNo).HasMaxLength(30);

            builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);

        }
    }
    public class AttorneyLawyerConfiguration : IEntityTypeConfiguration<AttorneyLawyer>
    {
        public void Configure(EntityTypeBuilder<AttorneyLawyer> builder)
        {
            builder.ToTable("AttorneyLawyers", "crm");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Attorney).WithMany(x => x.AttorneyLawyers).HasForeignKey(x => x.AttorneyId);
            builder.HasOne(x => x.Lawyer).WithMany().HasForeignKey(x => x.LawyerId);
        }
    }

    public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
    {
        public void Configure(EntityTypeBuilder<Opportunity> builder)
        {
            builder.ToTable("Opportunities", "crm");
            builder.HasKey(c => c.Id);

            builder.Property(b => b.Title).HasMaxLength(200);
            builder.Property(b => b.LostReasonDescription).HasMaxLength(200);



            builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.LostReason).WithMany().HasForeignKey(x => x.LostReasonId);
            builder.HasOne(x => x.AssignedToUser).WithMany().HasForeignKey(x => x.AssignedToUserId);
            builder.HasOne(x => x.TransportType).WithMany().HasForeignKey(x => x.TransportTypeId);
            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.WonJobId);
            builder.HasOne(x => x.Stage).WithMany().HasForeignKey(x => x.StageId);
        }
    }




}
