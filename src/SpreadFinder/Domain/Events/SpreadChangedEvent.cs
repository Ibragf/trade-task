namespace Domain.Events;

public class SpreadChangedEvent
{
    public string ExchangeName { get; set; }
    
    public decimal Spread { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public string FirstContract { get; set; }
    
    public string SecondContract { get; set; }
}