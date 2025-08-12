using MediatR;
using Shared.Kernel;
using Shared.Kernel.Dtos;

namespace E_Menu.Internal.Queries.ListBotMessages;

public record ListBotMessagesQuery(string ConversationId, string Watermark = null) : IRequest<Result<IEnumerable<BotMessageDto>>>;


