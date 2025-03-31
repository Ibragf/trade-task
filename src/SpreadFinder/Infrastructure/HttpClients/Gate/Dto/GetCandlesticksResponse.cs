using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Gate.Dto;

public class GetCandlesticksData
{
    [JsonPropertyName("t")]
    public long Timestamp { get; set; }
    
    [JsonPropertyName("o")]
    public string OpenPrice { get; set; }
    
    [JsonPropertyName("c")]
    public string ClosePrice { get; set; } 
    
    [JsonPropertyName("v")]
    public decimal Volume { get; set; }
    
    [JsonPropertyName("h")]
    public string HighestPrice { get; set; }
    
    [JsonPropertyName("l")]
    public string LowestPrice { get; set; } 
}