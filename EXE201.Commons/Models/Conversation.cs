using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Commons.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public int? LastMessageId { get; set; }

        public ICollection<Message> Messages { get; set; }

        // Relation
        [ForeignKey("User1Id")]
        public User User1 { get; set; }
        [ForeignKey("User2Id")]
        public User User2 { get; set; }

        [ForeignKey("LastMessageId")]
        public Message LastMessage { get; set; }
    }
}
