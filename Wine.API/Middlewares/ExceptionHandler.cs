using FluentValidation;
using Wine.Domain.Exceptions;
using System.Text.Json;

namespace Wine.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, Task>>
            {
                { typeof(ValidationException), HandleValidationExceptionAsync },
                { typeof(EntityNotFoundException), HandleEntityNotFoundExceptionAsync },
                { typeof(Exception), HandleUnexpectedExceptionAsync }
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (_exceptionHandlers.TryGetValue(ex.GetType(), out var handler))
                {
                    await handler(context, ex);
                }
                else
                {
                    await HandleUnexpectedExceptionAsync(context, ex);
                }
            }
        }

        private Task HandleValidationExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is not ValidationException validationException)
                return HandleUnexpectedExceptionAsync(context, exception);
            
            _logger.LogWarning($"Validation error: {validationException.Message}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var validationErrorDetails = new
            {
                message = "Validation error occurred.",
                errors = validationException.Errors.Select(x => x.ErrorMessage)
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(validationErrorDetails));

            // If it's not a ValidationException, just let the unexpected exception handler handle it
        }

        private Task HandleEntityNotFoundExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is not EntityNotFoundException entityNotFoundException)
                return HandleUnexpectedExceptionAsync(context, exception);
            
            _logger.LogWarning($"Entity not found: {entityNotFoundException.Message}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            var notFoundErrorDetails = new
            {
                message = "The requested entity was not found.",
                errors = new[] {entityNotFoundException.Message}
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(notFoundErrorDetails));

        }

        private Task HandleUnexpectedExceptionAsync(HttpContext context, Exception exception)
        {
            // Log unexpected errors
            _logger.LogError($"Unexpected error: {exception.Message}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                message = "An error occurred while processing your request.",
                errors = new[] {exception.Message}
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
