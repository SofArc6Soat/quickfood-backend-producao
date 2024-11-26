using Core.Infra.MessageBroker;
using Infra.Dto;
using Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker.Dtos.Events;

namespace Worker.BackgroundServices
{
    public class PedidoRecebidoBackgroundService(ISqsService<PedidoRecebidoEvent> sqsClient, IServiceScopeFactory serviceScopeFactory, ILogger<PedidoRecebidoBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMessageAsync(await sqsClient.ReceiveMessagesAsync(stoppingToken), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while processing messages.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ProcessMessageAsync(PedidoRecebidoEvent? message, CancellationToken cancellationToken)
        {
            if (message is not null && message.PedidoItems is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();

                var pedidoExistente = await pedidoRepository.FindByIdAsync(message.Id, cancellationToken);

                if (pedidoExistente is null)
                {
                    var pedidoDto = new PedidoDb
                    {
                        Id = message.Id,
                        NumeroPedido = message.NumeroPedido,
                        ClienteId = message.ClienteId,
                        Status = message.Status,
                        ValorTotal = message.ValorTotal,
                        DataPedido = message.DataPedido
                    };

                    foreach (var item in message.PedidoItems)
                    {
                        pedidoDto.Itens.Add(new PedidoItemDb
                        {
                            PedidoId = pedidoDto.Id,
                            ProdutoId = item.ProdutoId,
                            Quantidade = item.Quantidade,
                            ValorUnitario = item.ValorUnitario
                        });
                    }

                    await pedidoRepository.InsertAsync(pedidoDto, cancellationToken);

                    await pedidoRepository.UnitOfWork.CommitAsync(cancellationToken);
                }
            }
        }
    }
}