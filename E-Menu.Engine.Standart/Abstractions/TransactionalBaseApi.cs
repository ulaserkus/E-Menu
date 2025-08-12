using E_Menu.Engine.Constants;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Shared.Kernel;
using Shared.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace E_Menu.Engine.Abstractions
{
    public abstract class TransactionalBaseApi<TRequest, TResponse>
        : ICustomApiCommand<TRequest, TResponse>
        where TResponse : class
        where TRequest : ICustomApiRequest
    {
        private const int MaxTransactionCount = 250;
        private TRequest _request;
        private IOrganizationService _organizationService;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected TRequest Request => _request;
        protected IOrganizationService Service => _organizationService;

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
                _request = GetRequest(serviceProvider);
                var requests = PrepareRequests();
                if (requests == null || requests.Count == 0)
                {
                    SetOutput(context, Result<TResponse>.Failure("No requests to execute in transaction.", 400));
                    return;
                }

                if (requests.Count > MaxTransactionCount)
                {
                    SetOutput(context, Result<TResponse>.Failure($"Maximum transaction request count is {MaxTransactionCount}.", 400));
                    return;
                }

                var service = GetOrganizationService(serviceProvider);
                _organizationService = service;

                var transactionRequest = new ExecuteTransactionRequest
                {
                    Requests = new OrganizationRequestCollection(),
                    ReturnResponses = true
                };

                foreach (var req in requests)
                {
                    transactionRequest.Requests.Add(req);
                }

                var transactionResponse = (ExecuteTransactionResponse)service.Execute(transactionRequest);

                var mappedResult = Map(transactionResponse);
                SetOutput(context, Result<TResponse>.Success(mappedResult, 200));
            }
            catch (Exception ex)
            {
                SetOutput(context, Result<TResponse>.Failure(ex.Message, 500));
            }
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

        protected abstract List<OrganizationRequest> PrepareRequests();
        protected abstract TResponse Map(ExecuteTransactionResponse response);
        private void SetOutput(IPluginExecutionContext context, Result<TResponse> result)
        {
            context.OutputParameters[CustomApiConstants.Output] = JsonSerializer.Serialize(result, _jsonSerializerOptions);
        }
    }
}
