namespace Shared.Kernel.Constants
{
    public static class MenuAttributes
    {
        public const string Id = "uls_menuid";
        public const string Name = "uls_name";
        public const string Code = "uls_code";
        public const string TransactionCurrencyId = "uls_currencyid";
        public const string StateCode = "statecode";
        public const string Description = "uls_description";
    }

    public static class MenuItemAttributes
    {
        public const string Id = "uls_menuitemid";
        public const string MenuId = "uls_menuid";
        public const string MenuProductId = "uls_menuproductid";
        public const string ItemCode = "uls_itemcode";
        public const string TransactionCurrencyId = "transactioncurrencyid";
        public const string Price = "uls_price";
        public const string UpdatePriceAuto = "uls_updatepriceauto";
        public const string StateCode = "statecode";
        public const string Description = "uls_description";
    }

    public static class MenuProductAttributes
    {
        public const string Id = "uls_menuproductid";
        public const string ProductImage = "uls_productimage";
        public const string ProductCode = "uls_productcode";
        public const string Name = "uls_name";
        public const string Price = "uls_price";
        public const string TransactionCurrencyId = "transactioncurrencyid";
        public const string InStock = "uls_instock";
        public const string LatestExchangeRateHistoryId = "uls_latestexchangeratehistoryid";
        public const string StateCode = "statecode";
        public const string ImageUrl = "uls_imageurl";
        public const string ImageSyncDateUTC = "uls_imagesyncdate";
        public const string ProductCategoryId = "uls_productcategoryid";
    }

    public static class ProductCategoryAttributes
    {
        public const string Id = "uls_productcategoryid";
        public const string Name = "uls_name";
        public const string DisplayOrder = "uls_displayorder";
        public const string StateCode = "statecode";
    }

    public static class ExchangeRateHistoryAttributes
    {
        public const string Id = "uls_exchangeratehistoryid";
        public const string CurrencyName = "uls_currencyname";
        public const string TransactionCurrencyId = "uls_currencyid";
        public const string BuyingRate = "uls_forexbuyingrate";
        public const string SellingRate = "uls_forexsellingrate";
        public const string CreatedOn = "createdon";
    }

    public static class LogAttributes
    {
        public const string Id = "uls_logid";
        public const string Name = "uls_name";
        public const string Level = "uls_level";
        public const string Details = "uls_details";
        public const string ContextJson = "uls_contextjson";
    }

    public static class CustomApiAttributes
    {
        public const string Id = "customapiid";
        public const string UniqueName = "uniquename";
        public const string Name = "name";
        public const string DisplayName = "displayname";
        public const string Description = "description";
        public const string BindingType = "bindingtype";
        public const string AllowedCustomProcessingStepType = "allowedcustomprocessingsteptype";
        public const string IsFunction = "isfunction";
        public const string IsPrivate = "isprivate";
        public const string PluginTypeId = "plugintypeid";
        public const string IsCustomizable = "iscustomizable";
    }

    public static class CustomApiRequestParameterAttributes
    {
        public const string Id = "customapirequestparameterid";
        public const string Name = "name";
        public const string UniqueName = "uniquename";
        public const string DisplayName = "displayname";
        public const string Description = "description";
        public const string Type = "type";
        public const string IsOptional = "isoptional";
        public const string CustomApiId = "customapiid";
        public const string IsCustomizable = "iscustomizable";
    }
    public static class CustomApiResponsePropertyAttributes
    {
        public const string Id = "customapiresponsepropertyid";
        public const string Name = "name";
        public const string UniqueName = "uniquename";
        public const string DisplayName = "displayname";
        public const string Description = "description";
        public const string Type = "type";
        public const string CustomApiId = "customapiid";
        public const string IsCustomizable = "iscustomizable";
    }

    public static class PluginTypeAttributes
    {
        public const string Id = "plugintypeid";
        public const string TypeName = "typename";
        public const string FriendlyName = "friendlyname";
        public const string AssemblyName = "assemblyname";
        public const string IsCustomizable = "iscustomizable";
    }
}
