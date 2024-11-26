using Amazon.SQS;
using Core.Infra.MessageBroker;
using Core.Infra.MessageBroker.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Worker.BackgroundServices;
using Worker.Dtos.Events;

namespace Worker.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddWorkerDependencyServices(this IServiceCollection services, WorkerQueues queues)
        {
            // AWS SQS
            services.AddAwsSqsMessageBroker();

            services.AddSingleton<ISqsService<PedidoRecebidoEvent>>(provider => new SqsService<PedidoRecebidoEvent>(provider.GetRequiredService<IAmazonSQS>(), queues.QueuePedidoRecebidoEvent));

            services.AddHostedService<PedidoRecebidoBackgroundService>();
        }
    }

    [ExcludeFromCodeCoverage]
    public record WorkerQueues
    {
        public string QueuePedidoRecebidoEvent { get; set; } = string.Empty;
    }
}