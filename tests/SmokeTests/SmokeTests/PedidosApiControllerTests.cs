using Gateways.Dtos.Request;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace SmokeTests.SmokeTests;

public class PedidosApiControllerTests
{
    private readonly HttpClient _client;
    private readonly Mock<HttpMessageHandler> _handlerMock;

    public PedidosApiControllerTests()
    {
        _handlerMock = MockHttpMessageHandler.SetupMessageHandlerMock(HttpStatusCode.OK, "{\"Success\":true}");
        _client = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    [Fact]
    public async Task AlterarStatus_ReturnsSuccess_WhenStatusIsValid()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoStatusDto = new PedidoStatusRequestDto { Status = "EmPreparacao" };
        _handlerMock.SetupRequest(HttpMethod.Patch, $"http://localhost/producao/status/{pedidoId}", HttpStatusCode.OK, "{\"Success\":true}");

        // Act
        var response = await _client.PatchAsJsonAsync($"/producao/status/{pedidoId}", pedidoStatusDto);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task AlterarStatus_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoStatusDto = new PedidoStatusRequestDto { Status = "StatusInvalido" };
        _handlerMock.SetupRequest(HttpMethod.Patch, $"http://localhost/producao/status/{pedidoId}", HttpStatusCode.BadRequest, "{\"Success\":false, \"Errors\":[\"Status inválido.\"]}");

        // Act
        var response = await _client.PatchAsJsonAsync($"/producao/status/{pedidoId}", pedidoStatusDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Status inválido.", content);
    }

    [Fact]
    public async Task AlterarStatus_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoStatusDto = new PedidoStatusRequestDto { Status = "EmPreparacao" };
        _handlerMock.SetupRequest(HttpMethod.Patch, $"http://localhost/producao/status/{pedidoId}", HttpStatusCode.InternalServerError, "{\"Success\":false, \"Errors\":[\"Ocorreu um erro ao processar a solicitação.\"]}");

        // Act
        var response = await _client.PatchAsJsonAsync($"/producao/status/{pedidoId}", pedidoStatusDto);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Ocorreu um erro ao processar a solicitação.", content);
    }
}