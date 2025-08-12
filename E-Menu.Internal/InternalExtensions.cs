using Microsoft.Extensions.DependencyInjection;

namespace E_Menu.Internal;

public static class InternalExtensions
{
    public static IServiceCollection AddInternalServices(this IServiceCollection services)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(InternalExtensions).Assembly);
        });

        return services;
    }
}
