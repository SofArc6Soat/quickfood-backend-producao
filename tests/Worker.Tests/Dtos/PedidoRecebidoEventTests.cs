using Worker.Dtos.Events;

namespace Worker.Tests.Dtos;

public class PedidoRecebidoEventTests
{
    [Fact]
    public void PedidoRecebidoEvent_ShouldInitializeCorrectly()
    {
        // Arrange
        var numeroPedido = 123;
        var clienteId = Guid.NewGuid();
        var status = "Recebido";
        var valorTotal = 100.50m;
        var dataPedido = DateTime.UtcNow;
        var pedidoItems = new List<PedidoItemEvent>
            {
                new PedidoItemEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2, 50.25m)
            };

        // Act
        var pedidoRecebidoEvent = new PedidoRecebidoEvent
        {
            NumeroPedido = numeroPedido,
            ClienteId = clienteId,
            Status = status,
            ValorTotal = valorTotal,
            DataPedido = dataPedido,
            PedidoItems = pedidoItems
        };

        // Assert
        Assert.Equal(numeroPedido, pedidoRecebidoEvent.NumeroPedido);
        Assert.Equal(clienteId, pedidoRecebidoEvent.ClienteId);
        Assert.Equal(status, pedidoRecebidoEvent.Status);
        Assert.Equal(valorTotal, pedidoRecebidoEvent.ValorTotal);
        Assert.Equal(dataPedido, pedidoRecebidoEvent.DataPedido);
        Assert.Equal(pedidoItems, pedidoRecebidoEvent.PedidoItems);
    }

    [Fact]
    public void PedidoItemEvent_ShouldInitializeCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var quantidade = 3;
        var valorUnitario = 30.00m;

        // Act
        var pedidoItemEvent = new PedidoItemEvent(id, pedidoId, produtoId, quantidade, valorUnitario);

        // Assert
        Assert.Equal(id, pedidoItemEvent.Id);
        Assert.Equal(pedidoId, pedidoItemEvent.PedidoId);
        Assert.Equal(produtoId, pedidoItemEvent.ProdutoId);
        Assert.Equal(quantidade, pedidoItemEvent.Quantidade);
        Assert.Equal(valorUnitario, pedidoItemEvent.ValorUnitario);
    }
}
