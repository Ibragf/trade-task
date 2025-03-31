using Application.Interfaces.Dal;
using Dapper;

namespace Infrastructure.Repositories;

public class OutboxRepository : Repository, IOutboxRepository
{
    public async Task InsertMessage(string topic, long key, string body, CancellationToken token)
    {
        var sql = @"insert into outbox
                    values (@Topic, @Key, @Body, @Ts)";

        var cmd = new CommandDefinition(sql, new
        {
            Topic = topic,
            Key = key,
            Body = body,
            Ts = DateTimeOffset.UtcNow
        }, cancellationToken: token);

        using var connection = GetConnection();

        await connection.ExecuteAsync(cmd);
    }
}