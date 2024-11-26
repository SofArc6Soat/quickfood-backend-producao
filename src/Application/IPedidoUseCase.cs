using Domain.ValueObjects;

namespace UseCases
{
    public interface IPedidoUseCase
    {
        Task<bool> AlterarStatusAsync(Guid pedidoId, PedidoStatus pedidoStatus, CancellationToken cancellationToken);
    }
}
