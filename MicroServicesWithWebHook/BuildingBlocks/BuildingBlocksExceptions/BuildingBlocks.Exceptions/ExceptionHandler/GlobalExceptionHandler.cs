using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Data.Models.ErrorModel;

namespace BuildingBlocks.Exceptions.ExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this._logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ApplicationError applicationError = new ()
            {
                Message = exception.Message,
                When = DateTime.UtcNow
            };

            this._logger.LogError(exception: exception, message: "Global Exception caught: {ErrorMessage}", applicationError.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(applicationError, cancellationToken);

            return true;
        }
    }
}
