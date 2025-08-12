namespace Shared.Kernel.Dtos
{
    public class ConversationDto
    {
        public string ConversationId { get; set; }
        public string StreamUrl { get; set; }
        public string Token { get; set; }
        public int? ExpiresIn { get; set; }
    }
}
