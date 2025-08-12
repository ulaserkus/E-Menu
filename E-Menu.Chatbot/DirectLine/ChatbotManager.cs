using E_Menu.Chatbot.Interfaces;
using Microsoft.Bot.Connector.DirectLine;

namespace E_Menu.Chatbot.DirectLine;

public class ChatbotManager : IChatbotManager
{
    private readonly IDirectLineClient _client;

    public ChatbotManager(IDirectLineClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client), "DirectLine client cannot be null.");
    }

    public async Task<Conversation> StartConversationAsync()
    {
        return await _client.Conversations.StartConversationAsync();
    }


    public async Task SendMessageAsync(string conversationId, string message, string user)
    {
        if (string.IsNullOrEmpty(conversationId))
            throw new ArgumentException("Conversation ID cannot be null or empty.", nameof(conversationId));
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));
        var activity = new Activity
        {
            Type = ActivityTypes.Message,
            Text = message,
            From = new ChannelAccount(user),
            Locale = "tr-TR",


        };
        await _client.Conversations.PostActivityAsync(conversationId, activity);
    }

    public async Task<ActivitySet> GetMessagesAsync(string conversationId, string watermark = null)
    {
        if (string.IsNullOrEmpty(conversationId))
            throw new ArgumentException("Conversation ID cannot be null or empty.", nameof(conversationId));
        return await _client.Conversations.GetActivitiesAsync(conversationId, watermark);
    }
}
