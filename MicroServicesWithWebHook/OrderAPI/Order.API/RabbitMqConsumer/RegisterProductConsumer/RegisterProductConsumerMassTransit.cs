using MassTransit;
using Order.API.RabbitMqConsumer.ProductConsumer;

namespace Order.API.RabbitMqConsumer.RegisterProductConsumer
{
    public static class RegisterProductConsumerMassTransit
    {
        public static void RegisterProductConsumerMassTransitExtension(this IServiceCollection services)
        {
            services.AddMassTransit(serviceAddMassTransit =>
            {
                /* Register BOTH consumers here, inside the single AddMassTransit call */
                serviceAddMassTransit.AddConsumer<ProductConsumerV1>();
                serviceAddMassTransit.AddConsumer<ProductConsumerV2>();

                serviceAddMassTransit.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(host: "localhost", virtualHost: "/", configure: host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });

                    /* Queue for V1 consumer */
                    configurator.ReceiveEndpoint(queueName: "product-sync-queue-v1", configureEndpoint: endpointConfigurator =>
                    {
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.ConfigureConsumer<ProductConsumerV1>(context);
                    });

                    /* Queue for V2 consumer */
                    configurator.ReceiveEndpoint(queueName: "product-sync-queue-v2", configureEndpoint: endpointConfigurator =>
                    {
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.ConfigureConsumer<ProductConsumerV2>(context);
                    });
                });
            });
        }
    }
}