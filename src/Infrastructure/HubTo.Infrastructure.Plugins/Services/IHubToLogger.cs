using HubTo.Abstraction.Logging;
using Microsoft.Extensions.Logging;

namespace HubTo.Infrastructure.Plugins.Services;

internal sealed class HubToLoggerAdapter : IHubToLogger
{
    private readonly ILogger _logger;

    public HubToLoggerAdapter(ILogger logger) => _logger = logger;

    public void LogInformation(string message) => _logger.LogInformation(message);
    public void LogWarning(string message) => _logger.LogWarning(message);
    public void LogDebug(string message) => _logger.LogDebug(message);
    public void LogError(string message, Exception? exception = null)
        => _logger.LogError(exception, message);
}
