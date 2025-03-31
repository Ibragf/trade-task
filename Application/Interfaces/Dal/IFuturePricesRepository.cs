using Domain.Entities;

namespace Application.Interfaces.Dal;

public interface IFuturePricesRepository : IRepository
{
    Task<FuturePrice?> GetLastAvailablePrice(string contract, string exchangeName, CancellationToken token);

    Task InsertPrice(FuturePrice price, CancellationToken token);
}