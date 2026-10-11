using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Domain.Entities.Notifs;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Message).HasMaxLength(400);
            builder.Property(x => x.ReferenceNo).HasMaxLength(50);
            builder.Property(x => x.IconCssClass).HasMaxLength(40);
            builder.Property(x => x.TargetUrl).HasMaxLength(150);

            builder.HasOne(x => x.SourceType).WithMany().HasForeignKey(x => x.SourceTypeId);
            builder.HasOne(x => x.NotificationType).WithMany().HasForeignKey(x => x.NotificationTypeId);
            builder.HasOne(x => x.Severity).WithMany().HasForeignKey(x => x.SeverityId);
            builder.HasOne(x => x.Priority).WithMany().HasForeignKey(x => x.PriorityId);
            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);
            builder.HasOne(x => x.ActivityLog).WithMany(x => x.Notifications).HasForeignKey(x => x.ActivityLogId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.ArchivedByUser).WithMany().HasForeignKey(x => x.ArchivedByUserId);
        }
    }
    public class NotificationUserConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("UserNotifications", "comms");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Notification).WithMany(x => x.UserNotifications).HasForeignKey(x => x.NotificationId);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

        }
    }
    public class NotificationAudienceConfiguration : IEntityTypeConfiguration<NotificationAudience>
    {
        public void Configure(EntityTypeBuilder<NotificationAudience> builder)
        {
            builder.ToTable("NotificationAudiences", "comms");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Notification).WithMany(x => x.Audiences).HasForeignKey(x => x.NotificationId);
            builder.HasOne(x => x.AudienceType).WithMany().HasForeignKey(x => x.AudienceTypeId);



        }
    }

    public class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
    {
        public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
        {
            builder.ToTable("NotificationDelivery", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Destination).HasMaxLength(100);
            builder.Property(x => x.ErrorMessage).HasMaxLength(500);

            builder.HasOne(x => x.Notification).WithMany(x => x.Deliveries).HasForeignKey(x => x.NotificationId);
            builder.HasOne(x => x.UserNotification).WithMany().HasForeignKey(x => x.UserNotificationId);
            builder.HasOne(x => x.Channel).WithMany().HasForeignKey(x => x.ChannelId);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.DeliveryStatus).WithMany().HasForeignKey(x => x.DeliveryStatusId);

        }
    }

    public class NotificationSeverityConfiguration : IEntityTypeConfiguration<NotificationSeverity>
    {
        public void Configure(EntityTypeBuilder<NotificationSeverity> builder)
        {
            builder.ToTable("NotificationSeverity", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.SeverityName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SeverityLocalName).HasMaxLength(50);
            builder.Property(x => x.CssClass).HasMaxLength(50);
            builder.Property(x => x.IconCssClass).HasMaxLength(50);


            builder.HasData(
                new NotificationSeverity { Id = 1, SeverityName = "Info", SeverityLocalName = "اطلاع رسانی", CssClass = "text-info", IconCssClass = "bi bi-info-circle" },
                new NotificationSeverity { Id = 2, SeverityName = "Success", SeverityLocalName = "موفق", CssClass = "text-success", IconCssClass = "bi bi-check-circle" },
                new NotificationSeverity { Id = 3, SeverityName = "Warning", SeverityLocalName = "هشدار", CssClass = "text-warning", IconCssClass = "bi bi-error" },
                new NotificationSeverity { Id = 4, SeverityName = "Error", SeverityLocalName = "خطا", CssClass = "text-danger", IconCssClass = "bi bi-x-circle" },
                new NotificationSeverity { Id = 5, SeverityName = "Critical", SeverityLocalName = "بحرانی", CssClass = "text-danger", IconCssClass = "bi bi-error-circle" }
                );
        }
    }


    public class NotificationSourceTypeConfiguration : IEntityTypeConfiguration<NotificationSourceType>
    {
        public void Configure(EntityTypeBuilder<NotificationSourceType> builder)
        {
            builder.ToTable("NotificationSourceTypes", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.SourceTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SourceTypeLocalName).HasMaxLength(50);

            builder.HasData(
                new NotificationSourceType { Id = 1, SourceTypeName = "System Event", SourceTypeLocalName = "رویداد سیستمی" },
                new NotificationSourceType { Id = 2, SourceTypeName = "Manual", SourceTypeLocalName = "اعلان دستی مدیر" },
                new NotificationSourceType { Id = 3, SourceTypeName = "Scheduled", SourceTypeLocalName = "اعلان زمان بندی شده" },
                new NotificationSourceType { Id = 4, SourceTypeName = "Integration", SourceTypeLocalName = "اعلان ناشی از سیستم خارجی" },
                new NotificationSourceType { Id = 5, SourceTypeName = "Workflow", SourceTypeLocalName = "اعلان ناشی از فرایند کاری" },
                new NotificationSourceType { Id = 6, SourceTypeName = "ActivityLog", SourceTypeLocalName = "لاگ عملیاتی" }
                );



        }
    }
    public class NotificationAudienceTypeConfiguration : IEntityTypeConfiguration<NotificationAudienceType>
    {
        public void Configure(EntityTypeBuilder<NotificationAudienceType> builder)
        {
            builder.ToTable("NotificationAudienceTypes", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.AudienceTypeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.AudienceTypeLocalName).HasMaxLength(50);

            builder.HasData(
                new NotificationAudienceType { Id = 1, AudienceTypeName = "User", AudienceTypeLocalName = "" },
                new NotificationAudienceType { Id = 2, AudienceTypeName = "Role", AudienceTypeLocalName = "" },
                new NotificationAudienceType { Id = 3, AudienceTypeName = "Customer", AudienceTypeLocalName = "" },
                new NotificationAudienceType { Id = 4, AudienceTypeName = "Company", AudienceTypeLocalName = "" },
                new NotificationAudienceType { Id = 5, AudienceTypeName = "All Users", AudienceTypeLocalName = "" },
                new NotificationAudienceType { Id = 6, AudienceTypeName = "All Customers", AudienceTypeLocalName = "" },
                new NotificationAudienceType { Id = 7, AudienceTypeName = "All Agents", AudienceTypeLocalName = "" }
                );


        }
    }
    public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
    {
        public void Configure(EntityTypeBuilder<NotificationChannel> builder)
        {
            builder.ToTable("NotificationChannels", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ChannelName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ChannelLocalName).HasMaxLength(50);


            builder.HasData(
                new NotificationChannel { Id = 1, ChannelName = "InApp", ChannelLocalName = "داخل اپلیکین" },
                new NotificationChannel { Id = 2, ChannelName = "Email", ChannelLocalName = "ایمیل" },
                new NotificationChannel { Id = 3, ChannelName = "SMS", ChannelLocalName = "اس ام اس" },
                new NotificationChannel { Id = 4, ChannelName = "Push", ChannelLocalName = "پوش" },
                new NotificationChannel { Id = 5, ChannelName = "WhatsApp", ChannelLocalName = "واتس اپ" }

                );

        }
    }
    public class NotificationDeliveryStatusConfiguration : IEntityTypeConfiguration<NotificationDeliveryStatus>
    {
        public void Configure(EntityTypeBuilder<NotificationDeliveryStatus> builder)
        {
            builder.ToTable("NotificationDeliveryStatus", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.DeliveryStatusName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DeliveryStatusLocalName).HasMaxLength(50);


            builder.HasData(
                new NotificationDeliveryStatus { Id = 1, DeliveryStatusName = "Pending", DeliveryStatusLocalName = "در انتظار ارسال" },
                new NotificationDeliveryStatus { Id = 2, DeliveryStatusName = "Sent", DeliveryStatusLocalName = "ارسال شد" },
                new NotificationDeliveryStatus { Id = 3, DeliveryStatusName = "Delivered", DeliveryStatusLocalName = "تحویل شد" },
                new NotificationDeliveryStatus { Id = 4, DeliveryStatusName = "Failed", DeliveryStatusLocalName = "ناموفق" },
                new NotificationDeliveryStatus { Id = 5, DeliveryStatusName = "Canceled", DeliveryStatusLocalName = "لغو شد" }
                );

        }
    }

    public class NotificationRecipientModeConfiguration : IEntityTypeConfiguration<NotificationRecipientMode>
    {
        public void Configure(EntityTypeBuilder<NotificationRecipientMode> builder)
        {
            builder.ToTable("NotificationRecipientModes", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.RecipientModeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.RecipientModeLocalName).HasMaxLength(50);


            builder.HasData(
                new NotificationRecipientMode { Id = -1, RecipientModeName = "None", RecipientModeLocalName = "هیچ" },
                new NotificationRecipientMode { Id = 1, RecipientModeName = "Actor", RecipientModeLocalName = "فقط کاربر انجام دهنده عملیات" },
                new NotificationRecipientMode { Id = 2, RecipientModeName = "Admins", RecipientModeLocalName = "همه ادمین ها" },
                new NotificationRecipientMode { Id = 3, RecipientModeName = "SpecificUsers", RecipientModeLocalName = "کاربران مشخص شده در Rule" },
                new NotificationRecipientMode { Id = 4, RecipientModeName = "UserTypes", RecipientModeLocalName = "همه کاربران یک یا چند نوع کاربر" },
                new NotificationRecipientMode { Id = 5, RecipientModeName = "EntityRelatedUsers", RecipientModeLocalName = "کاربران مرتبط" },
                new NotificationRecipientMode { Id = 6, RecipientModeName = "Mixed", RecipientModeLocalName = "ترکیبی از چند حالت" }
                );

        }
    }

    public class NotificationRuleConfiguration : IEntityTypeConfiguration<NotificationRule>
    {
        public void Configure(EntityTypeBuilder<NotificationRule> builder)
        {
            builder.ToTable("NotificationRules", "comms");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ActivityType).HasMaxLength(50);
            builder.Property(x => x.TitleTemplate).HasMaxLength(500);
            builder.Property(x => x.MessageTemplate).HasMaxLength(500);
            builder.Property(x => x.TargetUrlTemplate).HasMaxLength(500);

            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);
            builder.HasOne(x => x.RecipientMode).WithMany().HasForeignKey(x => x.RecipientModeId);
        }
    }


    public class NotificationRuleUserConfiguration : IEntityTypeConfiguration<NotificationRuleUser>
    {
        public void Configure(EntityTypeBuilder<NotificationRuleUser> builder)
        {
            builder.ToTable("NotificationRuleUsers", "comms");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.NotificationRule).WithMany(x => x.Users).HasForeignKey(x => x.NotificationRuleId);
        }
    }

    public class NotificationRuleUserTypeConfiguration : IEntityTypeConfiguration<NotificationRuleUserType>
    {
        public void Configure(EntityTypeBuilder<NotificationRuleUserType> builder)
        {
            builder.ToTable("NotificationRuleUserTypes", "comms");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.UserType).WithMany().HasForeignKey(x => x.UserTypeId);
            builder.HasOne(x => x.NotificationRule).WithMany(x => x.UserTypes).HasForeignKey(x => x.NotificationRuleId);
        }
    }

    public class MessagingConversationConfiguration : IEntityTypeConfiguration<MessagingConversation>
    {
        public void Configure(EntityTypeBuilder<MessagingConversation> builder)
        {
            builder.ToTable("MessagingConversations", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Subject).HasMaxLength(300).IsRequired();

            builder.HasOne(x => x.Scope).WithMany().HasForeignKey(x => x.ScopeId);
            builder.HasOne(x => x.Type).WithMany().HasForeignKey(x => x.TypeId);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.AssignedToUser).WithMany().HasForeignKey(x => x.AssignedToUserId);
            builder.HasOne(x => x.CustomerPerson).WithMany().HasForeignKey(x => x.CustomerPersonId);
            builder.HasOne(x => x.PartnerPerson).WithMany().HasForeignKey(x => x.PartnerPersonId);
            builder.HasOne(x => x.PartnerKind).WithMany().HasForeignKey(x => x.PartnerKindId);
            builder.HasOne(x => x.RelatedEntityType).WithMany().HasForeignKey(x => x.RelatedEntityTypeId);

            builder.HasMany(x => x.Participants).WithOne(x => x.Conversation).HasForeignKey(x => x.ConversationId);
            builder.HasMany(x => x.Messages).WithOne(x => x.Conversation).HasForeignKey(x => x.ConversationId);

            builder.HasIndex(x => x.ScopeId);
            builder.HasIndex(x => x.TypeId);
            builder.HasIndex(x => x.StatusId);
            builder.HasIndex(x => x.CustomerPersonId);
            builder.HasIndex(x => x.PartnerPersonId);
            builder.HasIndex(x => x.AssignedToUserId);
            builder.HasIndex(x => new { x.RelatedEntityTypeId, x.RelatedEntityId });
            builder.HasIndex(x => x.LastMessageAt);
        }
    }

    public class MessagingConversationParticipantConfiguration : IEntityTypeConfiguration<MessagingConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<MessagingConversationParticipant> builder)
        {
            builder.ToTable("MessagingConversationParticipants", "comms");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Conversation).WithMany(x => x.Participants).HasForeignKey(x => x.ConversationId);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Kind).WithMany().HasForeignKey(x => x.KindId);
            builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);

            builder.HasIndex(x => x.ConversationId);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.KindId);
            builder.HasIndex(x => x.RoleId);
            builder.HasIndex(x => new { x.ConversationId, x.UserId }).IsUnique();
            builder.HasIndex(x => new { x.ConversationId, x.IsPrimary });
        }
    }

    public class MessagingMessageConfiguration : IEntityTypeConfiguration<MessagingMessage>
    {
        public void Configure(EntityTypeBuilder<MessagingMessage> builder)
        {
            builder.ToTable("MessagingMessages", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ConversationId).IsRequired();
            builder.Property(x => x.SenderUserId).IsRequired();
            builder.Property(x => x.VisibilityId).IsRequired();
            builder.Property(x => x.Body).HasMaxLength(4000);
            builder.Property(x => x.SentAt).IsRequired();
            builder.HasOne(x => x.Conversation).WithMany(x => x.Messages).HasForeignKey(x => x.ConversationId);
            builder.HasOne(x => x.SenderUser).WithMany().HasForeignKey(x => x.SenderUserId);
            builder.HasOne(x => x.Visibility).WithMany().HasForeignKey(x => x.VisibilityId);
            builder.HasMany(x => x.Attachments).WithOne(x => x.Message).HasForeignKey(x => x.MessageId);
            builder.HasIndex(x => x.ConversationId);
            builder.HasIndex(x => x.SenderUserId);
            builder.HasIndex(x => x.VisibilityId);
            builder.HasIndex(x => x.SentAt);
            builder.HasIndex(x => new { x.ConversationId, x.SentAt });
        }
    }

    public class MessagingMessageAttachmentConfiguration : IEntityTypeConfiguration<MessagingMessageAttachment>
    {
        public void Configure(EntityTypeBuilder<MessagingMessageAttachment> builder)
        {
            builder.ToTable("MessagingMessageAttachments", "comms");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            builder.Property(x => x.StoredFileName).HasMaxLength(255).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
            builder.Property(x => x.StoragePath).HasMaxLength(1000).IsRequired();

            builder.HasOne(x => x.Message).WithMany(x => x.Attachments).HasForeignKey(x => x.MessageId);
            builder.HasOne(x => x.UploadedByUser).WithMany().HasForeignKey(x => x.CreateUserId);
            builder.HasOne(x => x.Visibility).WithMany().HasForeignKey(x => x.VisibilityId);

            builder.HasIndex(x => x.MessageId);
            builder.HasIndex(x => x.CreateUserId);
            builder.HasIndex(x => x.VisibilityId);
        }
    }

    public class MessagingConversationScopeConfiguration : IEntityTypeConfiguration<MessagingConversationScope>
    {
        public void Configure(EntityTypeBuilder<MessagingConversationScope> builder)
        {
            builder.ToTable("MessagingConversationScopes", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();
            builder.HasData(
                new MessagingConversationScope { Id = MessagingConversationScopeIds.Internal, Title = "Internal", SortOrder = 1 },
                new MessagingConversationScope { Id = MessagingConversationScopeIds.CustomerCompany, Title = "Customer Company", SortOrder = 2 },
                new MessagingConversationScope { Id = MessagingConversationScopeIds.PartnerCompany, Title = "Partner Company", SortOrder = 3 }
            );
        }
    }

    public class MessagingConversationStatusConfiguration : IEntityTypeConfiguration<MessagingConversationStatus>
    {
        public void Configure(EntityTypeBuilder<MessagingConversationStatus> builder)
        {
            builder.ToTable("MessagingConversationStatuses", "comms");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();
            builder.HasData(
                new MessagingConversationStatus { Id = MessagingConversationStatusIds.Open, Title = "Open", SortOrder = 1 },
                new MessagingConversationStatus { Id = MessagingConversationStatusIds.PendingStaff, Title = "Pending Staff", SortOrder = 2 },
                new MessagingConversationStatus { Id = MessagingConversationStatusIds.PendingCustomer, Title = "Pending Customer", SortOrder = 3 },
                new MessagingConversationStatus { Id = MessagingConversationStatusIds.PendingPartner, Title = "Pending Partner", SortOrder = 4 },
                new MessagingConversationStatus { Id = MessagingConversationStatusIds.Resolved, Title = "Resolved", SortOrder = 5 },
                new MessagingConversationStatus { Id = MessagingConversationStatusIds.Closed, Title = "Closed", SortOrder = 6 }
            );
        }
    }

    public class MessagingParticipantKindConfiguration : IEntityTypeConfiguration<MessagingParticipantKind>
    {
        public void Configure(EntityTypeBuilder<MessagingParticipantKind> builder)
        {
            builder.ToTable("MessagingParticipantKinds", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();


            builder.HasData(
                new MessagingParticipantKind { Id = MessagingParticipantKindIds.Internal, Title = "Internal", SortOrder = 1 },
                new MessagingParticipantKind { Id = MessagingParticipantKindIds.Customer, Title = "Customer", SortOrder = 2 },
                new MessagingParticipantKind { Id = MessagingParticipantKindIds.ExternalPartner, Title = "External Partner", SortOrder = 3 }
            );
        }
    }


    public class MessagingParticipantRoleConfiguration : IEntityTypeConfiguration<MessagingParticipantRole>
    {
        public void Configure(EntityTypeBuilder<MessagingParticipantRole> builder)
        {
            builder.ToTable("MessagingParticipantRoles", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();

            builder.HasIndex(x => x.Title).IsUnique();

            builder.HasData(
                new MessagingParticipantRole { Id = MessagingParticipantRoleIds.Owner, Title = "Owner", SortOrder = 1 },
                new MessagingParticipantRole { Id = MessagingParticipantRoleIds.Member, Title = "Member", SortOrder = 2 },
                new MessagingParticipantRole { Id = MessagingParticipantRoleIds.Observer, Title = "Observer", SortOrder = 3 },
                new MessagingParticipantRole { Id = MessagingParticipantRoleIds.Supervisor, Title = "Supervisor", SortOrder = 4 }
            );
        }
    }

    public class MessagingMessageVisibilityConfiguration : IEntityTypeConfiguration<MessagingMessageVisibility>
    {
        public void Configure(EntityTypeBuilder<MessagingMessageVisibility> builder)
        {
            builder.ToTable("MessagingMessageVisibilities", "comms");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();

            builder.HasIndex(x => x.Title).IsUnique();

            builder.HasData(
                new MessagingMessageVisibility { Id = MessagingMessageVisibilityIds.AllParticipants, Title = "All Participants", SortOrder = 1 },
                new MessagingMessageVisibility { Id = MessagingMessageVisibilityIds.InternalOnly, Title = "Internal Only", SortOrder = 2 }
            );
        }
    }

    public class MessagingAttachmentVisibilityConfiguration : IEntityTypeConfiguration<MessagingAttachmentVisibility>
    {
        public void Configure(EntityTypeBuilder<MessagingAttachmentVisibility> builder)
        {
            builder.ToTable("MessagingAttachmentVisibilities", "comms");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();

            builder.HasData(
                new MessagingAttachmentVisibility { Id = MessagingAttachmentVisibilityIds.SameAsMessage, Title = "Same As Message", SortOrder = 1 },
                new MessagingAttachmentVisibility { Id = MessagingAttachmentVisibilityIds.InternalOnly, Title = "Internal Only", SortOrder = 2 }
            );
        }
    }

    public class MessagingPartnerKindConfiguration : IEntityTypeConfiguration<MessagingPartnerKind>
    {
        public void Configure(EntityTypeBuilder<MessagingPartnerKind> builder)
        {
            builder.ToTable("MessagingPartnerKinds", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();

            builder.HasData(
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.Carrier, Title = "Carrier", SortOrder = 1 },
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.Agent, Title = "Agent", SortOrder = 2 },
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.Broker, Title = "Broker", SortOrder = 3 },
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.Vendor, Title = "Vendor", SortOrder = 4 },
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.CustomsBroker, Title = "Customs Broker", SortOrder = 5 },
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.Warehouse, Title = "Warehouse", SortOrder = 6 },
                new MessagingPartnerKind { Id = MessagingPartnerKindIds.Other, Title = "Other", SortOrder = 99 }
            );
        }
    }

    public class MessagingConversationTypeConfiguration : IEntityTypeConfiguration<MessagingConversationType>
    {
        public void Configure(EntityTypeBuilder<MessagingConversationType> builder)
        {
            builder.ToTable("MessagingConversationTypes", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SortOrder).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();

            builder.HasData(
                new MessagingConversationType { Id = MessagingConversationTypeIds.General, Title = "General", SortOrder = 1 },
                new MessagingConversationType { Id = MessagingConversationTypeIds.Shipment, Title = "Shipment", SortOrder = 2 },
                new MessagingConversationType { Id = MessagingConversationTypeIds.Operation, Title = "Operation", SortOrder = 3 },
                new MessagingConversationType { Id = MessagingConversationTypeIds.Finance, Title = "Finance", SortOrder = 4 },
                new MessagingConversationType { Id = MessagingConversationTypeIds.Documentation, Title = "Documentation", SortOrder = 5 },
                new MessagingConversationType { Id = MessagingConversationTypeIds.Claim, Title = "Claim", SortOrder = 6 },
                new MessagingConversationType { Id = MessagingConversationTypeIds.Support, Title = "Support", SortOrder = 7 }
            );
        }
    }


    public class SMSMessageConfiguration : IEntityTypeConfiguration<SmsMessage>
    {
        public void Configure(EntityTypeBuilder<SmsMessage> builder)
        {
            builder.ToTable("SmsMessages", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Tag).HasMaxLength(100);
            builder.Property(x => x.Body).HasMaxLength(500);
            builder.Property(x => x.Sender).HasMaxLength(20);
        }
    }

    public class SmsRecipientConfiguration : IEntityTypeConfiguration<SmsRecipient>
    {
        public void Configure(EntityTypeBuilder<SmsRecipient> builder)
        {
            builder.ToTable("SmsRecipients", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CustomerTrackingId).HasMaxLength(100);
            builder.Property(x => x.GatewayServerId).HasMaxLength(100);
            builder.Property(x => x.Mobile).HasMaxLength(20);
            builder.Property(x => x.ErrorCode).HasMaxLength(20);
            builder.Property(x => x.ErrorDescription).HasMaxLength(500);
            builder.Property(r => r.Status)
                       .HasConversion<int>()
                       .IsRequired();
            builder.HasOne(x => x.SmsMessage).WithMany().HasForeignKey(x => x.SmsMessageId);
        }
    }

    public class SmsIncomingMessageConfiguration : IEntityTypeConfiguration<SmsIncomingMessage>
    {
        public void Configure(EntityTypeBuilder<SmsIncomingMessage> builder)
        {
            builder.ToTable("SmsIncomingMessages", "comms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever(); // 👈 جلوگیری از ساخت به عنوان Identity
            builder.Property(x => x.LineNumber).HasMaxLength(20);
            builder.Property(x => x.FromMobile).HasMaxLength(20);
            builder.Property(x => x.Body).HasMaxLength(500);
        }
    }
    public class SmsMoCursorConfiguration : IEntityTypeConfiguration<SmsMoCursor>
    {
        public void Configure(EntityTypeBuilder<SmsMoCursor> builder)
        {
            builder.ToTable("SmsMoCursors", "comms");
            builder.HasKey(x => x.LineNumber);
            builder.Property(x => x.LineNumber).HasMaxLength(20);
        }
    }
    public class MarketingCallLogConfiguration : IEntityTypeConfiguration<MarketingCallLog>
    {
        public void Configure(EntityTypeBuilder<MarketingCallLog> builder)
        {
            builder.ToTable("MarketingCallLogs", "comms");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired().IsUnicode(false); // شماره تلفن کاراکتر غیریونیکد است
            builder.Property(x => x.Subject).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.InquiryReference).HasMaxLength(50).IsRequired(false).IsUnicode(false);

            // ۴. زمان‌ها و ثانیه‌شمار مکالمه
            builder.Property(x => x.CallStartTime).IsRequired();

            builder.Property(x => x.CallEndTime).IsRequired(false);
            builder.Property(x => x.DurationSeconds).HasDefaultValue(0);

            // ۵. وضعیت و یادداشت‌ها
            builder.Property(x => x.Outcome).IsRequired()
                   .HasConversion<byte>(); // ذخیره در دیتابیس به صورت TinyInt برای بهینه‌سازی حجم

            builder.Property(x => x.Notes).HasMaxLength(2000).IsRequired(false);
            builder.Property(x => x.FollowUpDate).IsRequired(false);

            // ۶. مشخصات کارشناس و تجهیزات سیسکو
            builder.Property(x => x.CallerUserId).HasMaxLength(450).IsRequired();
            builder.Property(x => x.CallerExtension).HasMaxLength(20).IsRequired(false).IsUnicode(false);

            builder.Property(x => x.PhoneIpAddress).HasMaxLength(50).IsRequired(false).IsUnicode(false);

            // ۷. ایندکس‌های کلیدی جهت گزارش‌گیری سریع و سرچ
            builder.HasIndex(x => x.PhoneNumber).HasDatabaseName("IX_MarketingCallLogs_PhoneNumber");
            builder.HasIndex(x => x.CallStartTime).HasDatabaseName("IX_MarketingCallLogs_CallStartTime");
            builder.HasIndex(x => x.CallerUserId).HasDatabaseName("IX_MarketingCallLogs_CallerUserId");
            builder.HasIndex(x => x.InquiryReference).HasDatabaseName("IX_MarketingCallLogs_InquiryReference");
            builder.HasIndex(x => x.FollowUpDate).HasDatabaseName("IX_MarketingCallLogs_FollowUpDate");
        }
    }
}
