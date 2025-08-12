using E_Menu.API.Abstraction;
using E_Menu.Caching.Interfaces;
using E_Menu.Internal.Commands.SendMessage;
using E_Menu.Internal.Commands.StartConversation;
using E_Menu.Internal.Queries.ListBotMessages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace E_Menu.API.Controllers;


public class ConversationsController : ApiController
{
    public ConversationsController(IOrganizationServiceAsync service, IMediator mediator, ICacheService cacheService) : base(service, mediator, cacheService)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Start(StartConversationCommand request)
    {
        return await ExecuteInternalApiAsync(request, cache: false);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(SendMessageCommand request)
    {
        return await ExecuteInternalApiAsync(request, cache: false);
    }

    [HttpPost]
    public async Task<IActionResult> Messages(ListBotMessagesQuery request)
    {
        return await ExecuteInternalApiAsync(request, cache: false);
    }
}
