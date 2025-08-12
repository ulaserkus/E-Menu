using E_Menu.Engine.Constants;
using E_Menu.Logging.Interfaces;
using E_Menu.Logging.Loggers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel;
using Shared.Kernel.Interfaces;
using System;
using System.Text.Json;

namespace E_Menu.Engine.Abstractions
{
    public abstract class RetrieveBaseApi<TRequest, TResponse>
        : ICustomApiCommand<TRequest, TResponse>
        where TResponse : class
        where TRequest : ICustomApiRequest
    {
        protected ICrmLogger _logger;
        private EntityCollection _collection;
        private QueryExpression _queryExpression;
        private IOrganizationService _organizationService;
        private TRequest _request;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected IOrganizationService Service => _organizationService;
        protected EntityCollection Collection => _collection;
        protected QueryExpression QueryExpression => _queryExpression;
        protected TRequest Request => _request;

        private IPluginExecutionContext GetContext(IServiceProvider serviceProvider)
            => (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        private IOrganizationService GetOrganizationService(IServiceProvider serviceProvider)
        {
            var context = GetContext(serviceProvider);
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            return serviceFactory.CreateOrganizationService(context.UserId);
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = GetContext(serviceProvider);
            try
            {
                var service = GetOrganizationService(serviceProvider);
                _organizationService = service;
                _logger = new CrmLogger(service);
                _request = GetRequest(serviceProvider);
                var query = CreateQueryExpression();

                // Log the request
                _logger.LogInfoAsync($"{GetClassName()} - Request received", new { request = Request }).Wait();

                if (query != null)
                {
                    _queryExpression = query;
                    var results = service.RetrieveMultiple(query);
                    _collection = results;
                    var mappedResults = Map();
                    SetOutput(context, Result<TResponse>.Success(mappedResults, 200));
                }
                else
                {
                    SetOutput(context, Result<TResponse>.Failure("QueryExpression is null or invalid.", 400));
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorAsync($"{GetClassName()}", ex, new { request = Request }).Wait();
                SetOutput(context, Result<TResponse>.Failure(ex.Message, 500));
            }
        }
        private string GetClassName()
        {
            var type = GetType();
            return type.Name;
        }
        private TRequest GetRequest(IServiceProvider serviceProvider)
        {
            var context = GetContext(serviceProvider);
            if (context.InputParameters.TryGetValue(CustomApiConstants.Input, out var input))
            {
                return JsonSerializer.Deserialize<TRequest>(input.ToString(), _jsonSerializerOptions);
            }
            return default;
        }
        protected abstract TResponse Map();
        protected abstract QueryExpression CreateQueryExpression();
        private void SetOutput(IPluginExecutionContext context, Result<TResponse> result)
        {
            context.OutputParameters[CustomApiConstants.Output] = JsonSerializer.Serialize(result, _jsonSerializerOptions);
        }
    }
}