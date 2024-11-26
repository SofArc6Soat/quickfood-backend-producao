using Core.Infra.MessageBroker;
using Domain.Entities;
using Domain.ValueObjects;
using Gateways.Dtos.Events;
using Infra.Dto;
using Infra.Repositories;

namespace Gateways
{
    public class PedidoGateway(IPedidoRepository pedidoRepository, ISqsService<PedidoStatusAlteradoEvent> sqs) : IPedidoGateway
    {
        public async Task<bool> AtualizarPedidoAsync(Pedido pedido, CancellationToken cancellationToken)
        {
            var pedidoDto = new PedidoDb
            {
                Id = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                ClienteId = pedido.ClienteId,
                Status = pedido.Status.ToString(),
                ValorTotal = pedido.ValorTotal,
                DataPedido = pedido.DataPedido
            };

            await pedidoRepository.UpdateAsync(pedidoDto, cancellationToken);

            return await pedidoRepository.UnitOfWork.CommitAsync(cancellationToken) && await sqs.SendMessageAsync(GerarPedidoStatusAlteradoEvent(pedidoDto));
        }

        public async Task<Pedido?> ObterPedidoAsync(Guid id, CancellationToken cancellationToken)
        {
            var pedidoDto = await pedidoRepository.FindByIdAsync(id, cancellationToken);

            if (pedidoDto is null)
            {
                return null;
            }

            _ = Enum.TryParse(pedidoDto.Status, out PedidoStatus status);
            return new Pedido(pedidoDto.Id, pedidoDto.NumeroPedido, pedidoDto.ClienteId, status, pedidoDto.ValorTotal, pedidoDto.DataPedido);
        }

        private static PedidoStatusAlteradoEvent GerarPedidoStatusAlteradoEvent(PedidoDb pedidoDb) => new()
        {
            Id = pedidoDb.Id,
            Status = pedidoDb.Status
        };
    }
}
