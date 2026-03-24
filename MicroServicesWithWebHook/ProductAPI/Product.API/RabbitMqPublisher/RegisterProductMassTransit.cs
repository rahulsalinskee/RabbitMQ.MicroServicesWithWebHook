using MassTransit;

namespace Product.API.RabbitMqPublisher
{
    public static class RegisterProductMassTransit
    {
        public static void RegisterProductMassTransitExtension(this IServiceCollection services)
        {
            services.AddMassTransit(serviceAddMassTransit =>
            {
                serviceAddMassTransit.UsingRabbitMq((context, configurator) =>
                {
                    /* Default RabbitMQ configuration (localhost:5672, guest/guest) */
                    /* Update these credentials if our RabbitMQ server requires different ones or is hosted elsewhere. */
                    configurator.Host(host: "localhost", virtualHost: "/", configure: host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });
                });
            });
        }
    }
}