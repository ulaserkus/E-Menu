using E_Menu.Chatbot.Interfaces;
using MediatR;
using Shared.Kernel;
using Shared.Kernel.Dtos;

namespace E_Menu.Internal.Commands.StartConversation;

internal class StartConversationCommandHandler(IChatbotManager chatbotManager) : IRequestHandler<StartConversationCommand, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await chatbotManager.StartConversationAsync();

        if (conversation == null)
        {
            return Result<ConversationDto>.Failure("Failed to start conversation.");
        }

        var dto = new ConversationDto
        {
            ConversationId = conversation.ConversationId,
            StreamUrl = conversation.StreamUrl,
            ExpiresIn = conversation.ExpiresIn,
            Token = conversation.Token,
        };

        return Result<ConversationDto>.Success(dto);
    }
}
