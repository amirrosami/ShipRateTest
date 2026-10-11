using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Messaging
{
    public sealed class CreateInternalConversationDto
    {
        public string Subject { get; set; } = null!;
        public string? InitialMessage { get; set; } = null!;
        public int TypeId { get; set; }
        public int? RelatedEntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public IReadOnlyCollection<Guid> ParticipantUserIds { get; set; } = Array.Empty<Guid>();
    }

    public sealed class CreateCustomerConversationDto
    {
        public string Subject { get; set; } = null!;
        public int TypeId { get; set; }
        public Guid CustomerPersonId { get; set; }
        public int? RelatedEntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }

    public sealed class CreatePartnerConversationDto
    {
        public string Subject { get; set; } = null!;
        public int TypeId { get; set; }
        public Guid PartnerPersonId { get; set; }
        public int PartnerKindId { get; set; }
        public int? RelatedEntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }

    public sealed class AddMessageDto
    {
        public Guid ConversationId { get; set; }
        public string? Body { get; set; }
        public int VisibilityId { get; set; }
        public List<IFormFile>? Attachments { get; set; }

    }

    public sealed class AddParticipantDto
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public int KindId { get; set; }
        public int RoleId { get; set; }
        public bool IsPrimary { get; set; }
    }

    public sealed class MessagingConversationListItemDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = null!;
        public int ScopeId { get; set; }
        public string ScopeTitle { get; set; } = null!;
        public int TypeId { get; set; }
        public string TypeTitle { get; set; } = null!;
        public int StatusId { get; set; }
        public string StatusTitle { get; set; } = null!;
        public Guid? CustomerPersonId { get; set; }
        public Guid? PartnerPersonId { get; set; }
        public int? PartnerKindId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserDisplayName { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

    public sealed class MessagingMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderUserId { get; set; }
        public string SenderDisplayName { get; set; } = null!;
        public string? Body { get; set; }
        public int VisibilityId { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsMine { get; set; }

        public IReadOnlyList<MessageAttachmentDto> Attachments { get; set; }
            = Array.Empty<MessageAttachmentDto>();

    }

    public sealed class MessagingParticipantDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public string UserDisplayName { get; set; } = null!;
        public int KindId { get; set; }
        public string KindTitle { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleTitle { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public bool hasProfilePicture { get; set; }
    }




    public sealed class MessagingIndexViewModel
    {
        public IReadOnlyList<MessagingConversationListItemDto> Conversations { get; set; }
            = Array.Empty<MessagingConversationListItemDto>();
    }

    public sealed class MessagingConversationViewModel
    {
        public Guid ConversationId { get; set; }
        public MessagingConversationListItemDto? Conversation { get; set; }

        public IReadOnlyList<MessagingMessageDto> Messages { get; set; }
            = Array.Empty<MessagingMessageDto>();

        public SendInternalMessageViewModel SendMessage { get; set; } = new();


        public Guid? AssignedToUserId { get; set; }

        public string? AssignedToDisplayName { get; set; }

        public List<MessagingParticipantDto> Participants { get; set; } = new();

        public AddInternalParticipantViewModel AddParticipant { get; set; } = new();



    }

    public class AddInternalParticipantViewModel
    {
        [Required]
        public Guid ConversationId { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        public Guid UserId { get; set; }

        public List<SelectListItem> AvailableUsers { get; set; } = new();
    }

    public sealed class CreateInternalConversationViewModel
    {
        [Required]
        [StringLength(250)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public int TypeId { get; set; } = 1;

        public int? RelatedEntityTypeId { get; set; }

        public Guid? RelatedEntityId { get; set; }

        public Guid? AssignedToUserId { get; set; }

        [Required]
        [StringLength(4000)]
        public string InitialMessage { get; set; } = string.Empty;




        public List<Guid> ParticipantUserIds { get; set; } = new();

        public List<SelectListItem> InternalUsers { get; set; } = new();




    }

    public sealed class SendInternalMessageViewModel
    {
        [Required]
        public Guid ConversationId { get; set; }

        [StringLength(4000)]
        public string? Body { get; set; }

        public List<IFormFile>? Attachments { get; set; }



    }


    public sealed class MessagingUnreadHeaderItemDto
    {
        public Guid ConversationId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string SenderDisplayName { get; set; } = string.Empty;

        public string LastMessagePreview { get; set; } = string.Empty;

        public DateTime? LastMessageAt { get; set; }

        public int UnreadCount { get; set; }
    }

    public sealed class MessagingUnreadHeaderDto
    {
        public int TotalUnreadCount { get; set; }

        public List<MessagingUnreadHeaderItemDto> Items { get; set; } = new();
    }

    public sealed class MessageAttachmentDto
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
    }

    public sealed class MessageAttachmentDownloadDto
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public string StoragePath { get; set; } = string.Empty;
    }

    public sealed class MessagingAttachmentDownloadDto
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public string PhysicalPath { get; set; } = string.Empty;
    }


}
