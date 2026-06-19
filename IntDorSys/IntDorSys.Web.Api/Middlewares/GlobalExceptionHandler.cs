using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace IntDorSys.Web.Api.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
            context.Request.Method, context.Request.Path);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        var problem = new
        {
            type = "https://httpstatuses.io/500",
            title = "Internal Server Error",
            status = context.Response.StatusCode,
            detail = env.IsDevelopment()
                ? $"{exception.GetType().Name}: {exception.Message}"
                : "Internal server error",
            instance = context.Request.Path,
            traceId = Activity.Current?.Id ?? context.TraceIdentifier,
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, cancellationToken);

        return true;
    }
}
