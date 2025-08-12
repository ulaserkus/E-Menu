using System;

namespace Shared.Kernel.Dtos
{
    public class BotMessageDto
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public string Text { get; set; }
        public DateTime? Timestamp { get; set; }
        public string UserId { get; set; }

    }
}
