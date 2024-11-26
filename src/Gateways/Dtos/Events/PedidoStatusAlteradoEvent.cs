using Core.Domain.Entities;

namespace Gateways.Dtos.Events
{
    public record PedidoStatusAlteradoEvent : Event
    {
        public string Status { get; set; } = string.Empty;
    }
}
