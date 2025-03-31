using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Dal;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class OutboxService : IOutboxService
{
    private readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    private readonly IOutboxRepository _outboxRepository;

    public OutboxService(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    public async Task Queue<T>(string topic, long key, T message, CancellationToken token)
    {
        var json = JsonSerializer.Serialize(message, JsonSerializerOptions);
        await _outboxRepository.InsertMessage(topic, key, json, token);
    }
}