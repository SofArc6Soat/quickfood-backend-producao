﻿using Core.Domain.Data;
using Domain.Tests.TestHelpers;
using Infra.Dto;
using Infra.Repositories;
using Moq;

namespace Infra.Tests.Repositories;

public class PedidoRepositoryTests
{
    private readonly Mock<IPedidoRepository> _mockPedidoRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    public PedidoRepositoryTests()
    {
        _mockPedidoRepository = new Mock<IPedidoRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPedidoRepository.Setup(repo => repo.UnitOfWork).Returns(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task FindByIdAsync_DeveRetornarPedidoPorId()
    {
        // Arrange
        var pedidoDb = PedidoFakeDataFactory.CriarPedidoDbValido();
        var cancellationToken = CancellationToken.None;

        _mockPedidoRepository.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoDb);

        // Act
        var resultado = await _mockPedidoRepository.Object.FindByIdAsync(pedidoDb.Id, cancellationToken);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pedidoDb.Id, resultado.Id);
        _mockPedidoRepository.Verify(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_DeveLancarExcecao_QuandoOcorreErro()
    {
        // Arrange
        var pedidoId = PedidoFakeDataFactory.ObterNovoGuid();
        var cancellationToken = CancellationToken.None;

        _mockPedidoRepository.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro ao encontrar pedido por ID"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _mockPedidoRepository.Object.FindByIdAsync(pedidoId, cancellationToken));
        Assert.Equal("Erro ao encontrar pedido por ID", exception.Message);
        _mockPedidoRepository.Verify(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_DeveInserirPedido()
    {
        // Arrange
        var pedidoDb = PedidoFakeDataFactory.CriarPedidoDbValido();
        var cancellationToken = CancellationToken.None;

        _mockPedidoRepository.Setup(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockPedidoRepository.Object.InsertAsync(pedidoDb, cancellationToken);

        // Assert
        _mockPedidoRepository.Verify(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_DeveLancarExcecao_QuandoOcorreErro()
    {
        // Arrange
        var pedidoDbInvalido = PedidoFakeDataFactory.CriarPedidoDbInvalido();
        var cancellationToken = CancellationToken.None;

        _mockPedidoRepository.Setup(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro ao inserir pedido"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _mockPedidoRepository.Object.InsertAsync(pedidoDbInvalido, cancellationToken));
        Assert.Equal("Erro ao inserir pedido", exception.Message);
        _mockPedidoRepository.Verify(x => x.InsertAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeveDeletarPedido()
    {
        // Arrange
        var pedidoDb = PedidoFakeDataFactory.CriarPedidoDbValido();
        var cancellationToken = CancellationToken.None;

        _mockPedidoRepository.Setup(x => x.DeleteAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockPedidoRepository.Object.DeleteAsync(pedidoDb, cancellationToken);

        // Assert
        _mockPedidoRepository.Verify(x => x.DeleteAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeveLancarExcecao_QuandoOcorreErro()
    {
        // Arrange
        var pedidoDbInexistente = PedidoFakeDataFactory.CriarPedidoDbValido();
        pedidoDbInexistente.Id = PedidoFakeDataFactory.ObterNovoGuid(); 
        var cancellationToken = CancellationToken.None;

        _mockPedidoRepository.Setup(x => x.DeleteAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro ao deletar pedido"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _mockPedidoRepository.Object.DeleteAsync(pedidoDbInexistente, cancellationToken));
        Assert.Equal("Erro ao deletar pedido", exception.Message);
        _mockPedidoRepository.Verify(x => x.DeleteAsync(It.IsAny<PedidoDb>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
