using EXE201.Commons.Data;
using EXE201.Commons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Serenity_Solution.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị danh sách conversation của user hiện tại
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = _userManager.GetUserId(User);

                var conversations = await _context.Conversations
                    .Include(c => c.User1)
                    .Include(c => c.User2)
                    .Include(c => c.LastMessage)
                    .Where(c => c.User1Id == currentUserId || c.User2Id == currentUserId)
                    .OrderByDescending(c => c.LastMessage.Timestamp)
                    .ToListAsync();

                if (conversations == null)
                {
                    return NotFound();
                }
                ViewBag.CurrentUserId = currentUserId;
                return View(conversations);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return Content("Lỗi: " + ex.Message + (ex.InnerException?.Message ?? ""));
            }
        }

        // Hiển thị chi tiết conversation và messages
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            var conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => c.ConversationId == id &&
                    (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (conversation == null)
            {
                return NotFound();
            }

            // Sắp xếp messages theo thời gian
            conversation.Messages = conversation.Messages.OrderBy(m => m.Timestamp).ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.OtherUser = conversation.User1Id == currentUserId ? conversation.User2 : conversation.User1;

            return View(conversation);
        }

        // Gửi tin nhắn
        [HttpPost]
        public async Task<IActionResult> SendMessage(int conversationId, string messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                return Json(new { success = false, message = "Tin nhắn không được để trống" });
            }

            var currentUserId = _userManager.GetUserId(User);

            // Kiểm tra conversation có tồn tại và user có quyền truy cập
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                    (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (conversation == null)
            {
                return Json(new { success = false, message = "Không tìm thấy cuộc trò chuyện" });
            }

            // Tạo message mới
            var message = new Message
            {
                SenderId = currentUserId,
                MessageText = messageText.Trim(),
                Timestamp = DateTime.Now,
                ConversationId = conversationId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Cập nhật LastMessage của conversation
            conversation.LastMessageId = message.MessageId;
            await _context.SaveChangesAsync();

            // Lấy thông tin sender để trả về
            var sender = await _userManager.FindByIdAsync(currentUserId);

            return Json(new
            {
                success = true,
                messageId = message.MessageId,
                senderName = sender.Name,
                messageText = message.MessageText,
                timestamp = message.Timestamp.ToString("HH:mm dd/MM/yyyy")
            });
        }

        // Load messages mới (dùng cho AJAX polling hoặc refresh)
        [HttpGet]
        public async Task<IActionResult> GetNewMessages(int conversationId, int lastMessageId = 0)
        {
            var currentUserId = _userManager.GetUserId(User);

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                    (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (conversation == null)
            {
                return Json(new { success = false });
            }

            var newMessages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId && m.MessageId > lastMessageId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    messageId = m.MessageId,
                    senderId = m.SenderId,
                    senderName = m.Sender.Name,
                    messageText = m.MessageText,
                    timestamp = m.Timestamp.ToString("HH:mm dd/MM/yyyy")
                })
                .ToListAsync();

            return Json(new { success = true, messages = newMessages });
        }
        // Thêm method này vào ChatController của bạn

        [HttpGet]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                    (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (conversation == null)
            {
                return Json(new { success = false, message = "Không tìm thấy cuộc trò chuyện" });
            }

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    messageId = m.MessageId,
                    senderId = m.SenderId,
                    senderName = m.Sender.Name,
                    messageText = m.MessageText,
                    timestamp = m.Timestamp.ToString("HH:mm dd/MM/yyyy")
                })
                .ToListAsync();

            return Json(new { success = true, messages = messages });
        }

        // Tạo conversation mới (gọi sau khi thanh toán)
        [HttpPost]
        public async Task<IActionResult> CreateConversation(string user1Id, string user2Id)
        {
            // Kiểm tra conversation đã tồn tại chưa
            var existingConversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == user1Id && c.User2Id == user2Id) ||
                    (c.User1Id == user2Id && c.User2Id == user1Id));

            if (existingConversation != null)
            {
                return Json(new { success = true, conversationId = existingConversation.ConversationId });
            }

            // Tạo conversation mới
            var conversation = new Conversation
            {
                User1Id = user1Id,
                User2Id = user2Id
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            return Json(new { success = true, conversationId = conversation.ConversationId });
        }
    }
}
