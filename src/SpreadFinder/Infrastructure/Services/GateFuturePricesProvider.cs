using Application.Interfaces.Dal;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.HttpClients.Interfaces;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class GateFuturePricesProvider : IGateFuturePricesProvider
{
    private readonly IFuturePricesRepository _futurePricesRepository;
    private readonly IGateFuturesApiClient _futuresApiClient;

    private const string ExchangeName = "gate";

    public GateFuturePricesProvider(
        IFuturePricesRepository futurePricesRepository, 
        IGateFuturesApiClient futuresApiClient)
    {
        _futurePricesRepository = futurePricesRepository;
        _futuresApiClient = futuresApiClient;
    }

    public async Task<FuturePrice> GetFuturePrice(string contract, DateTimeOffset from, DateTimeOffset to, CancellationToken token)
    {
        var prices = await _futuresApiClient.GetDeliveryCandlesticks(contract, from, to, token);
        if (prices.Length > 0)
        {
            
            var lastChange = prices.MaxBy(x => x.Timestamp);
            
            return new FuturePrice
            {
                ExchangeName = ExchangeName,
                Price = decimal.Parse(lastChange.ClosePrice),
                UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(lastChange.Timestamp),
                Contract = contract
            };
        }

        var lastPrice = await _futurePricesRepository.GetLastAvailablePrice(contract, ExchangeName, DateTimeOffset.UtcNow.AddDays(-1), token);
        if (lastPrice is null)
        {
            throw new FuturePriceNotFoundException($"Price not found for {contract}");
        }

        return lastPrice;
    }
}