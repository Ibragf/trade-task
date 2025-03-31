using Application.Objects;

namespace Application.Interfaces.Dal;

public interface IOutboxRepository : IRepository
{
    Task InsertMessage(string topic, long key, string payload, CancellationToken token);

    Task<OutboxMessage[]> GetUnprocessedMessages(CancellationToken token);

    Task MarkProcessed(long[] ids, CancellationToken token);
}