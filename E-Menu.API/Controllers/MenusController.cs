using E_Menu.API.Abstraction;
using E_Menu.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Shared.Kernel.Requests;

namespace E_Menu.API.Controllers;


public class MenusController : ApiController
{
    public MenusController(IOrganizationServiceAsync service, IMediator mediator, ICacheService cacheService) : base(service, mediator, cacheService)
    {
    }

    [HttpPost]
    public async Task<IActionResult> All(GetAllMenusRequest request)
    {
        return await ExecuteCustomApiAsync(request, cache: true);
    }


    [HttpPost]
    public async Task<IActionResult> Categories(GetMenuProductCategoriesRequest request)
    {
        return await ExecuteCustomApiAsync(request, cache: true);
    }

    [HttpPost]
    public async Task<IActionResult> Items(GetMenuWithItemsRequest request)
    {
        return await ExecuteCustomApiAsync(request, cache: true);
    }
}
