using Estoque.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.ErrorHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
                ConcurrencyConflictException => (StatusCodes.Status409Conflict, "Conflito de concorrência"),
                BusinessRuleException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Erro não tratado na Estoque.API");

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro inesperado."
                    : exception.Message
            }, cancellationToken);

            return true;
        }
    }
}
