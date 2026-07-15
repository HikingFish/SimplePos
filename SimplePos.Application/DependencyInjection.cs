using Microsoft.Extensions.DependencyInjection;
using SimplePos.Application.Abstractions.Messaging;

namespace SimplePos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICqrsDispatcher, CqrsDispatcher>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        return services;
    }
}