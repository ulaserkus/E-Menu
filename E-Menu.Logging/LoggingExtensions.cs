using E_Menu.Logging.Interfaces;
using E_Menu.Logging.Loggers;
using Microsoft.Extensions.DependencyInjection;

namespace E_Menu.Logging
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            services.AddTransient<ICrmLogger, CrmLogger>();
            return services;
        }
    }
}
