using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Repository.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }
        public void Update(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
        }
    }
}
