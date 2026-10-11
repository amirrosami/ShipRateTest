using ShipRate.Application.DTOs.Messaging;

namespace ShipRate.Application.Interfaces.Messaging
{
    public interface IMessagingService
    {
        Task<Guid> CreateInternalConversationAsync(
            CreateInternalConversationDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateCustomerConversationAsync(
            CreateCustomerConversationDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<Guid> CreatePartnerConversationAsync(
            CreatePartnerConversationDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<Guid> AddMessageAsync(
            AddMessageDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task AddParticipantAsync(
            AddParticipantDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MessagingConversationListItemDto>> GetInboxAsync(
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MessagingMessageDto>> GetMessagesAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<MessagingMessageDto?> GetMessageAsync(
            Guid conversationId,
            Guid messageId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<MessagingParticipantDto>> GetParticipantsAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task AddInternalParticipantAsync(
            Guid conversationId,
            Guid participantUserId,
            Guid addedByUserId,
            CancellationToken cancellationToken = default);

        Task<MessagingUnreadHeaderDto> GetUnreadHeaderAsync(
            Guid userId,
            int take,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Guid>> GetConversationParticipantUserIdsAsync(
            Guid conversationId,
            CancellationToken cancellationToken);

        Task<bool> IsParticipantAsync(
            Guid conversationId,
            Guid userId,
            CancellationToken cancellationToken);


        Task<MessageAttachmentDownloadDto?> GetAttachmentForDownloadAsync(
            Guid attachmentId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task CloseConversationAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);



    }
}
