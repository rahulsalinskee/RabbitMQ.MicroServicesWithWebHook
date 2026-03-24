using MassTransit;

namespace EmailNotification.WebHook.RabbitMqConsumer
{
    public static class RegisterWebHookConsumerMassTransit
    {
        public static void RegisterWebHookConsumerMassTransitExtension(this IServiceCollection services)
        {
            services.AddMassTransit(serviceAddMassTransit =>
            {
                /* Register Web Hook consumer here */
                serviceAddMassTransit.AddConsumer<WebhookConsumer>();

                serviceAddMassTransit.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(host: "rabbitmq://localhost", virtualHost: "/", configure: host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });

                    /* Queue for Web Hook Consumer */
                    configurator.ReceiveEndpoint(queueName: "email-webhook-queue", configureEndpoint: endpointConfigurator =>
                    {
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.ConfigureConsumer<WebhookConsumer>(context);
                    });
                });
            });
        }
    }
}
