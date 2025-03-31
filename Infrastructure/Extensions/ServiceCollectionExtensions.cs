using Application.Interfaces;
using Application.Interfaces.Dal;
using Application.Services;
using Infrastructure.HttpClients.Gate;
using Infrastructure.HttpClients.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddExchangeHttpClient<IGateFuturesApiClient, GateFuturesApiClient>();

        services.AddScoped<IGateFuturePricesProvider, GateFuturePricesProvider>();

        return services;
    }
    
    public static IServiceCollection AddExchangeHttpClient<TClient, TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient

    {
        services.AddHttpClient<TClient, TImplementation>()
            .AddTransientHttpErrorPolicy(policy =>
            {
                return policy.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromMicroseconds(200),
                    TimeSpan.FromMicroseconds(500),
                    TimeSpan.FromSeconds(1),
                });
            });

        return services;
    }

    public static IServiceCollection AddDal(this IServiceCollection services)
    {
        services
            .AddScoped<IFuturePricesRepository, FuturePricesRepository>()
            .AddScoped<IOutboxRepository, OutboxRepository>();
        
        return services;
    }

    public static IServiceCollection AddServices(IServiceCollection services)
    {
        services
            .AddScoped<GateSpreadService>()
            .AddScoped<ISpreadCalculator, SpreadCalculator>()
            .AddScoped<IOutboxService, OutboxService>();

        return services;
    }

    public static IServiceCollection AddJobs(IServiceCollection services)
    {
        
    }
}