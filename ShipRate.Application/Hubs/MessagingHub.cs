using Microsoft.AspNetCore.SignalR;
using ShipRate.Application.Interfaces.Messaging;
using System.Security.Claims;

namespace ShipRate.Application.Hubs
{
    public class MessagingHub : Hub
    {
        private readonly IMessagingService _messagingService;

        public MessagingHub(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(Guid.Parse(userId)));
            }

            await base.OnConnectedAsync();
        }


        public async Task JoinConversation(Guid conversationId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var canAccess = await _messagingService.IsParticipantAsync(
                conversationId,
                Guid.Parse(userId),
                Context.ConnectionAborted);

            if (!canAccess)
            {
                return;
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetConversationGroupName(conversationId));
        }

        //public async Task JoinConversation(Guid conversationId)
        //{
        //    await Groups.AddToGroupAsync(
        //        Context.ConnectionId,
        //        GetConversationGroupName(conversationId));
        //}

        public async Task LeaveConversation(Guid conversationId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GetConversationGroupName(conversationId));
        }

        public static string GetUserGroupName(Guid userId)
        {
            return $"user:{userId}";
        }

        public static string GetConversationGroupName(Guid conversationId)
        {
            return $"conversation:{conversationId}";
        }
    }
}
