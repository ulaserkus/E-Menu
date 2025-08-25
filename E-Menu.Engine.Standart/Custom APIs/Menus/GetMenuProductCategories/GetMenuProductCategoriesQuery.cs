using E_Menu.Engine.Abstractions;
using E_Menu.Engine.Attributes;
using FluentDynamics.QueryBuilder;
using FluentDynamics.QueryBuilder.Extensions;
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
            var fluentQuery = Query.For(EntityLogicalNames.ProductCategory)
                .Select(ProductCategoryAttributes.Id, ProductCategoryAttributes.Name)
                .Where(f =>
                {
                    f.Equal(ProductCategoryAttributes.StateCode, CrmCustomEntityStateCodes.Active);
                })
                .InnerJoin(EntityLogicalNames.MenuProduct, ProductCategoryAttributes.Id, MenuProductAttributes.ProductCategoryId, join =>
                {
                    join.InnerJoin(EntityLogicalNames.MenuItem, MenuProductAttributes.Id, MenuItemAttributes.MenuProductId, join2 =>
                    {
                        join2.Where(f2 =>
                        {
                            f2.Equal(MenuItemAttributes.MenuId, Request.MenuId);
                        }).As(EntityAliases.MenuItem);

                    }).As(EntityAliases.MenuProduct);
                })
                .NoLock();

            return fluentQuery.ToQueryExpression();
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
