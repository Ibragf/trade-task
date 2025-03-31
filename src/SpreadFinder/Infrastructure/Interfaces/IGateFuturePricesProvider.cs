using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IGateFuturePricesProvider
{
    Task<FuturePrice> GetFuturePrice(string contract, DateTimeOffset from, DateTimeOffset to, CancellationToken token);
}