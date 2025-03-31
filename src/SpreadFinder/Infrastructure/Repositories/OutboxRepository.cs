using Application.Interfaces.Dal;
using Application.Objects;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Repositories;

public class OutboxRepository : Repository, IOutboxRepository
{
    public OutboxRepository(NpgsqlDataSource source) : base(source)
    {
    }
    
    public async Task InsertMessage(string topic, long key, string payload, CancellationToken token)
    {
        var sql = @"insert into outbox (topic, key, payload, ts, processed)
                    values (@Topic, @Key, @Payload, @Ts, false)";

        var cmd = new CommandDefinition(sql, new
        {
            Topic = topic,
            Key = key,
            Payload = payload,
            Ts = DateTimeOffset.UtcNow
        }, cancellationToken: token);

        using var connection = GetConnection();

        await connection.ExecuteAsync(cmd);
    }

    public async Task<OutboxMessage[]> GetUnprocessedMessages(CancellationToken token)
    {
        var sql = @"select id, topic, key, payload from outbox
                    where not processed
                    order by id desc
                    limit 500";

        var cmd = new CommandDefinition(sql, cancellationToken: token);

        using var connection = GetConnection();

        var result = await connection.QueryAsync<OutboxMessage>(cmd);

        return result.ToArray();
    }

    public async Task MarkProcessed(long[] ids, CancellationToken token)
    {
        var sql = @"update outbox
                    set processed = true
                    where id = any(@Ids)";

        var cmd = new CommandDefinition(sql, new
        {
            Ids = ids
        }, cancellationToken: token);

        using var connection = GetConnection();

        await connection.ExecuteAsync(cmd);
    }
}