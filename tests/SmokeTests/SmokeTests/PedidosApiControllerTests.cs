using Api.Controllers;
using Controllers;
using Core.Domain.Notificacoes;
using Core.WebApi.Controller;
using Gateways.Dtos.Request;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SmokeTests.SmokeTests;

public class PedidosApiControllerTests
{
    private readonly Mock<IPedidoController> _pedidoControllerMock;
    private readonly Mock<INotificador> _notificadorMock;
    private readonly PedidosApiController _controller;

    public PedidosApiControllerTests()
    {
        _pedidoControllerMock = new Mock<IPedidoController>();
        _notificadorMock = new Mock<INotificador>();
        _controller = new PedidosApiController(_pedidoControllerMock.Object, _notificadorMock.Object);
    }

    [Fact]
    public async Task AlterarStatus_ReturnsSuccess_WhenStatusIsValid()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoStatusDto = new PedidoStatusRequestDto { Status = "EmPreparacao" };
        _pedidoControllerMock.Setup(x => x.AlterarStatusAsync(pedidoId, pedidoStatusDto, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(true);

        // Act
        var result = await _controller.AlterarStatus(pedidoId, pedidoStatusDto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BaseApiResponse>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task AlterarStatus_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoStatusDto = new PedidoStatusRequestDto { Status = "StatusInvalido" };
        _pedidoControllerMock.Setup(x => x.AlterarStatusAsync(pedidoId, pedidoStatusDto, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(false);

        // Act
        var result = await _controller.AlterarStatus(pedidoId, pedidoStatusDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<BaseApiResponse>(badRequestResult.Value);
        Assert.False(response.Success);
    }
}