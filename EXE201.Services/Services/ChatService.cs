using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Services.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation> GetOrCreateConversation(string user1Id, string user2Id)
        {
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == user1Id && c.User2Id == user2Id) ||
                    (c.User1Id == user2Id && c.User2Id == user1Id));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = user1Id,
                    User2Id = user2Id
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            return conversation;
        }

        public async Task SaveMessage(int conversationId, string senderId, string messageText)
        {
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                MessageText = messageText,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(message);

            var conversation = await _context.Conversations.FindAsync(conversationId);
            if (conversation != null)
            {
                conversation.LastMessage = message;
                conversation.LastMessageId = message.MessageId;
            }

            await _context.SaveChangesAsync();
        }
    }

}
