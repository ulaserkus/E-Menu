using Microsoft.Bot.Connector.DirectLine;

namespace E_Menu.Chatbot.Interfaces;

public interface IChatbotManager
{
    Task<Conversation> StartConversationAsync();

    Task SendMessageAsync(string conversationId, string message, string userId);

    Task<ActivitySet> GetMessagesAsync(string conversationId, string watermark = null);
}
