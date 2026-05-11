using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "There is an error: {Message}", exception.Message);

            (string Detail, string Title, int SatusCode) details = exception switch
            {
                NotFoundException notFoundException => (notFoundException.Message, "Not Found", StatusCodes.Status404NotFound),
                BadRequestException badRequestException => (badRequestException.Message, "Bad Request", StatusCodes.Status400BadRequest),
                InternalServerException internalServerException => (internalServerException.Message, "Internal Server Error", StatusCodes.Status500InternalServerError),
                ValidationException validationException => (validationException.Message, "Validation Error", StatusCodes.Status400BadRequest),
                _ => ("An unexpected error occurred.", "Internal Server Error", StatusCodes.Status500InternalServerError)
            };

            var problemDetails = new ProblemDetails
            {
                Status = details.SatusCode,
                Title = details.Title,
                Detail = details.Detail,
            };
            problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

            if (exception is ValidationException validationExce)
            {
                problemDetails.Extensions.Add("errors", validationExce.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}