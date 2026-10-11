using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.Messaging
{
    #region Main Messaging Entities
    public class MessagingConversation : BusinessEntity
    {
        public string Subject { get; set; } = string.Empty;
        public int ScopeId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public Guid? CustomerPersonId { get; set; }
        public Guid? PartnerPersonId { get; set; }
        public int? PartnerKindId { get; set; }
        public int? RelatedEntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? AssignedDepartmentId { get; set; }
        public DateTime? LastMessageAt { get; set; }

        //Relations
        public ApplicationUser CreatedByUser { get; set; } = null!;
        public ApplicationUser? AssignedToUser { get; set; }
        public EntityType? RelatedEntityType { get; set; }
        public MessagingPartnerKind? PartnerKind { get; set; }
        public Person? PartnerPerson { get; set; }
        public Person? CustomerPerson { get; set; }
        public MessagingConversationScope Scope { get; set; } = null!;
        public MessagingConversationType Type { get; set; } = null!;
        public MessagingConversationStatus Status { get; set; } = null!;


        public ICollection<MessagingConversationParticipant> Participants { get; set; } = new List<MessagingConversationParticipant>();
        public ICollection<MessagingMessage> Messages { get; set; } = new List<MessagingMessage>();
    }

    public class MessagingConversationParticipant : BusinessEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public int KindId { get; set; }
        public int RoleId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsMuted { get; set; }
        public bool IsArchived { get; set; }

        //Relations
        public MessagingParticipantRole Role { get; set; } = null!;
        public MessagingParticipantKind Kind { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public MessagingConversation Conversation { get; set; } = null!;
    }

    public class MessagingMessage : BusinessEntity
    {
        public Guid ConversationId { get; set; }
        public Guid SenderUserId { get; set; }
        public int VisibilityId { get; set; }
        public string? Body { get; set; }
        public DateTime SentAt { get; set; }
        //Relations
        public MessagingMessageVisibility Visibility { get; set; } = null!;
        public ApplicationUser SenderUser { get; set; } = null!;
        public MessagingConversation Conversation { get; set; } = null!;

        public ICollection<MessagingMessageAttachment> Attachments { get; set; } = new List<MessagingMessageAttachment>();
    }
    public class MessagingMessageAttachment : BusinessEntity
    {
        public Guid MessageId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public int VisibilityId { get; set; }
        //Relations
        public ApplicationUser UploadedByUser { get; set; } = null!;
        public MessagingAttachmentVisibility Visibility { get; set; } = null!;
        public MessagingMessage Message { get; set; } = null!;

    }
    #endregion


    #region Lookup Entities

    public class MessagingConversationScope : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class MessagingConversationType : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class MessagingConversationStatus : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }

    }

    public class MessagingParticipantKind : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }

    }

    public class MessagingParticipantRole : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class MessagingMessageVisibility : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class MessagingAttachmentVisibility : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class MessagingPartnerKind : SystemEntity
    {
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }

    }

    #endregion

    #region Lookup Id Constants

    public static class MessagingConversationTypeIds
    {
        public const int General = 1;
        public const int Shipment = 2;
        public const int Operation = 3;
        public const int Finance = 4;
        public const int Documentation = 5;
        public const int Claim = 6;
        public const int Support = 7;
    }


    public static class MessagingConversationScopeIds
    {
        public const int Internal = 1;
        public const int CustomerCompany = 2;
        public const int PartnerCompany = 3;
    }
    public static class MessagingConversationStatusIds
    {
        public const int Open = 1;
        public const int PendingStaff = 2;
        public const int PendingCustomer = 3;
        public const int PendingPartner = 4;
        public const int Resolved = 5;
        public const int Closed = 6;
    }

    public static class MessagingParticipantKindIds
    {
        public const int Internal = 1;
        public const int Customer = 2;
        public const int ExternalPartner = 3;
    }

    public static class MessagingParticipantRoleIds
    {
        public const int Owner = 1;
        public const int Member = 2;
        public const int Observer = 3;
        public const int Supervisor = 4;
    }

    public static class MessagingMessageVisibilityIds
    {
        public const int AllParticipants = 1;
        public const int InternalOnly = 2;
    }

    public static class MessagingAttachmentVisibilityIds
    {
        public const int SameAsMessage = 1;
        public const int InternalOnly = 2;
    }

    public static class MessagingPartnerKindIds
    {
        public const int Carrier = 1;
        public const int Agent = 2;
        public const int Broker = 3;
        public const int Vendor = 4;
        public const int CustomsBroker = 5;
        public const int Warehouse = 6;
        public const int Other = 99;
    }
    #endregion
}

