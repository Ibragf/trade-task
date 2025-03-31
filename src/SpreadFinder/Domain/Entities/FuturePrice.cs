namespace Domain.Entities;

public class FuturePrice
{
    public string ExchangeName { get; set; }
    
    public decimal Price { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public string Contract { get; set; }
}