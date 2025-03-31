using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SpreadCalculationJob<T> : IJob where T : ISpreadService
{
    private readonly T _spreadService;
    private readonly ILogger<SpreadCalculationJob<T>> _logger;
    private readonly SpreadCalculationJobSettings _settings;

    public SpreadCalculationJob(
        T spreadService, 
        ILogger<SpreadCalculationJob<T>> logger, 
        IOptionsSnapshot<SpreadCalculationJobSettings> settings)
    {
        _spreadService = spreadService;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("SpreadCalculationJob started");

        foreach (var pair in _settings.CalculationPairs)
        {
            await Process(pair.OneContract, pair.TwoContract, context.CancellationToken);
        }
    }

    private async Task Process(string one, string two, CancellationToken token)
    {
        try
        {
            await _spreadService.CalculateSpread(one, two, token);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured while calculating spread for {one} and {two}: {e.Message}", e.StackTrace);
        }
    }
}