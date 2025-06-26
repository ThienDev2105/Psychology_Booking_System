using EXE201.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Repository.Interfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        void Update(Conversation conversation);
    }
}
