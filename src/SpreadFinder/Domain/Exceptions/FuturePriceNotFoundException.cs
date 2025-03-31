namespace Domain.Exceptions;

public class FuturePriceNotFoundException : Exception
{
    public FuturePriceNotFoundException(string message) : base(message) 
    {
        
    }
}