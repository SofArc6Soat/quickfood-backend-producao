using Core.Domain.Entities;

namespace Worker.Dtos.Events
{
    public record PedidoRecebidoEvent : Event
    {
        public int NumeroPedido { get; set; }
        public Guid? ClienteId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public DateTime DataPedido { get; set; }
        public List<PedidoItemEvent>? PedidoItems { get; set; }
    }

    public record PedidoItemEvent(Guid Id, Guid PedidoId, Guid ProdutoId, int Quantidade, decimal ValorUnitario)
    {
        public Guid Id { get; private set; } = Id;
        public Guid PedidoId { get; private set; } = PedidoId;
        public Guid ProdutoId { get; private set; } = ProdutoId;
        public int Quantidade { get; private set; } = Quantidade;
        public decimal ValorUnitario { get; private set; } = ValorUnitario;
    }
}
