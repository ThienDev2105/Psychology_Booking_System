
using System;
using System.Collections.Generic;

namespace Serenity_Solution.Models

{
    public class ChatbotViewModel
    {
        public string Message { get; set; }
    }

    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Sender { get; set; } // "user" or "bot"
    }

    public class ChatbotResponse
    {
        public string Response { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

} 

