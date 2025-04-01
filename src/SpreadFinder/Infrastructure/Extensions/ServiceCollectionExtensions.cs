using Application.Interfaces;
using Application.Interfaces.Dal;
using Application.Services;
using Confluent.Kafka;
using Infrastructure.HttpClients.Gate;
using Infrastructure.HttpClients.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Jobs;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.NameTranslation;
using Polly;
using Quartz;

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

    public static IServiceCollection AddDal(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        var builder = new NpgsqlDataSourceBuilder(connectionString);
        builder.DefaultNameTranslator = new NpgsqlSnakeCaseNameTranslator();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        services
            .AddSingleton(_ => builder.Build())
            .AddScoped<IFuturePricesRepository, FuturePricesRepository>()
            .AddScoped<IOutboxRepository, OutboxRepository>();
        
        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services
            .AddScoped<GateSpreadService>()
            .AddScoped<ISpreadCalculator, SpreadCalculator>()
            .AddScoped<IOutboxService, OutboxService>();

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxBatchProcessor>();

        return services;
    }

    public static IServiceCollection AddQuartzJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            var settings = configuration.GetSection(nameof(SpreadCalculationJobSettings)).Get<SpreadCalculationJobSettings>();
            var jobKey = new JobKey(nameof(SpreadCalculationJob<GateSpreadService>));
            
            q.AddJob<SpreadCalculationJob<GateSpreadService>>(opts => opts.WithIdentity(jobKey));
            
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(SpreadCalculationJob<GateSpreadService>)}-trigger")
                .WithCronSchedule(settings.JobCron));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
    
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
        
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            ClientId = kafkaSettings.ClientId,
            AllowAutoCreateTopics = true,
            Acks = Acks.All,
            EnableIdempotence = false
        };

        services.AddSingleton(_ => new ProducerBuilder<string, string>(kafkaConfig).Build());

        return services;
    }
}