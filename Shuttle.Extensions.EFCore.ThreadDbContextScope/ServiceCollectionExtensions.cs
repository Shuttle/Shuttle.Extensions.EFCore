using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;

namespace Shuttle.Extensions.EFCore.ThreadDbContextScope;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddThreadDbContextScope(this IServiceCollection services)
    {
        Guard.AgainstNull(services);

        if (!services.Any(s => s.ServiceType == typeof(IHostedService) && s.ImplementationType == typeof(ThreadDbContextScopeHostedService)))
        {
            services.AddHostedService<ThreadDbContextScopeHostedService>();
        }

        return services;
    }
}