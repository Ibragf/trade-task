namespace Infrastructure.Interfaces;

public interface IOutboxService
{
    Task Queue<T>(string topic, long key, T message, CancellationToken token);
}