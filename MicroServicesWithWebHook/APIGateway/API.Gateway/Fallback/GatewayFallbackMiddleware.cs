using Shared.Data.DTOs.ResponseDTOs;
using System.Text.Json;

namespace API.Gateway.Fallback
{
    public class GatewayFallbackMiddleware
    {
        private readonly RequestDelegate _next;

        public GatewayFallbackMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await this._next(context);

            /* Check if YARP returned a downstream failure status code */
            switch (context.Response.StatusCode)
            {
                case StatusCodes.Status502BadGateway:
                case StatusCodes.Status503ServiceUnavailable:
                case StatusCodes.Status504GatewayTimeout:
                    /* Ensure headers haven't already been sent to the client */
                    if (!context.Response.HasStarted)
                    {
                        context.Response.ContentType = "application/json";

                        var fallBackResponse = new ResponseDto()
                        {
                            IsSuccess = false,
                            Result = null,
                            Message = "The requested service is currently offline or unreachable. Please try again later.",
                            When = DateTime.UtcNow
                        };

                        var jsonResponse = JsonSerializer.Serialize(fallBackResponse, new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        });
                        await context.Response.WriteAsync(jsonResponse);
                    }
                    break;
            }
        }
    }
}
