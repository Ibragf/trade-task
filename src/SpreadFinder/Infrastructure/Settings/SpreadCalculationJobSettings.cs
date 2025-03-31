namespace Infrastructure.Settings;

public class SpreadCalculationJobSettings
{
    public SpreadCalculationPair[] CalculationPairs { get; set; }
}

public class SpreadCalculationPair
{
    public string OneContract { get; set; }
    
    public string TwoContract { get; set; }
}