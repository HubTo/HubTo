namespace HubTo.Core.Application.Common.Results;

public record PluginHealthResult
(
    string Name, 
    bool IsHealthy, 
    string Details
);
