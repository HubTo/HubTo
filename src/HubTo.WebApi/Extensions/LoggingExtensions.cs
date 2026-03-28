using Serilog;
using Serilog.Events;

namespace HubTo.WebApi.Extensions;

internal static class LoggingExtensions
{
    internal static IHostBuilder AddSerilogLogging(this IHostBuilder host)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] [TraceId: {TraceId}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        return host.UseSerilog();
    }
}