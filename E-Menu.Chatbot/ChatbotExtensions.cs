using E_Menu.Chatbot.DirectLine;
using E_Menu.Chatbot.Interfaces;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Menu.Chatbot;

public static class ChatbotExtensions
{
    public static IServiceCollection AddChatbotService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDirectLineClient>(sp =>
        {
            var secret = configuration["DirectLine:Secret"] ?? "";
            var url = configuration["DirectLine:Url"] ?? "https://directline.botframework.com/";
            var credentials = new DirectLineClientCredentials(secret, endpoint: url);
            return new DirectLineClient(new Uri(url), credentials);
        });
        services.AddScoped<IChatbotManager, ChatbotManager>();
        return services;
    }

}
