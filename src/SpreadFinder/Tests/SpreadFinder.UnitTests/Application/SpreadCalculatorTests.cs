using Application.Services;
using Domain.Entities;
using FluentAssertions;

namespace SpreadFinder.UnitTests.Application;

public class SpreadCalculatorTests
{
    private SpreadCalculator _spreadCalculator;

    public SpreadCalculatorTests()
    {
        _spreadCalculator = new SpreadCalculator();
    }

    [Fact]
    public void CalculateSpread_ShouldReturnCorrectResult()
    {
        // Arrange
        var first = new FuturePrice
        {
            ExchangeName = "example",
            Price = 5,
            UpdatedAt = default,
            Contract = "aa"
        };

        var second = new FuturePrice
        {
            ExchangeName = "example",
            Price = 23,
            UpdatedAt = default,
            Contract = "bb"
        };
        
        // Act
        var result = _spreadCalculator.CalculateSpread(second, first);

        // Assert
        result.Spread.Should().Be(-18);
        result.FirstContract.Should().Be(first.Contract);
        result.SecondContract.Should().Be(second.Contract);
    }
    
    [Fact]
    public void CalculateSpread_ShouldThrowException_WhenSameContracts()
    {
        // Arrange
        var first = new FuturePrice
        {
            ExchangeName = "example",
            Price = 5,
            UpdatedAt = default,
            Contract = "f"
        };

        var second = new FuturePrice
        {
            ExchangeName = "example",
            Price = 23,
            UpdatedAt = default,
            Contract = "f"
        };
        
        // Act
        var act = () => _spreadCalculator.CalculateSpread(second, first);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}