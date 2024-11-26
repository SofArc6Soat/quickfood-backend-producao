using Gateways.Dtos.Request;

namespace Controllers
{
    public interface IPedidoController
    {
        Task<bool> AlterarStatusAsync(Guid pedidoId, PedidoStatusRequestDto pedidoStatusDto, CancellationToken cancellationToken);
    }
}
