using Core.Domain.Base;
using Core.Domain.Notificacoes;
using Domain.ValueObjects;
using Gateways;

namespace UseCases
{
    public class PedidoUseCase(IPedidoGateway pedidoGateway, INotificador notificador) : BaseUseCase(notificador), IPedidoUseCase
    {
        public async Task<bool> AlterarStatusAsync(Guid pedidoId, PedidoStatus pedidoStatus, CancellationToken cancellationToken)
        {
            var pedido = await pedidoGateway.ObterPedidoAsync(pedidoId, cancellationToken);

            if (pedido is null)
            {
                Notificar($"Pedido {pedidoId} não encontrado.");
                return false;
            }

            if (!pedido.AlterarStatus(pedidoStatus))
            {
                Notificar("Não foi possível alterar o status do pedido.");
                return false;
            }

            return await pedidoGateway.AtualizarPedidoAsync(pedido, cancellationToken);
        }
    }
}
