using AdamMIS.Abstractions.SignalR;
using AdamMIS.Entities.Messaging;
using AdamMIS.Entities.UserEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdamMIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(
            AppDbContext context,
            IHubContext<ChatHub> hubContext,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        // Get all users available for chat (excluding current user)
        [HttpGet("users")]
        public async Task<IActionResult> GetAvailableUsers()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var users = await _userManager.Users
                .Where(u => u.Id != currentUserId && !u.IsDisabled)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Title,
                    Department = u.Department != null ? u.Department.Name : null,
                    u.PhotoPath,
                    u.InternalPhone
                })
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return Ok(users);
        }

        // Get conversation between current user and another user
        [HttpGet("conversation/{otherUserId}")]
        public async Task<IActionResult> GetConversation(string otherUserId, int page = 1, int pageSize = 50)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate that the other user exists
            var otherUser = await _userManager.FindByIdAsync(otherUserId);
            if (otherUser == null)
            {
                return NotFound("User not found");
            }

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
                           (m.SenderId == otherUserId && m.RecipientId == currentUserId))
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.RecipientId,
                    m.Content,
                    m.SentAt,
                    m.IsRead,
                    m.ReadAt
                })
                .ToListAsync();

            return Ok(messages.OrderBy(m => m.SentAt)); // Return in chronological order
        }

        // Send a new message
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate recipient exists
            var recipient = await _userManager.FindByIdAsync(dto.RecipientId);
            if (recipient == null)
            {
                return BadRequest("Recipient not found");
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest("Message content cannot be empty");
            }

            var message = new Messagee
            {
                SenderId = currentUserId!,
                RecipientId = dto.RecipientId,
                Content = dto.Content.Trim(),
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var messageResponse = new
            {
                id = message.Id,
                senderId = message.SenderId,
                recipientId = message.RecipientId,
                content = message.Content,
                sentAt = message.SentAt,
                isRead = message.IsRead,
                readAt = message.ReadAt
            };

            // Send real-time notification via SignalR
            await _hubContext.Clients.Group($"user_{dto.RecipientId}")
                .SendAsync("ReceiveMessage", messageResponse);

            // Also notify sender for confirmation
            await _hubContext.Clients.Group($"user_{currentUserId}")
                .SendAsync("MessageSent", messageResponse);

            return Ok(messageResponse);
        }

        // Mark messages as read
        [HttpPut("mark-read/{senderId}")]
        public async Task<IActionResult> MarkAsRead(string senderId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate sender exists
            var sender = await _userManager.FindByIdAsync(senderId);
            if (sender == null)
            {
                return BadRequest("Sender not found");
            }

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
                await _hubContext.Clients.Group($"user_{senderId}")
                    .SendAsync("MessagesRead", new
                    {
                        readerId = currentUserId,
                        messageIds = messages.Select(m => m.Id)
                    });
            }

            return Ok();
        }

        // Get list of recent conversations with user details
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // First, get all messages for the current user and process in memory
            var userMessages = await _context.Messages
                .Where(m => m.SenderId == currentUserId || m.RecipientId == currentUserId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            // If no messages, return empty array
            if (!userMessages.Any())
            {
                return Ok(new object[0]);
            }

            // Group messages by conversation partner (in memory)
            var conversationData = userMessages
                .GroupBy(m => m.SenderId == currentUserId ? m.RecipientId : m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessage = g.First(), // Already ordered by SentAt descending
                    UnreadCount = g.Count(m => m.RecipientId == currentUserId && !m.IsRead)
                })
                .OrderByDescending(c => c.LastMessage.SentAt)
                .ToList();

            // Get user details for all conversation participants
            var userIds = conversationData.Select(c => c.UserId).ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Title,
                    Department = u.Department != null ? u.Department.Name : null,
                    u.PhotoPath,
                    u.InternalPhone
                })
                .ToListAsync();

            // Combine conversation data with user details
            var conversations = conversationData.Select(c => new
            {
                User = users.FirstOrDefault(u => u.Id == c.UserId),
                LastMessage = new
                {
                    c.LastMessage.Id,
                    c.LastMessage.SenderId,
                    c.LastMessage.RecipientId,
                    c.LastMessage.Content,
                    c.LastMessage.SentAt,
                    c.LastMessage.IsRead,
                    c.LastMessage.ReadAt
                },
                UnreadCount = c.UnreadCount
            }).ToList();

            return Ok(conversations);
        }

        // Get unread message count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var count = await _context.Messages
                .CountAsync(m => m.RecipientId == currentUserId && !m.IsRead);

            return Ok(new { count });
        }

        // Search users for starting new conversations
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] int limit = 10)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty");
            }

            var users = await _userManager.Users
                .Where(u => u.Id != currentUserId &&
                           !u.IsDisabled &&
                           (u.UserName!.Contains(query) ||
                            u.Email!.Contains(query) ||
                            (u.Title != null && u.Title.Contains(query))))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Title,
                    Department = u.Department != null ? u.Department.Name : null,
                    u.PhotoPath,
                    u.InternalPhone
                })
                .Take(limit)
                .ToListAsync();

            return Ok(users);
        }
    }

    // Only DTO we need for input validation
    public class SendMessageDto
    {
        public string RecipientId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}