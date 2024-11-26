using Core.Infra.MessageBroker;
using Domain.Tests.TestHelpers;
using Domain.ValueObjects;
using Gateways.Dtos.Events;
using Infra.Dto;
using Infra.Repositories;
using Moq;

namespace Gateways.Tests;

public class PedidoGatewayTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<ISqsService<PedidoStatusAlteradoEvent>> _sqsServiceMock;
    private readonly PedidoGateway _pedidoGateway;

    public PedidoGatewayTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _sqsServiceMock = new Mock<ISqsService<PedidoStatusAlteradoEvent>>();
        _pedidoGateway = new PedidoGateway(_pedidoRepositoryMock.Object, _sqsServiceMock.Object);
    }

    [Fact]
    public async Task ObterPedidoAsync_DeveRetornarPedido_QuandoPedidoExistir()
    {
        // Arrange
        var pedidoDb = PedidoFakeDataFactory.CriarPedidoDbValido();

        _pedidoRepositoryMock.Setup(x => x.FindByIdAsync(pedidoDb.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoDb);

        // Act
        var result = await _pedidoGateway.ObterPedidoAsync(pedidoDb.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pedidoDb.Id, result.Id);
        Assert.Equal(pedidoDb.NumeroPedido, result.NumeroPedido);
        Assert.Equal(pedidoDb.ClienteId, result.ClienteId);
        Assert.Equal(PedidoStatus.Rascunho, result.Status);
        Assert.Equal(pedidoDb.ValorTotal, result.ValorTotal);
        Assert.Equal(pedidoDb.DataPedido, result.DataPedido);
    }

    [Fact]
    public async Task ObterPedidoAsync_DeveRetornarNull_QuandoPedidoNaoExistir()
    {
        // Arrange
        var pedidoId = PedidoFakeDataFactory.ObterNovoGuid();

        _pedidoRepositoryMock.Setup(x => x.FindByIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PedidoDb)null);

        // Act
        var result = await _pedidoGateway.ObterPedidoAsync(pedidoId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AtualizarPedidoAsync_DeveRetornarTrue_QuandoAtualizacaoForBemSucedida()
    {
        // Arrange
        var pedido = PedidoFakeDataFactory.CriarPedidoValido();
        var pedidoDb = PedidoFakeDataFactory.CriarPedidoDbValido();

        _pedidoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _pedidoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _sqsServiceMock.Setup(x => x.SendMessageAsync(It.IsAny<PedidoStatusAlteradoEvent>())).ReturnsAsync(true);

        // Act
        var result = await _pedidoGateway.AtualizarPedidoAsync(pedido, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AtualizarPedidoAsync_DeveRetornarFalse_QuandoCommitFalhar()
    {
        // Arrange
        var pedido = PedidoFakeDataFactory.CriarPedidoValido();
        var pedidoDb = PedidoFakeDataFactory.CriarPedidoDbValido();

        _pedidoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _pedidoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _pedidoGateway.AtualizarPedidoAsync(pedido, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
