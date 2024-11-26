using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests.Entities
{
    public class PedidoTests
    {
        [Fact]
        public void AlterarStatusParaRecebido_DeveAtualizarStatusParaRecebido()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), 1, Guid.NewGuid(), PedidoStatus.PendentePagamento, 0, DateTime.Now);

            // Act
            var resultado = pedido.AlterarStatusParaRecebibo();

            // Assert
            resultado.Should().BeTrue();
            pedido.Status.Should().Be(PedidoStatus.Recebido);
        }

        [Fact]
        public void AlterarStatusParaRecebido_QuandoStatusNaoForPendentePagamento_DeveRetornarFalse()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), 1, Guid.NewGuid(), PedidoStatus.Rascunho, 0, DateTime.Now);

            // Act
            var resultado = pedido.AlterarStatusParaRecebibo();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void AlterarStatus_DeveAtualizarStatusCorretamente()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), 1, Guid.NewGuid(), PedidoStatus.Recebido, 0, DateTime.Now);

            // Act
            var resultado = pedido.AlterarStatus(PedidoStatus.EmPreparacao);

            // Assert
            resultado.Should().BeTrue();
            pedido.Status.Should().Be(PedidoStatus.EmPreparacao);
        }

        [Fact]
        public void AlterarStatus_QuandoTransicaoInvalida_DeveRetornarFalse()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), 1, Guid.NewGuid(), PedidoStatus.Rascunho, 0, DateTime.Now);

            // Act
            var resultado = pedido.AlterarStatus(PedidoStatus.Pronto);

            // Assert
            resultado.Should().BeFalse();
            pedido.Status.Should().Be(PedidoStatus.Rascunho);
        }
    }
}