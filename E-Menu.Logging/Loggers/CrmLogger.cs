using E_Menu.Logging.Interfaces;
using Microsoft.Xrm.Sdk;
using Shared.Kernel.Constants;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Menu.Logging.Loggers
{
    public class CrmLogger : ICrmLogger
    {
        private readonly IOrganizationService _crmService;

        public CrmLogger(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public async Task LogInfoAsync(string message, object context = null)
        {
            await CreateLogAsync("Info", message, null, context);
        }

        public async Task LogWarningAsync(string message, object context = null)
        {
            await CreateLogAsync("Warning", message, null, context);
        }

        public async Task LogErrorAsync(string message, Exception exception, object context = null)
        {
            await CreateLogAsync("Error", message, exception, context);
        }

        private async Task CreateLogAsync(string level, string message, Exception ex, object context)
        {
            var logEntity = new Entity(EntityLogicalNames.Log)
            {
                [LogAttributes.Name] = message,
                [LogAttributes.Level] = level,
                [LogAttributes.Details] = ex?.ToString(),
                [LogAttributes.ContextJson] = JsonSerializer.Serialize(context)
            };


            await Task.Run(() => _crmService.Create(logEntity));
        }
    }

}
