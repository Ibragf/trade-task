using Infrastructure.HttpClients.Gate.Dto;

namespace Infrastructure.HttpClients.Interfaces;

public interface IGateFuturesApiClient
{
    Task<GetCandlesticksData[]> GetDeliveryCandlesticks(string contract, DateTimeOffset from, DateTimeOffset to, CancellationToken token);
}