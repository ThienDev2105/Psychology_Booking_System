using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Commons.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public int ConversationId { get; set; }

        //Relation
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; }

    }
}
