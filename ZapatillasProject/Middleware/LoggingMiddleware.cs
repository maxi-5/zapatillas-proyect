using System.Diagnostics;

namespace ZapatillasProject.Middleware
{
    // Middleware propio: registra cada peticion HTTP (metodo, ruta, codigo de respuesta y tiempo)
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cronometro = Stopwatch.StartNew();

            _logger.LogInformation("--> {Metodo} {Ruta}", context.Request.Method, context.Request.Path);

            await _next(context);

            cronometro.Stop();
            // despues de _next la autenticacion JWT ya se ejecuto, asi que sabemos quien es el usuario
            var usuario = context.User?.Identity?.IsAuthenticated == true
                ? context.User.FindFirst("nombre")?.Value
                : "anonimo";
            _logger.LogInformation("<-- {Metodo} {Ruta} respondio {Codigo} en {Tiempo} ms (usuario: {Usuario})",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                cronometro.ElapsedMilliseconds,
                usuario);
        }
    }
}
