using Domain.ValueObjects;
using Gateways.Dtos.Request;
using UseCases;

namespace Controllers
{
    public class PedidoController(IPedidoUseCase pedidoUseCase) : IPedidoController
    {
        public async Task<bool> AlterarStatusAsync(Guid pedidoId, PedidoStatusRequestDto pedidoStatusDto, CancellationToken cancellationToken)
        {
            var pedidoStatusValido = Enum.TryParse<PedidoStatus>(pedidoStatusDto.Status, out var status);

            return pedidoStatusValido && await pedidoUseCase.AlterarStatusAsync(pedidoId, status, cancellationToken);
        }
    }
}