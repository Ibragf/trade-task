using Application.Interfaces.Dal;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Repositories;

public class FuturePricesRepository : Repository, IFuturePricesRepository
{
    public FuturePricesRepository(NpgsqlDataSource source) : base(source)
    {
    }
    
    public async Task<FuturePrice?> GetLastAvailablePrice(string contract, string exchangeName, DateTimeOffset from, CancellationToken token)
    {
        var sql = @"select * from future_prices
                    where updated_at > @From and exchange_name=@ExchangeName and contract=@Contract
                    order by updated_at desc
                    limit 1";

        var cmd = new CommandDefinition(sql, new
        {
            ExchangeName = exchangeName.ToUpper(),
            Contract = contract.ToUpper(),
            From = from
        }, cancellationToken: token);
        
        using var connection = GetConnection();

        var result = await connection.QueryFirstOrDefaultAsync<FuturePrice>(cmd);

        return result;
    }

    public async Task InsertPrice(FuturePrice price, CancellationToken token)
    {
        var sql = @"insert into future_prices (contract, exchange_name, price, updated_at)
                    values (@Contract, @ExchangeName, @Price, @UpdatedAt)";

        var cmd = new CommandDefinition(sql, new
        {
            Contract = price.Contract.ToUpper(),
            ExchangeName = price.ExchangeName.ToUpper(),
            Price = price.Price,
            UpdatedAt = price.UpdatedAt
        });

        using var connection = GetConnection();

        await connection.ExecuteAsync(cmd);
    }
}