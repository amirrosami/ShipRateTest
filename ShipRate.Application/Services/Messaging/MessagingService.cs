using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShipRate.Application.DTOs.Messaging;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Application.Security;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Messaging
{
    public sealed class MessagingService : IMessagingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        private readonly IAttachmentSecurityService _attachmentSecurityService;
        private readonly AttachmentSecurityOptions _attachmentSecurityOptions;

        public MessagingService(ApplicationDbContext db, IWebHostEnvironment environment, IAttachmentSecurityService attachmentSecurityService, IOptions<AttachmentSecurityOptions> attachmentSecurityOptions)
        {
            _db = db;
            _environment = environment;
            _attachmentSecurityService = attachmentSecurityService;
            _attachmentSecurityOptions = attachmentSecurityOptions.Value;
        }

        public async Task<Guid> CreateInternalConversationAsync(
            CreateInternalConversationDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateRelatedEntity(dto.RelatedEntityTypeId, dto.RelatedEntityId);

            var conversation = new MessagingConversation
            {
                Id = Guid.CreateVersion7(),
                Subject = dto.Subject.Trim(),
                ScopeId = MessagingConversationScopeIds.Internal,
                TypeId = dto.TypeId,
                StatusId = MessagingConversationStatusIds.Open,
                RelatedEntityTypeId = dto.RelatedEntityTypeId,
                RelatedEntityId = dto.RelatedEntityId,
                AssignedToUserId = dto.AssignedToUserId,
                CreateUserId = currentUserId,
                LastMessageAt = null
            };

            //conversation.Participants.Add(new MessagingConversationParticipant
            //{
            //    Id = Guid.CreateVersion7(),
            //    ConversationId = conversation.Id,
            //    UserId = currentUserId,
            //    KindId = MessagingParticipantKindIds.Internal,
            //    RoleId = MessagingParticipantRoleIds.Owner,
            //    IsPrimary = true,
            //    JoinedAt = DateTime.Now
            //});
            if (dto.InitialMessage.Length > 0)
            {
                conversation.Messages.Add(new MessagingMessage
                {
                    Id = Guid.CreateVersion7(),
                    SenderUserId = currentUserId,
                    SentAt = DateTime.Now,
                    VisibilityId = MessagingMessageVisibilityIds.InternalOnly,
                    ConversationId = conversation.Id,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId,
                    Body = dto.InitialMessage.Trim()
                });
            }
            _db.Set<MessagingConversation>().Add(conversation);
            await _db.SaveChangesAsync(cancellationToken);
            var participantUserIds = dto.ParticipantUserIds
                .Append(currentUserId)
                .Distinct()
                .ToList();

            foreach (var participantUserId in participantUserIds)
            {
                _db.MessagingConversationParticipants.Add(
                    new MessagingConversationParticipant
                    {
                        ConversationId = conversation.Id,
                        UserId = participantUserId,
                        CreateUserId = currentUserId,
                        JoinedAt = DateTime.Now,
                        //IsActive = true,
                        KindId = MessagingParticipantKindIds.Internal,
                        RoleId = participantUserId == currentUserId ? MessagingParticipantRoleIds.Owner : MessagingParticipantRoleIds.Member
                    });
            }


            await _db.SaveChangesAsync(cancellationToken);

            return conversation.Id;
        }

        public async Task<Guid> CreateCustomerConversationAsync(
            CreateCustomerConversationDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateRelatedEntity(dto.RelatedEntityTypeId, dto.RelatedEntityId);

            var conversation = new MessagingConversation
            {
                Id = Guid.CreateVersion7(),
                Subject = dto.Subject.Trim(),
                ScopeId = MessagingConversationScopeIds.CustomerCompany,
                TypeId = dto.TypeId,
                StatusId = MessagingConversationStatusIds.Open,
                CustomerPersonId = dto.CustomerPersonId,
                RelatedEntityTypeId = dto.RelatedEntityTypeId,
                RelatedEntityId = dto.RelatedEntityId,
                AssignedToUserId = dto.AssignedToUserId,
                CreateUserId = currentUserId,
                LastMessageAt = null
            };

            conversation.Participants.Add(new MessagingConversationParticipant
            {
                Id = Guid.CreateVersion7(),
                ConversationId = conversation.Id,
                UserId = currentUserId,
                KindId = MessagingParticipantKindIds.Internal,
                RoleId = MessagingParticipantRoleIds.Owner,
                IsPrimary = true,
                JoinedAt = DateTime.Now
            });

            _db.Set<MessagingConversation>().Add(conversation);
            await _db.SaveChangesAsync(cancellationToken);

            return conversation.Id;
        }

        public async Task<Guid> CreatePartnerConversationAsync(
            CreatePartnerConversationDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateRelatedEntity(dto.RelatedEntityTypeId, dto.RelatedEntityId);

            var conversation = new MessagingConversation
            {
                Id = Guid.CreateVersion7(),
                Subject = dto.Subject.Trim(),
                ScopeId = MessagingConversationScopeIds.PartnerCompany,
                TypeId = dto.TypeId,
                StatusId = MessagingConversationStatusIds.Open,
                PartnerPersonId = dto.PartnerPersonId,
                PartnerKindId = dto.PartnerKindId,
                RelatedEntityTypeId = dto.RelatedEntityTypeId,
                RelatedEntityId = dto.RelatedEntityId,
                AssignedToUserId = dto.AssignedToUserId,
                CreateUserId = currentUserId,
                LastMessageAt = null
            };

            conversation.Participants.Add(new MessagingConversationParticipant
            {
                Id = Guid.CreateVersion7(),
                ConversationId = conversation.Id,
                UserId = currentUserId,
                KindId = MessagingParticipantKindIds.Internal,
                RoleId = MessagingParticipantRoleIds.Owner,
                IsPrimary = true,
                JoinedAt = DateTime.Now
            });

            _db.Set<MessagingConversation>().Add(conversation);
            await _db.SaveChangesAsync(cancellationToken);

            return conversation.Id;
        }

        public async Task<Guid> AddMessageAsync(
            AddMessageDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var participantExists = await _db.Set<MessagingConversationParticipant>()
                .AnyAsync(x =>
                    x.ConversationId == dto.ConversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (!participantExists)
                throw new InvalidOperationException("Current user is not a participant of this conversation.");

            var body = dto.Body?.Trim();

            var attachments = dto.Attachments?
                .Where(x => x is not null && x.Length > 0)
                .ToList() ?? new List<IFormFile>();

            if (string.IsNullOrWhiteSpace(body) && attachments.Count == 0)
                throw new InvalidOperationException("Message body or at least one attachment is required.");

            ValidateAttachments(attachments);
            bool v = await ValidateAttachmentsSecurity(attachments);
            if (!v) { throw new InvalidOperationException("File mismatch."); }

            var conversation = await _db.Set<MessagingConversation>()
                .FirstOrDefaultAsync(x => x.Id == dto.ConversationId, cancellationToken);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found.");

            var now = DateTime.Now;

            var message = new MessagingMessage
            {
                Id = Guid.CreateVersion7(),
                ConversationId = dto.ConversationId,
                SenderUserId = currentUserId,
                Body = body,
                VisibilityId = dto.VisibilityId,
                SentAt = now,
                CreateUserId = currentUserId,
                CreatedAt = now
            };

            var savedPhysicalFiles = new List<string>();

            try
            {
                _db.Set<MessagingMessage>().Add(message);

                if (attachments.Count > 0)
                {
                    var dateSegment = now.ToString("yyyy/MM/dd")
                        .Replace('/', Path.DirectorySeparatorChar);

                    var relativeDirectory = Path.Combine(
                        "App_Data",
                        "Attachments",
                        "Messaging",
                        dateSegment);

                    var physicalDirectory = Path.Combine(
                        _environment.ContentRootPath,
                        relativeDirectory);

                    Directory.CreateDirectory(physicalDirectory);

                    foreach (var file in attachments)
                    {
                        var originalFileName = Path.GetFileName(file.FileName);
                        var extension = Path.GetExtension(originalFileName);

                        var storedFileName = $"{Guid.CreateVersion7()}{extension}";
                        var physicalPath = Path.Combine(physicalDirectory, storedFileName);

                        await using (var stream = new FileStream(physicalPath, FileMode.CreateNew))
                        {
                            await file.CopyToAsync(stream, cancellationToken);
                        }

                        savedPhysicalFiles.Add(physicalPath);

                        var storagePath = Path.Combine(relativeDirectory, storedFileName);

                        message.Attachments.Add(new MessagingMessageAttachment
                        {
                            Id = Guid.CreateVersion7(),
                            MessageId = message.Id,
                            FileName = originalFileName,
                            StoredFileName = storedFileName,
                            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                                ? "application/octet-stream"
                                : file.ContentType,
                            FileSize = file.Length,
                            StoragePath = storagePath,
                            VisibilityId = dto.VisibilityId,
                            CreateUserId = currentUserId,
                            CreatedAt = now
                        });
                    }
                }

                conversation.LastMessageAt = now;

                await _db.SaveChangesAsync(cancellationToken);

                return message.Id;
            }
            catch
            {
                foreach (var filePath in savedPhysicalFiles)
                {
                    try
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    catch
                    {
                        // deliberately ignored
                    }
                }

                throw;
            }
        }

        public async Task AddParticipantAsync(
            AddParticipantDto dto,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _db.Set<MessagingConversation>()
                .FirstOrDefaultAsync(x => x.Id == dto.ConversationId, cancellationToken);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found.");

            var currentUserIsParticipant = await _db.Set<MessagingConversationParticipant>()
                .AnyAsync(x =>
                    x.ConversationId == dto.ConversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (!currentUserIsParticipant)
                throw new InvalidOperationException("Current user is not allowed to add participants.");

            ValidateParticipantKindAgainstScope(conversation.ScopeId, dto.KindId);

            var exists = await _db.Set<MessagingConversationParticipant>()
                .AnyAsync(x =>
                    x.ConversationId == dto.ConversationId &&
                    x.UserId == dto.UserId,
                    cancellationToken);

            if (exists)
                return;

            var participant = new MessagingConversationParticipant
            {
                Id = Guid.CreateVersion7(),
                ConversationId = dto.ConversationId,
                UserId = dto.UserId,
                KindId = dto.KindId,
                RoleId = dto.RoleId,
                IsPrimary = dto.IsPrimary,
                JoinedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            _db.Set<MessagingConversationParticipant>().Add(participant);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MessagingConversationListItemDto>> GetInboxAsync(
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            return await _db.Set<MessagingConversationParticipant>()
                .Where(p => p.UserId == currentUserId)
                .Select(p => new MessagingConversationListItemDto
                {
                    Id = p.Conversation.Id,
                    Subject = p.Conversation.Subject,
                    ScopeId = p.Conversation.ScopeId,
                    ScopeTitle = p.Conversation.Scope.Title,
                    TypeId = p.Conversation.TypeId,
                    TypeTitle = p.Conversation.Type.Title,
                    StatusId = p.Conversation.StatusId,
                    StatusTitle = p.Conversation.Status.Title,
                    CustomerPersonId = p.Conversation.CustomerPersonId,
                    PartnerPersonId = p.Conversation.PartnerPersonId,
                    PartnerKindId = p.Conversation.PartnerKindId,
                    AssignedToUserId = p.Conversation.AssignedToUserId,
                    AssignedToUserDisplayName = p.Conversation.AssignedToUser.FullName,
                    LastMessageAt = p.Conversation.LastMessageAt,
                    UnreadCount = p.Conversation.Messages.Count(m =>
                        m.SenderUserId != currentUserId &&
                        (p.LastReadAt == null || m.SentAt > p.LastReadAt))
                })
                .OrderByDescending(x => x.LastMessageAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MessagingMessageDto>> GetMessagesAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var isParticipant = await _db.Set<MessagingConversationParticipant>()
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (!isParticipant)
                throw new InvalidOperationException("Current user is not a participant of this conversation.");

            return await _db.Set<MessagingMessage>()
                .Where(x => x.ConversationId == conversationId)
                .OrderBy(x => x.SentAt)
                .Select(x => new MessagingMessageDto
                {
                    Id = x.Id,
                    ConversationId = x.ConversationId,
                    SenderUserId = x.SenderUserId,
                    SenderDisplayName = x.SenderUser.UserName!,
                    Body = x.Body,
                    VisibilityId = x.VisibilityId,
                    SentAt = x.SentAt,
                    IsMine = x.SenderUserId == currentUserId,
                    Attachments = x.Attachments
                        .Where(a => !a.IsDeleted)
                        .OrderBy(a => a.CreatedAt)
                        .Select(a => new MessageAttachmentDto
                        {
                            Id = a.Id,
                            MessageId = a.MessageId,
                            FileName = a.FileName,
                            ContentType = a.ContentType,
                            FileSize = a.FileSize,
                            DownloadUrl = $"/Messaging/DownloadAttachment/{a.Id}"
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);
        }





        public async Task MarkAsReadAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var participant = await _db.Set<MessagingConversationParticipant>()
                .FirstOrDefaultAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (participant == null)
                throw new InvalidOperationException("Current user is not a participant of this conversation.");


            var lastMessageAt = await _db.MessagingMessages
                    .AsNoTracking()
                    .Where(x => x.ConversationId == conversationId)
                    .MaxAsync(x => (DateTime?)x.CreatedAt, cancellationToken);

            participant.LastReadAt = lastMessageAt ?? DateTime.Now;

            await _db.SaveChangesAsync(cancellationToken);
        }

        private static void ValidateRelatedEntity(int? relatedEntityTypeId, Guid? relatedEntityId)
        {
            if (relatedEntityTypeId.HasValue != relatedEntityId.HasValue)
                throw new InvalidOperationException("RelatedEntityTypeId and RelatedEntityId must both be null or both have value.");
        }

        private static void ValidateParticipantKindAgainstScope(int scopeId, int kindId)
        {
            if (scopeId == MessagingConversationScopeIds.Internal &&
                kindId != MessagingParticipantKindIds.Internal)
            {
                throw new InvalidOperationException("Internal conversations can only have internal participants.");
            }

            if (scopeId == MessagingConversationScopeIds.CustomerCompany &&
                kindId == MessagingParticipantKindIds.ExternalPartner)
            {
                throw new InvalidOperationException("Customer conversations cannot have partner participants.");
            }

            if (scopeId == MessagingConversationScopeIds.PartnerCompany &&
                kindId == MessagingParticipantKindIds.Customer)
            {
                throw new InvalidOperationException("Partner conversations cannot have customer participants.");
            }
        }



        public async Task<IReadOnlyList<MessagingParticipantDto>> GetParticipantsAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var canAccess = await _db.MessagingConversationParticipants
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == currentUserId
                    ,
                    cancellationToken);

            if (!canAccess)
            {
                throw new UnauthorizedAccessException("User does not have access to this conversation.");
            }

            return await _db.MessagingConversationParticipants
                .Where(x => x.ConversationId == conversationId)
                //.OrderByDescending(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new MessagingParticipantDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserDisplayName = x.User.FullName ?? x.User.UserName,
                    LastReadAt = x.LastReadAt,
                    hasProfilePicture = x.User.Pic != null
                })
                .ToListAsync(cancellationToken);
        }


        public async Task AddInternalParticipantAsync(
            Guid conversationId,
            Guid participantUserId,
            Guid addedByUserId,
            CancellationToken cancellationToken = default)
        {
            var canManage = await _db.MessagingConversationParticipants
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == addedByUserId,
                    cancellationToken);

            if (!canManage)
            {
                throw new UnauthorizedAccessException("User does not have access to this conversation.");
            }

            var existingParticipant = await _db.MessagingConversationParticipants
                .FirstOrDefaultAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == participantUserId,
                    cancellationToken);

            if (existingParticipant is not null)
            {
                //if (!existingParticipant.IsActive)
                //{
                //    existingParticipant.IsActive = true;
                //    existingParticipant.EditUserId = addedByUserId;
                //    existingParticipant.UpdatedAt = DateTime.Now;
                //}

                await _db.SaveChangesAsync(cancellationToken);
                return;
            }

            _db.MessagingConversationParticipants.Add(
                new MessagingConversationParticipant
                {
                    ConversationId = conversationId,
                    UserId = participantUserId,
                    //IsActive = true,
                    CreateUserId = addedByUserId,
                    CreatedAt = DateTime.Now,
                    KindId = MessagingParticipantKindIds.Internal,
                    RoleId = MessagingParticipantRoleIds.Member
                });

            await _db.SaveChangesAsync(cancellationToken);
        }



        public async Task<MessagingUnreadHeaderDto> GetUnreadHeaderAsync(
            Guid userId,
            int take,
            CancellationToken cancellationToken)
        {
            take = Math.Clamp(take, 1, 20);

            var unreadConversations = await _db.MessagingConversationParticipants
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .Select(x => new
                {
                    x.ConversationId,
                    x.Conversation.Subject,
                    x.Conversation.LastMessageAt,
                    UnreadCount = _db.MessagingMessages.Count(m =>
                        m.ConversationId == x.ConversationId &&
                        m.SenderUserId != userId &&
                        (!x.LastReadAt.HasValue || m.CreatedAt > x.LastReadAt.Value))
                })
                .Where(x => x.UnreadCount > 0)
                .OrderByDescending(x => x.LastMessageAt)
                .Take(take)
                .ToListAsync(cancellationToken);

            var totalUnreadCount = await _db.MessagingConversationParticipants
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .Select(x => _db.MessagingMessages.Count(m =>
                    m.ConversationId == x.ConversationId &&
                    m.SenderUserId != userId &&
                    (!x.LastReadAt.HasValue || m.CreatedAt > x.LastReadAt.Value)))
                .SumAsync(cancellationToken);

            if (!unreadConversations.Any())
            {
                return new MessagingUnreadHeaderDto
                {
                    TotalUnreadCount = totalUnreadCount
                };
            }

            var conversationIds = unreadConversations
                .Select(x => x.ConversationId)
                .ToList();

            var lastMessages = await _db.MessagingMessages
                .AsNoTracking()
                .Where(x => conversationIds.Contains(x.ConversationId))
                .GroupBy(x => x.ConversationId)
                .Select(g => g
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new
                    {
                        x.ConversationId,
                        x.Body,
                        x.CreatedAt,
                        SenderDisplayName =
                            x.SenderUser.FullName ??
                            x.SenderUser.UserName ??
                            x.SenderUser.Email ??
                            string.Empty
                    })
                    .FirstOrDefault())
                .ToListAsync(cancellationToken);

            var lastMessagesByConversationId = lastMessages
                .Where(x => x is not null)
                .ToDictionary(x => x!.ConversationId);

            var items = unreadConversations
                .Select(x =>
                {
                    lastMessagesByConversationId.TryGetValue(x.ConversationId, out var lastMessage);

                    return new MessagingUnreadHeaderItemDto
                    {
                        ConversationId = x.ConversationId,
                        Subject = x.Subject,
                        SenderDisplayName = lastMessage?.SenderDisplayName ?? string.Empty,
                        LastMessagePreview = CreateMessagePreview(lastMessage?.Body),
                        LastMessageAt = lastMessage?.CreatedAt ?? x.LastMessageAt,
                        UnreadCount = x.UnreadCount
                    };
                })
                .ToList();

            return new MessagingUnreadHeaderDto
            {
                TotalUnreadCount = totalUnreadCount,
                Items = items
            };
        }


        public async Task<IReadOnlyList<Guid>> GetConversationParticipantUserIdsAsync(
            Guid conversationId,
            CancellationToken cancellationToken)
        {
            return await _db.MessagingConversationParticipants
                .AsNoTracking()
                .Where(x =>
                    x.ConversationId == conversationId)
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken);
        }



        private static string CreateMessagePreview(string? body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return string.Empty;
            }

            var normalized = body
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Trim();

            const int maxLength = 80;

            if (normalized.Length <= maxLength)
            {
                return normalized;
            }

            return normalized[..maxLength] + "...";
        }


        public async Task<bool> IsParticipantAsync(
            Guid conversationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _db.MessagingConversationParticipants
                .AsNoTracking()
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == userId,
                    cancellationToken);
        }

        public async Task<MessagingMessageDto?> GetMessageAsync(
            Guid conversationId,
            Guid messageId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var isParticipant = await _db.Set<MessagingConversationParticipant>()
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (!isParticipant)
                throw new InvalidOperationException("Current user is not a participant of this conversation.");

            var message = await _db.Set<MessagingMessage>()
                .Where(x => x.ConversationId == conversationId && x.Id == messageId)
                .Select(x => new MessagingMessageDto
                {
                    Id = x.Id,
                    ConversationId = x.ConversationId,
                    SenderUserId = x.SenderUserId,
                    SenderDisplayName = x.SenderUser.UserName!,
                    Body = x.Body,
                    VisibilityId = x.VisibilityId,
                    SentAt = x.SentAt,
                    IsMine = x.SenderUserId == currentUserId,
                    Attachments = x.Attachments
                        .Where(a => !a.IsDeleted)
                        .OrderBy(a => a.CreatedAt)
                        .Select(a => new MessageAttachmentDto
                        {
                            Id = a.Id,
                            MessageId = a.MessageId,
                            FileName = a.FileName,
                            ContentType = a.ContentType,
                            FileSize = a.FileSize,
                            DownloadUrl = $"/Messaging/DownloadAttachment/{a.Id}"
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (message == null)
                throw new InvalidOperationException("Message not found.");
            return message;
        }


        private static void ValidateAttachments(List<IFormFile> attachments)
        {
            if (attachments.Count == 0)
                return;

            const int maxFileCount = 10;
            const long maxFileSize = 10 * 1024 * 1024; // 10 MB

            if (attachments.Count > maxFileCount)
                throw new InvalidOperationException($"Maximum {maxFileCount} attachments are allowed.");

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ".pdf",
                            ".doc",
                            ".docx",
                            ".xls",
                            ".xlsx",
                            ".png",
                            ".jpg",
                            ".jpeg",
                            ".gif",
                            ".zip",
                            ".rar",
                            ".txt"
                        };

            foreach (var file in attachments)
            {
                if (file.Length <= 0)
                    throw new InvalidOperationException("One of the attachments is empty.");

                if (file.Length > maxFileSize)
                    throw new InvalidOperationException(
                        $"File '{file.FileName}' exceeds the maximum allowed size.");

                var originalFileName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(originalFileName);

                if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
                    throw new InvalidOperationException(
                        $"File type of '{file.FileName}' is not allowed.");
            }
        }


        public async Task<MessageAttachmentDownloadDto?> GetAttachmentForDownloadAsync(
            Guid attachmentId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var attachment = await _db.Set<MessagingMessageAttachment>()
                .Where(x => x.Id == attachmentId)
                .Select(x => new
                {
                    x.Id,
                    x.MessageId,
                    x.FileName,
                    x.ContentType,
                    x.StoragePath,
                    ConversationId = x.Message.ConversationId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (attachment == null)
                return null;

            var isParticipant = await _db.Set<MessagingConversationParticipant>()
                .AnyAsync(x =>
                    x.ConversationId == attachment.ConversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (!isParticipant)
                throw new UnauthorizedAccessException("Current user does not have access to this attachment.");

            return new MessageAttachmentDownloadDto
            {
                Id = attachment.Id,
                MessageId = attachment.MessageId,
                ConversationId = attachment.ConversationId,
                FileName = attachment.FileName,
                ContentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                    ? "application/octet-stream"
                    : attachment.ContentType,
                StoragePath = attachment.StoragePath
            };
        }

        private async Task<bool> ValidateAttachmentsSecurity(List<IFormFile> attachments)
        {
            var savedPhysicalPaths = new List<string>();

            if (attachments != null && attachments.Any())
            {
                if (attachments.Count > 10)
                    throw new InvalidOperationException("You can upload up to 10 files.");

                foreach (var file in attachments)
                {
                    var securityResult = await _attachmentSecurityService.ValidateAndScanAsync(
                        file);
                    if (!securityResult.IsAllowed)
                    {
                        throw new InvalidOperationException(
                             file.FileName + ":" + securityResult.RejectionReason ?? "File is not allowed.");
                        //return false;
                    }
                }
            }
            return true;
        }


        public async Task CloseConversationAsync(
            Guid conversationId,
            Guid currentUserId,
            CancellationToken cancellationToken)
        {
            var hasAccess = await _db.MessagingConversationParticipants
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == currentUserId,
                    cancellationToken);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation.");
            }

            var conversation = await _db.MessagingConversations
                .FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);

            if (conversation is null)
            {
                throw new InvalidOperationException("Conversation was not found.");
            }

            if (conversation.StatusId == MessagingConversationStatusIds.Closed)
            {
                return;
            }

            conversation.StatusId = MessagingConversationStatusIds.Closed;
            conversation.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
