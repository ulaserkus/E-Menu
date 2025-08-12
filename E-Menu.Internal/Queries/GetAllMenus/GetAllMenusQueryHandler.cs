using E_Menu.Logging.Interfaces;
using MediatR;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel;
using Shared.Kernel.Constants;
using Shared.Kernel.Dtos;

namespace E_Menu.Internal.Queries.GetAllMenus;

internal class GetAllMenusQueryHandler(IOrganizationServiceAsync service, ICrmLogger crmLogger) : IRequestHandler<GetAllMenusInternalRequest, Result<IEnumerable<MenuDto>>>
{
    public async Task<Result<IEnumerable<MenuDto>>> Handle(GetAllMenusInternalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var query = new QueryExpression(EntityLogicalNames.Menu)
            {
                ColumnSet = new ColumnSet(MenuAttributes.Name, MenuAttributes.Code, MenuAttributes.TransactionCurrencyId)
            };

            var crmResult = await service.RetrieveMultipleAsync(query);

            if (crmResult.Entities.Count == 0)
            {
                await crmLogger.LogWarningAsync("No menus found", new { request = request });
                return Result<IEnumerable<MenuDto>>.Failure("No menus found.");

            }
            var menus = crmResult.Entities.Select(entity => new MenuDto
            {
                Code = entity.GetAttributeValue<string>(MenuAttributes.Code),
                Name = entity.GetAttributeValue<string>(MenuAttributes.Name),
                Id = entity.Id,
                CurrencyId = entity.GetAttributeValue<EntityReference>(MenuAttributes.TransactionCurrencyId)?.Id,
                CurrencyName = entity.GetAttributeValue<EntityReference>(MenuAttributes.TransactionCurrencyId)?.Name
            });

            return Result<IEnumerable<MenuDto>>.Success(menus);
        }
        catch (Exception ex)
        {
            await crmLogger.LogErrorAsync(nameof(GetAllMenusQueryHandler), ex, new { request = request });
            return Result<IEnumerable<MenuDto>>.Failure("An error occurred while retrieving menus.");
        }
    }
}
