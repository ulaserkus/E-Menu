using E_Menu.Engine.Abstractions;
using E_Menu.Engine.Attributes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel.Constants;
using Shared.Kernel.Dtos;
using Shared.Kernel.Requests;
using System.Collections.Generic;

namespace E_Menu.Engine.Custom_APIs.Menus.GetAllMenus
{
    [CustomApi(
        description: "Get all menus",
        targetType: typeof(GetAllMenusQuery)
     )]
    public class GetAllMenusQuery :
        RetrieveBaseApi<GetAllMenusRequest, IEnumerable<MenuDto>>,
        IPlugin
    {
        public GetAllMenusQuery() { }
        protected override QueryExpression CreateQueryExpression()
        {
            var query = new QueryExpression(EntityLogicalNames.Menu)
            {
                ColumnSet = new ColumnSet(MenuAttributes.Name, MenuAttributes.Code, MenuAttributes.TransactionCurrencyId, MenuAttributes.Description),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(MenuAttributes.StateCode, ConditionOperator.Equal, CrmCustomEntityStateCodes.Active)
                    }
                }
            };

            return query;
        }

        protected override IEnumerable<MenuDto> Map()
        {
            foreach (var entity in Collection.Entities)
            {
                yield return new MenuDto
                {
                    Code = entity.GetAttributeValue<string>(MenuAttributes.Code),
                    Name = entity.GetAttributeValue<string>(MenuAttributes.Name),
                    Id = entity.Id,
                    CurrencyId = entity.GetAttributeValue<EntityReference>(MenuAttributes.TransactionCurrencyId)?.Id,
                    CurrencyName = entity.GetAttributeValue<EntityReference>(MenuAttributes.TransactionCurrencyId)?.Name,
                    Description = entity.GetAttributeValue<string>(MenuAttributes.Description),
                };
            }
        }
    }
}