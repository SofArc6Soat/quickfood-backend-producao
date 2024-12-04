using Core.Domain.Data;
using Core.Infra.MessageBroker;
using Infra.Dto;
using Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Worker.BackgroundServices;
using Worker.Dtos.Events;

namespace Worker.Tests.BackgroundServices;

public class PedidoRecebidoBackgroundServiceTests
{
    private readonly Mock<ISqsService<PedidoRecebidoEvent>> _sqsServiceMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<ILogger<PedidoRecebidoBackgroundService>> _loggerMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public PedidoRecebidoBackgroundServiceTests()
    {
        _sqsServiceMock = new Mock<ISqsService<PedidoRecebidoEvent>>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<PedidoRecebidoBackgroundService>>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IPedidoRepository))).Returns(_pedidoRepositoryMock.Object);
        _pedidoRepositoryMock.Setup(x => x.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessMessage_WhenMessageIsReceived()
    {
        // Arrange
        var pedidoRecebidoEvent = new PedidoRecebidoEvent
        {
            Id = Guid.NewGuid(),
            NumeroPedido = 123,
            ClienteId = Guid.NewGuid(),
            Status = "Recebido",
            ValorTotal = 100.50m,
            DataPedido = DateTime.UtcNow,
            PedidoItems = new List<PedidoItemEvent>
                {
                    new PedidoItemEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2, 50.25m)
                }
        };

        _sqsServiceMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(pedidoRecebidoEvent);
        _pedidoRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((PedidoDb)null);

        var service = new PedidoRecebidoBackgroundService(_sqsServiceMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1)); // To stop the infinite loop
        await service.StartAsync(cancellationTokenSource.Token);

        // Assert
        _pedidoRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotProcessMessage_WhenMessageIsNull()
    {
        // Arrange
        _sqsServiceMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync((PedidoRecebidoEvent)null);

        var service = new PedidoRecebidoBackgroundService(_sqsServiceMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1)); // To stop the infinite loop
        await service.StartAsync(cancellationTokenSource.Token);

        // Assert
        _pedidoRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotProcessMessage_WhenPedidoItemsIsNull()
    {
        // Arrange
        var pedidoRecebidoEvent = new PedidoRecebidoEvent
        {
            Id = Guid.NewGuid(),
            NumeroPedido = 123,
            ClienteId = Guid.NewGuid(),
            Status = "Recebido",
            ValorTotal = 100.50m,
            DataPedido = DateTime.UtcNow,
            PedidoItems = null
        };

        _sqsServiceMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(pedidoRecebidoEvent);

        var service = new PedidoRecebidoBackgroundService(_sqsServiceMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1)); // To stop the infinite loop
        await service.StartAsync(cancellationTokenSource.Token);

        // Assert
        _pedidoRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
