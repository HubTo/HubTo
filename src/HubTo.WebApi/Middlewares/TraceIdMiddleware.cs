using System.Diagnostics;

namespace HubTo.WebApi.Middlewares;

public sealed class TraceIdMiddleware
{
    private const string TraceHeaderName = "X-Trace-Id";
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.TraceIdentifier = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[TraceHeaderName] = context.TraceIdentifier;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
