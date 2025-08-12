using E_Menu.Engine.Abstractions;
using E_Menu.Engine.Attributes;
using E_Menu.Engine.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel.Constants;
using Shared.Kernel.Dtos;
using Shared.Kernel.Requests;
using System.Collections.Generic;
using System.Linq;


namespace E_Menu.Engine.Custom_APIs.Menus.GetMenuWithItems
{

    [
        CustomApi(
            description: "Get menu with items",
            targetType: typeof(GetMenuWithItemsQuery)
    )]
    public class GetMenuWithItemsQuery : RetrieveBaseApi<GetMenuWithItemsRequest, MenuWithItemsDto>, IPlugin
    {
        protected override QueryExpression CreateQueryExpression()
        {
            var query = new QueryExpression(EntityLogicalNames.Menu)
            {
                ColumnSet = CreateMenuColumnSet(),
                NoLock = true,
                Distinct = true,
                Criteria = CreateMenuEntityFilter(),
                LinkEntities =
                {
                    CreateMenuItemLink()
                }
            };

            return query;
        }

        private ColumnSet CreateMenuColumnSet()
        {
            return new ColumnSet(
                MenuAttributes.Id,
                MenuAttributes.Name,
                MenuAttributes.TransactionCurrencyId,
                MenuAttributes.Code,
                MenuAttributes.Description
            );
        }

        private FilterExpression CreateMenuEntityFilter()
        {
            return new FilterExpression
            {
                Conditions =
        {
            new ConditionExpression(MenuAttributes.StateCode, ConditionOperator.Equal, CrmCustomEntityStateCodes.Active),
            new ConditionExpression(MenuAttributes.Id,ConditionOperator.Equal,Request.MenuId)
        }
            };
        }

        private FilterExpression CreateActiveEntityFilter(string stateCodeField)
        {
            return new FilterExpression
            {
                Conditions =
        {
            new ConditionExpression(stateCodeField, ConditionOperator.Equal, CrmCustomEntityStateCodes.Active)
        }
            };
        }

        private LinkEntity CreateMenuItemLink()
        {
            var menuItemLink = new LinkEntity
            {
                LinkFromEntityName = EntityLogicalNames.Menu,
                LinkFromAttributeName = MenuAttributes.Id,
                LinkToEntityName = EntityLogicalNames.MenuItem,
                LinkToAttributeName = MenuItemAttributes.MenuId,
                Columns = CreateMenuItemColumnSet(),
                EntityAlias = EntityAliases.MenuItem,
                LinkCriteria = CreateActiveEntityFilter(MenuItemAttributes.StateCode),
                LinkEntities =
                            {
                                CreateMenuProductLink()
                            }
            };

            return menuItemLink;
        }
        private ColumnSet CreateMenuItemColumnSet()
        {
            return new ColumnSet(
                MenuItemAttributes.Id,
                MenuItemAttributes.Price,
                MenuItemAttributes.TransactionCurrencyId,
                MenuItemAttributes.Description,
                MenuItemAttributes.ItemCode
            );
        }

        private LinkEntity CreateMenuProductLink()
        {
            return new LinkEntity
            {
                LinkFromEntityName = EntityLogicalNames.MenuItem,
                LinkFromAttributeName = MenuItemAttributes.MenuProductId,
                LinkToEntityName = EntityLogicalNames.MenuProduct,
                LinkToAttributeName = MenuProductAttributes.Id,
                Columns = CreateMenuProductColumnSet(),
                EntityAlias = EntityAliases.MenuProduct,
                LinkCriteria = CreateActiveEntityFilter(ProductCategoryAttributes.StateCode)
            };
        }

        private ColumnSet CreateMenuProductColumnSet()
        {
            return new ColumnSet(
                MenuProductAttributes.Name,
                MenuProductAttributes.InStock,
                MenuProductAttributes.ImageUrl,
                MenuProductAttributes.TransactionCurrencyId,
                MenuProductAttributes.ProductCategoryId,
                MenuProductAttributes.ProductCode,
                MenuProductAttributes.Id,
                MenuProductAttributes.Price
            );
        }

        protected override MenuWithItemsDto Map()
        {
            var menuRecord = Collection.Entities.FirstOrDefault();
            if (menuRecord == null)
                return null; // ya da uygun bir hata yönetimi

            var menuDto = new MenuWithItemsDto
            {
                Id = menuRecord.Id,
                Name = menuRecord.GetAttributeValue<string>(MenuAttributes.Name),
                Code = menuRecord.GetAttributeValue<string>(MenuAttributes.Code),
                CurrencyId = menuRecord.GetAttributeValue<EntityReference>(MenuAttributes.TransactionCurrencyId)?.Id,
                CurrencyName = menuRecord.GetAttributeValue<EntityReference>(MenuAttributes.TransactionCurrencyId)?.Name,
                Description = menuRecord.GetAttributeValue<string>(MenuAttributes.Description),
                Items = Collection.Entities.Select(item =>
                {
                    var menuItem = EngineHelper.GetAliasedEntity(item, EntityAliases.MenuItem, EntityLogicalNames.MenuItem, MenuItemAttributes.Id);
                    var menuProduct = EngineHelper.GetAliasedEntity(item, EntityAliases.MenuProduct, EntityLogicalNames.MenuProduct, MenuProductAttributes.Id);

                    return new MenuItemDto
                    {
                        Id = menuItem.Id,
                        Price = menuItem.GetAttributeValue<Money>(MenuItemAttributes.Price)?.Value ?? 0,
                        Description = menuItem.GetAttributeValue<string>(MenuItemAttributes.Description),
                        CurrencyId = menuItem.GetAttributeValue<EntityReference>(MenuItemAttributes.TransactionCurrencyId)?.Id,
                        CurrencyName = menuItem.GetAttributeValue<EntityReference>(MenuItemAttributes.TransactionCurrencyId)?.Name,
                        ItemCode = menuItem.GetAttributeValue<string>(MenuItemAttributes.ItemCode),
                        FormattedPrice = menuItem.FormattedValues.Contains(MenuItemAttributes.Price)
                            ? menuItem.FormattedValues[MenuItemAttributes.Price]
                            : null,
                        Product = menuProduct == null ? null : new MenuProductDto
                        {
                            Id = menuProduct.Id,
                            Name = menuProduct.GetAttributeValue<string>(MenuProductAttributes.Name),
                            ImageUrl = menuProduct.GetAttributeValue<string>(MenuProductAttributes.ImageUrl),
                            InStock = menuProduct.GetAttributeValue<bool>(MenuProductAttributes.InStock),
                            ProductCategory = new ProductCategoryDto
                            {
                                Id = menuProduct.GetAttributeValue<EntityReference>(MenuProductAttributes.ProductCategoryId)?.Id,
                                Name = menuProduct.GetAttributeValue<EntityReference>(MenuProductAttributes.ProductCategoryId)?.Name
                            }
                        }
                    };
                }).ToList()
            };

            return menuDto;
        }
    }
}
