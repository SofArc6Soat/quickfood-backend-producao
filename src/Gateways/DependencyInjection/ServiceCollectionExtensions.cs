using Amazon.SQS;
using Core.Infra.MessageBroker;
using Core.Infra.MessageBroker.DependencyInjection;
using Gateways.Dtos.Events;
using Infra.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Gateways.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddGatewayDependencyServices(this IServiceCollection services, string connectionString, Queues queues)
        {
            services.AddScoped<IPedidoGateway, PedidoGateway>();

            services.AddInfraDependencyServices(connectionString);

            // AWS SQS
            services.AddAwsSqsMessageBroker();

            services.AddSingleton<ISqsService<PedidoStatusAlteradoEvent>>(provider => new SqsService<PedidoStatusAlteradoEvent>(provider.GetRequiredService<IAmazonSQS>(), queues.QueuePedidoStatusAlteradoEvent));
        }
    }

    [ExcludeFromCodeCoverage]
    public record Queues
    {
        public string QueuePedidoStatusAlteradoEvent { get; set; } = string.Empty;
    }
}