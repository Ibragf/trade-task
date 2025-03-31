using Application.Interfaces;
using Application.Interfaces.Dal;
using Domain.Events;
using Infrastructure.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class GateSpreadService : ISpreadService
{
    private readonly IGateFuturePricesProvider _futurePricesProvider;
    private readonly IFuturePricesRepository _futurePricesRepository;
    private readonly ISpreadCalculator _spreadCalculator;
    private readonly IOutboxService _outboxService;
    private readonly KafkaTopics _kafkaTopics;

    public GateSpreadService(
        IGateFuturePricesProvider futurePricesProvider,
        IFuturePricesRepository futurePricesRepository, 
        ISpreadCalculator spreadCalculator, 
        IOutboxService outboxService,
        IOptions<KafkaTopics> kafkaOptions)
    {
        _futurePricesProvider = futurePricesProvider;
        _futurePricesRepository = futurePricesRepository;
        _spreadCalculator = spreadCalculator;
        _outboxService = outboxService;
        _kafkaTopics = kafkaOptions.Value;
    }

    public async Task CalculateSpread(string oneContract, string twoContract, CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var to = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.Zero);
        var from = to.AddHours(-1);

        var onePrice = await _futurePricesProvider.GetFuturePrice(oneContract, from, to, token);
        var twoPrice = await _futurePricesProvider.GetFuturePrice(twoContract, from, to, token);

        var spread = _spreadCalculator.CalculateSpread(onePrice, twoPrice);
        var spreadChangedEvent = new SpreadChangedEvent
        {
            ExchangeName = onePrice.ExchangeName,
            Spread = spread.Spread,
            UpdatedAt = DateTimeOffset.UtcNow,
            FirstContract = spread.FirstContract,
            SecondContract = spread.SecondContract
        };

        using var ts = _futurePricesRepository.CreateTransactionScope();
        
        await _futurePricesRepository.InsertPrice(onePrice, token);
        await _futurePricesRepository.InsertPrice(twoPrice, token);
        await _outboxService.Queue(_kafkaTopics.SpreadChanged, Random.Shared.NextInt64(), spreadChangedEvent, token);
        
        ts.Complete();
    }
}