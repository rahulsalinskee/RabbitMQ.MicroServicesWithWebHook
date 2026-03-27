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
                    configurator.Host(host: "localhost", virtualHost: "/", configure: host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });

                    /* Queue for Web Hook consumer: Since We have added AddConsumer for WebhookConsumer (Line 12), 
                    *  We need a receive endpoint for it
                    *  If We don't add it on the top, we don't need to add it here
                    */
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
