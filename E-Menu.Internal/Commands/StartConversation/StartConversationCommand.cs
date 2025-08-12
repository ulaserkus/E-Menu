using MediatR;
using Shared.Kernel;
using Shared.Kernel.Dtos;

namespace E_Menu.Internal.Commands.StartConversation;

public record StartConversationCommand() : IRequest<Result<ConversationDto>>;

