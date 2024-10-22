using Domain.Entities;

namespace Gateways
{
    public interface IPedidoGateway
    {
        Task<bool> AtualizarPedidoAsync(Pedido pedido, CancellationToken cancellationToken);
        Task<Pedido?> ObterPedidoAsync(Guid id, CancellationToken cancellationToken);
    }
}
