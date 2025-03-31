namespace Application.Objects;

public class OutboxMessage
{
    public long Id { get; set; }
    
    public string Topic { get; set; }
    
    public string Payload { get; set; }
    
    public long Key { get; set; }
}