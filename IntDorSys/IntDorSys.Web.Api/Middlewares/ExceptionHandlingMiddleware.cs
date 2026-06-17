using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace IntDorSys.Web.Api.Middlewares
{
    internal sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception processing {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandler != null)
                {
                    return;
                }

                await WriteProblemDetailsAsync(context, ex);
            }
        }

        private static async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var isDevelopment = context.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsDevelopment();

            var problem = new
            {
                type = "https://httpstatuses.io/500",
                title = "Internal Server Error",
                status = context.Response.StatusCode,
                detail = isDevelopment
                    ? $"{ex.GetType().Name}: {ex.Message}"
                    : "Internal server error",
                instance = context.Request.Path,
                traceId = context.TraceIdentifier,
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, problem);
        }
    }
}
