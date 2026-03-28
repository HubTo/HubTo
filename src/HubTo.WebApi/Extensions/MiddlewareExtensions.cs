using HubTo.WebApi.Middlewares;

namespace HubTo.WebApi.Extensions;

internal static class MiddlewareExtensions
{
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<TraceIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }
}
