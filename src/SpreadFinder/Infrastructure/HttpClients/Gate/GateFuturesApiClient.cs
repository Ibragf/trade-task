using System.Net.Http.Json;
using Infrastructure.HttpClients.Gate.Dto;
using Infrastructure.HttpClients.Interfaces;

namespace Infrastructure.HttpClients.Gate;

public class GateFuturesApiClient : IGateFuturesApiClient
{
    private readonly HttpClient _client;

    public GateFuturesApiClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri("https://api.gateio.ws");
    }
    
    public async Task<GetCandlesticksData[]> GetDeliveryCandlesticks(string contract, DateTimeOffset from, DateTimeOffset to, CancellationToken token)
    {
        var queryParams = $"contract={contract}&from={from.ToUnixTimeSeconds()}&to={to.ToUnixTimeSeconds()}"; 
        var response = await _client.GetFromJsonAsync<GetCandlesticksData[]>($"api/v4/delivery/usdt/candlesticks?{queryParams}", token);
        return response;
    }
}