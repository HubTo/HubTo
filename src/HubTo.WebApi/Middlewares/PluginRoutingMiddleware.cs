using HubTo.Abstraction.Models.Transport;
using HubTo.Core.Application.Contracts.Plugins;

namespace HubTo.WebApi.Middlewares;

public sealed class PluginRoutingMiddleware
{
    private readonly RequestDelegate _next;

    public PluginRoutingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IPluginRegistry registry)
    {
        if (registry.IsReloading)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Service is reloading plugins");
            return;
        }

        var path = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;

        var plugin = registry.GetPlugin(path, method);

        if (plugin is null)
        {
            await _next(context);
            return;
        }

        var request = new PluginRequest
        {
            Method = method,
            Path = path,
            NamespaceId = ExtractNamespace(path),
            Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            Query = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
            Body = context.Request.Body
        };

        var response = await plugin.ProcessRequestAsync(request, context.RequestAborted);

        await WriteResponse(context, response);
    }

    private static async Task WriteResponse(HttpContext context, PluginResponse response)
    {
        context.Response.StatusCode = response.StatusCode;

        if (!string.IsNullOrWhiteSpace(response.ContentType))
            context.Response.ContentType = response.ContentType;

        foreach (var header in response.Headers)
            context.Response.Headers[header.Key] = header.Value;

        if (response.IsStream && response.StreamContent != null)
        {
            await response.StreamContent.CopyToAsync(context.Response.Body);
        }
        else if (response.Content != null)
        {
            await context.Response.WriteAsJsonAsync(response.Content);
        }
    }

    private static string ExtractNamespace(string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
            return parts[1];

        return string.Empty;
    }
}
