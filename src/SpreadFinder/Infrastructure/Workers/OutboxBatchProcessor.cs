using System.Diagnostics;
using Application.Interfaces.Dal;
using Application.Objects;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Workers;

public class OutboxBatchProcessor : BackgroundService
{
    private readonly IProducer<string, string> _producer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxBatchProcessor> _logger;

    public OutboxBatchProcessor(
        IServiceProvider serviceProvider,
        IProducer<string, string> producer, 
        ILogger<OutboxBatchProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _producer = producer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (true)
            {
                stoppingToken.ThrowIfCancellationRequested();
                await Process(stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Outbox processor stopped: {ex.Message}", ex.StackTrace);
        }
    }

    private async Task Process(CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        using var scope = _serviceProvider.CreateScope();

        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var messages = await outboxRepository.GetUnprocessedMessages(token);

        foreach (var message in messages)
        {
            await SendMessage(message, token);
        }

        var ids = messages.Select(x => x.Id).ToArray();
        await outboxRepository.MarkProcessed(ids, token);
        
        _logger.LogInformation($"Outbox processed {messages.Length} messages in {sw.ElapsedMilliseconds}");
    }

    private async Task SendBatchMessages()
    {
        // todo
    }

    private async Task SendMessage(OutboxMessage message, CancellationToken token)
    {
        await _producer.ProduceAsync(message.Topic, new Message<string, string>
        {
            Key = message.Key.ToString(),
            Value = message.Payload
        }, token);
    }
}