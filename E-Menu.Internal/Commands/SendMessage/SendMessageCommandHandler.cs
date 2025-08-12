using E_Menu.Chatbot.Interfaces;
using MediatR;
using Shared.Kernel;

namespace E_Menu.Internal.Commands.SendMessage;

internal class SendMessageCommandHandler(IChatbotManager chatbotManager) : IRequestHandler<SendMessageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        await chatbotManager.SendMessageAsync(request.ConversationId, request.Message, request.UserId);
        return Result<bool>.Success(true);
    }
}