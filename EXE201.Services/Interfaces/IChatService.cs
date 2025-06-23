using EXE201.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Services.Interfaces
{
    public interface IChatService
    {
        Task<Conversation> GetOrCreateConversation(string user1Id, string user2Id);
        Task SaveMessage(int conversationId, string senderId, string messageText);
    }
}
