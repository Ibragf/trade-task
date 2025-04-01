namespace Infrastructure.Settings;

public class SpreadCalculationJobSettings
{
    public string JobCron { get; set; }
    public SpreadCalculationPair[] CalculationPairs { get; set; }
}

public class SpreadCalculationPair
{
    public string OneContract { get; set; }
    
    public string TwoContract { get; set; }
}