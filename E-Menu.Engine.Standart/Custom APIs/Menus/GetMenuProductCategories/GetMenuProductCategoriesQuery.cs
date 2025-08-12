using E_Menu.Engine.Abstractions;
using E_Menu.Engine.Attributes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel.Constants;
using Shared.Kernel.Dtos;
using Shared.Kernel.Requests;
using System.Collections.Generic;

namespace E_Menu.Engine.Custom_APIs.Menus.GetMenuProductCategories
{
    [CustomApi(
        description: "Get product categories for a specific menu",
        targetType: typeof(GetMenuProductCategoriesQuery)
    )]
    public class GetMenuProductCategoriesQuery : RetrieveBaseApi<GetMenuProductCategoriesRequest, IEnumerable<ProductCategoryDto>>,
      IPlugin
    {
        protected override QueryExpression CreateQueryExpression()
        {
            var query = new QueryExpression(EntityLogicalNames.ProductCategory)
            {
                NoLock = true,
                Distinct = true,
                ColumnSet = new ColumnSet(ProductCategoryAttributes.Id, ProductCategoryAttributes.Name),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(ProductCategoryAttributes.StateCode, ConditionOperator.Equal,CrmCustomEntityStateCodes.Active)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                       EntityLogicalNames.ProductCategory,
                       EntityLogicalNames.MenuProduct,
                       ProductCategoryAttributes.Id,
                       MenuProductAttributes.ProductCategoryId,
                        JoinOperator.Inner)
                    {
                        EntityAlias = EntityAliases.MenuProduct,
                        LinkEntities =
                        {
                            new LinkEntity(
                                EntityLogicalNames.MenuProduct,
                                EntityLogicalNames.MenuItem,
                                MenuProductAttributes.Id,
                                MenuItemAttributes.MenuProductId,
                                JoinOperator.Inner)
                            {
                                EntityAlias = EntityAliases.MenuItem,
                                LinkCriteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression(MenuItemAttributes.MenuId, ConditionOperator.Equal, Request.MenuId)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return query;
        }

        protected override IEnumerable<ProductCategoryDto> Map()
        {
            foreach (var entity in Collection.Entities)
            {
                yield return new ProductCategoryDto
                {
                    Id = entity.Id,
                    Name = entity.GetAttributeValue<string>(ProductCategoryAttributes.Name),
                };

            }
        }
    }
}
