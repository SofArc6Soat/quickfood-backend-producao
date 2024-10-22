using Core.Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;

namespace Domain.Entities
{
    public class Pedido : Entity, IAggregateRoot
    {
        public int NumeroPedido { get; private set; }
        public Guid? ClienteId { get; private set; }
        public PedidoStatus Status { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataPedido { get; private set; }

        private readonly List<PedidoItem> _pedidoItems;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;

        public Pedido(Guid id, Guid? clienteId)
        {
            Id = id;
            ClienteId = clienteId;
            _pedidoItems = [];
            Status = PedidoStatus.Rascunho;
            DataPedido = DateTime.Now;
        }

        public Pedido(Guid id, int numeroPedido, Guid? clienteId, PedidoStatus status, decimal valorTotal, DateTime dataCricacao)
        {
            Id = id;
            NumeroPedido = numeroPedido;
            ClienteId = clienteId;
            Status = status;
            ValorTotal = valorTotal;
            DataPedido = dataCricacao;
        }

        public bool AlterarStatusParaRecebibo()
        {
            if (Status == PedidoStatus.PendentePagamento)
            {
                Status = PedidoStatus.Recebido;
                return true;
            }

            return false;
        }

        public bool AlterarStatus(PedidoStatus novoStatus)
        {
            switch (Status)
            {
                case PedidoStatus.Recebido:
                    if (novoStatus == PedidoStatus.EmPreparacao)
                    {
                        Status = novoStatus;
                        return true;
                    }

                    break;
                case PedidoStatus.EmPreparacao:
                    if (novoStatus == PedidoStatus.Pronto)
                    {
                        Status = novoStatus;
                        return true;
                    }

                    break;
                case PedidoStatus.Pronto:
                    if (novoStatus == PedidoStatus.Finalizado)
                    {
                        Status = novoStatus;
                        return true;
                    }

                    break;
            }

            return false;
        }
    }

    public class ValidarPedido : AbstractValidator<Pedido>
    {
        public ValidarPedido() => RuleFor(c => c.Id)
                .NotNull().WithMessage("O {PropertyName} não pode ser nulo.")
                .NotEmpty().WithMessage("O {PropertyName} deve ser válido.");
    }
}
