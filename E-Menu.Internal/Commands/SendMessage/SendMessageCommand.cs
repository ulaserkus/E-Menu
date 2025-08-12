using MediatR;
using Shared.Kernel;

namespace E_Menu.Internal.Commands.SendMessage;

public record SendMessageCommand(string ConversationId, string Message, string UserId) : IRequest<Result<bool>>;
