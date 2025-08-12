using E_Menu.Chatbot.Interfaces;
using MediatR;
using Shared.Kernel;
using Shared.Kernel.Dtos;

namespace E_Menu.Internal.Queries.ListBotMessages;

internal class ListBotMessagesQueryHandler(IChatbotManager chatbotManager) : IRequestHandler<ListBotMessagesQuery, Result<IEnumerable<BotMessageDto>>>
{
    public async Task<Result<IEnumerable<BotMessageDto>>> Handle(ListBotMessagesQuery request, CancellationToken cancellationToken)
    {
        var listOfBotMessages = new List<BotMessageDto>();

        var messages = await chatbotManager.GetMessagesAsync(request.ConversationId, request.Watermark);

        if (messages.Activities == null)
        {
            return Result<IEnumerable<BotMessageDto>>.Success(listOfBotMessages);
        }

        foreach (var message in messages.Activities)
        {
            var botMessageDto = new BotMessageDto
            {
                Id = message.Id,
                Text = message.Text,
                Timestamp = message.Timestamp,
                UserId = message.From?.Id,
                ConversationId = request.ConversationId
            };
            listOfBotMessages.Add(botMessageDto);
        }

        return Result<IEnumerable<BotMessageDto>>.Success(listOfBotMessages);
    }
}


