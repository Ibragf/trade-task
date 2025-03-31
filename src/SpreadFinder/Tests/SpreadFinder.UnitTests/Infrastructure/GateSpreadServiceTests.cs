using System.Transactions;
using Application.Interfaces;
using Application.Interfaces.Dal;
using Application.Objects;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Moq;

namespace SpreadFinder.UnitTests.Infrastructure;

public class GateSpreadServiceTests
{
    private readonly Mock<IGateFuturePricesProvider> _futuresProviderMock = new(MockBehavior.Strict);
    private readonly Mock<IFuturePricesRepository> _pricesRepositoryMock = new(MockBehavior.Strict);
    private readonly Mock<ISpreadCalculator> _spreadCalculatorMock = new(MockBehavior.Strict);
    private readonly Mock<IOutboxService> _outboxService = new(MockBehavior.Strict);
    private readonly KafkaTopics _kafkaTopics = new()
    {
        SpreadChanged = "spread_changed"
    };
    
    private GateSpreadService _gateSpreadService;

    public GateSpreadServiceTests()
    {
        var optionsMock = new Mock<IOptions<KafkaTopics>>();
        optionsMock.Setup(x => x.Value).Returns(_kafkaTopics);

        _gateSpreadService = new GateSpreadService(
            _futuresProviderMock.Object,
            _pricesRepositoryMock.Object,
            _spreadCalculatorMock.Object,
            _outboxService.Object,
            optionsMock.Object);
    }

    [Fact]
    public async Task CalculateSpread_ShouldCalculateSpreadAndSendEvent()
    {
        // Arrange
        var one = "one";
        var two = "two";
        var onePrice = new FuturePrice
        {
            ExchangeName = "gate",
            Price = 0,
            UpdatedAt = default,
            Contract = one
        };

        var twoPrice = new FuturePrice
        {
            ExchangeName = "gate",
            Price = 0,
            UpdatedAt = default,
            Contract = two
        };

        var spread = new SpreadCalculationResult(one, two, -2);

        _futuresProviderMock
            .Setup(x => x.GetFuturePrice(one, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(onePrice)
            .Verifiable();
        
        _futuresProviderMock
            .Setup(x => x.GetFuturePrice(two,It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(twoPrice)
            .Verifiable();
        
        _spreadCalculatorMock
            .Setup(x => x.CalculateSpread(onePrice, twoPrice))
            .Returns(spread)
            .Verifiable();

        _pricesRepositoryMock
            .Setup(x => x.CreateTransactionScope())
            .Returns(new TransactionScope())
            .Verifiable();
        
        _pricesRepositoryMock
            .Setup(x => x.InsertPrice(onePrice, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        _pricesRepositoryMock
            .Setup(x => x.InsertPrice(twoPrice, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _outboxService
            .Setup(x => x.Queue(_kafkaTopics.SpreadChanged, It.IsAny<long>(),
                It.Is<SpreadChangedEvent>(e =>
                    e.Spread == spread.Spread &&
                    e.FirstContract == spread.FirstContract &&
                    e.SecondContract == spread.SecondContract &&
                    e.ExchangeName == "gate"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _gateSpreadService.CalculateSpread(one, two, default);
        
        //Assert
        _pricesRepositoryMock.Verify();
        _futuresProviderMock.Verify();
        _outboxService.Verify();
        _spreadCalculatorMock.Verify();
    }
}