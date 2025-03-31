namespace Application.Interfaces.Dal;

public interface IOutboxRepository : IRepository
{
    Task InsertMessage(string topic, long key, string body, CancellationToken token);
}