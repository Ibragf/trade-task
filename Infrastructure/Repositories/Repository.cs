using System.Transactions;
using Npgsql;

namespace Infrastructure.Repositories;

public class Repository
{
    public TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromMinutes(1) },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    protected NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection("");
    }
}