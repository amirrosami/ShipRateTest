using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.Messaging;
using ShipRate.Application.Hubs;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Domain.Entities.Security;
using System.Security.Claims;

namespace ShipRate.WebAppCustomers.Controllers
{
    [Authorize]
    public sealed class MessagingController : Controller
    {
        private const int GeneralConversationTypeId = 1;
        private const int InternalOnlyVisibilityId = 2;
        private readonly IMessagingService _messagingService;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<MessagingHub> _messagingHubContext;
        private readonly IWebHostEnvironment _environment;

        public MessagingController(IMessagingService messagingService, UserManager<ApplicationUser> userManager, IHubContext<MessagingHub> messagingHubContext, IWebHostEnvironment environment)
        {
            _messagingService = messagingService;
            _userManager = userManager;
            _messagingHubContext = messagingHubContext;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            var conversations = await _messagingService.GetInboxAsync(currentUserId, cancellationToken);

            var model = new MessagingIndexViewModel
            {
                Conversations = conversations
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> InboxList(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            var conversations = await _messagingService.GetInboxAsync(currentUserId, cancellationToken);

            return PartialView("_InboxList", conversations);
        }

        [HttpGet]
        public async Task<IActionResult> CreateInternal(CancellationToken cancellationToken)
        {
            var model = new CreateInternalConversationViewModel
            {
                TypeId = GeneralConversationTypeId
            };

            await PopulateInternalUsersAsync(model, cancellationToken);

            return PartialView("_CreateInternalConversationModal", model);
        }

        private async Task PopulateInternalUsersAsync(
            CreateInternalConversationViewModel model,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            model.InternalUsers = await _userManager.Users
                .Where(x => x.Id != currentUserId)
                .OrderBy(x => x.UserName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync(cancellationToken);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInternal(
    CreateInternalConversationViewModel model,
    CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateInternalUsersAsync(model, cancellationToken);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                return PartialView("_CreateInternalConversationModal", model);
            }

            var currentUserId = GetCurrentUserId();

            var participantUserIds = model.ParticipantUserIds
                .Distinct()
                .ToList();

            if (model.AssignedToUserId is not null && model.AssignedToUserId != Guid.Empty)
            {
                participantUserIds.Add(model.AssignedToUserId.Value);
            }

            participantUserIds = participantUserIds
                .Where(x => x != currentUserId)
                .Distinct()
                .ToList();

            var conversationId = await _messagingService.CreateInternalConversationAsync(
                new CreateInternalConversationDto
                {
                    Subject = model.Subject.Trim(),
                    TypeId = model.TypeId,
                    InitialMessage = model.InitialMessage.Trim(),
                    AssignedToUserId = model.AssignedToUserId,
                    ParticipantUserIds = participantUserIds
                },
                currentUserId,
                cancellationToken);

            var redirectUrl = Url.Action(nameof(Conversation), new { id = conversationId });

            if (Request.Headers.ContainsKey("HX-Request"))
            {
                Response.Headers["HX-Redirect"] = redirectUrl;
                return NoContent();
            }

            return Redirect(redirectUrl!);
        }
        [HttpGet]
        public async Task<IActionResult> Conversation(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            await _messagingService.MarkAsReadAsync(id, currentUserId, cancellationToken);

            var inbox = await _messagingService.GetInboxAsync(currentUserId, cancellationToken);
            var messages = await _messagingService.GetMessagesAsync(id, currentUserId, cancellationToken);
            var participants = await _messagingService.GetParticipantsAsync(id, currentUserId, cancellationToken);

            var conversation = inbox.FirstOrDefault(x => x.Id == id);


            //var model = new MessagingConversationViewModel
            //{
            //    ConversationId = id,
            //    Conversation = inbox.FirstOrDefault(x => x.Id == id),
            //    Messages = messages,
            //    SendMessage = new SendInternalMessageViewModel
            //    {
            //        ConversationId = id
            //    }
            //};
            var model = new MessagingConversationViewModel
            {
                ConversationId = id,
                Messages = messages,
                Conversation = inbox.FirstOrDefault(x => x.Id == id),
                AssignedToUserId = conversation.AssignedToUserId,
                AssignedToDisplayName = conversation.AssignedToUserDisplayName,
                Participants = participants.ToList(),
                SendMessage = new SendInternalMessageViewModel
                {
                    ConversationId = id
                },
                AddParticipant = new AddInternalParticipantViewModel
                {
                    ConversationId = id
                }
            };

            await PopulateAvailableParticipantsAsync(
                    model.AddParticipant,
                    cancellationToken);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConversationMessages(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            await _messagingService.MarkAsReadAsync(id, currentUserId, cancellationToken);

            var messages = await _messagingService.GetMessagesAsync(id, currentUserId, cancellationToken);
            Response.Headers["HX-Trigger"] = "messagingChanged";

            return PartialView("_ConversationMessages", messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(
            SendInternalMessageViewModel model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                Response.Headers["HX-Retarget"] = "#sendMessageFormHost";
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                //return PartialView("_SendMessageForm", model);
                return PartialView("_SendMessageForm", new SendInternalMessageViewModel
                {
                    ConversationId = model.ConversationId
                });
            }

            var hasBody = !string.IsNullOrWhiteSpace(model.Body);
            var hasAttachments = model.Attachments != null && model.Attachments.Any(x => x.Length > 0);

            if (!hasBody && !hasAttachments)
            {
                ModelState.AddModelError(nameof(model.Body), "Please enter a message or attach at least one file.");
                return PartialView("_SendMessageForm", new SendInternalMessageViewModel
                {
                    ConversationId = model.ConversationId
                });

            }



            var currentUserId = GetCurrentUserId();






            Guid messageId;

            try
            {
                messageId = await _messagingService.AddMessageAsync(
                    new AddMessageDto
                    {
                        ConversationId = model.ConversationId,
                        Body = model.Body?.Trim(),
                        VisibilityId = InternalOnlyVisibilityId,
                        Attachments = model.Attachments
                    },
                    currentUserId,
                    cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("_SendMessageForm", new SendInternalMessageViewModel { ConversationId = model.ConversationId });
            }



            await _messagingHubContext.Clients
                .Group(MessagingHub.GetConversationGroupName(model.ConversationId))
                .SendAsync("messageCreated", new
                {
                    conversationId = model.ConversationId,
                    messageId = messageId
                }, cancellationToken);

            var participantUserIds = await _messagingService.GetConversationParticipantUserIdsAsync(
                model.ConversationId,
                cancellationToken);

            foreach (var participantUserId in participantUserIds)
            {
                await _messagingHubContext.Clients
                    .Group(MessagingHub.GetUserGroupName(participantUserId))
                    .SendAsync("messagingChanged", new
                    {
                        conversationId = model.ConversationId,
                        messageId = messageId
                    }, cancellationToken);
            }


            Response.Headers["HX-Trigger"] = "messaging:message-sent, messagingChanged";

            //return PartialView("_ConversationMessages", messages);


            return PartialView("_SendMessageForm", new SendInternalMessageViewModel
            {
                ConversationId = model.ConversationId
            });


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid conversationId, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            await _messagingService.MarkAsReadAsync(conversationId, currentUserId, cancellationToken);

            return Ok();
        }

        private Guid GetCurrentUserId()
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("UserId") ??
                User.FindFirstValue("sub");

            if (!Guid.TryParse(value, out var userId))
            {
                throw new InvalidOperationException("Current user id claim is missing or invalid.");
            }

            return userId;
        }

        private async Task PopulateAvailableParticipantsAsync(
            AddInternalParticipantViewModel model,
            CancellationToken cancellationToken)
        {
            var existingUserIds = await _messagingService.GetParticipantsAsync(
                model.ConversationId,
                GetCurrentUserId(),
                cancellationToken);

            var excludedUserIds = existingUserIds
                .Select(x => x.UserId)
                .ToHashSet();

            model.AvailableUsers = await _userManager.Users
                .Where(x => !excludedUserIds.Contains(x.Id))
                .OrderBy(x => x.UserName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName ?? x.UserName
                })
                .ToListAsync(cancellationToken);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddParticipant(
            AddInternalParticipantViewModel model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAvailableParticipantsAsync(model, cancellationToken);

                Response.Headers["HX-Retarget"] = "#addParticipantFormHost";
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                return PartialView("_AddParticipantForm", model);
            }

            var currentUserId = GetCurrentUserId();

            await _messagingService.AddInternalParticipantAsync(
                model.ConversationId,
                model.UserId,
                currentUserId,
                cancellationToken);

            var participants = await _messagingService.GetParticipantsAsync(
                model.ConversationId,
                currentUserId,
                cancellationToken);

            Response.Headers["HX-Trigger"] = "messaging:participant-added";

            return PartialView("_ConversationParticipants", participants);
        }
        [HttpGet]
        public async Task<IActionResult> AddParticipantForm(
            Guid conversationId,
            CancellationToken cancellationToken)
        {
            var model = new AddInternalParticipantViewModel
            {
                ConversationId = conversationId
            };

            await PopulateAvailableParticipantsAsync(model, cancellationToken);

            return PartialView("_AddParticipantForm", model);
        }


        [HttpGet]
        public async Task<IActionResult> HeaderUnread(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var model = await _messagingService.GetUnreadHeaderAsync(
                currentUserId,
                take: 5,
                cancellationToken);

            return PartialView("_HeaderUnreadMessages", model);
        }


        [HttpGet]
        public async Task<IActionResult> Navbar(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var model = await _messagingService.GetUnreadHeaderAsync(
                currentUserId,
                take: 5,
                cancellationToken);

            return PartialView("_NavbarMessages", model);
        }


        [HttpGet]
        public async Task<IActionResult> ConversationMessage(
            Guid conversationId,
            Guid messageId,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var message = await _messagingService.GetMessageAsync(
                conversationId,
                messageId,
                currentUserId,
                cancellationToken);

            if (message is null)
            {
                return NotFound();
            }

            await _messagingService.MarkAsReadAsync(
                conversationId,
                currentUserId,
                cancellationToken);

            Response.Headers["HX-Trigger"] = "messagingChanged";

            return PartialView("_ConversationMessage", message);
        }



        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            MessageAttachmentDownloadDto? attachment;
            try
            {
                attachment = await _messagingService.GetAttachmentForDownloadAsync(
                    id,
                    currentUserId,
                    cancellationToken);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            if (attachment == null)
                return NotFound();

            var physicalPath = Path.Combine(_environment.ContentRootPath, attachment.StoragePath);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            return PhysicalFile(
                physicalPath,
                attachment.ContentType,
                attachment.FileName);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseConversation(Guid conversationId, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            await _messagingService.CloseConversationAsync(
                conversationId,
                currentUserId,
                cancellationToken);

            var conversations = await _messagingService.GetInboxAsync(
                currentUserId,
                cancellationToken);

            Response.Headers["HX-Trigger"] = "messagingChanged";

            return PartialView("_InboxList", conversations);
        }








    }
}
