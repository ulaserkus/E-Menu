using E_Menu.Engine.Attributes;
using E_Menu.Engine.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel.Constants;
using System;
using System.Linq;
using System.Reflection;

namespace E_Menu.Engine
{
    public static class EngineExtensions
    {
        public static IServiceCollection AddEngineServices(this IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var service = serviceProvider.GetRequiredService<IOrganizationServiceAsync>();
                RegisterCustomApis(service);
            }


            return services;
        }


        private static void RegisterCustomApis(IOrganizationService service)
        {
            var typesWithAttr = typeof(EngineExtensions).Assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<CustomApiAttribute>() != null);

            var requests = new OrganizationRequestCollection();

            foreach (var type in typesWithAttr)
            {
                var attr = type.GetCustomAttribute<CustomApiAttribute>();
                var pluginTypeId = GetPluginTypeIdByNamespace(service, attr.NamespaceName);
                if (pluginTypeId == Guid.Empty)
                {
                    continue;
                }

                string uniqueName = $"{CustomApiConstants.Prefix}{attr.Name}";
                var customApiId = GetCustomApiIdByUniqueName(service, uniqueName);

                if (customApiId == Guid.Empty)
                {

                    var customApiRecord = CreateCustomApiEntity(attr, pluginTypeId, uniqueName);
                    var createRequest = new CreateRequest
                    {
                        Target = customApiRecord
                    };
                    createRequest["SolutionUniqueName"] = attr.SolutionName;
                    requests.Add(createRequest);

                    if (attr.ContainsInput)
                    {
                        var requestParam = CreateCustomApiRequestParameterEntity(customApiRecord.Id, uniqueName);
                        var createRequestParam = new CreateRequest
                        {
                            Target = requestParam
                        };
                        createRequestParam["SolutionUniqueName"] = attr.SolutionName;
                        requests.Add(createRequestParam);
                    }

                    var responseProp = CreateCustomApiResponsePropertyEntity(customApiRecord.Id, uniqueName);
                    var createResponseProp = new CreateRequest
                    {
                        Target = responseProp
                    };
                    createResponseProp["SolutionUniqueName"] = attr.SolutionName;
                    requests.Add(createResponseProp);

                }
            }

            if (requests.Count > 0)
            {
                var executeMultipleRequest = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = false,
                        ReturnResponses = false
                    },
                    Requests = requests
                };
                service.Execute(executeMultipleRequest);
            }
        }

        private static Entity CreateCustomApiEntity(CustomApiAttribute attr, Guid pluginTypeId, string uniqueName)
        {
            var entity = new Entity(EntityLogicalNames.CustomApi, Guid.NewGuid())
            {
                [CustomApiAttributes.UniqueName] = uniqueName,
                [CustomApiAttributes.Name] = uniqueName,
                [CustomApiAttributes.DisplayName] = attr.Name,
                [CustomApiAttributes.Description] = attr.Description,
                [CustomApiAttributes.BindingType] = new OptionSetValue(0),
                [CustomApiAttributes.AllowedCustomProcessingStepType] = new OptionSetValue(2),
                [CustomApiAttributes.IsFunction] = false,
                [CustomApiAttributes.IsPrivate] = false,
                [CustomApiAttributes.PluginTypeId] = new EntityReference(EntityLogicalNames.PluginType, pluginTypeId),
                [CustomApiAttributes.IsCustomizable] = new BooleanManagedProperty(true)
            };
            return entity;
        }

        private static Entity CreateCustomApiRequestParameterEntity(Guid customApiId, string uniqueName)
        {
            return new Entity(EntityLogicalNames.CustomApiRequestParameter)
            {
                [CustomApiRequestParameterAttributes.Name] = $"{uniqueName}.Input",
                [CustomApiRequestParameterAttributes.UniqueName] = CustomApiConstants.Input,
                [CustomApiRequestParameterAttributes.DisplayName] = CustomApiConstants.Input,
                [CustomApiRequestParameterAttributes.Description] = CustomApiConstants.InputDescription,
                [CustomApiRequestParameterAttributes.Type] = new OptionSetValue(10),
                [CustomApiRequestParameterAttributes.IsOptional] = false,
                [CustomApiRequestParameterAttributes.CustomApiId] = new EntityReference(EntityLogicalNames.CustomApi, customApiId),
                [CustomApiRequestParameterAttributes.IsCustomizable] = new BooleanManagedProperty(true)
            };
        }

        private static Entity CreateCustomApiResponsePropertyEntity(Guid customApiId, string uniqueName)
        {
            return new Entity(EntityLogicalNames.CustomApiResponseProperty)
            {
                [CustomApiResponsePropertyAttributes.Name] = $"{uniqueName}.Output",
                [CustomApiResponsePropertyAttributes.UniqueName] = CustomApiConstants.Output,
                [CustomApiResponsePropertyAttributes.DisplayName] = CustomApiConstants.OutputDisplay,
                [CustomApiResponsePropertyAttributes.Description] = CustomApiConstants.OutputDescription,
                [CustomApiResponsePropertyAttributes.Type] = new OptionSetValue(10),
                [CustomApiResponsePropertyAttributes.IsCustomizable] = new BooleanManagedProperty(true),
                [CustomApiResponsePropertyAttributes.CustomApiId] = new EntityReference(EntityLogicalNames.CustomApi, customApiId)
            };
        }

        private static Guid GetPluginTypeIdByNamespace(IOrganizationService service, string namespaceName)
        {
            var query = new QueryExpression(EntityLogicalNames.PluginType)
            {
                ColumnSet = new ColumnSet(PluginTypeAttributes.Id, PluginTypeAttributes.TypeName)
            };
            query.Criteria.AddCondition(PluginTypeAttributes.TypeName, ConditionOperator.Like, namespaceName + "%");
            var results = service.RetrieveMultiple(query);
            return results.Entities.FirstOrDefault()?.Id ?? Guid.Empty;
        }

        private static Guid GetCustomApiIdByUniqueName(IOrganizationService service, string uniqueName)
        {
            var query = new QueryExpression(EntityLogicalNames.CustomApi)
            {
                ColumnSet = new ColumnSet(CustomApiAttributes.Id, CustomApiAttributes.UniqueName)
            };
            query.Criteria.AddCondition(CustomApiAttributes.UniqueName, ConditionOperator.Equal, uniqueName);
            var results = service.RetrieveMultiple(query);
            return results.Entities.FirstOrDefault()?.Id ?? Guid.Empty;
        }
    }
}