using MassTransit;
using Shared.Data.DTOs.EmailDTOs;

namespace EmailNotification.WebHook.RabbitMqConsumer
{
    public class WebhookConsumer : IConsumer<EmailDto>
    {
        private readonly HttpClient _httpClient;

        private const string REQUEST_URI = "https://localhost:7298/email-webhook";

        public WebhookConsumer(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task Consume(ConsumeContext<EmailDto> context)
        {
            var result = await this._httpClient.PostAsJsonAsync(requestUri: REQUEST_URI, value: new EmailDto()
            {
                Title = context.Message.Title,
                Content = context.Message.Content,
            });

            result.EnsureSuccessStatusCode();
        }
    }
}
