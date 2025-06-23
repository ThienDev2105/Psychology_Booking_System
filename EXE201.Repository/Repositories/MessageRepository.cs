
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
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }
        public void Update(Message message)
        {
            _context.Messages.Update(message);
        }
    }
}
