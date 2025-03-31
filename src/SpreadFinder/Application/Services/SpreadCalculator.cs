using Application.Interfaces;
using Application.Objects;
using Domain.Entities;

namespace Application.Services;

public class SpreadCalculator : ISpreadCalculator
{
    public SpreadCalculationResult CalculateSpread(FuturePrice one, FuturePrice two)
    {
        if (one.Contract.Equals(two.Contract, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("Can not calculate spread for the same contract");
        }

        var (first, second) = GetOrderedPricesByContractName(one, two);

        return new SpreadCalculationResult(first.Contract, second.Contract, first.Price - second.Price);
    }

    private (FuturePrice first, FuturePrice second) GetOrderedPricesByContractName(FuturePrice one, FuturePrice two)
    {
        var result = string.Compare(one.Contract, two.Contract, StringComparison.InvariantCultureIgnoreCase);
        if (result > 0)
        {
            return (two, one);
        }

        return (one, two);
    }
}