using Shared.Data.Models.ErrorModel;

namespace Product.API.Exception
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly RequestDelegate _nextCall;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, RequestDelegate nextCall)
        {
            this._logger = logger;
            this._nextCall = nextCall;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _nextCall(httpContext);
            }
            catch (System.Exception exception)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = exception.Message,
                };

                _logger.LogError(exception, "Application Error occurred on Date & Time: {ErrorDate}", applicationError.When);
                _logger.LogError(exception, "Application Error occurred with Message: {ErrorMessage}", applicationError.Message);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsJsonAsync(applicationError);
            }

        }
    }
}
