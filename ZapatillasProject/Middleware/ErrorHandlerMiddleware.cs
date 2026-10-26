using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ZapatillasProject.Middleware
{
    // Middleware propio: captura cualquier excepcion no controlada y responde
    // un error estructurado (ProblemDetails) en vez de caerse o mostrar el stack trace
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado en {Ruta}", context.Request.Path);

                var (codigo, titulo) = ex switch
                {
                    DbUpdateException => (StatusCodes.Status409Conflict, "Conflicto al guardar en la base de datos"),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                    ArgumentException => (StatusCodes.Status400BadRequest, "Solicitud invalida"),
                    _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
                };

                var problema = new ProblemDetails
                {
                    Status = codigo,
                    Title = titulo,
                    Detail = codigo == StatusCodes.Status500InternalServerError
                        ? "Ocurrio un error inesperado, intente nuevamente"
                        : ex.Message,
                    Instance = context.Request.Path
                };
                problema.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = codigo;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problema));
            }
        }
    }
}
