using System.Data;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.NameTranslation;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Infrastructure.Repositories;

public class Repository
{
    private readonly NpgsqlDataSource _source;

    public Repository(NpgsqlDataSource source)
    {
        _source = source;
    }
    
    public TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromMinutes(1) },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    protected NpgsqlConnection GetConnection()
    {
        return _source.CreateConnection();
    }
}