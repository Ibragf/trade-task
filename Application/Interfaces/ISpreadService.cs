using Domain.Entities;

namespace Application.Interfaces;

public interface ISpreadService
{
    Task CalculateSpread(string oneContract, string twoContract, CancellationToken token);
}