using Application.Objects;
using Domain.Entities;

namespace Application.Interfaces;

public interface ISpreadCalculator
{
    SpreadCalculationResult CalculateSpread(FuturePrice one, FuturePrice two);
}