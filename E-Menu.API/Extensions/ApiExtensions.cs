using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace E_Menu.API.Extensions;

public static class ApiExtensions
{

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<IOrganizationServiceAsync>(opt =>
        {
            var connectionString = configuration.GetConnectionString("MyCDSServer");
            var service = new ServiceClient(connectionString);
            return service;
        });

        services.AddTransient<IOrganizationService>(sp =>
               sp.GetRequiredService<IOrganizationServiceAsync>() as IOrganizationService);

        return services;
    }
}
