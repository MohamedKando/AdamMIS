using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AdamMIS.Entities.Messaging;

namespace AdamMIS.Abstractions.SignalR
{
    [Authorize] // Ensure only authenticated users can connect
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                // Join user to their personal group for receiving messages
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                Console.WriteLine($"User {userId} connected to SignalR");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                Console.WriteLine($"User {userId} disconnected from SignalR");
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Method to send a message to a specific user (saves to database)
        public async Task SendMessageToUser(string recipientId, string content)
        {
            var senderId = Context.UserIdentifier;
            if (senderId == null) return;

            try
            {
                // Save message to database
                var message = new Messagee
                {
                    SenderId = senderId,
                    RecipientId = recipientId,
                    Content = content.Trim(),
                    SentAt = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                var messageDto = new
                {
                    id = message.Id,
                    senderId = message.SenderId,
                    recipientId = message.RecipientId,
                    content = message.Content,
                    sentAt = message.SentAt,
                    isRead = message.IsRead,
                    readAt = message.ReadAt
                };

                // Send to the recipient
                await Clients.Group($"user_{recipientId}").SendAsync("ReceiveMessage", messageDto);

                // Send confirmation back to sender
                await Clients.Caller.SendAsync("MessageSent", messageDto);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("MessageError", $"Failed to send message: {ex.Message}");
            }
        }

        // Method to mark messages as read
        public async Task MarkAsRead(string senderId)
        {
            var currentUserId = Context.UserIdentifier;
            if (currentUserId == null) return;

            try
            {
                var messages = await _context.Messages
                    .Where(m => m.SenderId == senderId && m.RecipientId == currentUserId && !m.IsRead)
                    .ToListAsync();

                if (messages.Any())
                {
                    var readTime = DateTime.UtcNow;
                    foreach (var message in messages)
                    {
                        message.IsRead = true;
                        message.ReadAt = readTime;
                    }

                    await _context.SaveChangesAsync();

                    // Notify sender that messages were read
                    await Clients.Group($"user_{senderId}")
                        .SendAsync("MessagesRead", new
                        {
                            readerId = currentUserId,
                            messageIds = messages.Select(m => m.Id)
                        });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("MessageError", $"Failed to mark messages as read: {ex.Message}");
            }
        }

        // Join a specific conversation room (optional - for future group chat features)
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        // Leave a specific conversation room (optional - for future group chat features)
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        // Typing indicator
        public async Task SendTypingIndicator(string recipientId, bool isTyping)
        {
            var senderId = Context.UserIdentifier;
            if (senderId != null)
            {
                await Clients.Group($"user_{recipientId}").SendAsync("TypingIndicator", new
                {
                    senderId = senderId,
                    isTyping = isTyping
                });
            }
        }
    }
}